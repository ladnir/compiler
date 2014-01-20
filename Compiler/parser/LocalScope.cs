using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    interface LocalScope
    {
        bool inScope(Token name);
        void addToScope(DeclarationNode localVar);

    }
}
