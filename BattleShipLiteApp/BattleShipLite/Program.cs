using System;
using BattleShipLiteLibrary;
using BattleShipLiteLibrary.Models;

namespace BattleShipLite
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player1");
            PlayerInfoModel opponent = CreatePlayer("Player2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue)
                {
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to {winner.UserName} for winning");
            Console.WriteLine($"{winner.UserName} took {GameLogic.GetShotsCount(winner)} shots ");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            var isValidShoot = false;
            string row = "";
            int column = 0;

            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShoot = GameLogic.ValidateShoot(activePlayer, row, column);
                }
                catch (Exception ex)
                {
                    isValidShoot = false;
                }

                if (!isValidShoot)
                {
                    Console.WriteLine("Invalid shot location. Please try again..");
                }

            } while (!isValidShoot);

            var isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResult(row, column, isAHit);
        }

        private static void DisplayShotResult(string row, in int column, in bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}{column} is a hit!");
            }
            else
            {
                Console.WriteLine($"{row}{column} is a miss!");
            }

            Console.WriteLine();
        }

        private static string AskForShot(PlayerInfoModel activePlayer)
        {
            Console.Write($"{activePlayer.UserName }, Please enter your shot selection ");
            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            var currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();

                    currentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" { gridSpot.SpotLetter }{ gridSpot.SpotNumber } ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created By - Fahad");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string title)
        {
            Console.WriteLine($"Player Information for {title}");

            var output = new PlayerInfoModel {UserName = AskForUserName()};

            GameLogic.InitializeGrid(output);
            PlaceShips(output);

            Console.Clear();

            return output;
        }

        private static string AskForUserName()
        {
            Console.WriteLine("What is your name?");
            var output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.WriteLine($"Where do you want to place your ship{model.ShipLocations.Count + 1} : ");
                var location = Console.ReadLine();

                var isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);
                }
                catch (Exception ex)
                {
                    isValidLocation = false;
                }

                if (!isValidLocation)
                {
                    Console.WriteLine("That isn't a valid location. Try again.");
                }


            } while (model.ShipLocations.Count < 5);
        }
    }
}
