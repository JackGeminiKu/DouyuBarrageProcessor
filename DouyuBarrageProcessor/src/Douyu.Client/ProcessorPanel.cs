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
using My.Log;

namespace Douyu.Client
{
    public partial class ProcessorPanel : UserControl
    {
        BarrageProcessor _barrageProcessor;
        System.Timers.Timer _tmrUpdateRank;

        public ProcessorPanel()
        {
            InitializeComponent();

            _barrageProcessor = new BarrageProcessor();
            _barrageProcessor.ChatMessageProcessed += barrageProcessor_ChatMessageProcessed;
            _barrageProcessor.GiftMessageProcessed += barrageProcessor_GiftMessageProcessed;
            _barrageProcessor.ChouqinMessageProcessed += barrageProcessor_ChouqinMessageProcessed;
            _tmrUpdateRank = new System.Timers.Timer(1000);
            _tmrUpdateRank.Elapsed += new System.Timers.ElapsedEventHandler(_tmrUpdateRank_Elapsed);
        }

        void _tmrUpdateRank_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try {
                _tmrUpdateRank.Stop();
                Obs.TopMovies.Update();
                if (AppSettings.ShowTopUsers)
                    Obs.TopUsers.Update();
                else
                    Obs.TopUsers.Clear();
            } catch (Exception ex) {
                LogService.Error("更新排名出现异常!", ex);
            } finally {
                _tmrUpdateRank.Start();
            };
        }

        public int RoomId { get; set; }

        public void StartProcess()
        {
            _tmrUpdateRank.Start();
            if (bwDouyu.IsBusy) {
                MessageBox.Show("正在处理弹幕中, 请不要重复启动!", "开始处理弹幕");
                return;
            }
            bwDouyu.RunWorkerAsync();
        }

        private void bwDouyu_DoWork(object sender, DoWorkEventArgs e)
        {
            _barrageProcessor.RoomId = RoomId;
            _barrageProcessor.StartProcess();
        }

        public void StopProcess()
        {
            _tmrUpdateRank.Stop();
            _barrageProcessor.StopProcess();
        }

        #region 弹幕消息

        void barrageProcessor_ChatMessageProcessed(object sender, ProcessMessageEventArgs<ChatMessage> e)
        {
            ShowMessage("[{0:MM/dd HH:mm:ss}] [{1}]: {2}", e.Message.Time, e.Message.UserName, e.Message.Text);
        }

        void barrageProcessor_GiftMessageProcessed(object sender, ProcessMessageEventArgs<GiftMessage> e)
        {
            ShowMessage("[{0:MM/dd HH:mm:ss}] [{1}]: {2}", e.Message.Time, e.Message.UserName, e.Message.GiftName);
        }

        void barrageProcessor_ChouqinMessageProcessed(object sender, ProcessMessageEventArgs<ChouqinMessage> e)
        {
            ShowMessage("[{0:MM/dd HH:mm:ss}] [{1}]: {2}", e.Message.Time, e.Message.UserName, e.Message.ChouqinName);
        }

        void ShowMessage(string format, params object[] args)
        {
            if (txtMessage.GetLineCount() > 1000) {
                txtMessage.ClearCrossThread();
            }
            txtMessage.AppendLineCrossThread(format, args);
        }

        #endregion
    }
}
