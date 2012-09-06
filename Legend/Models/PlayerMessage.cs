using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Legend.Models
{
    public enum MessageType
    {
        Normal,
        Title,
        Important, 
        Notification,
        Attack
    }

    public class PlayerMessages
    {
        public readonly List<PlayerMessage> Messages;

        public PlayerMessages()
        {
            Messages = new List<PlayerMessage>();
        }

        public PlayerMessages(List<PlayerMessage> messages)
        {
            Messages = messages;
        }

        public PlayerMessages(string message)
        {
            Messages = new List<PlayerMessage>{ new PlayerMessage(message) };
        }

        public PlayerMessages(string message, MessageType type)
        {
            Messages = new List<PlayerMessage>
            {
                new PlayerMessage
                {
                    Message = message,
                    Type = type
                }
            };
        }
    }

    public class PlayerMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        public string Message { get; set; }

        public PlayerMessage()
        {
            Type = MessageType.Normal;
        }

        public PlayerMessage(string message)
        {
            Message = message;
            Type = MessageType.Normal;
        }

        public PlayerMessage(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }
    }
}