using Unity.Netcode;
using GameDB;
using UnityEngine;
using UnityEngine.UI;


public class RegUI : NetworkBehaviour
{
    public InputField user;
    public InputField pass;
    public InputField pass1;
    private Msgbox msgbox;
    private ClientRpcParams clientRpcParams;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MsgBox");
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3(0, 0, 0);
        instance.transform.rotation = Quaternion.identity;
        msgbox = instance.GetComponent<Msgbox>();

        clientRpcParams = new ClientRpcParams();
        clientRpcParams.Send = new ClientRpcSendParams();
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
        //用户名不能为空
        if(user.text.Trim()=="")
        {
            msgbox.Show("reg.msg5");
            return;
        }
        //密码必须大于6位
        if(pass.text.Trim().Length<6)
        {
            msgbox.Show("reg.msg6");
            return;
        }    
        //二次输入密码是否一致
        if(pass.text!=pass1.text)
        {
            msgbox.Show("reg.msg2");
            return;
        }
        RegUserServerRpc(user.text,pass.text);
    }

    [ServerRpc(RequireOwnership=false)]
    public void RegUserServerRpc(string user,string pass,ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;

        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;

        clientRpcParams.Send.TargetClientIds = new ulong[] { id };

        SqliteDB db=new SqliteDB();
        db.OpenDB();
        if (db.IsUserExists(user))
        {
            //注册失败，用户名已经存在。
            ResClientRpc(-1, clientRpcParams);
        }
        else
        {
            if (db.RegUser(user, pass))
                ResClientRpc(0, clientRpcParams);//注册成功。
            else
                ResClientRpc(-2, clientRpcParams);//注册失败，系统故障。
        }
        db.CloseDB();
    }

    [ClientRpc]
    public void ResClientRpc(int status, ClientRpcParams clientRpcParams = default)
    {
        if (status == 0)
        {
            //注册成功
            msgbox.Show("reg.msg4");
        }
        else if (status == -1)
        {
            //注册失败，用户名已经存在。
            msgbox.Show("reg.msg1");
        }
        else
        {
            //注册失败，系统故障。
            msgbox.Show("reg.msg3");
        }
    }
}
