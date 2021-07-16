using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using testovik.Command;
using Npgsql;
using System.IO;

namespace testovik
{
    class Program
    {
        //входные переменные
        public static string token {get;set;}="1878631598:AAFp2SFtcbcSMRNFkW9r1l_kMqh1MxaTvdA";
        public static TelegramBotClient client;
        public static List<Command.Command> commands;
        public static Command.addu Addu;
        public static UserStor userstor=new UserStor(200000);
        public static TgUser tguser;
        public static string vvedite="Введите команду\n";
        public static string host;
        public static string userid;
        public static string pw;
        public static string db;
        public static int port;
        public static bool dba = false;
        public static void savestor()
        {
            using (var conn = new NpgsqlConnection("Host="+host+"; User Id="+userid+";Password="+pw+";Database="+db+";Port="+port))
            {
                conn.Open(); NpgsqlCommand comm;
                using (comm=new NpgsqlCommand("DROP TABLE IF EXISTS spisok",conn)) {comm.ExecuteNonQuery();}
                using (comm = new NpgsqlCommand("CREATE TABLE spisok(id serial PRIMARY KEY, name VARCHAR(50), istester BOOLEAN)",conn)) {comm.ExecuteNonQuery();}
                using (StreamReader r = new StreamReader("Test.txt"))
                {
                    r.ReadLine(); int i=0;
                    string s=r.ReadLine();
                    while (s!="~END")
                    {
                        using (comm=new NpgsqlCommand("INSERT INTO spisok (name,istester) VALUES (@n"+Convert.ToString(i+1)+",@t"+Convert.ToString(i+1)+")",conn))
                        {
                            if (s.StartsWith("-"))
                            {
                                comm.Parameters.AddWithValue("@n"+Convert.ToString(i+1),s.Substring(1));
                                comm.Parameters.AddWithValue("@t"+Convert.ToString(i+1),true);
                            }
                            else
                            {
                                comm.Parameters.AddWithValue("@n"+Convert.ToString(i+1),s);
                                comm.Parameters.AddWithValue("@t"+Convert.ToString(i+1),false);
                            }
                            comm.ExecuteNonQuery();
                        }
                        i++; s=r.ReadLine();
                    }                        
                }
            }
        } //сохранение данных в бд
        public static void savestorf(UserStor _userstor)
        {
            using (StreamWriter w=new StreamWriter("Test.txt",false,System.Text.Encoding.Default))
            {
                _userstor.savestor(w);
            }
        } //сохранение данных в файл
        public static void load(UserStor _userstor, string _host, string _userid, string _pw, string _db, int _port)
        {
            using (var conn = new NpgsqlConnection("Host="+_host+"; User Id="+_userid+";Password="+_pw+";Database="+_db+";Port="+_port))
            {
                try
                {
                    savestor();
                    conn.Open();
                    using (var comm=new NpgsqlCommand("CREATE TABLE IF NOT EXISTS spisok(id serial PRIMARY KEY, name VARCHAR(50), istester BOOLEAN)",conn)) {comm.ExecuteNonQuery();}
                    using (var comm=new NpgsqlCommand("SELECT * FROM spisok",conn))
                    {
                        var reader=comm.ExecuteReader();
                        while (reader.Read())
                        {
                            TgUser _tguser=new TgUser(reader.GetString(1),reader.GetBoolean(2));
                            _userstor.addu(_tguser);
                        }
                        reader.Close();
                    }
                    dba = true;
                }
                catch
                {
                    using (StreamReader r = new StreamReader("Test.txt"))
                    {
                        _userstor.loadstor(r);
                    }
                }                                
            }
        } //загрузка данных
        [Obsolete]
        static void Main(string[] args)
        {
            client=new TelegramBotClient(token); commands=new List<Command.Command>();            
            commands.Add(new actualt());commands.Add(new nextt());
            commands.Add(new show());commands.Add(new del()); Addu=new addu();
            host="localhost"; userid="postgres"; pw="bigrobot"; db="tester"; port=5432;
            load(userstor,host,userid,pw,db,port); client.StartReceiving(); client.OnMessage+=OnMessageHandler;
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
                        userstor.deluser(i); savestorf(userstor); if (dba==true) savestor(); 
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
                        savestorf(userstor); if (dba==true) savestor(); 
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
                            savestorf(userstor); if (dba==true) savestor(); 
                        }                        
                        Addu.stopex(msg,client);  
                    }
                }
                else if (msg.Text=="/exit")
                {
                    await client.SendTextMessageAsync(msg.Chat.Id,"exit",replyMarkup:new ReplyKeyboardRemove());
                    if (Addu.active==true) Addu.stopex(msg, client);
                }
                else foreach (var comm in commands)
                {
                    if (comm.Contains(msg.Text))
                    {
                        comm.execute(msg,client,userstor);
                        if (Addu.active==true) Addu.stopex(msg,client);
                        savestorf(userstor); if (dba==true) savestor(); 
                    }
                }
            }
        }
    }
}