using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShipsLiteLibrary;
using BattleShipsLiteLibrary.Models;

namespace BattleshipLite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesTheGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesTheGameContinue)
                {
                    //Swap using temp value
                    //PlayerInfoModel tempHolder = opponent;
                    //opponent = activePlayer;
                    //activePlayer = tempHolder;

                    //Swap using tuple
                    (activePlayer,opponent) = (opponent,activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }


            } while (winner == null);

            IdentifyWinner(winner);

            Console.Read();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to { winner.UserName } for winning!");
            Console.WriteLine($"{ winner.UserName } took { GameLogic.GetShotCount(winner) } shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string shot;

            do
            {
                shot = AskForShot(activePlayer);
                isValidShot = GameLogic.ValidateShot(activePlayer,shot);

                if (!isValidShot)
                {
                    Console.WriteLine("Invalid coordinates. Try again!");
                }
            } while (!isValidShot);

            bool isAHit = GameLogic.IdentifyShotResults(opponent, shot);

            GameLogic.MarkShotResult(activePlayer, opponent, shot, isAHit);

            Console.WriteLine();

            if (isAHit)
            {
                Console.WriteLine("Hit!"); 
            }
            else
            {
                Console.WriteLine("Miss!");
            }
            Console.WriteLine();
        }

        private static string AskForShot(PlayerInfoModel activePlayer)
        {
            Console.Write($"{ activePlayer.UserName } please enter shot coordinates: ");
            string output = Console.ReadLine();
            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotLocations[0].SpotLetter;

            foreach (GridSpotModel spot in activePlayer.ShotLocations)
            {
                if (currentRow != spot.SpotLetter)
                {
                    Console.WriteLine();
                    currentRow = spot.SpotLetter;
                }
                if (spot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" { spot.SpotLetter }{ spot.SpotNumber } ");
                }
                else if (spot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" OO ");
                }
                else if(spot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" XX ");
                }
                else
                {
                    Console.Write(" ? ");
                }
                
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player information for { playerTitle }");

            output.UserName = AskForUserName();

            GameLogic.InitializeGrid(output);

            Console.WriteLine("Please place 5 ships");

            PlaceShips(output);

            Console.Clear();

            return output;
        }

        private static string AskForUserName()
        {
            Console.Write("What is your name ? ");
            string output = Console.ReadLine();
            return output;
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to BattleshipLite!");
            Console.WriteLine("Enjoy yourself!");
            Console.WriteLine();
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            string location = "";
            bool isValidLocation = false;
            do
            {
                Console.Write($"Where do you want to place ship number { model.ShipLocations.Count + 1 }: ");
                location = Console.ReadLine();

                isValidLocation = GameLogic.PlaceShip(model, location);

                if (!isValidLocation)
                {
                    Console.WriteLine("That is not valid location. Try again.");
                }
            } while (model.ShipLocations.Count < 4);

            Console.Write("Where do you want to place your battleship: ");

            do
            {
                location = Console.ReadLine();
                isValidLocation = GameLogic.PlaceBattleship(model, location);
                if (!isValidLocation)
                {
                    Console.WriteLine("That is not valid location. Try again.");
                } 
            } while (!isValidLocation);

        }
    }
}
