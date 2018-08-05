﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Threading;
using Jack4net.Log;
using System.Diagnostics;

namespace Douyu.Client
{
    public class SelfDeletingFile
    {
        System.Timers.Timer _tmrDeleter;
        Mutex _mutex;

        public SelfDeletingFile(string filePath, int maxLines, int maxTime)
        {
            FilePath = filePath;
            MaxLines = maxLines;
            _mutex = new Mutex(false, FilePath.Replace('\\', '-'));
            _tmrDeleter = new System.Timers.Timer(maxTime * 1000);
            _tmrDeleter.Elapsed += _timer_Elapsed;
            _tmrDeleter.Start();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                _mutex.WaitOne();

                if (!File.Exists(FilePath))
                    return;

                var fileInfo = new FileInfo(FilePath);
                if (fileInfo.Length == 0)
                    return;

                // 删除第一行
                var lines = File.ReadAllLines(FilePath);
                var contents = "";
                for (var i = 1; i < lines.Length; ++i) {
                    contents += lines[i] + "\n";
                }
                File.WriteAllText(FilePath, contents);
            } catch (Exception ex) {
                LogService.Error("Auto Delete Exception!", ex);
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        public string FilePath { get; private set; }

        public int MaxLines { get; set; }

        public void WriteLine(string messsage)
        {
            try {
                _mutex.WaitOne();
                WriteLine(FilePath, messsage.Trim());
            } catch (Exception ex) {
                LogService.Error("WriteLine Exception!", ex);
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        void WriteLine(string filePath, string message)
        {
            // 还没有message文件?
            if (!File.Exists(filePath)) {
                File.AppendAllText(filePath, message + "\n");
                return;
            }

            // 读取原有内容
            var lines = File.ReadAllLines(filePath);

            // 还没达到最大行数限制
            if (lines.Length < MaxLines) {
                File.AppendAllText(filePath, message + "\n");
                return;
            }

            // 已经达到最大行数限制
            var contents = "";
            for (var i = 1; i < MaxLines; ++i) {
                contents += lines[i] + "\n";
            }
            contents += message + "\n";

            File.WriteAllText(filePath, contents);
        }
    }
}
