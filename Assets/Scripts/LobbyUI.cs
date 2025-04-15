using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Localization.Components;
using System.Collections.Generic;

public class LobbyUI : NetworkBehaviour
{
    public NetworkVariable<short> m_Online = new NetworkVariable<short>();
    public LocalizeStringEvent m_Localize;
    public NetworkVariable<RoomList> m_list = new NetworkVariable<RoomList>();
    public RoomListUI ui;

    private Msgbox msgbox;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MsgBox");
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3(0, 0, 0);
        instance.transform.rotation = Quaternion.identity;
        msgbox = instance.GetComponent<Msgbox>();

        if (Global.Singleton.net.IsClient)
        {
            VerifyServerRpc();
        }
    }
    public override void OnNetworkSpawn()
    {
        if (Global.Singleton.net.IsClient)
        {
            if (m_Localize.StringReference.Arguments == null)
                m_Localize.StringReference.Arguments = new object[] { m_Online.Value };
            m_Localize.StringReference.StringChanged += OnStringChanged;
            m_Localize.RefreshString();
            m_Online.OnValueChanged += OnOnlineChanged;
            m_list.OnValueChanged += OnRoomListChanged;
            ui.UpdateView(m_list.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (Global.Singleton.net.IsClient)
        { 
            m_Online.OnValueChanged -= OnOnlineChanged;
            m_list.OnValueChanged -= OnRoomListChanged;
            m_Localize.StringReference.StringChanged -= OnStringChanged;
        }
    }

    public void OnRoomListChanged(RoomList previous, RoomList current)
    {
        if (Global.Singleton.net.IsClient)
        {
            ui.UpdateView(current);
        }
    }
    public void OnOnlineChanged(short previous, short current)
    {
        if (Global.Singleton.net.IsClient)
        {
            m_Localize.StringReference.Arguments[0] = current;
            m_Localize.RefreshString();
        }
    }

    void OnStringChanged(string updatedText)
    {
        m_Localize.GetComponent<Text>().text = updatedText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void VerifyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        if (!Global.Singleton.players.ContainsKey(id))
            Global.Singleton.net.SceneManager.ServerSwitchScene(id,"Main");
    }

    [ServerRpc(RequireOwnership = false)]
    public void LogoutServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;

        if (Global.Singleton.players.ContainsKey(id))
        {
            Debug.LogFormat("玩家:{0}退出登录", Global.Singleton.players[id].user);
            Global.Singleton.players.Remove(id);
            Global.Singleton.lobby.m_Online.Value = (short)Global.Singleton.players.Count;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateRoomServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        Player p = Global.Singleton.players[id];
        p.SetStatus(Player.status.InRoom);
        Global.Singleton.players[id] = p;

        Room r = new Room();
        r.id = id;
        r.username = Global.Singleton.players[id].user;
        r.maxcount = 3;
        r.list = new List<Player>();
        r.list.Add(Global.Singleton.players[id]);
        m_list.Value.list.Add(r);
        m_list.SetDirty(true);
        Global.Singleton.net.SceneManager.SvrCreateAndMergeScene(id, "Room_"+ Global.Singleton.players[id].user,"Room",new Vector3(0,0,0));
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinRoomServerRpc(int index,ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        if (m_list.Value.list[index].list.Count < m_list.Value.list[index].maxcount)
        {
            Player p = Global.Singleton.players[id];
            p.SetStatus(Player.status.InRoom);
            Global.Singleton.players[id] = p;
            m_list.Value.list[index].list.Add(Global.Singleton.players[id]);
            m_list.SetDirty(true);
            Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Room_" + m_list.Value.list[index].username);
        }
    }
    public void OnBtnCancel()
    {
        LogoutServerRpc();
        Global.Singleton.net.SceneManager.ClientSwitchScene("Main");
    }

    public void OnBtnCreate()
    {
        CreateRoomServerRpc();
    }

    public void OnBtnJoin()
    {
        if (ui.selectIndex >= 0 && ui.selectIndex <= m_list.Value.list.Count - 1)
        {
            if (m_list.Value.list[ui.selectIndex].list.Count < m_list.Value.list[ui.selectIndex].maxcount)
                JoinRoomServerRpc(ui.selectIndex);
            else
                msgbox.Show("lobby.roomfull");
        }
    }

}
