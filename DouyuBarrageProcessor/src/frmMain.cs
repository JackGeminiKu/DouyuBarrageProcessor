using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using Douyu.Messages;
using Jack4net;
using Douyu.Events;
using Jack4net.Log;
using System.Configuration;
using System.Diagnostics;
using DouyuLiveAssistant.Properties;

namespace Douyu.Client
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            SetFormLocation();
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogService.Error(e.Exception.Message, e.Exception);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogService.Error(ex.Message, ex);
        }

        void SetFormLocation()
        {
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Size.Width,
                Screen.PrimaryScreen.WorkingArea.Height - Size.Height);
        }

        private const int ROOM_ID = 122402;
        //private const int ROOM_ID = "71017"; // 冯提莫

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            Text += string.Format(" (房间号: {0})", ROOM_ID);
            processorPanel.RoomId = ROOM_ID;
            processorPanel.StartProcess();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
