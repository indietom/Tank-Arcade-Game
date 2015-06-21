﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CombatClone
{
    class Gui
    {
        Vector2 topBar;

        float displayScore;
        float displayAmmo;

        public void Update()
        {

        }

        public void DrawPlayerUi(SpriteBatch spriteBatch)
        {
            foreach (Player p in GameObjectManager.gameObjects.Where(item => item is Player))
            {
                displayScore = Globals.Lerp(displayScore, p.Score, 0.1f);
                displayAmmo = Globals.Lerp(displayAmmo, p.CurrentAmmo, 0.05f);

                for (int i = 0; i < p.Hp; i++)
                {
                    spriteBatch.Draw(AssetManager.spritesheet, topBar + new Vector2(10 + i*32, 10), new Rectangle(133, 1, 24, 24), Color.White);
                }
                spriteBatch.DrawString(AssetManager.bigFont, "SCORE: " + Convert.ToInt32(displayScore).ToString().PadLeft(8, '0'), topBar + new Vector2(10, 48), Color.LightGreen);
                if (p.GunType != 0)
                {
                    spriteBatch.DrawString(AssetManager.bigFont, "AMMO: " + Convert.ToInt32(displayAmmo).ToString() + "/" + p.GetMaxAmmo(p.GunType), topBar + new Vector2(10, 70), Color.Gold);
                }
            }
        }

        public void DrawGameOverUi(SpriteBatch spriteBatch)
        {
            DrawText(spriteBatch, AssetManager.bigFont, new Vector2(320, 100), "& - GAME OVER - &", Color.White, true);
            foreach (Player p in GameObjectManager.gameObjects.Where(item => item is Player))
            {
                DrawText(spriteBatch, AssetManager.bigFont, new Vector2(320, 148), "FINAL SCORE: " + p.Score.ToString().PadLeft(8, '0'), Color.Gold, true);
            }

            DrawText(spriteBatch, AssetManager.bigFont, new Vector2(320, 148 + 48), "HIGHSCORE: ", Color.LightBlue, true);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Globals.paused)
            {
                if (!Globals.gameOver)
                    DrawPlayerUi(spriteBatch);
            }
            else
            {
                DrawText(spriteBatch, AssetManager.bigFont, new Vector2(320, 240), "- PAUSED -", Color.Gold, true);
            }

            if (Globals.gameOver) DrawGameOverUi(spriteBatch);
            if (Globals.startScreen) spriteBatch.Draw(AssetManager.startScreen, Vector2.Zero, Color.White);
        }

        public void DrawText(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos, string text, Color color, bool center)
        {
            Vector2 orgin = (center) ? new Vector2(font.MeasureString(text).X / 2, font.MeasureString(text).Y / 2) : Vector2.Zero;

            spriteBatch.DrawString(font, text, pos, color, 0, orgin, 1, SpriteEffects.None, 0);
        }
    }
}
