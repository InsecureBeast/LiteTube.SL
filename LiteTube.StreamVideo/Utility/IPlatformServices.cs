using System.IO;

namespace LiteTube.StreamVideo.Utility
{
  public interface IPlatformServices
  {
    double GetRandomNumber();

    Stream Aes128DecryptionFilter(Stream stream, byte[] key, byte[] iv);

    void GetSecureRandom(byte[] bytes);
  }
}
