using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Statiq.Common;
using System;
using System.Linq;

namespace WebGenerator.Extensions
{
    class ImageUrlMarkdownExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // Make sure we don't have a delegate twice
            pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
            pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private void PipelineOnDocumentProcessed(MarkdownDocument document)
        {
            var nodes = document.Descendants()
                .Where(x => x is LinkInline)
                .Select(x => x as LinkInline)
                .Where(x => x.IsImage)
                ;
            foreach (var link in nodes)
            {
                if (!Uri.TryCreate(link.Url, UriKind.RelativeOrAbsolute, out var uri))
                {
                    continue;
                }

                if (uri.IsAbsoluteUri)
                {
                    continue;
                }

                var path = new NormalizedPath(uri.ToString());
                var segments = path.Segments.SkipWhile(x => x.SequenceEqual(NormalizedPath.DotDot.AsMemory())).ToList();
                if (segments.Count <= 2)
                {
                    continue;
                }

                if (!segments[0].SequenceEqual("assets".AsMemory()))
                {
                    continue;
                }

                var sub = segments[1].ToString();
                var rootUrl = string.Format("https://{0}.xkyii.com/", sub);
                if (!rootUrl.EndsWith("/"))
                {
                    rootUrl += "/";
                }
                var url = rootUrl + string.Join("/", segments.Skip(2));

                var config = Config.FromDocument(doc =>
                {
                    return doc.GetString("ImageRootUrl");
                });

                link.Url = url;
                ;
            }
        }
    }
}
