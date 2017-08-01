using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.MediaManager;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo
{
  public static class MediaStreamFacadeExtensions
  {
    public static void RequestStop(this IMediaStreamFacadeBase mediaStreamFacade)
    {
      Task<bool> task = MediaStreamFacadeExtensions.RequestStopAsync(mediaStreamFacade, TimeSpan.FromSeconds(5.0), CancellationToken.None);
      TaskCollector.Default.Add((Task) task, "MediaStreamFacade RequestStop");
    }

    public static async Task<bool> RequestStopAsync(this IMediaStreamFacadeBase mediaStreamFacade, TimeSpan timeout, CancellationToken cancellationToken)
    {
      bool flag;
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
      {
        linkedTokenSource.CancelAfter(timeout);
        try
        {
          await mediaStreamFacade.StopAsync(linkedTokenSource.Token).ConfigureAwait(false);
          flag = true;
          goto label_10;
        }
        catch (OperationCanceledException ex)
        {
          Debug.WriteLine(!cancellationToken.IsCancellationRequested ? "RequestStop timeout" : "RequestStop canceled");
        }
        catch (Exception ex)
        {
          Debug.WriteLine("RequestStop failed: " + ExceptionExtensions.ExtendedMessage(ex));
        }
        flag = false;
      }
label_10:
      return flag;
    }

    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IMediaManagerParameters parameters)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IMediaManagerParameters>(parameters);
    }

    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IBufferingPolicy policy)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IBufferingPolicy>(policy);
    }

    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IMediaStreamConfigurator mediaStreamConfigurator)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IMediaStreamConfigurator>(mediaStreamConfigurator);
    }

    public static void SetParameter(this IMediaStreamFacadeBase mediaStreamFacade, IApplicationInformation applicationInformation)
    {
      mediaStreamFacade.Builder.RegisterSingleton<IApplicationInformation>(applicationInformation);
    }
  }
}
