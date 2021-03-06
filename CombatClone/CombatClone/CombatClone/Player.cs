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

        Color orginalColor;
        Color shieldColor;

        public int Score { get; set; }

        float friction;
        public float TurretRotation { get; private set; }

        short fireRate;
        short explosionDelay;
        public short CurrentAmmo { get; set; }
        public short InvisibleCount { private get; set; }
        short maxInvisibleCount;
        short hurtCount;
        short maxHurtCount;

        public byte GunType { get; set; }

        public sbyte Hp { get; set; }
        sbyte maxHp;

        bool inputActive;
        public bool invisible;

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
            orginalColor = Color;
            Scale = 1;

            maxInvisibleCount = 128 * 3;

            maxHurtCount = 32;

            //GunType = 5;
            //CurrentAmmo = GetMaxAmmo(5);

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
                    case 4:
                        maxFireRate = 16;
                        break;
                    case 5:
                        maxFireRate = 4;
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
                case 4:
                    maxAmmo = 50;
                    break;
                case 5:
                    maxAmmo = 1000;
                    break;
            }

            return maxAmmo;
        }

        public void Movment()
        {
            Speed *= friction;

            AngleMath();

            Pos += Velocity;

            if ((float)Math.Abs(Speed) >= 0.5f)
            {
                GameObjectManager.Add(new Particle(Pos, new Point(1, 32), new Point(34, 34), Angle, 0, 0, 0, 0));
            }

            if (Pos.X >= 640 - 16 || Pos.X <= 16 || Pos.Y >= 480 - 16 || Pos.Y <= 16)
            {
                Speed = 0;
                Pos -= Velocity;
            }
        }

        public void Input()
        {
            Random random = new Random();

            if (gamePad.ThumbSticks.Right.Y >= 0.2f || gamePad.ThumbSticks.Right.Y <= -0.2 || gamePad.ThumbSticks.Right.X >= 0.2f || gamePad.ThumbSticks.Right.X <= -0.2)
                TurretRotation = (float)Math.Atan2(-gamePad.ThumbSticks.Right.Y, gamePad.ThumbSticks.Right.X);

            if (gamePad.ThumbSticks.Left.X >= 0.45f) Angle += 3;
            if (gamePad.ThumbSticks.Left.X <= -0.45f) Angle -= 3;

            if (gamePad.Triggers.Right >= 0.5f) Speed += 0.2f;
            if (gamePad.Triggers.Left >= 0.5f) Speed -= 0.1f;

            if ((gamePad.ThumbSticks.Right.X >= 0.7f || gamePad.ThumbSticks.Right.X <= -0.7f || gamePad.ThumbSticks.Right.Y >= 0.7f || gamePad.ThumbSticks.Right.Y <= -0.7f) && fireRate <= 0)
            {
                if (GunType == 0)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation)+random.Next(-8, 9), (float)Math.Abs(Speed) + 10, 0, 1, false));
                }
                if (GunType == 1)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation) + i*-8, (float)Math.Abs(Speed) + 10, 0, 1, false));
                    }
                }
                if (GunType == 2)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation) + random.Next(-16, 17), (float)Math.Abs(Speed) + 10, 0, 1, false));
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation) + random.Next(-32, 33), (float)Math.Abs(Speed) + 10 + random.Next(-3, 6), 1, 1, false));
                }
                if (GunType == 3)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation) + random.Next(-8, 9), (float)Math.Abs(Speed) + 1, 4, 1, false));
                }
                if (GunType == 4)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation) + random.Next(-8, 9), (float)Math.Abs(Speed) + 10, 5, 1, false));
                }
                if (GunType == 5)
                {
                    GameObjectManager.Add(new Projectile(Pos + new Vector2((float)Math.Cos(TurretRotation) * 20, (float)Math.Sin(TurretRotation) * 20), Globals.RadianToDegrees(TurretRotation) + random.Next(-16, 17), (float)Math.Abs(Speed) + random.Next(3, 6), 6, 1, false));
                }
                fireRate = 1;
            }
        }

        public void UpdateHealth()
        {
            if (hurtCount >= 1)
            {
                Color = Color.Red;

                hurtCount += 1;
                if (hurtCount >= maxHurtCount) hurtCount = 0;
            }
            else
            {
                Color = orginalColor;
            }

            foreach (Projectile p in GameObjectManager.gameObjects.Where(item => item is Projectile))
            {
                if (p.Hitbox.Intersects(Hitbox) && p.enemy && hurtCount <= 0)
                {
                    hurtCount = 1;
                    PlayHitSound();
                    Hp -= (sbyte)p.Damege;
                    p.destroy = true;
                }
            }

            foreach (Enemy e in GameObjectManager.gameObjects.Where(item => item is Enemy))
            {
                if (!e.leaveCorpse && e.TouchedPlayer() && hurtCount == 0)
                {
                    // F
                    if (e.ToString().Split('.')[1] != "Helicopter")
                    {
                        hurtCount = 1;
                        PlayHitSound();
                        Hp -= 1;
                    }
                }
            }

            Globals.gameOver = (Hp <= 0);

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

        public void PlayHitSound()
        {
            if (Hp > 0) AssetManager.hitSound.Play();
        }

        public override void Update()
        {
            Random random = new Random();

            prevGamePad = gamePad;
            gamePad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            if (fireRate == 2 && GunType != 0)
            {
                CurrentAmmo -= 1;
            }

            if (fireRate == 2)
            {
                AssetManager.shootSound.Play();
            }

            if (CurrentAmmo <= 0)
            {
                GunType = 0;
            }

            if(!invisible) UpdateHealth();

            invisible = (InvisibleCount > 0);

            InvisibleCount = (InvisibleCount >= 1) ? (short)(InvisibleCount + 1) : InvisibleCount;
            InvisibleCount = (InvisibleCount >= maxInvisibleCount) ? (short)0 : InvisibleCount;

            if (InvisibleCount >= maxInvisibleCount - 128)
            {
                shieldColor = Color.Red;
            }
            else
            {
                shieldColor = Color.Blue;
            }

            if (inputActive) Input();
            Movment();

            if (fireRate >= 1)
            {
                fireRate += 1;
                if (fireRate >= MaxFireRate) fireRate = 0;
            }

            crossHair = new Vector2(Globals.Lerp(crossHair.X, ((float)Math.Cos(TurretRotation) * 100), 0.1f), Globals.Lerp(crossHair.Y, ((float)Math.Sin(TurretRotation) * 100), 0.1f));

            Rotation = Angle;

            base.Update();
        }

        public override void DrawSprite(SpriteBatch spriteBatch)
        {
            if (Hp >= 1)
            {
                spriteBatch.Draw(AssetManager.spritesheet, Pos + Globals.screenOffset, new Rectangle(34, 1, 28, 20), Color, TurretRotation, new Vector2(9.5f, 10), 1, SpriteEffects.None, 0.98f);
                spriteBatch.Draw(AssetManager.spritesheet, Pos + crossHair + Globals.screenOffset, new Rectangle(100, 1, 16, 16), Color.White, Speed, new Vector2(8, 8), 1, SpriteEffects.None, 1);
            }

            if (invisible)
            {
                for (int i = 0; i < 360; i++)
                {
                    spriteBatch.Draw(AssetManager.spritesheet, Pos + new Vector2((float)Math.Cos(i) * 32, (float)Math.Sin(i) * 32), new Rectangle(166, 1, 4, 4), shieldColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0.999f); 
                }
            }
            base.DrawSprite(spriteBatch);
        }
    }
}
