using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public interface LocalScope
    {
        bool inScope(Token name);
        void addToScope(DeclarationNode localVar);

        bool funcInScope(CallNode function);
        bool funcInScope(FunctionNode fn);

        string getDataType(Token varName);
    }
}
