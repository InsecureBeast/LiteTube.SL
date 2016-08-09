using LiteTube.Core.Common;
using LiteTube.Core.DataModel;

namespace LiteTubePro
{
    class ContainerBootstrapper
    {
        private IDialogService _dialogService;
        private Context _context;

        public void Build()
        {
            _context = new Context();
            _context.BuidContext();
            _dialogService = new DialogService();
        }

        internal IDataSource GetDataSource()
        {
            return _context.DataSource;
        }

        internal IDialogService DialogService
        {
            get { return _dialogService; }
        }

        internal IConnectionListener ConnectionListener
        {
            get { return _context.ConnectionListener; }
        }
    }
}
