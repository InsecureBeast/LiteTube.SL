using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTube.DataClasses
{
    public interface IProfile
    {
        string Image { get; }
        string DisplayName { get; }
        DateTime? Registered { get; }
    }

    class MProfile : IProfile
    {
        public MProfile(string image, string displayName)
        {
            Image = image;
            DisplayName = displayName;
        }

        public MProfile(string image, string displayName, DateTime? registered) : this(image, displayName)
        {
            Registered = registered;
        }

        public string DisplayName
        {
            get;
            private set;
        }

        public string Image
        {
            get;
            private set;
        }

        public DateTime? Registered
        {
            get;
            private set;
        }
    }
}
