
public class SqliteDB : IDatabase
{
    public bool OpenDB(string db)//�����ݿ�
    {
        return false;
    }

    public bool RegUser(string username, string password)//ע���û�
    {
        return false;
    }
    public bool IsUserExists(string username)//�û�������
    {  
        return false;
    }

    public bool LoginUser(string username, string password)//��½�û�
    {
        return false;
    }
    public bool CloseDB()//�ر����ݿ�
    {
        return false;
    }
}
