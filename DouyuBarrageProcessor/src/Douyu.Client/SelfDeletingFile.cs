using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Threading;
using My.Log;
using System.Diagnostics;

namespace Douyu.Client
{
    public class SelfDeletingFile
    {
        System.Timers.Timer _trmFileUpdater;
        Mutex _mutex;

        public SelfDeletingFile(string filePath, int maxLines, int maxLiveTime)
        {
            FilePath = filePath;
            File.WriteAllText(FilePath, "");    //清空内容
            MaxLines = maxLines;
            MaxLiveTime = maxLiveTime;
            _mutex = new Mutex(false, FilePath.Replace('\\', '-'));
            _trmFileUpdater = new System.Timers.Timer(100);
            _trmFileUpdater.Elapsed += _tmrFileUpdater_Elapsed;
            _trmFileUpdater.Start();
        }

        public string FilePath { get; private set; }
        public int MaxLines { get; set; }
        public int MaxLiveTime { get; set; }

        bool _messageChanged = false;

        void _tmrFileUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                _mutex.WaitOne();

                while (_messages.Count > MaxLines) {
                    _messages.RemoveAt(0);
                    _messageChanged = true;
                }
                for (var i = _messages.Count - 1; i >= 0; --i) {
                    if ((DateTime.Now - _messages[i].Time).TotalSeconds > MaxLiveTime) {
                        _messages.RemoveAt(i);
                        _messageChanged = true;
                    }
                }

                if (!_messageChanged) return;

                var messages = "";
                foreach (var item in _messages) {
                    messages += item.Message + "\n";
                }

                File.WriteAllText(FilePath, messages);
                _messageChanged = false;
            } catch (Exception ex) {
                LogService.Error("Auto Delete Exception!", ex);
            } finally {
                _mutex.ReleaseMutex();
            }
        }


        List<MessageItem> _messages = new List<MessageItem>();

        public void WriteLine(string messsage)
        {
            _mutex.WaitOne();
            _messages.Add(new MessageItem(DateTime.Now, messsage.Trim()));
            _messageChanged = true;
            _mutex.ReleaseMutex();
        }
    }



    public class MessageItem
    {
        public MessageItem(DateTime time, string message)
        {
            Time = time;
            Message = message;
        }

        public DateTime Time { get; private set; }
        public string Message { get; private set; }
    }
}
