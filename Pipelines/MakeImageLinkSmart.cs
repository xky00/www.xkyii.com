using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Statiq.Common;
using Statiq.Html;

namespace WebGenerator.Extensions
{
    internal class MakeImageLinkSmart : ProcessHtml
    {
        public MakeImageLinkSmart() : base("[src]", ProcessElement)
        {
        }

        private static void ProcessElement(Statiq.Common.IDocument document, IExecutionContext context, IElement element, Dictionary<string, object> pairs)
        {
            if (element is IHtmlImageElement image)
            {
                MakeImageLinSmart(image, "src", document, context);
            }
        }

        private static void MakeImageLinSmart(IHtmlImageElement element, string attribute, Statiq.Common.IDocument document, IExecutionContext context)
        {
            string value = element.GetAttribute(attribute);
            if (!string.IsNullOrEmpty(value)
                && Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri uri)
                && !uri.IsAbsoluteUri)
            {
                var path = new NormalizedPath(uri.ToString());
                var segments = path.Segments.SkipWhile(x => x.SequenceEqual(NormalizedPath.DotDot.AsMemory())).ToList();
                if (segments.Count <= 2)
                {
                    return;
                }

                if (!segments[0].SequenceEqual("assets".AsMemory()))
                {
                    return;
                }

                var sub = segments[1].ToString();
                var urlFormat = document.Get<string>(GenKeys.ImageUrlFormat);
                var rootUrl = string.Format(urlFormat, sub);
                if (!rootUrl.EndsWith("/"))
                {
                    rootUrl += "/";
                }
                var url = rootUrl + string.Join("/", segments.Skip(2));

                element.SetAttribute(attribute, url);
            }
        }
    }
}
