using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Threading;
using Jack4net.Log;

namespace Douyu.Client
{
    public class TimerFile
    {
        System.Timers.Timer _tmrDeleter;
        Mutex _mutex;

        public TimerFile(string filePath, int maxTime)
        {
            FilePath = filePath;
            _mutex = new Mutex(false, filePath.Replace('\\','-'));
            _tmrDeleter = new System.Timers.Timer(maxTime * 1000);
            _tmrDeleter.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        public string FilePath { get; private set; }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                _mutex.WaitOne();

                if (!File.Exists(FilePath))
                    return;

                var fileInfo = new FileInfo(FilePath);
                if (fileInfo.Length == 0)
                    return;

                // 清空
                File.WriteAllText(FilePath, "");
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        public void WriteLine(string contents)
        {
            try {
                _mutex.WaitOne();
                _tmrDeleter.Stop();
                File.WriteAllText(FilePath, contents);
                _tmrDeleter.Start();
            } catch (Exception ex) {
                LogService.Error("WriteLine Exception!", ex);
            } finally {
                _mutex.ReleaseMutex();
            }
        }
    }
}
