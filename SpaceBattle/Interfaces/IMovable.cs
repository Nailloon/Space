using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Classes;

namespace SpaceBattle.Interfaces
{
    public interface IMovable
    {
        Vector Position { get; set; }
        Vector Velocity { get; }
    }
}
