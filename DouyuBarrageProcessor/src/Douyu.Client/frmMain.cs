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
            ShowAppVersion();
        }

        void SetFormLocation()
        {
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Size.Width,
                Screen.PrimaryScreen.WorkingArea.Height - Size.Height);
        }

        void ShowAppVersion()
        {
            this.Text += " v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogService.GetLogger("Error").Error(e.Exception.Message, e.Exception);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogService.GetLogger("Error").Error(ex.Message, ex);
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            processorPanel.StartProcess(DouyuLiveAssistant.Properties.Settings.Default.SavedRoom);
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
