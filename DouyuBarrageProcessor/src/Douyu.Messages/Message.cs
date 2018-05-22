using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jack4net.Log;

namespace Douyu.Messages
{
    public abstract class Message
    {
        protected abstract int MessageType { get; }

        public DateTime Time { get; set; }

        public string MessageText { get; protected set; }

        public byte[] MessgeBytes { get { return CreateMessageBytes(MessageText); } }

        byte[] CreateMessageBytes(string messageText)
        {
            if (messageText.EndsWith("\0") == false) messageText += "\0";
            
            var result = new List<byte>();
            try {
                var lenBytes = (messageText.Length + 8).ToLittleEndian();
                result.AddRange(lenBytes);
                result.AddRange(lenBytes);
                result.AddRange((MessageType).ToLittleEndian().Take(2));
                result.Add(0);
                result.Add(0);
                result.AddRange(Encoding.UTF8.GetBytes(messageText));
            } catch (Exception e) {
                LogService.Error(e.Message, e);
                throw;
            }

            return result.ToArray<byte>();
        }

        protected void AddItem(string key, string value)
        {
            MessageText += string.Format("{0}@={1}/", ConvertKeyWord(key), ConvertKeyWord(value));
        }

        private string ConvertKeyWord(string value)
        {
            return value.Replace("/", "@S").Replace("@", "@A");
        }

        public override string ToString()
        {
            return MessageText;
        }
    }
}
