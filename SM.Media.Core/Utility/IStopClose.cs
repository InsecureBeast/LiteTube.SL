using System;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public interface IStopClose : IDisposable
  {
    Task StopAsync();

    Task CloseAsync();
  }
}
