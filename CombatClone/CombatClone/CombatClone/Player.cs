﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CombatClone
{
    class Player : GameObject
    {
        GamePadState gamePad;
        GamePadState prevGamePad;

        Vector2 crossHair;

        public int Score { get; set; }

        float friction;
        float turretRotation;

        short fireRate;
        short explosionDelay;
        public short CurrentAmmo { get; set; }

        public byte GunType { get; set; }

        public sbyte Hp { get; set; }
        sbyte maxHp;

        bool inputActive;
        bool disabeld;
        bool deadCrew;
        bool invisible = true;

        public Player()
        {
            Random random = new Random();

            Pos = new Vector2(320, 240);

            friction = 0.95f;
            maxHp = 3;
            Hp = maxHp;

            SpriteCoords = new Point(1, 1);
            Size = new Point(32, 32);
            Orgin = new Vector2(16, 16);

            Color = new Color(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255));
            Scale = 1;

            GunType = 3;
            inputActive = true;

            Z = 0.9f;
        }

        public short MaxFireRate
        {
            get
            {
                short maxFireRate = 0;

                switch (GunType)
                {
                    case 0:
                        maxFireRate = 32;
                        break;
                    case 1:
                        maxFireRate = 48;
                        break;
                    case 2:
                        maxFireRate = 8;
                        break;
                    case 3:
                        maxFireRate = 64;
                        break;
                }

                return maxFireRate;
            }
        }

        public short GetMaxAmmo(byte type)
        {
            short maxAmmo = 0;

            switch (type)
            {
                case 1:
                    maxAmmo = 30;
                    break;
                case 2:
                    maxAmmo = 100;
                    break;
                case 3:
                    maxAmmo = 10;
                    break;
            }

            return maxAmmo;
        }

        public void Movment()
        {
            Speed *= friction;

            AngleMath();

            Pos += Velocity;
        }

        public void Input()
        {
            Random random = new Random();

            if (gamePad.ThumbSticks.Right.Y >= 0.2f || gamePad.ThumbSticks.Right.Y <= -0.2 || gamePad.ThumbSticks.Right.X >= 0.2f || gamePad.ThumbSticks.Right.X <= -0.2)
                turretRotation = (float)Math.Atan2(-gamePad.ThumbSticks.Right.Y, gamePad.ThumbSticks.Right.X);

            if (gamePad.ThumbSticks.Left.X >= 0.45f) Angle += 3;
            if (gamePad.ThumbSticks.Left.X <= -0.45f) Angle -= 3;

            if (gamePad.Triggers.Right >= 0.5f) Speed += 0.2f;
            if (gamePad.Triggers.Left >= 0.5f) Speed -= 0.1f;

            if ((gamePad.ThumbSticks.Right.X >= 0.7f || gamePad.ThumbSticks.Right.X <= -0.7f || gamePad.ThumbSticks.Right.Y >= 0.7f || gamePad.ThumbSticks.Right.Y <= -0.7f) && fireRate <= 0)
            {
                if (GunType == 0)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(turretRotation) * 20, (float)Math.Sin(turretRotation) * 20), Globals.RadianToDegrees(turretRotation)+random.Next(-8, 9), (float)Math.Abs(Speed) + 10, 0, 1, false));
                }
                if (GunType == 1)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(turretRotation) * 20, (float)Math.Sin(turretRotation) * 20), Globals.RadianToDegrees(turretRotation) + i*-8, (float)Math.Abs(Speed) + 10, 0, 1, false));
                    }
                }
                if (GunType == 2)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(turretRotation) * 20, (float)Math.Sin(turretRotation) * 20), Globals.RadianToDegrees(turretRotation) + random.Next(-16, 17), (float)Math.Abs(Speed) + 10, 0, 1, false));
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(turretRotation) * 20, (float)Math.Sin(turretRotation) * 20), Globals.RadianToDegrees(turretRotation) + random.Next(-32, 33), (float)Math.Abs(Speed) + 10 + random.Next(-3, 6), 1, 1, false));
                }
                if (GunType == 3)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(turretRotation) * 20, (float)Math.Sin(turretRotation) * 20), Globals.RadianToDegrees(turretRotation) + random.Next(-8, 9), (float)Math.Abs(Speed) + 1, 4, 1, false));
                }
                fireRate = 1;
            }
        }

        public void UpdateHealth()
        {
            foreach (Projectile p in GameObjectManager.gameObjects.Where(item => item is Projectile))
            {
                if (p.Hitbox.Intersects(Hitbox) && p.enemy)
                {
                    Hp -= (sbyte)p.Damege;
                    p.destroy = true;
                }
            }

            if (Hp <= 0)
            {
                inputActive = false;
                explosionDelay += 1;
                if (explosionDelay >= 8*4 + 24)
                {
                    GameObjectManager.Add(new Expolsion(Pos - new Vector2(32, 32), 65, false));
                    explosionDelay = 0;
                }
            }
        }

        public override void Update()
        {
            prevGamePad = gamePad;
            gamePad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            if (fireRate == 2 && GunType != 0)
            {
                CurrentAmmo -= 1;
            }

            if (CurrentAmmo <= 0)
            {
                GunType = 0;
            }

            if(!invisible) UpdateHealth();

            if (inputActive) Input();
            Movment();

            if (fireRate >= 1)
            {
                fireRate += 1;
                if (fireRate >= MaxFireRate) fireRate = 0;
            }

            crossHair = new Vector2(Globals.Lerp(crossHair.X, ((float)Math.Cos(turretRotation) * 100), 0.1f), Globals.Lerp(crossHair.Y, ((float)Math.Sin(turretRotation) * 100), 0.1f));

            Rotation = Angle;

            base.Update();
        }

        public override void DrawSprite(SpriteBatch spriteBatch)
        {
            if (Hp >= 1)
            {
                spriteBatch.Draw(AssetManager.spritesheet, Pos, new Rectangle(34, 1, 28, 20), Color, turretRotation, new Vector2(9.5f, 10), 1, SpriteEffects.None, 0.99f);
                spriteBatch.Draw(AssetManager.spritesheet, Pos + crossHair, new Rectangle(100, 1, 16, 16), Color.White, Speed, new Vector2(8, 8), 1, SpriteEffects.None, 1);
            }
            base.DrawSprite(spriteBatch);
        }
    }
}
