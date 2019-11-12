/*
 *  C# interface for commanding the Arduinos controlling the TDT sandwiches and recording the data.
 *
 *  created 2017
 *  by Soon Kiat Lau
*/

using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;

namespace TDTSandwich
{
  public partial class TDTSandwich : Form
  {
    private bool initialized;
    private const string errorLogFilePath_ = @".\errorLog.txt"; // Relative path to error log file. @ is to escape forward slashes
    private const string COMDeviceNameFragment_ = "CH340 (COM"; // A fragment of the name shared by all the Arduino devices. Open device manager, select device -> Properties -> Details -> Friendly Name
    private ErrorLogger errorLogger_;
    private List<COMPort> COMPortAssignment;
    private List<string> unoccupiedCOMPorts;

    private List<Sandwich> sandwiches;
    private int sandwichCount;
    private int sandwichCreatedControlCount; // This is used in naming of the sandwich controls and never decreases. Therefore, each sandwich always has a unique control name.

    // Threads
    private Thread errorLogWorker_;

    public TDTSandwich()
    {
      initialized = false;
      InitializeComponent();
      sandwichCount = 0;
      sandwichCreatedControlCount = 0;
      errorLogger_ = new ErrorLogger(errorLogFilePath_);

      COMPortAssignment = new List<COMPort>();
      unoccupiedCOMPorts = new List<string>(SerialPort.GetPortNames());
      sandwiches = new List<Sandwich>();

      // Clear the dummy sandwich used for testing out control arrangement.
      mainFlow.Controls.Clear();

      // Try to find the default config file in current directory. If it doesn't exist, create one blank sandwich.
      bool defaultConfigExists = false;

      try
      {
        openApplyConfigFile("defaultConfig.csv");
        defaultConfigExists = true;
      }
      catch (Exception)
      {
        defaultConfigExists = false;
      }

      if (!defaultConfigExists)
      {// Force creation of a single sandwich. This should be done if no config file can be found.
        menu_config_addSandwich_Click(this, new EventArgs());
      }

      errorLogWorker_ = new Thread(errorLogger_.logErrors);
      errorLogWorker_.IsBackground = true;
      errorLogWorker_.Start();

      refreshPortList();
      scanSandwichIDs();

      initialized = true;
    }

    public void updateOccupiedPort(int sandwichControlID, string newPort)
    {
      COMPortAssignment.Find(x => x.sandwichControlID == sandwichControlID).portName = newPort;
    }

    public void exileSandwich(Sandwich refSandwich)
    {
      sandwiches.Remove(refSandwich);
    }
        
    public void refreshPortList()
    {
      // Refresh the list of available COM ports that will be displayed when opening the combobox for port
      string[] portList = SerialPort.GetPortNames();
      unoccupiedCOMPorts.Clear();
      bool[] portTaken = new bool[COMPortAssignment.Count];

      for (int i = 0; i < portTaken.Length; i++)
      {
          portTaken[i] = false;
      }

      // Identify unoccupied ports
      for (int i = 0; i < portList.Length; i++)
      {
        bool portOccupied = false;

        for (int j = 0; j < portTaken.Length; j++)
        { // If the port is already flagged as taken, don't bother checking it so we can speed up the search
          if (!portTaken[j])
          {
            if (COMPortAssignment[j].portName == portList[i])
            {
              portOccupied = true;
              portTaken[j] = true;
              break;
            }
          }
        }

        if (!portOccupied)
        { // After looping thru all the ports, this port had no occupying sandwiches
            unoccupiedCOMPorts.Add(portList[i]);
        }
      }

      // Update the dropdown list for each sandwich
      for (int i = 0; i < sandwiches.Count; i++)
      {
          sandwiches[i].updateCOMPortList(unoccupiedCOMPorts);
      }
    }
        
