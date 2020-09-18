using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using ManicMiner.Utility;

namespace ManicMiner.Global
{
    public static class ResourceManager
    {
        // This will give us 50 fixed frames per second.
        public const float FixedDeltaTime = 0.02f;

        // Set fixed update. This should be for whole app, so it should be set in project settings, or somewhere elese.
        // Time.fixedDeltaTime = m_FiexedDeltaTime;

        // Dynamic resources for sprite animation.
        public static Sprite[] PlayerSprites = Resources.LoadAll<Sprite>("Textures/Player");
        public static Sprite[] ThreadmillSprites = Resources.LoadAll<Sprite>("Textures/Threadmills");
        public static Sprite[] MudSprites = Resources.LoadAll<Sprite>("Textures/Mud");
        public static Sprite[] LeverSprites = Resources.LoadAll<Sprite>("Textures/Levers");
        public static Sprite[] GuardiansSprites = Resources.LoadAll<Sprite>("Textures/Guardians");

        // Colors for pickup animation.
        public static readonly Color32[] PickupColors = { new Color32(0xcc, 0xcc, 0x00, 0xff), new Color32(0x00, 0xcc, 0xcc, 0xff), new Color32(0x00, 0xcc, 0x00, 0xff), new Color32(0xcc, 0x00, 0xcc, 0xff) };

        // Game animation ticks.
        public const int AnimateOnTicksForScroller = 1;
        public const int AnimateOnTicksForPlayer = 4;
        public const int AnimateOnTicksForAir = 36;
        public const int AnimateOnTicksForLives = 8;
        public const int AnimateOnTicksForThreadmill = 2;
        public const int AnimateOnTicksForMud = 4;
        public const int AnimateOnTicksForPickup = 4;
        public const int AnimateOnTicksForPickupLight = 1;
        public const int AnimateOnTicksForElevator = 16;
        public const int AnimateOnTicksForElevatorLight = 1;
        public const int AnimateOnTicksForDeath = 50;

        // Current buttons pressed or clicked. Defined in Project Settings, but does not gives enough precise results for game controls (pause, music, quit, game start).
        public static KeyController KeyForLeft = new KeyController(KeyCode.LeftArrow);
        public static KeyController KeyForRight = new KeyController(KeyCode.RightArrow);
        public static KeyController KeyForJump = new KeyController(KeyCode.Space);

        public static KeyController KeyForMusic = new KeyController(KeyCode.M);
        public static KeyController KeyForPause = new KeyController(KeyCode.P);
        public static KeyController KeyForGameStart = new KeyController(KeyCode.Return);
        public static KeyController KeyForQuit = new KeyController(KeyCode.Escape);

        // Input methods, should be called in monmobehaviour update methods.
        public static void CatchFreshGameControlButtonsPressed()
        {
            KeyForLeft.Refresh();
            KeyForRight.Refresh();
            KeyForJump.Refresh();

            KeyForMusic.Refresh();
            KeyForPause.Refresh();
            KeyForGameStart.Refresh();
            KeyForQuit.Refresh();
        }

        public static void ResetKeys()
        {
            KeyForLeft.Reset();
            KeyForRight.Reset();
            KeyForJump.Reset();

            KeyForMusic.Reset();
            KeyForPause.Reset();
            KeyForGameStart.Reset();
            KeyForQuit.Reset();
        }

        // Helper function to find index of sprites in sprites array.
        public static int FindSpriteIndex(Sprite[] sprites, Sprite sprite)
        {
            int spriteIndex = -1;
            string spriteName = sprite.name;

            // Go through all pickup colors.
            for (int count = 0; count < sprites.Length; count++)
            {
                // Is it the same as initial color?
                if (sprites[count].name == sprite.name)
                {
                    // Yes.
                    spriteIndex = count;
                    break;
                }
            }

            return spriteIndex;
        }
    } // Class end.
}
