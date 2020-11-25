using System.Collections.Generic;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;
#if !DEBUG
using Statiq.Core;
using Statiq.Web.Pipelines;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace WebGenerator
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await Bootstrapper
                .Factory
                .CreateWeb(args)
                .SetThemePath("theme")
                .AddSetting(WebKeys.InputPaths, new [] { "docs" })
                .AddSetting(WebKeys.ExcludedPaths, new List<NormalizedPath>
                {
                    new NormalizedPath(".github"),
                    new NormalizedPath(".git"),
                    new NormalizedPath("assets/image1/.git"),
                })
#if !DEBUG
                // do some trick
                .ConfigureEngine(engine =>
                {
                    var templates = engine.Services.GetRequiredService<Templates>();
                    foreach (var pipeline in engine.Pipelines.Values)
                    {
                        if (pipeline is Content content)
                        {
                            content.PostProcessModules.Clear();
                            content.PostProcessModules.Add(new Modules.RenderContentPostProcessTemplates(templates));
                        }
                        else if (pipeline is Archives archives)
                        {
                            archives.PostProcessModules.Clear();
                            archives.PostProcessModules.Add(
                                new ExecuteSwitch(Config.FromDocument(doc => doc.Get<ContentType>(WebKeys.ContentType)))
                                    .Case(ContentType.Data, templates.GetModule(ContentType.Data, Phase.PostProcess))
                                    .Case(ContentType.Content, (IModule)new Modules.RenderContentPostProcessTemplates(templates)));
                        }
                    }
                })
#endif
                .RunAsync();
        }
    }
}
