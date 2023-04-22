using BattleShipsLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipsLiteLibrary
{
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            List<int> numbers = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };

            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }

        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };
            model.ShotLocations.Add(spot);
        }

        public static bool PlayerStillActive(PlayerInfoModel opponent)
        {
            var myLinqQuery = from shipSpot in opponent.ShipLocations
                              where shipSpot.Status == GridSpotStatus.Ship ||
                              shipSpot.Status == GridSpotStatus.BattleShip ||
                              shipSpot.Status == GridSpotStatus.Damaged
                              select shipSpot;

            if (myLinqQuery.Count() > 0)
            {
                return true;
            }

            return false;
        }

        public static bool PlaceShip(PlayerInfoModel model, string location)
        {
            bool output = false;
            (string row, int column, bool success) = LocationToCords(location);

            bool isValidLocation = ValidateGridLocation(model, location);
            bool isSpotOpen = ValidateShipLocation(model, location);

            if (isValidLocation && isSpotOpen && success)
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row,
                    SpotNumber = column,
                    Status = GridSpotStatus.Ship
                });

                output = true;
            }
            return output;

        }

        private static bool ValidateShipLocation(PlayerInfoModel model, string location)
        {
            (string row, int column, bool success) = LocationToCords(location);

            var spot = from spots in model.ShipLocations
                       where spots.SpotLetter == row &&
                       spots.SpotNumber == column &&
                       spots.Status == GridSpotStatus.Ship
                       select spots;
            if (spot.Count() == 0 && success)
            {
                return true;
            }
            return false;
        }

        private static bool ValidateGridLocation(PlayerInfoModel model, string location)
        {
            (string row, int column, bool success) = LocationToCords(location);

            var spot = from spots in model.ShotLocations
                       where spots.SpotLetter.Equals(row) &&
                       spots.SpotNumber.Equals(column)
                       select spots;
            if (spot.Count() == 1 && success)
            {
                return true;
            }
            return false;
        }

        public static int GetShotCount(PlayerInfoModel winner)
        {
            var shots = from shot in winner.ShotLocations
                        where shot.Status == GridSpotStatus.Hit ||
                        shot.Status == GridSpotStatus.Miss
                        select shot;
            return shots.Count() + 1;
        }

        public static bool ValidateShot(PlayerInfoModel activePlayer, string shot)
        {
            (string row, int column, bool success) = LocationToCords(shot);

            var spot = from spots in activePlayer.ShotLocations
                       where spots.SpotLetter.Equals(row) &&
                       spots.SpotNumber.Equals(column) &&
                       spots.Status != GridSpotStatus.Miss
                       select spots;
            if (spot.Count() == 1)
            {
                return true;
            }
            return false;
        }

        public static bool IdentifyShotResults(PlayerInfoModel opponent, string shot)
        {
            (string row, int column, bool success) = LocationToCords(shot);

            var spot = from spots in opponent.ShipLocations
                       where spots.SpotLetter == row &&
                       spots.SpotNumber == column &&
                       (spots.Status == GridSpotStatus.Ship ||
                       spots.Status == GridSpotStatus.BattleShip ||
                       spots.Status == GridSpotStatus.Damaged)
                       select spots;
            if (spot.Count() == 1 && success)
            {
                return true;
            }
            return false;
        }

        public static void MarkShotResult(PlayerInfoModel activePlayer, PlayerInfoModel opponent, string shot, bool isAHit)
        {
            (string row, int column, bool success) = LocationToCords(shot);

            foreach (var gridSpot in activePlayer.ShotLocations)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if (isAHit)
                    {
                        gridSpot.Status = GridSpotStatus.Hit;
                        
                    }
                    else
                    {
                        gridSpot.Status = GridSpotStatus.Miss;
                    }
                }
            }
            var validSpots = from spot in opponent.ShipLocations
                             where spot.SpotLetter == row &&
                             spot.SpotNumber == column &&
                             (spot.Status == GridSpotStatus.Ship ||
                             spot.Status == GridSpotStatus.Damaged)
                             select spot;
            foreach (GridSpotModel spot in validSpots)
            {
                spot.Status = GridSpotStatus.Sunk;
            }
            var battleship = from spot in opponent.ShipLocations
                             where spot.SpotLetter == row &&
                             spot.SpotNumber == column &&
                             spot.Status == GridSpotStatus.BattleShip
                             select spot;
            foreach (GridSpotModel spot in battleship)
            {
                spot.Status = GridSpotStatus.Damaged;
            }
        }

        public static bool PlaceBattleship(PlayerInfoModel model, string location)
        {
            bool output = false;
            (string row, int column, bool success) = LocationToCords(location);

            bool isValidLocation = ValidateGridLocation(model, location);
            bool isSpotOpen = ValidateShipLocation(model, location);

            if (isValidLocation && isSpotOpen && success)
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row,
                    SpotNumber = column,
                    Status = GridSpotStatus.BattleShip
                });

                output = true;
            }
            return output;
        }

        private static (string row, int column, bool success) LocationToCords(string location)
        {
            string row = "";
            int column = 0;
            bool success = false;
            bool validLength = true;
            bool isNumber = true;

            if (location.Length != 2)
            {
                validLength = false;
            }

            else
            {
                char[] shotArray = location.ToArray();

                row = shotArray[0].ToString();
                bool isCapitalLetter = Char.IsUpper(row, 0);

                if (int.TryParse(shotArray[1].ToString(), out column))
                {
                    column = int.Parse(shotArray[1].ToString());
                }
                else
                {
                    isNumber = false;
                }
                if (isNumber && validLength && isCapitalLetter)
                {
                    success = true;
                } 
            }

            return (row, column, success);
        }
    }
}
