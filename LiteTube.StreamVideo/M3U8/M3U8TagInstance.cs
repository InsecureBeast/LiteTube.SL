namespace LiteTube.StreamVideo.M3U8
{
  public class M3U8TagInstance
  {
    public readonly M3U8Tag Tag;

    public M3U8TagInstance(M3U8Tag tag)
    {
      this.Tag = tag;
    }

    public override string ToString()
    {
      return this.Tag.ToString();
    }
  }
}
