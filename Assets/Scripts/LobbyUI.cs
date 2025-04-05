using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Localization.Components;
using Newtonsoft.Json.Bson;

public class LobbyUI : NetworkBehaviour
{
    public NetworkVariable<short> m_Online = new NetworkVariable<short>();
    public LocalizeStringEvent m_Localize;
    public NetworkVariable<RoomList> m_list = new NetworkVariable<RoomList>();
    public RoomListUI ui;
    // Start is called before the first frame update
    void Start()
    {
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
        }
    }

    public override void OnNetworkDespawn()
    {
        if (Global.Singleton.net.IsClient)
        { 
            m_Online.OnValueChanged -= OnOnlineChanged;
            m_list.OnValueChanged -= OnRoomListChanged;
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

    public void OnBtnCancel()
    {
        LogoutServerRpc();
        Global.Singleton.net.SceneManager.ClientSwitchScene("Main");
    }

    public void OnBtnCreate()
    {
        Global.Singleton.net.SceneManager.ClientSwitchScene("Room");
    }
}
