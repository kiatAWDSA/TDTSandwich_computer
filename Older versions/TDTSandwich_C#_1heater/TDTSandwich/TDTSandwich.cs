/*
 *  C# interface for commanding the Arduinos controlling the TDT sandwiches and recording the data.
 *
 *  created 2017
 *  by Soon Kiat Lau
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;

namespace TDTSandwich
{
    // TODO: Provide blink LED button
    public partial class TDTSandwich : Form
    {
        bool initialized;
        List<Sandwich> sandwiches;
        List<COMPort> COMPortAssignment;
        List<string> unoccupiedCOMPorts;
        int sandwichCount;
        int sandwichCreatedControlCount; // This is used in naming of the sandwich controls and never decreases. Therefore, each sandwich always has a unique control name.

        public TDTSandwich()
        {
            initialized = false;
            InitializeComponent();
            sandwichCount = 0;
            sandwichCreatedControlCount = 0;

            COMPortAssignment = new List<COMPort>();
            unoccupiedCOMPorts = new List<string>(SerialPort.GetPortNames());
            sandwiches = new List<Sandwich>();



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
            {   // Force creation of a single sandwich. This should be done if no config file can be found.
                menu_config_addSandwich_Click(this, new EventArgs());
            }

            initialized = true;
        }

        public void updateOccupiedPort(int sandwichControlID, string newPort)
        {
                COMPortAssignment.Find(x => x.sandwichControlID == sandwichControlID).port = newPort;
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
                        if (COMPortAssignment[j].port == portList[i])
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
            List<COMPort> inactivePorts = new List<COMPort>(); // In this function, the sandwichControlID is actually the sandwichID.. just reusing the class...
            string[] allPorts = SerialPort.GetPortNames();
            bool[] portAssigned = new bool[sandwiches.Count];

            for (int i = 0; i < portAssigned.Length; i++)
            {
                portAssigned[i] = false;
            }

            for (int i = 0; i < allPorts.Length; i++)
            {
                inactivePorts.Add(new COMPort(0, allPorts[i]));
            }

            for (int i = 0; i < sandwiches.Count; i++)
            {
                // Identify which sandwiches are already running (so they have a working port) and exclude them from the search
                if (sandwiches[i].getDAQStatus())
                {
                    inactivePorts.RemoveAll(x => x.port == sandwiches[i].getCOMPort());
                    portAssigned[i] = true;
                }
            }

            for (int i = (inactivePorts.Count - 1); i >= 0; i--)
            {   // Get IDs of the sandwiches and associate them with the COM ports. If an invalid or no response is receivem remove that port from the list
                SerialPort port = new SerialPort(inactivePorts[i].port, Sandwich.getBaudRate(), Sandwich.getParity(), Sandwich.getDataBits(), Sandwich.getStopBits());
                port.Handshake = Handshake.None;
                port.Encoding = System.Text.Encoding.ASCII;
                port.ReadTimeout = 200; // This timeout is shorter than usual to reduce delay during program start-up
                port.Open();
                port.Write("w\n");

                try
                {
                    // Since we set ReadTimeout to a finite number, ReadLine will return a TimeoutException if no response is received within
                    // that time limit. This is done on purpose because the thread would be blocked forever by ReadLine if ReadTimeout is not a
                    // finite number. If no response is received after the timeout (200 ms), then we assume that this port is not occupied by
                    // an Arduino controlling a sandwich
                    int sandwichID = Convert.ToInt32(port.ReadTo("\n").Substring(1));

                    if (sandwichID > 0)
                    {
                        inactivePorts[i].sandwichControlID = sandwichID;
                    }
                    else
                    {
                        inactivePorts.RemoveAt(i);
                    }
                }
                catch (TimeoutException)
                {
                    inactivePorts.RemoveAt(i);
                }

                port.Close();
            }

            for (int i = 0; i < inactivePorts.Count; i++)
            {   // We now know the IDs of sandwiches and their ports. Give the port names to the sandwich controls with the associated ID
                for (int j = 0; j < sandwiches.Count; j++)
                {
                    if (!portAssigned[j])
                    {
                        if (inactivePorts[i].sandwichControlID == sandwiches[j].getSandwichID())
                        {
                            sandwiches[j].setCOMPort(inactivePorts[i].port);
                            portAssigned[j] = true;
                        }
                    }
                }
            }
        }
        /*
        private void but_autoCOMPort_Click(object sender, EventArgs e)
        {
            scanSandwichIDs();
        }

        private void but_refreshPortList_Click(object sender, EventArgs e)
        {
            refreshPortList();
        }
        */
        private void menu_config_addSandwich_Click(object sender, EventArgs e)
        {
            sandwiches.Add(new Sandwich(this, mainFlow, (sandwichCreatedControlCount + 1), sandwichCreatedControlCount, unoccupiedCOMPorts));
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

        private void menu_clearErrors_click(object sender, EventArgs e)
        {
            for (int i = 0; i < sandwiches.Count; i++)
            {
                sandwiches[i].clearError();
            }
        }

        private void openApplyConfigFile(string filePath)
        {
            FileStream openConfigStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader openConfigReader = new StreamReader(openConfigStream);

            if (initialized)
            {   // If we have initialized, then
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
                    MessageBox.Show("Error saving configuration file. Please choose a different configuration file name or check if you appropriate privileges to save file to this location.", "Error");
                    MessageBox.Show(err.Message, "Exception message (contact Kiat about this)");
                }
            }
        }
    }// END TDTSandwich class def.

    public class COMPort
    {
        public int sandwichControlID;
        public string port;

        public COMPort(int givenSandwichControlID, string givenPort)
        {
            sandwichControlID = givenSandwichControlID;
            port = givenPort;
        }
    }

    public class Sandwich
    {
        private TDTSandwich _owningForm;
        private FlowLayoutPanel _mainFlowPanel;

        private FlowLayoutPanel _sandwich;
        // ID controls
        private GroupBox _ID;
        private FlowLayoutPanel _ID_flow;
        private TextBox _ID_textbox;
        // DAQ controls
        private GroupBox _DAQ;
        private FlowLayoutPanel _DAQ_flow;
        private FlowLayoutPanel _DAQ_heater;
        private FlowLayoutPanel _DAQ_sample;
        private Button _DAQ_startDAQ;
        private TextBox _DAQ_heater_textbox;
        private Label _DAQ_heater_label;
        private Button _DAQ_stopDAQ;
        private Label _DAQ_sample_label;
        private TextBox _DAQ_sample_textbox;
        private CheckBox _DAQ_readSample;
        // Heat controls
        private GroupBox _heat;
        private FlowLayoutPanel _heat_flow;
        private FlowLayoutPanel _heat_setpoint;
        private Label _heat_setpoint_label;
        private NumericUpDown _heat_setpoint_upDown;
        private FlowLayoutPanel _heat_ramp;
        private Label _heat_ramp_label;
        private NumericUpDown _heat_ramp_upDown;
        private GroupBox _heat_timer;
        private FlowLayoutPanel _heat_timer_flow;
        private Label _heat_timer_h_label;
        private NumericUpDown _heat_timer_h_upDown;
        private Label _heat_timer_m_label;
        private NumericUpDown _heat_timer_m_upDown;
        private Label _heat_timer_s_label;
        private NumericUpDown _heat_timer_s_upDown;
        private Button _heat_startHeat;
        private Button _heat_stopHeat;
        // Record controls
        private GroupBox _record;
        private FlowLayoutPanel _record_flow;
        private FlowLayoutPanel _record_filepath;
        private Label _record_filepath_label;
        private Button _record_startRecord;
        private Button _record_stopRecord;
        private TextBox _record_filepath_textbox;
        private Button _record_browse;
        // Advanced controls
        private GroupBox _advanced;
        private Button _advanced_show;
        private FlowLayoutPanel _advanced_flow;
        private FlowLayoutPanel _advanced_hiddenFlow;
        private Button _advanced_hide;
        private Button _advanced_blinkLED;
        private FlowLayoutPanel _advanced_thermocouple;
        private Label _advanced_thermocouple_label;
        private ComboBox _advanced_thermocouple_dropdown;
        private FlowLayoutPanel _advanced_port;
        private Label _advanced_port_label;
        private ComboBox _advanced_port_dropdown;
        private FlowLayoutPanel _advanced_ID;
        private Label _advanced_ID_label;
        private NumericUpDown _advanced_ID_upDown;
        private Button _advanced_removeSandwich;
        private FlowLayoutPanel _advanced_PID_radius;
        private Label _advanced_PID_radius_label;
        private NumericUpDown _advanced_PID_radius_upDown;
        private FlowLayoutPanel _advanced_PID_proportional;
        private Label _advanced_PID_proportional_label;
        private NumericUpDown _advanced_PID_proportional_upDown;
        private FlowLayoutPanel _advanced_PID_integral;
        private Label _advanced_PID_integral_label;
        private NumericUpDown _advanced_PID_integral_upDown;
        private FlowLayoutPanel _advanced_error;
        private Label _advanced_error_label;
        private TextBox _advanced_error_textbox;


        private int _sandwichHeight;

        private Thread _readThread;
        private Thread _closeThread;
        private SerialPort _port;
        private FileStream _recordStream;
        private StreamWriter _CSVWriter;

        // Serial port settings. Must match Arduino
        private static int _baudRate =      9600;
        private static Parity _parity =     Parity.None;
        private static int _dataBits =      8;
        private static StopBits _stopBits = StopBits.One;
        
        private bool _DAQActive;
        private bool _DAQSample;
        private bool _recordActive;
        private bool _recordStart;
        private bool _recordStop;
        private bool _heatActive;
        private bool _heatStart;
        private bool _heatStop;
        private bool _countingDown;
        private bool _countingDownStart;
        private bool _legitFilePath;
        private int _sandwichControlID;
        private int _sandwichID;
        private ulong _heatingDuration;     // ms
        private ulong _arduinoElapsedTime;  // ms
        private ulong _recordingStartTime;  // ms
        private ulong _recordingElapsedTime;         // ms
        private ulong _heatingStartTime;    // ms
        private string _controlPrefix;
        private string _replyBuffer;
        private string[] _readData;
        private string _heaterT;
        private string _sampleT;
        private string _savePathway;
        private string _COMPort;
        private string _errorMessage;
        private delegate void _suspendLayoutControlCallback(Control refControl);
        private delegate void _resumeLayoutControlCallback(Control refControl);
        private delegate void _performLayoutControlCallback(Control refControl);
        private delegate void _changeFlowVisibilityCallback(FlowLayoutPanel flowPanel, bool visibility);
        private delegate void _changeControlEnableCallback(Control butControl, bool state);
        private delegate void _showControlCallback(Control refControl, Control parentControl);
        private delegate void _hideControlCallback(Control refControl, Control parentControl);
        private delegate void _updateTextboxCallback(TextBox boxControl, string data);
        private delegate void _updateControlValueCallback(NumericUpDown refControl, decimal data);
        private delegate string _getComboSelectedItemCallback(ComboBox comboControl);
        private delegate void _changeControlBgCallback(Control refControl, Color refColor);

