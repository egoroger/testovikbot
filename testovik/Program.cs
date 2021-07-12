using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using testovik.Command;
using Npgsql;
namespace testovik
{
    class Program
    {
        public static string token {get;set;}="1878631598:AAFp2SFtcbcSMRNFkW9r1l_kMqh1MxaTvdA";
        public static TelegramBotClient client;
        public static List<Command.Command> commands;
        public static Command.addu Addu;
        public static void savestor(UserStor _userstor)
        {
            using (var conn = new NpgsqlConnection("Host=localhost; User Id=postgres;Password=bigrobot;Database=testovik;Port=5432"))
            {
                conn.Open(); NpgsqlCommand comm;
                using (comm=new NpgsqlCommand("DROP TABLE IF EXISTS users",conn)) {comm.ExecuteNonQuery();}
                using (comm = new NpgsqlCommand("CREATE TABLE users(id serial PRIMARY KEY, name VARCHAR(50), istester BOOLEAN)",conn)) {comm.ExecuteNonQuery();}              
                for (int i=0; i<_userstor.count; i++)
                using (comm=new NpgsqlCommand("INSERT INTO users (name,istester) VALUES (@n"+Convert.ToString(i+1)+",@t"+Convert.ToString(i+1)+")",conn))
                {
                    comm.Parameters.AddWithValue("@n"+Convert.ToString(i+1),_userstor.getu(i).name);
                    comm.Parameters.AddWithValue("@t"+Convert.ToString(i+1),_userstor.getu(i).istester);
                    comm.ExecuteNonQuery();
                }
            }
        }
        public static void load(UserStor _userstor)
        {
            using (var conn = new NpgsqlConnection("Host=localhost; User Id=postgres;Password=bigrobot;Database=testovik;Port=5432"))
            {
                conn.Open();
                using (var comm = new NpgsqlCommand("SELECT * FROM users", conn))
                {
                    var reader=comm.ExecuteReader();
                    while (reader.Read())
                    {
                        TgUser _tguser=new TgUser(reader.GetString(1),reader.GetBoolean(2));
                        _userstor.addu(_tguser);
                    }
                    reader.Close();
                }
            }
        }        
        public static UserStor userstor=new UserStor(200000);
        public static TgUser tguser;
        public static string vvedite="Введите команду\n";
        [Obsolete]
        static void Main(string[] args)
        {
            client=new TelegramBotClient(token); commands=new List<Command.Command>();            
            commands.Add(new actualt());commands.Add(new nextt());
            commands.Add(new show());commands.Add(new del()); Addu=new addu(); 
            load(userstor); client.StartReceiving(); client.OnMessage += OnMessageHandler;
            Console.ReadLine(); client.StopReceiving();
        }
        [Obsolete]
        public static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg=e.Message;
            if (msg.Text!=null)
            {
                Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                if (Addu.Contains(msg.Text)) Addu.execute(msg,client,userstor);       
                else if (msg.Text.StartsWith("/del") && msg.Text!="/del" && !msg.Text.StartsWith("/del@"))
                {
                    for (int i=0; i<userstor.count; i++)
                    if (msg.Text.EndsWith("/del"+Convert.ToString(i+1)) && msg.Text.StartsWith("/del"+Convert.ToString(i+1)))
                    {
                        if (userstor.getu(i).istester == true)
                        {
                            if (i<userstor.count-1) userstor.getu(i+1).istester=true;
                            else userstor.getu(i+1-userstor.count).istester=true;
                        }
                        await client.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(i).name+" удалён.\n",replyMarkup:new ReplyKeyboardRemove());
                        userstor.deluser(i); savestor(userstor);
                    }
                    if (Addu.active==true) Addu.stopex(msg,client); 
                }
                else if (msg.Text.StartsWith("/choose") && msg.Text!="/choose" && !msg.Text.StartsWith("/choose@"))
                {
                    for (int i=0; i<userstor.count; i++)
                    if (msg.Text.EndsWith("/choose"+Convert.ToString(i+1)) && msg.Text.StartsWith("/choose"+Convert.ToString(i+1)))
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(i).name+" новый тестировщик\n\n",replyMarkup:new ReplyKeyboardRemove());
                        userstor.getu(i).istester=true;
                        for (int j=0; j<userstor.count; j++)
                        if (userstor.getu(j).istester==true && i!=j) userstor.getu(j).istester=false;
                        savestor(userstor);
                    }
                    if (Addu.active==true) Addu.stopex(msg,client); 
                }
                else if (msg.Text.StartsWith("@") && !msg.Text.StartsWith("@ "))
                {
                    if (Addu.active==true)
                    {
                        char Mychar='@';
                        bool x=false;
                        for (int i=0; i<userstor.count; i++)
                        if (msg.Text.TrimStart(Mychar)==userstor.getu(i).name) x=true;
                        if (x==true) await client.SendTextMessageAsync(msg.Chat.Id,msg.Text+" уже есть в списке\n\n");
                        else 
                        { 
                            tguser=new TgUser(msg.Text.TrimStart(Mychar),false); userstor.addu(tguser);
                            await client.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(userstor.count-1).name+" добавлен\n\n");
                            savestor(userstor);
                        }                        
                        Addu.stopex(msg,client);  
                    }
                }
                else if (msg.Text=="/exit")
                {
                    await client.SendTextMessageAsync(msg.Chat.Id,"exit",replyMarkup:new ReplyKeyboardRemove());
                    if (Addu.active==true) Addu.stopex(msg, client);
                    //savestor(userstor);
                }
                else foreach (var comm in commands)
                {
                    if (comm.Contains(msg.Text))
                    {
                        comm.execute(msg,client,userstor);
                        if (Addu.active==true) Addu.stopex(msg,client);
                        savestor(userstor);
                    }
                }
            }
        }
    }
}