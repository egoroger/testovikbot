using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using testovik.Command;
namespace testovik
{
    class Program
    {
        public static string token {get;set;}="1878631598:AAFp2SFtcbcSMRNFkW9r1l_kMqh1MxaTvdA";
        public static TelegramBotClient client;
        public static List<Command.Command> commands;
        public static Command.addu Addu;
        public static Command.start Start;
        public static void savestor(UserStor _userstor)
        {
            using (StreamWriter sw=new StreamWriter(@"Test.txt",false,System.Text.Encoding.Default))
            {_userstor.saveobj(sw);sw.Close();}
        }
        public static void load(UserStor _userstor)
        {
            using (StreamReader sr=new StreamReader(@"Test.txt",System.Text.Encoding.Default)) {_userstor.loadobj(sr,0);}
        }        
        public static UserStor userstor=new UserStor(200000);
        public static TgUser tguser;
        public static string vvedite = "Введите команду\n";
        [Obsolete]
        static void Main(string[] args)
        {
            client=new TelegramBotClient(token);
            commands=new List<Command.Command>();            
            commands.Add(new actualt());commands.Add(new nextt());
            commands.Add(new show());commands.Add(new del());
            Addu=new addu(); Start=new start();
            load(userstor);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
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
                        await client.SendTextMessageAsync(msg.Chat.Id, "@" + userstor.getu(i).name + " удалён. \n", replyMarkup: new ReplyKeyboardRemove());
                        userstor.deluser(i);
                    }
                    if (Addu.active==true) Addu.stopex(msg,client);
                    savestor(userstor);
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
                    }
                    if (Addu.active==true) Addu.stopex(msg,client);
                    savestor(userstor);
                }
                else if (msg.Text.StartsWith("@") && !msg.Text.StartsWith("@ "))
                {
                    if (Addu.active == true)
                    {
                        char Mychar='@';
                        tguser = new TgUser(msg.Text.TrimStart(Mychar), false);
                        userstor.addu(tguser);
                        await client.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(userstor.count-1).name+" добавлен\n\n");
                        Addu.stopex(msg,client);
                        savestor(userstor); 
                    }
                }
                else if (msg.Text=="/exit")
                {
                    await client.SendTextMessageAsync(msg.Chat.Id, vvedite,replyMarkup:new ReplyKeyboardRemove());
                    if (Addu.active==true) Addu.stopex(msg, client);
                    savestor(userstor);
                }                
                else if (msg.Text=="/start")
                {
                    Start.execute(msg,client,userstor);
                    if (Addu.active==true) Addu.stopex(msg,client);
                    //load(userstor);
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