using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface IFunctionNode
    {

        LinkedList<ParamNode> getParameters();
        string getName();
        string getReturnType();
    }
}
