using Microsoft.Extensions.DependencyInjection;
using Rentify.FileWorkflow.Core.Generators;
using Rentify.FileWorkflow.Core.Inspectors;
using Rentify.FileWorkflow.Implementation.Generators;
using Rentify.FileWorkflow.Implementation.Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.FileWorkflow.Implementation.Extensions
{
    public static class FileWorkflowExtension
    {
        public static void AddFileWorkflowServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IFileInspector, FileInspector>();
            serviceCollection.AddSingleton<IImageThumbnailGenerator, ImageThumbnailGenerator>();

        }
    }
}
