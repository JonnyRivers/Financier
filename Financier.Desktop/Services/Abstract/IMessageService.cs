using System;

namespace Financier.Desktop.Services
{
    public interface IMessageService
    {
        void Register<T>(Action<T> action);
        void Unregister<T>(Action<T> action);
        void Send<T>(T message);
    }
}
