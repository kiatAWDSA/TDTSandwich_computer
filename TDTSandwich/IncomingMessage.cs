using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDTSandwich
{
    class IncomingMessage
    {
        // Status of the command
        public const int COMMAND_STATUS_SUCC        = 0;    // Command received by Arduino and carried out successfully
        public const int COMMAND_STATUS_FAIL        = 1;    // Command received by Arduino, but failed to execute
        public const int COMMAND_STATUS_TIMEOUT     = 2;    // Timed out while waiting for Arduino to confirm receipt of command
        public const int COMMAND_STATUS_MAXATTEMPT  = 3;    // Reached maximum number of attempts to re-send the command due to timeouts
        public const int COMMAND_STATUS_ERROR       = 4;    // Encountered an error while processing the response from Arduino

        public int commandID;
        public int status;

        public IncomingMessage(int givenCommandID, int givenStatus)
        {
            commandID = givenCommandID;
            status = givenStatus;
        }
    }
}
