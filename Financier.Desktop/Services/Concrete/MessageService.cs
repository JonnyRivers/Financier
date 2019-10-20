using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Services
{
    public class MessageService : IMessageService
    {
        private Dictionary<Type, List<Object>> m_actionsByType;
        private ILogger<MessageService> m_logger;

        public MessageService(ILoggerFactory loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<MessageService>();
            m_actionsByType = new Dictionary<Type, List<Object>>();
        }

        public void Register<T>(Action<T> action)
        {
            Type messageType = typeof(T);

            if (!m_actionsByType.ContainsKey(messageType))
                m_actionsByType.Add(messageType, new List<Object>());

            m_actionsByType[messageType].Add(action);
        }

        public void Unregister<T>(Action<T> action)
        {
            Type messageType = typeof(T);

            if (!m_actionsByType.ContainsKey(messageType))
                return;

            m_actionsByType[messageType].Remove(action);
        }

        public void Send<T>(T message)
        {
            Type messageType = typeof(T);

            if (!m_actionsByType.ContainsKey(messageType))
                return;

            foreach (Action<T> action in m_actionsByType[messageType].Cast<Action<T>>())
            {
                action(message);
            }
        }
    }
}
