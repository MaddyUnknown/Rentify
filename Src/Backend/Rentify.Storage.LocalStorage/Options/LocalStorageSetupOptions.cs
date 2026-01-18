using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Storage.LocalStorage.Options
{
    public class LocalStorageSetupOptions
    {
        public string? RootFolder { get; set; }
        public int? StreamBufferSize { get; set; }
    }
}
