using Microsoft.Advertising.Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LiteTube.ViewModels.Nodes
{
    class AdvNodeViewModel : NodeViewModelBase
    {
        private FrameworkElement _element;
        private bool _isAvailable = true;

        public AdvNodeViewModel()
        {
            _element = GetAdvElement();
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

        private FrameworkElement GetAdvElement()
        {
            var adv = new AdControl()
            {
                AdUnitId = "11626916",
                ApplicationId = "a5239a3d-fa0b-4995-9c19-0f7a998b83c3",
                Height = 80,
                Width = 480,
            };

            adv.ErrorOccurred += Adv_ErrorOccurred;
            return adv;
        }

        private void Adv_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            //TODO Change advControl tu duplex or smaato
            //IsAvailable = false;
        }
    }
}
