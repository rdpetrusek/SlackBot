using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace SlackBot
{
    public abstract class SlackBotBase
    {
        private readonly SlackSocketClient _client = null;
        protected SlackBotBase(string authToken)
        {
            _client = new SlackSocketClient(authToken);
            _client.Connect(OnLogin, OnSocketConnect);
            _client.OnMessageReceived += MessageReceived;
        }

        protected List<User> GetUsers()
        {
            return _client.Users.Where(x => !x.IsSlackBot || !x.id.Equals(_client.MySelf.id)).ToList();
        }

        protected User GetUser(string id)
        {
            return _client.Users.Single(x => x.id.Equals(id));
        }

        protected abstract void MessageReceived(NewMessage message);

        protected abstract void OnLogin(LoginResponse response);

        protected abstract void OnSocketConnect();

        protected bool DoesUserExist(string userName)
        {
            return GetUsers().Any(x => x.name.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
        }

        protected void Reply(NewMessage receivedMessage, string reply)
        {
            SendToUserId(receivedMessage.user, reply);
        }

        protected void SendToUserId(string id, string message, Action<MessageReceived> onReceipt = null)
        {
            var user = _client.Users.SingleOrDefault(x => x.id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
            if (user == null)
                return;

            var dmChannel = _client.DirectMessages.SingleOrDefault(x => x.user.Equals(user.id, StringComparison.InvariantCultureIgnoreCase));
            if (dmChannel == null)
                return;

            _client.SendMessage(onReceipt, dmChannel.id, message);
        }

        protected void SendToUserName(string userName, string message, Action<MessageReceived> onReceipt = null)
        {
            var user = _client.Users.SingleOrDefault(x => x.name.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
            if (user == null)
                return;

            var dmChannel = _client.DirectMessages.SingleOrDefault(x => x.user.Equals(user.id, StringComparison.InvariantCultureIgnoreCase));
            if (dmChannel == null)
                return;

            _client.SendMessage(onReceipt, dmChannel.id, message);
        }

        protected void SendToChannel(string channelName, string message, Action<MessageReceived> onReceipt = null)
        {
            var channel = _client.Channels.SingleOrDefault(x => x.name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));
            if (channel == null)
                return;

            _client.SendMessage(onReceipt, channel.id, message);
        }
    }
}
