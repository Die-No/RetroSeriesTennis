using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RetroSeriesTennis
{
    class Ball: Sprite
    {
        public Vector2 Velocity;
        public Random Random;

        public Ball()
        {
            Random = new Random();
        }

        public void Serve(float Velocity)
        {
            Position = new Vector2(Game1.ScreenWidth / 2 - Texture.Width / 2, Game1.ScreenHeight / 2 - Texture.Height / 2);
            float angle = (float) ( Math.PI/2 + (Random.NextDouble() * (Math.PI/1.5f) - Math.PI/3));
            this.Velocity.X = (float) Math.Sin(angle);
            this.Velocity.Y = (float) Math.Cos(angle);

            if (Random.Next(2) == 1)
            {
                this.Velocity.X *= -1;
            }

            this.Velocity *= Velocity;
        }

        public void CheckWallCollision()
        {
            if (Position.Y < 0)
            {
                Position.Y = 0;
                Velocity.Y *= -1;
                Game1.BallCollisionBorderSound.Play();
            }

            if (Position.Y + Texture.Height > Game1.ScreenHeight)
            {
                Position.Y = Game1.ScreenHeight - Texture.Height;
                Velocity.Y *= -1;
                Game1.BallCollisionBorderSound.Play();
            }
        }

        public override void Move(Vector2 Length)
        {
            base.Move(Length);
            CheckWallCollision();
        }


    }
}
