using Microsoft.Extensions.DependencyInjection;
using Rentify.Storage.Core;
using Rentify.Storage.LocalStorage.Constants;
using Rentify.Storage.LocalStorage.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.LocalStorage.Extensions
{
    public static class LocalStorageExtension
    {
        public static void AddLocalStorageServices(this IServiceCollection serviceCollection, Action<LocalStorageSetupOptions> optionsSetupAction)
        {
            LocalStorageSetupOptions storageOptions = new LocalStorageSetupOptions();
            optionsSetupAction(storageOptions);

            if (storageOptions.RootFolder == null) throw new ArgumentNullException(nameof(storageOptions.RootFolder), FileStorageConstant.RootPathNotConfigurated);

            serviceCollection.AddTransient<IFileStorageManager>((provider) => 
                storageOptions.StreamBufferSize.HasValue 
                ? new LocalFileStorageManager(storageOptions.RootFolder, storageOptions.StreamBufferSize.Value) 
                : new LocalFileStorageManager(storageOptions.RootFolder)
             );
        }
    }
}
