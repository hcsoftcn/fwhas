using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Localization.Components;


public class LobbyUI : NetworkBehaviour
{
    public NetworkVariable<short> m_Online = new NetworkVariable<short>();
    public LocalizeStringEvent m_Localize;
    private ClientRpcParams clientRpcParams;
    // Start is called before the first frame update
    void Start()
    {
        if (Global.Singleton.net.IsClient)
        {
            VerifyServerRpc();

            m_Localize.StringReference.Arguments = new object[] { m_Online.Value };
            m_Localize.StringReference.StringChanged += OnStringChanged;
            m_Localize.RefreshString();

            clientRpcParams = new ClientRpcParams();
            clientRpcParams.Send = new ClientRpcSendParams();
        }

        //GetOnlineServerRpc();
    }
    public override void OnNetworkSpawn()
    {
        if (Global.Singleton.net.IsClient)
            m_Online.OnValueChanged += OnOnlineChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (Global.Singleton.net.IsClient)
            m_Online.OnValueChanged -= OnOnlineChanged;
    }

    public void OnOnlineChanged(short previous, short current)
    {
        if (Global.Singleton.net.IsClient)
        {
            m_Localize.StringReference.Arguments = new object[] { current };
            m_Localize.RefreshString();
        }
    }

    void OnStringChanged(string updatedText)
    {
        m_Localize.GetComponent<Text>().text = updatedText;// m_Localize.StringReference.GetLocalizedString();
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

    //[ServerRpc(RequireOwnership = false)]
    //public void GetOnlineServerRpc(ServerRpcParams serverRpcParams = default)
    //{
    //    ulong id = serverRpcParams.Receive.SenderClientId;
    //    clientRpcParams.Send.TargetClientIds = new ulong[] { id };
    //    ResClientRpc(Global.Singleton.players.Count, clientRpcParams);
    //}

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

    //[ClientRpc]
    //public void ResClientRpc(int online, ClientRpcParams clientRpcParams = default)
    //{
    //    Debug.LogFormat("online:{0}", online);
    //    m_Localize.StringReference.Arguments[0] = online;
    //    m_Localize.RefreshString();
    //}

    public void OnBtnCancel()
    {
        LogoutServerRpc();
        Global.Singleton.net.SceneManager.ClientSwitchScene("Main");
    }
}
