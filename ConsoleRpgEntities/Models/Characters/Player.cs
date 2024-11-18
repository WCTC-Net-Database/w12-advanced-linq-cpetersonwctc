using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using ConsoleRpgEntities.Models.Equipments;
using Microsoft.EntityFrameworkCore;
using ConsoleRpgEntities.Data;

namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : ITargetable, IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }

        // Foreign key
        public int? EquipmentId { get; set; }

        // Navigation properties
        public virtual Inventory Inventory { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }

        public void Attack(ITargetable target)
        {
            // Player-specific attack logic
            Console.WriteLine($"{Name} attacks {target.Name} with a {Equipment.Weapon.Name} dealing {Equipment.Weapon.Attack} damage!");
            target.Health -= Equipment.Weapon.Attack;
            System.Console.WriteLine($"{target.Name} has {target.Health} health remaining.");

        }

        public void UseAbility(IAbility ability, ITargetable target)
        {
            if (Abilities.Contains(ability))
            {
                ability.Activate(this, target);
            }
            else
            {
                Console.WriteLine($"{Name} does not have the ability {ability.Name}!");
            }
        }

        public void AddItemToInventory(GameContext context)
        {
            Console.Write("What item will be added to inventory? (name of any item) : ");
            var input = Console.ReadLine();
            var item = context.Items.FirstOrDefault(i => i.Name == input);
            if (item != null)
            {
                Inventory.Items.Add(item);
                Console.WriteLine($"{item.Name} has been added to inventory.");
            }
            else
            {
                Console.WriteLine($"'{input}' could not be found");
            }
        }

        public void RemoveItemFromInventory()
        {
            Console.WriteLine("What item will be removed from inventory?: ");

            for (int i=0; i<Inventory.Items.Count; i++)
            {
                Console.WriteLine($"{i+1}. {Inventory.Items.ElementAt(i).Name}");
            }

            var input = Convert.ToInt32(Console.ReadLine());

            if (input > 0 && input < Inventory.Items.Count + 1) {
                Inventory.Items.Remove(Inventory.Items.ElementAt(input-1));
            }
        }

        public void ViewInventory()
        {
            Console.WriteLine("You have: ");
            foreach (Item item in Inventory.Items)
            {
                Console.WriteLine(item.Name);
            }
        }

        public void EquipItem()
        {
            Console.WriteLine("What item will be equipped?: ");

            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Inventory.Items.ElementAt(i).Name}");
            }

            try
            {
                var input = Convert.ToInt32(Console.ReadLine());

                if (input > 0 && input < Inventory.Items.Count + 1)
                {
                    var item = Inventory.Items.ElementAt(input - 1);
                    if (item != null && item.Type == "Weapon")
                    {
                        Equipment.Weapon = item;
                    }
                    if (item != null && item.Type == "Armor")
                    {
                        Equipment.Armor = item;
                    }
                    Console.WriteLine($"{item.Name} has been equipped");
                }
            }
            catch (Exception ex) {
                Console.WriteLine("You probably need to add an item to the inventory first!");
            }
        }
    }
}
