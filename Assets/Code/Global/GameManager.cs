using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using ManicMiner.Utility;

namespace ManicMiner.Global
{
    public static class GameManager
    {
        // Game Score points.
        public const int GameScoreForPickup = 100;
        public const int GameScoreForAirBubble = 18;
        public const int GameScoreForNewLife = 10000;

        // Game state.
        public static int GameHighScore = 0;

        public static bool IsMusicOff = false;
        public static bool IsGamePaused = false;

        // Game start settings.
        public const int GameStartPlayerLives = 3;
        public const int GameNumberOfLevels = 20;

        // Current Game.
        public static int CurrentPlayerLives = 3;
        public static int CurrentScore = 0;
        public static int CurrentLevel = 0;

        // Level start settings.
        // 18 ticks per pixel or air: 8 pixel per char, 28 chars in total = 224 pixels of air. End of the level: 1 tick per pixel.
        public const int GameMaxLevelAirBubbles = 224;

        // Current Level states.
        public static int CurrentAirBubbles;
        public static int CurrentPickupsToCollect = 0;
        public static int CurrentLeversToBeActivated = 0;

        public static bool ThreadmillIsRotatingRight;

        public static bool ElevatorIsActive = false;
        public static bool ElevatorCounting = false;

        public static bool LevelCompleted = false;
        public static bool PlayerIsDead = false;

        // Game and Level run methods. -1: Death level, 0: Menu, 1: Central Cavern...
        public static void RunLevel(int level)
        {
            // 
            CurrentLevel = level;
            SceneManager.LoadScene(CurrentLevel);
        }

        public static void RunMenu()
        {
            RunLevel(0);
        }

        // Initial setup for game.
        public static void RunGame()
        {
            CurrentPlayerLives = GameStartPlayerLives;
            CurrentScore = 0;
            RunLevel(1);
        }

        public static void ReloadLevel()
        {
            RunLevel(CurrentLevel);
        }

        public static void RunNextLevel()
        {
            // -1: Death level, 0: Menu, 1: Central Cavern...
            CurrentLevel++;

            // Is the last level finished?
            if (CurrentLevel > GameNumberOfLevels)
            {
                // Yes, load menu.
                RunMenu();
            }
            else
            {
                // No, load next level.
                RunLevel(CurrentLevel);
            }
        }

        public static void RegisterLever()
        {
            // Register lever.
            CurrentLeversToBeActivated++;
        }

        public static void LeverActivated()
        {
            // One lever less.
            CurrentLeversToBeActivated--;

            // Did we collect all pickups for the level and activated all levers?
            if (CurrentPickupsToCollect == 0 && CurrentLeversToBeActivated == 0)
            {
                // Yes.
                ElevatorIsActive = true;
            }
        }

        public static void RegisterPickup()
        {
            // Register pickup for collection.
            CurrentPickupsToCollect++;
        }

        public static void PickupCollected()
        {
            // One pickup less.
            CurrentPickupsToCollect--;

            // Add points for collected pickup.
            AddScore(GameScoreForPickup);

            // Did we collect all pickups for the level and activated all levers?
            if (CurrentPickupsToCollect == 0 && CurrentLeversToBeActivated == 0)
            {
                // Yes.
                ElevatorIsActive = true;
            }
        }

        public static void AddAirScore()
        {
            AddScore(GameScoreForAirBubble);
        }

        // Calculator of Score, and extra lives.
        private static void AddScore(int points)
        {
            int oldScore = CurrentScore;
            int newScore = CurrentScore + points;

            // Set current score.
            CurrentScore = newScore;

            // Is it new high score?
            if (newScore > GameHighScore)
            {
                // Yes.
                GameHighScore = newScore;
            }

            // Did score just passed any 10000 points increment (10000, 20000, 30000...)?
            if (Math.Floor((double)newScore / GameScoreForNewLife) - Math.Floor((double)oldScore / GameScoreForNewLife) == 1)
            {
                // Yes, increase number of lives.
                CurrentPlayerLives++;
            }
        }
    } // Class end.
}
