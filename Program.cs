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
                })
                .RunAsync();
        }
    }
}
