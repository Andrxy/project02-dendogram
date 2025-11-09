using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project02_Dendogram.Models
{
    internal class ClusterJson
    {
        public string n { get; set; }
        public double d { get; set; }
        public List<ClusterJson> c { get; set; }

        public ClusterJson()
        {
            c = new List<ClusterJson>();
        }
    }
}
