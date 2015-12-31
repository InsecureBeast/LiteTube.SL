using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTube.Common
{
    internal static class MediaPlayerVisualStates
    {
        internal static class GroupNames
        {
            internal const string InteractiveStates = "InteractiveStates";
            internal const string MediaStates = "MediaStates";
            internal const string PlayerStates = "PlayerStates";
            internal const string CaptionsStates = "CaptionsStates";
            internal const string FullScreenStates = "FullScreenStates";
            internal const string AdvertisingStates = "AdvertisingStates";
            internal const string PlayToStates = "PlayToStates";
            internal const string MediaTypeStates = "MediaTypeStates";
        }

        internal static class InteractiveStates
        {
            internal const string StartInteracting = "StartInteracting";
            internal const string StopInteracting = "StopInteracting";
            internal const string Visible = "Visible";
            internal const string Hidden = "Hidden";
        }
    }
}
