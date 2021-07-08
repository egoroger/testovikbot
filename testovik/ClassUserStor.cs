using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace testovik
{
    public class TgUser
    {
        public string name;
        public TgUser(string _name) {name=_name;}
        public void saveobj(StreamWriter w) {w.WriteLine(name);}
    }
    public class UserStor
    {
        public string[] users;
        public int size, count;
        public UserStor(int size) {this.size=size;users=new string[size];count=0;}
        ~UserStor() {}
        public void addu(string user) {users[count]=user; count++;}
        public void deluser(int index)
        {
            if (count == 1) users[count-1] = null;
            else if (count>1) for (int i=index; i<count; i++) users[i]=users[i+1];
            count--;
        }
        public string getu(int index) {return users[index];}
        public void saveobj(StreamWriter w)
        {
            w.WriteLine("~BEGIN");
            if (count != 0)
            for (int i = 0; i < count; i++) w.WriteLine(users[i]);
            w.WriteLine("~END");
        }
        public void loadobj(StreamReader w,int _count)
        {
            count=_count;string s= w.ReadLine(); s = w.ReadLine();
            while (!s.Equals("~END")) {users[count]=s;count++;s=w.ReadLine();} 
        }
    }
}
