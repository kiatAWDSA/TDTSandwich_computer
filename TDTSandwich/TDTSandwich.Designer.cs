namespace TDTSandwich
{
    partial class TDTSandwich
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TDTSandwich));
      this.sandwich_0_DAQ_startDAQ = new System.Windows.Forms.Button();
      this.sandwich_0_DAQ_heater_textbox = new System.Windows.Forms.TextBox();
      this.sandwich_0_DAQ_heater_label = new System.Windows.Forms.Label();
      this.sandwich_0_DAQ_stopDAQ = new System.Windows.Forms.Button();
      this.sandwich_0_record_startRecord = new System.Windows.Forms.Button();
      this.sandwich_0_record_stopRecord = new System.Windows.Forms.Button();
      this.sandwich_0_heat_stopHeat = new System.Windows.Forms.Button();
      this.sandwich_0_heat_startHeat = new System.Windows.Forms.Button();
      this.sandwich_0_advanced_thermocouple_dropdown = new System.Windows.Forms.ComboBox();
      this.sandwich_0_advanced_blinkLED = new System.Windows.Forms.Button();
      this.sandwich_0_ID = new System.Windows.Forms.GroupBox();
      this.sandwich_0_ID_flow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_advanced_ID_upDown = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_DAQ = new System.Windows.Forms.GroupBox();
      this.sandwich_0_DAQ_flow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_DAQ_heater = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_DAQ_heater2 = new System.Windows.Forms.FlowLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.sandwich_0_DAQ_heater2_textbox = new System.Windows.Forms.TextBox();
      this.sandwich_0_DAQ_readSample = new System.Windows.Forms.CheckBox();
      this.sandwich_0_DAQ_sample = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_DAQ_sample_label = new System.Windows.Forms.Label();
      this.sandwich_0_DAQ_sample_textbox = new System.Windows.Forms.TextBox();
      this.sandwich_0_heat = new System.Windows.Forms.GroupBox();
      this.sandwich_0_heat_flow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_heat_setpoint = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_heat_setpoint_label = new System.Windows.Forms.Label();
      this.sandwich_0_heat_setpoint_upDown = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_heat_maxRate = new System.Windows.Forms.CheckBox();
      this.sandwich_0_heat_rate = new System.Windows.Forms.FlowLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_heat_timer = new System.Windows.Forms.GroupBox();
      this.sandwich_0_heat_timer_flow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_heat_timer_h_upDown = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_heat_timer_h_label = new System.Windows.Forms.Label();
      this.sandwich_0_heat_timer_m_upDown = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_heat_timer_m_label = new System.Windows.Forms.Label();
      this.sandwich_0_heat_timer_s_upDown = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_heat_timer_s_label = new System.Windows.Forms.Label();
      this.sandwich_0_record = new System.Windows.Forms.GroupBox();
      this.sandwich_0_record_flow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_record_filepath = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_record_filepath_label = new System.Windows.Forms.Label();
      this.sandwich_0_record_filepath_textbox = new System.Windows.Forms.TextBox();
      this.sandwich_0_record_browse = new System.Windows.Forms.Button();
      this.sandwich_0_advanced = new System.Windows.Forms.GroupBox();
      this.sandwich_0_advanced_table = new System.Windows.Forms.TableLayoutPanel();
      this.sandwich_0_advanced_hiddenFlow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_advanced_removeSandwich = new System.Windows.Forms.Button();
      this.sandwich_0_advanced_port = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_advanced_port_label = new System.Windows.Forms.Label();
      this.sandwich_0_advanced_port_dropdown = new System.Windows.Forms.ComboBox();
      this.sandwich_0_advanced_thermocouple = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0_advanced_thermocouple_label = new System.Windows.Forms.Label();
      this.sandwich_0_advanced_oversampling = new System.Windows.Forms.FlowLayoutPanel();
      this.label3 = new System.Windows.Forms.Label();
      this.sandwich_0_advanced_oversampling_dropdown = new System.Windows.Forms.ComboBox();
      this.sandwich_0_advanced_PID_proportional = new System.Windows.Forms.FlowLayoutPanel();
      this.label4 = new System.Windows.Forms.Label();
      this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_advanced_PID_integral = new System.Windows.Forms.FlowLayoutPanel();
      this.label5 = new System.Windows.Forms.Label();
      this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
      this.sandwich_0_advanced_show = new System.Windows.Forms.Button();
      this.sandwich_0_advanced_hide = new System.Windows.Forms.Button();
      this.mainFlow = new System.Windows.Forms.FlowLayoutPanel();
      this.sandwich_0 = new System.Windows.Forms.TableLayoutPanel();
      this.menu = new System.Windows.Forms.MenuStrip();
      this.menu_config = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_config_addSandwich = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_config_openConfig = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_config_saveConfig = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_port = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_port_refresh = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_port_autoFill = new System.Windows.Forms.ToolStripMenuItem();
      this.menu_copyright = new System.Windows.Forms.ToolStripTextBox();
      this.openConfigFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveConfigFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.recordFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.sandwich_0_ID.SuspendLayout();
      this.sandwich_0_ID_flow.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_advanced_ID_upDown)).BeginInit();
      this.sandwich_0_DAQ.SuspendLayout();
      this.sandwich_0_DAQ_flow.SuspendLayout();
      this.sandwich_0_DAQ_heater.SuspendLayout();
      this.sandwich_0_DAQ_heater2.SuspendLayout();
      this.sandwich_0_DAQ_sample.SuspendLayout();
      this.sandwich_0_heat.SuspendLayout();
      this.sandwich_0_heat_flow.SuspendLayout();
      this.sandwich_0_heat_setpoint.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_setpoint_upDown)).BeginInit();
      this.sandwich_0_heat_rate.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
      this.sandwich_0_heat_timer.SuspendLayout();
      this.sandwich_0_heat_timer_flow.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_timer_h_upDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_timer_m_upDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_timer_s_upDown)).BeginInit();
      this.sandwich_0_record.SuspendLayout();
      this.sandwich_0_record_flow.SuspendLayout();
      this.sandwich_0_record_filepath.SuspendLayout();
      this.sandwich_0_advanced.SuspendLayout();
      this.sandwich_0_advanced_table.SuspendLayout();
      this.sandwich_0_advanced_hiddenFlow.SuspendLayout();
      this.sandwich_0_advanced_port.SuspendLayout();
      this.sandwich_0_advanced_thermocouple.SuspendLayout();
      this.sandwich_0_advanced_oversampling.SuspendLayout();
      this.sandwich_0_advanced_PID_proportional.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
      this.sandwich_0_advanced_PID_integral.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
      this.mainFlow.SuspendLayout();
      this.sandwich_0.SuspendLayout();
      this.menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // sandwich_0_DAQ_startDAQ
      // 
      this.sandwich_0_DAQ_startDAQ.Location = new System.Drawing.Point(281, 11);
      this.sandwich_0_DAQ_startDAQ.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
      this.sandwich_0_DAQ_startDAQ.Name = "sandwich_0_DAQ_startDAQ";
      this.sandwich_0_DAQ_startDAQ.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_DAQ_startDAQ.TabIndex = 4;
      this.sandwich_0_DAQ_startDAQ.Text = "Start DAQ";
      this.sandwich_0_DAQ_startDAQ.UseVisualStyleBackColor = true;
      this.sandwich_0_DAQ_startDAQ.Visible = false;
      // 
      // sandwich_0_DAQ_heater_textbox
      // 
      this.sandwich_0_DAQ_heater_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_DAQ_heater_textbox.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_DAQ_heater_textbox.MaxLength = 6;
      this.sandwich_0_DAQ_heater_textbox.Name = "sandwich_0_DAQ_heater_textbox";
      this.sandwich_0_DAQ_heater_textbox.ReadOnly = true;
      this.sandwich_0_DAQ_heater_textbox.Size = new System.Drawing.Size(60, 26);
      this.sandwich_0_DAQ_heater_textbox.TabIndex = 1;
      this.sandwich_0_DAQ_heater_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_DAQ_heater_label
      // 
      this.sandwich_0_DAQ_heater_label.AutoSize = true;
      this.sandwich_0_DAQ_heater_label.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_DAQ_heater_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.sandwich_0_DAQ_heater_label.Name = "sandwich_0_DAQ_heater_label";
      this.sandwich_0_DAQ_heater_label.Size = new System.Drawing.Size(68, 13);
      this.sandwich_0_DAQ_heater_label.TabIndex = 0;
      this.sandwich_0_DAQ_heater_label.Text = "Heater 1 (°C)";
      // 
      // sandwich_0_DAQ_stopDAQ
      // 
      this.sandwich_0_DAQ_stopDAQ.Enabled = false;
      this.sandwich_0_DAQ_stopDAQ.Location = new System.Drawing.Point(332, 11);
      this.sandwich_0_DAQ_stopDAQ.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
      this.sandwich_0_DAQ_stopDAQ.Name = "sandwich_0_DAQ_stopDAQ";
      this.sandwich_0_DAQ_stopDAQ.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_DAQ_stopDAQ.TabIndex = 5;
      this.sandwich_0_DAQ_stopDAQ.Text = "Stop DAQ";
      this.sandwich_0_DAQ_stopDAQ.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_record_startRecord
      // 
      this.sandwich_0_record_startRecord.Enabled = false;
      this.sandwich_0_record_startRecord.Location = new System.Drawing.Point(183, 11);
      this.sandwich_0_record_startRecord.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_record_startRecord.Name = "sandwich_0_record_startRecord";
      this.sandwich_0_record_startRecord.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_record_startRecord.TabIndex = 2;
      this.sandwich_0_record_startRecord.Text = "Start record";
      this.sandwich_0_record_startRecord.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_record_stopRecord
      // 
      this.sandwich_0_record_stopRecord.Enabled = false;
      this.sandwich_0_record_stopRecord.Location = new System.Drawing.Point(234, 11);
      this.sandwich_0_record_stopRecord.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_record_stopRecord.Name = "sandwich_0_record_stopRecord";
      this.sandwich_0_record_stopRecord.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_record_stopRecord.TabIndex = 3;
      this.sandwich_0_record_stopRecord.Text = "Stop record";
      this.sandwich_0_record_stopRecord.UseVisualStyleBackColor = true;
      this.sandwich_0_record_stopRecord.Visible = false;
      // 
      // sandwich_0_heat_stopHeat
      // 
      this.sandwich_0_heat_stopHeat.Enabled = false;
      this.sandwich_0_heat_stopHeat.Location = new System.Drawing.Point(478, 11);
      this.sandwich_0_heat_stopHeat.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_heat_stopHeat.Name = "sandwich_0_heat_stopHeat";
      this.sandwich_0_heat_stopHeat.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_heat_stopHeat.TabIndex = 5;
      this.sandwich_0_heat_stopHeat.Text = "Stop heat";
      this.sandwich_0_heat_stopHeat.UseVisualStyleBackColor = true;
      this.sandwich_0_heat_stopHeat.Visible = false;
      // 
      // sandwich_0_heat_startHeat
      // 
      this.sandwich_0_heat_startHeat.Enabled = false;
      this.sandwich_0_heat_startHeat.Location = new System.Drawing.Point(427, 11);
      this.sandwich_0_heat_startHeat.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_heat_startHeat.Name = "sandwich_0_heat_startHeat";
      this.sandwich_0_heat_startHeat.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_heat_startHeat.TabIndex = 4;
      this.sandwich_0_heat_startHeat.Text = "Start heat";
      this.sandwich_0_heat_startHeat.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_advanced_thermocouple_dropdown
      // 
      this.sandwich_0_advanced_thermocouple_dropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sandwich_0_advanced_thermocouple_dropdown.FormattingEnabled = true;
      this.sandwich_0_advanced_thermocouple_dropdown.Items.AddRange(new object[] {
            "B",
            "E",
            "J",
            "K",
            "N",
            "R",
            "S",
            "T"});
      this.sandwich_0_advanced_thermocouple_dropdown.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_advanced_thermocouple_dropdown.Name = "sandwich_0_advanced_thermocouple_dropdown";
      this.sandwich_0_advanced_thermocouple_dropdown.Size = new System.Drawing.Size(75, 21);
      this.sandwich_0_advanced_thermocouple_dropdown.TabIndex = 2;
      this.sandwich_0_advanced_thermocouple_dropdown.Tag = "";
      // 
      // sandwich_0_advanced_blinkLED
      // 
      this.sandwich_0_advanced_blinkLED.Location = new System.Drawing.Point(3, 11);
      this.sandwich_0_advanced_blinkLED.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_advanced_blinkLED.Name = "sandwich_0_advanced_blinkLED";
      this.sandwich_0_advanced_blinkLED.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_advanced_blinkLED.TabIndex = 0;
      this.sandwich_0_advanced_blinkLED.Text = "Blink LED";
      this.sandwich_0_advanced_blinkLED.UseVisualStyleBackColor = true;
      this.sandwich_0_advanced_blinkLED.Visible = false;
      // 
      // sandwich_0_ID
      // 
      this.sandwich_0_ID.AutoSize = true;
      this.sandwich_0_ID.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_ID.Controls.Add(this.sandwich_0_ID_flow);
      this.sandwich_0_ID.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_ID.Location = new System.Drawing.Point(3, 0);
      this.sandwich_0_ID.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_ID.Name = "sandwich_0_ID";
      this.sandwich_0_ID.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_ID.Size = new System.Drawing.Size(57, 62);
      this.sandwich_0_ID.TabIndex = 0;
      this.sandwich_0_ID.TabStop = false;
      this.sandwich_0_ID.Text = "ID";
      // 
      // sandwich_0_ID_flow
      // 
      this.sandwich_0_ID_flow.AutoSize = true;
      this.sandwich_0_ID_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_ID_flow.Controls.Add(this.sandwich_0_advanced_ID_upDown);
      this.sandwich_0_ID_flow.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_ID_flow.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_ID_flow.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_ID_flow.Name = "sandwich_0_ID_flow";
      this.sandwich_0_ID_flow.Size = new System.Drawing.Size(51, 49);
      this.sandwich_0_ID_flow.TabIndex = 0;
      // 
      // sandwich_0_advanced_ID_upDown
      // 
      this.sandwich_0_advanced_ID_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_advanced_ID_upDown.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_advanced_ID_upDown.Margin = new System.Windows.Forms.Padding(3, 13, 3, 3);
      this.sandwich_0_advanced_ID_upDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
      this.sandwich_0_advanced_ID_upDown.Name = "sandwich_0_advanced_ID_upDown";
      this.sandwich_0_advanced_ID_upDown.Size = new System.Drawing.Size(45, 29);
      this.sandwich_0_advanced_ID_upDown.TabIndex = 0;
      this.sandwich_0_advanced_ID_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.sandwich_0_advanced_ID_upDown.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
      // 
      // sandwich_0_DAQ
      // 
      this.sandwich_0_DAQ.AutoSize = true;
      this.sandwich_0_DAQ.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_DAQ.Controls.Add(this.sandwich_0_DAQ_flow);
      this.sandwich_0_DAQ.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_DAQ.Location = new System.Drawing.Point(66, 0);
      this.sandwich_0_DAQ.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_DAQ.Name = "sandwich_0_DAQ";
      this.sandwich_0_DAQ.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_DAQ.Size = new System.Drawing.Size(386, 62);
      this.sandwich_0_DAQ.TabIndex = 1;
      this.sandwich_0_DAQ.TabStop = false;
      this.sandwich_0_DAQ.Text = "DAQ";
      // 
      // sandwich_0_DAQ_flow
      // 
      this.sandwich_0_DAQ_flow.AutoSize = true;
      this.sandwich_0_DAQ_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_DAQ_flow.Controls.Add(this.sandwich_0_DAQ_heater);
      this.sandwich_0_DAQ_flow.Controls.Add(this.sandwich_0_DAQ_heater2);
      this.sandwich_0_DAQ_flow.Controls.Add(this.sandwich_0_DAQ_readSample);
      this.sandwich_0_DAQ_flow.Controls.Add(this.sandwich_0_DAQ_sample);
      this.sandwich_0_DAQ_flow.Controls.Add(this.sandwich_0_DAQ_startDAQ);
      this.sandwich_0_DAQ_flow.Controls.Add(this.sandwich_0_DAQ_stopDAQ);
      this.sandwich_0_DAQ_flow.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_DAQ_flow.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_DAQ_flow.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_DAQ_flow.Name = "sandwich_0_DAQ_flow";
      this.sandwich_0_DAQ_flow.Size = new System.Drawing.Size(380, 49);
      this.sandwich_0_DAQ_flow.TabIndex = 0;
      // 
      // sandwich_0_DAQ_heater
      // 
      this.sandwich_0_DAQ_heater.AutoSize = true;
      this.sandwich_0_DAQ_heater.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_DAQ_heater.Controls.Add(this.sandwich_0_DAQ_heater_label);
      this.sandwich_0_DAQ_heater.Controls.Add(this.sandwich_0_DAQ_heater_textbox);
      this.sandwich_0_DAQ_heater.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_DAQ_heater.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_DAQ_heater.Location = new System.Drawing.Point(0, 0);
      this.sandwich_0_DAQ_heater.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_DAQ_heater.Name = "sandwich_0_DAQ_heater";
      this.sandwich_0_DAQ_heater.Size = new System.Drawing.Size(74, 48);
      this.sandwich_0_DAQ_heater.TabIndex = 0;
      // 
      // sandwich_0_DAQ_heater2
      // 
      this.sandwich_0_DAQ_heater2.AutoSize = true;
      this.sandwich_0_DAQ_heater2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_DAQ_heater2.Controls.Add(this.label1);
      this.sandwich_0_DAQ_heater2.Controls.Add(this.sandwich_0_DAQ_heater2_textbox);
      this.sandwich_0_DAQ_heater2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_DAQ_heater2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_DAQ_heater2.Location = new System.Drawing.Point(74, 0);
      this.sandwich_0_DAQ_heater2.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_DAQ_heater2.Name = "sandwich_0_DAQ_heater2";
      this.sandwich_0_DAQ_heater2.Size = new System.Drawing.Size(74, 48);
      this.sandwich_0_DAQ_heater2.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 3);
      this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(68, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Heater 2 (°C)";
      // 
      // sandwich_0_DAQ_heater2_textbox
      // 
      this.sandwich_0_DAQ_heater2_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_DAQ_heater2_textbox.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_DAQ_heater2_textbox.MaxLength = 6;
      this.sandwich_0_DAQ_heater2_textbox.Name = "sandwich_0_DAQ_heater2_textbox";
      this.sandwich_0_DAQ_heater2_textbox.ReadOnly = true;
      this.sandwich_0_DAQ_heater2_textbox.Size = new System.Drawing.Size(60, 26);
      this.sandwich_0_DAQ_heater2_textbox.TabIndex = 1;
      this.sandwich_0_DAQ_heater2_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_DAQ_readSample
      // 
      this.sandwich_0_DAQ_readSample.AutoSize = true;
      this.sandwich_0_DAQ_readSample.Location = new System.Drawing.Point(151, 12);
      this.sandwich_0_DAQ_readSample.Margin = new System.Windows.Forms.Padding(3, 12, 0, 0);
      this.sandwich_0_DAQ_readSample.Name = "sandwich_0_DAQ_readSample";
      this.sandwich_0_DAQ_readSample.Size = new System.Drawing.Size(59, 30);
      this.sandwich_0_DAQ_readSample.TabIndex = 2;
      this.sandwich_0_DAQ_readSample.Text = "Read\r\nsample";
      this.sandwich_0_DAQ_readSample.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_DAQ_sample
      // 
      this.sandwich_0_DAQ_sample.AutoSize = true;
      this.sandwich_0_DAQ_sample.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_DAQ_sample.Controls.Add(this.sandwich_0_DAQ_sample_label);
      this.sandwich_0_DAQ_sample.Controls.Add(this.sandwich_0_DAQ_sample_textbox);
      this.sandwich_0_DAQ_sample.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_DAQ_sample.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_DAQ_sample.Location = new System.Drawing.Point(210, 0);
      this.sandwich_0_DAQ_sample.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_DAQ_sample.Name = "sandwich_0_DAQ_sample";
      this.sandwich_0_DAQ_sample.Size = new System.Drawing.Size(68, 48);
      this.sandwich_0_DAQ_sample.TabIndex = 3;
      // 
      // sandwich_0_DAQ_sample_label
      // 
      this.sandwich_0_DAQ_sample_label.AutoSize = true;
      this.sandwich_0_DAQ_sample_label.Enabled = false;
      this.sandwich_0_DAQ_sample_label.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_DAQ_sample_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.sandwich_0_DAQ_sample_label.Name = "sandwich_0_DAQ_sample_label";
      this.sandwich_0_DAQ_sample_label.Size = new System.Drawing.Size(62, 13);
      this.sandwich_0_DAQ_sample_label.TabIndex = 0;
      this.sandwich_0_DAQ_sample_label.Text = "Sample (°C)";
      // 
      // sandwich_0_DAQ_sample_textbox
      // 
      this.sandwich_0_DAQ_sample_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_DAQ_sample_textbox.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_DAQ_sample_textbox.MaxLength = 6;
      this.sandwich_0_DAQ_sample_textbox.Name = "sandwich_0_DAQ_sample_textbox";
      this.sandwich_0_DAQ_sample_textbox.ReadOnly = true;
      this.sandwich_0_DAQ_sample_textbox.Size = new System.Drawing.Size(60, 26);
      this.sandwich_0_DAQ_sample_textbox.TabIndex = 1;
      this.sandwich_0_DAQ_sample_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_heat
      // 
      this.sandwich_0_heat.AutoSize = true;
      this.sandwich_0_heat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_heat.Controls.Add(this.sandwich_0_heat_flow);
      this.sandwich_0_heat.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_heat.Location = new System.Drawing.Point(458, 0);
      this.sandwich_0_heat.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_heat.Name = "sandwich_0_heat";
      this.sandwich_0_heat.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_heat.Size = new System.Drawing.Size(532, 62);
      this.sandwich_0_heat.TabIndex = 2;
      this.sandwich_0_heat.TabStop = false;
      this.sandwich_0_heat.Text = "Heat";
      // 
      // sandwich_0_heat_flow
      // 
      this.sandwich_0_heat_flow.AutoSize = true;
      this.sandwich_0_heat_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_heat_flow.Controls.Add(this.sandwich_0_heat_setpoint);
      this.sandwich_0_heat_flow.Controls.Add(this.sandwich_0_heat_maxRate);
      this.sandwich_0_heat_flow.Controls.Add(this.sandwich_0_heat_rate);
      this.sandwich_0_heat_flow.Controls.Add(this.sandwich_0_heat_timer);
      this.sandwich_0_heat_flow.Controls.Add(this.sandwich_0_heat_startHeat);
      this.sandwich_0_heat_flow.Controls.Add(this.sandwich_0_heat_stopHeat);
      this.sandwich_0_heat_flow.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_heat_flow.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_heat_flow.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_heat_flow.Name = "sandwich_0_heat_flow";
      this.sandwich_0_heat_flow.Size = new System.Drawing.Size(526, 49);
      this.sandwich_0_heat_flow.TabIndex = 0;
      // 
      // sandwich_0_heat_setpoint
      // 
      this.sandwich_0_heat_setpoint.AutoSize = true;
      this.sandwich_0_heat_setpoint.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_heat_setpoint.Controls.Add(this.sandwich_0_heat_setpoint_label);
      this.sandwich_0_heat_setpoint.Controls.Add(this.sandwich_0_heat_setpoint_upDown);
      this.sandwich_0_heat_setpoint.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_heat_setpoint.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_heat_setpoint.Location = new System.Drawing.Point(0, 0);
      this.sandwich_0_heat_setpoint.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_heat_setpoint.Name = "sandwich_0_heat_setpoint";
      this.sandwich_0_heat_setpoint.Size = new System.Drawing.Size(72, 49);
      this.sandwich_0_heat_setpoint.TabIndex = 0;
      // 
      // sandwich_0_heat_setpoint_label
      // 
      this.sandwich_0_heat_setpoint_label.AutoSize = true;
      this.sandwich_0_heat_setpoint_label.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_heat_setpoint_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.sandwich_0_heat_setpoint_label.Name = "sandwich_0_heat_setpoint_label";
      this.sandwich_0_heat_setpoint_label.Size = new System.Drawing.Size(66, 13);
      this.sandwich_0_heat_setpoint_label.TabIndex = 0;
      this.sandwich_0_heat_setpoint_label.Text = "Setpoint (°C)";
      // 
      // sandwich_0_heat_setpoint_upDown
      // 
      this.sandwich_0_heat_setpoint_upDown.DecimalPlaces = 1;
      this.sandwich_0_heat_setpoint_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_setpoint_upDown.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_heat_setpoint_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.sandwich_0_heat_setpoint_upDown.Maximum = new decimal(new int[] {
            140,
            0,
            0,
            0});
      this.sandwich_0_heat_setpoint_upDown.Name = "sandwich_0_heat_setpoint_upDown";
      this.sandwich_0_heat_setpoint_upDown.Size = new System.Drawing.Size(60, 26);
      this.sandwich_0_heat_setpoint_upDown.TabIndex = 1;
      this.sandwich_0_heat_setpoint_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_heat_maxRate
      // 
      this.sandwich_0_heat_maxRate.AutoSize = true;
      this.sandwich_0_heat_maxRate.Location = new System.Drawing.Point(75, 3);
      this.sandwich_0_heat_maxRate.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
      this.sandwich_0_heat_maxRate.Name = "sandwich_0_heat_maxRate";
      this.sandwich_0_heat_maxRate.Size = new System.Drawing.Size(61, 43);
      this.sandwich_0_heat_maxRate.TabIndex = 1;
      this.sandwich_0_heat_maxRate.Text = "Max\r\nheating\r\nrate";
      this.sandwich_0_heat_maxRate.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_heat_rate
      // 
      this.sandwich_0_heat_rate.AutoSize = true;
      this.sandwich_0_heat_rate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_heat_rate.Controls.Add(this.label2);
      this.sandwich_0_heat_rate.Controls.Add(this.numericUpDown2);
      this.sandwich_0_heat_rate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_heat_rate.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_heat_rate.Location = new System.Drawing.Point(136, 0);
      this.sandwich_0_heat_rate.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_heat_rate.Name = "sandwich_0_heat_rate";
      this.sandwich_0_heat_rate.Size = new System.Drawing.Size(72, 49);
      this.sandwich_0_heat_rate.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(0, 3);
      this.label2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(71, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Rate (°C/min)";
      // 
      // numericUpDown2
      // 
      this.numericUpDown2.DecimalPlaces = 2;
      this.numericUpDown2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numericUpDown2.Location = new System.Drawing.Point(3, 19);
      this.numericUpDown2.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new System.Drawing.Size(69, 26);
      this.numericUpDown2.TabIndex = 1;
      this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_heat_timer
      // 
      this.sandwich_0_heat_timer.AutoSize = true;
      this.sandwich_0_heat_timer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_heat_timer.Controls.Add(this.sandwich_0_heat_timer_flow);
      this.sandwich_0_heat_timer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_heat_timer.Location = new System.Drawing.Point(211, 0);
      this.sandwich_0_heat_timer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_heat_timer.Name = "sandwich_0_heat_timer";
      this.sandwich_0_heat_timer.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_heat_timer.Size = new System.Drawing.Size(210, 49);
      this.sandwich_0_heat_timer.TabIndex = 3;
      this.sandwich_0_heat_timer.TabStop = false;
      this.sandwich_0_heat_timer.Text = "Timer (Leave all 0 for infinite)";
      // 
      // sandwich_0_heat_timer_flow
      // 
      this.sandwich_0_heat_timer_flow.AutoSize = true;
      this.sandwich_0_heat_timer_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_heat_timer_flow.Controls.Add(this.sandwich_0_heat_timer_h_upDown);
      this.sandwich_0_heat_timer_flow.Controls.Add(this.sandwich_0_heat_timer_h_label);
      this.sandwich_0_heat_timer_flow.Controls.Add(this.sandwich_0_heat_timer_m_upDown);
      this.sandwich_0_heat_timer_flow.Controls.Add(this.sandwich_0_heat_timer_m_label);
      this.sandwich_0_heat_timer_flow.Controls.Add(this.sandwich_0_heat_timer_s_upDown);
      this.sandwich_0_heat_timer_flow.Controls.Add(this.sandwich_0_heat_timer_s_label);
      this.sandwich_0_heat_timer_flow.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_heat_timer_flow.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_heat_timer_flow.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_heat_timer_flow.Name = "sandwich_0_heat_timer_flow";
      this.sandwich_0_heat_timer_flow.Size = new System.Drawing.Size(204, 36);
      this.sandwich_0_heat_timer_flow.TabIndex = 0;
      // 
      // sandwich_0_heat_timer_h_upDown
      // 
      this.sandwich_0_heat_timer_h_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_timer_h_upDown.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_heat_timer_h_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.sandwich_0_heat_timer_h_upDown.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
      this.sandwich_0_heat_timer_h_upDown.Name = "sandwich_0_heat_timer_h_upDown";
      this.sandwich_0_heat_timer_h_upDown.Size = new System.Drawing.Size(40, 26);
      this.sandwich_0_heat_timer_h_upDown.TabIndex = 0;
      this.sandwich_0_heat_timer_h_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_heat_timer_h_label
      // 
      this.sandwich_0_heat_timer_h_label.AutoSize = true;
      this.sandwich_0_heat_timer_h_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_timer_h_label.Location = new System.Drawing.Point(43, 3);
      this.sandwich_0_heat_timer_h_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.sandwich_0_heat_timer_h_label.Name = "sandwich_0_heat_timer_h_label";
      this.sandwich_0_heat_timer_h_label.Size = new System.Drawing.Size(25, 24);
      this.sandwich_0_heat_timer_h_label.TabIndex = 1;
      this.sandwich_0_heat_timer_h_label.Text = "H";
      // 
      // sandwich_0_heat_timer_m_upDown
      // 
      this.sandwich_0_heat_timer_m_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_timer_m_upDown.Location = new System.Drawing.Point(71, 3);
      this.sandwich_0_heat_timer_m_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.sandwich_0_heat_timer_m_upDown.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
      this.sandwich_0_heat_timer_m_upDown.Name = "sandwich_0_heat_timer_m_upDown";
      this.sandwich_0_heat_timer_m_upDown.Size = new System.Drawing.Size(40, 26);
      this.sandwich_0_heat_timer_m_upDown.TabIndex = 2;
      this.sandwich_0_heat_timer_m_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_heat_timer_m_label
      // 
      this.sandwich_0_heat_timer_m_label.AutoSize = true;
      this.sandwich_0_heat_timer_m_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_timer_m_label.Location = new System.Drawing.Point(111, 3);
      this.sandwich_0_heat_timer_m_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.sandwich_0_heat_timer_m_label.Name = "sandwich_0_heat_timer_m_label";
      this.sandwich_0_heat_timer_m_label.Size = new System.Drawing.Size(27, 24);
      this.sandwich_0_heat_timer_m_label.TabIndex = 3;
      this.sandwich_0_heat_timer_m_label.Text = "M";
      // 
      // sandwich_0_heat_timer_s_upDown
      // 
      this.sandwich_0_heat_timer_s_upDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_timer_s_upDown.Location = new System.Drawing.Point(141, 3);
      this.sandwich_0_heat_timer_s_upDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.sandwich_0_heat_timer_s_upDown.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
      this.sandwich_0_heat_timer_s_upDown.Name = "sandwich_0_heat_timer_s_upDown";
      this.sandwich_0_heat_timer_s_upDown.Size = new System.Drawing.Size(40, 26);
      this.sandwich_0_heat_timer_s_upDown.TabIndex = 4;
      this.sandwich_0_heat_timer_s_upDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // sandwich_0_heat_timer_s_label
      // 
      this.sandwich_0_heat_timer_s_label.AutoSize = true;
      this.sandwich_0_heat_timer_s_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_heat_timer_s_label.Location = new System.Drawing.Point(181, 3);
      this.sandwich_0_heat_timer_s_label.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.sandwich_0_heat_timer_s_label.Name = "sandwich_0_heat_timer_s_label";
      this.sandwich_0_heat_timer_s_label.Size = new System.Drawing.Size(23, 24);
      this.sandwich_0_heat_timer_s_label.TabIndex = 5;
      this.sandwich_0_heat_timer_s_label.Text = "S";
      // 
      // sandwich_0_record
      // 
      this.sandwich_0_record.AutoSize = true;
      this.sandwich_0_record.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_record.Controls.Add(this.sandwich_0_record_flow);
      this.sandwich_0_record.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_record.Location = new System.Drawing.Point(996, 0);
      this.sandwich_0_record.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_record.Name = "sandwich_0_record";
      this.sandwich_0_record.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_record.Size = new System.Drawing.Size(288, 62);
      this.sandwich_0_record.TabIndex = 3;
      this.sandwich_0_record.TabStop = false;
      this.sandwich_0_record.Text = "Record";
      // 
      // sandwich_0_record_flow
      // 
      this.sandwich_0_record_flow.AutoSize = true;
      this.sandwich_0_record_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_record_flow.Controls.Add(this.sandwich_0_record_filepath);
      this.sandwich_0_record_flow.Controls.Add(this.sandwich_0_record_browse);
      this.sandwich_0_record_flow.Controls.Add(this.sandwich_0_record_startRecord);
      this.sandwich_0_record_flow.Controls.Add(this.sandwich_0_record_stopRecord);
      this.sandwich_0_record_flow.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_record_flow.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_record_flow.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_record_flow.Name = "sandwich_0_record_flow";
      this.sandwich_0_record_flow.Size = new System.Drawing.Size(282, 49);
      this.sandwich_0_record_flow.TabIndex = 0;
      // 
      // sandwich_0_record_filepath
      // 
      this.sandwich_0_record_filepath.AutoSize = true;
      this.sandwich_0_record_filepath.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_record_filepath.Controls.Add(this.sandwich_0_record_filepath_label);
      this.sandwich_0_record_filepath.Controls.Add(this.sandwich_0_record_filepath_textbox);
      this.sandwich_0_record_filepath.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_record_filepath.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_record_filepath.Location = new System.Drawing.Point(0, 0);
      this.sandwich_0_record_filepath.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_record_filepath.Name = "sandwich_0_record_filepath";
      this.sandwich_0_record_filepath.Size = new System.Drawing.Size(148, 49);
      this.sandwich_0_record_filepath.TabIndex = 0;
      // 
      // sandwich_0_record_filepath_label
      // 
      this.sandwich_0_record_filepath_label.AutoSize = true;
      this.sandwich_0_record_filepath_label.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_record_filepath_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.sandwich_0_record_filepath_label.Name = "sandwich_0_record_filepath_label";
      this.sandwich_0_record_filepath_label.Size = new System.Drawing.Size(47, 13);
      this.sandwich_0_record_filepath_label.TabIndex = 0;
      this.sandwich_0_record_filepath_label.Text = "File path";
      // 
      // sandwich_0_record_filepath_textbox
      // 
      this.sandwich_0_record_filepath_textbox.BackColor = System.Drawing.SystemColors.Window;
      this.sandwich_0_record_filepath_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_record_filepath_textbox.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_record_filepath_textbox.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
      this.sandwich_0_record_filepath_textbox.Name = "sandwich_0_record_filepath_textbox";
      this.sandwich_0_record_filepath_textbox.Size = new System.Drawing.Size(144, 20);
      this.sandwich_0_record_filepath_textbox.TabIndex = 1;
      this.sandwich_0_record_filepath_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.sandwich_0_record_filepath_textbox.WordWrap = false;
      // 
      // sandwich_0_record_browse
      // 
      this.sandwich_0_record_browse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.sandwich_0_record_browse.Location = new System.Drawing.Point(148, 18);
      this.sandwich_0_record_browse.Margin = new System.Windows.Forms.Padding(0, 18, 3, 3);
      this.sandwich_0_record_browse.Name = "sandwich_0_record_browse";
      this.sandwich_0_record_browse.Size = new System.Drawing.Size(29, 22);
      this.sandwich_0_record_browse.TabIndex = 1;
      this.sandwich_0_record_browse.Text = "...";
      this.sandwich_0_record_browse.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_advanced
      // 
      this.sandwich_0_advanced.AutoSize = true;
      this.sandwich_0_advanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced.Controls.Add(this.sandwich_0_advanced_table);
      this.sandwich_0_advanced.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced.Location = new System.Drawing.Point(1290, 0);
      this.sandwich_0_advanced.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_advanced.MinimumSize = new System.Drawing.Size(100, 0);
      this.sandwich_0_advanced.Name = "sandwich_0_advanced";
      this.sandwich_0_advanced.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.sandwich_0_advanced.Size = new System.Drawing.Size(588, 62);
      this.sandwich_0_advanced.TabIndex = 4;
      this.sandwich_0_advanced.TabStop = false;
      this.sandwich_0_advanced.Text = "Advanced";
      // 
      // sandwich_0_advanced_table
      // 
      this.sandwich_0_advanced_table.AutoSize = true;
      this.sandwich_0_advanced_table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_table.ColumnCount = 3;
      this.sandwich_0_advanced_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0_advanced_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0_advanced_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0_advanced_table.Controls.Add(this.sandwich_0_advanced_hiddenFlow, 2, 0);
      this.sandwich_0_advanced_table.Controls.Add(this.sandwich_0_advanced_show, 1, 0);
      this.sandwich_0_advanced_table.Controls.Add(this.sandwich_0_advanced_hide, 0, 0);
      this.sandwich_0_advanced_table.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_table.Location = new System.Drawing.Point(3, 13);
      this.sandwich_0_advanced_table.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_table.Name = "sandwich_0_advanced_table";
      this.sandwich_0_advanced_table.RowCount = 1;
      this.sandwich_0_advanced_table.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.sandwich_0_advanced_table.Size = new System.Drawing.Size(582, 49);
      this.sandwich_0_advanced_table.TabIndex = 0;
      // 
      // sandwich_0_advanced_hiddenFlow
      // 
      this.sandwich_0_advanced_hiddenFlow.AutoSize = true;
      this.sandwich_0_advanced_hiddenFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_blinkLED);
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_removeSandwich);
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_port);
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_thermocouple);
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_oversampling);
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_PID_proportional);
      this.sandwich_0_advanced_hiddenFlow.Controls.Add(this.sandwich_0_advanced_PID_integral);
      this.sandwich_0_advanced_hiddenFlow.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_hiddenFlow.Location = new System.Drawing.Point(102, 0);
      this.sandwich_0_advanced_hiddenFlow.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_hiddenFlow.Name = "sandwich_0_advanced_hiddenFlow";
      this.sandwich_0_advanced_hiddenFlow.Size = new System.Drawing.Size(480, 49);
      this.sandwich_0_advanced_hiddenFlow.TabIndex = 3;
      // 
      // sandwich_0_advanced_removeSandwich
      // 
      this.sandwich_0_advanced_removeSandwich.Location = new System.Drawing.Point(54, 11);
      this.sandwich_0_advanced_removeSandwich.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_advanced_removeSandwich.Name = "sandwich_0_advanced_removeSandwich";
      this.sandwich_0_advanced_removeSandwich.Size = new System.Drawing.Size(60, 35);
      this.sandwich_0_advanced_removeSandwich.TabIndex = 1;
      this.sandwich_0_advanced_removeSandwich.Text = "Delete sandwich";
      this.sandwich_0_advanced_removeSandwich.UseVisualStyleBackColor = true;
      // 
      // sandwich_0_advanced_port
      // 
      this.sandwich_0_advanced_port.AutoSize = true;
      this.sandwich_0_advanced_port.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_port.Controls.Add(this.sandwich_0_advanced_port_label);
      this.sandwich_0_advanced_port.Controls.Add(this.sandwich_0_advanced_port_dropdown);
      this.sandwich_0_advanced_port.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_port.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_advanced_port.Location = new System.Drawing.Point(117, 0);
      this.sandwich_0_advanced_port.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_port.Name = "sandwich_0_advanced_port";
      this.sandwich_0_advanced_port.Size = new System.Drawing.Size(61, 49);
      this.sandwich_0_advanced_port.TabIndex = 2;
      this.sandwich_0_advanced_port.Visible = false;
      // 
      // sandwich_0_advanced_port_label
      // 
      this.sandwich_0_advanced_port_label.AutoSize = true;
      this.sandwich_0_advanced_port_label.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_advanced_port_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.sandwich_0_advanced_port_label.Name = "sandwich_0_advanced_port_label";
      this.sandwich_0_advanced_port_label.Size = new System.Drawing.Size(26, 13);
      this.sandwich_0_advanced_port_label.TabIndex = 1;
      this.sandwich_0_advanced_port_label.Text = "Port";
      // 
      // sandwich_0_advanced_port_dropdown
      // 
      this.sandwich_0_advanced_port_dropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sandwich_0_advanced_port_dropdown.FormattingEnabled = true;
      this.sandwich_0_advanced_port_dropdown.Items.AddRange(new object[] {
            "B",
            "E",
            "J",
            "K",
            "N",
            "R",
            "S",
            "T"});
      this.sandwich_0_advanced_port_dropdown.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_advanced_port_dropdown.Name = "sandwich_0_advanced_port_dropdown";
      this.sandwich_0_advanced_port_dropdown.Size = new System.Drawing.Size(55, 21);
      this.sandwich_0_advanced_port_dropdown.TabIndex = 2;
      // 
      // sandwich_0_advanced_thermocouple
      // 
      this.sandwich_0_advanced_thermocouple.AutoSize = true;
      this.sandwich_0_advanced_thermocouple.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_thermocouple.Controls.Add(this.sandwich_0_advanced_thermocouple_label);
      this.sandwich_0_advanced_thermocouple.Controls.Add(this.sandwich_0_advanced_thermocouple_dropdown);
      this.sandwich_0_advanced_thermocouple.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_thermocouple.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_advanced_thermocouple.Location = new System.Drawing.Point(178, 0);
      this.sandwich_0_advanced_thermocouple.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_thermocouple.Name = "sandwich_0_advanced_thermocouple";
      this.sandwich_0_advanced_thermocouple.Size = new System.Drawing.Size(81, 49);
      this.sandwich_0_advanced_thermocouple.TabIndex = 3;
      this.sandwich_0_advanced_thermocouple.Visible = false;
      // 
      // sandwich_0_advanced_thermocouple_label
      // 
      this.sandwich_0_advanced_thermocouple_label.AutoSize = true;
      this.sandwich_0_advanced_thermocouple_label.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0_advanced_thermocouple_label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.sandwich_0_advanced_thermocouple_label.Name = "sandwich_0_advanced_thermocouple_label";
      this.sandwich_0_advanced_thermocouple_label.Size = new System.Drawing.Size(75, 13);
      this.sandwich_0_advanced_thermocouple_label.TabIndex = 1;
      this.sandwich_0_advanced_thermocouple_label.Text = "Thermocouple\r\n";
      // 
      // sandwich_0_advanced_oversampling
      // 
      this.sandwich_0_advanced_oversampling.AutoSize = true;
      this.sandwich_0_advanced_oversampling.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_oversampling.Controls.Add(this.label3);
      this.sandwich_0_advanced_oversampling.Controls.Add(this.sandwich_0_advanced_oversampling_dropdown);
      this.sandwich_0_advanced_oversampling.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_oversampling.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_advanced_oversampling.Location = new System.Drawing.Point(259, 0);
      this.sandwich_0_advanced_oversampling.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_oversampling.Name = "sandwich_0_advanced_oversampling";
      this.sandwich_0_advanced_oversampling.Size = new System.Drawing.Size(61, 49);
      this.sandwich_0_advanced_oversampling.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 3);
      this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 13);
      this.label3.TabIndex = 14;
      this.label3.Text = "Sampling";
      // 
      // sandwich_0_advanced_oversampling_dropdown
      // 
      this.sandwich_0_advanced_oversampling_dropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sandwich_0_advanced_oversampling_dropdown.FormattingEnabled = true;
      this.sandwich_0_advanced_oversampling_dropdown.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8"});
      this.sandwich_0_advanced_oversampling_dropdown.Location = new System.Drawing.Point(3, 19);
      this.sandwich_0_advanced_oversampling_dropdown.MaxDropDownItems = 5;
      this.sandwich_0_advanced_oversampling_dropdown.Name = "sandwich_0_advanced_oversampling_dropdown";
      this.sandwich_0_advanced_oversampling_dropdown.Size = new System.Drawing.Size(55, 21);
      this.sandwich_0_advanced_oversampling_dropdown.TabIndex = 15;
      // 
      // sandwich_0_advanced_PID_proportional
      // 
      this.sandwich_0_advanced_PID_proportional.AutoSize = true;
      this.sandwich_0_advanced_PID_proportional.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_PID_proportional.Controls.Add(this.label4);
      this.sandwich_0_advanced_PID_proportional.Controls.Add(this.numericUpDown4);
      this.sandwich_0_advanced_PID_proportional.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_PID_proportional.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_advanced_PID_proportional.Location = new System.Drawing.Point(320, 0);
      this.sandwich_0_advanced_PID_proportional.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_PID_proportional.Name = "sandwich_0_advanced_PID_proportional";
      this.sandwich_0_advanced_PID_proportional.Size = new System.Drawing.Size(80, 49);
      this.sandwich_0_advanced_PID_proportional.TabIndex = 5;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 3);
      this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(63, 13);
      this.label4.TabIndex = 1;
      this.label4.Text = "Proportional";
      // 
      // numericUpDown4
      // 
      this.numericUpDown4.DecimalPlaces = 2;
      this.numericUpDown4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numericUpDown4.Location = new System.Drawing.Point(3, 19);
      this.numericUpDown4.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.numericUpDown4.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            131072});
      this.numericUpDown4.Name = "numericUpDown4";
      this.numericUpDown4.Size = new System.Drawing.Size(77, 26);
      this.numericUpDown4.TabIndex = 2;
      this.numericUpDown4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.numericUpDown4.Value = new decimal(new int[] {
            888888,
            0,
            0,
            131072});
      // 
      // sandwich_0_advanced_PID_integral
      // 
      this.sandwich_0_advanced_PID_integral.AutoSize = true;
      this.sandwich_0_advanced_PID_integral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0_advanced_PID_integral.Controls.Add(this.label5);
      this.sandwich_0_advanced_PID_integral.Controls.Add(this.numericUpDown5);
      this.sandwich_0_advanced_PID_integral.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0_advanced_PID_integral.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.sandwich_0_advanced_PID_integral.Location = new System.Drawing.Point(400, 0);
      this.sandwich_0_advanced_PID_integral.Margin = new System.Windows.Forms.Padding(0);
      this.sandwich_0_advanced_PID_integral.Name = "sandwich_0_advanced_PID_integral";
      this.sandwich_0_advanced_PID_integral.Size = new System.Drawing.Size(80, 49);
      this.sandwich_0_advanced_PID_integral.TabIndex = 6;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 3);
      this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(42, 13);
      this.label5.TabIndex = 1;
      this.label5.Text = "Integral";
      // 
      // numericUpDown5
      // 
      this.numericUpDown5.DecimalPlaces = 2;
      this.numericUpDown5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numericUpDown5.Location = new System.Drawing.Point(3, 19);
      this.numericUpDown5.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
      this.numericUpDown5.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            131072});
      this.numericUpDown5.Name = "numericUpDown5";
      this.numericUpDown5.Size = new System.Drawing.Size(77, 26);
      this.numericUpDown5.TabIndex = 2;
      this.numericUpDown5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.numericUpDown5.Value = new decimal(new int[] {
            888888,
            0,
            0,
            131072});
      // 
      // sandwich_0_advanced_show
      // 
      this.sandwich_0_advanced_show.Enabled = false;
      this.sandwich_0_advanced_show.Location = new System.Drawing.Point(54, 11);
      this.sandwich_0_advanced_show.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_advanced_show.Name = "sandwich_0_advanced_show";
      this.sandwich_0_advanced_show.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_advanced_show.TabIndex = 1;
      this.sandwich_0_advanced_show.Text = "Show";
      this.sandwich_0_advanced_show.UseVisualStyleBackColor = true;
      this.sandwich_0_advanced_show.Visible = false;
      // 
      // sandwich_0_advanced_hide
      // 
      this.sandwich_0_advanced_hide.Location = new System.Drawing.Point(3, 11);
      this.sandwich_0_advanced_hide.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
      this.sandwich_0_advanced_hide.Name = "sandwich_0_advanced_hide";
      this.sandwich_0_advanced_hide.Size = new System.Drawing.Size(45, 35);
      this.sandwich_0_advanced_hide.TabIndex = 2;
      this.sandwich_0_advanced_hide.Text = "Hide";
      this.sandwich_0_advanced_hide.UseVisualStyleBackColor = true;
      this.sandwich_0_advanced_hide.Visible = false;
      // 
      // mainFlow
      // 
      this.mainFlow.AutoSize = true;
      this.mainFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.mainFlow.Controls.Add(this.sandwich_0);
      this.mainFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.mainFlow.Location = new System.Drawing.Point(0, 27);
      this.mainFlow.Margin = new System.Windows.Forms.Padding(0);
      this.mainFlow.Name = "mainFlow";
      this.mainFlow.Size = new System.Drawing.Size(1887, 68);
      this.mainFlow.TabIndex = 1;
      // 
      // sandwich_0
      // 
      this.sandwich_0.AutoSize = true;
      this.sandwich_0.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.sandwich_0.ColumnCount = 5;
      this.sandwich_0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.sandwich_0.Controls.Add(this.sandwich_0_advanced, 4, 0);
      this.sandwich_0.Controls.Add(this.sandwich_0_heat, 2, 0);
      this.sandwich_0.Controls.Add(this.sandwich_0_ID, 0, 0);
      this.sandwich_0.Controls.Add(this.sandwich_0_DAQ, 1, 0);
      this.sandwich_0.Controls.Add(this.sandwich_0_record, 3, 0);
      this.sandwich_0.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sandwich_0.Location = new System.Drawing.Point(3, 3);
      this.sandwich_0.Name = "sandwich_0";
      this.sandwich_0.RowCount = 1;
      this.sandwich_0.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.sandwich_0.Size = new System.Drawing.Size(1881, 62);
      this.sandwich_0.TabIndex = 20;
      // 
      // menu
      // 
      this.menu.BackColor = System.Drawing.Color.White;
      this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_config,
            this.menu_port,
            this.menu_copyright});
      this.menu.Location = new System.Drawing.Point(0, 0);
      this.menu.Name = "menu";
      this.menu.Size = new System.Drawing.Size(1887, 27);
      this.menu.TabIndex = 19;
      this.menu.Text = "Menu";
      // 
      // menu_config
      // 
      this.menu_config.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_config_addSandwich,
            this.menu_config_openConfig,
            this.menu_config_saveConfig});
      this.menu_config.Name = "menu_config";
      this.menu_config.Size = new System.Drawing.Size(93, 23);
      this.menu_config.Text = "Configuration";
      // 
      // menu_config_addSandwich
      // 
      this.menu_config_addSandwich.Name = "menu_config_addSandwich";
      this.menu_config_addSandwich.Size = new System.Drawing.Size(212, 22);
      this.menu_config_addSandwich.Text = "Add sandwich";
      this.menu_config_addSandwich.Click += new System.EventHandler(this.menu_config_addSandwich_Click);
      // 
      // menu_config_openConfig
      // 
      this.menu_config_openConfig.Name = "menu_config_openConfig";
      this.menu_config_openConfig.Size = new System.Drawing.Size(212, 22);
      this.menu_config_openConfig.Text = "Open configuration file";
      this.menu_config_openConfig.Click += new System.EventHandler(this.menu_config_openConfig_Click);
      // 
      // menu_config_saveConfig
      // 
      this.menu_config_saveConfig.Name = "menu_config_saveConfig";
      this.menu_config_saveConfig.Size = new System.Drawing.Size(212, 22);
      this.menu_config_saveConfig.Text = "Save configuration file as..";
      this.menu_config_saveConfig.Click += new System.EventHandler(this.menu_config_saveConfig_Click);
      // 
      // menu_port
      // 
      this.menu_port.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_port_refresh,
            this.menu_port_autoFill});
      this.menu_port.Name = "menu_port";
      this.menu_port.Size = new System.Drawing.Size(46, 23);
      this.menu_port.Text = "Ports";
      // 
      // menu_port_refresh
      // 
      this.menu_port_refresh.Name = "menu_port_refresh";
      this.menu_port_refresh.Size = new System.Drawing.Size(266, 22);
      this.menu_port_refresh.Text = "Refresh port list";
      this.menu_port_refresh.Click += new System.EventHandler(this.menu_port_refresh_Click);
      // 
      // menu_port_autoFill
      // 
      this.menu_port_autoFill.Name = "menu_port_autoFill";
      this.menu_port_autoFill.Size = new System.Drawing.Size(266, 22);
      this.menu_port_autoFill.Text = "Auto-fill ports based on sandwich ID";
      this.menu_port_autoFill.Click += new System.EventHandler(this.menu_port_autoFill_Click);
      // 
      // menu_copyright
      // 
      this.menu_copyright.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.menu_copyright.BackColor = System.Drawing.Color.White;
      this.menu_copyright.Enabled = false;
      this.menu_copyright.ForeColor = System.Drawing.Color.White;
      this.menu_copyright.Name = "menu_copyright";
      this.menu_copyright.ReadOnly = true;
      this.menu_copyright.Size = new System.Drawing.Size(150, 23);
      this.menu_copyright.Text = "v 3.3 Soon Kiat Lau, 2020";
      this.menu_copyright.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // openConfigFileDialog
      // 
      this.openConfigFileDialog.AddExtension = false;
      this.openConfigFileDialog.Filter = "CSV file|*.csv";
      this.openConfigFileDialog.Title = "Open configuration file";
      // 
      // saveConfigFileDialog
      // 
      this.saveConfigFileDialog.DefaultExt = "csv";
      this.saveConfigFileDialog.Filter = "CSV file|*.csv";
      this.saveConfigFileDialog.Title = "Save configuration file";
      // 
      // recordFileDialog
      // 
      this.recordFileDialog.DefaultExt = "csv";
      this.recordFileDialog.Filter = "CSV file|*.csv";
      this.recordFileDialog.Title = "File to save recorded data";
      // 
      // TDTSandwich
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.ClientSize = new System.Drawing.Size(1134, 662);
      this.Controls.Add(this.mainFlow);
      this.Controls.Add(this.menu);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menu;
      this.Name = "TDTSandwich";
      this.Text = "TDT Sandwiches";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TDTSandwich_FormClosing);
      this.sandwich_0_ID.ResumeLayout(false);
      this.sandwich_0_ID.PerformLayout();
      this.sandwich_0_ID_flow.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_advanced_ID_upDown)).EndInit();
      this.sandwich_0_DAQ.ResumeLayout(false);
      this.sandwich_0_DAQ.PerformLayout();
      this.sandwich_0_DAQ_flow.ResumeLayout(false);
      this.sandwich_0_DAQ_flow.PerformLayout();
      this.sandwich_0_DAQ_heater.ResumeLayout(false);
      this.sandwich_0_DAQ_heater.PerformLayout();
      this.sandwich_0_DAQ_heater2.ResumeLayout(false);
      this.sandwich_0_DAQ_heater2.PerformLayout();
      this.sandwich_0_DAQ_sample.ResumeLayout(false);
      this.sandwich_0_DAQ_sample.PerformLayout();
      this.sandwich_0_heat.ResumeLayout(false);
      this.sandwich_0_heat.PerformLayout();
      this.sandwich_0_heat_flow.ResumeLayout(false);
      this.sandwich_0_heat_flow.PerformLayout();
      this.sandwich_0_heat_setpoint.ResumeLayout(false);
      this.sandwich_0_heat_setpoint.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_setpoint_upDown)).EndInit();
      this.sandwich_0_heat_rate.ResumeLayout(false);
      this.sandwich_0_heat_rate.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
      this.sandwich_0_heat_timer.ResumeLayout(false);
      this.sandwich_0_heat_timer.PerformLayout();
      this.sandwich_0_heat_timer_flow.ResumeLayout(false);
      this.sandwich_0_heat_timer_flow.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_timer_h_upDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_timer_m_upDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.sandwich_0_heat_timer_s_upDown)).EndInit();
      this.sandwich_0_record.ResumeLayout(false);
      this.sandwich_0_record.PerformLayout();
      this.sandwich_0_record_flow.ResumeLayout(false);
      this.sandwich_0_record_flow.PerformLayout();
      this.sandwich_0_record_filepath.ResumeLayout(false);
      this.sandwich_0_record_filepath.PerformLayout();
      this.sandwich_0_advanced.ResumeLayout(false);
      this.sandwich_0_advanced.PerformLayout();
      this.sandwich_0_advanced_table.ResumeLayout(false);
      this.sandwich_0_advanced_table.PerformLayout();
      this.sandwich_0_advanced_hiddenFlow.ResumeLayout(false);
      this.sandwich_0_advanced_hiddenFlow.PerformLayout();
      this.sandwich_0_advanced_port.ResumeLayout(false);
      this.sandwich_0_advanced_port.PerformLayout();
      this.sandwich_0_advanced_thermocouple.ResumeLayout(false);
      this.sandwich_0_advanced_thermocouple.PerformLayout();
      this.sandwich_0_advanced_oversampling.ResumeLayout(false);
      this.sandwich_0_advanced_oversampling.PerformLayout();
      this.sandwich_0_advanced_PID_proportional.ResumeLayout(false);
      this.sandwich_0_advanced_PID_proportional.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
      this.sandwich_0_advanced_PID_integral.ResumeLayout(false);
      this.sandwich_0_advanced_PID_integral.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
      this.mainFlow.ResumeLayout(false);
      this.mainFlow.PerformLayout();
      this.sandwich_0.ResumeLayout(false);
      this.sandwich_0.PerformLayout();
      this.menu.ResumeLayout(false);
      this.menu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sandwich_0_DAQ_startDAQ;
        private System.Windows.Forms.TextBox sandwich_0_DAQ_heater_textbox;
        private System.Windows.Forms.Label sandwich_0_DAQ_heater_label;
        private System.Windows.Forms.Button sandwich_0_DAQ_stopDAQ;
        private System.Windows.Forms.Button sandwich_0_record_startRecord;
        private System.Windows.Forms.Button sandwich_0_record_stopRecord;
        private System.Windows.Forms.Button sandwich_0_heat_stopHeat;
        private System.Windows.Forms.Button sandwich_0_heat_startHeat;
        private System.Windows.Forms.ComboBox sandwich_0_advanced_thermocouple_dropdown;
        private System.Windows.Forms.Button sandwich_0_advanced_blinkLED;
        private System.Windows.Forms.TextBox sandwich_0_DAQ_sample_textbox;
        private System.Windows.Forms.ComboBox sandwich_0_advanced_port_dropdown;
        private System.Windows.Forms.GroupBox sandwich_0_DAQ;
        private System.Windows.Forms.Label sandwich_0_DAQ_sample_label;
        private System.Windows.Forms.GroupBox sandwich_0_heat;
        private System.Windows.Forms.GroupBox sandwich_0_heat_timer;
        private System.Windows.Forms.Label sandwich_0_heat_timer_s_label;
        private System.Windows.Forms.Label sandwich_0_heat_timer_h_label;
        private System.Windows.Forms.Label sandwich_0_heat_timer_m_label;
        private System.Windows.Forms.Label sandwich_0_heat_setpoint_label;
        private System.Windows.Forms.GroupBox sandwich_0_record;
        private System.Windows.Forms.TextBox sandwich_0_record_filepath_textbox;
        private System.Windows.Forms.Button sandwich_0_record_browse;
        private System.Windows.Forms.GroupBox sandwich_0_advanced;
        private System.Windows.Forms.NumericUpDown sandwich_0_advanced_ID_upDown;
        private System.Windows.Forms.Label sandwich_0_advanced_port_label;
        private System.Windows.Forms.Label sandwich_0_advanced_thermocouple_label;
        private System.Windows.Forms.CheckBox sandwich_0_DAQ_readSample;
        private System.Windows.Forms.GroupBox sandwich_0_ID;
        private System.Windows.Forms.Button sandwich_0_advanced_show;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_ID_flow;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_DAQ_flow;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_DAQ_heater;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_DAQ_sample;
        private System.Windows.Forms.NumericUpDown sandwich_0_heat_timer_h_upDown;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_heat_flow;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_heat_setpoint;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_record_flow;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_record_filepath;
        private System.Windows.Forms.Label sandwich_0_record_filepath_label;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_advanced_thermocouple;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_advanced_port;
        private System.Windows.Forms.Button sandwich_0_advanced_hide;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_heat_timer_flow;
        private System.Windows.Forms.NumericUpDown sandwich_0_heat_timer_m_upDown;
        private System.Windows.Forms.NumericUpDown sandwich_0_heat_timer_s_upDown;
        private System.Windows.Forms.FlowLayoutPanel mainFlow;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_advanced_hiddenFlow;
        private System.Windows.Forms.NumericUpDown sandwich_0_heat_setpoint_upDown;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_heat_rate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_advanced_oversampling;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_advanced_PID_proportional;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_advanced_PID_integral;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.Button sandwich_0_advanced_removeSandwich;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menu_config;
        private System.Windows.Forms.ToolStripMenuItem menu_config_openConfig;
        private System.Windows.Forms.ToolStripMenuItem menu_config_addSandwich;
        private System.Windows.Forms.ToolStripMenuItem menu_port;
        private System.Windows.Forms.ToolStripMenuItem menu_port_refresh;
        private System.Windows.Forms.ToolStripMenuItem menu_port_autoFill;
        private System.Windows.Forms.ToolStripTextBox menu_copyright;
        private System.Windows.Forms.ToolStripMenuItem menu_config_saveConfig;
        private System.Windows.Forms.OpenFileDialog openConfigFileDialog;
        private System.Windows.Forms.SaveFileDialog saveConfigFileDialog;
        public System.Windows.Forms.SaveFileDialog recordFileDialog;
        private System.Windows.Forms.FlowLayoutPanel sandwich_0_DAQ_heater2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sandwich_0_DAQ_heater2_textbox;
        private System.Windows.Forms.ComboBox sandwich_0_advanced_oversampling_dropdown;
    private System.Windows.Forms.TableLayoutPanel sandwich_0;
    private System.Windows.Forms.TableLayoutPanel sandwich_0_advanced_table;
    private System.Windows.Forms.CheckBox sandwich_0_heat_maxRate;
  }
}

