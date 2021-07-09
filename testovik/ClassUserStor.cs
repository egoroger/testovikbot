using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace testovik
{
    public class TgUser
    {
        public string name;
        public bool istester = false;
        public TgUser(string _name, bool _istester) {name=_name; istester = _istester;}
        public void saveobj(StreamWriter w) 
        {
            if (istester==true) w.WriteLine("-"+name);
            else w.WriteLine(name);
        }
    }
    public class UserStor
    {
        public TgUser[] users;
        public int size, count;
        public UserStor(int size) {this.size=size;users=new TgUser[size];count=0;}
        ~UserStor() {}
        public void addu(TgUser user) 
        {
            users[count]=user;
            //if (count==0) users[count].istester=true;
            count++;
        }
        public void deluser(int index)
        {
            if (count == 1) users[count-1] = null;
            else if (count>1) for (int i=index; i<count; i++) users[i]=users[i+1];
            count--;
        }
        public TgUser getu(int index) {return users[index];}
        public void saveobj(StreamWriter w)
        {
            w.WriteLine("~BEGIN");
            if (count != 0)
            for (int i=0; i<count; i++) users[i].saveobj(w);
            w.WriteLine("~END");
        }
        public void loadobj(StreamReader w,int _count)
        {
            
            count=_count;string s= w.ReadLine(); s = w.ReadLine();
            
            while (!s.Equals("~END"))
            {
                if (s.StartsWith("-"))
                {                 
                    users[count] = new TgUser(s.Substring(1), true);
                    //count++; s = w.ReadLine();
                }
                else 
                {
                    users[count] = new TgUser(s,false); 
                    //count++; s=w.ReadLine();
                }
                count++; s = w.ReadLine();
            } 
        }
    }
}
