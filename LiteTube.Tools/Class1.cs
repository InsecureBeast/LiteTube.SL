using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteTube.Tools
{
    public class Tools
    {
        public static bool IsRotationOn()
        {
            var info = Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences;
            


            return true;
        }

    }
}
