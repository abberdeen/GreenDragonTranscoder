using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.Core.Services.SlateService
{
    public class SlateOptions
    {
        public string FontFile { get; set; }
        public string SlateDrawingConfigPath { get; set; }
        public string SlateBackgroundPath { get; set; } 
        public SlateInfo  SlateInfo { get; set; }   
    } 
}
