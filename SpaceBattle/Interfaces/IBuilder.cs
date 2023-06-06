using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Interfaces
{
    public interface IBuilder
    {
        IBuilder addMembers(string param, params object[] args);
        object Build();
    }
}
