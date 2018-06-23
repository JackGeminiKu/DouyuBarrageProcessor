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
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.gbRoomOperation = new System.Windows.Forms.GroupBox();
            this.btnSaveRoom = new System.Windows.Forms.Button();
            this.chkSimpleMode = new System.Windows.Forms.CheckBox();
            this.cboRoom = new System.Windows.Forms.ComboBox();
            this.btnStopProces = new System.Windows.Forms.Button();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.lblRoom = new System.Windows.Forms.Label();
            this.bwDouyu = new System.ComponentModel.BackgroundWorker();
            this.tmrUpdateRank = new System.Windows.Forms.Timer(this.components);
            this.chkShowGift = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel.SuspendLayout();
            this.gbRoomOperation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.txtMessage, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.gbRoomOperation, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(646, 279);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // txtMessage
            // 
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Font = new System.Drawing.Font("YaHei Consolas Hybrid", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(3, 53);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessage.Size = new System.Drawing.Size(640, 223);
            this.txtMessage.TabIndex = 4;
            this.txtMessage.WordWrap = false;
            // 
            // gbRoomOperation
            // 
            this.gbRoomOperation.Controls.Add(this.chkShowGift);
            this.gbRoomOperation.Controls.Add(this.btnSaveRoom);
            this.gbRoomOperation.Controls.Add(this.chkSimpleMode);
            this.gbRoomOperation.Controls.Add(this.cboRoom);
            this.gbRoomOperation.Controls.Add(this.btnStopProces);
            this.gbRoomOperation.Controls.Add(this.btnStartProcess);
            this.gbRoomOperation.Controls.Add(this.lblRoom);
            this.gbRoomOperation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRoomOperation.Location = new System.Drawing.Point(3, 3);
            this.gbRoomOperation.Name = "gbRoomOperation";
            this.gbRoomOperation.Size = new System.Drawing.Size(640, 44);
            this.gbRoomOperation.TabIndex = 3;
            this.gbRoomOperation.TabStop = false;
            this.gbRoomOperation.Text = "选择房间";
            // 
            // btnSaveRoom
            // 
            this.btnSaveRoom.Location = new System.Drawing.Point(285, 17);
            this.btnSaveRoom.Name = "btnSaveRoom";
            this.btnSaveRoom.Size = new System.Drawing.Size(73, 21);
            this.btnSaveRoom.TabIndex = 6;
            this.btnSaveRoom.Text = "保存房间";
            this.btnSaveRoom.UseVisualStyleBackColor = true;
            this.btnSaveRoom.Click += new System.EventHandler(this.btnSaveRoom_Click);
            // 
            // chkSimpleMode
            // 
            this.chkSimpleMode.AutoSize = true;
            this.chkSimpleMode.Checked = true;
            this.chkSimpleMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSimpleMode.Location = new System.Drawing.Point(362, 22);
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
            this.cboRoom.Location = new System.Drawing.Point(60, 17);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(66, 21);
            this.cboRoom.TabIndex = 4;
            this.cboRoom.Text = "122402";
            // 
            // btnStopProces
            // 
            this.btnStopProces.Enabled = false;
            this.btnStopProces.Location = new System.Drawing.Point(207, 17);
            this.btnStopProces.Name = "btnStopProces";
            this.btnStopProces.Size = new System.Drawing.Size(73, 21);
            this.btnStopProces.TabIndex = 3;
            this.btnStopProces.Text = "停止处理";
            this.btnStopProces.UseVisualStyleBackColor = true;
            this.btnStopProces.Click += new System.EventHandler(this.btnStopListen_Click);
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.Location = new System.Drawing.Point(130, 17);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(73, 21);
            this.btnStartProcess.TabIndex = 2;
            this.btnStartProcess.Text = "开始处理";
            this.btnStartProcess.UseVisualStyleBackColor = true;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartListen_Click);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(9, 22);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(55, 13);
            this.lblRoom.TabIndex = 0;
            this.lblRoom.Text = "房间号：";
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
            // chkShowGift
            // 
            this.chkShowGift.AutoSize = true;
            this.chkShowGift.Location = new System.Drawing.Point(433, 22);
            this.chkShowGift.Name = "chkShowGift";
            this.chkShowGift.Size = new System.Drawing.Size(74, 17);
            this.chkShowGift.TabIndex = 10;
            this.chkShowGift.Text = "显示礼物";
            this.chkShowGift.UseVisualStyleBackColor = true;
            // 
            // ProcessorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "ProcessorPanel";
            this.Size = new System.Drawing.Size(646, 279);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.gbRoomOperation.ResumeLayout(false);
            this.gbRoomOperation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox gbRoomOperation;
        private System.Windows.Forms.ComboBox cboRoom;
        private System.Windows.Forms.Button btnStopProces;
        private System.Windows.Forms.Button btnStartProcess;
        private System.Windows.Forms.Label lblRoom;
        private System.ComponentModel.BackgroundWorker bwDouyu;
        private System.Windows.Forms.Timer tmrUpdateRank;
        private System.Windows.Forms.CheckBox chkSimpleMode;
        private System.Windows.Forms.Button btnSaveRoom;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.CheckBox chkShowGift;
    }
}
