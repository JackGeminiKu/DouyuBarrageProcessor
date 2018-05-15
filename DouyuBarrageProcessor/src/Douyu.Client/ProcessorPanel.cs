using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Douyu.Events;

namespace Douyu.Client
{
    public partial class ProcessorPanel : UserControl
    {
        BarrageProcessor _barrageProcessor;

        public ProcessorPanel()
        {
            InitializeComponent();

            _barrageProcessor = new BarrageProcessor();
            _barrageProcessor.ChatMessageProcessed += barrageProcessor_ChatMessageProcessed;
            _barrageProcessor.GiftMessageProcessed += barrageProcessor_GiftMessageProcessed;
            _barrageProcessor.ChouqinMessageProcessed += barrageProcessor_ChouqinMessageProcessed;
            _barrageProcessor.ScoreAdded += barrageProcessor_ScoreAdded;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _barrageProcessor.ChatMessageProcessed -= barrageProcessor_ChatMessageProcessed;
            _barrageProcessor.GiftMessageProcessed -= barrageProcessor_GiftMessageProcessed;
            _barrageProcessor.ChouqinMessageProcessed -= barrageProcessor_ChouqinMessageProcessed;
            _barrageProcessor.ScoreAdded -= barrageProcessor_ScoreAdded;
            _barrageProcessor.StopProcess();
            base.OnHandleDestroyed(e);
        }

        public void StartProcess(int roomId)
        {
            cboRoom.Text = roomId.ToString();
            StartProcess();
        }

        private void btnStartListen_Click(object sender, EventArgs e)
        {
            StartProcess();
        }

        void StartProcess()
        {
            tmrUpdateRank.Start();
            cboRoom.Enabled = false;
            btnStartProcess.Enabled = false;
            btnStopProces.Enabled = true;
            if (bwDouyu.IsBusy)
                MessageBox.Show("正在处理弹幕中...", "开始处理弹幕");
            bwDouyu.RunWorkerAsync();
        }

        private void bwDouyu_DoWork(object sender, DoWorkEventArgs e)
        {
            _barrageProcessor.ChangeRoomId(int.Parse(cboRoom.GetTextCrossThread()));
            _barrageProcessor.StartProcess();
        }

        private void btnStopListen_Click(object sender, EventArgs e)
        {
            StopProcess();
        }

        private void StopProcess()
        {
            tmrUpdateRank.Stop();
            cboRoom.Enabled = true;
            btnStartProcess.Enabled = true;
            btnStopProces.Enabled = false;
            _barrageProcessor.StopProcess();
        }

        private void tmrUpdateRank_Tick(object sender, EventArgs e)
        {
            tmrUpdateRank.Stop();
            Obs.TopMovies.Update();
            Obs.TopUsers.Update();
            tmrUpdateRank.Start();
        }

        #region 弹幕消息

        void barrageProcessor_ChatMessageProcessed(object sender, ChatMessageEventArgs e)
        {
            if (chkSimpleMode.Checked) {
                AppendText(txtBarrage, "[{0}]: {1}", e.ChatMessage.UserName, e.ChatMessage.Text);
            } else {
                AppendText(txtBarrage, "[{0:HH:mm:ss}] [{1}] [弹幕] [{2}]: {3}",
                    e.ChatMessage.Time, e.ChatMessage.RoomId, e.ChatMessage.UserName, e.ChatMessage.Text);
            }
        }

        void barrageProcessor_GiftMessageProcessed(object sender, GiftMessageEventArgs e)
        {
            if (chkSimpleMode.Checked) {
                AppendText(txtGift, "[{0}]: {1}", e.GiftMessage.UserName, e.GiftMessage.GiftName);
            } else {
                AppendText(txtGift, "[{0:HH:mm:ss}] [{1}] [礼物] [{2}]: {3}",
                    e.GiftMessage.Time, e.GiftMessage.RoomId, e.GiftMessage.UserName, e.GiftMessage.GiftName);
            }
        }

        void barrageProcessor_ChouqinMessageProcessed(object sender, ChouqinMessageEventArgs e)
        {
            if (chkSimpleMode.Checked) {
                AppendText(txtGift, "等级{0}酬勤", e.ChouqinMessage.Level);
            } else {
                AppendText(txtGift, "[{0:HH:mm:ss}] [{1}] [酬勤]: 等级{2}",
                    e.ChouqinMessage.Time, e.ChouqinMessage.RoomId, e.ChouqinMessage.Level);
            }
        }

        void barrageProcessor_ScoreAdded(object sender, ScoreAddedEventArgs e)
        {
            if (chkSimpleMode.Checked) {
                AppendText(txtGift, "[{0}]: + {1}", e.User, e.Score);
            } else {
                AppendText(txtGift, "[{0:HH:mm:ss}] [{1}] [积分] [{2}]: + {3}",
                    DateTime.Now, cboRoom.GetTextCrossThread(), e.User, e.Score);
            }
        }

        #endregion

        void AppendText(TextBox textBox, string format, params object[] args)
        {
            if (textBox.GetLineCount() > 1000) {
                textBox.ClearCrossThread();
            }
            textBox.AppendLineCrossThread(format, args);
        }

        private void btnSaveRoom_Click(object sender, EventArgs e)
        {
            DouyuLiveAssistant.Properties.Settings.Default.SavedRoom = int.Parse(cboRoom.Text);
            DouyuLiveAssistant.Properties.Settings.Default.Save();
            MessageBox.Show("房间" + cboRoom.Text + "已经保存完毕!", "保存房间", MessageBoxButtons.OK);
        }
    }
}
