using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDTSandwich
{
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //
  // Commands
  // Handles all communication with the Arduino
  //
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  class Communication
  {
    // Default settings for sending commands
    private const int DEFAULT_COMMAND_MAXATTEMPTS = 1;    // The default max number of times to attempt a command.
    private const int DEFAULT_COMMAND_RESPONSETIMEOUT = 1500; // The default duration (ms) for a sent message to be considered as unreceived.

    // Codes sent during communication with Arduino
    public const string SERIAL_CMD_START = "^";
    public const string SERIAL_CMD_CONNECTION = "w";
    public const string SERIAL_CMD_DAQ_START_HEATER = "d";
    public const string SERIAL_CMD_DAQ_START_SAMPLE = "b";
    public const string SERIAL_CMD_DAQ_STOP = "s";
    public const string SERIAL_CMD_HEAT_START = "h";
    public const string SERIAL_CMD_HEAT_STOP = "c";
    public const string SERIAL_CMD_BLINK = "l";
    public const string SERIAL_CMD_SHUTDOWN = "x";
    public const string SERIAL_CMD_SEPARATOR = "|";
    public const string SERIAL_CMD_END = "@";
    public const string SERIAL_CMD_EOL = "\n";

    // Codes received during communication with Arduino
    public const string SERIAL_REPLY_START = "^";
    public const string SERIAL_REPLY_CONNECTION = "w";
    public const string SERIAL_SEND_TEMP_HEATER = "t";
    public const string SERIAL_SEND_TEMP_HEATERSAMPLE = "b";
    public const string SERIAL_SEND_TEMP_ERROR = "e";
    public const string SERIAL_SEND_TEMP_HEATINGDONE = "c";
    public const string SERIAL_REPLY_ERROR = "e";
    public const char SERIAL_REPLY_SEPARATOR = '|';
    public const string SERIAL_REPLY_END = "@";
    public const string SERIAL_REPLY_EOL = "\n";
    public const string SERIAL_REPLY_CORRUPTCMD = "x";
    public const string SERIAL_REPLY_CORRUPTCMD_START = "s";  // The command string does not have a start flag
    public const string SERIAL_REPLY_CORRUPTCMD_END = "e";  // The command string does not have an end flag
    public const string SERIAL_REPLY_CORRUPTCMD_UNKNOWNCMD = "c";  // Unknown command received
    public const string SERIAL_REPLY_CORRUPTCMD_PARAM_LESS = "l";  // There are fewer params in a command line than expected
    public const string SERIAL_REPLY_CORRUPTCMD_PARAM_MORE = "m";  // There are more params in a command line than expected
    public const string SERIAL_REPLY_CORRUPTCMD_PARAM_NONE = "n";  // There are no params in a command line, even though some are expected
    public const string SERIAL_REPLY_CMDRESPONSE = "r";  // Used to indicate execution status of a received command
    public const string SERIAL_REPLY_CMDRESPONSE_SUCC = "y";  // Success
    public const string SERIAL_REPLY_CMDRESPONSE_FAIL = "n";  // Failed

    // State of the command response
    public const int COMMAND_STATE_SUCCESS = 0;
    public const int COMMAND_STATE_FAIL = 1;
    public const int COMMAND_STATE_TIMEOUT = 2;
    public const int COMMAND_STATE_DISCONNECTED = 3;
    public const int COMMAND_STATE_ERROR = 4;

    // References to external classes
    private Sandwich parentSandwich_;
    private ErrorLogger errorLogger_;

    // General vars
    private readonly int maxCommandID_ = 100;   // The upper limit of command ids before rollover occurs
    private int curCommandID_ = 0;        // Tracks the most recently assigned command ID
    private bool commandOutActive_ = false;
    private bool commandOutSending_ = false;
    private bool commandInActive_ = false;
    private List<OutgoingMessage> commandsWaiting_;        // Stores the commands which are waiting for responses
    private ConcurrentQueue<IncomingMessage> commandsIn_;  // Queue for status of commands which we are awaiting responses for
    private ConcurrentQueue<OutgoingMessage> commandsOut_; // Queue for outgoing commands

    // Serial port
    private SerialPort port_;
    private bool connected_ = false;
    public static int serialBaudRate = 9600;
    public static Parity serialParity = Parity.None;
    public static int serialDataBits = 8;
    public static StopBits serialStopBits = StopBits.One;
    public static Handshake serialHandshake = Handshake.None;
    public static Encoding serialEncoding = Encoding.ASCII;
    private int serialReadTimeout_ = 2000; // 2000 ms before timeout.
    private SerialDataReceivedEventHandler serialDataReceivedHandler_;


    /*********************************
     *      CALLBACK FUNCTIONS
     * ******************************/
    public delegate void HandleNewReadingsCallback(string heater1T, string heater2T, bool sampleTAvailable, string sampleT = "");
    public delegate void UpdateControlsCallback();

    private HandleNewReadingsCallback HandleNewReadings;
    private UpdateControlsCallback HandleStartDAQSuccess;
    private UpdateControlsCallback HandleStartDAQFail;
    private UpdateControlsCallback HandleStopDAQSuccess;
    private UpdateControlsCallback HandleStopDAQFail;
    private UpdateControlsCallback HandleStartHeatSuccess;
    private UpdateControlsCallback HandleStartHeatFail;
    private UpdateControlsCallback HandleStopHeatSuccess;
    private UpdateControlsCallback HandleStopHeatFail;
    private UpdateControlsCallback HandleBlinkLEDSuccess;
    private UpdateControlsCallback HandleBlinkLEDFail;
    private UpdateControlsCallback HandleHeatingComplete;


    public Communication(Sandwich parentSandwich, ErrorLogger errorLogger,
                    HandleNewReadingsCallback HandleNewReadingsFunc,
                    UpdateControlsCallback HandleStartDAQSuccessFunc,
                    UpdateControlsCallback HandleStartDAQFailFunc,
                    UpdateControlsCallback HandleStopDAQSuccessFunc,
                    UpdateControlsCallback HandleStopDAQFailFunc,
                    UpdateControlsCallback HandleStartHeatSuccessFunc,
                    UpdateControlsCallback HandleStartHeatFailFunc,
                    UpdateControlsCallback HandleStopHeatSuccessFunc,
                    UpdateControlsCallback HandleStopHeatFailFunc,
                    UpdateControlsCallback HandleBlinkLEDSuccessFunc,
                    UpdateControlsCallback HandleBlinkLEDFailFunc,
                    UpdateControlsCallback HandleHeatingCompleteFunc)
    {
      // Assign vars
      parentSandwich_ = parentSandwich;
      errorLogger_ = errorLogger;

      // Initialize stuff
      commandsWaiting_ = new List<OutgoingMessage>();
      commandsOut_ = new ConcurrentQueue<OutgoingMessage>();
      commandsIn_ = new ConcurrentQueue<IncomingMessage>();

      // Assign the callback functions
      HandleNewReadings = new HandleNewReadingsCallback(HandleNewReadingsFunc);
      HandleStartDAQSuccess = new UpdateControlsCallback(HandleStartDAQSuccessFunc);
      HandleStartDAQFail = new UpdateControlsCallback(HandleStartDAQFailFunc);
      HandleStopDAQSuccess = new UpdateControlsCallback(HandleStopDAQSuccessFunc);
      HandleStopDAQFail = new UpdateControlsCallback(HandleStopDAQFailFunc);
      HandleStartHeatSuccess = new UpdateControlsCallback(HandleStartHeatSuccessFunc);
      HandleStartHeatFail = new UpdateControlsCallback(HandleStartHeatFailFunc);
      HandleStopHeatSuccess = new UpdateControlsCallback(HandleStopHeatSuccessFunc);
      HandleStopHeatFail = new UpdateControlsCallback(HandleStopHeatFailFunc);
      HandleBlinkLEDSuccess = new UpdateControlsCallback(HandleBlinkLEDSuccessFunc);
      HandleBlinkLEDFail = new UpdateControlsCallback(HandleBlinkLEDFailFunc);
      HandleHeatingComplete = new UpdateControlsCallback(HandleHeatingCompleteFunc);
    }

    // Handles the event when the timer has timed out.
    // Renew the timed out command and place it on the outgoing queue, keeping
    // the total number of attempts intact.
    // Note that the renewed command will have a different command ID.
    private void handleTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      OutgoingMessage command = (sender as MessageTimer).command;

      try
      {
        command.attempts++;

        // Check if we are still waiting for a response for the command, and haven't exceeded max attempts
        // The first check is necessary because we might receive a response before a timeout condition,
        // therefore we should not send again
        if (command.attempts < command.maxAttempts && commandsWaiting_.Find(x => (x.commandID == command.commandID)) != null)
        {// Resend the command since timeout has occured but we are within max attempts
         // Place on command response queue indicating this command has timed-out, and should be resent
          commandsIn_.Enqueue(new IncomingMessage(command.commandID, IncomingMessage.COMMAND_STATUS_TIMEOUT));
        }
        else
        {
          // Place on command response queue indicating this command has reached maximum retry attempts
          commandsIn_.Enqueue(new IncomingMessage(command.commandID, IncomingMessage.COMMAND_STATUS_MAXATTEMPT));
        }
      }
      catch (Exception err)
      {
        errorLogger_.logUnknownError(err);

        // The time out condition is the safety net for commands that are not received. Therefore, it
        // is essential that it works. If it doesn't, then put on queue that an error was encountered.
        // If that still doesn't work, at least we have something from the errorLog to work with.
        try
        {
          commandsIn_.Enqueue(new IncomingMessage(command.commandID, IncomingMessage.COMMAND_STATUS_ERROR));
        }
        catch (Exception err2)
        {
          errorLogger_.logUnknownError(err2);
        }
      }
    }

    /*************************************
     * Handles the events raised by SerialPort
     * upon receiving data
     ************************************/
    public void handleSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      try
      {
        string readBuffer = port_.ReadTo(SERIAL_REPLY_EOL);

        // Check if the start and end of the response have the correct flags
        if (readBuffer.Substring(0, 1) == SERIAL_REPLY_START && readBuffer.Substring(readBuffer.Length - 1, 1) == SERIAL_REPLY_END)
        {
          int startPos = readBuffer.IndexOf(SERIAL_REPLY_START);
          int endPos = readBuffer.LastIndexOf(SERIAL_REPLY_END);

          // System.Diagnostics.Debug.WriteLine("Received reply: " + readBuffer);

          string[] bufferFragments = extractReplyFragments(readBuffer, startPos, endPos);

          // The first fragment is the command type
          switch (bufferFragments[0])
          {
            case SERIAL_REPLY_CMDRESPONSE:
              {
                /**********************************
                *     RESPONSE TO COMMAND        *
                * *******************************/
                /* Arduino's response to a received command
                * Format:
                * ^r|[commandID]|[executedCommandType]|[succ/fail]@
                * where    ^               is SERIAL_REPLY_START
                *          r               is SERIAL_REPLY_CMDRESPONSE
                *          |               is SERIAL_REPLY_SEPARATOR
                *          [commandID]     is the ID for the command
                *          [executedCommandType]   is the type of command sent by C# program (e.g. SERIAL_CMD_STATE_ACQUISITION)
                *          [succ/fail]     is SERIAL_REPLY_CMDRESPONSE_SUCCESS or SERIAL_REPLY_CMDRESPONSE_FAIL
                *          @               is SERIAL_REPLY_END
                */

                // System.Diagnostics.Debug.WriteLine("Received command confirmation: " + readBuffer);

                // Sanity check for number of fragments
                if (bufferFragments.Length == 4)
                {
                  int commandID = Convert.ToInt32(bufferFragments[1]);
                  string executedCommandType = bufferFragments[2];
                  bool executionSuccess = false;

                  // Get the status of command execution, and also check if this is corrupted.
                  if (bufferFragments[3].Equals(SERIAL_REPLY_CMDRESPONSE_SUCC))
                  {
                    executionSuccess = true;
                  }
                  else if (bufferFragments[3].Equals(SERIAL_REPLY_CMDRESPONSE_FAIL))
                  {
                    executionSuccess = false;
                  }

                  // Check if the executed command type is one of the known ones, or corrupted.
                  switch (executedCommandType)
                  {
                    case SERIAL_CMD_DAQ_START_HEATER:
                    case SERIAL_CMD_DAQ_START_SAMPLE:
                    case SERIAL_CMD_DAQ_STOP:
                    case SERIAL_CMD_HEAT_START:
                    case SERIAL_CMD_HEAT_STOP:
                    case SERIAL_CMD_BLINK:
                      // One of the valid command types; Add to queue for processing this completed command
                      if (executionSuccess)
                      {
                        commandsIn_.Enqueue(new IncomingMessage(commandID, IncomingMessage.COMMAND_STATUS_SUCC));
                      }
                      else
                      {
                        commandsIn_.Enqueue(new IncomingMessage(commandID, IncomingMessage.COMMAND_STATUS_FAIL));
                      }
                      break;
                    default:
                      // Unrecognized command type, consider as corrupted.
                      // Log error and return function
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, SERIAL_REPLY_CMDRESPONSE", readBuffer);
                      return;
                  }
                }
                else
                {// Somehow, number of parameters is wrong.
                  errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, SERIAL_REPLY_CMDRESPONSE", readBuffer);
                }
              }
              break;
            case SERIAL_SEND_TEMP_HEATER:
              {
                /*********************************
                *     TEMPERATURE READINGS       *
                * *******************************/
                /* Temperature reading of both heaters.
                * Format:
                * ^t|[heaterID]|[heater2T]@
                * where    ^              is SERIAL_REPLY_START
                *          t              is SERIAL_SEND_TEMP_HEATER
                *          |              is SERIAL_REPLY_SEPARATOR
                *          [heater1T]     is the temperature of heater 1
                *          [heater2T]     is the temperature of heater 2
                *          @              is SERIAL_REPLY_END
                */

                // Sanity check for number of fragments
                if (bufferFragments.Length == 3)
                {
                  HandleNewReadings(bufferFragments[1],
                                    bufferFragments[2],
                                    false);
                }
                else
                {// Somehow, number of parameters is wrong.
                  errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, SERIAL_SEND_TEMP_HEATER", readBuffer);
                }
              }
              break;
            case SERIAL_SEND_TEMP_HEATERSAMPLE:
              {
                /*********************************
                *     TEMPERATURE READINGS       *
                * *******************************/
                /* Temperature reading of both heaters and the sample.
                * Format:
                * ^b|[heaterID]|[heater2T]|[sampleT]@
                * where    ^              is SERIAL_REPLY_START
                *          b              is SERIAL_SEND_TEMP_HEATERSAMPLE
                *          |              is SERIAL_REPLY_SEPARATOR
                *          [heater1T]     is the temperature of heater 1
                *          [heater2T]     is the temperature of heater 2
                *          [sampleT]      is the temperature of the sample
                *          @              is SERIAL_REPLY_END
                */

                // Sanity check for number of fragments
                if (bufferFragments.Length == 4)
                {
                  HandleNewReadings(bufferFragments[1],
                                    bufferFragments[2],
                                    true,
                                    bufferFragments[3] );
                }
                else
                {// Somehow, number of parameters is wrong.
                  errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, SERIAL_SEND_TEMP_HEATERSAMPLE", readBuffer);
                }
              }
              break;
            case SERIAL_SEND_TEMP_HEATINGDONE:
              {
                /***************************************
                * The duration of heating is completed *
                * *************************************/
                /* Calculated scale factor based on the calibration weight
                * Format:
                * ^c@
                * where    ^               is SERIAL_REPLY_START
                *          c               is SERIAL_SEND_TEMP_HEATINGDONE
                *          @               is SERIAL_REPLY_END
                */
                // Update controls to reflect completion of heating
                HandleHeatingComplete();
              }
              break;
            case SERIAL_REPLY_ERROR:
              {
                /*********************************
                *               ERROR            *
                * *******************************/
                /* An error occured while Arduino was performing an operation
                * Format:
                * ^e|[deviceID]|[errorCode]@
                * where    ^               is SERIAL_REPLY_START
                *          e               is SERIAL_REPLY_ERROR
                *          |               is SERIAL_REPLY_SEPARATOR
                *          [deviceID]      is the ID of the implicated device
                *          [errorCode]     is the error code
                *          @               is SERIAL_REPLY_END
                */

                // Sanity check for number of fragments
                if (bufferFragments.Length == 3)
                {
                  ushort deviceID = Convert.ToUInt16(bufferFragments[1]);
                  ushort errorCode = Convert.ToUInt16(bufferFragments[2]);

                  // All Arduino errors are handled by the ErrorLogger class
                  errorLogger_.logArduinoError(errorCode, parentSandwich_.getSandwichID(), deviceID, "handleSerialDataReceived, SERIAL_REPLY_ERROR");
                }
                else
                {
                  errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, SERIAL_REPLY_ERROR", readBuffer);
                }
              }
              break;
            case SERIAL_REPLY_CORRUPTCMD:
              {
                /*********************************
                *        INVALID COMMAND         *
                * *******************************/
                /* Arduino received a command which doesn't follow the communication standards
                * Format:
                * ^x|[commandType]@
                * where    ^               is SERIAL_REPLY_START
                *          x               is SERIAL_REPLY_CORRUPTCMD
                *          |               is SERIAL_REPLY_SEPARATOR
                *          [corruptionType]is the type of corruption detected in the command received by Arduino
                *          @               is SERIAL_REPLY_END
                */

                // Sanity check for number of fragments
                if (bufferFragments.Length == 2)
                {
                  // Sanity check for number of parameters
                  switch (bufferFragments[1])
                  {
                    case SERIAL_REPLY_CORRUPTCMD_START:
                      // The command string does not have a start flag
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_START, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                      break;
                    case SERIAL_REPLY_CORRUPTCMD_END:
                      // The command string does not have an end flag
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_END, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                      break;
                    case SERIAL_REPLY_CORRUPTCMD_UNKNOWNCMD:
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_UNKNOWNCMD, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                      break;
                    case SERIAL_REPLY_CORRUPTCMD_PARAM_LESS:
                      // There are fewer params in a command line than expected
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_PARAM_LESS, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                      break;
                    case SERIAL_REPLY_CORRUPTCMD_PARAM_MORE:
                      // There are more params in a command line than expected
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_PARAM_MORE, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                      break;
                    case SERIAL_REPLY_CORRUPTCMD_PARAM_NONE:
                      // There are no params in a command line, even though some are expected
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_PARAM_NONE, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                      break;
                    default:
                      // All Arduino errors are handled by the ErrorLogger class
                      errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPTCMD_UNKNOWN, "handleSerialDataReceived, SERIAL_REPLY_ERROR", readBuffer);
                      break;
                  }
                }
                else
                {
                  errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                }
              }
              break;
            default:
              // The reply sent from Arduino doesn't comply with any of the defined formats, so assume it is corrupted.
              errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, Unknown command", readBuffer);
              break;
          }
        }
        else
        {
          // The string sent from Arduino doesn't have the correct start and end flags, so assume it is corrupted.
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "handleSerialDataReceived, start & end flags", readBuffer);
        }
      }
      catch (TimeoutException err)
      {
        errorLogger_.logCProgError(013, err, err.Message);
      }
      catch (Exception err)
      {
        errorLogger_.logUnknownError(err);
      }
    }

    // Remove the start flag, command type, and end flag from the serial buffer
    // to give only the reply parameters.
    public static string trimSerialBuffer(string rawBuffer, int startPos, int endPos)
    {
      // Trim the buffer to remove the start and end flags
      return rawBuffer.Substring(startPos + 1, endPos - (startPos + 1));
    }

    // Split the trimmed buffer based on a delimiter and return the splitted parts as an array.
    public static string[] extractReplyFragments(string rawBuffer, int startPos, int endPos)
    {
      string trimmedBuffer = trimSerialBuffer(rawBuffer, startPos, endPos);

      // Get the parameters
      return trimmedBuffer.Split(SERIAL_REPLY_SEPARATOR);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Queue handlers
    //
    // Tackle the tasks in queue for outgoing commands and command statuses.
    // These functions should be run in unique threads, and loop endlessly until commandMonitoringActive_ becomes false.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Sends the commands in the outgoing queue (commandsOut_).
    // Note that this only sends messages if the Arduino was connected.
    // awaitReponse is especially important here; it would decide if
    // we should wait for a confirmation from Arduino that it has received the command.
    public void sendCommands()
    {
      commandOutActive_ = true;
      commandOutSending_ = true;

      while (commandOutActive_)
      {
        if (commandOutSending_ && connected_)
        {
          try
          {
            OutgoingMessage outgoingMessage;

            while (commandsOut_.TryDequeue(out outgoingMessage))
            {
              // Timer for tracking command responses
              MessageTimer responseTimer = new MessageTimer();

              if (outgoingMessage.awaitResponse)
              {// Begin forming the timer for tracking the response here, so less delay later when starting the timer
               // Add this command to the waiting list. This will be referred to upon receiving
               // a response for this command, or the command timed out.
                commandsWaiting_.Add(outgoingMessage);

                // Prepare timer
                responseTimer.Interval = outgoingMessage.timeout;
                responseTimer.AutoReset = false;
                responseTimer.command = outgoingMessage;
                // For the "=> {}" notation (statement lambdas): https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions#statement-lambdas
                responseTimer.Elapsed += handleTimerElapsed;
              }
              else
              {
                responseTimer.Dispose();
              }

              // Update and assign command id
              // The modulus (%) operator takes the remainder of divison of 
              // (curCommandID_ + 1) by (maxCommandID_ + 1)
              curCommandID_ = (curCommandID_ + 1) % (maxCommandID_ + 1);
              outgoingMessage.commandID = curCommandID_;

              // Combine the command, command ID, and command params into a single string, delimited by SERIAL_CMD_SEPARATOR
              // Start off with the command type and command ID
              string commandString = string.Format("{0}{1}{2}",
                                                    outgoingMessage.commandType,
                                                    SERIAL_CMD_SEPARATOR,
                                                    outgoingMessage.commandID);

              // Append command params, if any
              for (int i = 0; i < outgoingMessage.paramList.Length; i++)
              {
                commandString = string.Format("{0}{1}{2}",
                                              commandString,
                                              SERIAL_CMD_SEPARATOR,
                                              outgoingMessage.paramList[i]);
              }

              // Add the start and end flags
              outgoingMessage.fullCommand = string.Format("{0}{1}{2}{3}",
                                                          SERIAL_CMD_START,
                                                          commandString,
                                                          SERIAL_CMD_END,
                                                          SERIAL_CMD_EOL);

              try
              {// Send the command
                // System.Diagnostics.Debug.WriteLine("Sending command: " + outgoingCommand.fullCommand);
                port_.Write(outgoingMessage.fullCommand);
              }
              catch (Exception err)
              {
                errorLogger_.logUnknownError(err);
              }

              System.Threading.Thread.Sleep(10);
            }
          }
          catch (Exception err)
          {
            errorLogger_.logUnknownError(err);
          }
        }

        System.Threading.Thread.Sleep(10);
      }
    }

    // Handles the the command statuses in the incoming queue (commandsIn_).
    /* Four possible situations for incomingCommand.status:
     * COMMAND_STATUS_SUCC: Command received by Arduino and carried out successfully
     * COMMAND_STATUS_FAIL: Command received by Arduino, but failed to execute
     * COMMAND_STATUS_TIME: Timed out while waiting for Arduino to confirm receipt of command
     * COMMAND_STATUS_ERROR: Encountered an error while processing the response from Arduino
     */
    public void handleCommandStatuses()
    {
      commandInActive_ = true;

      while (commandInActive_)
      {
        try
        {
          IncomingMessage incomingMessage;

          while (commandsIn_.TryDequeue(out incomingMessage))
          {
            // Find the outgoing command associated with this command status
            OutgoingMessage outgoingMessage = commandsWaiting_.Find(x => (x.commandID == incomingMessage.commandID));

            // If a match wasn't found, Find() returns the default value of CommandResponseTimer which should be null.
            if (outgoingMessage != null)
            {// Found a match
              switch (incomingMessage.status)
              {
                case IncomingMessage.COMMAND_STATUS_SUCC:
                  {
                    // Command received by Arduino and carried out successfully.
                    // Execute callback functions to update controls and vars in parent form.
                    string commandType = outgoingMessage.commandType;

                    switch (commandType)
                    {
                      case SERIAL_CMD_DAQ_START_HEATER:
                      case SERIAL_CMD_DAQ_START_SAMPLE:
                        /************************************************************
                        *  START DAQ
                        * **********************************************************/
                        HandleStartDAQSuccess();
                        break;
                      case SERIAL_CMD_DAQ_STOP:
                        /************************************************************
                        *  STOP DAQ
                        * **********************************************************/
                        HandleStopDAQSuccess();
                        break;
                      case SERIAL_CMD_HEAT_START:
                        /************************************************************
                        *  START HEAT
                        * **********************************************************/
                        HandleStartHeatSuccess();
                        break;
                      case SERIAL_CMD_HEAT_STOP:
                        /************************************************************
                        *  STOP HEAT
                        * **********************************************************/
                        HandleStopHeatSuccess();
                        break;
                      case SERIAL_CMD_BLINK:
                        /************************************************************
                        *  BLINK LED
                        * **********************************************************/
                        HandleBlinkLEDSuccess();
                        break;
                    }
                  }
                  break;
                case IncomingMessage.COMMAND_STATUS_TIMEOUT:
                  // Command timed-out and haven't passed the max attempts; resend it.
                  // Prepare new command to be re-sent (keeping in mind of no. of attempts)
                  OutgoingMessage resendCommand = new OutgoingMessage(outgoingMessage.commandType, outgoingMessage.paramList, outgoingMessage.awaitResponse, outgoingMessage.timeout, outgoingMessage.maxAttempts);
                  resendCommand.attempts = outgoingMessage.attempts;
                  resendCommand.fullCommand = outgoingMessage.fullCommand;

                  // Place new command on outgoing queue
                  commandsOut_.Enqueue(resendCommand);
                  break;
                case IncomingMessage.COMMAND_STATUS_FAIL:
                // Command received by Arduino, but failed to execute
                case IncomingMessage.COMMAND_STATUS_MAXATTEMPT:
                // Reached maximum attempts for resending commands (due to time-out) to Arduino
                case IncomingMessage.COMMAND_STATUS_ERROR:
                  // Encountered an error while processing the response from Arduino
                  {
                    // Execute callback functions to revert controls and update vars in parent form to reflect failed command.
                    string commandType = outgoingMessage.commandType;

                    switch (commandType)
                    {
                      case SERIAL_CMD_DAQ_START_HEATER:
                      case SERIAL_CMD_DAQ_START_SAMPLE:
                        /************************************************************
                        *  START DAQ
                        * **********************************************************/
                        HandleStartDAQFail();
                        break;
                      case SERIAL_CMD_DAQ_STOP:
                        /************************************************************
                        *  STOP DAQ
                        * **********************************************************/
                        HandleStopDAQFail();
                        break;
                      case SERIAL_CMD_HEAT_START:
                        /************************************************************
                        *  START HEAT
                        * **********************************************************/
                        HandleStartHeatFail();
                        break;
                      case SERIAL_CMD_HEAT_STOP:
                        /************************************************************
                        *  STOP HEAT
                        * **********************************************************/
                        HandleStopHeatFail();
                        break;
                      case SERIAL_CMD_BLINK:
                        /************************************************************
                        *  BLINK LED
                        * **********************************************************/
                        HandleBlinkLEDFail();
                        break;
                    }

                    // Log errors depending on the type of command failure
                    switch (incomingMessage.status)
                    {
                      case IncomingMessage.COMMAND_STATUS_FAIL:
                        errorLogger_.logCProgError(ErrorLogger.PROG_COMMAND_FAIL, "handleCommandStatuses", outgoingMessage.fullCommand);
                        break;
                      case IncomingMessage.COMMAND_STATUS_MAXATTEMPT:
                        errorLogger_.logCProgError(ErrorLogger.PROG_COMMAND_MAXATTEMPT, "handleCommandStatuses", outgoingMessage.fullCommand);
                        break;
                      case IncomingMessage.COMMAND_STATUS_ERROR:
                        errorLogger_.logCProgError(ErrorLogger.PROG_COMMAND_ERROR, "handleCommandStatuses", outgoingMessage.fullCommand);
                        break;
                    }
                  }
                  break;
              }

              // Done with this command; remove it from the waiting list.
              commandsWaiting_.Remove(outgoingMessage);
            }
            else
            {// Did not find a match in terms of commandID.
             /* This situation happens when:
              * A) a response is received while waiting to handle a timed-out command in queue.
              * B) time out occurs while waiting to handle a response in queue.
              * C) commandID is changed due to noise, thus there is naturally no match.
              *    The original command will time out because it doesn't receive a response.
              * Case A and B are complements of each other, therefore no resolution is required
              * for either case. As for case C, the original command will time out and be handled
              * accordingly. Therefore, we don't need to do anything special here.
              * */
            }
          }
        }
        catch (Exception err)
        {
          errorLogger_.logUnknownError(err);
        }

        System.Threading.Thread.Sleep(10);
      }
    }

    // Stop sending out commands, but keep the thread running it alive
    public void pauseCommandOut()
    {
      commandOutSending_ = false;
    }

    // Resume sending out commands
    public void resumeCommandOut()
    {
      commandOutSending_ = true;
    }

    // Clear the outgoing command queue
    public void clearCommandOut()
    {
      OutgoingMessage dummy;

      // Dequeue all outgoing commands
      while (commandsOut_.TryDequeue(out dummy)) { }
    }

    // Ends the command sending loop at the end of its current iteration
    public void stopCommandOut()
    {
      commandOutActive_ = false;
    }

    // Ends the command status parser loop at the end of its current iteration
    public void stopCommandIn()
    {
      commandInActive_ = false;
    }

    // Get a count of how many command responses are waiting to be handled
    public int getCommandInTotal()
    {
      return commandsIn_.Count;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Commands
    //
    // Commands to Arduino
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Blink the alarm LED
    public void QueueCommandBlink()
    {
      // Queue command
      OutgoingMessage command = new OutgoingMessage(SERIAL_CMD_BLINK, new string[0], true, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);
      commandsOut_.Enqueue(command);
    }

    // Start DAQ for the heaters and sample
    public void QueueCommandStartDAQ(bool DAQSample, string thermocoupleType, int samplesAverageCount)
    {
      string[] commandParams = {thermocoupleType,
                                string.Format("{0:D}", samplesAverageCount) };

      // Determine DAQ for heaters only or also for sample
      string commandType;
      if (DAQSample)
      {
        commandType = SERIAL_CMD_DAQ_START_SAMPLE;
      }
      else
      {
        commandType = SERIAL_CMD_DAQ_START_HEATER;
      }

      // Queue command
      OutgoingMessage command = new OutgoingMessage(commandType, commandParams, true, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);
      commandsOut_.Enqueue(command);
    }

    // Start DAQ for the heaters and sample
    public void QueueCommandStopDAQ()
    {
      // Queue command
      OutgoingMessage command = new OutgoingMessage(SERIAL_CMD_DAQ_STOP, new string[0], true, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);
      commandsOut_.Enqueue(command);
    }

    // Start heating
    public void QueueCommandStartHeat(decimal TSetpoint, decimal heatingRate, double heatingDuration, decimal propConstant, decimal integConstant)
    {
      /* Format:
      * ^h|[commandID]|s|r|d|kp|ki@
      * where    ^    is SERIAL_CMD_START
      *          h    is SERIAL_CMD_HEAT_START
      *          |            is SERIAL_CMD_SEPARATOR
      *          [commandID]  is the ID of this command
      *          s    is the temperature setpoint, in °C
      *          r    is the heating rate, in °C/min
      *          d    is the heating duration, in seconds (to be converted into ms before going into startHeat() function)
      *          kp   is the proportional constant for PI algorithm
      *          ki   is the integral constant for PI algorithm
      *          @    is SERIAL_CMD_END
      */
      string[] commandParams = {string.Format("{0:f1}", TSetpoint),       // 1 decimal place
                                string.Format("{0:f2}", heatingRate),     // 2 decimal places
                                string.Format("{0:d}", Convert.ToInt32(heatingDuration)),  // Integers (seconds)
                                string.Format("{0:f4}", propConstant),    // 4 decimal places
                                string.Format("{0:f4}", integConstant) }; // 4 decimal places

      // Queue command
      OutgoingMessage command = new OutgoingMessage(SERIAL_CMD_HEAT_START, commandParams, true, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);
      commandsOut_.Enqueue(command);
    }

    // Stop heating
    public void QueueCommandStopHeat()
    {
      // Queue command
      OutgoingMessage command = new OutgoingMessage(SERIAL_CMD_HEAT_STOP, new string[0], true, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);
      commandsOut_.Enqueue(command);
    }

    // Shutdown sandwich operations
    // Skips the command queue because the situation that calls for this function does not guarantee a connection to Arduino
    public bool SendCommandShutdown(bool logError = false)
    {
      try
      {
        port_.Write(string.Format("{0}{1}{2}{3}",
                                  SERIAL_CMD_START,
                                  SERIAL_CMD_SHUTDOWN,
                                  SERIAL_CMD_END,
                                  SERIAL_CMD_EOL));
        return true;
      }
      catch (Exception err)
      {
        if (logError)
        {
          errorLogger_.logUnknownError(err);
        }
        return false;
      }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Utility functions
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Open connection to a port
    public bool OpenPortConnection(string portName)
    {
      try
      {
        port_ = new SerialPort(portName, serialBaudRate, serialParity, serialDataBits, serialStopBits);
        port_.Handshake = serialHandshake;
        port_.Encoding = serialEncoding;
        port_.ReadTimeout = serialReadTimeout_; // 2000 ms before timeout.
        port_.Open();

        // Assign event handler for the port
        serialDataReceivedHandler_ = new SerialDataReceivedEventHandler(handleSerialDataReceived);
        port_.DataReceived += serialDataReceivedHandler_;
        connected_ = true;
        return true;
      }
      catch (UnauthorizedAccessException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PORT_ACCESS, err, err.Message);
      }
      catch (ArgumentOutOfRangeException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PORT_SETTINGS, err, err.Message);
      }
      catch (ArgumentException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PORT_NAME, err, err.Message);
      }
      catch (IOException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PORT_STATE, err, err.Message);
      }
      catch (InvalidOperationException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PORT_OPEN, err, err.Message);
      }
      catch (Exception err)
      {
        // None of the above exceptions were caught
        errorLogger_.logUnknownError(err);
      }

      // Will only reach here if there was a problem opening the port.
      connected_ = false;
      return false;
    }

    // Close connection to the port
    public bool ClosePortConnection()
    {
      try
      {
        // Remove serial data event handler
        port_.DataReceived -= serialDataReceivedHandler_;
        serialDataReceivedHandler_ = null;

        // Stop all message handling; This would also lead to the threads eventually terminating by themselves
        stopCommandOut();
        stopCommandIn();

        // Close the port
        port_.Close();

        connected_ = false;
        return true;
      }
      catch (Exception err)
      {
        errorLogger_.logUnknownError(err);
      }

      // Will only reach here if there was a problem closing the port.
      connected_ = true;
      return false;
    }
  }
}
