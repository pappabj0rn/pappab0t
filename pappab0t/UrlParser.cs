using System;
using System.Text.RegularExpressions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t
{
    public class UrlParser : IUrlParser
    {
        public UrlMatchData Parse(string url)
        {
            try
            {
                if (url.StartsWith("<"))
                    url = url.TrimStart('<');

                if (url.EndsWith(">"))
                    url = url.TrimEnd('>');

                if (url.Contains("|"))
                    url = url.Substring(0,url.IndexOf("|"));

                var uri = new Uri(url);

                var data = new UrlMatchData
                {
                    Protocol = uri.Scheme,
                    Domain = uri.Host,
                    FileName = GetFileName(uri),
                    Path = GetPath(uri),
                    Query = uri.Query.Replace("?",""),
                    Anchor = uri.Fragment.Replace("#", ""),
                    TargetType = UrlTargetType.Other
                };

                UpdateTargetType(data);

                return data;
            }
            catch (Exception)
            {
                return UrlMatchData.Empty;
            }
        }

        private static string GetFileName(Uri uri)
        {
            var lastSegment = uri.Segments[uri.Segments.Length - 1];

            return lastSegment.Contains(".")
                ? lastSegment
                : "";
        }

        private static string GetPath(Uri uri)
        {
            var fileName = GetFileName(uri);

            var path = fileName.IsNullOrEmpty()
                ? uri.AbsolutePath.EndsWith("/")
                    ? uri.AbsolutePath
                    : uri.AbsolutePath + "/"
                : uri.AbsolutePath.Replace(fileName, "");
            return path;
        }

        private void UpdateTargetType(UrlMatchData data)
        {
            if(IsVideo(data))
                data.TargetType = UrlTargetType.Video;

            else if (IsImage(data))
                data.TargetType = UrlTargetType.Image;

            else if (IsDocument(data))
                data.TargetType = UrlTargetType.Document;

            else if (IsMusic(data))
                data.TargetType = UrlTargetType.Music;
        }

        private bool IsVideo(UrlMatchData data)
        {
            return IsYoutubeVideo(data)
                   || IsVimeoVideo(data)
                   || IsFacebookVideo(data)
                   || data.FileName.EndsWith(".gifv");
        }

        private bool IsVimeoVideo(UrlMatchData data)
        {
            var match = Regex.Match(data.Path, "/[\\d]+/");

            return data.Domain.EndsWith("vimeo.com")
                   && match.Success;
        }

        private bool IsFacebookVideo(UrlMatchData data)
        {
            var match = Regex.Match(data.Path, "/videos/[\\d]+/$");

            return data.Domain.EndsWith("facebook.com")
                   && match.Success;
        }

        private bool IsYoutubeVideo(UrlMatchData data)
        {
            switch (data.Domain)
            {
                case "www.youtube.com":
                    if (data.Path == "/watch/" && data.Query.Contains("v="))
                        return true;
                    if (data.Path == "/attribution_link/" && data.Query.Contains("watch"))
                        return true;
                    if (data.Path.StartsWith("/embed/"))
                        return true;
                    if (data.Path.StartsWith("/v/"))
                        return true;
                    break;
                case "youtu.be":
                    return true;
            }
            return false;
        }

        private bool IsImage(UrlMatchData data)
        {
            return data.FileName.EndsWith(".jpg")
                    || data.FileName.EndsWith(".jpeg")
                    || data.FileName.EndsWith(".gif")
                    || data.FileName.EndsWith(".png")
                    || data.FileName.EndsWith(".bmp")
                    || data.FileName.EndsWith(".pcx")
                    || data.FileName.EndsWith(".tiff");
        }

        private bool IsDocument(UrlMatchData data)
        {
            return data.FileName.EndsWith(".pdf")
                   || data.FileName.EndsWith(".txt")
                   || data.FileName.EndsWith(".doc")
                   || data.FileName.EndsWith(".docx")
                   || data.FileName.EndsWith(".xls")
                   || data.FileName.EndsWith(".xlsx");
        }

        private bool IsMusic(UrlMatchData data)
        {
            return IsSpotify(data);
        }

        private bool IsSpotify(UrlMatchData data)
        {
            return data.Domain.EndsWith("spotify.com")
                   && (data.Path.StartsWith("/track/")
                       || data.Path.Contains("/playlist/"));
        }
    }
}