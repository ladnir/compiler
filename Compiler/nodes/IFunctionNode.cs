using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface IFunctionNode
    {

        List<ParamNode> getParameters();
        string getName();
        string getReturnType();

        //List<Gate> NodeOutGates { get; }

        //void toCiruit(List<Gate> gates, int nextWireID);
    }
}
