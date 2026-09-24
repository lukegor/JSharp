using System.Windows;

namespace JSharp.Services
{
    /// <summary>
    /// Abstraction over <see cref="MessageBox"/>
    /// </summary>
    public interface IMessageService
    {
        public abstract MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton buttonSettings, MessageBoxImage icon);

        public abstract MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton buttonSettings, MessageBoxImage icon);
    }
}
