
namespace GameDB
{
    public interface IDatabase
    {
        bool OpenDB();//�����ݿ�
        bool RegUser(string username, string password);//ע���û�
        bool IsUserExists(string username);//�û�������
        bool LoginUser(string username, string password);//��½�û�
        bool CloseDB();//�ر����ݿ�
    }
}