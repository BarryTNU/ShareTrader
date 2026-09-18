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
        // Used for Information / Warning / Error messages.
        public static Func<string, string, MessageType, Task>? ShowAsync;

        // Used for Yes/No questions.
        public static Func<string, string, Task<bool>>? ShowQuestionAsync;

        // Which button gets the keyboard focus.
        public static DefaultButton DefaultFocus { get; set; } = DefaultButton.OK;
    }

    public enum DefaultButton
    {
        OK,
        Yes,
        No,
        Exit
    }

}

