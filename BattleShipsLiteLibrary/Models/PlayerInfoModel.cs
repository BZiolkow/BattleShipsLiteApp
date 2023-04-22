using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipsLiteLibrary.Models
{
    public class PlayerInfoModel
    {
        public string UserName { get; set; }
        public List<GridSpotModel> ShipLocations { get; set; } = new List<GridSpotModel>(); 
        public List<GridSpotModel> ShotLocations { get; set; } = new List<GridSpotModel>();

    }
}
