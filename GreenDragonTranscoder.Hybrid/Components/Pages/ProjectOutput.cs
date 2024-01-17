using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.Hybrid.Components.Pages
{
    public class ProjectOutput
    {
        public string Codec { get; set; }
        public string FPS { get; set; }
        public double Progress { get; set; }
        public bool Started { get; set; }

        public ProjectOutput(string codec, string fps, double progress, bool started)
        {
            Codec = codec;
            FPS = fps;
            Progress = progress;
            Started = started;
        }
    }
}
