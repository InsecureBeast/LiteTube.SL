namespace LiteTube.StreamVideo.M3U8
{
  public class M3U8AttributeInstance
  {
    public readonly M3U8Attribute Attribute;

    public M3U8AttributeInstance(M3U8Attribute attribute)
    {
      this.Attribute = attribute;
    }

    public override string ToString()
    {
      return this.Attribute.Name;
    }
  }
}
