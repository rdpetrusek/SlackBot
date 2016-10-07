using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot.SlackBotCommands
{
    interface ISlackCommand
    {
        bool CanIHandleThis(string commandText);
    }
}
