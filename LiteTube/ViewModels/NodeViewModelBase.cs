
namespace LiteTube.ViewModels
{
    public abstract class NodeViewModelBase : PropertyChangedBase
    {
        public abstract string Id { get; }
        public abstract string VideoId { get; }
    }
}
