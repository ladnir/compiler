using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class BlankNode : Node
    {
        override public void outputGForth(int tabCount, StringBuilder sb)
        {

            
        }
        override public string outputC(int tabCount)
        {

            return "";
        }
    }
}
