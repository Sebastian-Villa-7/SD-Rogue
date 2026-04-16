using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Engine
{

    public class MessageLog
    {
        // single instance
        private static MessageLog? _instance;

        // private constructor - nobody can create it directly
        private MessageLog() { }

        // only way to get the instance
        public static MessageLog Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageLog();
                return _instance;
            }
        }

        public string Message { get; private set; } = string.Empty;

        public void Add(string message)
        {
            Message = message;
        }
    }
}
