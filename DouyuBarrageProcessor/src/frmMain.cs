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
using My;
using Douyu.Events;
using My.Log;
using System.Configuration;
using System.Diagnostics;
using DouyuLiveAssistant.Properties;
using My.Windows.Forms;

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
                Screen.PrimaryScreen.WorkingArea.Height/2 - Size.Height/2);
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            Text += string.Format(" (房间号: {0})", AppSettings.RoomId);
            Text += " v" + MyApplication.Version;
            processorPanel.RoomId = AppSettings.RoomId;
            processorPanel.StartProcess();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
