using Douyu.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Douyu.Events
{
    public class ServerMessageEventArgs<M> : EventArgs where M : ServerMessage
    {
        public ServerMessageEventArgs(M message)
        {
            Message = message;
        }

        public M Message { get; private set; }
    }
}
