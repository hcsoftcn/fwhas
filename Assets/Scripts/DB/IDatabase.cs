
namespace GameDB
{
    public interface IDatabase
    {
        bool OpenDB();//打开数据库
        bool RegUser(string username, string password);//注册用户
        bool IsUserExists(string username);//用户存在吗？
        bool LoginUser(string username, string password);//登陆用户
        bool CloseDB();//关闭数据库
    }
}