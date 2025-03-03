using SQLite4Unity3d;
using UnityEngine;

namespace GameDB
{
    public class SqliteDB : IDatabase
    {
        private SQLiteConnection connection;
        public bool OpenDB()//�����ݿ�
        {
            string path = Application.streamingAssetsPath + "/data.db";
            connection = new SQLiteConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            return true;
        }

        public bool RegUser(string username, string password)//ע���û�
        {
            if (IsUserExists(username))
                return false;

            UserTbl table = new UserTbl
            {
                UserName = username,
                Password = password,
                RegDate = System.DateTime.Now
            };
            int n=connection.Insert(table);
            if(n>0)
                return true;
            else
                return false;
        }
        public bool IsUserExists(string username)//�û�������
        {
            var query = connection.Table<UserTbl>().Where(v => v.UserName == username);
            if (query.Count() > 0)
                return true;
            else
                return false;
        }

        public bool LoginUser(string username, string password)//��½�û�
        {
            var query = connection.Table<UserTbl>().Where(v => v.UserName == username && v.Password == password);
            if (query.Count() > 0)
                return true;
            else
                return false;
        }
        public bool CloseDB()//�ر����ݿ�
        {
            connection.Close();
            return true;
        }
    }

    public class UserTbl
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public System.DateTime RegDate { get; set; }
    }
}