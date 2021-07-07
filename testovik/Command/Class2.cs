using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace testovik.Command
{
    public abstract class Command
    {
        public abstract string[] Names { get; set; }
        public abstract void execute(Message msg, TelegramBotClient clnt, UserStor userstor);
        public bool Contains(string msg)
        {
            foreach(var ms in Names)
            {
                if (msg.Contains(ms)) return true;
            }
            return false;
        }
    }
    public class addu:Command
    {
        public bool active = false;
        public override string[] Names { get; set; } = new string[] {"add"};
        public override async void execute(Message msg, TelegramBotClient clnt, UserStor userstor) 
        {
            active = true;
            await clnt.SendTextMessageAsync(msg.Chat.Id, "Add is activated");
        }
        public async void stopex(Message msg, TelegramBotClient clnt)
        {
            active = false;
            await clnt.SendTextMessageAsync(msg.Chat.Id, "Add is stopped");
        }
    }
    public class choose : Command
    {
        public override string[] Names { get; set; } = new string[] {"choose"};
        public override async void execute(Message msg, TelegramBotClient clnt, UserStor userstor)
        {
            string chosen = "@"; int[] k = new int[userstor.count];
            for (int j = 0; j < userstor.count; j++) k[j] = j;
            if (userstor.count <= 0) await clnt.SendTextMessageAsync(msg.Chat.Id, "пусто");
            else
            {
                if (userstor.count == 1) chosen += userstor.getu(userstor.count - 1).name;
                else chosen += userstor.getu(k[new Random().Next(0, k.Length)]).name;
                await clnt.SendTextMessageAsync(msg.Chat.Id, chosen+" тестировщик");
            }
        }
    }
    public class del : Command
    {
        public override string[] Names { get; set; } = new string[] {"del"};
        public override async void execute(Message msg, TelegramBotClient clnt, UserStor userstor)
        {
            if (userstor.count <= 0) await clnt.SendTextMessageAsync(msg.Chat.Id, "пусто");
            else if (userstor.count == 1)
            {
                await clnt.SendTextMessageAsync(msg.Chat.Id, "@" + userstor.getu(userstor.count-1).name+" удалён",replyMarkup:GetButtons());
                userstor.deluser(userstor.count-1);
            }
            else
            {
                await clnt.SendTextMessageAsync(msg.Chat.Id,"choose number ("+1+" to "+userstor.count+")",replyMarkup:GetButtons2(userstor));
                {
                    Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                }
            }
        }
        private IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton {Text="/add"},
                        new KeyboardButton {Text="/choose"},
                        new KeyboardButton {Text="/del"},
                        new KeyboardButton {Text="/show"}
                    }
                }
            };
        }
        public IReplyMarkup GetButtons2(UserStor userstor)
        {
            var rkm = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            for (int i = 0; i < userstor.count; i++)
            {
                cols.Add(new KeyboardButton("/del" + Convert.ToString(i + 1)));
                if (userstor.count % 2 == 0) { if ((i + 1) % (userstor.count / 2) != 0) continue; }
                else if (userstor.count % 3 == 0) { if ((i + 1) % (userstor.count / 3) != 0) continue; }
                else if (userstor.count % 5 == 0) { if ((i + 1) % (userstor.count / 5) != 0) continue; }
                else if (userstor.count % 7 == 0) { if ((i + 1) % (userstor.count / 7) != 0) continue; }
                else if ((i + 1) % (userstor.count) != 0) continue;
                rows.Add(cols.ToArray());
                cols = new List<KeyboardButton>();
            }
            cols.Add(new KeyboardButton("/exit"));
            rows.Add(cols.ToArray());
            rkm.Keyboard = rows.ToArray();
            return rkm;
        }
    }
    public class show : Command
    {
        public override string[] Names { get; set; } = new string[] { "show" };
        public override async void execute(Message msg, TelegramBotClient clnt, UserStor userstor)
        {
            if (userstor.count <= 0) await clnt.SendTextMessageAsync(msg.Chat.Id, "пусто");
            else
            {
                string spisok = "1 - @";
                if (userstor.count > 1)
                {
                    for (int i = 0; i < userstor.count - 1; i++) { spisok += userstor.getu(i).name + ", \n" + Convert.ToString(i + 2) + " - @"; }
                    spisok += userstor.getu(userstor.count - 1).name; await clnt.SendTextMessageAsync(msg.Chat.Id, spisok);
                }
                else { spisok += userstor.getu(userstor.count - 1).name; await clnt.SendTextMessageAsync(msg.Chat.Id, spisok); }
            }
        }
    }
}
