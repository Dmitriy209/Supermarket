using System;
using System.Collections.Generic;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Supermarket supermarket = new Supermarket();
            supermarket.Work();
        }
    }

    class Supermarket
    {
        private int _money;

        private List<Product> _products;
        private Queue<Client> _clients = new Queue<Client>();

        public Supermarket()
        {
            _products = CreateProducts();
        }

        public void Work()
        {
            const string CommandAddClient = "1";
            const string CommandServe = "2";
            const string CommandExit = "exit";

            bool isRunnning = true;

            while (isRunnning)
            {
                GetNumbersClients();

                Console.WriteLine($"Баланс магазина - {_money}.");

                Console.WriteLine($"\nВведите {CommandAddClient}, чтобы добавить людей в очередь.\n" +
                    $"Введите {CommandServe}, чтобы обслужить клиента.\n" +
                    $"Введите {CommandExit}, чтобы выйти.");
                string userInput = Console.ReadLine();

                Console.Clear();

                switch (userInput)
                {
                    case CommandAddClient:
                        AddClients();
                        break;

                    case CommandServe:
                        Serve();
                        break;

                    case CommandExit:
                        isRunnning = false;
                        break;

                    default:
                        Console.WriteLine("Такой команды нет.");
                        break;
                }
            }
        }

        private void GetNumbersClients()
        {
            if (_clients.Count == 0)
                Console.WriteLine("Очередь пуста.");
            else
                Console.WriteLine($"Размер очереди: {_clients.Count}");
        }

        private void AddClients()
        {
            CreateClients(ReadNumberClients());
        }

        private void Serve()
        {
            int numberClients = ReadNumberClients();

            if (_clients.Count != 0 && _clients.Count >= numberClients)
            {
                for (int i = 0; i < numberClients; i++)
                {
                    Client client = _clients.Peek();

                    client.ShowBasket();

                    Console.WriteLine($"\nСумма покупок {client.GetSumBasket()}.\n" +
                        $"Баланс клиента {client.Money}\n");

                    while (TryPay(client) == false)
                        DeleteRandomProduct(client);

                    client.ShowBasket();

                    Sell(_clients.Dequeue().Pay());
                }
            }
        }

        private void Sell(int price)
        {
            _money += price;
        }

        private void DeleteRandomProduct(Client client)
        {
            Product product = client.Basket[UserUtils.GenerateRandomNumber(0, client.Basket.Count)];

            Console.WriteLine("Клиент убирает из корзины:");
            product.ShowStats();

            client.Basket.Remove(product);
        }

        private bool TryPay(Client client)
        {
            if (client.Money >= client.GetSumBasket())
            {
                Console.WriteLine("У клиента достаточно средств.");
                return true;
            }
            else
            {
                Console.WriteLine("У клиента недостаточно средств.");
                return false;
            }
        }

        private int ReadNumberClients()
        {
            int number;

            do
            {
                Console.WriteLine("Введите количество клиентов.");
                number = ReadInt();
            }
            while (number < 0);

            return number;
        }

        private int ReadInt()
        {
            int number;

            while (int.TryParse(Console.ReadLine(), out number) == false)
                Console.WriteLine("Это не число.");

            return number;
        }

        private List<Product> CreateProducts()
        {
            List<Product> products = new List<Product>();
            List<string> productsNames = new List<string>() { "молоко", "колбаса", "хлеб", "яйцо", "йогурт", "мороженое", "кефир", "вода", "картофель", "помидор", "огурец" };

            int minLimitPrice = 50;
            int maxLimitPrice = 201;

            foreach (string name in productsNames)
                products.Add(new Product(name, UserUtils.GenerateRandomNumber(minLimitPrice, maxLimitPrice)));

            return products;
        }

        private void CreateClients(int numerClients)
        {
            for (int i = 0; i < numerClients; i++)
                _clients.Enqueue(new Client(CreateBasket()));
        }

        private List<Product> CreateBasket()
        {
            List<Product> basket = new List<Product>();

            int minLimitSizeBasket = 1;

            for (int i = 0; i < UserUtils.GenerateRandomNumber(minLimitSizeBasket, _products.Count); i++)
                basket.Add(_products[UserUtils.GenerateRandomNumber(minLimitSizeBasket, _products.Count)]);

            return basket;
        }
    }

    class Client
    {
        private List<Product> _bag;

        public Client(List<Product> basket)
        {
            Money = CreateMoney();
            Basket = basket;
        }

        public int Money { get; private set; }
        public List<Product> Basket { get; private set; }

        public int Pay()
        {
            Money -= GetSumBasket();

            return GetSumBasket();
        }

        public void ShowBasket()
        {
            Console.WriteLine("Продукты в корзине.");

            foreach (Product product in Basket)
                product.ShowStats();
        }

        public void ShowBag()
        {
            Console.WriteLine("Продукты в сумке.");

            foreach (Product product in _bag)
                product.ShowStats();
        }

        public int GetSumBasket()
        {
            int sumPrice = 0;

            foreach (Product product in Basket)
                sumPrice += product.Price;

            return sumPrice;
        }

        private int CreateMoney()
        {
            int minLimitWallet = 200;
            int maxLimitWallet = 1000;

            return UserUtils.GenerateRandomNumber(minLimitWallet, maxLimitWallet);
        }
    }

    class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }

        public void ShowStats()
        {
            Console.WriteLine($"{Name} по цене {Price}");
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}
