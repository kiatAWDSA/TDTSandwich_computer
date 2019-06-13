using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace TDTSandwich
{
  public partial class Sandwich : FlowLayoutPanel
  {
    private TDTSandwich     owningForm_;
    private FlowLayoutPanel mainFlowPanel_;
    private ErrorLogger     errorLogger_;
    private Communication   communications_;

    private FlowLayoutPanel sandwich_;
    // ID controls
    private GroupBox        ID_;
    private FlowLayoutPanel ID_flow_;
    private TextBox         ID_textbox_;
    // DAQ controls
    private GroupBox        DAQ_;
    private FlowLayoutPanel DAQ_flow_;
    private FlowLayoutPanel DAQ_heater1_;
    private TextBox         DAQ_heater1_textbox_;
    private Label           DAQ_heater1_label_;
    private FlowLayoutPanel DAQ_heater2_;
    private TextBox         DAQ_heater2_textbox_;
    private Label           DAQ_heater2_label_;
    private FlowLayoutPanel DAQ_sample_;
    private Button          DAQ_startDAQ_;
    private Button          DAQ_stopDAQ_;
    private Label           DAQ_sample_label_;
    private TextBox         DAQ_sample_textbox_;
    private CheckBox        DAQ_readSample_;
    // Heat controls
    private GroupBox        heat_;
    private FlowLayoutPanel heat_flow_;
    private FlowLayoutPanel heat_setpoint_;
    private Label           heat_setpoint_label_;
    private NumericUpDown   heat_setpoint_upDown_;
    private FlowLayoutPanel heat_rate_;
    private Label           heat_rate_label_;
    private NumericUpDown   heat_rate_upDown_;
    private GroupBox        heat_timer_;
    private FlowLayoutPanel heat_timer_flow_;
    private Label           heat_timer_h_label_;
    private NumericUpDown   heat_timer_h_upDown_;
    private Label           heat_timer_m_label_;
    private NumericUpDown   heat_timer_m_upDown_;
    private Label           heat_timer_s_label_;
    private NumericUpDown   heat_timer_s_upDown_;
    private Button          heat_startHeat_;
    private Button          heat_stopHeat_;
    // Record controls
    private GroupBox        record_;
    private FlowLayoutPanel record_flow_;
    private FlowLayoutPanel record_filepath_;
    private Label           record_filepath_label_;
    private Button          record_startRecord_;
    private Button          record_stopRecord_;
    private TextBox         record_filepath_textbox_;
    private Button          record_browse_;
    // Advanced controls
    private GroupBox        advanced_;
    private Button          advanced_show_;
    private FlowLayoutPanel advanced_flow_;
    private FlowLayoutPanel advanced_hiddenFlow_;
    private Button          advanced_hide_;
    private Button          advanced_blinkLED_;
    private FlowLayoutPanel advanced_thermocouple_;
    private Label           advanced_thermocouple_label_;
    private ComboBox        advanced_thermocouple_dropdown_;
    private FlowLayoutPanel advanced_port_;
    private Label           advanced_port_label_;
    private ComboBox        advanced_port_dropdown_;
    private FlowLayoutPanel advanced_ID_;
    private Label           advanced_ID_label_;
    private NumericUpDown   advanced_ID_upDown_;
    private Button          advanced_removeSandwich_;
    private FlowLayoutPanel advanced_oversampling_;
    private Label           advanced_oversampling_label_;
    private ComboBox        advanced_oversampling_dropdown_;
    private FlowLayoutPanel advanced_TStepDuration_;
    private Label           advanced_TStepDuration_label_;
    private NumericUpDown   advanced_TStepDuration_upDown_;
    private FlowLayoutPanel advanced_PID_proportional_;
    private Label           advanced_PID_proportional_label_;
    private NumericUpDown   advanced_PID_proportional_upDown_;
    private FlowLayoutPanel advanced_PID_integral_;
    private Label           advanced_PID_integral_label_;
    private NumericUpDown   advanced_PID_integral_upDown_;
    private FlowLayoutPanel advanced_sampleOffset_;
    private Label           advanced_sampleOffset_label_;
    private NumericUpDown   advanced_sampleOffset_upDown_;


    private int sandwichHeight_;

    private Thread commandOutWorker_;
    private Thread commandInWorker_;
    private FileStream recordStream_;
    private StreamWriter CSVWriter_;

    private bool TCAmplifierErr_;
    private bool DAQActive_;
    private bool DAQSample_;
    private bool recordActive_;
    private bool heatActive_;
    private int sandwichControlID_;
    private int sandwichID_;
    private double heater1T_;
    private double heater2T_;
    private double sampleT_;
    private string controlPrefix_;
    private string savePathway_;
    private string COMPort_;
    private DateTime recordingStartTime_;
    private DateTime heatingStartTime_;
    private TimeSpan heatingDuration_;
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

    public Sandwich(TDTSandwich owningForm, FlowLayoutPanel mainFlowPanel, ErrorLogger errorLogger, int sandwichControlID, int sandwichCreatedControlCount, List<string> unoccupiedCOMPorts)
    {
      sandwichControlID_ = sandwichControlID;
      controlPrefix_ = "sandwich_" + Convert.ToString(sandwichControlID_);
      errorLogger_ = errorLogger;

      sandwichHeight_ = 68;

      owningForm_ = owningForm;
      mainFlowPanel_ = mainFlowPanel;

      DAQ_startDAQ_ = new System.Windows.Forms.Button();
      DAQ_stopDAQ_ = new System.Windows.Forms.Button();
      record_startRecord_ = new System.Windows.Forms.Button();
      record_stopRecord_ = new System.Windows.Forms.Button();
      heat_stopHeat_ = new System.Windows.Forms.Button();
      heat_startHeat_ = new System.Windows.Forms.Button();
      advanced_thermocouple_dropdown_ = new System.Windows.Forms.ComboBox();
      advanced_blinkLED_ = new System.Windows.Forms.Button();
      sandwich_ = new System.Windows.Forms.FlowLayoutPanel();
      ID_ = new System.Windows.Forms.GroupBox();
      ID_flow_ = new System.Windows.Forms.FlowLayoutPanel();
      ID_textbox_ = new System.Windows.Forms.TextBox();
      DAQ_ = new System.Windows.Forms.GroupBox();
      DAQ_flow_ = new System.Windows.Forms.FlowLayoutPanel();
      DAQ_heater1_ = new System.Windows.Forms.FlowLayoutPanel();
      DAQ_heater1_textbox_ = new System.Windows.Forms.TextBox();
      DAQ_heater1_label_ = new System.Windows.Forms.Label();
      DAQ_heater2_ = new System.Windows.Forms.FlowLayoutPanel();
      DAQ_heater2_textbox_ = new System.Windows.Forms.TextBox();
      DAQ_heater2_label_ = new System.Windows.Forms.Label();
      DAQ_sample_ = new System.Windows.Forms.FlowLayoutPanel();
      DAQ_sample_label_ = new System.Windows.Forms.Label();
      DAQ_sample_textbox_ = new System.Windows.Forms.TextBox();
      DAQ_readSample_ = new System.Windows.Forms.CheckBox();
      heat_ = new System.Windows.Forms.GroupBox();
      heat_flow_ = new System.Windows.Forms.FlowLayoutPanel();
      heat_setpoint_ = new System.Windows.Forms.FlowLayoutPanel();
      heat_setpoint_label_ = new System.Windows.Forms.Label();
      heat_setpoint_upDown_ = new System.Windows.Forms.NumericUpDown();
      heat_rate_ = new System.Windows.Forms.FlowLayoutPanel();
      heat_rate_label_ = new System.Windows.Forms.Label();
      heat_rate_upDown_ = new System.Windows.Forms.NumericUpDown();
      heat_timer_ = new System.Windows.Forms.GroupBox();
      heat_timer_flow_ = new System.Windows.Forms.FlowLayoutPanel();
      heat_timer_h_upDown_ = new System.Windows.Forms.NumericUpDown();
      heat_timer_h_label_ = new System.Windows.Forms.Label();
      heat_timer_m_upDown_ = new System.Windows.Forms.NumericUpDown();
      heat_timer_m_label_ = new System.Windows.Forms.Label();
      heat_timer_s_upDown_ = new System.Windows.Forms.NumericUpDown();
      heat_timer_s_label_ = new System.Windows.Forms.Label();
      record_ = new System.Windows.Forms.GroupBox();
      record_flow_ = new System.Windows.Forms.FlowLayoutPanel();
      record_filepath_ = new System.Windows.Forms.FlowLayoutPanel();
      record_filepath_label_ = new System.Windows.Forms.Label();
      record_filepath_textbox_ = new System.Windows.Forms.TextBox();
      record_browse_ = new System.Windows.Forms.Button();
      advanced_ = new System.Windows.Forms.GroupBox();
      advanced_flow_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_hiddenFlow_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_show_ = new System.Windows.Forms.Button();
      advanced_hide_ = new System.Windows.Forms.Button();
      advanced_thermocouple_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_thermocouple_label_ = new System.Windows.Forms.Label();
      advanced_port_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_port_label_ = new System.Windows.Forms.Label();
      advanced_port_dropdown_ = new System.Windows.Forms.ComboBox();
      advanced_ID_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_ID_label_ = new System.Windows.Forms.Label();
      advanced_ID_upDown_ = new System.Windows.Forms.NumericUpDown();
      advanced_removeSandwich_ = new System.Windows.Forms.Button();
      advanced_oversampling_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_oversampling_label_ = new System.Windows.Forms.Label();
      advanced_oversampling_dropdown_ = new System.Windows.Forms.ComboBox();
      advanced_TStepDuration_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_TStepDuration_label_ = new System.Windows.Forms.Label();
      advanced_TStepDuration_upDown_ = new System.Windows.Forms.NumericUpDown();
      advanced_PID_proportional_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_PID_proportional_label_ = new System.Windows.Forms.Label();
      advanced_PID_proportional_upDown_ = new System.Windows.Forms.NumericUpDown();
      advanced_PID_integral_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_PID_integral_label_ = new System.Windows.Forms.Label();
      advanced_PID_integral_upDown_ = new System.Windows.Forms.NumericUpDown();
      advanced_sampleOffset_ = new System.Windows.Forms.FlowLayoutPanel();
      advanced_sampleOffset_label_ = new System.Windows.Forms.Label();
      advanced_sampleOffset_upDown_ = new System.Windows.Forms.NumericUpDown();
      sandwich_.SuspendLayout();
      ID_.SuspendLayout();
      ID_flow_.SuspendLayout();
      DAQ_.SuspendLayout();
      DAQ_flow_.SuspendLayout();
      DAQ_heater1_.SuspendLayout();
      DAQ_heater2_.SuspendLayout();
      DAQ_sample_.SuspendLayout();
      heat_.SuspendLayout();
      heat_flow_.SuspendLayout();
      heat_setpoint_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(heat_setpoint_upDown_)).BeginInit();
      heat_rate_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(heat_rate_upDown_)).BeginInit();
      heat_timer_.SuspendLayout();
      heat_timer_flow_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(heat_timer_h_upDown_)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(heat_timer_m_upDown_)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(heat_timer_s_upDown_)).BeginInit();
      record_.SuspendLayout();
      record_flow_.SuspendLayout();
      record_filepath_.SuspendLayout();
      advanced_.SuspendLayout();
      advanced_flow_.SuspendLayout();
      advanced_hiddenFlow_.SuspendLayout();
      advanced_thermocouple_.SuspendLayout();
      advanced_port_.SuspendLayout();
      advanced_ID_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_ID_upDown_)).BeginInit();
      advanced_oversampling_.SuspendLayout();
      advanced_TStepDuration_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_TStepDuration_upDown_)).BeginInit();
      advanced_PID_proportional_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_PID_proportional_upDown_)).BeginInit();
      advanced_PID_integral_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_PID_integral_upDown_)).BeginInit();
      advanced_sampleOffset_.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_sampleOffset_upDown_)).BeginInit();
      mainFlowPanel_.SuspendLayout();
      owningForm_.SuspendLayout();


      ////////////////////////////////////
      //       Sandwich
      ////////////////////////////////////
      // 
      // Parent container for all the controls
      // 
      sandwich_.Controls.Add(ID_);
      sandwich_.Controls.Add(DAQ_);
      sandwich_.Controls.Add(heat_);
      sandwich_.Controls.Add(record_);
      sandwich_.Controls.Add(advanced_);
      sandwich_.Location = new System.Drawing.Point(0, 0); // Since we are adding to a flowLayoutPanel in a top-down direction, we do not have to worry about the exact location of the sandwich (it will be autocalculated)
      sandwich_.Margin = new System.Windows.Forms.Padding(0);
      sandwich_.Name = controlPrefix_;
      //_sandwich.Size = new System.Drawing.Size(1966, 67);
      sandwich_.Size = new System.Drawing.Size(2288, 67);
      sandwich_.TabIndex = 12;
      // Add the sandwich and adjust the height of the main flowlayout panel
      mainFlowPanel_.Controls.Add(sandwich_);
      mainFlowPanel_.Size = new System.Drawing.Size(sandwich_.Size.Width, mainFlowPanel_.Size.Height + sandwichHeight_);

      ////////////////////////////////////
      //       ID
      ////////////////////////////////////
      // 
      // ID
      // 
      ID_.Controls.Add(ID_flow_);
      ID_.Location = new System.Drawing.Point(3, 0);
      ID_.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      ID_.Name = controlPrefix_ + "_ID";
      ID_.Size = new System.Drawing.Size(40, 66);
      ID_.TabIndex = 0;
      ID_.TabStop = false;
      ID_.Text = "ID";
      // 
      // ID_flow
      // 
      ID_flow_.Controls.Add(ID_textbox_);
      ID_flow_.Location = new System.Drawing.Point(3, 13);
      ID_flow_.Name = controlPrefix_ + "_ID_flow";
      ID_flow_.Size = new System.Drawing.Size(34, 52);
      ID_flow_.TabIndex = 19;
      // 
      // ID_textbox
      // 
      ID_textbox_.BackColor = System.Drawing.SystemColors.Control;
      ID_textbox_.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      ID_textbox_.Location = new System.Drawing.Point(26, 19);
      ID_textbox_.Margin = new System.Windows.Forms.Padding(3, 19, 3, 3);
      ID_textbox_.MaxLength = 2;
      ID_textbox_.Name = controlPrefix_ + "_ID_textbox";
      ID_textbox_.ReadOnly = true;
      ID_textbox_.Size = new System.Drawing.Size(28, 26);
      ID_textbox_.TabIndex = 2;
      // _ID_textbox.Text = "1"; // This is automatically updated whenever the sandwich ID numericupdown in the advanced section is changed.
      ID_textbox_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

      ////////////////////////////////////
      //       DAQ
      ////////////////////////////////////
      // 
      // DAQ
      // 
      DAQ_.Controls.Add(DAQ_flow_);
      DAQ_.Location = new System.Drawing.Point(49, 0);
      DAQ_.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      DAQ_.Name = controlPrefix_ + "_DAQ";
      DAQ_.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      //_DAQ.Size = new System.Drawing.Size(260, 66);
      DAQ_.Size = new System.Drawing.Size(342, 66);
      DAQ_.TabIndex = 1;
      DAQ_.TabStop = false;
      DAQ_.Text = "DAQ";
      // 
      // DAQ_flow
      // 
      DAQ_flow_.Controls.Add(DAQ_heater1_);
      DAQ_flow_.Controls.Add(DAQ_heater2_);
      DAQ_flow_.Controls.Add(DAQ_sample_);
      DAQ_flow_.Controls.Add(DAQ_readSample_);
      DAQ_flow_.Controls.Add(DAQ_startDAQ_);
      DAQ_flow_.Location = new System.Drawing.Point(3, 13);
      DAQ_flow_.Name = controlPrefix_ + "_DAQ_flow";
      //_DAQ_flow.Size = new System.Drawing.Size(256, 52);
      DAQ_flow_.Size = new System.Drawing.Size(338, 52);
      DAQ_flow_.TabIndex = 1;
      // 
      // DAQ_heater1
      // 
      DAQ_heater1_.Controls.Add(DAQ_heater1_label_);
      DAQ_heater1_.Controls.Add(DAQ_heater1_textbox_);
      DAQ_heater1_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      DAQ_heater1_.Location = new System.Drawing.Point(0, 0);
      DAQ_heater1_.Margin = new System.Windows.Forms.Padding(0);
      DAQ_heater1_.Name = controlPrefix_ + "_DAQ_heater1";
      //_DAQ_heater1.Size = new System.Drawing.Size(68, 49);
      DAQ_heater1_.Size = new System.Drawing.Size(74, 49);
      DAQ_heater1_.TabIndex = 1;
      // 
      // DAQ_heater1_label
      // 
      DAQ_heater1_label_.AutoSize = true;
      DAQ_heater1_label_.Location = new System.Drawing.Point(3, 3);
      DAQ_heater1_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      DAQ_heater1_label_.Name = controlPrefix_ + "_DAQ_heater1_label";
      //_DAQ_heater1_label.Size = new System.Drawing.Size(59, 13);
      DAQ_heater1_label_.Size = new System.Drawing.Size(68, 13);
      DAQ_heater1_label_.TabIndex = 1;
      DAQ_heater1_label_.Text = "Heater 1 (°C)";
      // 
      // DAQ_heater1_textbox
      // 
      DAQ_heater1_textbox_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      DAQ_heater1_textbox_.Location = new System.Drawing.Point(3, 19);
      DAQ_heater1_textbox_.MaxLength = 6;
      DAQ_heater1_textbox_.Name = controlPrefix_ + "_DAQ_heater1_textbox";
      DAQ_heater1_textbox_.ReadOnly = true;
      //_DAQ_heater1_textbox.Size = new System.Drawing.Size(60, 26);
      DAQ_heater1_textbox_.Size = new System.Drawing.Size(65, 26);
      DAQ_heater1_textbox_.TabIndex = 2;
      DAQ_heater1_textbox_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // _DAQ_heater2
      // 
      DAQ_heater2_.Controls.Add(DAQ_heater2_label_);
      DAQ_heater2_.Controls.Add(DAQ_heater2_textbox_);
      DAQ_heater2_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      DAQ_heater2_.Location = new System.Drawing.Point(0, 0);
      DAQ_heater2_.Margin = new System.Windows.Forms.Padding(0);
      DAQ_heater2_.Name = controlPrefix_ + "_DAQ_heater2";
      //_DAQ_heater2.Size = new System.Drawing.Size(68, 49);
      DAQ_heater2_.Size = new System.Drawing.Size(74, 49);
      DAQ_heater2_.TabIndex = 2;
      // 
      // _DAQ_heater2_label
      // 
      DAQ_heater2_label_.AutoSize = true;
      DAQ_heater2_label_.Location = new System.Drawing.Point(3, 3);
      DAQ_heater2_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      DAQ_heater2_label_.Name = controlPrefix_ + "_DAQ_heater2_label";
      //_DAQ_heater2_label.Size = new System.Drawing.Size(59, 13);
      DAQ_heater2_label_.Size = new System.Drawing.Size(68, 13);
      DAQ_heater2_label_.TabIndex = 1;
      DAQ_heater2_label_.Text = "Heater 2 (°C)";
      // 
      // _DAQ_heater2_textbox
      // 
      DAQ_heater2_textbox_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      DAQ_heater2_textbox_.Location = new System.Drawing.Point(3, 19);
      DAQ_heater2_textbox_.MaxLength = 6;
      DAQ_heater2_textbox_.Name = controlPrefix_ + "_DAQ_heater2_textbox";
      DAQ_heater2_textbox_.ReadOnly = true;
      //_DAQ_heater2_textbox.Size = new System.Drawing.Size(60, 26);
      DAQ_heater2_textbox_.Size = new System.Drawing.Size(65, 26);
      DAQ_heater2_textbox_.TabIndex = 2;
      DAQ_heater2_textbox_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // DAQ_sample
      // 
      DAQ_sample_.Controls.Add(DAQ_sample_label_);
      DAQ_sample_.Controls.Add(DAQ_sample_textbox_);
      DAQ_sample_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      DAQ_sample_.Location = new System.Drawing.Point(68, 0);
      DAQ_sample_.Margin = new System.Windows.Forms.Padding(0);
      DAQ_sample_.Name = controlPrefix_ + "_DAQ_sample";
      DAQ_sample_.Size = new System.Drawing.Size(68, 49);
      DAQ_sample_.TabIndex = 3;
      // 
      // DAQ_sample_label
      // 
      DAQ_sample_label_.Enabled = false;
      DAQ_sample_label_.AutoSize = true;
      DAQ_sample_label_.Location = new System.Drawing.Point(3, 3);
      DAQ_sample_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      DAQ_sample_label_.Name = controlPrefix_ + "_DAQ_sample_label";
      DAQ_sample_label_.Size = new System.Drawing.Size(62, 13);
      DAQ_sample_label_.TabIndex = 1;
      DAQ_sample_label_.Text = "Sample (°C)";
      // 
      // DAQ_sample_textbox
      // 
      DAQ_sample_textbox_.Enabled = false;
      DAQ_sample_textbox_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      DAQ_sample_textbox_.Location = new System.Drawing.Point(3, 19);
      DAQ_sample_textbox_.MaxLength = 6;
      DAQ_sample_textbox_.Name = controlPrefix_ + "_DAQ_sample_textbox";
      DAQ_sample_textbox_.ReadOnly = true;
      DAQ_sample_textbox_.Size = new System.Drawing.Size(60, 26);
      DAQ_sample_textbox_.TabIndex = 2;
      DAQ_sample_textbox_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // DAQ_readSample
      // 
      DAQ_readSample_.AutoSize = true;
      DAQ_readSample_.Location = new System.Drawing.Point(139, 16);
      DAQ_readSample_.Margin = new System.Windows.Forms.Padding(3, 16, 0, 3);
      DAQ_readSample_.Name = controlPrefix_ + "_DAQ_readSample";
      DAQ_readSample_.Size = new System.Drawing.Size(65, 30);
      DAQ_readSample_.TabIndex = 4;
      DAQ_readSample_.Text = "Read\r\nsample?";
      DAQ_readSample_.UseVisualStyleBackColor = true;
      // 
      // DAQ_startDAQ
      // 
      DAQ_startDAQ_.Location = new System.Drawing.Point(287, 11);
      DAQ_startDAQ_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      DAQ_startDAQ_.Name = controlPrefix_ + "_DAQ_startDAQ";
      DAQ_startDAQ_.Size = new System.Drawing.Size(45, 35);
      DAQ_startDAQ_.TabIndex = 5;
      DAQ_startDAQ_.Text = "Start DAQ";
      DAQ_startDAQ_.UseVisualStyleBackColor = true;
      // 
      // DAQ_stopDAQ
      // 
      DAQ_stopDAQ_.Enabled = false;
      DAQ_stopDAQ_.Location = new System.Drawing.Point(287, 11);
      DAQ_stopDAQ_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      DAQ_stopDAQ_.Name = controlPrefix_ + "_DAQ_stopDAQ";
      DAQ_stopDAQ_.Size = new System.Drawing.Size(45, 35);
      DAQ_stopDAQ_.TabIndex = 6;
      DAQ_stopDAQ_.Text = "Stop DAQ";
      DAQ_stopDAQ_.Visible = false;
      DAQ_stopDAQ_.UseVisualStyleBackColor = true;


      ////////////////////////////////////
      //       Heat
      ////////////////////////////////////
      // 
      // heat
      // 
      heat_.Controls.Add(heat_flow_);
      heat_.Location = new System.Drawing.Point(357, 0);
      heat_.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      heat_.Name = controlPrefix_ + "_heat";
      heat_.Size = new System.Drawing.Size(416, 66);
      heat_.TabIndex = 2;
      heat_.TabStop = false;
      heat_.Text = "Heat";
      // 
      // heat_flow
      // 
      heat_flow_.Controls.Add(heat_setpoint_);
      heat_flow_.Controls.Add(heat_rate_);
      heat_flow_.Controls.Add(heat_timer_);
      heat_flow_.Controls.Add(heat_startHeat_);
      heat_flow_.Location = new System.Drawing.Point(3, 13);
      heat_flow_.Name = controlPrefix_ + "_heat_flow";
      heat_flow_.Size = new System.Drawing.Size(412, 52);
      heat_flow_.TabIndex = 22;
      // 
      // heat_setpoint
      // 
      heat_setpoint_.Controls.Add(heat_setpoint_label_);
      heat_setpoint_.Controls.Add(heat_setpoint_upDown_);
      heat_setpoint_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      heat_setpoint_.Location = new System.Drawing.Point(0, 0);
      heat_setpoint_.Margin = new System.Windows.Forms.Padding(0);
      heat_setpoint_.Name = controlPrefix_ + "_heat_setpoint";
      heat_setpoint_.Size = new System.Drawing.Size(72, 49);
      heat_setpoint_.TabIndex = 1;
      // 
      // heat_setpoint_label
      // 
      heat_setpoint_label_.AutoSize = true;
      heat_setpoint_label_.Location = new System.Drawing.Point(3, 3);
      heat_setpoint_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      heat_setpoint_label_.Name = controlPrefix_ + "_heat_setpoint_label";
      heat_setpoint_label_.Size = new System.Drawing.Size(66, 13);
      heat_setpoint_label_.TabIndex = 14;
      heat_setpoint_label_.Text = "Setpoint (°C)";
      // 
      // heat_setpoint_upDown
      // 
      heat_setpoint_upDown_.DecimalPlaces = 1;
      heat_setpoint_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_setpoint_upDown_.Location = new System.Drawing.Point(3, 19);
      heat_setpoint_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      heat_setpoint_upDown_.Minimum = 0;
      heat_setpoint_upDown_.Maximum = 140;
      heat_setpoint_upDown_.Name = controlPrefix_ + "_heat_setpoint_upDown";
      heat_setpoint_upDown_.Size = new System.Drawing.Size(60, 26);
      heat_setpoint_upDown_.TabIndex = 1;
      heat_setpoint_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // heat_rate
      // 
      heat_rate_.Controls.Add(heat_rate_label_);
      heat_rate_.Controls.Add(heat_rate_upDown_);
      heat_rate_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      heat_rate_.Location = new System.Drawing.Point(72, 0);
      heat_rate_.Margin = new System.Windows.Forms.Padding(0);
      heat_rate_.Name = controlPrefix_ + "_heat_rate";
      heat_rate_.Size = new System.Drawing.Size(71, 49);
      heat_rate_.TabIndex = 2;
      // 
      // heat_rate_label
      // 
      heat_rate_label_.AutoSize = true;
      heat_rate_label_.Location = new System.Drawing.Point(3, 3);
      heat_rate_label_.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      heat_rate_label_.Name = controlPrefix_ + "_heat_rate_label";
      heat_rate_label_.Size = new System.Drawing.Size(71, 13);
      heat_rate_label_.TabIndex = 14;
      heat_rate_label_.Text = "Rate (°C/min)";
      // 
      // heat_rate_upDown
      // 
      heat_rate_upDown_.DecimalPlaces = 2;
      heat_rate_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_rate_upDown_.Location = new System.Drawing.Point(3, 19);
      heat_rate_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      heat_rate_upDown_.Minimum = 0;
      heat_rate_upDown_.Maximum = 9999;
      heat_rate_upDown_.Name = controlPrefix_ + "_heat_rate_upDown";
      heat_rate_upDown_.Size = new System.Drawing.Size(69, 26);
      heat_rate_upDown_.TabIndex = 2;
      heat_rate_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // heat_timer
      // 
      heat_timer_.Controls.Add(heat_timer_flow_);
      heat_timer_.Location = new System.Drawing.Point(147, 3);
      heat_timer_.Name = controlPrefix_ + "_heat_timer";
      heat_timer_.Size = new System.Drawing.Size(210, 47);
      heat_timer_.TabIndex = 3;
      heat_timer_.TabStop = false;
      heat_timer_.Text = "Timer (0 if not needed)";
      // 
      // heat_timer_flow
      // 
      heat_timer_flow_.Controls.Add(heat_timer_h_upDown_);
      heat_timer_flow_.Controls.Add(heat_timer_h_label_);
      heat_timer_flow_.Controls.Add(heat_timer_m_upDown_);
      heat_timer_flow_.Controls.Add(heat_timer_m_label_);
      heat_timer_flow_.Controls.Add(heat_timer_s_upDown_);
      heat_timer_flow_.Controls.Add(heat_timer_s_label_);
      heat_timer_flow_.Location = new System.Drawing.Point(3, 12);
      heat_timer_flow_.Margin = new System.Windows.Forms.Padding(0);
      heat_timer_flow_.Name = controlPrefix_ + "_heat_timer_flow";
      heat_timer_flow_.Size = new System.Drawing.Size(205, 32);
      heat_timer_flow_.TabIndex = 23;
      // 
      // heat_timer_h_upDown
      // 
      heat_timer_h_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_timer_h_upDown_.Location = new System.Drawing.Point(3, 3);
      heat_timer_h_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      heat_timer_h_upDown_.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
      heat_timer_h_upDown_.Name = controlPrefix_ + "_heat_timer_h_upDown";
      heat_timer_h_upDown_.Size = new System.Drawing.Size(40, 26);
      heat_timer_h_upDown_.TabIndex = 3;
      heat_timer_h_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // heat_timer_h_label
      // 
      heat_timer_h_label_.AutoSize = true;
      heat_timer_h_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_timer_h_label_.Location = new System.Drawing.Point(43, 3);
      heat_timer_h_label_.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      heat_timer_h_label_.Name = controlPrefix_ + "_heat_timer_h_label";
      heat_timer_h_label_.Size = new System.Drawing.Size(25, 24);
      heat_timer_h_label_.TabIndex = 3;
      heat_timer_h_label_.Text = "H";
      // 
      // heat_timer_m_upDown
      // 
      heat_timer_m_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_timer_m_upDown_.Location = new System.Drawing.Point(71, 3);
      heat_timer_m_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      heat_timer_m_upDown_.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
      heat_timer_m_upDown_.Name = controlPrefix_ + "_heat_timer_m_upDown";
      heat_timer_m_upDown_.Size = new System.Drawing.Size(40, 26);
      heat_timer_m_upDown_.TabIndex = 4;
      heat_timer_m_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // heat_timer_m_label
      // 
      heat_timer_m_label_.AutoSize = true;
      heat_timer_m_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_timer_m_label_.Location = new System.Drawing.Point(111, 3);
      heat_timer_m_label_.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      heat_timer_m_label_.Name = controlPrefix_ + "_heat_timer_m_label";
      heat_timer_m_label_.Size = new System.Drawing.Size(27, 24);
      heat_timer_m_label_.TabIndex = 4;
      heat_timer_m_label_.Text = "M";
      // 
      // heat_timer_s_upDown
      // 
      heat_timer_s_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_timer_s_upDown_.Location = new System.Drawing.Point(141, 3);
      heat_timer_s_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      heat_timer_s_upDown_.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
      heat_timer_s_upDown_.Name = controlPrefix_ + "_heat_timer_s_upDown";
      heat_timer_s_upDown_.Size = new System.Drawing.Size(40, 26);
      heat_timer_s_upDown_.TabIndex = 5;
      heat_timer_s_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // heat_timer_s_label
      // 
      heat_timer_s_label_.AutoSize = true;
      heat_timer_s_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      heat_timer_s_label_.Location = new System.Drawing.Point(181, 3);
      heat_timer_s_label_.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      heat_timer_s_label_.Name = controlPrefix_ + "_heat_timer_s_label";
      heat_timer_s_label_.Size = new System.Drawing.Size(23, 24);
      heat_timer_s_label_.TabIndex = 5;
      heat_timer_s_label_.Text = "S";
      // 
      // heat_startHeat
      // 
      heat_startHeat_.Enabled = false;
      heat_startHeat_.Location = new System.Drawing.Point(363, 11);
      heat_startHeat_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      heat_startHeat_.Name = controlPrefix_ + "_heat_startHeat";
      heat_startHeat_.Size = new System.Drawing.Size(45, 35);
      heat_startHeat_.TabIndex = 6;
      heat_startHeat_.Text = "Start heat";
      heat_startHeat_.UseVisualStyleBackColor = true;
      // 
      // heat_stopHeat
      // 
      heat_stopHeat_.Enabled = false;
      heat_stopHeat_.Location = new System.Drawing.Point(363, 11);
      heat_stopHeat_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      heat_stopHeat_.Name = controlPrefix_ + "_heat_stopHeat";
      heat_stopHeat_.Size = new System.Drawing.Size(45, 35);
      heat_stopHeat_.TabIndex = 7;
      heat_stopHeat_.Text = "Stop heat";
      heat_stopHeat_.UseVisualStyleBackColor = true;
      heat_stopHeat_.Visible = false;


      ////////////////////////////////////
      //       Record
      ////////////////////////////////////// 
      // record
      // 
      record_.Controls.Add(record_flow_);
      record_.Location = new System.Drawing.Point(707, 0);
      record_.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      record_.Name = controlPrefix_ + "_record";
      record_.Size = new System.Drawing.Size(235, 66);
      record_.TabIndex = 3;
      record_.TabStop = false;
      record_.Text = "Record";
      // 
      // record_flow
      // 
      record_flow_.Controls.Add(record_filepath_);
      record_flow_.Controls.Add(record_browse_);
      record_flow_.Controls.Add(record_startRecord_);
      record_flow_.Location = new System.Drawing.Point(3, 13);
      record_flow_.Name = controlPrefix_ + "_record_flow";
      record_flow_.Size = new System.Drawing.Size(231, 52);
      record_flow_.TabIndex = 18;
      // 
      // record_filepath
      // 
      record_filepath_.Controls.Add(record_filepath_label_);
      record_filepath_.Controls.Add(record_filepath_textbox_);
      record_filepath_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      record_filepath_.Location = new System.Drawing.Point(0, 0);
      record_filepath_.Margin = new System.Windows.Forms.Padding(0);
      record_filepath_.Name = controlPrefix_ + "_record_filepath";
      record_filepath_.Size = new System.Drawing.Size(148, 49);
      record_filepath_.TabIndex = 1;
      // 
      // record_filepath_label
      // 
      record_filepath_label_.AutoSize = true;
      record_filepath_label_.Location = new System.Drawing.Point(3, 3);
      record_filepath_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      record_filepath_label_.Name = controlPrefix_ + "_record_filepath_label";
      record_filepath_label_.Size = new System.Drawing.Size(47, 13);
      record_filepath_label_.TabIndex = 14;
      record_filepath_label_.Text = "Save to (.csv):";
      // 
      // record_filepath_textbox
      // 
      record_filepath_textbox_.BackColor = System.Drawing.SystemColors.Window;
      record_filepath_textbox_.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      record_filepath_textbox_.Location = new System.Drawing.Point(3, 19);
      record_filepath_textbox_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      record_filepath_textbox_.Name = controlPrefix_ + "_record_filepath_textbox";
      record_filepath_textbox_.Size = new System.Drawing.Size(144, 20);
      record_filepath_textbox_.TabIndex = 1;
      record_filepath_textbox_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      record_filepath_textbox_.WordWrap = false;
      // 
      // record_browse
      // 
      record_browse_.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      record_browse_.Location = new System.Drawing.Point(148, 18);
      record_browse_.Margin = new System.Windows.Forms.Padding(0, 18, 3, 3);
      record_browse_.Name = controlPrefix_ + "_record_browse";
      record_browse_.Size = new System.Drawing.Size(29, 22);
      record_browse_.TabIndex = 2;
      record_browse_.Text = "...";
      record_browse_.UseVisualStyleBackColor = true;
      // 
      // record_startRecord
      // 
      record_startRecord_.Enabled = false;
      record_startRecord_.Location = new System.Drawing.Point(183, 11);
      record_startRecord_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      record_startRecord_.Name = controlPrefix_ + "_record_startRecord";
      record_startRecord_.Size = new System.Drawing.Size(45, 35);
      record_startRecord_.TabIndex = 3;
      record_startRecord_.Text = "Start record";
      record_startRecord_.UseVisualStyleBackColor = true;
      // 
      // record_stopRecord
      // 
      record_stopRecord_.Enabled = false;
      record_stopRecord_.Location = new System.Drawing.Point(183, 11);
      record_stopRecord_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      record_stopRecord_.Name = controlPrefix_ + "_record_stopRecord";
      record_stopRecord_.Size = new System.Drawing.Size(45, 35);
      record_stopRecord_.TabIndex = 4;
      record_stopRecord_.Text = "Stop record";
      record_stopRecord_.UseVisualStyleBackColor = true;
      record_stopRecord_.Visible = false;


      ////////////////////////////////////
      //       Advanced
      ////////////////////////////////////
      // 
      // advanced
      // 
      advanced_.Controls.Add(advanced_flow_);
      advanced_.Location = new System.Drawing.Point(948, 0);
      advanced_.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      advanced_.Name = controlPrefix_ + "_advanced";
      //advanced_.Size = new System.Drawing.Size(1224, 66); // with error textbox
      advanced_.Size = new System.Drawing.Size(828, 66);    // without error textbox
      advanced_.TabIndex = 4;
      advanced_.TabStop = false;
      advanced_.Text = "Advanced";
      // 
      // advanced_flow
      // 
      advanced_flow_.Controls.Add(advanced_show_);
      advanced_flow_.Controls.Add(advanced_hiddenFlow_);
      advanced_flow_.Location = new System.Drawing.Point(3, 13);
      advanced_flow_.Name = controlPrefix_ + "_advanced_flow";
      //advanced_flow_.Size = new System.Drawing.Size(1215, 52);  // with error textbox
      advanced_flow_.Size = new System.Drawing.Size(819, 52);     // without error textbox
      advanced_flow_.TabIndex = 1;
      // 
      // _advanced_hiddenFlow
      // 
      advanced_hiddenFlow_.Controls.Add(advanced_hide_);
      advanced_hiddenFlow_.Controls.Add(advanced_blinkLED_);
      advanced_hiddenFlow_.Controls.Add(advanced_thermocouple_);
      advanced_hiddenFlow_.Controls.Add(advanced_port_);
      advanced_hiddenFlow_.Controls.Add(advanced_ID_);
      advanced_hiddenFlow_.Controls.Add(advanced_removeSandwich_);
      advanced_hiddenFlow_.Controls.Add(advanced_oversampling_);
      advanced_hiddenFlow_.Controls.Add(advanced_TStepDuration_);
      advanced_hiddenFlow_.Controls.Add(advanced_PID_proportional_);
      advanced_hiddenFlow_.Controls.Add(advanced_PID_integral_);
      advanced_hiddenFlow_.Controls.Add(advanced_sampleOffset_);
      advanced_hiddenFlow_.Location = new System.Drawing.Point(0, 0);
      advanced_hiddenFlow_.Margin = new System.Windows.Forms.Padding(0);
      advanced_hiddenFlow_.Name = "_advanced_hiddenFlow";
      //advanced_hiddenFlow_.Size = new System.Drawing.Size(1215, 52);  // with error textbox
      advanced_hiddenFlow_.Size = new System.Drawing.Size(819, 52);     // without error textbox
      advanced_hiddenFlow_.TabIndex = 2; ;
      advanced_hiddenFlow_.Visible = false;
      // 
      // advanced_show
      // 
      advanced_show_.Location = new System.Drawing.Point(3, 11);
      advanced_show_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      advanced_show_.Name = controlPrefix_ + "_advanced_show";
      advanced_show_.Size = new System.Drawing.Size(45, 35);
      advanced_show_.TabIndex = 1;
      advanced_show_.Text = "Show";
      advanced_show_.UseVisualStyleBackColor = true;
      // 
      // advanced_hide
      // 
      advanced_hide_.Location = new System.Drawing.Point(54, 11);
      advanced_hide_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      advanced_hide_.Name = controlPrefix_ + "_advanced_hide";
      advanced_hide_.Size = new System.Drawing.Size(45, 35);
      advanced_hide_.TabIndex = 2;
      advanced_hide_.Text = "Hide";
      advanced_hide_.UseVisualStyleBackColor = true;
      // 
      // advanced_blinkLED
      // 
      advanced_blinkLED_.Location = new System.Drawing.Point(105, 11);
      advanced_blinkLED_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      advanced_blinkLED_.Name = controlPrefix_ + "_advanced_blinkLED";
      advanced_blinkLED_.Size = new System.Drawing.Size(45, 35);
      advanced_blinkLED_.TabIndex = 3;
      advanced_blinkLED_.Text = "Blink LED";
      advanced_blinkLED_.UseVisualStyleBackColor = true;
      advanced_blinkLED_.Enabled = false;
      // 
      // advanced_thermocouple
      // 
      advanced_thermocouple_.Controls.Add(advanced_thermocouple_label_);
      advanced_thermocouple_.Controls.Add(advanced_thermocouple_dropdown_);
      advanced_thermocouple_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_thermocouple_.Location = new System.Drawing.Point(153, 0);
      advanced_thermocouple_.Margin = new System.Windows.Forms.Padding(0);
      advanced_thermocouple_.Name = controlPrefix_ + "_advanced_thermocouple";
      advanced_thermocouple_.Size = new System.Drawing.Size(84, 49);
      advanced_thermocouple_.TabIndex = 23;
      // 
      // advanced_thermocouple_label
      // 
      advanced_thermocouple_label_.AutoSize = true;
      advanced_thermocouple_label_.Location = new System.Drawing.Point(3, 3);
      advanced_thermocouple_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_thermocouple_label_.Name = controlPrefix_ + "_advanced_thermocouple_label";
      advanced_thermocouple_label_.Size = new System.Drawing.Size(75, 13);
      advanced_thermocouple_label_.TabIndex = 18;
      advanced_thermocouple_label_.Text = "Thermocouple\r\n";
      // 
      // advanced_thermocouple_dropdown
      // 
      advanced_thermocouple_dropdown_.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      advanced_thermocouple_dropdown_.FormattingEnabled = true;
      advanced_thermocouple_dropdown_.Items.AddRange(new object[] {
            "B",
            "E",
            "J",
            "K",
            "N",
            "R",
            "S",
            "T"});
      advanced_thermocouple_dropdown_.Location = new System.Drawing.Point(3, 19);
      advanced_thermocouple_dropdown_.Name = controlPrefix_ + "_advanced_thermocouple_dropdown";
      advanced_thermocouple_dropdown_.Size = new System.Drawing.Size(75, 21);
      advanced_thermocouple_dropdown_.SelectedValue = "T"; // This sets the default value of the thermocouple type drop down list to T...
      advanced_thermocouple_dropdown_.SelectedItem = "T"; // ...and this makes it display the value of T
      advanced_thermocouple_dropdown_.TabIndex = 4;
      advanced_thermocouple_dropdown_.Tag = "";
      // 
      // advanced_port
      // 
      advanced_port_.Controls.Add(advanced_port_label_);
      advanced_port_.Controls.Add(advanced_port_dropdown_);
      advanced_port_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_port_.Location = new System.Drawing.Point(237, 0);
      advanced_port_.Margin = new System.Windows.Forms.Padding(0);
      advanced_port_.Name = controlPrefix_ + "_advanced_port";
      advanced_port_.Size = new System.Drawing.Size(64, 49);
      advanced_port_.TabIndex = 24;
      // 
      // advanced_port_label
      // 
      advanced_port_label_.AutoSize = true;
      advanced_port_label_.Location = new System.Drawing.Point(3, 3);
      advanced_port_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_port_label_.Name = controlPrefix_ + "_advanced_port_label";
      advanced_port_label_.Size = new System.Drawing.Size(26, 13);
      advanced_port_label_.TabIndex = 19;
      advanced_port_label_.Text = "Port";
      // 
      // advanced_port_dropdown
      // 
      advanced_port_dropdown_.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      advanced_port_dropdown_.FormattingEnabled = true;
      advanced_port_dropdown_.Location = new System.Drawing.Point(3, 19);
      advanced_port_dropdown_.Name = controlPrefix_ + "_advanced_port_dropdown";
      advanced_port_dropdown_.Items.AddRange(unoccupiedCOMPorts.ToArray());
      advanced_port_dropdown_.Items.Insert(0, "None");
      advanced_port_dropdown_.SelectedValue = "None"; // This sets the default value to be none...
      advanced_port_dropdown_.SelectedItem = "None"; // ...and this makes it display the value of none
      COMPort_ = "None";
      advanced_port_dropdown_.Size = new System.Drawing.Size(55, 21);
      advanced_port_dropdown_.TabIndex = 5;
      // 
      // advanced_ID
      // 
      advanced_ID_.Controls.Add(advanced_ID_label_);
      advanced_ID_.Controls.Add(advanced_ID_upDown_);
      advanced_ID_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_ID_.Location = new System.Drawing.Point(301, 0);
      advanced_ID_.Margin = new System.Windows.Forms.Padding(0);
      advanced_ID_.Name = controlPrefix_ + "_advanced_ID";
      advanced_ID_.Size = new System.Drawing.Size(40, 49);
      advanced_ID_.TabIndex = 25;
      // 
      // advanced_ID_label
      // 
      advanced_ID_label_.AutoSize = true;
      advanced_ID_label_.Location = new System.Drawing.Point(3, 3);
      advanced_ID_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_ID_label_.Name = controlPrefix_ + "_advanced_ID_label";
      advanced_ID_label_.Size = new System.Drawing.Size(18, 13);
      advanced_ID_label_.TabIndex = 21;
      advanced_ID_label_.Text = "ID";
      // 
      // advanced_ID_upDown
      // 
      advanced_ID_upDown_.Location = new System.Drawing.Point(3, 19);
      advanced_ID_upDown_.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
      advanced_ID_upDown_.Name = controlPrefix_ + "_advanced_ID_upDown";
      advanced_ID_upDown_.Size = new System.Drawing.Size(37, 20);
      advanced_ID_upDown_.Value = 0;
      advanced_ID_upDown_.TabIndex = 6;
      // 
      // _advanced_removeSandwich
      //
      advanced_removeSandwich_.Location = new System.Drawing.Point(293, 11);
      advanced_removeSandwich_.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      advanced_removeSandwich_.Name = controlPrefix_ + "_advanced_removeSandwich";
      advanced_removeSandwich_.Size = new System.Drawing.Size(60, 35);
      advanced_removeSandwich_.TabIndex = 7;
      advanced_removeSandwich_.Text = "Delete sandwich";
      advanced_removeSandwich_.UseVisualStyleBackColor = true;

      /*
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
      _advanced_PID_radius_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
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
      */

      // 
      // advanced_oversampling
      // 
      advanced_oversampling_.Controls.Add(advanced_oversampling_label_);
      advanced_oversampling_.Controls.Add(advanced_oversampling_dropdown_);
      advanced_oversampling_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_oversampling_.Location = new System.Drawing.Point(0, 0);
      advanced_oversampling_.Margin = new System.Windows.Forms.Padding(0);
      advanced_oversampling_.Name = controlPrefix_ + "_advanced_oversampling";
      advanced_oversampling_.Size = new System.Drawing.Size(63, 49);
      advanced_oversampling_.TabIndex = 8;
      // 
      // advanced_oversampling_label
      // 
      advanced_oversampling_label_.AutoSize = true;
      advanced_oversampling_label_.Location = new System.Drawing.Point(3, 3);
      advanced_oversampling_label_.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      advanced_oversampling_label_.Name = controlPrefix_ + "_advanced_oversampling_label";
      advanced_oversampling_label_.Size = new System.Drawing.Size(50, 13);
      advanced_oversampling_label_.TabIndex = 8;
      advanced_oversampling_label_.Text = "Sampling";
      // 
      // advanced_oversampling_dropdown
      // 
      advanced_oversampling_dropdown_.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      advanced_oversampling_dropdown_.FormattingEnabled = true;
      advanced_oversampling_dropdown_.Items.AddRange(new object[] {
            1,
            2,
            4,
            8,
            16});
      advanced_oversampling_dropdown_.Location = new System.Drawing.Point(0, 0);
      advanced_oversampling_dropdown_.Name = controlPrefix_ + "_advanced_oversampling_dropdown";
      advanced_oversampling_dropdown_.Size = new System.Drawing.Size(55, 21);
      advanced_oversampling_dropdown_.SelectedValue = 2; // This sets the default value of the thermocouple type drop down list to T...
      advanced_oversampling_dropdown_.SelectedItem = 2; // ...and this makes it display the value of T
      advanced_oversampling_dropdown_.TabIndex = 8;
      advanced_oversampling_dropdown_.Tag = "";
      // 
      // advanced_TStepDuration
      // 
      advanced_TStepDuration_.Controls.Add(advanced_TStepDuration_label_);
      advanced_TStepDuration_.Controls.Add(advanced_TStepDuration_upDown_);
      advanced_TStepDuration_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_TStepDuration_.Location = new System.Drawing.Point(0, 0);
      advanced_TStepDuration_.Margin = new System.Windows.Forms.Padding(0);
      advanced_TStepDuration_.Name = controlPrefix_ + "_advanced_TStepDuration";
      advanced_TStepDuration_.Size = new System.Drawing.Size(80, 49);
      advanced_TStepDuration_.TabIndex = 9;
      // 
      // advanced_TStepDuration_label
      // 
      advanced_TStepDuration_label_.AutoSize = true;
      advanced_TStepDuration_label_.Location = new System.Drawing.Point(3, 3);
      advanced_TStepDuration_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_TStepDuration_label_.Name = controlPrefix_ + "_advanced_TStepDuration_label";
      advanced_TStepDuration_label_.Size = new System.Drawing.Size(66, 13);
      advanced_TStepDuration_label_.TabIndex = 9;
      advanced_TStepDuration_label_.Text = "Step width (s)";
      // 
      // advanced_TStepDuration_upDown
      // 
      advanced_TStepDuration_upDown_.DecimalPlaces = 2;
      advanced_TStepDuration_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      advanced_TStepDuration_upDown_.Location = new System.Drawing.Point(3, 19);
      advanced_TStepDuration_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      advanced_TStepDuration_upDown_.Minimum = 0.01M;
      advanced_TStepDuration_upDown_.Maximum = 99.99M;
      advanced_TStepDuration_upDown_.Value = 1.00M;   // Set a default T step duration of 1 s
      advanced_TStepDuration_upDown_.Name = controlPrefix_ + "_advanced_TStepDuration_upDown";
      advanced_TStepDuration_upDown_.Size = new System.Drawing.Size(77, 26);
      advanced_TStepDuration_upDown_.TabIndex = 9;
      advanced_TStepDuration_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // advanced_PID_proportional
      // 
      advanced_PID_proportional_.Controls.Add(advanced_PID_proportional_label_);
      advanced_PID_proportional_.Controls.Add(advanced_PID_proportional_upDown_);
      advanced_PID_proportional_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_PID_proportional_.Location = new System.Drawing.Point(0, 0);
      advanced_PID_proportional_.Margin = new System.Windows.Forms.Padding(0);
      advanced_PID_proportional_.Name = controlPrefix_ + "_advanced_PID_proportional";
      advanced_PID_proportional_.Size = new System.Drawing.Size(80, 49);
      advanced_PID_proportional_.TabIndex = 10;
      // 
      // advanced_PID_proportional_label
      // 
      advanced_PID_proportional_label_.AutoSize = true;
      advanced_PID_proportional_label_.Location = new System.Drawing.Point(3, 3);
      advanced_PID_proportional_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_PID_proportional_label_.Name = controlPrefix_ + "_advanced_PID_proportional_label";
      advanced_PID_proportional_label_.Size = new System.Drawing.Size(66, 13);
      advanced_PID_proportional_label_.TabIndex = 10;
      advanced_PID_proportional_label_.Text = "Proportional";
      // 
      // advanced_PID_proportional_upDown
      // 
      advanced_PID_proportional_upDown_.DecimalPlaces = 4;
      advanced_PID_proportional_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      advanced_PID_proportional_upDown_.Location = new System.Drawing.Point(3, 19);
      advanced_PID_proportional_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      advanced_PID_proportional_upDown_.Minimum = 0;
      advanced_PID_proportional_upDown_.Maximum = 9999.9999M;
      advanced_PID_proportional_upDown_.Name = controlPrefix_ + "_advanced_PID_proportional_upDown";
      advanced_PID_proportional_upDown_.Size = new System.Drawing.Size(77, 26);
      advanced_PID_proportional_upDown_.TabIndex = 10;
      advanced_PID_proportional_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // advanced_PID_integral
      // 
      advanced_PID_integral_.Controls.Add(advanced_PID_integral_label_);
      advanced_PID_integral_.Controls.Add(advanced_PID_integral_upDown_);
      advanced_PID_integral_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_PID_integral_.Location = new System.Drawing.Point(0, 0);
      advanced_PID_integral_.Margin = new System.Windows.Forms.Padding(0);
      advanced_PID_integral_.Name = controlPrefix_ + "_advanced_PID_integral";
      advanced_PID_integral_.Size = new System.Drawing.Size(80, 49);
      advanced_PID_integral_.TabIndex = 11;
      // 
      // advanced_PID_integral_label
      // 
      advanced_PID_integral_label_.AutoSize = true;
      advanced_PID_integral_label_.Location = new System.Drawing.Point(3, 3);
      advanced_PID_integral_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_PID_integral_label_.Name = controlPrefix_ + "_advanced_PID_integral_label";
      advanced_PID_integral_label_.Size = new System.Drawing.Size(66, 13);
      advanced_PID_integral_label_.TabIndex = 11;
      advanced_PID_integral_label_.Text = "Integral";
      // 
      // advanced_PID_integral_upDown
      // 
      advanced_PID_integral_upDown_.DecimalPlaces = 4;
      advanced_PID_integral_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      advanced_PID_integral_upDown_.Location = new System.Drawing.Point(3, 19);
      advanced_PID_integral_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      advanced_PID_integral_upDown_.Minimum = 0;
      advanced_PID_integral_upDown_.Maximum = 9999.9999M;
      advanced_PID_integral_upDown_.Name = controlPrefix_ + "_advanced_PID_integral_upDown";
      advanced_PID_integral_upDown_.Size = new System.Drawing.Size(77, 26);
      advanced_PID_integral_upDown_.TabIndex = 11;
      advanced_PID_integral_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // advanced_sampleOffset
      // 
      advanced_sampleOffset_.Controls.Add(advanced_sampleOffset_label_);
      advanced_sampleOffset_.Controls.Add(advanced_sampleOffset_upDown_);
      advanced_sampleOffset_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      advanced_sampleOffset_.Location = new System.Drawing.Point(0, 0);
      advanced_sampleOffset_.Margin = new System.Windows.Forms.Padding(0);
      advanced_sampleOffset_.Name = controlPrefix_ + "_advanced_sampleOffset";
      advanced_sampleOffset_.Size = new System.Drawing.Size(80, 49);
      advanced_sampleOffset_.TabIndex = 9;
      // 
      // advanced_sampleOffset_label
      // 
      advanced_sampleOffset_label_.AutoSize = true;
      advanced_sampleOffset_label_.Location = new System.Drawing.Point(0, 0);
      advanced_sampleOffset_label_.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      advanced_sampleOffset_label_.Name = controlPrefix_ + "_advanced_sampleOffset_label";
      advanced_sampleOffset_label_.Size = new System.Drawing.Size(71, 13);
      advanced_sampleOffset_label_.TabIndex = 9;
      advanced_sampleOffset_label_.Text = "Sample offset";
      // 
      // advanced_sampleOffset_upDown
      // 
      advanced_sampleOffset_upDown_.DecimalPlaces = 2;
      advanced_sampleOffset_upDown_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      advanced_sampleOffset_upDown_.Location = new System.Drawing.Point(3, 19);
      advanced_sampleOffset_upDown_.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      advanced_sampleOffset_upDown_.Minimum = -999.99M;
      advanced_sampleOffset_upDown_.Maximum = 999.99M;
      advanced_sampleOffset_upDown_.Value = 0.00M;   // Set a default value of 0
      advanced_sampleOffset_upDown_.Name = controlPrefix_ + "_advanced_sampleOffset_upDown";
      advanced_sampleOffset_upDown_.Size = new System.Drawing.Size(77, 20);
      advanced_sampleOffset_upDown_.TabIndex = 9;
      advanced_sampleOffset_upDown_.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      /*
      // 
      // advanced_error
      // 
      _advanced_error.Controls.Add(_advanced_error_label);
      _advanced_error.Controls.Add(advanced_error_textbox_);
      _advanced_error.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      _advanced_error.Location = new System.Drawing.Point(356, 0);
      _advanced_error.Margin = new System.Windows.Forms.Padding(0);
      _advanced_error.Name = controlPrefix_ + "_advanced_error";
      _advanced_error.Size = new System.Drawing.Size(396, 49);
      _advanced_error.TabIndex = 12;
      // 
      // advanced_error_label
      // 
      _advanced_error_label.AutoSize = true;
      _advanced_error_label.Location = new System.Drawing.Point(3, 3);
      _advanced_error_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      _advanced_error_label.Name = controlPrefix_ + "_advanced_error_label";
      _advanced_error_label.Size = new System.Drawing.Size(29, 13);
      _advanced_error_label.TabIndex = 12;
      _advanced_error_label.Text = "Error message";
      // 
      // advanced_error_textbox
      // 
      advanced_error_textbox_.Enabled = true;
      advanced_error_textbox_.Location = new System.Drawing.Point(3, 19);
      advanced_error_textbox_.Name = controlPrefix_ + "_advanced_error_textbox";
      advanced_error_textbox_.ReadOnly = true;
      advanced_error_textbox_.Size = new System.Drawing.Size(390, 20);
      advanced_error_textbox_.TabIndex = 12;
      */


      // 
      // Finalize the layout
      // 
      ID_.ResumeLayout(false);
      ID_flow_.ResumeLayout(false);
      ID_flow_.PerformLayout();
      DAQ_.ResumeLayout(false);
      DAQ_flow_.ResumeLayout(false);
      DAQ_flow_.PerformLayout();
      DAQ_heater1_.ResumeLayout(false);
      DAQ_heater1_.PerformLayout();
      DAQ_heater2_.ResumeLayout(false);
      DAQ_heater2_.PerformLayout();
      DAQ_sample_.ResumeLayout(false);
      DAQ_sample_.PerformLayout();
      heat_.ResumeLayout(false);
      heat_flow_.ResumeLayout(false);
      heat_setpoint_.ResumeLayout(false);
      heat_setpoint_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(heat_setpoint_upDown_)).EndInit();
      heat_rate_.ResumeLayout(false);
      heat_rate_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(heat_rate_upDown_)).EndInit();
      heat_timer_.ResumeLayout(false);
      heat_timer_flow_.ResumeLayout(false);
      heat_timer_flow_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(heat_timer_h_upDown_)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(heat_timer_m_upDown_)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(heat_timer_s_upDown_)).EndInit();
      record_.ResumeLayout(false);
      record_flow_.ResumeLayout(false);
      record_filepath_.ResumeLayout(false);
      record_filepath_.PerformLayout();
      advanced_.ResumeLayout(false);
      advanced_flow_.ResumeLayout(false);
      advanced_hiddenFlow_.ResumeLayout(false);
      advanced_thermocouple_.ResumeLayout(false);
      advanced_thermocouple_.PerformLayout();
      advanced_port_.ResumeLayout(false);
      advanced_port_.PerformLayout();
      advanced_ID_.ResumeLayout(false);
      advanced_ID_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_ID_upDown_)).EndInit();
      advanced_oversampling_.ResumeLayout(false);
      advanced_oversampling_.PerformLayout();
      advanced_TStepDuration_.ResumeLayout(false);
      advanced_TStepDuration_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_TStepDuration_upDown_)).EndInit();
      advanced_PID_proportional_.ResumeLayout(false);
      advanced_PID_proportional_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_PID_proportional_upDown_)).EndInit();
      advanced_PID_integral_.ResumeLayout(false);
      advanced_PID_integral_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_PID_integral_upDown_)).EndInit();
      advanced_sampleOffset_.ResumeLayout(false);
      advanced_sampleOffset_.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(advanced_sampleOffset_upDown_)).EndInit();
      sandwich_.ResumeLayout(false);
      sandwich_.PerformLayout();
      mainFlowPanel_.ResumeLayout(false);
      mainFlowPanel_.PerformLayout();
      owningForm_.ResumeLayout(false);
      owningForm_.PerformLayout();

      //
      // Assign event handlers
      //
      DAQ_startDAQ_.Click += _startDAQ_click;
      DAQ_stopDAQ_.Click += _stopDAQ_click;
      DAQ_readSample_.CheckedChanged += _sample_checkedChanged;
      heat_startHeat_.Click += _startHeat_click;
      heat_stopHeat_.Click += _stopHeat_click;
      record_filepath_textbox_.TextChanged += _recordFilePath_change;
      record_browse_.Click += _recordBrowse_click;
      record_startRecord_.Click += _startRecord_click;
      record_stopRecord_.Click += _stopRecord_click;
      advanced_ID_upDown_.ValueChanged += _updateSandwichID;
      advanced_ID_upDown_.Leave += _updateSandwichID;
      advanced_show_.Click += _showAdvancedHiddenPanel_click;
      advanced_hide_.Click += _hideAdvancedHiddenPanel_click;
      advanced_blinkLED_.Click += _blinkLED_click;
      advanced_port_dropdown_.SelectedValueChanged += _portValueChanged;
      advanced_removeSandwich_.Click += deleteSandwich;


      //
      // Remove all mouse wheel events from numericUpDown and dropdown boxes. Sometimes, users will attempt to scroll while focused on a numericUpDown and accidentally change its value
      //
      heat_setpoint_upDown_.MouseWheel += _chill_MouseWheel;
      heat_rate_upDown_.MouseWheel += _chill_MouseWheel;
      heat_timer_h_upDown_.MouseWheel += _chill_MouseWheel;
      heat_timer_m_upDown_.MouseWheel += _chill_MouseWheel;
      heat_timer_s_upDown_.MouseWheel += _chill_MouseWheel;
      advanced_thermocouple_dropdown_.MouseWheel += _chill_MouseWheel;
      advanced_port_dropdown_.MouseWheel += _chill_MouseWheel;
      advanced_oversampling_dropdown_.MouseWheel += _chill_MouseWheel;
      advanced_ID_upDown_.MouseWheel += _chill_MouseWheel;
      advanced_TStepDuration_upDown_.MouseWheel += _chill_MouseWheel;
      advanced_PID_proportional_upDown_.MouseWheel += _chill_MouseWheel;
      advanced_PID_integral_upDown_.MouseWheel += _chill_MouseWheel;
      advanced_sampleOffset_upDown_.MouseWheel += _chill_MouseWheel;


      //
      // Initialize other variables
      //
      TCAmplifierErr_ = false;
      DAQActive_ = false;
      DAQSample_ = false;
      recordActive_ = false;
      heatActive_ = false;
      savePathway_ = "";

      // Communications manager
      communications_ = new Communication(this, errorLogger_,
                                          HandleNewReadings,
                                          HandleStartDAQSuccess,
                                          HandleStartDAQFail,
                                          HandleStopDAQSuccess,
                                          HandleStopDAQFail,
                                          HandleStartHeatSuccess,
                                          HandleStartHeatFail,
                                          HandleStopHeatSuccess,
                                          HandleStopHeatFail,
                                          HandleBlinkLEDSuccess,
                                          HandleBlinkLEDFail,
                                          HandleHeatingComplete);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Functions for interfacing with external callers
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public void updateCOMPortList(List<string> unoccupiedCOMPorts)
    {
      List<string> dummyUnoccupiedPorts = unoccupiedCOMPorts;
      advanced_port_dropdown_.Items.Clear();
      advanced_port_dropdown_.Items.AddRange(unoccupiedCOMPorts.ToArray());
      advanced_port_dropdown_.Items.Insert(0, COMPort_);
      if (COMPort_ != "None")
      {
        advanced_port_dropdown_.Items.Insert(0, "None");
      }
      advanced_port_dropdown_.SelectedValueChanged -= _portValueChanged;
      advanced_port_dropdown_.SelectedValue = COMPort_;
      advanced_port_dropdown_.SelectedItem = COMPort_;
      advanced_port_dropdown_.SelectedValueChanged += _portValueChanged;
    }

    public bool getDAQStatus()
    {
      return DAQActive_;
    }

    public int getSandwichID()
    {
      return sandwichID_;
    }

    public string getCOMPort()
    {
      return COMPort_;
    }

    public void setCOMPort(string COMPort)
    { // These will trigger the _portValueChanged event handler which will cause an update of COM port list for all sandwiches
      advanced_port_dropdown_.SelectedValue = COMPort;
      advanced_port_dropdown_.SelectedItem = COMPort;
    }

    public string getConfiguration()
    {
      string configuration = "";
      configuration += Convert.ToString(sandwichID_);
      configuration += ",";
      configuration += Convert.ToString(DAQ_readSample_.Checked); // This will write "TRUE" if true, "FALSE" if false.
      configuration += ",";
      configuration += Convert.ToString(heat_setpoint_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(heat_rate_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(heat_timer_h_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(heat_timer_m_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(heat_timer_s_upDown_.Value);
      configuration += ",";
      configuration += record_filepath_textbox_.Text;
      configuration += ",";
      configuration += Convert.ToString(advanced_thermocouple_dropdown_.SelectedItem);
      configuration += ",";
      configuration += Convert.ToString(advanced_oversampling_dropdown_.SelectedItem);
      configuration += ",";
      configuration += Convert.ToString(advanced_TStepDuration_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(advanced_PID_proportional_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(advanced_PID_integral_upDown_.Value);
      configuration += ",";
      configuration += Convert.ToString(advanced_sampleOffset_upDown_.Value);
      return configuration;
    }

    public void setConfiguration(string configuration)
    {
      configuration = configuration.TrimEnd('\n');
      string[] configurationParts = configuration.Split(',');

      advanced_ID_upDown_.Value = Convert.ToDecimal(configurationParts[0]);
      DAQ_readSample_.Checked = Convert.ToBoolean(configurationParts[1]);
      heat_setpoint_upDown_.Value = Convert.ToDecimal(configurationParts[2]);
      heat_rate_upDown_.Value = Convert.ToDecimal(configurationParts[3]);
      heat_timer_h_upDown_.Value = Convert.ToDecimal(configurationParts[4]);
      heat_timer_m_upDown_.Value = Convert.ToDecimal(configurationParts[5]);
      heat_timer_s_upDown_.Value = Convert.ToDecimal(configurationParts[6]);
      record_filepath_textbox_.Text = configurationParts[7];
      savePathway_ = record_filepath_textbox_.Text;   // Just following the event that is triggered when the record file path is changed
      advanced_thermocouple_dropdown_.SelectedItem = configurationParts[8];
      advanced_thermocouple_dropdown_.SelectedValue = configurationParts[8];
      advanced_oversampling_dropdown_.SelectedItem = Convert.ToInt32(configurationParts[9]);
      advanced_oversampling_dropdown_.SelectedValue = Convert.ToInt32(configurationParts[9]);
      advanced_TStepDuration_upDown_.Value = Convert.ToDecimal(configurationParts[10]);
      advanced_PID_proportional_upDown_.Value = Convert.ToDecimal(configurationParts[11]);
      advanced_PID_integral_upDown_.Value = Convert.ToDecimal(configurationParts[12]);
      advanced_sampleOffset_upDown_.Value = Convert.ToDecimal(configurationParts[13]);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Event handlers
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    private void _chill_MouseWheel(object sender, MouseEventArgs e)
    {
      ((HandledMouseEventArgs)e).Handled = true;
    }

    private void _startDAQ_click(object sender, EventArgs e)
    {
      ChangeControlEnable(DAQ_startDAQ_, false);
      ChangeControlEnable(advanced_removeSandwich_, false);
      ChangeControlEnable(DAQ_readSample_, false);
      ChangeControlEnable(advanced_port_dropdown_, false);
      DAQActive_ = false;

      if (communications_.OpenPortConnection(GetComboSelectedItem(advanced_port_dropdown_)))
      {// Port was successfully opened; begin sending command to start DAQ
        // Start the sending and reading message threads
        commandInWorker_ = new Thread(communications_.handleCommandStatuses);
        commandInWorker_.IsBackground = true;
        commandOutWorker_ = new Thread(communications_.sendCommands);
        commandOutWorker_.IsBackground = true;
        commandInWorker_.Start();
        commandOutWorker_.Start();

        // Collect parameters for the DAQ
        string thermocoupleType;
        int samplesAverageCount;

        switch (GetComboSelectedItem(advanced_thermocouple_dropdown_))
        {
          case "B":
            thermocoupleType = "B";
            break;
          case "E":
            thermocoupleType = "E";
            break;
          case "J":
            thermocoupleType = "J";
            break;
          case "K":
            thermocoupleType = "K";
            break;
          case "N":
            thermocoupleType = "N";
            break;
          case "R":
            thermocoupleType = "R";
            break;
          case "S":
            thermocoupleType = "S";
            break;
          case "T":
          default:
            thermocoupleType = "T";
            break;
        }

        switch (Convert.ToUInt16(GetComboSelectedItem(advanced_oversampling_dropdown_)))
        {
          case 1:
            samplesAverageCount = 1;
            break;
          case 2:
            samplesAverageCount = 2;
            break;
          case 4:
            samplesAverageCount = 4;
            break;
          case 8:
            samplesAverageCount = 8;
            break;
          case 16:
          default:
            samplesAverageCount = 16;
            break;
        }

        // Send command
        communications_.QueueCommandStartDAQ(DAQSample_, thermocoupleType, samplesAverageCount);
      }
      else
      {// Failed to open port; revert changes to controls
        ChangeControlEnable(DAQ_startDAQ_, true);
        ChangeControlEnable(advanced_removeSandwich_, true);
        ChangeControlEnable(DAQ_readSample_, true);
      }
    }

    private void _stopDAQ_click(object sender, EventArgs e)
    {
      ChangeControlEnable(DAQ_stopDAQ_, false);
      ChangeControlEnable(record_startRecord_, false);
      ChangeControlEnable(record_stopRecord_, false);
      ChangeControlEnable(heat_startHeat_, false);
      ChangeControlEnable(heat_stopHeat_, false);
      DAQActive_ = true;

      // If user request to stop DAQ and we were recording, stop recording and flush
      if (recordActive_)
      {
        recordActive_ = false;

        try
        {
          CSVWriter_.Flush();
          CSVWriter_.Close();
        }
        catch (ObjectDisposedException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_CLOSED, err, err.Message);
        }
        catch (IOException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_IO, err, err.Message);
        }
        catch (EncoderFallbackException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_ENCODING, err, err.Message);
        }
        
        SuspendLayoutControl(record_flow_);
        ShowControl(record_startRecord_, record_flow_);
        HideControl(record_stopRecord_, record_flow_);
        ChangeControlBg(record_flow_, SystemColors.Control);
        ResumeLayoutControl(record_flow_);
      }
      
      // Now stop DAQ
      communications_.QueueCommandStopDAQ();
    }

    private void _sample_checkedChanged(object sender, EventArgs e)
    {
      if (DAQ_readSample_.Checked)
      {
        DAQSample_ = true;
        ChangeControlEnable(DAQ_sample_label_, true);
        ChangeControlEnable(DAQ_sample_textbox_, true);
      }
      else
      {
        DAQSample_ = false;
        ChangeControlEnable(DAQ_sample_label_, false);
        ChangeControlEnable(DAQ_sample_textbox_, false);
      }
    }

    private void _startHeat_click(object sender, EventArgs e)
    {
      ChangeControlEnable(heat_startHeat_, false);
      ChangeControlEnable(DAQ_stopDAQ_, false);
      ChangeControlEnable(heat_setpoint_upDown_, false);
      ChangeControlEnable(heat_rate_upDown_, false);
      ChangeControlEnable(heat_timer_h_upDown_, false);
      ChangeControlEnable(heat_timer_m_upDown_, false);
      ChangeControlEnable(heat_timer_s_upDown_, false);
      heatActive_ = false;

      heatingDuration_ = new TimeSpan(Convert.ToInt16(heat_timer_h_upDown_.Value), Convert.ToInt16(heat_timer_m_upDown_.Value), Convert.ToInt16(heat_timer_s_upDown_.Value));

      if (heatingDuration_.TotalSeconds > 0)
      {// Begin heating
        // Send command to start heating
        communications_.QueueCommandStartHeat(heat_setpoint_upDown_.Value, 
                                              heat_rate_upDown_.Value,
                                              heatingDuration_.TotalSeconds,
                                              advanced_PID_proportional_upDown_.Value,
                                              advanced_PID_integral_upDown_.Value);
      }
      else
      {// Heating duration is nil, so don't start the heating at all
        HandleStartHeatFail();
      }
    }

    private void _stopHeat_click(object sender, EventArgs e)
    {
      ChangeControlEnable(heat_stopHeat_, false);
      heatActive_ = true;
      communications_.QueueCommandStopHeat();
    }

    private void _recordFilePath_change(object sender, EventArgs e)
    {
      savePathway_ = record_filepath_textbox_.Text;
    }

    private void _recordBrowse_click(object sender, EventArgs e)
    {
      if (owningForm_.recordFileDialog.ShowDialog() == DialogResult.OK)
      {
        record_filepath_textbox_.Text = owningForm_.recordFileDialog.FileName;
        savePathway_ = record_filepath_textbox_.Text;
      }
    }

    private void _startRecord_click(object sender, EventArgs e)
    {
      ChangeControlEnable(record_startRecord_, false);
      bool legitFilePath_ = false;
      bool fileExists = File.Exists(savePathway_);
      recordActive_ = false;

      try
      {
        recordStream_ = new FileStream(savePathway_, FileMode.Append, FileAccess.Write, FileShare.Read); // append or create new file; write-only; don't allow anyone else to open this file
        CSVWriter_ = new StreamWriter(recordStream_, Encoding.GetEncoding("ISO-8859-15")); // Needs this encoding, otherwise the degree symbol doesn't come out properly
        CSVWriter_.AutoFlush = true;
        legitFilePath_ = true;
      }
      catch (Exception err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PATH, err, err.Message);
        ChangeControlEnable(record_startRecord_, true);
        UpdateTextBox(record_filepath_textbox_, "INVALID FILE PATH");
      }

      if (legitFilePath_)
      {// File path is valid; begin recording
        try
        {
          // Check if the file is already created. Add the header at first line if it is a new file.
          if (!fileExists)
          {
            if (DAQSample_)
            {
              CSVWriter_.Write("{0}{1}{2}{3}{4}{5}{6}{7}",
                                new Object[] {  "Time (ms)",
                                              ",",
                                              "Heater 1 temperature (°C)",
                                              ",",
                                              "Heater 2 temperature (°C)",
                                              ",",
                                              "Sample temperature (°C)",
                                              Environment.NewLine });
            }
            else
            {
              CSVWriter_.Write("{0}{1}{2}{3}{4}{5}",
                                new Object[] {  "Time (ms)",
                                              ",",
                                              "Heater 1 temperature (°C)",
                                              ",",
                                              "Heater 2 temperature (°C)",
                                              Environment.NewLine });
            }
          }

          // Set the flag
          recordActive_ = true;
        }
        catch (ArgumentNullException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_ARG, err, err.Message);
        }
        catch (ObjectDisposedException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_CLOSED, err, err.Message);
        }
        catch (FormatException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_FORMAT, err, err.Message);
        }
        catch (IOException err)
        {
          errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_IO, err, err.Message);
        }

        if (recordActive_)
        {// We only reach here if everything worked properly
          ChangeControlEnable(record_stopRecord_, true);
          SuspendLayoutControl(record_flow_);
          HideControl(record_startRecord_, record_flow_);
          ShowControl(record_stopRecord_, record_flow_);
          ChangeControlBg(record_flow_, Color.Gold);
          ResumeLayoutControl(record_flow_);

          // Determine the starting time.
          recordingStartTime_ = DateTime.Now;
          RecordData(recordingStartTime_);
        }
        else
        {// Failed to write headers to the file when we were supposed to; revert state to before clicking start record
          ChangeControlEnable(record_startRecord_, true);
        }
      }
      else
      {// Invalid file path; revert state to before clicking start record
        ChangeControlEnable(record_startRecord_, true);
      }
    }

    private void _stopRecord_click(object sender, EventArgs e)
    {
      ChangeControlEnable(record_stopRecord_, false);
      recordActive_ = false;

      try
      {
        CSVWriter_.Flush();
        CSVWriter_.Close();
      }
      catch (ObjectDisposedException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_CLOSED, err, err.Message);
      }
      catch (IOException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_IO, err, err.Message);
      }
      catch (EncoderFallbackException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_ENCODING, err, err.Message);
      }
      
      ChangeControlEnable(record_startRecord_, true); // re-enable the start recording button
      SuspendLayoutControl(record_flow_);
      ShowControl(record_startRecord_, record_flow_);
      HideControl(record_stopRecord_, record_flow_);
      ChangeControlBg(record_flow_, SystemColors.Control);
      ResumeLayoutControl(record_flow_);
    }

    private void _showAdvancedHiddenPanel_click(object sender, EventArgs e)
    {
      SuspendLayoutControl(advanced_flow_);
      HideControl(advanced_show_, advanced_flow_);
      ChangeFlowVisibility(advanced_hiddenFlow_, true);
      ResumeLayoutControl(advanced_flow_);
    }

    private void _hideAdvancedHiddenPanel_click(object sender, EventArgs e)
    {
      SuspendLayoutControl(advanced_flow_);
      ShowControl(advanced_show_, advanced_flow_);
      ChangeFlowVisibility(advanced_hiddenFlow_, false);
      ResumeLayoutControl(advanced_flow_);
    }

    private void _blinkLED_click(object sender, EventArgs e)
    {
      ChangeControlEnable(advanced_blinkLED_, false);
      communications_.QueueCommandBlink();
    }

    private void _updateSandwichID(object sender, EventArgs e)
    {
      ID_textbox_.Text = Convert.ToString(advanced_ID_upDown_.Value);
      sandwichID_ = Convert.ToInt32(advanced_ID_upDown_.Value);
    }

    private void _portValueChanged(object sender, EventArgs e)
    {
      COMPort_ = advanced_port_dropdown_.SelectedItem.ToString();
      owningForm_.updateOccupiedPort(sandwichControlID_, COMPort_);
      owningForm_.refreshPortList();
    }

    public void deleteSandwich(object sender, EventArgs e)
    {
      // TODO need to remove the DataReceived event handler for the port associated with this sandwich
      _deleteControl(sandwich_);
      owningForm_.exileSandwich(this);
      mainFlowPanel_.Size = new System.Drawing.Size(mainFlowPanel_.Size.Width, mainFlowPanel_.Size.Height - sandwichHeight_);
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////
    // Heavy-lifting functions
    //
    // Deez bois do all the heavy duty work.
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    // Record data to the opened file stream object
    private void RecordData(DateTime currentTime)
    {
      try
      {
        ulong milSecsElapsed = Convert.ToUInt64(currentTime.Subtract(recordingStartTime_).TotalMilliseconds);

        if (DAQSample_)
        {
          CSVWriter_.Write("{0}{1}{2}{3}{4}{5}{6}{7}",
                            new Object[] {milSecsElapsed,
                                          ",",
                                          heater1T_,
                                          ",",
                                          heater2T_,
                                          ",",
                                          sampleT_,
                                          Environment.NewLine });
        }
        else
        {
          CSVWriter_.Write("{0}{1}{2}{3}{4}{5}",
                            new Object[] {milSecsElapsed,
                                          ",",
                                          heater1T_,
                                          ",",
                                          heater2T_,
                                          Environment.NewLine });
        }
      }
      catch (ArgumentNullException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_ARG, err, err.Message);
      }
      catch (ObjectDisposedException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_CLOSED, err, err.Message);
      }
      catch (FormatException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_FORMAT, err, err.Message);
      }
      catch (IOException err)
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_WRITER_IO, err, err.Message);
      }
    }

    private void HandleNewReadings(string heater1T, string heater2T, bool sampleTAvailable, string sampleT = "")
    {
      string[] TList;
      string[] TNameList;
      TextBox[] TBoxList;

      if (sampleTAvailable)
      {
        TList = new string[] { heater1T, heater2T, sampleT };
        TNameList = new string[] { "heater1", "heater2", "sample" };
        TBoxList = new TextBox[] { DAQ_heater1_textbox_, DAQ_heater2_textbox_, DAQ_sample_textbox_ };
      }
      else
      {
        TList = new string[] { heater1T, heater2T };
        TNameList = new string[] { "heater1", "heater2" };
        TBoxList = new TextBox[] { DAQ_heater1_textbox_, DAQ_heater2_textbox_ };
      }

      // Begin converting the raw readings into numbers
      bool allReadingsOK = true;
      for (int i = 0; i < TList.Length; i++)
      {
        bool conversionSuccess = false;

        if (TList[i] != Communication.SERIAL_SEND_TEMP_ERROR)
        {
          try
          {
            switch (i)
            {
              case 0:
                heater1T_ = Convert.ToDouble(TList[i]);
                break;
              case 1:
                heater2T_ = Convert.ToDouble(TList[i]);
                break;
              case 2:
                sampleT_ = Convert.ToDouble(TList[i]);
                break;
              default:
                // Missed an index here...
                throw new Exception("Received temperature reading from a device that was not accounted for.");
            }

            conversionSuccess = true;
          }
          catch (Exception err)
          {// There was an error while trying to convert the reading into Double format
            TCAmplifierErr_ = true;
            allReadingsOK = false;
            UpdateTextBox(TBoxList[i], "Error");
            errorLogger_.logArduinoError(ErrorLogger.ERR_ARDUINO_TCAMPLIFIER_FORMAT, sandwichID_, ErrorLogger.DEVICE_HEATER1TCAMPLIFIER, "UpdateReadings, " + TNameList[i], err.Message);
            
            // If we are in DAQ mode only, prevent user from starting heat until the TC error is resolved.
            // If we are in heating mode, the Arduino itself stops the heat, although the heating timer is still ticking.
            ChangeControlEnable(heat_startHeat_, false);
          }

          if (conversionSuccess)
          {// Display the value only if it is actually a number
            UpdateTextBox(TBoxList[i], TList[i]);
          }
        }
        else
        {// The thermocouple had an error, which was already sent before by another message (SERIAL_REPLY_ERROR)
          TCAmplifierErr_ = true;
          allReadingsOK = false;
          UpdateTextBox(TBoxList[i], "Error");
        }
      }

      if (TCAmplifierErr_ && allReadingsOK)
      {// If there was previously an error with TC amplifier but we just received a completely good set of readings now, that means the error has been resolved.
        TCAmplifierErr_ = false;

        if (!heatActive_)
        {// If we haven't started heating, re-enable the start heat button which was disabled previously when a thermocouple error was found.
          ChangeControlEnable(heat_startHeat_, true);
        }
      }

      if (heatActive_)
      {
        // Obtain the time elapsed since the previously received data and calculate remaining time for heating.
        TimeSpan heatingElapsedTime = DateTime.Now.Subtract(heatingStartTime_);
        TimeSpan heatingTimeRemaining = heatingDuration_.Subtract(heatingElapsedTime);
        if (heatingTimeRemaining.TotalSeconds < 0) { heatingTimeRemaining = TimeSpan.Zero; } // Force it to be at least 0
        UpdateControlValue(heat_timer_h_upDown_, heatingTimeRemaining.Hours);
        UpdateControlValue(heat_timer_m_upDown_, heatingTimeRemaining.Minutes);
        UpdateControlValue(heat_timer_s_upDown_, heatingTimeRemaining.Seconds);
      }

      // Record the data if applicable
      if (recordActive_)
      {
          RecordData(DateTime.Now);
      }
    }

    // Handle the situation when DAQ was successfully started
    private void HandleStartDAQSuccess()
    {
      DAQActive_ = true;
      TCAmplifierErr_ = false;
      ChangeControlEnable(record_startRecord_, true);
      ChangeControlEnable(heat_startHeat_, true);
      ChangeControlEnable(DAQ_stopDAQ_, true);
      SuspendLayoutControl(DAQ_flow_);
      HideControl(DAQ_startDAQ_, DAQ_flow_);
      ShowControl(DAQ_stopDAQ_, DAQ_flow_);
      ChangeControlBg(DAQ_flow_, Color.LightGreen);
      UpdateTextBox(DAQ_heater1_textbox_, "");
      UpdateTextBox(DAQ_heater2_textbox_, "");
      UpdateTextBox(DAQ_sample_textbox_, "");
      ResumeLayoutControl(DAQ_flow_);
      ChangeControlEnable(advanced_blinkLED_, true);
    }

    // Handle the situation when there was a problem starting DAQ
    private void HandleStartDAQFail()
    {
      // Close the serial connection
      try
      {
        Shutdown();
      }
      catch (Exception err)
      {
        errorLogger_.logUnknownError(err);
      }

      DAQActive_ = false;
      ChangeControlEnable(DAQ_startDAQ_, true);
      ChangeControlEnable(advanced_removeSandwich_, true);
      ChangeControlEnable(DAQ_readSample_, true);
      ChangeControlEnable(advanced_port_dropdown_, true);
    }

    // Handle the situation when DAQ was successfully stopped
    private void HandleStopDAQSuccess()
    {
      // Close the serial connection
      try
      {
        Shutdown();
      }
      catch (Exception err)
      {
        errorLogger_.logUnknownError(err);
      }

      DAQActive_ = false;
      ChangeControlEnable(DAQ_startDAQ_, true); // re-enable the start DAQ button
      ChangeControlEnable(advanced_removeSandwich_, true);
      ChangeControlEnable(DAQ_readSample_, true);
      ChangeControlEnable(advanced_blinkLED_, false);
      SuspendLayoutControl(DAQ_flow_);
      ShowControl(DAQ_startDAQ_, DAQ_flow_);
      HideControl(DAQ_stopDAQ_, DAQ_flow_);
      ChangeControlBg(DAQ_flow_, SystemColors.Control);
      ResumeLayoutControl(DAQ_flow_);
      ChangeControlEnable(advanced_port_dropdown_, true);
    }

    // Handle the situation when there was a problem stopping DAQ
    private void HandleStopDAQFail()
    {
      DAQActive_ = true;
      ChangeControlEnable(DAQ_stopDAQ_, true);
      ChangeControlEnable(record_startRecord_, true);
      ChangeControlEnable(record_stopRecord_, true);
      ChangeControlEnable(heat_startHeat_, true);
      ChangeControlEnable(heat_stopHeat_, true);
    }

    // Handle the situation when heating was successfully started
    private void HandleStartHeatSuccess()
    {
      heatingStartTime_ = DateTime.Now;
      heatActive_ = true;
      HideControl(heat_startHeat_, heat_flow_);
      ShowControl(heat_stopHeat_, heat_flow_);
      ChangeControlEnable(heat_stopHeat_, true);
      ChangeControlBg(heat_flow_, Color.Red);
    }

    // Handle the situation when there was a problem starting heating
    private void HandleStartHeatFail()
    {
      heatActive_ = false;
      ChangeControlEnable(heat_startHeat_, true);
      ChangeControlEnable(DAQ_stopDAQ_, true);
      ChangeControlEnable(heat_setpoint_upDown_, true);
      ChangeControlEnable(heat_rate_upDown_, true);
      ChangeControlEnable(heat_timer_h_upDown_, true);
      ChangeControlEnable(heat_timer_m_upDown_, true);
      ChangeControlEnable(heat_timer_s_upDown_, true);
    }

    // Handle the situation when heating was successfully stopped
    private void HandleStopHeatSuccess()
    {
      heatActive_ = false;
      ChangeControlEnable(heat_setpoint_upDown_, true);
      ChangeControlEnable(heat_rate_upDown_, true);
      ChangeControlEnable(heat_timer_h_upDown_, true);
      ChangeControlEnable(heat_timer_m_upDown_, true);
      ChangeControlEnable(heat_timer_s_upDown_, true);
      HideControl(heat_stopHeat_, heat_flow_);
      ShowControl(heat_startHeat_, heat_flow_);
      ChangeControlEnable(heat_startHeat_, true);
      ChangeControlEnable(DAQ_stopDAQ_, true);
      ChangeControlBg(heat_flow_, SystemColors.Control);
    }

    // Handle the situation when there was a problem stopping heating
    private void HandleStopHeatFail()
    {
      ChangeControlEnable(heat_stopHeat_, true);
    }

    // Handle the situation when heating has been completed
    private void HandleHeatingComplete()
    {
      heatActive_ = false;

      // Reset the timers to 0
      UpdateControlValue(heat_timer_h_upDown_, 0);
      UpdateControlValue(heat_timer_m_upDown_, 0);
      UpdateControlValue(heat_timer_s_upDown_, 0);

      ChangeControlEnable(heat_setpoint_upDown_, true);
      ChangeControlEnable(heat_rate_upDown_, true);
      ChangeControlEnable(heat_timer_h_upDown_, true);
      ChangeControlEnable(heat_timer_m_upDown_, true);
      ChangeControlEnable(heat_timer_s_upDown_, true);
      HideControl(heat_stopHeat_, heat_flow_);
      ShowControl(heat_startHeat_, heat_flow_);
      ChangeControlEnable(heat_stopHeat_, false);
      ChangeControlEnable(heat_startHeat_, true);
      ChangeControlEnable(DAQ_stopDAQ_, true);
      ChangeControlBg(heat_flow_, SystemColors.Control);
    }

    // Handle the situation when the LED was successfully blinked
    private void HandleBlinkLEDSuccess()
    {
      ChangeControlEnable(advanced_blinkLED_, true);
    }

    // Handle the situation when there was a problem blinking the LED
    private void HandleBlinkLEDFail()
    {
      ChangeControlEnable(advanced_blinkLED_, true);
    }

    // Replace the contents of one of the temperature readings textbox with "Error"
    private void UpdateControlsTCAmplifierError(int TCAmplifierIndex)
    {
      TextBox[] TBoxList = new TextBox[] { DAQ_heater1_textbox_, DAQ_heater2_textbox_, DAQ_sample_textbox_ };
      UpdateTextBox(TBoxList[TCAmplifierIndex], "Error");
    }

    // Stop operation of the sandwich
    public void Shutdown()
    {
      // Stop all sandwich activity, if any
      communications_.SendCommandShutdown();

      // Close the port
      if (!communications_.ClosePortConnection())
      {
        errorLogger_.logCProgError(ErrorLogger.ERR_PROG_GENERAL_PORT_CANTCLOSE, "CloseConnection");
      }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    // Control-modifying functions
    //
    // Attempts to modify a control in a thread separate from the thread running the form will result in an error
    // because it is an unsafe practice. These functions ensure that we can modify controls while sticking to
    // safe practices.
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SuspendLayoutControl(Control refControl)
    {
      if (refControl.InvokeRequired)
      {
        _suspendLayoutControlCallback d = new _suspendLayoutControlCallback(SuspendLayoutControl);
        owningForm_.Invoke(d, new object[] { refControl });
      }
      else
      {
        refControl.SuspendLayout();
      }
    }

    private void ResumeLayoutControl(Control refControl)
    {
      if (refControl.InvokeRequired)
      {
        _resumeLayoutControlCallback d = new _resumeLayoutControlCallback(ResumeLayoutControl);
        owningForm_.Invoke(d, new object[] { refControl });
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
        owningForm_.Invoke(d, new object[] { refControl });
      }
      else
      {
        refControl.PerformLayout();
      }
    }

    private void ChangeFlowVisibility(FlowLayoutPanel flowPanel, bool visibility)
    {
      if (flowPanel.InvokeRequired)
      {
        _changeFlowVisibilityCallback d = new _changeFlowVisibilityCallback(ChangeFlowVisibility);
        owningForm_.Invoke(d, new object[] { flowPanel, visibility });
      }
      else
      {
        flowPanel.Visible = visibility;
      }
    }

    private void ChangeControlEnable(Control refControl, bool enable)
    {
      if (refControl.InvokeRequired)
      {
        _changeControlEnableCallback d = new _changeControlEnableCallback(ChangeControlEnable);
        owningForm_.Invoke(d, new object[] { refControl, enable });
      }
      else
      {
        refControl.Enabled = enable;
      }
    }

    private void ShowControl(Control refControl, Control parentControl)
    {
      if (refControl.InvokeRequired || parentControl.InvokeRequired)
      {
        _showControlCallback d = new _showControlCallback(ShowControl);
        owningForm_.Invoke(d, new object[] { refControl, parentControl });
      }
      else
      {
        parentControl.Controls.Add(refControl);
        refControl.Visible = true;
      }
    }

    private void HideControl(Control refControl, Control parentControl)
    {
      if (refControl.InvokeRequired || parentControl.InvokeRequired)
      {
        _hideControlCallback d = new _hideControlCallback(HideControl);
        owningForm_.Invoke(d, new object[] { refControl, parentControl });
      }
      else
      {
        parentControl.Controls.Remove(refControl);
        refControl.Visible = false;
      }
    }

    private void UpdateTextBox(TextBox boxControl, string data)
    {
      if (boxControl.InvokeRequired)
      {
        _updateTextboxCallback d = new _updateTextboxCallback(UpdateTextBox);
        owningForm_.Invoke(d, new object[] { boxControl, data });
      }
      else
      {
        boxControl.Text = data;
      }
    }

    private void UpdateControlValue(NumericUpDown refControl, decimal data)
    {
      if (refControl.InvokeRequired)
      {
        _updateControlValueCallback d = new _updateControlValueCallback(UpdateControlValue);
        owningForm_.Invoke(d, new object[] { refControl, data });
      }
      else
      {
        refControl.Value = data;
      }
    }

    private string GetComboSelectedItem(ComboBox comboControl)
    {
      if (comboControl.InvokeRequired)
      {
        _getComboSelectedItemCallback d = new _getComboSelectedItemCallback(GetComboSelectedItem);
        string selectedItem = (string)owningForm_.Invoke(d, new object[] { comboControl });
        return selectedItem;
      }
      else
      {
        return Convert.ToString(comboControl.SelectedItem);
      }
    }

    private void ChangeControlBg(Control refControl, Color refColor)
    {
      if (refControl.InvokeRequired)
      {
        _changeControlBgCallback d = new _changeControlBgCallback(ChangeControlBg);
        owningForm_.Invoke(d, new object[] { refControl, refColor });
      }
      else
      {
        refControl.BackColor = refColor;
      }
    }

    private void _deleteControl(Control refControl)
    {// This one doesn't require the invoke workaround because the remove sandwich action is only performed by the user (and thus, on the main thread)
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
  }// END Sandwich class def.

}
