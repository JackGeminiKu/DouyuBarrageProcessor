namespace Douyu.Client

{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.processorPanel = new Douyu.Client.ProcessorPanel();
            this.SuspendLayout();
            // 
            // processorPanel
            // 
            this.processorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processorPanel.Location = new System.Drawing.Point(0, 0);
            this.processorPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.processorPanel.Name = "processorPanel";
            this.processorPanel.RoomId = 742805;
            this.processorPanel.Size = new System.Drawing.Size(992, 282);
            this.processorPanel.TabIndex = 0;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 282);
            this.Controls.Add(this.processorPanel);
            this.Font = new System.Drawing.Font("YaHei Consolas Hybrid", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "斗鱼弹幕处理器";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private ProcessorPanel processorPanel;

    }
}

