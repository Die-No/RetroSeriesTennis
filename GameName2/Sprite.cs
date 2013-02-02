using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroSeriesTennis
{
    class Sprite
    {
        public Texture2D Texture;
        public Vector2 Position;

        public void Draw(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(Texture, Position, Color.White);
        }

        public virtual void Move(Vector2 Length)
        {
            Position += Length;
        }

        public Rectangle HitBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            }
        }

        public static bool CheckPadlleHitsBall(Paddle paddle, Ball ball)
        {
            if (paddle.HitBox.Intersects(ball.HitBox))
            {
                return true;
            }
            else
                return false;
            }
        
    }
}