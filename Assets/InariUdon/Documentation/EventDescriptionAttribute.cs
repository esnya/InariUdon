using System;
using System.Collections.Generic;

namespace InariUdon.Documentation
{
    public class EventDescriptionAttribute : Attribute
    {
        public string description;
        public EventDescriptionAttribute(string description)
        {
            this.description = description;
        }
    }

    public class ImageAttachments : Attribute
    {
        public string[] urls;
        public ImageAttachments(string url) : this(new [] { url }) {}
        public ImageAttachments(string[] urls)
        {
            this.urls = urls;
        }
    }
}
