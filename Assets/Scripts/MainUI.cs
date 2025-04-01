using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameDB;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainUI : NetworkBehaviour
{
    public InputField user;
    public InputField pass;
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

    public void OnBtnExit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        EditorApplication.isPaused = false;
        EditorApplication.isPlaying = false;
        #endif
    }
    public override void OnNetworkSpawn()
    {
        
    }
    public void OnBtnReg()
    {
        Global.Singleton.net.SceneManager.ClientSwitchScene("Reg");
    }

    public void OnBtnLogin()
    {
        //用户名不能为空
        if (user.text.Trim() == "")
        {
            msgbox.Show("reg.msg5");
            return;
        }
        //密码必须大于6位
        if (pass.text.Trim().Length < 6)
        {
            msgbox.Show("reg.msg6");
            return;
        }
        LoginServerRpc(user.text, pass.text);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoginServerRpc(string user, string pass, ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        clientRpcParams.Send.TargetClientIds = new ulong[] { id };

        SqliteDB db = new SqliteDB();
        db.OpenDB();
        if (db.LoginUser(user, pass))
        {
            if (Global.Singleton.UserHaveLogin(user))//重复登录
                ResClientRpc(-2, clientRpcParams);
            else
            {
                Player player = new Player();
                player.user = user;
                player.id = id;
                player.bLogin = true;
                player.sta = Player.status.Idle;
                Global.Singleton.players.Add(id, player);
                Debug.LogFormat("玩家:{0}登录成功", user);
                ResClientRpc(0, clientRpcParams);
            }
        }
        else
            ResClientRpc(-1, clientRpcParams);
        db.CloseDB();
    }

    [ClientRpc]
    public void ResClientRpc(int status, ClientRpcParams clientRpcParams = default)
    {
        if (status == 0)//登陆成功
            Global.Singleton.net.SceneManager.ClientSwitchScene("PlayScene");
        else if (status == -1)//验证失败
            msgbox.Show("main.msg1");
        else//重复登陆
            msgbox.Show("main.msg2");
    }
}
