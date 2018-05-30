using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Threading;

namespace Douyu.Client
{
    public class TimerFile
    {
        System.Timers.Timer _tmrDeleter;
        Mutex _mutex;

        public TimerFile(string path, int maxTime)
        {
            Path = path;
            _tmrDeleter = new System.Timers.Timer();
            _tmrDeleter.Interval = maxTime * 1000;
            _tmrDeleter.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        public TimerFile(string path)
            : this(path, 10)
        { }

        public string Path { get; private set; }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                _mutex = GetMutex();
                _mutex.WaitOne();

                if (!File.Exists(Path)) {
                    return;
                }

                FileInfo fileInfo = new FileInfo(Path);
                if (fileInfo.Length == 0) {
                    return;
                }

                // 删除所有内容
                File.WriteAllText(Path, "");                
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        public void WriteText(string contents)
        {
            try {
                _mutex = GetMutex();
                _mutex.WaitOne();
                _tmrDeleter.Stop();
                File.WriteAllText(Path, contents);
                _tmrDeleter.Start();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        Mutex GetMutex()
        {
            if (_mutex == null)
                _mutex = new Mutex(false, System.IO.Path.GetFileName(Path));
            return _mutex;
        }
    }
}
