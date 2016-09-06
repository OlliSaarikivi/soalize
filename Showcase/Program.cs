using Soalize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase
{
    sealed class Entity
    {
        [Column]
        public int Id { get; private set; }
        
        int hitpoints = 100;
        int strength = 20;

        public Entity() : this(0) { }

        public Entity(int id)
        {
            Id = id;
        }

        public void Damage(int damage)
        {
            hitpoints = Math.Max(0, hitpoints - damage);
        }

        public bool IsDead() => hitpoints <= 0;
    }

    class Program
    {
        static ValueArray<Entity> heroes;

        static ValueArray<Entity> monsters { get; set; }

        static ValueArray<Entity> heroesAlias
        {
            get
            {
                return heroes;
            }
        }

        static void Main(string[] args)
        {
            heroes = new ValueArray<Entity>(128);
            heroes[0] = new Entity(10);
            heroes[0].Damage(50);

            foreach (var entity in heroes)
            {
                entity.Damage(25);
            }
        }
    }
}
