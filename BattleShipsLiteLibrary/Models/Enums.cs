using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipsLiteLibrary.Models
{
    public enum GridSpotStatus
    {
        Empty,
        Ship,
        Miss,
        Hit,
        Sunk,
        BattleShip,
        Damaged
    }
}
