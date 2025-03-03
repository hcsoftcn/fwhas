
public class SqliteDB : IDatabase
{
    public bool OpenDB(string db)//打开数据库
    {
        return false;
    }

    public bool RegUser(string username, string password)//注册用户
    {
        return false;
    }
    public bool IsUserExists(string username)//用户存在吗？
    {  
        return false;
    }

    public bool LoginUser(string username, string password)//登陆用户
    {
        return false;
    }
    public bool CloseDB()//关闭数据库
    {
        return false;
    }
}
