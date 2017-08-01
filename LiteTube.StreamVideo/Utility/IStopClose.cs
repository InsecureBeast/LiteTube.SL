using System;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public interface IStopClose : IDisposable
  {
    Task StopAsync();

    Task CloseAsync();
  }
}
