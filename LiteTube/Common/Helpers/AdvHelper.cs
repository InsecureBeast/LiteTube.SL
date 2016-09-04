using LiteTube.ViewModels.Nodes;
using System.Collections;

namespace LiteTube.Common.Helpers
{
    static class AdvHelper
    {
        private static int AdvCount = 30;

        public static void AddAdv(IList items, bool showAdv)
        {
            if ((items.Count - 10) % AdvCount == 0 && items.Count != 0 && showAdv)
            {
                items.Add(new AdvNodeViewModel());
            }
        }
    }
}
