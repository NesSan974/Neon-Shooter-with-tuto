using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace neonShooter
{
    class PlayerStatus
    {
        // amount of time it takes, in seconds, for a multiplier to expire.
        private const float multiplierExpiryTime = 0.8f;
        private const int maxMultiplier = 20;

        public static int Lives { get; private set; }
        public static int Score { get; private set; }
        public static int Multiplier { get; private set; }

        private static float multiplierTimeLeft;    // time until the current multiplier expires
        private static int scoreForExtraLife;       // score required to gain an extra life
        public static bool IsGameOver { get { return Lives <= 0; } }

        public static int HighScore { get; private set; }

        // Static constructor
        static PlayerStatus()
        {
            HighScore = LoadHighScore();
            Reset();
        }

        public static void Reset()
        {
            if (Score > HighScore)
                SaveHighScore(HighScore = Score);


            Score = 0;
            Multiplier = 1;
            Lives = 3;
            scoreForExtraLife = 2000;
            multiplierTimeLeft = 0;
        }

        public static void Update()
        {
            if (Multiplier > 1)
            {
                // update the multiplier timer
                if ((multiplierTimeLeft -= (float)Game1.gt.ElapsedGameTime.TotalSeconds) <= 0)
                {
                    multiplierTimeLeft = multiplierExpiryTime;
                    ResetMultiplier();
                }
            }
        }

        public static void AddPoints(int basePoints)
        {
            if (PlayerShip.Instance.IsDead)
                return;

            Score += basePoints * Multiplier;
            while (Score >= scoreForExtraLife)
            {
                scoreForExtraLife += 2000;
                Lives++;
            }
        }

        public static void IncreaseMultiplier()
        {
            if (PlayerShip.Instance.IsDead)
                return;

            multiplierTimeLeft = multiplierExpiryTime;
            if (Multiplier < maxMultiplier)
                Multiplier++;
        }

        public static void ResetMultiplier()
        {
            Multiplier = 1;
        }

        public static void RemoveLife()
        {
            Lives--;
        }

        private const string highScoreFilename = "highscore.txt";

        private static int LoadHighScore()
        {
            // return the saved high score if possible and return 0 otherwise
            int score;
            return File.Exists(highScoreFilename) && int.TryParse(File.ReadAllText(highScoreFilename), out score) ? score : 0;
        }

        private static void SaveHighScore(int score)
        {
            File.WriteAllText(highScoreFilename, score.ToString());
        }
    }
}