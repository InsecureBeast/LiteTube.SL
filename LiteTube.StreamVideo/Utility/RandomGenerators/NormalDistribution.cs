using System;

namespace LiteTube.StreamVideo.Utility.RandomGenerators
{
  public class NormalDistribution
  {
    private readonly float _mean;
    private readonly IRandomGenerator _randomGenerator;
    private readonly float _standardDeviation;
    private float? _value;

    public NormalDistribution(IRandomGenerator randomGenerator, float mean, float standardDeviation)
    {
      if (randomGenerator == null)
        throw new ArgumentNullException("randomGenerator");
      if ((double) standardDeviation <= 0.0)
        throw new ArgumentOutOfRangeException("standardDeviation");
      this._randomGenerator = randomGenerator;
      this._mean = mean;
      this._standardDeviation = standardDeviation;
    }

    public float Next()
    {
      float num1;
      if (this._value.HasValue)
      {
        num1 = this._value.Value;
        this._value = new float?();
      }
      else
      {
        float num2;
        float num3;
        float num4;
        do
        {
          num2 = (float) (2.0 * (double) this._randomGenerator.NextFloat() - 1.0);
          num3 = (float) (2.0 * (double) this._randomGenerator.NextFloat() - 1.0);
          num4 = (float) ((double) num2 * (double) num2 + (double) num3 * (double) num3);
        }
        while ((double) num4 <= 0.0 || (double) num4 >= 1.0);
        float num5 = (float) Math.Sqrt(-2.0 * Math.Log((double) num4) / (double) num4);
        this._value = new float?(num3 * num5);
        num1 = num2 * num5;
      }
      return num1 * this._standardDeviation + this._mean;
    }
  }
}
