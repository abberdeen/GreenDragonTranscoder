using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.MauiCore.Services
{
    public static class FileService
    {
        public static string AppPath =>
            AppContext.BaseDirectory;

        public static string AppDataPath => Path.Combine(AppPath, "data");
    }
}
