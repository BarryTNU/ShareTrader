using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader.Helpers
{
        public enum MessageType
        {
        Information,          
        Question,
        Warning,
        Error
    }

        public static class CustomMessageBox
        {
            public static Func<string, string, MessageType, Task>? ShowAsync;
            public static Func<string, string, Task<bool>>? ShowQuestionAsync;
    }

 }