    public void scanSandwichIDs()
    {
      List<COMPort> legitPorts = new List<COMPort>();

      /////////////////////////////////////////////////////////////
      // Get the ports where the sandwich Arduinos are connected //
      /////////////////////////////////////////////////////////////
      // Combination of https://stackoverflow.com/a/46683622 and https://stackoverflow.com/a/6017027
      // Selecting from Win32_PnPEntity returns ALL of plug n play devices (as opposed to Win32_SerialPort);
      // Adding restriction for Name such as '(COM' narrows down to serial devices
      ManagementObjectSearcher arduinoSearch = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name like '%" + COMDeviceNameFragment_ + "%'");
      ManagementObjectCollection arduinoList = arduinoSearch.Get();

      
      if (arduinoList.Count > 0)
      {// Only proceed if there were any ports to which sandwich Arduinos are connected
        // Extract the names of the ports where sandwich Arduinos are connected
        foreach (ManagementObject arduino in arduinoList)
        {
          string caption = arduino["Caption"].ToString();
          int COMStart = caption.IndexOf("(COM") + 1;  // +1 to go past the opening bracket
          int COMEnd = caption.IndexOf(")", COMStart);
          string portName = caption.Substring(COMStart, COMEnd - COMStart);

          // Add this port to the list of legit ports
          legitPorts.Add(new COMPort(0, portName));
        }

        // Out of all the identified legit ports, identify the ones that are already assigned to any sandwiches
        for (int i = 0; i < sandwiches.Count; i++)
        {
          // Identify which sandwiches are already running (implying that they have a working port) and exclude them from the search
          if (sandwiches[i].getDAQStatus())
          {
            legitPorts.RemoveAll(x => x.portName == sandwiches[i].getCOMPort());
          }
        }
      }


      // Assign the remaining legit ports to the appropriate sandwich class instances
      // Go in reverse direction because we will be removing ports as we assign them
      // Remaining ports will be assumed as unassigned
      for (int i = 0; i < legitPorts.Count; i++)
      {// Get IDs of the sandwiches and associate them with the COM ports. If an invalid or no response is received, remove that port from the list
        SerialPort port = new SerialPort(legitPorts[i].portName, Communication.serialBaudRate, Communication.serialParity, Communication.serialDataBits, Communication.serialStopBits);
        port.Handshake = Communication.serialHandshake;
        port.Encoding = Communication.serialEncoding;
        port.ReadTimeout = 100; // Increase this if the sandwiches are not responding quick enough
        bool commandSent = false;

        try
        {
          port.Open();

          // Send a check for connection (SERIAL_CMD_CONNECTION command), which the Arduino would respond
          // with SERIAL_REPLY_CONNECTION and the sandwich ID.
          port.Write(string.Format( "{0}{1}{2}{3}",
                                    Communication.SERIAL_CMD_START,
                                    Communication.SERIAL_CMD_CONNECTION,
                                    Communication.SERIAL_CMD_END,
                                    Communication.SERIAL_CMD_EOL));

          // Give time for Arduino to respond
          System.Threading.Thread.Sleep(20);

          // Sometimes, the first command may not receive a response. So we send a second time
          // Clear all serial port buffers (to prepare for the second try)
          port.DiscardInBuffer();
          port.DiscardOutBuffer();
          port.Write(string.Format( "{0}{1}{2}{3}",
                                    Communication.SERIAL_CMD_START,
                                    Communication.SERIAL_CMD_CONNECTION,
                                    Communication.SERIAL_CMD_END,
                                    Communication.SERIAL_CMD_EOL));

          // Give time for Arduino to respond
          System.Threading.Thread.Sleep(50);

          commandSent = true;
        }
        catch (Exception err)
        {
          errorLogger_.logUnknownError(err);
        }

        if (commandSent)
        {// If the command was successfully sent, start scraping for the response from Arduino
          try
          {
            // Since we set ReadTimeout to a finite number, ReadTo will return a TimeoutException if no response is received within
            // that time limit. This is done on purpose because the thread would be blocked forever by ReadLine if ReadTimeout is not a
            // finite number. If no response is received after the timeout or the response is an invalid format, then we assume
            // that this port is not occupied by the Arduino controlling a sandwich.
            string readBuffer = port.ReadTo(Communication.SERIAL_REPLY_EOL);

            // Check if the start and end of the response have the correct flags
            if (readBuffer.Substring(0, 1) == Communication.SERIAL_REPLY_START && readBuffer.Substring(readBuffer.Length - 1, 1) == Communication.SERIAL_REPLY_END)
            {
              // Now extract the sandwich ID that was sent out by this Arduino
              int startPos = readBuffer.IndexOf(Communication.SERIAL_REPLY_START);
              int endPos = readBuffer.LastIndexOf(Communication.SERIAL_REPLY_END);

              string[] replyFragments = Communication.extractReplyFragments(readBuffer, startPos, endPos);

              if (replyFragments.Length == 2)
              {// Sanity check; should have two fragments. One for SERIAL_REPLY_CONNECTION and the other for the sandwich ID
                try
                {
                  int sandwichID = Convert.ToInt16(replyFragments[1]);

                  // Assign the port to the sandwich class instance with the same ID that we got from the physical sandwich Arduino
                  Sandwich matchingSandwich = sandwiches.Find(x => x.getSandwichID() == sandwichID);
                  matchingSandwich.setCOMPort(legitPorts[i].portName);
                }
                catch (Exception err)
                {// There was a problem in the conversion process; corrupted message maybe?
                  errorLogger_.logUnknownError(err);
                }
              }
              else
              {// Somehow, number of parameters is wrong.
                errorLogger_.logCProgError(ErrorLogger.ERR_PROG_REPLY_CORRUPT, "scanSandwichIDs, SERIAL_REPLY_CONNECTION", readBuffer);
              }

              // Finally, clear all serial port buffers and close the port
              port.DiscardInBuffer();
              port.DiscardOutBuffer();
              port.Close();
            }
            else
            {// The response doesn't follow the communication standards; either the message is corrupted or the Arduino is not programmed as a TDT Sandwich controller
              port.Close();
            }
          }
          catch (TimeoutException)
          {// No response from this device; it's probably an Arduino which isn't programmed as a TDT Sandwich controller
            port.Close();
          }
        }
        else
        {// If the command was unsuccessfully sent, close the port
          // Note that since this could also happen if the port failed to open, we ignore any errors from closing the (unopened) port
          try { port.Close(); }
          catch (Exception) { }
        }
      }
    }

