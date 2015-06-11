﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CombatClone
{
    abstract class GameObject
    {
        public Vector2 Pos { get; set; }
        public Vector2 Orgin { get; set; }
        public Vector2 Velocity { get; set; }

        public Point Size { get; set; }
        public Point SpriteCoords { get; set; }

        public float Angle { get; set; }
        public float Speed { get; set; }
        public float Scale { get; set; }
        public float Z { get; set; }
        public float Rotation { get; set; }

        public Color Color { get; set; } 

        public bool destroy;

        public void AngleMath()
        {
            Velocity = new Vector2((float)Math.Cos(Globals.DegreesToRadian(Angle)) * Speed, (float)Math.Sin(Globals.DegreesToRadian(Angle)) * Speed);
        }

        public float GetAimAngle(Vector2 target)
        {
            return Globals.RadianToDegrees((float)Math.Atan2(Pos.Y - target.Y, Pos.X - target.X));
        }

        public float GetDistance(Vector2 target)
        {
            return (float)Math.Sqrt((Pos.X - target.X) * (Pos.X - target.X) + (Pos.Y  - target.Y) * (Pos.X - target.Y));  
        }

        public virtual void Update()
        {
            if (destroy) GameObjectManager.Remove(this);
        }

        public virtual void DrawSprite(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(AssetManager.spritesheet, Pos, new Rectangle(SpriteCoords.X, SpriteCoords.Y, Size.X, Size.Y), Color, Globals.DegreesToRadian(Rotation), Orgin, Scale, SpriteEffects.FlipVertically, Z); 
        }
    }
}
