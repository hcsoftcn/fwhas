using Unity.Netcode;
using GameDB;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class RegUI : NetworkBehaviour
{
    public InputField user;
    public InputField pass;
    public InputField pass1;
    private Msgbox msgbox;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MsgBox.prefab");
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3(0, 0, 0);
        instance.transform.rotation = Quaternion.identity;
        msgbox = instance.GetComponent<Msgbox>();
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
            msgbox.SetMsg("reg.msg5");
            msgbox.Show(true);
            return;
        }
        //密码必须大于6位
        if(pass.text.Trim().Length<6)
        {
            msgbox.SetMsg("reg.msg6");
            msgbox.Show(true);
            return;
        }    
        //二次输入密码是否一致
        if(pass.text!=pass1.text)
        {
            msgbox.SetMsg("reg.msg2");
            msgbox.Show(true);
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
            //注册失败，用户名已经存在。
            ResClientRpc(-1);
        }
        else
        {
            if (db.RegUser(user, pass))
                ResClientRpc(0);//注册成功。
            else
                ResClientRpc(-2);//注册失败，系统故障。
        }
        db.CloseDB();
    }

    [ClientRpc]
    public void ResClientRpc(int status)
    {
        if (status == 0)
        {
            //注册成功
            msgbox.SetMsg("reg.msg4");
            msgbox.Show(true);
        }
        else if (status == -1)
        {
            //注册失败，用户名已经存在。
            msgbox.SetMsg("reg.msg1");
            msgbox.Show(true);
        }
        else
        {
            //注册失败，系统故障。
            msgbox.SetMsg("reg.msg3");
            msgbox.Show(true);
        }
    }
}
