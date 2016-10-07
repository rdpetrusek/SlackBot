using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace SlackBot
{
    public class RoundRobinSlackBot: SlackBotBase
    {
        public const string ExampleUserName = "JohnDoeUser";

        public List<string> Assignees = new List<string>();
        public List<string> AlreadyAssigned = new List<string>();
 
        public List<Command> Commands = new List<Command>(); 

        public RoundRobinSlackBot(string authToken) : base(authToken)
        {
            InitializeCommands();
        }

        public void InitializeCommands()
        {
            Commands.Add(Command.Create(name: "Add", 
                actionToTake:(value, message, bot) =>
                {
                    var userExists = DoesUserExist(value);
                    if (!userExists)
                    {
                        bot.Reply(message, string.Format("User \"{0}\" doesn't exist!", value));
                        return;
                    }

                    if (bot.Assignees.Any(x => x.Equals(value, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        bot.Reply(message, string.Format("User \"{0}\" is already in the queue!", value));
                        return;
                    }

                    bot.Assignees.Add(value);
                    bot.Reply(message, string.Format("Added \"{0}\" to the queue! Current members: {1}", value, string.Join(", ", bot.Assignees)));
                },
                valueMissingError: "You should tell me a username to add! ex: \"Add " + ExampleUserName + "\""));

            Commands.Add(Command.Create(name: "Remove", 
                actionToTake: (value, message, bot) =>
                {
                    var userExists = DoesUserExist(value);
                    if (!userExists)
                    {
                        bot.Reply(message, string.Format("User \"{0}\" doesn't exist!", value));
                        return;
                    }

                    if (!bot.Assignees.Any(x => x.Equals(value, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        bot.Reply(message, string.Format("User \"{0}\" isnt in the queue!", value));
                        return;
                    }

                    bot.Assignees.Remove(value);
                    var assigneesMembersText = bot.Assignees.Any() ? string.Format("Current members: {1}", string.Join(", ", bot.Assignees)) : "The list is empty!";
                    bot.Reply(message, string.Format("Removed \"{0}\" from the queue! {1}", value, assigneesMembersText));
                },
                valueMissingError: "You should tell me a username to remove! ex: \"Remove " + ExampleUserName + "\""));

            Commands.Add(Command.Create(name: "Next",
                actionToTake: (value, message, bot) =>
                {
                    if (!Assignees.Any())
                    {
                        bot.Reply(message, "No assignees yet! Add some with the add command ex: \"Add " + ExampleUserName + "\"");
                    }
                    var urlRegex = new Regex("[^<](.*?)|");
                    var matched = urlRegex.Match(value);
                    var matches = urlRegex.Matches(value);
                    var ticketUrl = value;

                    var haventGoneYet = Assignees.FirstOrDefault(x => !AlreadyAssigned.Contains(x));
                    if (haventGoneYet == null)
                    {
                        AlreadyAssigned = new List<string>();
                        haventGoneYet = Assignees.First();
                    }

                    bot.SendToUserId(message.user, string.Format("You're up next! {0}", value));
                    AlreadyAssigned.Add(haventGoneYet);
                },
                valueMissingError: "You should tell me a username to remove! ex: \"Remove " + ExampleUserName + "\""));
        }

        public Command ParseCommand(string value)
        {
            var commandName = value.Split(' ').First();
            var command = Commands.SingleOrDefault(x => x.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));

            if (command == null)
                return null;
            var restOfCommandText = value.Substring(value.IndexOf(" ") + 1);

            command.SetValue(restOfCommandText);
            return command;
        }

        protected override void MessageReceived(NewMessage message)
        {
            var command = ParseCommand(message.text);
            if (command == null)
            {
                SendToUserId(message.user, string.Format("That command doesn't make any sense. THESE commands make sense: \"{0}\"", string.Join(", ", Commands.Select(x => x.Name))));
                return;
            }

            if (string.IsNullOrWhiteSpace(command.Value))
            {
                SendToUserId(message.user, command.ValueMissingError);
                return;
            }

            command.ActionToTake(command.Value, message, this);
        }

        protected override void OnLogin(LoginResponse response)
        {
        }

        protected override void OnSocketConnect()
        {
        }
    }

    public class Command
    {
        private Command()
        {
            
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public static Command Create(string name, Action<string, NewMessage, RoundRobinSlackBot> actionToTake, string valueMissingError)
        {
            return new Command
            {
                Name = name,
                ActionToTake = actionToTake,
                ValueMissingError = valueMissingError
            };
        }

        public string Name { get; set; }
        public string Value { get; private set; }
        public Action<string, NewMessage, RoundRobinSlackBot> ActionToTake { get; set; }
        public string ValueMissingError { get; set; }
    }

}
