using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;

namespace WebGenerator
{
    public class Program
    {
        private static readonly NormalizedPath ArtifactsFolder = "artifacts";

        public static async Task<int> Main(string[] args)
        {
            var rootPath = new NormalizedPath(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent;
            return await Bootstrapper
                .Factory
                .CreateWeb(args)
#if !DEBUG
                .AddSetting("SiteTitle", "Program Title")
                .ConfigureEngine(x =>
                {
                   x.FileSystem.RootPath = rootPath;
                   x.FileSystem.TempPath = "temp";
                   x.FileSystem.OutputPath = x.FileSystem.RootPath / ArtifactsFolder;
                })
                .SetThemePath(rootPath / "theme")
                .AddSetting(WebKeys.InputPaths, new [] { rootPath / "docs" })
                .AddSetting(WebKeys.ExcludedPaths, new List<NormalizedPath>
                {
                    new NormalizedPath(".gen"),
                    new NormalizedPath(".github"),
                    new NormalizedPath(".git"),
                })
#endif
                .RunAsync();
        }
    }
}
