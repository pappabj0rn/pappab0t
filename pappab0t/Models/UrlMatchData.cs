using System;
using System.Text;
using pappab0t.Extensions;

namespace pappab0t.Models
{
    public class UrlMatchData : IEquatable<UrlMatchData>
    {
        public UrlMatchData()
        {
            Protocol = "http";
        }

        public UrlMatchData(UrlMatchData data)
        {
            Protocol = data.Protocol;
            Domain = data.Domain;
            Path = data.Path;
            FileName = data.FileName;
            Query = data.Query;
            Anchor = data.Anchor;
            TargetType = data.TargetType;
        }

        public string Protocol { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Query { get; set; }
        public string Anchor { get; set; }
        public UrlTargetType TargetType { get; set; }

        public static UrlMatchData Empty => new UrlMatchData
        {
            Protocol = "",
            Domain = "",
            Path = "",
            FileName = "",
            Query = "",
            Anchor = "",
            TargetType = UrlTargetType.Other
        };

        public bool Equals(UrlMatchData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Protocol, other.Protocol) 
                && string.Equals(Domain, other.Domain) 
                && string.Equals(Path, other.Path) 
                && string.Equals(FileName, other.FileName) 
                && string.Equals(Query, other.Query)
                && string.Equals(Anchor, other.Anchor)
                && TargetType == other.TargetType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UrlMatchData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Protocol != null ? Protocol.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Domain != null ? Domain.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FileName != null ? FileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Query != null ? Query.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Anchor != null ? Anchor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TargetType.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(UrlMatchData left, UrlMatchData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UrlMatchData left, UrlMatchData right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Protocol);
            sb.Append("://");
            sb.Append(Domain);
            sb.Append(Path);
            sb.Append(FileName);
            sb.Append(Query.IsNullOrEmpty() ? "" : $"?{Query}");
            sb.Append(Anchor.IsNullOrEmpty() ? "" : $"#{Anchor}");

            return sb.ToString();
        }
    }
}