    private void menu_config_addSandwich_Click(object sender, EventArgs e)
    {
        sandwiches.Add(new Sandwich(this, mainFlow, errorLogger_, (sandwichCreatedControlCount + 1), sandwichCreatedControlCount, unoccupiedCOMPorts));
        COMPortAssignment.Add(new COMPort(sandwichCreatedControlCount + 1, "None"));
        sandwichCount++;
        sandwichCreatedControlCount++;
    }

    private void menu_port_refresh_Click(object sender, EventArgs e)
    {
      refreshPortList();
    }

    private void menu_port_autoFill_Click(object sender, EventArgs e)
    {
      scanSandwichIDs();
    }

    private void menu_config_openConfig_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("This will delete all the sandwiches you currently have. Also, you must stop all DAQ/heating/recording, or else unexpected things will occur. Are you sure you want to proceed?", "WARNING", MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
        if (openConfigFileDialog.ShowDialog() == DialogResult.OK)
        {
          try
          {
            openApplyConfigFile(openConfigFileDialog.FileName);
          }
          catch (Exception err)
          {
            MessageBox.Show("Error opening configuration file. Please check if the file exists or if you have chosen the correct location.", "Error");
            MessageBox.Show(err.Message, "Exception message (contact Kiat about this)");
          }
        }
      }
    }

    private void openApplyConfigFile(string filePath)
    {
      FileStream openConfigStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
      StreamReader openConfigReader = new StreamReader(openConfigStream);

      if (initialized)
      {// If we have initialized, then
        // delete all sandwiches, if any, and reset sandwich count counters.
        for (int i = (sandwiches.Count - 1); i >= 0; i--)
        {
          sandwiches[i].deleteSandwich(this, new EventArgs());
        }

        sandwichCount = 0;
        sandwichCreatedControlCount = 0;

        COMPortAssignment = new List<COMPort>();
        unoccupiedCOMPorts = new List<string>(SerialPort.GetPortNames());
        sandwiches = new List<Sandwich>();
      }

      string configuration = "";

      while (openConfigReader.Peek() >= 0)
      {
        configuration = openConfigReader.ReadLine();
        menu_config_addSandwich_Click(this, new EventArgs());
        sandwiches.Last().setConfiguration(configuration);
      }

      openConfigReader.Close();

      refreshPortList();
      scanSandwichIDs();
    }

    private void menu_config_saveConfig_Click(object sender, EventArgs e)
    {
      if (saveConfigFileDialog.ShowDialog() == DialogResult.OK)
      {
        try
        {
          FileStream saveConfigStream = new FileStream(saveConfigFileDialog.FileName, FileMode.CreateNew, FileAccess.Write, FileShare.None);

          string configuration = "";

          for (int i = 0; i < sandwiches.Count; i++)
          {
            configuration += sandwiches[i].getConfiguration();
            configuration += "\n";
          }

          StreamWriter saveConfigWriter = new StreamWriter(saveConfigStream);
          saveConfigWriter.Write(configuration);
          saveConfigWriter.Flush();
          saveConfigWriter.Close();
        }
        catch (Exception err)
        {
          MessageBox.Show("The program encountered an error when attempting to save the configuration. Please choose a different name for the configuration or check if you appropriate/administrator privileges to save file to this location.", "Error saving configuration file");
          MessageBox.Show(err.Message, "Error saving configuration file");
        }
      }
    }

    private Sandwich getSandwichByControlID(int sandwichControlID)
    {
      return sandwiches.Find(s => s.getSandwichID() == sandwichControlID);
    }

    private void TDTSandwich_FormClosing(object sender, FormClosingEventArgs e)
    {
      const string message = "Are you sure that you want to close the program? ALL UNSAVED DATA WILL BE LOST.\n\nPress Yes to close; press No to resume the program.";
      const string caption = "Close program";
      if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
      {
        // User cancelled closing
        e.Cancel = true;
      }
      else
      {
        // Stop operation of all sandwiches and disconnect the ports
        foreach (Sandwich sandwich in sandwiches)
        {
          sandwich.SendCommandShutdown();
        }
        foreach (Sandwich sandwich in sandwiches)
        {
          // Don't close port because it will cause the thread to hang:
          // https://stackoverflow.com/questions/8843301/c-sharp-winform-freezing-on-serialport-close
          // Instead, let the closing routine handle it because it will be automatically closed upon program exit anyway.
          sandwich.Shutdown(false);
        }

        // Stop error logging
        errorLogger_.stopLoggingErrors();
        errorLogWorker_.Join();
      }
    }
  }// END TDTSandwich class def.

  public class COMPort
  {
    public int sandwichControlID;
    public string portName;

    public COMPort(int givenSandwichControlID, string givenPortName)
    {
        sandwichControlID = givenSandwichControlID;
        portName = givenPortName;
    }
  }
}// END TDTSandwich namespace