        public Sandwich(TDTSandwich owningForm, FlowLayoutPanel mainFlowPanel, int sandwichControlID, int sandwichCreatedControlCount, List<string> unoccupiedCOMPorts)
        {
            _sandwichControlID = sandwichControlID;
            _controlPrefix = "sandwich_" + Convert.ToString(_sandwichControlID);

            _sandwichHeight = 68;

            _owningForm = owningForm;
            _mainFlowPanel = mainFlowPanel;

            _DAQ_startDAQ = new System.Windows.Forms.Button();
            _DAQ_heater_textbox = new System.Windows.Forms.TextBox();
            _DAQ_heater_label = new System.Windows.Forms.Label();
            _DAQ_stopDAQ = new System.Windows.Forms.Button();
            _record_startRecord = new System.Windows.Forms.Button();
            _record_stopRecord = new System.Windows.Forms.Button();
            _advanced_error_textbox = new System.Windows.Forms.TextBox();
            _advanced_error_label = new System.Windows.Forms.Label();
            _heat_stopHeat = new System.Windows.Forms.Button();
            _heat_startHeat = new System.Windows.Forms.Button();
            _advanced_thermocouple_dropdown = new System.Windows.Forms.ComboBox();
            _advanced_blinkLED = new System.Windows.Forms.Button();
            _sandwich = new System.Windows.Forms.FlowLayoutPanel();
            _ID = new System.Windows.Forms.GroupBox();
            _ID_flow = new System.Windows.Forms.FlowLayoutPanel();
            _ID_textbox = new System.Windows.Forms.TextBox();
            _DAQ = new System.Windows.Forms.GroupBox();
            _DAQ_flow = new System.Windows.Forms.FlowLayoutPanel();
            _DAQ_heater = new System.Windows.Forms.FlowLayoutPanel();
            _DAQ_sample = new System.Windows.Forms.FlowLayoutPanel();
            _DAQ_sample_label = new System.Windows.Forms.Label();
            _DAQ_sample_textbox = new System.Windows.Forms.TextBox();
            _DAQ_readSample = new System.Windows.Forms.CheckBox();
            _heat = new System.Windows.Forms.GroupBox();
            _heat_flow = new System.Windows.Forms.FlowLayoutPanel();
            _heat_setpoint = new System.Windows.Forms.FlowLayoutPanel();
            _heat_setpoint_label = new System.Windows.Forms.Label();
            _heat_setpoint_upDown = new System.Windows.Forms.NumericUpDown();
            _heat_ramp = new System.Windows.Forms.FlowLayoutPanel();
            _heat_ramp_label = new System.Windows.Forms.Label();
            _heat_ramp_upDown = new System.Windows.Forms.NumericUpDown();
            _heat_timer = new System.Windows.Forms.GroupBox();
            _heat_timer_flow = new System.Windows.Forms.FlowLayoutPanel();
            _heat_timer_h_upDown = new System.Windows.Forms.NumericUpDown();
            _heat_timer_h_label = new System.Windows.Forms.Label();
            _heat_timer_m_upDown = new System.Windows.Forms.NumericUpDown();
            _heat_timer_m_label = new System.Windows.Forms.Label();
            _heat_timer_s_upDown = new System.Windows.Forms.NumericUpDown();
            _heat_timer_s_label = new System.Windows.Forms.Label();
            _record = new System.Windows.Forms.GroupBox();
            _record_flow = new System.Windows.Forms.FlowLayoutPanel();
            _record_filepath = new System.Windows.Forms.FlowLayoutPanel();
            _record_filepath_label = new System.Windows.Forms.Label();
            _record_filepath_textbox = new System.Windows.Forms.TextBox();
            _record_browse = new System.Windows.Forms.Button();
            _advanced = new System.Windows.Forms.GroupBox();
            _advanced_flow = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_hiddenFlow = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_show = new System.Windows.Forms.Button();
            _advanced_hide = new System.Windows.Forms.Button();
            _advanced_thermocouple = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_thermocouple_label = new System.Windows.Forms.Label();
            _advanced_port = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_port_label = new System.Windows.Forms.Label();
            _advanced_port_dropdown = new System.Windows.Forms.ComboBox();
            _advanced_ID = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_ID_label = new System.Windows.Forms.Label();
            _advanced_ID_upDown = new System.Windows.Forms.NumericUpDown();
            _advanced_removeSandwich = new System.Windows.Forms.Button();
            _advanced_PID_radius = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_PID_radius_label = new System.Windows.Forms.Label();
            _advanced_PID_radius_upDown = new System.Windows.Forms.NumericUpDown();
            _advanced_PID_proportional = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_PID_proportional_label = new System.Windows.Forms.Label();
            _advanced_PID_proportional_upDown = new System.Windows.Forms.NumericUpDown();
            _advanced_PID_integral = new System.Windows.Forms.FlowLayoutPanel();
            _advanced_PID_integral_label = new System.Windows.Forms.Label();
            _advanced_PID_integral_upDown = new System.Windows.Forms.NumericUpDown();
            _advanced_error = new System.Windows.Forms.FlowLayoutPanel();
            _sandwich.SuspendLayout();
            _ID.SuspendLayout();
            _ID_flow.SuspendLayout();
            _DAQ.SuspendLayout();
            _DAQ_flow.SuspendLayout();
            _DAQ_heater.SuspendLayout();
            _DAQ_sample.SuspendLayout();
            _heat.SuspendLayout();
            _heat_flow.SuspendLayout();
            _heat_setpoint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_heat_setpoint_upDown)).BeginInit();
            _heat_ramp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_heat_ramp_upDown)).BeginInit();
            _heat_timer.SuspendLayout();
            _heat_timer_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_heat_timer_h_upDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(_heat_timer_m_upDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(_heat_timer_s_upDown)).BeginInit();
            _record.SuspendLayout();
            _record_flow.SuspendLayout();
            _record_filepath.SuspendLayout();
            _advanced.SuspendLayout();
            _advanced_flow.SuspendLayout();
            _advanced_hiddenFlow.SuspendLayout();
            _advanced_thermocouple.SuspendLayout();
            _advanced_port.SuspendLayout();
            _advanced_ID.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_ID_upDown)).BeginInit();
            _advanced_PID_radius.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_PID_radius_upDown)).BeginInit();
            _advanced_PID_proportional.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_PID_proportional_upDown)).BeginInit();
            _advanced_PID_integral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_PID_integral_upDown)).BeginInit();
            _advanced_error.SuspendLayout();
            _mainFlowPanel.SuspendLayout();
            _owningForm.SuspendLayout();


            ////////////////////////////////////
            //       Sandwich
            ////////////////////////////////////
            // 
            // Parent container for all the controls
            // 
            _sandwich.Controls.Add(_ID);
            _sandwich.Controls.Add(_DAQ);
            _sandwich.Controls.Add(_heat);
            _sandwich.Controls.Add(_record);
            _sandwich.Controls.Add(_advanced);
            _sandwich.Location = new System.Drawing.Point(0, 0); // Since we are adding to a flowLayoutPanel in a top-down direction, we do not have to worry about the exact location of the sandwich (it will be autocalculated)
            _sandwich.Margin = new System.Windows.Forms.Padding(0);
            _sandwich.Name = _controlPrefix;
            _sandwich.Size = new System.Drawing.Size(1966, 67);
            _sandwich.TabIndex = 12;
            // Add the sandwich and adjust the height of the main flowlayout panel
            _mainFlowPanel.Controls.Add(_sandwich);
            _mainFlowPanel.Size = new System.Drawing.Size(_sandwich.Size.Width, _mainFlowPanel.Size.Height + _sandwichHeight);

            ////////////////////////////////////
            //       ID
            ////////////////////////////////////
            // 
            // ID
            // 
            _ID.Controls.Add(_ID_flow);
            _ID.Location = new System.Drawing.Point(3, 0);
            _ID.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _ID.Name = _controlPrefix + "_ID";
            _ID.Size = new System.Drawing.Size(40, 66);
            _ID.TabIndex = 0;
            _ID.TabStop = false;
            _ID.Text = "ID";
            // 
            // ID_flow
            // 
            _ID_flow.Controls.Add(_ID_textbox);
            _ID_flow.Location = new System.Drawing.Point(3, 13);
            _ID_flow.Name = _controlPrefix + "_ID_flow";
            _ID_flow.Size = new System.Drawing.Size(34, 52);
            _ID_flow.TabIndex = 19;
            // 
            // ID_textbox
            // 
            _ID_textbox.BackColor = System.Drawing.SystemColors.Control;
            _ID_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _ID_textbox.Location = new System.Drawing.Point(26, 19);
            _ID_textbox.Margin = new System.Windows.Forms.Padding(3, 19, 3, 3);
            _ID_textbox.MaxLength = 2;
            _ID_textbox.Name = _controlPrefix + "_ID_textbox";
            _ID_textbox.ReadOnly = true;
            _ID_textbox.Size = new System.Drawing.Size(28, 26);
            _ID_textbox.TabIndex = 2;
            // _ID_textbox.Text = "1"; // This is automatically updated whenever the sandwich ID numericupdown in the advanced section is changed.
            _ID_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            ////////////////////////////////////
            //       DAQ
            ////////////////////////////////////
            // 
            // DAQ
            // 
            _DAQ.Controls.Add(_DAQ_flow);
            _DAQ.Location = new System.Drawing.Point(49, 0);
            _DAQ.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _DAQ.Name = _controlPrefix + "_DAQ";
            _DAQ.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _DAQ.Size = new System.Drawing.Size(260, 66);
            _DAQ.TabIndex = 1;
            _DAQ.TabStop = false;
            _DAQ.Text = "DAQ";
            // 
            // DAQ_flow
            // 
            _DAQ_flow.Controls.Add(_DAQ_heater);
            _DAQ_flow.Controls.Add(_DAQ_sample);
            _DAQ_flow.Controls.Add(_DAQ_readSample);
            _DAQ_flow.Controls.Add(_DAQ_startDAQ);
            _DAQ_flow.Location = new System.Drawing.Point(3, 13);
            _DAQ_flow.Name = _controlPrefix + "_DAQ_flow";
            _DAQ_flow.Size = new System.Drawing.Size(256, 52);
            _DAQ_flow.TabIndex = 1;
            // 
            // DAQ_heater
            // 
            _DAQ_heater.Controls.Add(_DAQ_heater_label);
            _DAQ_heater.Controls.Add(_DAQ_heater_textbox);
            _DAQ_heater.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _DAQ_heater.Location = new System.Drawing.Point(0, 0);
            _DAQ_heater.Margin = new System.Windows.Forms.Padding(0);
            _DAQ_heater.Name = _controlPrefix + "_DAQ_heater";
            _DAQ_heater.Size = new System.Drawing.Size(68, 49);
            _DAQ_heater.TabIndex = 1;
            // 
            // DAQ_heater_label
            // 
            _DAQ_heater_label.AutoSize = true;
            _DAQ_heater_label.Location = new System.Drawing.Point(3, 3);
            _DAQ_heater_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _DAQ_heater_label.Name = _controlPrefix + "_DAQ_heater_label";
            _DAQ_heater_label.Size = new System.Drawing.Size(59, 13);
            _DAQ_heater_label.TabIndex = 3;
            _DAQ_heater_label.Text = "Heater (°C)";
            // 
            // DAQ_heater_textbox
            // 
            _DAQ_heater_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _DAQ_heater_textbox.Location = new System.Drawing.Point(3, 19);
            _DAQ_heater_textbox.MaxLength = 6;
            _DAQ_heater_textbox.Name = _controlPrefix + "_DAQ_heater_textbox";
            _DAQ_heater_textbox.ReadOnly = true;
            _DAQ_heater_textbox.Size = new System.Drawing.Size(60, 26);
            _DAQ_heater_textbox.TabIndex = 2;
            _DAQ_heater_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DAQ_sample
            // 
            _DAQ_sample.Controls.Add(_DAQ_sample_label);
            _DAQ_sample.Controls.Add(_DAQ_sample_textbox);
            _DAQ_sample.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _DAQ_sample.Location = new System.Drawing.Point(68, 0);
            _DAQ_sample.Margin = new System.Windows.Forms.Padding(0);
            _DAQ_sample.Name = _controlPrefix + "_DAQ_sample";
            _DAQ_sample.Size = new System.Drawing.Size(68, 49);
            _DAQ_sample.TabIndex = 2;
            // 
            // DAQ_sample_label
            // 
            _DAQ_sample_label.Enabled = false;
            _DAQ_sample_label.AutoSize = true;
            _DAQ_sample_label.Location = new System.Drawing.Point(3, 3);
            _DAQ_sample_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _DAQ_sample_label.Name = _controlPrefix + "_DAQ_sample_label";
            _DAQ_sample_label.Size = new System.Drawing.Size(62, 13);
            _DAQ_sample_label.TabIndex = 13;
            _DAQ_sample_label.Text = "Sample (°C)";
            // 
            // DAQ_sample_textbox
            // 
            _DAQ_sample_textbox.Enabled = false;
            _DAQ_sample_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _DAQ_sample_textbox.Location = new System.Drawing.Point(3, 19);
            _DAQ_sample_textbox.MaxLength = 6;
            _DAQ_sample_textbox.Name = _controlPrefix + "_DAQ_sample_textbox";
            _DAQ_sample_textbox.ReadOnly = true;
            _DAQ_sample_textbox.Size = new System.Drawing.Size(60, 26);
            _DAQ_sample_textbox.TabIndex = 2;
            _DAQ_sample_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DAQ_readSample
            // 
            _DAQ_readSample.AutoSize = true;
            _DAQ_readSample.Location = new System.Drawing.Point(139, 16);
            _DAQ_readSample.Margin = new System.Windows.Forms.Padding(3, 16, 0, 3);
            _DAQ_readSample.Name = _controlPrefix + "_DAQ_readSample";
            _DAQ_readSample.Size = new System.Drawing.Size(65, 30);
            _DAQ_readSample.TabIndex = 3;
            _DAQ_readSample.Text = "Read\r\nsample?";
            _DAQ_readSample.UseVisualStyleBackColor = true;
            // 
            // DAQ_startDAQ
            // 
            _DAQ_startDAQ.Location = new System.Drawing.Point(207, 11);
            _DAQ_startDAQ.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _DAQ_startDAQ.Name = _controlPrefix + "_DAQ_startDAQ";
            _DAQ_startDAQ.Size = new System.Drawing.Size(45, 35);
            _DAQ_startDAQ.TabIndex = 4;
            _DAQ_startDAQ.Text = "Start DAQ";
            _DAQ_startDAQ.UseVisualStyleBackColor = true;
            // 
            // DAQ_stopDAQ
            // 
            _DAQ_stopDAQ.Enabled = false;
            _DAQ_stopDAQ.Location = new System.Drawing.Point(207, 11);
            _DAQ_stopDAQ.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _DAQ_stopDAQ.Name = _controlPrefix + "_DAQ_stopDAQ";
            _DAQ_stopDAQ.Size = new System.Drawing.Size(45, 35);
            _DAQ_stopDAQ.TabIndex = 5;
            _DAQ_stopDAQ.Text = "Stop DAQ";
            _DAQ_stopDAQ.Visible = false;
            _DAQ_stopDAQ.UseVisualStyleBackColor = true;
            

            ////////////////////////////////////
            //       Heat
            ////////////////////////////////////
            // 
            // heat
            // 
            _heat.Controls.Add(_heat_flow);
            _heat.Location = new System.Drawing.Point(357, 0);
            _heat.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _heat.Name = _controlPrefix + "_heat";
            _heat.Size = new System.Drawing.Size(416, 66);
            _heat.TabIndex = 2;
            _heat.TabStop = false;
            _heat.Text = "Heat";
            // 
            // heat_flow
            // 
            _heat_flow.Controls.Add(_heat_setpoint);
            _heat_flow.Controls.Add(_heat_ramp);
            _heat_flow.Controls.Add(_heat_timer);
            _heat_flow.Controls.Add(_heat_startHeat);
            _heat_flow.Location = new System.Drawing.Point(3, 13);
            _heat_flow.Name = _controlPrefix + "_heat_flow";
            _heat_flow.Size = new System.Drawing.Size(412, 52);
            _heat_flow.TabIndex = 22;
            // 
            // heat_setpoint
            // 
            _heat_setpoint.Controls.Add(_heat_setpoint_label);
            _heat_setpoint.Controls.Add(_heat_setpoint_upDown);
            _heat_setpoint.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _heat_setpoint.Location = new System.Drawing.Point(0, 0);
            _heat_setpoint.Margin = new System.Windows.Forms.Padding(0);
            _heat_setpoint.Name = _controlPrefix + "_heat_setpoint";
            _heat_setpoint.Size = new System.Drawing.Size(72, 49);
            _heat_setpoint.TabIndex = 1;
            // 
            // heat_setpoint_label
            // 
            _heat_setpoint_label.AutoSize = true;
            _heat_setpoint_label.Location = new System.Drawing.Point(3, 3);
            _heat_setpoint_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _heat_setpoint_label.Name = _controlPrefix + "_heat_setpoint_label";
            _heat_setpoint_label.Size = new System.Drawing.Size(66, 13);
            _heat_setpoint_label.TabIndex = 14;
            _heat_setpoint_label.Text = "Setpoint (°C)";
            // 
            // heat_setpoint_upDown
            // 
            _heat_setpoint_upDown.DecimalPlaces = 1;
            _heat_setpoint_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_setpoint_upDown.Location = new System.Drawing.Point(3, 19);
            _heat_setpoint_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _heat_setpoint_upDown.Minimum = 0;
            _heat_setpoint_upDown.Maximum = 140;
            _heat_setpoint_upDown.Name = _controlPrefix + "_heat_setpoint_upDown";
            _heat_setpoint_upDown.Size = new System.Drawing.Size(60, 26);
            _heat_setpoint_upDown.TabIndex = 1;
            _heat_setpoint_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // heat_ramp
            // 
            _heat_ramp.Controls.Add(_heat_ramp_label);
            _heat_ramp.Controls.Add(_heat_ramp_upDown);
            _heat_ramp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _heat_ramp.Location = new System.Drawing.Point(72, 0);
            _heat_ramp.Margin = new System.Windows.Forms.Padding(0);
            _heat_ramp.Name = _controlPrefix + "_heat_ramp";
            _heat_ramp.Size = new System.Drawing.Size(72, 49);
            _heat_ramp.TabIndex = 2;
            // 
            // heat_ramp_label
            // 
            _heat_ramp_label.AutoSize = true;
            _heat_ramp_label.Location = new System.Drawing.Point(3, 3);
            _heat_ramp_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _heat_ramp_label.Name = _controlPrefix + "_heat_ramp_label";
            _heat_ramp_label.Size = new System.Drawing.Size(66, 13);
            _heat_ramp_label.TabIndex = 14;
            _heat_ramp_label.Text = "Ramp (%)";
            // 
            // heat_ramp_upDown
            // 
            _heat_ramp_upDown.DecimalPlaces = 2;
            _heat_ramp_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_ramp_upDown.Location = new System.Drawing.Point(3, 19);
            _heat_ramp_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _heat_ramp_upDown.Minimum = 0;
            _heat_ramp_upDown.Maximum = 100;
            _heat_ramp_upDown.Name = _controlPrefix + "_heat_ramp_upDown";
            _heat_ramp_upDown.Size = new System.Drawing.Size(69, 26);
            _heat_ramp_upDown.TabIndex = 2;
            _heat_ramp_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // heat_timer
            // 
            _heat_timer.Controls.Add(_heat_timer_flow);
            _heat_timer.Location = new System.Drawing.Point(147, 3);
            _heat_timer.Name = _controlPrefix + "_heat_timer";
            _heat_timer.Size = new System.Drawing.Size(210, 47);
            _heat_timer.TabIndex = 3;
            _heat_timer.TabStop = false;
            _heat_timer.Text = "Timer (0 if not needed)";
            // 
            // heat_timer_flow
            // 
            _heat_timer_flow.Controls.Add(_heat_timer_h_upDown);
            _heat_timer_flow.Controls.Add(_heat_timer_h_label);
            _heat_timer_flow.Controls.Add(_heat_timer_m_upDown);
            _heat_timer_flow.Controls.Add(_heat_timer_m_label);
            _heat_timer_flow.Controls.Add(_heat_timer_s_upDown);
            _heat_timer_flow.Controls.Add(_heat_timer_s_label);
            _heat_timer_flow.Location = new System.Drawing.Point(3, 12);
            _heat_timer_flow.Margin = new System.Windows.Forms.Padding(0);
            _heat_timer_flow.Name = _controlPrefix + "_heat_timer_flow";
            _heat_timer_flow.Size = new System.Drawing.Size(205, 32);
            _heat_timer_flow.TabIndex = 23;
            // 
            // heat_timer_h_upDown
            // 
            _heat_timer_h_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_timer_h_upDown.Location = new System.Drawing.Point(3, 3);
            _heat_timer_h_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _heat_timer_h_upDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            _heat_timer_h_upDown.Name = _controlPrefix + "_heat_timer_h_upDown";
            _heat_timer_h_upDown.Size = new System.Drawing.Size(40, 26);
            _heat_timer_h_upDown.TabIndex = 3;
            _heat_timer_h_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // heat_timer_h_label
            // 
            _heat_timer_h_label.AutoSize = true;
            _heat_timer_h_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_timer_h_label.Location = new System.Drawing.Point(43, 3);
            _heat_timer_h_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            _heat_timer_h_label.Name = _controlPrefix + "_heat_timer_h_label";
            _heat_timer_h_label.Size = new System.Drawing.Size(25, 24);
            _heat_timer_h_label.TabIndex = 3;
            _heat_timer_h_label.Text = "H";
            // 
            // heat_timer_m_upDown
            // 
            _heat_timer_m_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_timer_m_upDown.Location = new System.Drawing.Point(71, 3);
            _heat_timer_m_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _heat_timer_m_upDown.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            _heat_timer_m_upDown.Name = _controlPrefix + "_heat_timer_m_upDown";
            _heat_timer_m_upDown.Size = new System.Drawing.Size(40, 26);
            _heat_timer_m_upDown.TabIndex = 4;
            _heat_timer_m_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // heat_timer_m_label
            // 
            _heat_timer_m_label.AutoSize = true;
            _heat_timer_m_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_timer_m_label.Location = new System.Drawing.Point(111, 3);
            _heat_timer_m_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            _heat_timer_m_label.Name = _controlPrefix + "_heat_timer_m_label";
            _heat_timer_m_label.Size = new System.Drawing.Size(27, 24);
            _heat_timer_m_label.TabIndex = 4;
            _heat_timer_m_label.Text = "M";
            // 
            // heat_timer_s_upDown
            // 
            _heat_timer_s_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_timer_s_upDown.Location = new System.Drawing.Point(141, 3);
            _heat_timer_s_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _heat_timer_s_upDown.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            _heat_timer_s_upDown.Name = _controlPrefix + "_heat_timer_s_upDown";
            _heat_timer_s_upDown.Size = new System.Drawing.Size(40, 26);
            _heat_timer_s_upDown.TabIndex = 5;
            _heat_timer_s_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // heat_timer_s_label
            // 
            _heat_timer_s_label.AutoSize = true;
            _heat_timer_s_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _heat_timer_s_label.Location = new System.Drawing.Point(181, 3);
            _heat_timer_s_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            _heat_timer_s_label.Name = _controlPrefix + "_heat_timer_s_label";
            _heat_timer_s_label.Size = new System.Drawing.Size(23, 24);
            _heat_timer_s_label.TabIndex = 5;
            _heat_timer_s_label.Text = "S";
            // 
            // heat_startHeat
            // 
            _heat_startHeat.Enabled = false;
            _heat_startHeat.Location = new System.Drawing.Point(363, 11);
            _heat_startHeat.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _heat_startHeat.Name = _controlPrefix + "_heat_startHeat";
            _heat_startHeat.Size = new System.Drawing.Size(45, 35);
            _heat_startHeat.TabIndex = 6;
            _heat_startHeat.Text = "Start heat";
            _heat_startHeat.UseVisualStyleBackColor = true;
            // 
            // heat_stopHeat
            // 
            _heat_stopHeat.Enabled = false;
            _heat_stopHeat.Location = new System.Drawing.Point(363, 11);
            _heat_stopHeat.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _heat_stopHeat.Name = _controlPrefix + "_heat_stopHeat";
            _heat_stopHeat.Size = new System.Drawing.Size(45, 35);
            _heat_stopHeat.TabIndex = 7;
            _heat_stopHeat.Text = "Stop heat";
            _heat_stopHeat.UseVisualStyleBackColor = true;
            _heat_stopHeat.Visible = false;


            ////////////////////////////////////
            //       Record
            ////////////////////////////////////// 
            // record
            // 
            _record.Controls.Add(_record_flow);
            _record.Location = new System.Drawing.Point(707, 0);
            _record.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _record.Name = _controlPrefix + "_record";
            _record.Size = new System.Drawing.Size(235, 66);
            _record.TabIndex = 3;
            _record.TabStop = false;
            _record.Text = "Record";
            // 
            // record_flow
            // 
            _record_flow.Controls.Add(_record_filepath);
            _record_flow.Controls.Add(_record_browse);
            _record_flow.Controls.Add(_record_startRecord);
            _record_flow.Location = new System.Drawing.Point(3, 13);
            _record_flow.Name = _controlPrefix + "_record_flow";
            _record_flow.Size = new System.Drawing.Size(231, 52);
            _record_flow.TabIndex = 18;
            // 
            // record_filepath
            // 
            _record_filepath.Controls.Add(_record_filepath_label);
            _record_filepath.Controls.Add(_record_filepath_textbox);
            _record_filepath.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _record_filepath.Location = new System.Drawing.Point(0, 0);
            _record_filepath.Margin = new System.Windows.Forms.Padding(0);
            _record_filepath.Name = _controlPrefix + "_record_filepath";
            _record_filepath.Size = new System.Drawing.Size(148, 49);
            _record_filepath.TabIndex = 1;
            // 
            // record_filepath_label
            // 
            _record_filepath_label.AutoSize = true;
            _record_filepath_label.Location = new System.Drawing.Point(3, 3);
            _record_filepath_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _record_filepath_label.Name = _controlPrefix + "_record_filepath_label";
            _record_filepath_label.Size = new System.Drawing.Size(47, 13);
            _record_filepath_label.TabIndex = 14;
            _record_filepath_label.Text = "Save to (.csv):";
            // 
            // record_filepath_textbox
            // 
            _record_filepath_textbox.BackColor = System.Drawing.SystemColors.Window;
            _record_filepath_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _record_filepath_textbox.Location = new System.Drawing.Point(3, 19);
            _record_filepath_textbox.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _record_filepath_textbox.Name = _controlPrefix + "_record_filepath_textbox";
            _record_filepath_textbox.Size = new System.Drawing.Size(144, 20);
            _record_filepath_textbox.TabIndex = 1;
            _record_filepath_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            _record_filepath_textbox.WordWrap = false;
            // 
            // record_browse
            // 
            _record_browse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _record_browse.Location = new System.Drawing.Point(148, 18);
            _record_browse.Margin = new System.Windows.Forms.Padding(0, 18, 3, 3);
            _record_browse.Name = _controlPrefix + "_record_browse";
            _record_browse.Size = new System.Drawing.Size(29, 22);
            _record_browse.TabIndex = 2;
            _record_browse.Text = "...";
            _record_browse.UseVisualStyleBackColor = true;
            // 
            // record_startRecord
            // 
            _record_startRecord.Enabled = false;
            _record_startRecord.Location = new System.Drawing.Point(183, 11);
            _record_startRecord.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _record_startRecord.Name = _controlPrefix + "_record_startRecord";
            _record_startRecord.Size = new System.Drawing.Size(45, 35);
            _record_startRecord.TabIndex = 3;
            _record_startRecord.Text = "Start record";
            _record_startRecord.UseVisualStyleBackColor = true;
            // 
            // record_stopRecord
            // 
            _record_stopRecord.Enabled = false;
            _record_stopRecord.Location = new System.Drawing.Point(183, 11);
            _record_stopRecord.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _record_stopRecord.Name = _controlPrefix + "_record_stopRecord";
            _record_stopRecord.Size = new System.Drawing.Size(45, 35);
            _record_stopRecord.TabIndex = 4;
            _record_stopRecord.Text = "Stop record";
            _record_stopRecord.UseVisualStyleBackColor = true;
            _record_stopRecord.Visible = false;


            ////////////////////////////////////
            //       Advanced
            ////////////////////////////////////
            // 
            // advanced
            // 
            _advanced.Controls.Add(_advanced_flow);
            _advanced.Location = new System.Drawing.Point(948, 0);
            _advanced.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _advanced.Name = _controlPrefix + "_advanced";
            _advanced.Size = new System.Drawing.Size(984, 66);
            _advanced.TabIndex = 4;
            _advanced.TabStop = false;
            _advanced.Text = "Advanced";
            // 
            // advanced_flow
            // 
            _advanced_flow.Controls.Add(_advanced_show);
            _advanced_flow.Controls.Add(_advanced_hiddenFlow);
            _advanced_flow.Location = new System.Drawing.Point(3, 13);
            _advanced_flow.Name = _controlPrefix + "_advanced_flow";
            _advanced_flow.Size = new System.Drawing.Size(975, 52);
            _advanced_flow.TabIndex = 1;
            // 
            // _advanced_hiddenFlow
            // 
            _advanced_hiddenFlow.Controls.Add(_advanced_hide);
            _advanced_hiddenFlow.Controls.Add(_advanced_blinkLED);
            _advanced_hiddenFlow.Controls.Add(_advanced_thermocouple);
            _advanced_hiddenFlow.Controls.Add(_advanced_port);
            _advanced_hiddenFlow.Controls.Add(_advanced_ID);
            _advanced_hiddenFlow.Controls.Add(_advanced_removeSandwich);
            _advanced_hiddenFlow.Controls.Add(_advanced_PID_radius);
            _advanced_hiddenFlow.Controls.Add(_advanced_PID_proportional);
            _advanced_hiddenFlow.Controls.Add(_advanced_PID_integral);
            _advanced_hiddenFlow.Controls.Add(_advanced_error);
            _advanced_hiddenFlow.Location = new System.Drawing.Point(0, 0);
            _advanced_hiddenFlow.Margin = new System.Windows.Forms.Padding(0);
            _advanced_hiddenFlow.Name = "_advanced_hiddenFlow";
            _advanced_hiddenFlow.Size = new System.Drawing.Size(975, 52);
            _advanced_hiddenFlow.TabIndex = 2; ;
            _advanced_hiddenFlow.Visible = false;
            // 
            // advanced_show
            // 
            _advanced_show.Location = new System.Drawing.Point(3, 11);
            _advanced_show.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _advanced_show.Name = _controlPrefix + "_advanced_show";
            _advanced_show.Size = new System.Drawing.Size(45, 35);
            _advanced_show.TabIndex = 1;
            _advanced_show.Text = "Show";
            _advanced_show.UseVisualStyleBackColor = true;
            // 
            // advanced_hide
            // 
            _advanced_hide.Location = new System.Drawing.Point(54, 11);
            _advanced_hide.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _advanced_hide.Name = _controlPrefix + "_advanced_hide";
            _advanced_hide.Size = new System.Drawing.Size(45, 35);
            _advanced_hide.TabIndex = 2;
            _advanced_hide.Text = "Hide";
            _advanced_hide.UseVisualStyleBackColor = true;
            // 
            // advanced_blinkLED
            // 
            _advanced_blinkLED.Location = new System.Drawing.Point(105, 11);
            _advanced_blinkLED.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _advanced_blinkLED.Name = _controlPrefix + "_advanced_blinkLED";
            _advanced_blinkLED.Size = new System.Drawing.Size(45, 35);
            _advanced_blinkLED.TabIndex = 3;
            _advanced_blinkLED.Text = "Blink LED";
            _advanced_blinkLED.UseVisualStyleBackColor = true;
            // 
            // advanced_thermocouple
            // 
            _advanced_thermocouple.Controls.Add(_advanced_thermocouple_label);
            _advanced_thermocouple.Controls.Add(_advanced_thermocouple_dropdown);
            _advanced_thermocouple.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_thermocouple.Location = new System.Drawing.Point(153, 0);
            _advanced_thermocouple.Margin = new System.Windows.Forms.Padding(0);
            _advanced_thermocouple.Name = _controlPrefix + "_advanced_thermocouple";
            _advanced_thermocouple.Size = new System.Drawing.Size(84, 49);
            _advanced_thermocouple.TabIndex = 23;
            // 
            // advanced_thermocouple_label
            // 
            _advanced_thermocouple_label.AutoSize = true;
            _advanced_thermocouple_label.Location = new System.Drawing.Point(3, 3);
            _advanced_thermocouple_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_thermocouple_label.Name = _controlPrefix + "_advanced_thermocouple_label";
            _advanced_thermocouple_label.Size = new System.Drawing.Size(75, 13);
            _advanced_thermocouple_label.TabIndex = 18;
            _advanced_thermocouple_label.Text = "Thermocouple\r\n";
            // 
            // advanced_thermocouple_dropdown
            // 
            _advanced_thermocouple_dropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _advanced_thermocouple_dropdown.FormattingEnabled = true;
            _advanced_thermocouple_dropdown.Items.AddRange(new object[] {
            "B",
            "E",
            "J",
            "K",
            "N",
            "R",
            "S",
            "T"});
            _advanced_thermocouple_dropdown.Location = new System.Drawing.Point(3, 19);
            _advanced_thermocouple_dropdown.Name = _controlPrefix + "_advanced_thermocouple_dropdown";
            _advanced_thermocouple_dropdown.Size = new System.Drawing.Size(75, 21);
            _advanced_thermocouple_dropdown.SelectedValue = "T"; // This sets the default value of the thermocouple type drop down list to T...
            _advanced_thermocouple_dropdown.SelectedItem = "T"; // ...and this makes it display the value of T
            _advanced_thermocouple_dropdown.TabIndex = 4;
            _advanced_thermocouple_dropdown.Tag = "";
            // 
            // advanced_port
            // 
            _advanced_port.Controls.Add(_advanced_port_label);
            _advanced_port.Controls.Add(_advanced_port_dropdown);
            _advanced_port.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_port.Location = new System.Drawing.Point(237, 0);
            _advanced_port.Margin = new System.Windows.Forms.Padding(0);
            _advanced_port.Name = _controlPrefix + "_advanced_port";
            _advanced_port.Size = new System.Drawing.Size(64, 49);
            _advanced_port.TabIndex = 24;
            // 
            // advanced_port_label
            // 
            _advanced_port_label.AutoSize = true;
            _advanced_port_label.Location = new System.Drawing.Point(3, 3);
            _advanced_port_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_port_label.Name = _controlPrefix + "_advanced_port_label";
            _advanced_port_label.Size = new System.Drawing.Size(26, 13);
            _advanced_port_label.TabIndex = 19;
            _advanced_port_label.Text = "Port";
            // 
            // advanced_port_dropdown
            // 
            _advanced_port_dropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _advanced_port_dropdown.FormattingEnabled = true;
            _advanced_port_dropdown.Location = new System.Drawing.Point(3, 19);
            _advanced_port_dropdown.Name = _controlPrefix + "_advanced_port_dropdown";
            _advanced_port_dropdown.Items.AddRange(unoccupiedCOMPorts.ToArray());
            _advanced_port_dropdown.Items.Insert(0, "None");
            _advanced_port_dropdown.SelectedValue = "None"; // This sets the default value to be none...
            _advanced_port_dropdown.SelectedItem = "None"; // ...and this makes it display the value of none
            _COMPort = "None";
            _advanced_port_dropdown.Size = new System.Drawing.Size(55, 21);
            _advanced_port_dropdown.TabIndex = 5;
            // 
            // advanced_ID
            // 
            _advanced_ID.Controls.Add(_advanced_ID_label);
            _advanced_ID.Controls.Add(_advanced_ID_upDown);
            _advanced_ID.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_ID.Location = new System.Drawing.Point(301, 0);
            _advanced_ID.Margin = new System.Windows.Forms.Padding(0);
            _advanced_ID.Name = _controlPrefix + "_advanced_ID";
            _advanced_ID.Size = new System.Drawing.Size(40, 49);
            _advanced_ID.TabIndex = 25;
            // 
            // advanced_ID_label
            // 
            _advanced_ID_label.AutoSize = true;
            _advanced_ID_label.Location = new System.Drawing.Point(3, 3);
            _advanced_ID_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_ID_label.Name = _controlPrefix + "_advanced_ID_label";
            _advanced_ID_label.Size = new System.Drawing.Size(18, 13);
            _advanced_ID_label.TabIndex = 21;
            _advanced_ID_label.Text = "ID";
            // 
            // advanced_ID_upDown
            // 
            _advanced_ID_upDown.Location = new System.Drawing.Point(3, 19);
            _advanced_ID_upDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            _advanced_ID_upDown.Name = _controlPrefix + "_advanced_ID_upDown";
            _advanced_ID_upDown.Size = new System.Drawing.Size(37, 20);
            _advanced_ID_upDown.Value = 0;
            _advanced_ID_upDown.TabIndex = 6;
            // 
            // _advanced_removeSandwich
            //
            _advanced_removeSandwich.Location = new System.Drawing.Point(293, 11);
            _advanced_removeSandwich.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            _advanced_removeSandwich.Name = _controlPrefix + "_advanced_removeSandwich";
            _advanced_removeSandwich.Size = new System.Drawing.Size(60, 35);
            _advanced_removeSandwich.TabIndex = 7;
            _advanced_removeSandwich.Text = "Delete sandwich";
            _advanced_removeSandwich.UseVisualStyleBackColor = true;
            // 
            // advanced_PID_radius
            // 
            _advanced_PID_radius.Controls.Add(_advanced_PID_radius_label);
            _advanced_PID_radius.Controls.Add(_advanced_PID_radius_upDown);
            _advanced_PID_radius.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_PID_radius.Location = new System.Drawing.Point(0, 0);
            _advanced_PID_radius.Margin = new System.Windows.Forms.Padding(0);
            _advanced_PID_radius.Name = _controlPrefix + "_advanced_PID_radius";
            _advanced_PID_radius.Size = new System.Drawing.Size(63, 49);
            _advanced_PID_radius.TabIndex = 8;
            // 
            // advanced_PID_radius_label
            // 
            _advanced_PID_radius_label.AutoSize = true;
            _advanced_PID_radius_label.Location = new System.Drawing.Point(3, 3);
            _advanced_PID_radius_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_PID_radius_label.Name = _controlPrefix + "_advanced_PID_radius_label";
            _advanced_PID_radius_label.Size = new System.Drawing.Size(60, 13);
            _advanced_PID_radius_label.TabIndex = 8;
            _advanced_PID_radius_label.Text = "PID radius";
            // 
            // advanced_PID_radius_upDown
            // 
            _advanced_PID_radius_upDown.DecimalPlaces = 2;
            _advanced_PID_radius_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _advanced_PID_radius_upDown.Location = new System.Drawing.Point(3, 19);
            _advanced_PID_radius_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _advanced_PID_radius_upDown.Minimum = 0;
            _advanced_PID_radius_upDown.Maximum = 99.99M;
            _advanced_PID_radius_upDown.Name = _controlPrefix + "_advanced_PID_radius_upDown";
            _advanced_PID_radius_upDown.Size = new System.Drawing.Size(60, 26);
            _advanced_PID_radius_upDown.TabIndex = 8;
            _advanced_PID_radius_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // advanced_PID_proportional
            // 
            _advanced_PID_proportional.Controls.Add(_advanced_PID_proportional_label);
            _advanced_PID_proportional.Controls.Add(_advanced_PID_proportional_upDown);
            _advanced_PID_proportional.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_PID_proportional.Location = new System.Drawing.Point(0, 0);
            _advanced_PID_proportional.Margin = new System.Windows.Forms.Padding(0);
            _advanced_PID_proportional.Name = _controlPrefix + "_advanced_PID_proportional";
            _advanced_PID_proportional.Size = new System.Drawing.Size(80, 49);
            _advanced_PID_proportional.TabIndex = 9;
            // 
            // advanced_PID_proportional_label
            // 
            _advanced_PID_proportional_label.AutoSize = true;
            _advanced_PID_proportional_label.Location = new System.Drawing.Point(3, 3);
            _advanced_PID_proportional_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_PID_proportional_label.Name = _controlPrefix + "_advanced_PID_proportional_label";
            _advanced_PID_proportional_label.Size = new System.Drawing.Size(66, 13);
            _advanced_PID_proportional_label.TabIndex = 9;
            _advanced_PID_proportional_label.Text = "Proportional";
            // 
            // advanced_PID_proportional_upDown
            // 
            _advanced_PID_proportional_upDown.DecimalPlaces = 2;
            _advanced_PID_proportional_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _advanced_PID_proportional_upDown.Location = new System.Drawing.Point(3, 19);
            _advanced_PID_proportional_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _advanced_PID_proportional_upDown.Minimum = 0;
            _advanced_PID_proportional_upDown.Maximum = 9999.99M;
            _advanced_PID_proportional_upDown.Name = _controlPrefix + "_advanced_PID_proportional_upDown";
            _advanced_PID_proportional_upDown.Size = new System.Drawing.Size(77, 26);
            _advanced_PID_proportional_upDown.TabIndex = 9;
            _advanced_PID_proportional_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // advanced_PID_integral
            // 
            _advanced_PID_integral.Controls.Add(_advanced_PID_integral_label);
            _advanced_PID_integral.Controls.Add(_advanced_PID_integral_upDown);
            _advanced_PID_integral.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_PID_integral.Location = new System.Drawing.Point(0, 0);
            _advanced_PID_integral.Margin = new System.Windows.Forms.Padding(0);
            _advanced_PID_integral.Name = _controlPrefix + "_advanced_PID_integral";
            _advanced_PID_integral.Size = new System.Drawing.Size(80, 49);
            _advanced_PID_integral.TabIndex = 10;
            // 
            // advanced_PID_integral_label
            // 
            _advanced_PID_integral_label.AutoSize = true;
            _advanced_PID_integral_label.Location = new System.Drawing.Point(3, 3);
            _advanced_PID_integral_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_PID_integral_label.Name = _controlPrefix + "_advanced_PID_integral_label";
            _advanced_PID_integral_label.Size = new System.Drawing.Size(66, 13);
            _advanced_PID_integral_label.TabIndex = 10;
            _advanced_PID_integral_label.Text = "Integral";
            // 
            // advanced_PID_integral_upDown
            // 
            _advanced_PID_integral_upDown.DecimalPlaces = 2;
            _advanced_PID_integral_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _advanced_PID_integral_upDown.Location = new System.Drawing.Point(3, 19);
            _advanced_PID_integral_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            _advanced_PID_integral_upDown.Minimum = 0;
            _advanced_PID_integral_upDown.Maximum = 9999.99M;
            _advanced_PID_integral_upDown.Name = _controlPrefix + "_advanced_PID_integral_upDown";
            _advanced_PID_integral_upDown.Size = new System.Drawing.Size(77, 26);
            _advanced_PID_integral_upDown.TabIndex = 10;
            _advanced_PID_integral_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // advanced_error
            // 
            _advanced_error.Controls.Add(_advanced_error_label);
            _advanced_error.Controls.Add(_advanced_error_textbox);
            _advanced_error.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _advanced_error.Location = new System.Drawing.Point(356, 0);
            _advanced_error.Margin = new System.Windows.Forms.Padding(0);
            _advanced_error.Name = _controlPrefix + "_advanced_error";
            _advanced_error.Size = new System.Drawing.Size(396, 49);
            _advanced_error.TabIndex = 11;
            // 
            // advanced_error_label
            // 
            _advanced_error_label.AutoSize = true;
            _advanced_error_label.Location = new System.Drawing.Point(3, 3);
            _advanced_error_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            _advanced_error_label.Name = _controlPrefix + "_advanced_error_label";
            _advanced_error_label.Size = new System.Drawing.Size(29, 13);
            _advanced_error_label.TabIndex = 11;
            _advanced_error_label.Text = "Error message";
            // 
            // advanced_error_textbox
            // 
            _advanced_error_textbox.Enabled = true;
            _advanced_error_textbox.Location = new System.Drawing.Point(3, 19);
            _advanced_error_textbox.Name = _controlPrefix + "_advanced_error_textbox";
            _advanced_error_textbox.ReadOnly = true;
            _advanced_error_textbox.Size = new System.Drawing.Size(390, 20);
            _advanced_error_textbox.TabIndex = 11;


            // 
            // Finalize the layout
            // 
            _ID.ResumeLayout(false);
            _ID_flow.ResumeLayout(false);
            _ID_flow.PerformLayout();
            _DAQ.ResumeLayout(false);
            _DAQ_flow.ResumeLayout(false);
            _DAQ_flow.PerformLayout();
            _DAQ_heater.ResumeLayout(false);
            _DAQ_heater.PerformLayout();
            _DAQ_sample.ResumeLayout(false);
            _DAQ_sample.PerformLayout();
            _heat.ResumeLayout(false);
            _heat_flow.ResumeLayout(false);
            _heat_setpoint.ResumeLayout(false);
            _heat_setpoint.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_heat_setpoint_upDown)).EndInit();
            _heat_ramp.ResumeLayout(false);
            _heat_ramp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_heat_ramp_upDown)).EndInit();
            _heat_timer.ResumeLayout(false);
            _heat_timer_flow.ResumeLayout(false);
            _heat_timer_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_heat_timer_h_upDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(_heat_timer_m_upDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(_heat_timer_s_upDown)).EndInit();
            _record.ResumeLayout(false);
            _record_flow.ResumeLayout(false);
            _record_filepath.ResumeLayout(false);
            _record_filepath.PerformLayout();
            _advanced.ResumeLayout(false);
            _advanced_flow.ResumeLayout(false);
            _advanced_hiddenFlow.ResumeLayout(false);
            _advanced_thermocouple.ResumeLayout(false);
            _advanced_thermocouple.PerformLayout();
            _advanced_port.ResumeLayout(false);
            _advanced_port.PerformLayout();
            _advanced_ID.ResumeLayout(false);
            _advanced_ID.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_ID_upDown)).EndInit();
            _advanced_PID_radius.ResumeLayout(false);
            _advanced_PID_radius.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_PID_radius_upDown)).EndInit();
            _advanced_PID_proportional.ResumeLayout(false);
            _advanced_PID_proportional.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_PID_proportional_upDown)).EndInit();
            _advanced_PID_integral.ResumeLayout(false);
            _advanced_PID_integral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_advanced_PID_integral_upDown)).EndInit();
            _advanced_error.ResumeLayout(false);
            _advanced_error.PerformLayout();
            _sandwich.ResumeLayout(false);
            _sandwich.PerformLayout();
            _mainFlowPanel.ResumeLayout(false);
            _mainFlowPanel.PerformLayout();
            _owningForm.ResumeLayout(false);
            _owningForm.PerformLayout();

            //
            // Assign event handlers
            //
            _DAQ_startDAQ.Click +=                          _startDAQ_click;
            _DAQ_stopDAQ.Click +=                           _stopDAQ_click;
            _DAQ_readSample.CheckedChanged +=               _sample_checkedChanged;
            _heat_startHeat.Click +=                        _startHeat_click;
            _heat_stopHeat.Click +=                         _stopHeat_click;
            _record_filepath_textbox.TextChanged +=         _recordFilePath_change;
            _record_browse.Click +=                         _recordBrowse_click;
            _record_startRecord.Click +=                    _startRecord_click;
            _record_stopRecord.Click +=                     _stopRecord_click;
            _advanced_ID_upDown.ValueChanged +=             _updateSandwichID;
            _advanced_ID_upDown.Leave +=                    _updateSandwichID;
            _advanced_show.Click +=                         _showAdvancedHiddenPanel_click;
            _advanced_hide.Click +=                         _hideAdvancedHiddenPanel_click;
            _advanced_blinkLED.Click +=                     _blinkLED_click;
            _advanced_port_dropdown.SelectedValueChanged += _portValueChanged;
            _advanced_removeSandwich.Click +=               deleteSandwich;


            //
            // Initialize other variables
            //
            _DAQActive = false;
            _DAQSample = false;
            _recordActive = false;
            _recordStart = false;
            _recordStop = false;
            _heatActive = false;
            _heatStart = false;
            _heatStop = false;
            _savePathway = ""; // TODO: allow user to choose location and name of file
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Functions for interfacing with external callers
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void updateCOMPortList(List<string> unoccupiedCOMPorts)
        {
            List<string> dummyUnoccupiedPorts = unoccupiedCOMPorts;
            _advanced_port_dropdown.Items.Clear();
            _advanced_port_dropdown.Items.AddRange(unoccupiedCOMPorts.ToArray());
            _advanced_port_dropdown.Items.Insert(0, _COMPort);
            if (_COMPort != "None")
            {
                _advanced_port_dropdown.Items.Insert(0, "None");
            }
            _advanced_port_dropdown.SelectedValueChanged -= _portValueChanged;
            _advanced_port_dropdown.SelectedValue = _COMPort;
            _advanced_port_dropdown.SelectedItem = _COMPort;
            _advanced_port_dropdown.SelectedValueChanged += _portValueChanged;
        }

        public bool getDAQStatus()
        {
            return _DAQActive;
        }

        public int getSandwichID()
        {
            return _sandwichID;
        }

        public string getCOMPort()
        {
            return _COMPort;
        }

        public void setCOMPort(string COMPort)
        { // These will trigger the _portValueChanged event handler which will cause an update of COM port list for all sandwiches
            _advanced_port_dropdown.SelectedValue = COMPort;
            _advanced_port_dropdown.SelectedItem = COMPort;
        }

        public void clearError()
        {
            _advanced_error_textbox.Text = "";
        }

        public static int getBaudRate()
        {
            return _baudRate;
        }

        public static Parity getParity()
        {
            return _parity;
        }

        public static int getDataBits()
        {
            return _dataBits;
        }

        public static StopBits getStopBits()
        {
            return _stopBits;
        }

        public string getConfiguration()
        {
            string configuration = "";
            configuration += Convert.ToString(_sandwichID);
            configuration += ",";
            configuration += Convert.ToString(_DAQ_readSample.Checked); // This will write "TRUE" if true, "FALSE" if false.
            configuration += ",";
            configuration += Convert.ToString(_heat_setpoint_upDown.Value);
            configuration += ",";
            configuration += Convert.ToString(_heat_ramp_upDown.Value);
            configuration += ",";
            configuration += Convert.ToString(_heat_timer_h_upDown.Value);
            configuration += ",";
            configuration += Convert.ToString(_heat_timer_m_upDown.Value);
            configuration += ",";
            configuration += Convert.ToString(_heat_timer_s_upDown.Value);
            configuration += ",";
            configuration += _record_filepath_textbox.Text;
            configuration += ",";
            configuration += Convert.ToString(_advanced_thermocouple_dropdown.SelectedItem);
            configuration += ",";
            configuration += Convert.ToString(_advanced_PID_radius_upDown.Value);
            configuration += ",";
            configuration += Convert.ToString(_advanced_PID_proportional_upDown.Value);
            configuration += ",";
            configuration += Convert.ToString(_advanced_PID_integral_upDown.Value);
            return configuration;
        }

        public void setConfiguration(string configuration)
        {
            configuration = configuration.TrimEnd('\n');
            string[] configurationParts = configuration.Split(',');

            _advanced_ID_upDown.Value =                     Convert.ToDecimal(configurationParts[0]);
            _DAQ_readSample.Checked =                       Convert.ToBoolean(configurationParts[1]);
            _heat_setpoint_upDown.Value =                   Convert.ToDecimal(configurationParts[2]);
            _heat_ramp_upDown.Value =                       Convert.ToDecimal(configurationParts[3]);
            _heat_timer_h_upDown.Value =                    Convert.ToDecimal(configurationParts[4]);
            _heat_timer_m_upDown.Value =                    Convert.ToDecimal(configurationParts[5]);
            _heat_timer_s_upDown.Value =                    Convert.ToDecimal(configurationParts[6]);
            _record_filepath_textbox.Text =                 configurationParts[7];
            _savePathway = _record_filepath_textbox.Text;   // Just following the event that is triggered when the record file path is changed
            _advanced_thermocouple_dropdown.SelectedItem =  configurationParts[8];
            _advanced_thermocouple_dropdown.SelectedValue = configurationParts[8];
            _advanced_PID_radius_upDown.Value =             Convert.ToDecimal(configurationParts[9]);
            _advanced_PID_proportional_upDown.Value =       Convert.ToDecimal(configurationParts[10]);
            _advanced_PID_integral_upDown.Value =           Convert.ToDecimal(configurationParts[11]);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Events from buttons
        //
        // Button clicks either make a short update to GUI or create a thread for heavy/recursive operations
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        private void _startDAQ_click(object sender, EventArgs e)
        {
            _changeControlEnable(_DAQ_startDAQ, false);
            _changeControlEnable(_advanced_removeSandwich, false);
            _changeControlEnable(_DAQ_readSample, false);
            _changeControlEnable(_advanced_blinkLED, false);
            _readThread = new Thread(_acquireData);
            _readThread.Start();
        }

        private void _stopDAQ_click(object sender, EventArgs e)
        {
            _changeControlEnable(_DAQ_stopDAQ, false);
            _changeControlEnable(_record_startRecord, false);
            _changeControlEnable(_record_stopRecord, false);
            _changeControlEnable(_heat_startHeat, false);
            _changeControlEnable(_heat_stopHeat, false);
            _closeThread = new Thread(_stopAcquiringData);
            _closeThread.Start();
        }

        private void _sample_checkedChanged(object sender, EventArgs e)
        {
            if (_DAQ_readSample.Checked)
            {
                _DAQSample = true;
                _changeControlEnable(_DAQ_sample_label, true);
                _changeControlEnable(_DAQ_sample_textbox, true);
            }
            else
            {
                _DAQSample = false;
                _changeControlEnable(_DAQ_sample_label, false);
                _changeControlEnable(_DAQ_sample_textbox, false);
            }
        }

        private void _startHeat_click(object sender, EventArgs e)
        {
            _changeControlEnable(_heat_startHeat, false);
            _changeControlEnable(_DAQ_stopDAQ, false);
            _changeControlEnable(_heat_setpoint_upDown, false);
            _changeControlEnable(_heat_ramp_upDown, false);
            _changeControlEnable(_heat_timer_h_upDown, false);
            _changeControlEnable(_heat_timer_m_upDown, false);
            _changeControlEnable(_heat_timer_s_upDown, false);
            _heatStart = true; // this should be before heatActive is changed to prevent potential complications due to multithreading
            _heatActive = true;
        }

        private void _stopHeat_click(object sender, EventArgs e)
        {
            _changeControlEnable(_heat_stopHeat, false);
            _heatStop = true;
        }

        private void _recordFilePath_change(object sender, EventArgs e)
        {
            _savePathway = _record_filepath_textbox.Text;
        }

        private void _recordBrowse_click(object sender, EventArgs e)
        {
            if (_owningForm.recordFileDialog.ShowDialog() == DialogResult.OK)
            {
                _record_filepath_textbox.Text = _owningForm.recordFileDialog.FileName;
                _savePathway = _record_filepath_textbox.Text;
            }
        }

        private void _startRecord_click(object sender, EventArgs e)
        {
            _changeControlEnable(_record_startRecord, false);
            _recordStart = true;
            _recordStop = false;
            _recordActive = true;
        }

        private void _stopRecord_click(object sender, EventArgs e)
        {
            _changeControlEnable(_record_stopRecord, false);
            _recordStop = true;
            _recordActive = false;
        }

        private void _showAdvancedHiddenPanel_click(object sender, EventArgs e)
        {
            _suspendLayoutControl(_advanced_flow);
            _hideControl(_advanced_show, _advanced_flow);
            _changeFlowVisibility(_advanced_hiddenFlow, true);
            _resumeLayoutControl(_advanced_flow);
        }

        private void _hideAdvancedHiddenPanel_click(object sender, EventArgs e)
        {
            _suspendLayoutControl(_advanced_flow);
            _showControl(_advanced_show, _advanced_flow);
            _changeFlowVisibility(_advanced_hiddenFlow, false);
            _resumeLayoutControl(_advanced_flow);
        }

        private void _blinkLED_click(object sender, EventArgs e)
        {
            try
            {
                _port = new SerialPort(_getComboSelectedItem(_advanced_port_dropdown), _baudRate, _parity, _dataBits, _stopBits);
                _port.Handshake = Handshake.None;
                _port.Encoding = System.Text.Encoding.ASCII;
                _port.ReadTimeout = 2000; // 2000 ms before timeout.
                _port.Open();
                _sendCommand("l");
                _port.Close();
            }
            catch (Exception err)
            { // TODO: I'm just being lazy here in catching the diff exceptions.. by right should handle the diff exceptions under the 1xx error code as done in the _acquiredata function
                _handleError(999, err.Message);
            }
        }

        private void _updateSandwichID(object sender, EventArgs e)
        {
            _ID_textbox.Text = Convert.ToString(_advanced_ID_upDown.Value);
            _sandwichID = Convert.ToInt32(_advanced_ID_upDown.Value);
        }

        private void _portValueChanged(object sender, EventArgs e)
        {
            _COMPort = _advanced_port_dropdown.SelectedItem.ToString();
            _owningForm.updateOccupiedPort(_sandwichControlID, _COMPort);
            _owningForm.refreshPortList();
        }

        public void deleteSandwich(object sender, EventArgs e)
        {
            _deleteControl(_sandwich);
            _owningForm.exileSandwich(this);
            _mainFlowPanel.Size = new System.Drawing.Size(_mainFlowPanel.Size.Width, _mainFlowPanel.Size.Height - _sandwichHeight);
        }
        

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Heavy-lifting functions
        //
        // Deez bois do all the heavy duty work.
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void _acquireData()
        {
            try
            {
                _DAQActive = true;
                _recordStop = false;
                _port = new SerialPort(_getComboSelectedItem(_advanced_port_dropdown), _baudRate, _parity, _dataBits, _stopBits);
                _port.Handshake = Handshake.None;
                _port.Encoding = System.Text.Encoding.ASCII;
                _port.ReadTimeout = 2000; // 2000 ms before timeout.
                _port.Open();

                if (_DAQSample)
                {
                    _sendCommand("b" + _getComboSelectedItem(_advanced_thermocouple_dropdown));
                }
                else
                {
                    _sendCommand("g" + _getComboSelectedItem(_advanced_thermocouple_dropdown));
                }

                // Only enable the buttons once we are sure everything is ready to go
                _changeControlEnable(_record_startRecord, true);
                _changeControlEnable(_heat_startHeat, true);
                _changeControlEnable(_DAQ_stopDAQ, true);
                _suspendLayoutControl(_DAQ_flow);
                _hideControl(_DAQ_startDAQ, _DAQ_flow);
                _showControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);

                while (_DAQActive)
                {
                    _replyBuffer = _receiveReply();

                    switch (_replyBuffer.Substring(0, 1))
                    {
                        case "t":
                            _replyBuffer = _replyBuffer.TrimStart('t');
                            _readData = _replyBuffer.Split(',');
                            _heaterT = _readData[0];

                            if (_DAQSample)
                            {
                                _sampleT = _readData[2];

                            }

                            _updateTextBox(_DAQ_heater_textbox, _heaterT);
                            _updateTextBox(_DAQ_sample_textbox, _sampleT);


                            // We will rely on the time given by Arduino because the computer timer will be bogged down by the serial communication delays
                            // Arduino simply provides the current time in ms; the computer's job is to calculate the amount of time elapsed since the first given time.
                            _arduinoElapsedTime = Convert.ToUInt64(_readData[1], 10);

                            if (_countingDown)
                            {
                                // This section must be before the _heatActive code block so that when _countingDownStart is set to true in the _heatActive code block,
                                // the program is forced to wait until the next reply from Arduino before setting the _heatingStartTime. This behavior is desirable because
                                // the Arduino only starts the heating upon receiving a heat command on its next cycle.
                                if (_countingDownStart)
                                {
                                    _countingDownStart = false;
                                    _heatingStartTime = _arduinoElapsedTime;
                                }

                                decimal timeRemaining = (_heatingDuration - (_arduinoElapsedTime - _heatingStartTime)) / 1000; // seconds
                                decimal hoursRemaining = Math.Floor(timeRemaining / 3600);
                                decimal minsRemaining = Math.Floor((timeRemaining - hoursRemaining * 3600) / 60);
                                decimal secsRemaining = Math.Ceiling(timeRemaining - hoursRemaining * 3600 - minsRemaining * 60);

                                _updateControlValue(_heat_timer_h_upDown, hoursRemaining);
                                _updateControlValue(_heat_timer_m_upDown, minsRemaining);
                                _updateControlValue(_heat_timer_s_upDown, secsRemaining);
                            }

                            if (_heatActive)
                            {
                                if (_heatStart)
                                {
                                    _heatStart = false;
                                    string heatCommand = "h";
                                    heatCommand += _heat_setpoint_upDown.Value.ToString("000.0");
                                    heatCommand += _heat_ramp_upDown.Value.ToString("000.00");
                                    heatCommand += _advanced_PID_radius_upDown.Value.ToString("00.00");
                                    heatCommand += _advanced_PID_proportional_upDown.Value.ToString("0000.00");
                                    heatCommand += _advanced_PID_integral_upDown.Value.ToString("0000.00");

                                    _heatingDuration = Convert.ToUInt64(1000 * (_heat_timer_h_upDown.Value * 3600 + _heat_timer_m_upDown.Value * 60 + _heat_timer_s_upDown.Value)); // Convert to ms

                                    if (_heatingDuration > 0)
                                    {
                                        _countingDown = true;
                                        _countingDownStart = true;
                                    }
                                    else
                                    {
                                        _countingDown = false;
                                    }

                                    heatCommand += _heatingDuration.ToString("0000000000");

                                    _sendCommand(heatCommand);

                                    _hideControl(_heat_startHeat, _heat_flow);
                                    _showControl(_heat_stopHeat, _heat_flow);
                                    _changeControlEnable(_heat_stopHeat, true);
                                    _changeControlBg(_heat_flow, Color.Red);
                                }

                                if (_heatStop)
                                {
                                    _heatActive = false;
                                    _heatStop = false;
                                    _countingDown = false;
                                    _sendCommand("c");
                                    _changeControlEnable(_heat_setpoint_upDown, true);
                                    _changeControlEnable(_heat_ramp_upDown, true);
                                    _changeControlEnable(_heat_timer_h_upDown, true);
                                    _changeControlEnable(_heat_timer_m_upDown, true);
                                    _changeControlEnable(_heat_timer_s_upDown, true);
                                    _hideControl(_heat_stopHeat, _heat_flow);
                                    _showControl(_heat_startHeat, _heat_flow);
                                    _changeControlEnable(_heat_startHeat, true);
                                    _changeControlEnable(_DAQ_stopDAQ, true);
                                    _changeControlBg(_heat_flow, SystemColors.Control);
                                }
                            }

                            if (_recordActive)
                            {
                                if (_recordStart)
                                {
                                    _recordStart = false;

                                    try
                                    {
                                        _recordStream = new FileStream(_savePathway, FileMode.Append, FileAccess.Write, FileShare.None); // append or create new file; write-only; don't allow anyone else to open this file
                                        _CSVWriter = new StreamWriter(_recordStream); // default encoding is UTF-8
                                        _legitFilePath = true;
                                    }
                                    catch (Exception e)
                                    {
                                        _handleError(205, e.Message);
                                        _legitFilePath = false;
                                        _recordActive = false;
                                        _changeControlEnable(_record_startRecord, true);
                                        _updateTextBox(_record_filepath_textbox, "WARNING: Invalid file path");
                                    }

                                    if (_legitFilePath)
                                    {
                                        _changeControlEnable(_record_stopRecord, true); // Enable user to stop recording only after we are sure the recording code block is already running
                                        _suspendLayoutControl(_record_flow);
                                        _hideControl(_record_startRecord, _record_flow);
                                        _showControl(_record_stopRecord, _record_flow);
                                        _resumeLayoutControl(_record_flow);

                                        // For first writing, force the time to be 0.
                                        _recordingElapsedTime = 0;
                                        _recordingStartTime = _arduinoElapsedTime;
                                    }
                                }
                                else
                                {
                                    // Remember, time is given in ms.
                                    _recordingElapsedTime = _arduinoElapsedTime - _recordingStartTime;
                                }

                                if (_legitFilePath)
                                {
                                    try
                                    {
                                        if (_DAQSample)
                                        {
                                            _CSVWriter.Write("{0}{1}{2}{3}{4}{5}", new Object[] { _recordingElapsedTime, ",", _heaterT, ",", _sampleT, "\n" });
                                        }
                                        else
                                        {
                                            _CSVWriter.Write("{0}{1}{2}{3}", new Object[] { _recordingElapsedTime, ",", _heaterT, "\n" });
                                        }
                                    }
                                    catch (ArgumentNullException)
                                    {
                                        _handleError(201, "");
                                    }
                                    catch (ObjectDisposedException)
                                    {
                                        _handleError(202, "");
                                    }
                                    catch (FormatException)
                                    {
                                        _handleError(203, "");
                                    }
                                    catch (IOException)
                                    {
                                        _handleError(204, "");
                                    }


                                    // If user request to stop while the thread is in middle of writing to file, this runs.
                                    if (_recordStop)
                                    {
                                        try
                                        {
                                            _CSVWriter.Flush();
                                            _CSVWriter.Close();
                                        }
                                        catch (ObjectDisposedException)
                                        {
                                            _handleError(202, "");
                                        }
                                        catch (IOException)
                                        {
                                            _handleError(204, "");
                                        }
                                        catch (EncoderFallbackException)
                                        {
                                            _handleError(200, "");
                                        }

                                        _recordStop = false;
                                        _changeControlEnable(_record_startRecord, true); // re-enable the start recording button
                                        _suspendLayoutControl(_record_flow);
                                        _showControl(_record_startRecord, _record_flow);
                                        _hideControl(_record_stopRecord, _record_flow);
                                        _resumeLayoutControl(_record_flow);
                                    }
                                }
                            }
                            else if (_recordStop) // If user request to stop when the thread is at before or after the _recordActive if-code block, this runs
                            {
                                try
                                {
                                    _CSVWriter.Flush();
                                    _CSVWriter.Close();
                                }
                                catch (ObjectDisposedException)
                                {
                                    _handleError(202, "");
                                }
                                catch (IOException)
                                {
                                    _handleError(204, "");
                                }
                                catch (EncoderFallbackException)
                                {
                                    _handleError(200, "");
                                }

                                _recordStop = false;
                                _changeControlEnable(_record_startRecord, true); // re-enable the start recording button
                                _suspendLayoutControl(_record_flow);
                                _showControl(_record_startRecord, _record_flow);
                                _hideControl(_record_stopRecord, _record_flow);
                                _resumeLayoutControl(_record_flow);
                            }


                            break;
                        case "d": // The countdown in the Arduino for heating is done. Heating has been completed
                            if (_countingDown)
                            {
                                _countingDown = false;
                                _heatActive = false;

                                // Reset the timers to 0
                                _updateControlValue(_heat_timer_h_upDown, 0);
                                _updateControlValue(_heat_timer_m_upDown, 0);
                                _updateControlValue(_heat_timer_s_upDown, 0);

                                _changeControlEnable(_heat_setpoint_upDown, true);
                                _changeControlEnable(_heat_ramp_upDown, true);
                                _changeControlEnable(_heat_timer_h_upDown, true);
                                _changeControlEnable(_heat_timer_m_upDown, true);
                                _changeControlEnable(_heat_timer_s_upDown, true);
                                _hideControl(_heat_stopHeat, _heat_flow);
                                _showControl(_heat_startHeat, _heat_flow);
                                _changeControlEnable(_heat_stopHeat, false);
                                _changeControlEnable(_heat_startHeat, true);
                                _changeControlEnable(_DAQ_stopDAQ, true);
                                _changeControlBg(_heat_flow, SystemColors.Control);

                                // This is a workaround to a strange behavior of the Arduino freezing up once the countdown is complete. If the serial monitor
                                // of the Arduino IDE is used, the Arduino continues to transmit temperature data after countdown is complete.
                                // However, the Arduino would just stop transmitting abruptly after countdown when using this C# program.
                                // This workaround basically forces the Arduino to restart DAQ
                                _sendCommand("s");
                                if (_DAQSample)
                                {
                                    _sendCommand("b" + _getComboSelectedItem(_advanced_thermocouple_dropdown));
                                }
                                else
                                {
                                    _sendCommand("g" + _getComboSelectedItem(_advanced_thermocouple_dropdown));
                                }
                            }
                            break;
                        case "e":
                            _handleError(Convert.ToInt32(_replyBuffer.Substring(1)), "");
                            break;
                        default:
                            // Unknown response type obtained
                            _handleError(999, _replyBuffer);
                            break;
                    }
                }

                // From here on, it is code for stopping the DAQ because the user requested to stop

                if (_recordActive) // If user request to stop DAQ and we were halfway recording, stop it and flush
                {
                    _recordActive = false;
                    _recordStop = false; // Reset to original value
                    try
                    {
                        _CSVWriter.Flush();
                        _CSVWriter.Close();
                    }
                    catch (ObjectDisposedException)
                    {
                        _handleError(202, "");
                    }
                    catch (IOException)
                    {
                        _handleError(204, "");
                    }
                    catch (EncoderFallbackException)
                    {
                        _handleError(200, "");
                    }
                    
                    _changeControlEnable(_record_startRecord, false); // keep the start recording button disabled since DAQ is stopped
                    _changeControlEnable(_record_stopRecord, false);
                    _suspendLayoutControl(_record_flow);
                    _showControl(_record_startRecord, _record_flow);
                    _hideControl(_record_stopRecord, _record_flow);
                    _resumeLayoutControl(_record_flow);
                }

                // We got out of the recordActive loop indicating that DAQActive was set to false, indicating user wants to stop acquiring T data. Tell Arduino to stop.
                _sendCommand("s");
                _changeControlEnable(_DAQ_startDAQ, true); // re-enable the start DAQ button
                _changeControlEnable(_advanced_removeSandwich, true);
                _changeControlEnable(_DAQ_readSample, true);
                _changeControlEnable(_advanced_blinkLED, true);
                _suspendLayoutControl(_DAQ_flow);
                _showControl(_DAQ_startDAQ, _DAQ_flow);
                _hideControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);
            }
            catch (UnauthorizedAccessException)
            {
                _handleError(100, "");
                _changeControlEnable(_DAQ_startDAQ, true);
                _changeControlEnable(_DAQ_stopDAQ, false);
                _changeControlEnable(_advanced_removeSandwich, true);
                _changeControlEnable(_DAQ_readSample, true);
                _changeControlEnable(_advanced_blinkLED, true);
                _suspendLayoutControl(_DAQ_flow);
                _showControl(_DAQ_startDAQ, _DAQ_flow);
                _hideControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);
            }
            catch (ArgumentOutOfRangeException)
            {
                _handleError(101, "");
                _changeControlEnable(_DAQ_startDAQ, true);
                _changeControlEnable(_DAQ_stopDAQ, false);
                _changeControlEnable(_advanced_removeSandwich, true);
                _changeControlEnable(_DAQ_readSample, true);
                _changeControlEnable(_advanced_blinkLED, true);
                _suspendLayoutControl(_DAQ_flow);
                _showControl(_DAQ_startDAQ, _DAQ_flow);
                _hideControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);
            }
            catch (ArgumentException)
            {
                _handleError(102, "");
                _changeControlEnable(_DAQ_startDAQ, true);
                _changeControlEnable(_DAQ_stopDAQ, false);
                _changeControlEnable(_advanced_removeSandwich, true);
                _changeControlEnable(_DAQ_readSample, true);
                _changeControlEnable(_advanced_blinkLED, true);
                _suspendLayoutControl(_DAQ_flow);
                _showControl(_DAQ_startDAQ, _DAQ_flow);
                _hideControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);
            }
            catch (IOException)
            {
                _handleError(103, "");
                _changeControlEnable(_DAQ_startDAQ, true);
                _changeControlEnable(_DAQ_stopDAQ, false);
                _changeControlEnable(_advanced_removeSandwich, true);
                _changeControlEnable(_DAQ_readSample, true);
                _changeControlEnable(_advanced_blinkLED, true);
                _suspendLayoutControl(_DAQ_flow);
                _showControl(_DAQ_startDAQ, _DAQ_flow);
                _hideControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);
            }
            catch (InvalidOperationException)
            {
                _handleError(104, "");
                _changeControlEnable(_DAQ_startDAQ, true);
                _changeControlEnable(_DAQ_stopDAQ, false);
                _changeControlEnable(_advanced_removeSandwich, true);
                _changeControlEnable(_DAQ_readSample, true);
                _changeControlEnable(_advanced_blinkLED, true);
                _suspendLayoutControl(_DAQ_flow);
                _showControl(_DAQ_startDAQ, _DAQ_flow);
                _hideControl(_DAQ_stopDAQ, _DAQ_flow);
                _resumeLayoutControl(_DAQ_flow);
            }
            catch (Exception e)
            {
                // None of the above exceptions were caught
                _handleError(999, e.Message);
            }
        }

        private void _stopAcquiringData()
        {
            _DAQActive = false;
            _readThread.Join();
            _port.Close();
        }

        private void _handleError(int errorCode, string message)
        {
            // Error code format:
            // First digit is the type of error:
            //      0: Error coming from Arduino
            //      1: Error coming from SerialPort class (shares case 103 with StreamWriter)
            //      2: Error coming from StreamWriter class
            // The remaining 2 digits identify the specific error
            switch (errorCode)
            {
                case 000:
                    // Timeout in communication with Arduino. Note that this is purposely used in some situations e.g. Identifying the COM port associated with each Arduino
                    _errorMessage = "000: Communication timeout with Arduino.";
                    break;
                case 001:
                    // Heater - Invalid thermocouple type
                    _errorMessage = "001: Heater - Invalid thermocouple type.";
                    break;
                case 002:
                    // Unrecognized command sent to Arduino
                    _errorMessage = "002: Unrecognized command sent to Arduino.";
                    break;
                case 003:
                    // Heater - Thermocouple Open Fault
                    _errorMessage = "003: Heater - Thermocouple Open Fault.";
                    break;
                case 004:
                    // Heater - Cold Junction Range Fault
                    _errorMessage = "004: Heater - Cold Junction Range Fault.";
                    break;
                case 005:
                    // Heater - Thermocouple Range Fault
                    _errorMessage = "005: Heater - Thermocouple Range Fault.";
                    break;
                case 006:
                    // Heater - Cold Junction High Fault
                    _errorMessage = "006: Heater - Cold Junction High Fault.";
                    break;
                case 007:
                    // Heater - Cold Junction Low Fault
                    _errorMessage = "007: Heater - Cold Junction Low Fault.";
                    break;
                case 008:
                    // Heater - Thermocouple High Fault
                    _errorMessage = "008: Heater - Thermocouple High Fault.";
                    break;
                case 009:
                    // Heater - Thermocouple Low Fault
                    _errorMessage = "009: Heater - Thermocouple Low Fault.";
                    break;
                case 010:
                    // Heater - Over/Under Voltage Fault
                    _errorMessage = "010: Heater - Over/Under Voltage Fault.";
                    break;
                case 021:
                    // Sample - Invalid thermocouple type
                    _errorMessage = "021: Sample - Invalid thermocouple type.";
                    break;
                case 023:
                    // Sample - Thermocouple Open Fault
                    _errorMessage = "023: Sample - Thermocouple Open Fault.";
                    break;
                case 024:
                    // Sample - Cold Junction Range Fault
                    _errorMessage = "024: Sample - Cold Junction Range Fault.";
                    break;
                case 025:
                    // Sample - Thermocouple Range Fault
                    _errorMessage = "025: Sample - Thermocouple Range Fault.";
                    break;
                case 026:
                    // Sample - Cold Junction High Fault
                    _errorMessage = "026: Sample - Cold Junction High Fault.";
                    break;
                case 027:
                    // Sample - Cold Junction Low Fault
                    _errorMessage = "027: Sample - Cold Junction Low Fault.";
                    break;
                case 028:
                    // Sample - Thermocouple High Fault
                    _errorMessage = "028: Sample - Thermocouple High Fault.";
                    break;
                case 029:
                    // Sample - Thermocouple Low Fault
                    _errorMessage = "029: Sample - Thermocouple Low Fault.";
                    break;
                case 030:
                    // Sample - Over/Under Voltage Fault
                    _errorMessage = "030: Sample - Over/Under Voltage Fault.";
                    break;
                case 100:
                    //Access is denied to the port.
                    // -or -
                    // The current process, or another process on the system, already has the specified COM port open either by a SerialPort instance or in unmanaged code.
                    _errorMessage = "100: Access denied to port or port opened by others.";
                    break;
                case 101:
                    // One or more of the properties for this instance are invalid.
                    // For example, the Parity, DataBits, or Handshake properties are not valid values; the BaudRate is less than or equal to zero;
                    // the ReadTimeout or WriteTimeout property is less than zero and is not InfiniteTimeout. 
                    _errorMessage = "101: Invalid argument for port settings.";
                    break;
                case 102:
                    // The port name does not begin with "COM".
                    // -or -
                    // The file type of the port is not supported.
                    _errorMessage = "102: Invalid argument for port name.";
                    break;
                case 103:
                    // The port is in an invalid state.
                    // -or -
                    // An attempt to set the state of the underlying port failed. For example, the parameters passed from this SerialPort object were invalid.
                    _errorMessage = "103: Port is or is trying to be set to invalid state.";
                    break;
                case 104:
                    // The specified port on the current instance of the SerialPort is already open.
                    _errorMessage = "104: Port is already open.";
                    break;
                case 200:
                    // EncoderFallbackException	
                    // The current encoding does not support displaying half of a Unicode surrogate pair.
                    _errorMessage = "200: CSVWriter encoding has an issue.";
                    break;
                case 201:
                    // ArgumentNullException	
                    // format or arg is null.
                    _errorMessage = "201: CSVWriter format or arg is null.";
                    break;
                case 202:
                    // ObjectDisposedException
                    // The TextWriter is closed.
                    _errorMessage = "202: CSVWriter TextWriter stream is closed.";
                    break;
                case 203:
                    // FormatException	
                    // format is not a valid composite format string.
                    // - or -
                    // The index of a format item is less than 0(zero), or greater than or equal to the length of the arg array. 
                    _errorMessage = "203: CSVWriter has an invalid format string for writing.";
                    break;
                case 204:
                    // IOException
                    // CSVWriter encountered an I/O error
                    _errorMessage = "204: CSVWriter encountered an I/O error";
                    break;
                case 205:
                    // A group of exceptions thrown when trying to open the file path specified by the user
                    _errorMessage = "205: Error with file path: " + message;
                    break;
                case 999:
                    // No error code has been defined for this exception yet
                    _errorMessage = "999: Undefined exception: " + message;
                    break;
                default:
                    // Error code has not been defined in this function yet
                    _errorMessage = "998: Undefined error code: " + message;
                    break;
            }
            
            _updateTextBox(_advanced_error_textbox, _errorMessage);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Control-modifying functions
        //
        // Attempts to modify a control in a thread separate from the thread running the form will result in an error
        // because it is an unsafe practice. These functions ensure that we can modify controls while sticking to
        // safe practices.
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void _suspendLayoutControl(Control refControl)
        {
            if (refControl.InvokeRequired)
            {
                _suspendLayoutControlCallback d = new _suspendLayoutControlCallback(_suspendLayoutControl);
                _owningForm.Invoke(d, new object[] { refControl });
            }
            else
            {
                refControl.SuspendLayout();
            }
        }

        private void _resumeLayoutControl(Control refControl)
        {
            if (refControl.InvokeRequired)
            {
                _resumeLayoutControlCallback d = new _resumeLayoutControlCallback(_resumeLayoutControl);
                _owningForm.Invoke(d, new object[] { refControl });
            }
            else
            {
                refControl.ResumeLayout(false);
            }
        }

        private void _performLayoutControl(Control refControl)
        {
            if (refControl.InvokeRequired)
            {
                _performLayoutControlCallback d = new _performLayoutControlCallback(_performLayoutControl);
                _owningForm.Invoke(d, new object[] { refControl });
            }
            else
            {
                refControl.PerformLayout();
            }
        }

        private void _changeFlowVisibility(FlowLayoutPanel flowPanel, bool visibility)
        {
            if (flowPanel.InvokeRequired)
            {
                _changeFlowVisibilityCallback d = new _changeFlowVisibilityCallback(_changeFlowVisibility);
                _owningForm.Invoke(d, new object[] { flowPanel, visibility });
            }
            else
            {
                flowPanel.Visible = visibility;
            }
        }

        private void _changeControlEnable(Control refControl, bool enable)
        {
            if (refControl.InvokeRequired)
            {
                _changeControlEnableCallback d = new _changeControlEnableCallback(_changeControlEnable);
                _owningForm.Invoke(d, new object[] { refControl, enable });
            }
            else
            {
                refControl.Enabled = enable;
            }
        }

        private void _showControl(Control refControl, Control parentControl)
        {
            if (refControl.InvokeRequired || parentControl.InvokeRequired)
            {
                _showControlCallback d = new _showControlCallback(_showControl);
                _owningForm.Invoke(d, new object[] { refControl, parentControl });
            }
            else
            {
                parentControl.Controls.Add(refControl);
                refControl.Visible = true;
            }
        }

        private void _hideControl(Control refControl, Control parentControl)
        {
            if (refControl.InvokeRequired || parentControl.InvokeRequired)
            {
                _hideControlCallback d = new _hideControlCallback(_hideControl);
                _owningForm.Invoke(d, new object[] { refControl, parentControl });
            }
            else
            {
                parentControl.Controls.Remove(refControl);
                refControl.Visible = false;
            }
        }

        private void _updateTextBox(TextBox boxControl, string data)
        {
            if (boxControl.InvokeRequired)
            {
                _updateTextboxCallback d = new _updateTextboxCallback(_updateTextBox);
                _owningForm.Invoke(d, new object[] { boxControl, data });
            }
            else
            {
                boxControl.Text = data;
            }
        }

        private void _updateControlValue(NumericUpDown refControl, decimal data)
        {
            if (refControl.InvokeRequired)
            {
                _updateControlValueCallback d = new _updateControlValueCallback(_updateControlValue);
                _owningForm.Invoke(d, new object[] { refControl, data });
            }
            else
            {
                refControl.Value = data;
            }
        }

        private string _getComboSelectedItem(ComboBox comboControl)
        {
            if (comboControl.InvokeRequired)
            {
                _getComboSelectedItemCallback d = new _getComboSelectedItemCallback(_getComboSelectedItem);
                string selectedItem = (string) _owningForm.Invoke(d, new object[] { comboControl });
                return selectedItem;
            }
            else
            {
                return (string) comboControl.SelectedItem;
            }
        }

        private void _changeControlBg(Control refControl, Color refColor)
        {
            if (refControl.InvokeRequired)
            {
                _changeControlBgCallback d = new _changeControlBgCallback(_changeControlBg);
                _owningForm.Invoke(d, new object[] { refControl, refColor });
            }
            else
            {
                refControl.BackColor = refColor;
            }
        }

        private void _deleteControl(Control refControl)
        {   // This one doesn't require the invoke workaround because the remove sandwich doesn't create a separate thread
            if (!refControl.HasChildren)
            {
                refControl.Dispose();
            }
            else
            {
                for (int i = (refControl.Controls.Count - 1); i >= 0; i--)
                {
                    _deleteControl(refControl.Controls[i]);
                }

                refControl.Dispose();
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Communication functions
        //
        // Functions to communicate with the Arduino
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void _sendCommand(string command)
        {
            _port.Write(command + "\n");
        }

        private String _receiveReply()
        {
            try
            {
                // Since we set ReadTimeout to a finite number, ReadLine will return a TimeoutException if no temperature is received within
                // that time limit. This is done on purpose because the thread would be blocked forever by ReadLine if ReadTimeout is not a
                // finite number. We want this loop to be refreshed every now and then to find out if collectData has been set to false.
                // As for the exception thrown upon timeout, tell user that no data was received.
                return _port.ReadTo("\n");
            }
            catch (TimeoutException)
            {
                return "e000";
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Exceptions
        //
        // Definitions for custom exceptions thrown by the program
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public class ArduinoException : System.Exception
        {
            public ArduinoException() : base() { }
            public ArduinoException(string message) : base(message) { }
            public ArduinoException(string message, System.Exception inner) : base(message, inner) { }
        }
    }// END Sandwich class def.
}// END TDTSandwich namespace
