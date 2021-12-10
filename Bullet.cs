using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace neonShooter
{
    class Bullet : Entity
    {
        private static Random rand = new Random();



        public Bullet(Vector2 position, Vector2 velocity)
        {
            image = Art.Bullet;
            Position = position;
            Velocity = velocity;
            Orientation = Velocity.ToAngle();
            Radius = 8;
        }

        public override void Update()
        {
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            Position += Velocity;


            if (!Game1.Viewport.Bounds.Contains(Position.ToPoint()))
            {
                IsExpired = true;

                for (int i = 0; i < 30; i++)
                    Game1.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, Vector2.One,new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });

            }
        }
    }
}