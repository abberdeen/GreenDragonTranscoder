using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.MauiCore.Services.SlateService
{
    public class SlateOptions
    {
     //   public string FontFile { get; set; } = string.Empty;
        public Stream? FontFile{ get; set; }
        public Stream? SlateDrawingConfig{ get; set; }
        public Stream? SlateBackground { get; set; }
        //public string SlateDrawingConfigPath { get; set; } = string.Empty;
        //public string SlateBackgroundPath { get; set; } = string.Empty;
        public SlateInfo  SlateInfo { get; set; } = new SlateInfo();  
    } 
}
