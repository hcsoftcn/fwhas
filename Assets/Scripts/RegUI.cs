using Unity.Netcode;
using GameDB;
using UnityEngine;
using UnityEngine.UI;

public class RegUI : NetworkBehaviour
{
    public InputField user;
    public InputField pass;
    public InputField pass1;
    // Start is called before the first frame update
    void Start()
    {
        //测试中文编码
        //if (IsServer)
            //GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnCancel()
    {
        Global.Singleton.net.SceneManager.ClientSwitchScene("Main");
    }

    public void OnBtnSubmit()
    {
        //Debug.LogFormat("{0},{1}", pass.text, pass1.text);
        if(pass.text!=pass1.text)
        {
            Debug.Log("两次输入的密码不一致。");
            return;
        }
        RegUserServerRpc(user.text,pass.text);
    }

    [ServerRpc(RequireOwnership=false)]
    public void RegUserServerRpc(string user,string pass)
    {
        SqliteDB db=new SqliteDB();
        db.OpenDB();
        if (db.IsUserExists(user))
        {
            ResClientRpc(-1);
        }
        else
        {
            if (db.RegUser(user, pass))
                ResClientRpc(0);
            else
                ResClientRpc(-2);
        }
        db.CloseDB();
    }

    [ClientRpc]
    public void ResClientRpc(int status)
    {
        if (status == 0)
            Debug.Log("注册成功。");
        else if (status == -1)
            Debug.Log("注册失败，用户名已经存在。");
        else
            Debug.Log("注册失败，系统故障。");
    }
}
