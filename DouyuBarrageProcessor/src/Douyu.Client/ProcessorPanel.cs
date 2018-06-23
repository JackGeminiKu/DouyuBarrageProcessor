﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Douyu.Events;
using Douyu.Messages;

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
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _barrageProcessor.ChatMessageProcessed -= barrageProcessor_ChatMessageProcessed;
            _barrageProcessor.GiftMessageProcessed -= barrageProcessor_GiftMessageProcessed;
            _barrageProcessor.ChouqinMessageProcessed -= barrageProcessor_ChouqinMessageProcessed;
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
            if (bwDouyu.IsBusy) {
                MessageBox.Show("正在处理弹幕中, 请不要重复启动!", "开始处理弹幕");
                return;
            }
            bwDouyu.RunWorkerAsync();
        }

        private void bwDouyu_DoWork(object sender, DoWorkEventArgs e)
        {
            _barrageProcessor.StartProcess(int.Parse(cboRoom.GetTextCrossThread()));
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

        private void btnSaveRoom_Click(object sender, EventArgs e)
        {
            DouyuLiveAssistant.Properties.Settings.Default.SavedRoom = int.Parse(cboRoom.Text);
            DouyuLiveAssistant.Properties.Settings.Default.Save();
            MessageBox.Show("房间" + cboRoom.Text + "已经保存完毕!", "保存房间", MessageBoxButtons.OK);
        }

        private void tmrUpdateRank_Tick(object sender, EventArgs e)
        {
            tmrUpdateRank.Stop();
            Obs.TopMovies.Update();
            Obs.TopUsers.Update();
            tmrUpdateRank.Start();
        }

        #region 弹幕消息

        void barrageProcessor_ChatMessageProcessed(object sender, ServerMessageEventArgs<ChatMessage> e)
        {
            if (chkSimpleMode.Checked) {
                ShowMessage("[{0}]: {1}", e.Message.UserName, e.Message.Text);
            } else {
                ShowMessage("[{0:HH:mm:ss}] [{1}] [弹幕] [{2}]: {3}",
                    e.Message.Time, e.Message.RoomId, e.Message.UserName, e.Message.Text);
            }
        }

        void barrageProcessor_GiftMessageProcessed(object sender, ServerMessageEventArgs<GiftMessage> e)
        {
            if (!chkShowGift.Checked)
                return;

            if (chkSimpleMode.Checked) {
                ShowMessage("[{0}]: {1}", e.Message.UserName, e.Message.GiftName);
            } else {
                ShowMessage("[{0:HH:mm:ss}] [{1}] [礼物] [{2}]: {3}",
                    e.Message.Time, e.Message.RoomId, e.Message.UserName, e.Message.GiftName);
            }
        }

        void barrageProcessor_ChouqinMessageProcessed(object sender, ServerMessageEventArgs<ChouqinMessage> e)
        {
            if (!chkShowGift.Checked)
                return;

            if (chkSimpleMode.Checked) {
                ShowMessage("等级{0}酬勤", e.Message.Level);
            } else {
                ShowMessage("[{0:HH:mm:ss}] [{1}] [酬勤]: 等级{2}",
                    e.Message.Time, e.Message.RoomId, e.Message.Level);
            }
        }

        #endregion

        void ShowMessage(string format, params object[] args)
        {
            if (txtMessage.GetLineCount() > 1000) {
                txtMessage.ClearCrossThread();
            }
            txtMessage.AppendLineCrossThread(format, args);
        }
    }
}
