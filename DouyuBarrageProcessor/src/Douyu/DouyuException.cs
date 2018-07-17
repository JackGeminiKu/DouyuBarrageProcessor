using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Douyu
{
    public class DouyuException : Exception
    {
        public DouyuException(string message)
            : base(message)
        { }

        public DouyuException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public DouyuException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
