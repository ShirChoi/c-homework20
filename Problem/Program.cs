using System;
using System.Threading;
using System.Collections.Generic;

namespace Problem {
    class Program {
        static List<Client> clients;
        static int ID = 0;
        static void Main(string[] args) {
            clients = new List<Client>();
            TimerCallback tm = new TimerCallback(DifferenceLog);
            Timer timer = new Timer(tm, null, 0, 1000);

            while(true) {
                System.Console.WriteLine(
                    "Выберите действие:\n" +
                    "1. Добавить пользователя\n" +
                    "2. Обновить баланс пользователя\n" +
                    "3. Удалить пользователя\n" +
                    "4. Выбрать пользователя"
                );
                int input = -1;
                while(input <= 0 || input >= 5) {
                    int.TryParse(Console.ReadLine(), out input);
                }

                

                switch(input) {
                    case 1: {
                        Client client = new Client();
                        Console.Write("Введите имя клиента:");
                        client.Name = Console.ReadLine();
                        Console.Write("Введите возраст клиента:");
                        client.Age = int.Parse(Console.ReadLine());
                        Console.Write("Введите баланс клиента:");
                        client.Balance = int.Parse(Console.ReadLine());

                        Thread taskThread = new Thread(new ParameterizedThreadStart(Insert));
                        taskThread.Start(client);
                        
                    } break;

                    case 2: {
                        bool validID = false;
                        int ID = -1;
                        while(!validID) {
                            Console.Write("Введите ID клиента:");
                            ID = int.Parse(Console.ReadLine());
                            validID = clients.Find((cl) => cl.ID == ID) != null;
                            if(!validID) {
                                System.Console.WriteLine("клиента с таким ID не существует");
                                continue;
                            }
                        }

                        Console.Write("Введите новый баланс клиента:");
                        int balance = int.Parse(Console.ReadLine());

                        Thread taskThread = new Thread(() => {
                           Update(ID, balance); 
                           System.Console.WriteLine("клиент успешно обновлён");
                        });

                        taskThread.Start();
                        
                    } break;
                    
                    case 3: {
                        bool validID = false;
                        int ID = -1;
                        while(!validID) {
                            Console.Write("Введите ID клиента:");
                            ID = int.Parse(Console.ReadLine());
                            validID = clients.Find((cl) => cl.ID == ID) != null;
                            if(!validID) {
                                System.Console.WriteLine("клиента с таким ID не существует");
                                continue;
                            }
                        }

                        Thread taskThread = new Thread(() => {
                            Delete(ID);
                        });
                        taskThread.Start();
                        
                    } break;

                    case 4: {
                        Console.Write("Введите ID клиента:");
                        int ID = int.Parse(Console.ReadLine());

                        Client client = null;
                        Thread taskThread = new Thread(() => {
                            client = Select(ID);
                            Console.WriteLine(client);
                        });
                        taskThread.Start();
                    } break;
                }
                
            }
        }
        static void DifferenceLog(object? state) {
            foreach(Client client in clients) {
                if(client.BalanceDelta != 0) {
                    System.Console.WriteLine(
                        $"Измненение баланса клиента с ID: {client.ID}\n" +
                        $"С {client.OldBalance} до {client.Balance}"
                    );
                    Console.ForegroundColor = client.BalanceDelta > 0 ? ConsoleColor.Green : ConsoleColor.Red;
                    System.Console.WriteLine(
                        $"Изменение на {client.BalanceDelta}"
                    );
                    Console.ForegroundColor = ConsoleColor.White;
                    client.OldBalance = client.Balance;
                    System.Console.WriteLine();
                }
            }
        }
        static void Insert(object objClient) {
            Client client = (Client)objClient;
            ID++;
            client.ID = ID;
            client.OldBalance = client.Balance;
            clients.Add(client);
        }

        static void Update( int ID,  int balance) {
            Client client1 = clients.Find((cl) => cl.ID == ID);

            client1.Balance = balance;
        }

        static void Delete( int ID) {
            bool del = clients.Remove(clients.Find(cl => cl.ID == ID));
            // System.Console.WriteLine($"deleted = {del}");
        }

        static Client Select( int ID) {
            return clients.Find(cl => cl.ID == ID);
        }
    }

    class Client {
        public int ID {get; set; }
        public decimal OldBalance {get; set;}
        public decimal Balance {get; set; }
        public decimal BalanceDelta => Balance - OldBalance;
        public string Name {get; set; }
        public int Age {get; set;}

        public override string ToString() {
            return 
                $"ID        = {ID}\n" +
                $"Name      = {Name}\n" +
                $"Age       = {Age}\n" +
                $"Balance   = {Balance}";
        }
    }
}
