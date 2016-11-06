using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_1
{
    public class EdgeChangedEventArgs : EventArgs
    {
        public Edge Edge { get; set; }

        public EdgeChangedEventArgs(Edge Edge)
        {
            this.Edge = Edge;
        }
    }
}
