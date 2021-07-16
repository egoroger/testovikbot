using System.IO;
namespace testovik
{
    public class TgUser
    {
        public string name;
        public bool istester = false;
        public TgUser(string _name, bool _istester) {name=_name; istester=_istester;}
        public void saveobj(StreamWriter w) 
        {
            if (istester == false) w.WriteLine(name);
            else w.WriteLine("-"+name);
        }
    } //пользователь
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
        public void savestor(StreamWriter w)
        {
            w.WriteLine("~BEGIN");
            for (int i=0; i<count; i++) getu(i).saveobj(w);
            w.WriteLine("~END");
        }
        public void loadstor(StreamReader r)
        {
            for (int i=0; i<count; i++) users[i]=null;
            string s; r.ReadLine();          
            count=0; s=r.ReadLine();
            while (s!="~END")
            {
                if (s.StartsWith("-")) users[count]=new TgUser(s.Substring(1),true);
                else users[count]=new TgUser(s,false);
                count++;
                s=r.ReadLine();
            }
        }
    } //список пользователей
}