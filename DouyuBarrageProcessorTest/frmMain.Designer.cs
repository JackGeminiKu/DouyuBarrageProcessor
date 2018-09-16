namespace DouyuBarrageProcessorTest
{
    partial class frmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnScoreTest = new System.Windows.Forms.Button();
            this.txtRoom = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTestChatData = new System.Windows.Forms.Button();
            this.txtChatData = new System.Windows.Forms.TextBox();
            this.btnTestGiftData = new System.Windows.Forms.Button();
            this.txtGiftData = new System.Windows.Forms.TextBox();
            this.btnTestMessageFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnScoreTest
            // 
            this.btnScoreTest.Location = new System.Drawing.Point(67, 158);
            this.btnScoreTest.Name = "btnScoreTest";
            this.btnScoreTest.Size = new System.Drawing.Size(194, 42);
            this.btnScoreTest.TabIndex = 0;
            this.btnScoreTest.Text = "积分测试";
            this.btnScoreTest.UseVisualStyleBackColor = true;
            this.btnScoreTest.Click += new System.EventHandler(this.btnScoreTest_Click);
            // 
            // txtRoom
            // 
            this.txtRoom.Location = new System.Drawing.Point(67, 118);
            this.txtRoom.Name = "txtRoom";
            this.txtRoom.Size = new System.Drawing.Size(100, 20);
            this.txtRoom.TabIndex = 1;
            this.txtRoom.Text = "20415";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "房间号";
            // 
            // btnTestChatData
            // 
            this.btnTestChatData.Location = new System.Drawing.Point(69, 238);
            this.btnTestChatData.Name = "btnTestChatData";
            this.btnTestChatData.Size = new System.Drawing.Size(194, 42);
            this.btnTestChatData.TabIndex = 3;
            this.btnTestChatData.Text = "弹幕数据";
            this.btnTestChatData.UseVisualStyleBackColor = true;
            this.btnTestChatData.Click += new System.EventHandler(this.btnTestChatData_Click);
            // 
            // txtChatData
            // 
            this.txtChatData.Location = new System.Drawing.Point(73, 308);
            this.txtChatData.Multiline = true;
            this.txtChatData.Name = "txtChatData";
            this.txtChatData.Size = new System.Drawing.Size(188, 105);
            this.txtChatData.TabIndex = 4;
            // 
            // btnTestGiftData
            // 
            this.btnTestGiftData.Location = new System.Drawing.Point(290, 238);
            this.btnTestGiftData.Name = "btnTestGiftData";
            this.btnTestGiftData.Size = new System.Drawing.Size(194, 42);
            this.btnTestGiftData.TabIndex = 5;
            this.btnTestGiftData.Text = "礼物数据";
            this.btnTestGiftData.UseVisualStyleBackColor = true;
            this.btnTestGiftData.Click += new System.EventHandler(this.btnTestGiftData_Click);
            // 
            // txtGiftData
            // 
            this.txtGiftData.Location = new System.Drawing.Point(290, 308);
            this.txtGiftData.Multiline = true;
            this.txtGiftData.Name = "txtGiftData";
            this.txtGiftData.Size = new System.Drawing.Size(188, 105);
            this.txtGiftData.TabIndex = 6;
            // 
            // btnTestMessageFile
            // 
            this.btnTestMessageFile.Location = new System.Drawing.Point(530, 238);
            this.btnTestMessageFile.Name = "btnTestMessageFile";
            this.btnTestMessageFile.Size = new System.Drawing.Size(194, 42);
            this.btnTestMessageFile.TabIndex = 7;
            this.btnTestMessageFile.Text = "测试Message File";
            this.btnTestMessageFile.UseVisualStyleBackColor = true;
            this.btnTestMessageFile.Click += new System.EventHandler(this.btnTestTimerFile_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 580);
            this.Controls.Add(this.btnTestMessageFile);
            this.Controls.Add(this.txtGiftData);
            this.Controls.Add(this.btnTestGiftData);
            this.Controls.Add(this.txtChatData);
            this.Controls.Add(this.btnTestChatData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRoom);
            this.Controls.Add(this.btnScoreTest);
            this.Name = "frmMain";
            this.Text = "斗鱼弹幕处理器测试";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnScoreTest;
        private System.Windows.Forms.TextBox txtRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTestChatData;
        private System.Windows.Forms.TextBox txtChatData;
        private System.Windows.Forms.Button btnTestGiftData;
        private System.Windows.Forms.TextBox txtGiftData;
        private System.Windows.Forms.Button btnTestMessageFile;
    }
}

