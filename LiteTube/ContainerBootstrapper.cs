using LiteTube.Common;
using LiteTube.DataModel;

namespace LiteTube
{
    class ContainerBootstrapper
    {
        private IDialogService _dialogService;
        private Context _context;
        private IPurchase _purchase;

        public void Build()
        {
            _context = new Context();
            _context.BuidContext();
            _dialogService = new DialogService();
            _purchase = new Purchase();
            //_purchase = new PurchaseMock();
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

        internal IPurchase Purchase
        {
            get { return _purchase; }
        }
    }
}
