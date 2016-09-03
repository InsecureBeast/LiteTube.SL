using LiteTube.ViewModels.Nodes;
using System.Collections;

namespace LiteTube.Common.Helpers
{
    static class AdvHelper
    {
        public static void AddAdv(IList items, bool showAdv)
        {
            if (items.Count == 15 /*% SettingsHelper.AdvCount == 0*/ && items.Count != 0 && showAdv)
            {
                items.Add(new AdvNodeViewModel());
            }
        }
    }
}
