using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic; //list
using System.Linq;
using System;





namespace neonShooter
{
    class Enemy : Entity
    {
        private int timeUntilStart = 60;

        Random rand = new Random();
        public bool IsActive { get { return timeUntilStart <= 0; } }


        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();




        public void HandleCollision(Enemy other)
        {
            var d = Position - other.Position;
            Velocity += 10 * d / (d.LengthSquared() + 1);
        }

        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            behaviours.Add(behaviour.GetEnumerator());
        }

        private void ApplyBehaviours()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }



        public Enemy(Texture2D image, Vector2 position)
        {
            this.image = image;
            Position = position;
            Radius = image.Width / 2f;
            color = Color.Transparent;
        }


        public static Enemy CreateWanderer(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehaviour(enemy.MoveRandomly());
            return enemy;
        }

        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(Art.Seeker, position);
            enemy.AddBehaviour(enemy.FollowPlayer());

            return enemy;
        }



        IEnumerable<int> FollowPlayer(float acceleration = 1f)
        {
            while (true)
            {
                Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();

                yield return 0;
            }
        }

        IEnumerable<int> MoveRandomly()
        {
            float direction = rand.NextFloat(0f, MathHelper.TwoPi);


            while (true)
            {
                direction += rand.NextFloat(-0.1f, 0.1f);
                direction = MathHelper.WrapAngle(direction);

                for (int i = 0; i < 6; i++)
                {
                    Velocity += MathUtil.FromPolar(direction, 0.4f);
                    Orientation -= 0.05f;

                    var bounds = Game1.Viewport.Bounds;
                    bounds.Inflate(-image.Width, -image.Height);

                    // if the enemy is outside the bounds, make it move away from the edge
                    if (!bounds.Contains(Position.ToPoint()))
                        direction = (Game1.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

                    yield return 0;
                }
            }
        }




        public override void Update()
        {
            if (timeUntilStart <= 0)
            {
                ApplyBehaviours();
            }
            else
            {
                timeUntilStart--;
                color = Color.White * (1 - timeUntilStart / 60f);
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, Game1.ScreenSize - Size / 2);

            Velocity *= 0.8f;
        }

        public void WasShot()
        {
            IsExpired = true;
            Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);

            float hue1 = rand.NextFloat(0, 6);
            float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;
            Color color1 = Extensions.HSVToColor(hue1, 0.5f, 1);
            Color color2 = Extensions.HSVToColor(hue2, 0.5f, 1);


            for (int i = 0; i < 120; i++)
            {
                float speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));


                // rand=2 | 90% du temps est superieur a la moitier (50%) de 18   90% -> 9 = 50% de 18
                //( 1/2=0,5 | donc pour rand compris entre 2 et 10, 1 - 1/rand est plus grand ou egal que 0.5 )

                // rand=3 | 80% -> 12.6 = 70% de 18
                // rand=4 | 70% -> 13.6 = 75.5% de 18


                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed), //dans NextVector2(), l'angle est full aleatoir,
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));
                Game1.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, new Vector2(1.5f), state);
            }
        }
    }







    static class EnemySpawner
    {
        static Random rand = new Random();
        static float inverseSpawnChance = 60;
        static float maxBlackHole = 2f;


        public static void Update()
        {
            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
            {
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));

                if (EntityManager.blackHoles.Count < (int)maxBlackHole && rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(new BlackHole(GetSpawnPosition()));

            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 20)
            {
                inverseSpawnChance -= 0.004f;
                maxBlackHole += 0.0006f;
            }

        }

        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos;
            do
            {
                pos = new Vector2(rand.Next((int)Game1.ScreenSize.X), rand.Next((int)Game1.ScreenSize.Y));
            }
            while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250); //vu u'on mesure la distance au carré, et qu'on veut 250, ba on met au carré

            return pos;
        }

        public static void Reset()
        {
            inverseSpawnChance = 60;
        }
    }


}