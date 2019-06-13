using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDTSandwich
{
  class OutgoingMessage
  {
    public int commandID;   // Identifier of this command

    public readonly string commandType;     // The type of the command. See the constants at beginning of Commands class definition
    public readonly string[] paramList;     // Would contain parameters of the command such as duration of heating, etc..
    public readonly bool awaitResponse;     // Whether to wait for a response to this command or not
    public readonly int timeout;            // Max amount of time (ms) to wait before considering this command as timed out
    public readonly int maxAttempts;        // Max number of times to retry this command if it times out
    public int attempts;                    // Tracks the number of attempts to send this command
    public string fullCommand;              // The full command string
    
    public OutgoingMessage(string givenCommandType, string[] givenParamList, bool givenAwaitResponse, int givenTimeout, int givenMaxAttempts)
    {
      commandType     = givenCommandType;
      paramList       = givenParamList;
      awaitResponse   = givenAwaitResponse;
      timeout         = givenTimeout;
      maxAttempts     = givenMaxAttempts;
      attempts        = 0;
    }
  }
}
