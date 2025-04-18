using System.Numerics;

namespace Text_Rpg
{
    public class GameLogic
    {
        private PlayerNameJob _playerNameJob;
        private PlayerStat _playerStat;
        private Inventory _inventory = new Inventory();
        private Shop _shop = new Shop();
        private Dungeon _dungeon =new Dungeon();
        private GameOver _gameOver = new GameOver();

        private bool _isGameOver = false;
        public void StartMenu()
        {
            Intro();
        }

        public void Intro()
        {
            Console.Clear();
            Console.WriteLine("스파르타 던전에 오신것을 환영합니다.\n이름을 입력하세요.");
            string? playerName = Console.ReadLine();

            if (string.IsNullOrEmpty(playerName))
            {
                Console.WriteLine("잘못된 이름입니다.");
                Thread.Sleep(1500); //잠시 기다리기
                Intro();
            }
            else
            {
                _playerNameJob = new PlayerNameJob(playerName);
                Console.WriteLine($"{_playerNameJob.Name}님, 환영합니다.");
            }



            while (true)
            {
                Console.WriteLine("직업을 선택하세요. [1:전사 | 2:법사 | 3:궁수 | 4:도적]");
                string? input = Console.ReadLine();

                try
                {
                    int job = int.Parse(input);

                    if (job >= 1 && job <= 4)
                    {
                        _playerNameJob.job = (Job)job;
                        Console.WriteLine($"{_playerNameJob.job}를 선택했습니다.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다. 1~4 중에서 다시 선택해주세요.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                }
                catch (Exception)
                {
                    Console.WriteLine("입력 처리 중 오류가 발생했습니다.");
                }

                Thread.Sleep(1500);
                Console.Clear();
            }

            Thread.Sleep(1500); //잠시 기다리기

            MainMenu();
        }

        public GameLogic()
        {
            _playerNameJob = new PlayerNameJob("직업");
            _playerNameJob.job = Job.Warrior;
            _playerStat = new PlayerStat(1, 10, 5, 100, 3000);
            _inventory = new Inventory();
        }

        

        public void StatusMenu()
        {
            Console.Clear ();
            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine();

            int addAttack = 0;
            int addDefend = 0;

            foreach (var item in _inventory.GetEquippedItems())
            {
                addAttack += item.Attack;
                addDefend += item.Defend;
            }


            Console.WriteLine($"Lv. {_playerStat.Level:00}");
            Console.WriteLine($"{_playerNameJob.Name} ({_playerNameJob.job.ToString()})");
            Console.WriteLine($"공격력 : {_playerStat.Attack} {(addAttack > 0 ? $"(+{addAttack})" : "")}");
            Console.WriteLine($"방어력 : {_playerStat.Defend} {(addDefend > 0 ? $"(+{addDefend})" : "")}");
            Console.WriteLine($"체 력 : {_playerStat.Hp}");
            Console.WriteLine($"Gold : {_playerStat.Gold}");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");

            while (true)
            {
                Console.WriteLine("\n원하는 행동을 입력해주세요.\n>>");
                string? input = Console.ReadLine();

                if (input == "0")
                {
                    MainMenu();
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요");
                }
            }
        }

        

        public void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("5. 게임 종료");

            switch (UserInput(1, 5))
            {
                case 1:
                    StatusMenu();
                    break;
                case 2:
                    _inventory.InventoryMenu();
                    MainMenu();
                    break;
                case 3:
                    _shop.ShopMenu(_inventory, _playerStat);
                    MainMenu();
                    break;
                case 4:
                    _dungeon.DungeonMenu();
                    break;
                case 5:
                    _gameOver.GameOverMenu();
                    break;
            }


        }
        private int UserInput(int min, int max)
        {
            int numberInput;
            bool result;
            do
            {
                Console.WriteLine("원하는 활동을 선택해주세요.");
                numberInput = int.Parse(Console.ReadLine());
            }
            while (UserInputValidCheck(numberInput, min, max) == false);

            return numberInput;
        }

        private bool UserInputValidCheck(int numberInput, int min, int max)
        {
            if (min <= numberInput && numberInput <= max)
                return true;

            return false;
        }        
    }
    class PlayerNameJob
    {
        public string Name;
        public Job job;

        public PlayerNameJob(string name)
        {
            Name = name;
        }
    }

    class PlayerStat
    {
        public int Level;
        public int Attack;
        public int Defend;
        public int Hp;
        public int Gold;

        public PlayerStat(int level, int attack, int defend, int hp, int gold)
        {
            Level = level;
            Attack = attack;
            Defend = defend;
            Hp = hp;
            Gold = gold;
        }
    }
    public enum Job
    {
        Warrior = 1,
        Wizard,
        Archor,
        Rogue
    }


    class Inventory
    {

        private List<Item> _items = new List<Item>();
        public Inventory()
        {

        }
        private GameLogic _gameLogic;
        public Inventory(GameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }
        public void AddItem(Item item)
        {
            if (!item.IsPurchased)
            {
                item.IsPurchased = true;
            }
            _items.Add(item);
            Console.WriteLine($"{item.Name}을 인벤토리에 추가했습니다");
            Thread.Sleep(1500);
        }
        public void InventoryMenu()
        {
            Console.Clear();
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();

            Console.WriteLine($"[아이템 목록]");
            Console.WriteLine();

            if (_items.Count == 0)
            {
                Console.WriteLine("보유중인 아이템이 없습니다.");
            }
            else
            {
                foreach (var item in _items)
                {
                    string equipMark = item.IsEquipped ? "[E]" : "   ";
                    Console.WriteLine($"- {equipMark}{item.Name} | {item.GetStatText()} | {item.Description}");
                }
            }

            Console.WriteLine();

            if (_items.Count > 0)
            {
                Console.WriteLine("1. 장착 관리");
            }
            Console.WriteLine("0. 나가기");

            while (true)
            {
                Console.WriteLine("\n원하는 행동을 입력해주세요.\n>>");
                string? input = Console.ReadLine();

                if (input == "0")
                {
                    return;
                }
                else if (input == "1" && _items.Count > 0)
                {
                    EquipManager();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요");
                }
            }
        }

        private void EquipManager()
        {
            Console.Clear();
            Console.WriteLine("인벤토리 - 장착관리");
            Console.WriteLine("장착할 아이템 번호를 입력해주세요.");
            Console.WriteLine();

            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                string equipMark = item.IsEquipped ? "[E]" : "   ";
                Console.WriteLine($"{i + 1}. {equipMark}{item.Name} | {item.GetStatText()} | {item.Description}");
            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.Write("\n>>");
            string? input = Console.ReadLine();

            if (input == "0")
            {
                return;
            }

            try
            {
                int index = int.Parse(input);
                if (index < 1 || index > _items.Count)
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }
                else
                {
                    var selectedItem = _items[index - 1];

                    if (selectedItem.IsEquipped)
                    {
                        selectedItem.IsEquipped = false;
                        Console.WriteLine($"{selectedItem.Name}의 장착을 해제했습니다.");
                    }
                    else
                    {
                        // 같은 타입의 장비 장착 해제
                        foreach (var item in _items)
                        {
                            if (item.Type == selectedItem.Type && item.IsEquipped)
                            {
                                item.IsEquipped = false;
                            }
                        }

                        selectedItem.IsEquipped = true;
                        Console.WriteLine($"{selectedItem.Name}을(를) 장착했습니다.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }

            Thread.Sleep(1500);
            InventoryMenu();
        }

        public List<Item> GetEquippedItems()
        {
            return _items.Where(i => i.IsEquipped).ToList();
        }

    }

    enum ItemType
    {
        Weapon,
        Armor
    }

    class Item
    {
        

        public string Name { get; set; }
        public string Description { get; set; }
        public int Attack { get; set; }
        public int Defend { get; set;  }
        public int Price { get; set; }
        public ItemType Type { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsPurchased { get; set; }


        public Item(string name, string description, int attack, int defend, int price, ItemType type)
        {
            Name = name;
            Description = description;
            Type = type;
            Attack = attack;
            Defend = defend;
            Price = price;
            IsEquipped = false;
            IsPurchased = false;
        }

        public string GetStatText()
        {
            if (Type == ItemType.Weapon)
                return $"공격력 +{Attack}";
            else
                return $"방어력 +{Defend}";
        }
    }
    


    class Shop
    {
        private List<Item> _shopItems = new List<Item>();

        public Shop()
        {
            _shopItems.Add(new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 0, 5, 1000, ItemType.Armor));
            _shopItems.Add(new Item("무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 0, 9, 1200, ItemType.Armor));
            _shopItems.Add(new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 0, 15, 3500, ItemType.Armor));
            _shopItems.Add(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검 입니다.", 2, 0, 600, ItemType.Weapon));
            _shopItems.Add(new Item("청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", 5, 0, 1500, ItemType.Weapon));
            _shopItems.Add(new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 7, 0, 2500, ItemType.Weapon));
        }
        public void ShopMenu(Inventory inventory, PlayerStat stat)
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("상점");
                Console.WriteLine("필요한 아이템을 구매할 수 있는 상점입니다.");
                Console.WriteLine();
                Console.WriteLine("[보유 골드]");
                Console.WriteLine($"{stat.Gold} G");
                Console.WriteLine();
                Console.WriteLine("[아이템 목록]");

                for (int i = 0; i < _shopItems.Count; i++)
                {
                    var item = _shopItems[i];
                    string statText = item.GetStatText();
                    string priceText = item.IsPurchased ? "구매완료" : $"{item.Price} G";
                    Console.WriteLine($"- {item.Name} | {statText} | {item.Description} | {priceText}");
                }

                Console.WriteLine();
                Console.WriteLine("1. 아이템 구매");
                Console.WriteLine("0. 나가기");
                Console.Write("\n원하는 행동을 입력해주세요.\n>>");
                string? input = Console.ReadLine();

                if (input == "0")
                    break;
                else if (input == "1")
                    BuyItem(inventory, stat);
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    Thread.Sleep(1500);
                }
            }
        }

        private void BuyItem(Inventory inventory, PlayerStat stat)
        {
            Console.Clear();
            Console.WriteLine("구매할 아이템 번호를 입력해주세요.\n");

            for (int i = 0; i < _shopItems.Count; i++)
            {
                var item = _shopItems[i];
                string status = item.IsPurchased ? "(구매완료)" : $"[{item.Price} G]";
                Console.WriteLine($"{i + 1}. {item.Name} {status}");
            }

            Console.WriteLine("0. 취소");
            Console.Write("\n>> ");
            string? input = Console.ReadLine();

            if (input == "0") return;

            try
            {
                int index = int.Parse(input);

                if (index >= 1 && index <= _shopItems.Count)
                {
                    var item = _shopItems[index - 1];

                    if (item.IsPurchased)
                    {
                        Console.WriteLine("이미 구매한 아이템입니다.");
                    }
                    else if (stat.Gold < item.Price)
                    {
                        Console.WriteLine("Gold가 부족합니다.");
                    }
                    else
                    {
                        stat.Gold -= item.Price;
                        item.IsPurchased = true;
                        inventory.AddItem(item);
                    }
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }
            catch (Exception)
            {
                Console.WriteLine("입력 처리 중 오류가 발생했습니다.");
            }

            Thread.Sleep(1500);
        }
    }


    class Dungeon
    {
        public void DungeonMenu()
        { 
        
        }
    }

    class GameOver
    {
        public void GameOverMenu()
        {
            Console.WriteLine("게임을 종료합니다.");
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            GameLogic gameLogic = new GameLogic();
            gameLogic.Intro();

        }
        
        
    }
}