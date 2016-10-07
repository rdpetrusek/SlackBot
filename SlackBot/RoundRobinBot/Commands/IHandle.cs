using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot.SlackBotCommands
{
    public interface IBotCommand<T>
    {
        void Handle(string commandString);
        bool CanIHandleThis(string command);
    }

    public interface IBotCommand<TInput, TReturn>
    {
        TReturn Handle(string commandString);
        bool CanIHandleThis(string command);
    }
}
