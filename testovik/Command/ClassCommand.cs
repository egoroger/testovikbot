using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
namespace testovik.Command
{
    public abstract class Command
    {
        public string vvedite="Введите команду \n";
        public abstract string[] Names {get;set;}
        public abstract void execute(Message msg,TelegramBotClient clnt,UserStor userstor);
        public bool Contains(string msg)
        {
            foreach(var ms in Names) {if (msg.Contains(ms)) return true;}
            return false;
        }
    }
    public class addu:Command
    {
        public bool active=false;
        public override string[] Names {get; set;}=new string[] {"add"};
        public override async void execute(Message msg,TelegramBotClient clnt,UserStor userstor) 
        {
            active=true;
            await clnt.SendTextMessageAsync(msg.Chat.Id,"Операция Add запущена. Введите пользователя");
        }
        public async void stopex(Message msg,TelegramBotClient clnt)
        {
            active=false;
            await clnt.SendTextMessageAsync(msg.Chat.Id,"Операция Add приостановлена");
        }
    }
    public class del:Command
    {
        public string opisanie="";
        public override string[] Names {get;set;}=new string[] {"del"};
        public override async void execute(Message msg,TelegramBotClient clnt,UserStor userstor)
        {
            if (userstor.count<=0) await clnt.SendTextMessageAsync(msg.Chat.Id,"пусто\n\n");
            else if (userstor.count==1)
            {
                await clnt.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(userstor.count-1).name+" удалён\n\n");
                userstor.deluser(userstor.count-1);
            }
            else
            {
                opisanie="";
                for (int i=0; i<userstor.count; i++)
                opisanie=opisanie+"del"+Convert.ToString(i+1)+" - удалить пользователя под номером "+Convert.ToString(i+1)+" из списка\n";
                opisanie=opisanie+"exit - прекращение операции удаления\n";
                await clnt.SendTextMessageAsync(msg.Chat.Id,"choose number ("+1+" to "+userstor.count+")\n\n"+opisanie,replyMarkup:GetButtons(userstor));
                {Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");}
            }
        }
        public IReplyMarkup GetButtons(UserStor userstor)
        {
            var rkm=new ReplyKeyboardMarkup();
            var rows=new List<KeyboardButton[]>();
            var cols=new List<KeyboardButton>();
            for (int i=0; i<userstor.count; i++)
            {
                cols.Add(new KeyboardButton("/del"+Convert.ToString(i+1)));
                if (userstor.count%2==0) {if ((i+1)%(userstor.count/2)!=0) continue;}
                else if (userstor.count%3==0) {if ((i+1)%(userstor.count/3)!=0) continue;}
                else if (userstor.count%5==0) {if ((i+1)%(userstor.count/5)!=0) continue;}
                else if (userstor.count%7==0) {if ((i+1)%(userstor.count/7)!=0) continue;}
                else if ((i+1)%(userstor.count)!=0) continue;
                rows.Add(cols.ToArray()); cols=new List<KeyboardButton>();
            }
            cols.Add(new KeyboardButton("/exit"));rows.Add(cols.ToArray());rkm.Keyboard=rows.ToArray();return rkm;
        }
    }
    public class show:Command
    {
        public override string[] Names {get;set;}=new string[] {"show"};
        public override async void execute(Message msg,TelegramBotClient clnt,UserStor userstor)
        {
            string spisok;
            if (userstor.count<=0) spisok="пусто\n\n";
            else
            {
                spisok="1 - @";
                if (userstor.count>1)
                {
                    for (int i=0; i<userstor.count-1; i++) spisok+=userstor.getu(i).name+",\n"+Convert.ToString(i+2)+" - @";
                    spisok+=userstor.getu(userstor.count-1).name+"\n\n";
                }
                else spisok+=userstor.getu(userstor.count-1).name;                
            }
            await clnt.SendTextMessageAsync(msg.Chat.Id,spisok+"\n\n");
        }
    }
    public class actualt:Command
    {
        public override string[] Names {get;set;}=new string[] {"actualt"};
        public override async void execute(Message msg,TelegramBotClient clnt,UserStor userstor)
        {
            if (userstor.count<=0) await clnt.SendTextMessageAsync(msg.Chat.Id, "пусто\n\n");
            else
            {
                string nm="";
                for (int i=0; i<userstor.count; i++)
                if (userstor.getu(i).istester==true) nm=userstor.getu(i).name;
                if (nm=="") await clnt.SendTextMessageAsync(msg.Chat.Id,"нет тестировщика\n\n");
                else await clnt.SendTextMessageAsync(msg.Chat.Id,"@"+nm+" тестировщик\n\n");
            }
        }
    }
    public class nextt:Command
    {
        public string opisanie="";
        public override string[] Names {get;set;}=new string[] {"nextt"};
        public override async void execute(Message msg,TelegramBotClient clnt,UserStor userstor)
        {
            if (userstor.count<=0) await clnt.SendTextMessageAsync(msg.Chat.Id,"пусто\n\n");
            else if (userstor.count==1)
            {
                userstor.getu(userstor.count-1).istester=true;
                await clnt.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(userstor.count-1).name+" новый тестировщик \n\n");
            }
            else if (userstor.count==2)
            {
                if (userstor.getu(0).istester==true && userstor.getu(1).istester==false)
                {
                    userstor.getu(0).istester=false; userstor.getu(1).istester=true;
                    await clnt.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(1).name+" новый тестировщик\n\n");
                }
                else if (userstor.getu(0).istester==false && userstor.getu(1).istester==true)
                {
                    userstor.getu(0).istester=true; userstor.getu(1).istester=false;
                    await clnt.SendTextMessageAsync(msg.Chat.Id,"@"+userstor.getu(0).name+" новый тестировщик\n\n");
                }
                else if (userstor.getu(0).istester==false && userstor.getu(1).istester==false)
                {
                    opisanie=opisanie+"choose  1 - назначить пользователя под номером 1 тестировщиком\n";
                    opisanie=opisanie+"choose  2 - назначить пользователя под номером 2 тестировщиком\n";
                    await clnt.SendTextMessageAsync(msg.Chat.Id,"choose number\n\n"+opisanie, replyMarkup:GetButtons(userstor,2));
                    {
                        opisanie=opisanie+"exit - прекращение операции назначения тестировщика\n";
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                    }
                }
            }
            else
            {
                int i=0; bool it=false;opisanie="";
                while (it==false && i<userstor.count) 
                {
                    it=userstor.getu(i).istester;    
                    if (it==false) opisanie=opisanie+"choose"+Convert.ToString(i+1)+" - назначить пользователя под номером "+Convert.ToString(i+1)+" тестировщиком\n";                    
                    i++;
                }               
                for (int j=i; j<userstor.count; j++)
                {opisanie=opisanie+"choose"+Convert.ToString(j+1)+" - назначить пользователя под номером "+Convert.ToString(j+1)+" тестировщиком\n";}
                int x=i;
                for (int j=0; j<userstor.count; j++)
                if (userstor.getu(j).istester==true) x=i-1;
                await clnt.SendTextMessageAsync(msg.Chat.Id,"choose number\n\n"+opisanie,replyMarkup:GetButtons(userstor,x));
                {
                    opisanie=opisanie+"exit - прекращение операции назначения тестировщика\n";
                    Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                }
            }
        }
        public IReplyMarkup GetButtons(UserStor userstor,int j)
        {
            var rkm=new ReplyKeyboardMarkup(); var rows=new List<KeyboardButton[]>();
            var cols=new List<KeyboardButton>();
            for (int i=0; i<j; i++)
            {
                cols.Add(new KeyboardButton("/choose"+Convert.ToString(i+1)));
                if ((userstor.count-1)%2==0) {if ((i+1)%((userstor.count-1)/2)!=0) continue;}
                else if ((userstor.count-1)%3==0) {if ((i+1)%((userstor.count-1)/3)!=0) continue;}
                else if ((userstor.count-1)%5==0) {if ((i+1)%((userstor.count-1)/5)!=0) continue;}
                else if ((userstor.count-1)%7==0) {if ((i+1)%((userstor.count-1)/7)!=0) continue;}
                else if ((i+1)%(userstor.count-1)!=0) continue;
                rows.Add(cols.ToArray()); cols=new List<KeyboardButton>();
            }
            if (j<userstor.count-1)
            for (int i=j+1; i<userstor.count; i++)
            {
                cols.Add(new KeyboardButton("/choose"+Convert.ToString(i+1)));
                if ((userstor.count-1)%2==0) {if ((i+1)%(userstor.count/2)!=0) continue;}
                else if ((userstor.count-1)%3==0) {if ((i+1)%((userstor.count-1)/3)!=0) continue;}
                else if ((userstor.count-1)%5==0) {if ((i+1)%((userstor.count-1)/5)!=0) continue;}
                else if ((userstor.count-1)%7==0) {if ((i+1)%((userstor.count-1)/7)!=0) continue;}
                else if ((i+1)%(userstor.count-1)!=0) continue;
                rows.Add(cols.ToArray()); cols=new List<KeyboardButton>();
            }
            cols.Add(new KeyboardButton("/exit")); rows.Add(cols.ToArray()); rkm.Keyboard=rows.ToArray(); return rkm;
        }
    }
}