using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Threading;

namespace Douyu.Client
{
    public class Snapfile
    {
        System.Timers.Timer _tmrDeleter;
        Mutex _mutex;

        public Snapfile(string path, int maxLines, int maxTime)
        {
            Path = path;
            MaxLines = maxLines;
            _tmrDeleter = new System.Timers.Timer();
            _tmrDeleter.Interval = maxTime * 1000;
            _tmrDeleter.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        public Snapfile(string path)
            : this(path, 5, 10)
        { }

        public string Path { get; private set; }

        public int MaxLines { get; set; }

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

                // 删除第一行
                string[] lines = File.ReadAllLines(Path);
                string contents = "";
                for (int i = 1; i < lines.Length; ++i) {
                    contents += lines[i] + "\n";
                }

                File.WriteAllText(Path, contents);
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        public void AppendText(string contents)
        {
            try {
                _mutex = GetMutex();
                _mutex.WaitOne();
                _tmrDeleter.Stop();
                AppendMessage(contents);
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

        void AppendMessage(string message)
        {
            // 还没有message文件?
            if (!File.Exists(Path)) {
                File.AppendAllText(Path, message + "\r\n");
                return;
            }

            // 读取原有内容
            string[] lines = File.ReadAllLines(Path);

            // 还没达到最大行数限制
            if (lines.Length < MaxLines) {
                File.AppendAllText(Path, message + "\r\n");
                return;
            }

            // 已经达到最大行数限制
            string contents = "";
            for (int i = lines.Length - MaxLines + 1; i < lines.Length; ++i) {
                contents += lines[i] + "\r\n";
            }
            File.WriteAllText(Path, contents);
        }      
    }
}
