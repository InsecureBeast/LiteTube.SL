using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Content
{
    public class ContentType : IEquatable<ContentType>
    {
        private readonly ICollection<string> _alternateMimeTypes;
        private readonly ICollection<string> _fileExts;
        private readonly ContentKind _kind;
        private readonly string _mimeType;
        private readonly string _name;

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public ICollection<string> AlternateMimeTypes
        {
            get
            {
                return this._alternateMimeTypes;
            }
        }

        public ICollection<string> FileExts
        {
            get
            {
                return this._fileExts;
            }
        }

        public ContentKind Kind
        {
            get
            {
                return this._kind;
            }
        }

        public string MimeType
        {
            get
            {
                return this._mimeType;
            }
        }

        public ContentType(string name, ContentKind kind, string mimeType, string fileExt, IEnumerable<string> alternateMimeTypes = null)
          : this(name, kind, mimeType, (IEnumerable<string>)new string[1]
          {
        fileExt
          }, alternateMimeTypes)
        {
        }

        public ContentType(string name, ContentKind kind, string mimeType, IEnumerable<string> fileExts, IEnumerable<string> alternateMimeTypes = null)
        {
            if (null == name)
                throw new ArgumentNullException("name");
            if (mimeType == null)
                throw new ArgumentNullException("mimeType");
            if (null == fileExts)
                throw new ArgumentNullException("fileExts");
            this._name = name;
            this._kind = kind;
            this._mimeType = mimeType;
            this._alternateMimeTypes = alternateMimeTypes == null ? (ICollection<string>)new List<string>() : (ICollection<string>)Enumerable.ToList<string>(alternateMimeTypes);
            this._fileExts = (ICollection<string>)Enumerable.ToList<string>(fileExts);
        }

        public static bool operator ==(ContentType a, ContentType b)
        {
            if (object.ReferenceEquals((object)a, (object)b))
                return true;
            if (object.ReferenceEquals((object)a, (object)null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(ContentType a, ContentType b)
        {
            return !(a == b);
        }

        public bool Equals(ContentType other)
        {
            if (object.ReferenceEquals((object)this, (object)other))
                return true;
            if (object.ReferenceEquals((object)null, (object)other))
                return false;
            return string.Equals(this._mimeType, other._mimeType, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this._mimeType);
        }

        public override bool Equals(object obj)
        {
            ContentType other = obj as ContentType;
            if (object.ReferenceEquals((object)null, (object)other))
                return false;
            return this.Equals(other);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", new object[2]
            {
        (object) this._name,
        (object) this._mimeType
            });
        }
    }
}
