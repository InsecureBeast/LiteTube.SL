using System.IO;

namespace SM.Media.Core.Utility
{
  public interface IPlatformServices
  {
    double GetRandomNumber();

    Stream Aes128DecryptionFilter(Stream stream, byte[] key, byte[] iv);

    void GetSecureRandom(byte[] bytes);
  }
}
