using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



using System;

namespace neonShooter
{
    class PlayerShip : Entity
    {
        private static PlayerShip instance;

        const int cooldownFrames = 6;
        int cooldownRemaining = 0;
        static Random rand = new Random();



        public static PlayerShip Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerShip();

                return instance;
            }
        }

        private PlayerShip()
        {
            image = Art.Player;
            Position = Game1.ScreenSize / 2;
            Radius = 10;
        }

        int framesUntilRespawn = 0;
        public bool IsDead { get { return framesUntilRespawn > 0; } }


        public override void Update()
        {
            if (IsDead)
            {
                framesUntilRespawn--;
                return;
            }


            const float speed = 8;
            Velocity = speed * Input.GetMovementDirection();
            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, Game1.ScreenSize - Size / 2);

            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            var aim = Input.GetAimDirection();
            if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
            {
                cooldownRemaining = cooldownFrames;
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle); //Y, X, Z axis

                float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);
                //11f = bullet speed  | une coordonné polair, est désigné par le fait que n'importe quel point se fait avec un angle et une distance

                Vector2 offset = Vector2.Transform(new Vector2(25, -8), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                offset = Vector2.Transform(new Vector2(25, 8), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);

                
            }

            if (cooldownRemaining > 0)
                cooldownRemaining--;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
                base.Draw(spriteBatch);
        }

        public void Kill()
        {
            framesUntilRespawn = 60;
        }

    }
}