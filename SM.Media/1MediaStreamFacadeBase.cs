// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaStreamFacadeExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Buffering;
using SM.Media.MediaManager;
using SM.Media.MediaParser;
using SM.Media.Utility;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media
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
