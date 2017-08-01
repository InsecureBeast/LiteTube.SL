namespace LiteTube.StreamVideo.MediaManager
{
    public enum MediaManagerState
    {
        Idle,
        Opening,
        OpenMedia,
        Seeking,
        Playing,
        Closed,
        Error,
        Closing,
    }
}
