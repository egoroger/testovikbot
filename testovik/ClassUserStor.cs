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
    }
    public class UserStor
    {
        public TgUser[] users;
        public int size, count;
        public UserStor(int size) {this.size=size;users=new TgUser[size];count=0;}
        ~UserStor() {}
        public void addu(TgUser user) {users[count]=user; count++;}
        public void deluser(int index)
        {
            if (count==1) users[count-1] = null;
            else if (count>1) for (int i=index; i<count; i++) users[i]=users[i+1];
            count--;
        }
        public TgUser getu(int index) {return users[index];}
    }
}
