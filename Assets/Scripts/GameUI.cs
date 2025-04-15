using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class GameUI : NetworkBehaviour
{
    public NetworkVariable<Room> m_Game = new NetworkVariable<Room>();
    public NetworkVariable<bool> IsGameHaveOwner = new NetworkVariable<bool>();
    public List<Text> players = new List<Text>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayers(Room current)
    {
        for (int i = 0; i < 3; i++)
        {
            players[i].text = "";
        }

        for (int i = 0; i < current.list.Count; i++)
        {
            if (current.list[i].sta == Player.status.Play)
            {
                players[i].text = current.list[i].user;
            }
        }
    }

    public void OnRoomValueChanged(Room previous, Room current)
    {
        if (Global.Singleton.net.IsClient)
        {
            UpdatePlayers(current);
        }
    }
    public override void OnNetworkSpawn()
    {
        if (Global.Singleton.net.IsClient)
        {
            m_Game.OnValueChanged += OnRoomValueChanged;

            if (!IsGameHaveOwner.Value)
                SetGameOwnerServerRpc();
            else
                JoinGameServerRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
 
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetGameOwnerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        foreach (Room r in Global.Singleton.lobby.m_list.Value.list)
        {
            if (r.id == id)
            {
                m_Game.Value = r;
                m_Game.Value.SetStatus(id, Player.status.Play);

                Player p = Global.Singleton.players[id];
                p.SetStatus(Player.status.Play);
                Global.Singleton.players[id] = p;

                m_Game.SetDirty(true);
                IsGameHaveOwner.Value = true;
                break;
            }
        }
        if(IsGameHaveOwner.Value)
        {
            for (int i = 0; i < m_Game.Value.list.Count; i++)
            {
                if (id == m_Game.Value.list[i].id) continue;
                Global.Singleton.net.SceneManager.ServerSwitchScene(m_Game.Value.list[i].id, "Game_" + m_Game.Value.username);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        m_Game.Value.SetStatus(id, Player.status.Play);

        Player p = Global.Singleton.players[id];
        p.SetStatus(Player.status.Play);
        Global.Singleton.players[id] = p;

        m_Game.SetDirty(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;

        m_Game.Value.list.Remove(Global.Singleton.players[id]);
        m_Game.SetDirty(true);

        Player p = Global.Singleton.players[id];
        p.SetStatus(Player.status.Idle);
        Global.Singleton.players[id] = p;

        if (m_Game.Value.list.Count == 0)
            Global.Singleton.net.SceneManager.SvrUnloadScene("Game_"+ m_Game.Value.username);

        Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Lobby");
    }

    public void OnBtnExit()
    {
        LeaveGameServerRpc();
    }
}
