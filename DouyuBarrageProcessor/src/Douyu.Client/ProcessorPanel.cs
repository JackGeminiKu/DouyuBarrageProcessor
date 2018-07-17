using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Douyu.Events;
using Douyu.Messages;
using Jack4net.Log;

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
            base.OnHandleDestroyed(e);
        }

        public string RoomId { get; set; }

        public void StartProcess()
        {
            tmrUpdateRank.Start();
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
            tmrUpdateRank.Stop();
            _barrageProcessor.StopProcess();
        }

        private void tmrUpdateRank_Tick(object sender, EventArgs e)
        {
            try {
                tmrUpdateRank.Stop();
                Obs.TopMovies.Update();
                Obs.TopUsers.Update();
            } catch (Exception ex) {
                LogService.Error("更新排名异常: " + ex.Message, ex);
            } finally {
                tmrUpdateRank.Start();
            }
        }

        #region 弹幕消息

        void barrageProcessor_ChatMessageProcessed(object sender, ServerMessageEventArgs<ChatMessage> e)
        {
            ShowMessage("[{0:HH:mm}] [{1}]: {2}", e.Message.Time, e.Message.UserName, e.Message.Text);
        }

        void barrageProcessor_GiftMessageProcessed(object sender, ServerMessageEventArgs<GiftMessage> e)
        {
            ShowMessage("[{0:HH:mm}] [{1}]: {2}", e.Message.Time, e.Message.UserName, e.Message.GiftName);
        }

        void barrageProcessor_ChouqinMessageProcessed(object sender, ServerMessageEventArgs<ChouqinMessage> e)
        {
            ShowMessage("[{0:HH:mm}] [{1}]: 酬勤{2}", e.Message.Time, e.Message.UserName, e.Message.Level);
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
