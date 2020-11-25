using System.Collections.Generic;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Markdown;
using Statiq.Web;
using WebGenerator.Extensions;

namespace WebGenerator
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await Bootstrapper
                .Factory
                .CreateWeb(args)
                .ConfigureTemplates(templates =>
                {
                    if (templates.ContainsKeys(MediaTypes.Markdown))
                    {
                        templates.Remove(MediaTypes.Markdown);

                        var render = new RenderMarkdown()
                            .UseExtensions()
                            .UseExtension<ImageUrlMarkdownExtension>();
                        templates.Add(MediaTypes.Markdown, new Template(ContentType.Content, Phase.Process, render));
                    }
                })
                .SetThemePath("theme")
                .AddSetting(WebKeys.InputPaths, new [] { "docs" })
                .AddSetting(WebKeys.ExcludedPaths, new List<NormalizedPath>
                {
                    new NormalizedPath(".github"),
                    new NormalizedPath(".git"),
                    new NormalizedPath("assets/image1/.git"),
                })
                .RunAsync();
        }
    }
}
