using Douyu.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Douyu.Events
{
    public class ServerMessageEventArgs<T> : EventArgs where T : ServerMessage
    {
        public ServerMessageEventArgs(T message)
        {
            Message = message;
        }

        public T Message { get; private set; }
    }
}
