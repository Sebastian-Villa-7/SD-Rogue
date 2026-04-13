using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Engine
{
    public static class MessageLog
    {
        public static string Message { get; private set; } = string.Empty;

        public static void Add(string message)
        {
            Message = message;
        }
    }
}
