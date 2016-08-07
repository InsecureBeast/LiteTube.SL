using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.ViewModels.Nodes
{
    class AddNodeViewModel : NodeViewModelBase
    {
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
    }
}
