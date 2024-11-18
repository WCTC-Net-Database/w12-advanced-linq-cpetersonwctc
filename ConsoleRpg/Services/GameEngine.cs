using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpg.Services;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;

    private IPlayer _player;
    private IMonster _goblin;

    public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager)
    {
        _menuManager = menuManager;
        _outputManager = outputManager;
        _context = context;
    }

    public void Run()
    {
        if (_menuManager.ShowMainMenu())
        {
            SetupGame();
        }
    }

    private void GameLoop()
    {
        _outputManager.Clear();

        while (true)
        {
            _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Attack");
            _outputManager.WriteLine("2. Sort items");
            _outputManager.WriteLine("3. Add item to inventory");
            _outputManager.WriteLine("4. Remove item from inventory");
            _outputManager.WriteLine("5. View inventory");
            _outputManager.WriteLine("6. Equip item");
            _outputManager.WriteLine("7. Quit");

            _outputManager.Display();

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AttackCharacter();
                    break;
                case "2":

                    _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
                    _outputManager.WriteLine("1. Search by name");
                    _outputManager.WriteLine("2. Sort by type");
                    _outputManager.WriteLine("3. Sort items by Name/Attack/Defense");
                    _outputManager.WriteLine("4. Quit");

                    _outputManager.Display();

                    input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            SearchInventory();
                            break;
                        case "2":
                            ListByType();
                            break;
                        case "3":
                            SortInventory();
                            break;
                        case "4":
                            break;
                    }
                    break;
                case "3":
                    AddItemToInventory();
                    break;
                case "4":
                    RemoveItemFromInventory();
                    break;
                case "5":
                    ViewInventory();
                    break;
                case "6":
                    EquipItem();
                    break;
                case "7":
                    _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                    _outputManager.Display();
                    Environment.Exit(0);
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection. Please choose 1.", ConsoleColor.Red);
                    break;
            }
        }
    }

    public void SearchInventory()
    {

        _outputManager.Write("What do you want to search?: ");

        _outputManager.Display();

        var input = Console.ReadLine();

        var items = _context.Items.Where(i => i.Name == input);
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Name} is a {item.Type}");
        }
    }

    public void ListByType()
    {
        _outputManager.WriteLine("\nHow do you want to search?: ");
        _outputManager.WriteLine("1. Weapons");
        _outputManager.WriteLine("2. Armors");
        _outputManager.WriteLine("3. Potions");
        _outputManager.WriteLine("4. Accessories");

        _outputManager.Display();

        var input = Console.ReadLine();
        var chosenType = "";
        switch (input)
        {
            case "1":
                chosenType = "Weapon";
                break;
            case "2":
                chosenType = "Armor";
                break;
            case "3":
                chosenType = "Potion";
                break;
            case "4":
                chosenType = "Accessory";
                break;
            default:
                _outputManager.WriteLine("Invalid selection");
                break;
        }



        var itemsByType = _context.Items
            .GroupBy(i => i.Type).Select(g => new { Type = g.Key, Count = g.Count() })
            .ToList();
            
        foreach (var item in itemsByType)
        {
            if (item.Type == chosenType)
            {
                Console.WriteLine($"{item.Type} has {item.Count} items");
            }
        }

        var items = _context.Items.Where(i => i.Type == chosenType);
        Console.WriteLine($"\n");
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Name}");
        }
    }

    public void SortInventory()
    {
        _outputManager.WriteLine("\nHow do you want to search?: ");
        _outputManager.WriteLine("1. Name");
        _outputManager.WriteLine("2. Attack Damage");
        _outputManager.WriteLine("3. Defense");

        _outputManager.Display();

        var input = Console.ReadLine();
        var items = _context.Items.OrderBy(i => i.Id);
        switch (input)
        {
            case "1":
                items = _context.Items.OrderBy(i => i.Name) ;
                break;
            case "2":
                items = _context.Items.OrderByDescending(i => i.Attack);
                break;
            case "3":
                items = _context.Items.OrderByDescending(i => i.Defense);
                break;
            default:
                _outputManager.WriteLine("Invalid selection");
                break;
        }

  
        foreach (var item in items)
        {
            Console.WriteLine($"{item.Name}");
        }
    }

    public void AddItemToInventory()
    {
        if (_player is Player player) {
            player.AddItemToInventory(_context); 
        }
    }

    public void RemoveItemFromInventory()
    {
        if (_player is Player player) { player.RemoveItemFromInventory(); }
    }

    public void ViewInventory()
    {
        if (_player is Player player) { player.ViewInventory(); }
    }

    public void EquipItem()
    {
        if (_player is Player player) { player.EquipItem(); }
    }
    private void AttackCharacter()
    {
        if (_goblin is ITargetable targetableGoblin)
        {
            _player.Attack(targetableGoblin);
            _player.UseAbility(_player.Abilities.First(), targetableGoblin);
        }
    }

    private void SetupGame()
    {
        _player = _context.Players.FirstOrDefault();
        _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);

        // Load monsters into random rooms 
        LoadMonsters();

        // Pause before starting the game loop
        Thread.Sleep(500);
        GameLoop();
    }

    private void LoadMonsters()
    {
        _goblin = _context.Monsters.OfType<Goblin>().FirstOrDefault();
    }

}
