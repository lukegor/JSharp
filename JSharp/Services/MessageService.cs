using JSharp.Services;
using System.Windows;

namespace JSharp.Services
{
    /// <inheritdoc cref="IMessageService"/>
    internal class MessageService : IMessageService
    {
        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton buttonSettings, MessageBoxImage icon)
        {
            return MessageBox.Show(message, caption, buttonSettings, icon);
        }

        public MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton buttonSettings, MessageBoxImage icon)
        {
            return MessageBox.Show(owner, message, caption, buttonSettings, icon);
        }
    }
}
