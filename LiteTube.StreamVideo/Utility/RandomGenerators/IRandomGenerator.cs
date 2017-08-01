namespace LiteTube.StreamVideo.Utility.RandomGenerators
{
    public interface IRandomGenerator<out T> : IRandomGenerator
    {
        T Next();
    }

    public interface IRandomGenerator
    {
        void GetBytes(byte[] buffer, int offset, int count);

        float NextFloat();

        double NextDouble();

        void Reseed();
    }
}
