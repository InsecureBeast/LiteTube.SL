using System;
using System.Windows.Input;
using LiteTube.DataModel;

namespace LiteTube.Common
{
    class UnsubscribeCommand : ICommand
    {
        private readonly Func<IDataSource> _getDataSource;
        private readonly Func<string> _getChannelId;
        private readonly Action _postAction;
        private bool _isRequested;

        public UnsubscribeCommand(Func<IDataSource> getDataSource, Func<string> getChannelId, Action postAction)
        {
            _getDataSource = getDataSource;
            _getChannelId = getChannelId;
            _postAction = postAction;
        }

        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
        {
            return !_isRequested && _getDataSource().IsAuthorized;
        }

        public void Execute(object parameter)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                InvalidateCommands(true);

                var subscriptionId = _getDataSource().GetSubscriptionId(_getChannelId());
                if (string.IsNullOrEmpty(subscriptionId))
                    return;

                await _getDataSource().Unsubscribe(subscriptionId);
                _postAction();
                InvalidateCommands(false);
            });
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void InvalidateCommands(bool isRequested)
        {
            _isRequested = isRequested;
            RaiseCanExecuteChanged();
        }
    }
}
