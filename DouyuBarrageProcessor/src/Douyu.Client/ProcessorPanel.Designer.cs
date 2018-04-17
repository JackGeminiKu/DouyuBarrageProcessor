namespace Douyu.Client
{
    partial class ProcessorPanel
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
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.gbRoomOperation = new System.Windows.Forms.GroupBox();
            this.chkSimpleMode = new System.Windows.Forms.CheckBox();
            this.cboRoom = new System.Windows.Forms.ComboBox();
            this.btnStopProces = new System.Windows.Forms.Button();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.lblRoom = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageChatMessage = new System.Windows.Forms.TabPage();
            this.txtBarrage = new System.Windows.Forms.TextBox();
            this.tabPageGift = new System.Windows.Forms.TabPage();
            this.txtGift = new System.Windows.Forms.TextBox();
            this.tabPageDebug = new System.Windows.Forms.TabPage();
            this.bwDouyu = new System.ComponentModel.BackgroundWorker();
            this.tmrUpdateRank = new System.Windows.Forms.Timer(this.components);
            this.btnSaveRoom = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.gbRoomOperation.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageChatMessage.SuspendLayout();
            this.tabPageGift.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.gbRoomOperation, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.tabControl, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(621, 332);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // gbRoomOperation
            // 
            this.gbRoomOperation.Controls.Add(this.btnSaveRoom);
            this.gbRoomOperation.Controls.Add(this.chkSimpleMode);
            this.gbRoomOperation.Controls.Add(this.cboRoom);
            this.gbRoomOperation.Controls.Add(this.btnStopProces);
            this.gbRoomOperation.Controls.Add(this.btnStartProcess);
            this.gbRoomOperation.Controls.Add(this.lblRoom);
            this.gbRoomOperation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRoomOperation.Location = new System.Drawing.Point(3, 3);
            this.gbRoomOperation.Name = "gbRoomOperation";
            this.gbRoomOperation.Size = new System.Drawing.Size(615, 52);
            this.gbRoomOperation.TabIndex = 3;
            this.gbRoomOperation.TabStop = false;
            this.gbRoomOperation.Text = "选择房间";
            // 
            // chkSimpleMode
            // 
            this.chkSimpleMode.AutoSize = true;
            this.chkSimpleMode.Checked = true;
            this.chkSimpleMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSimpleMode.Location = new System.Drawing.Point(537, 27);
            this.chkSimpleMode.Name = "chkSimpleMode";
            this.chkSimpleMode.Size = new System.Drawing.Size(74, 17);
            this.chkSimpleMode.TabIndex = 5;
            this.chkSimpleMode.Text = "简单模式";
            this.chkSimpleMode.UseVisualStyleBackColor = true;
            // 
            // cboRoom
            // 
            this.cboRoom.FormattingEnabled = true;
            this.cboRoom.Items.AddRange(new object[] {
            "122402",
            "138286",
            "85894"});
            this.cboRoom.Location = new System.Drawing.Point(73, 23);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(123, 21);
            this.cboRoom.TabIndex = 4;
            this.cboRoom.Text = "122402";
            // 
            // btnStopProces
            // 
            this.btnStopProces.Enabled = false;
            this.btnStopProces.Location = new System.Drawing.Point(320, 18);
            this.btnStopProces.Name = "btnStopProces";
            this.btnStopProces.Size = new System.Drawing.Size(98, 26);
            this.btnStopProces.TabIndex = 3;
            this.btnStopProces.Text = "结束处理";
            this.btnStopProces.UseVisualStyleBackColor = true;
            this.btnStopProces.Click += new System.EventHandler(this.btnStopListen_Click);
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.Location = new System.Drawing.Point(213, 18);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(98, 26);
            this.btnStartProcess.TabIndex = 2;
            this.btnStartProcess.Text = "开始处理";
            this.btnStartProcess.UseVisualStyleBackColor = true;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartListen_Click);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(12, 31);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(55, 13);
            this.lblRoom.TabIndex = 0;
            this.lblRoom.Text = "房间号：";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageChatMessage);
            this.tabControl.Controls.Add(this.tabPageGift);
            this.tabControl.Controls.Add(this.tabPageDebug);
            this.tabControl.Font = new System.Drawing.Font("YaHei Consolas Hybrid", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(3, 61);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(615, 268);
            this.tabControl.TabIndex = 4;
            // 
            // tabPageChatMessage
            // 
            this.tabPageChatMessage.Controls.Add(this.txtBarrage);
            this.tabPageChatMessage.Location = new System.Drawing.Point(4, 25);
            this.tabPageChatMessage.Name = "tabPageChatMessage";
            this.tabPageChatMessage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageChatMessage.Size = new System.Drawing.Size(607, 239);
            this.tabPageChatMessage.TabIndex = 0;
            this.tabPageChatMessage.Text = "弹幕信息";
            this.tabPageChatMessage.UseVisualStyleBackColor = true;
            // 
            // txtBarrage
            // 
            this.txtBarrage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBarrage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarrage.Font = new System.Drawing.Font("YaHei Consolas Hybrid", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBarrage.Location = new System.Drawing.Point(3, 3);
            this.txtBarrage.Multiline = true;
            this.txtBarrage.Name = "txtBarrage";
            this.txtBarrage.ReadOnly = true;
            this.txtBarrage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBarrage.Size = new System.Drawing.Size(601, 233);
            this.txtBarrage.TabIndex = 1;
            this.txtBarrage.WordWrap = false;
            // 
            // tabPageGift
            // 
            this.tabPageGift.Controls.Add(this.txtGift);
            this.tabPageGift.Location = new System.Drawing.Point(4, 25);
            this.tabPageGift.Name = "tabPageGift";
            this.tabPageGift.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGift.Size = new System.Drawing.Size(607, 239);
            this.tabPageGift.TabIndex = 7;
            this.tabPageGift.Text = "礼物";
            this.tabPageGift.UseVisualStyleBackColor = true;
            // 
            // txtGift
            // 
            this.txtGift.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGift.Location = new System.Drawing.Point(3, 3);
            this.txtGift.Multiline = true;
            this.txtGift.Name = "txtGift";
            this.txtGift.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGift.Size = new System.Drawing.Size(601, 233);
            this.txtGift.TabIndex = 0;
            // 
            // tabPageDebug
            // 
            this.tabPageDebug.Location = new System.Drawing.Point(4, 25);
            this.tabPageDebug.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPageDebug.Name = "tabPageDebug";
            this.tabPageDebug.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPageDebug.Size = new System.Drawing.Size(607, 239);
            this.tabPageDebug.TabIndex = 6;
            this.tabPageDebug.Text = "调试";
            this.tabPageDebug.UseVisualStyleBackColor = true;
            // 
            // bwDouyu
            // 
            this.bwDouyu.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDouyu_DoWork);
            // 
            // tmrUpdateRank
            // 
            this.tmrUpdateRank.Interval = 1000;
            this.tmrUpdateRank.Tick += new System.EventHandler(this.tmrUpdateRank_Tick);
            // 
            // btnSaveRoom
            // 
            this.btnSaveRoom.Location = new System.Drawing.Point(427, 18);
            this.btnSaveRoom.Name = "btnSaveRoom";
            this.btnSaveRoom.Size = new System.Drawing.Size(98, 26);
            this.btnSaveRoom.TabIndex = 6;
            this.btnSaveRoom.Text = "保存房间";
            this.btnSaveRoom.UseVisualStyleBackColor = true;
            this.btnSaveRoom.Click += new System.EventHandler(this.btnSaveRoom_Click);
            // 
            // ProcessorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "ProcessorPanel";
            this.Size = new System.Drawing.Size(621, 332);
            this.tableLayoutPanel.ResumeLayout(false);
            this.gbRoomOperation.ResumeLayout(false);
            this.gbRoomOperation.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageChatMessage.ResumeLayout(false);
            this.tabPageChatMessage.PerformLayout();
            this.tabPageGift.ResumeLayout(false);
            this.tabPageGift.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox gbRoomOperation;
        private System.Windows.Forms.ComboBox cboRoom;
        private System.Windows.Forms.Button btnStopProces;
        private System.Windows.Forms.Button btnStartProcess;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageChatMessage;
        private System.Windows.Forms.TextBox txtBarrage;
        private System.Windows.Forms.TabPage tabPageDebug;
        private System.ComponentModel.BackgroundWorker bwDouyu;
        private System.Windows.Forms.TabPage tabPageGift;
        private System.Windows.Forms.TextBox txtGift;
        private System.Windows.Forms.Timer tmrUpdateRank;
        private System.Windows.Forms.CheckBox chkSimpleMode;
        private System.Windows.Forms.Button btnSaveRoom;
    }
}
