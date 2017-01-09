using LiteTube.Common;
using Microsoft.AdMediator.WindowsPhone8;
using MyToolkit.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LiteTube.ViewModels.Nodes
{
    class AdvNodeViewModel : NodeViewModelBase
    {
        private FrameworkElement _element;
        private bool _isAvailable = true;
        private readonly RelayCommand<AdMediatorControl> _mediatorErrorCommand;

        public AdvNodeViewModel()
        {
            _mediatorErrorCommand = new RelayCommand<AdMediatorControl>(OnMediatorError);
        }

        public override string Id
        {
            get
            {
                var guid = Guid.NewGuid().ToString();
                var id = guid.Replace("-", "");
                return id;
            }
        }

        public override string VideoId
        {
            get { return string.Empty; }
        }

        public FrameworkElement Element
        {
            get { return _element; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                NotifyOfPropertyChanged(() => IsAvailable);
            }
        }

        public ICommand MediatorErrorCommand
        {
            get { return _mediatorErrorCommand; }
        }

        private void OnMediatorError(AdMediatorControl adMediator)
        {

        }
    }
}
