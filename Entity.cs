using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;





namespace neonShooter
{
    abstract class Entity
    {
        protected Texture2D image;
        // The tint of the image. This will also allow us to change the transparency.
        protected Color color = Color.White;

        public Vector2 Position, Velocity;
        public float Orientation;
        public float Radius = 20;   // used for circular collision detection
        public bool IsExpired;      // true if the entity was destroyed and should be deleted.

        public Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
        }
    }



















    static class EntityManager
    {
        static List<Entity> entities = new List<Entity>();

        static bool isUpdating;
        static List<Entity> addedEntities = new List<Entity>();

        static List<Enemy> enemies = new List<Enemy>();
        static List<Bullet> bullets = new List<Bullet>();
        static public List<BlackHole> blackHoles = new List<BlackHole>();


        public static int Count { get { return entities.Count; } }

        public static void Add(Entity entity)
        {
            if (!isUpdating)
            {
                entities.Add(entity);
            }
            else
            {
                addedEntities.Add(entity);
            }

            if (entity is Bullet)
                bullets.Add(entity as Bullet);
            else if (entity is Enemy)
                enemies.Add(entity as Enemy);
            else if (entity is BlackHole)
                blackHoles.Add(entity as BlackHole);
        }


        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }


        static void HandleCollisions()
        {
            // handle collisions between enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    if (IsColliding(enemies[i], enemies[j]))
                    {
                        enemies[i].HandleCollision(enemies[j]);
                        enemies[j].HandleCollision(enemies[i]);
                    }
                }

            // handle collisions between bullets and enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(enemies[i], bullets[j]))
                    {
                        enemies[i].WasShot();
                        bullets[j].IsExpired = true;
                    }
                }

            // handle collisions between the player and enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
                {
                    PlayerShip.Instance.Kill();
                    enemies.ForEach(x => x.WasShot());
                    blackHoles.ForEach(x => x.Kill());

                    break;
                }
            }


            // handle collisions with black holes
            for (int i = 0; i < blackHoles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                    if (enemies[j].IsActive && IsColliding(blackHoles[i], enemies[j]))
                        enemies[j].WasShot();

                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(blackHoles[i], bullets[j]))
                    {
                        bullets[j].IsExpired = true;
                        blackHoles[i].WasShot();
                        
                    }
                }

                if (IsColliding(PlayerShip.Instance, blackHoles[i]))
                {
                    PlayerShip.Instance.Kill();
                    enemies.ForEach(x => x.WasShot());
                    blackHoles.ForEach(x => x.Kill());
                    break;
                }
            }
        }


        public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
        {
            return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
        }


        public static void Update()
        {
            isUpdating = true;

            HandleCollisions();

            foreach (var entity in entities)
                entity.Update();

            isUpdating = false;

            foreach (var entity in addedEntities)
                entities.Add(entity);

            addedEntities.Clear();

            // remove any expired entities.
            entities = entities.Where(x => !x.IsExpired).ToList();

            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
            blackHoles = blackHoles.Where(x => !x.IsExpired).ToList();



        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }


    }

}
