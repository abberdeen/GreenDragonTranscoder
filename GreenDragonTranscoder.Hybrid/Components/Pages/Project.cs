using GreenDragonTranscoder.Hybrid.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.Hybrid.Components.Pages
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Project(int id, string name)
        {
            Id = id;
            Name = name; 
        }
    }

}
