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

        public TimerFile(string path, int maxTime)
        {
            Path = path;
            _mutex = new Mutex(false, System.IO.Path.GetFileName(Path));
            _tmrDeleter = new System.Timers.Timer();
            _tmrDeleter.Interval = maxTime * 1000;
            _tmrDeleter.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        public string Path { get; private set; }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                _mutex.WaitOne();

                if (!File.Exists(Path))
                    return;

                FileInfo fileInfo = new FileInfo(Path);
                if (fileInfo.Length == 0)
                    return;

                // 删除所有内容
                File.WriteAllText(Path, "");
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        public void WriteLine(string contents)
        {
            try {
                _mutex.WaitOne();
                _tmrDeleter.Stop();
                File.WriteAllText(Path, contents);
                _tmrDeleter.Start();
            } catch (Exception ex) {
                LogService.Error("WriteLine() Exception: " + ex.Message, ex);
            } finally {
                _mutex.ReleaseMutex();
            }
        }
    }
}
