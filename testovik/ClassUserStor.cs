using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace testovik
{
    public class TgUser
    {
        public string name;
        public TgUser(string name) {this.name=name;}
        public void saveobj(StreamWriter w)
        {
            w.WriteLine("N:" + name);
        }
    }
    public class UserStor
    {
        public TgUser[] users;
        public int size, count;
        public UserStor(int size)
        {
            this.size=size;
            users=new TgUser[size];
            count=0;
        }
        ~UserStor() {}
        public void addu(TgUser user) {users[count]=user; count++;}
        public void deluser(int index)
        {
            for (int i=index; i<count; i++) users[i]=users[i+1];
            count--;
        }
        public TgUser getu(int index) {return users[index];}
        public void saveobj(StreamWriter w)
        {
            w.WriteLine("~BEGIN");
            w.WriteLine(count);
            for (int i = 0; i < count; i++) users[i].saveobj(w);
            w.WriteLine("~END");
        }
        public void loadobj(StreamReader w)
        {
            w.ReadLine();
            for (int i = 0; i < count; i++) users[i].name=w.ReadLine();
            w.ReadLine();
        }
    }
}
