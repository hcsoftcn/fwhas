using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class GameUI : NetworkBehaviour
{
    public List<Transform> list=new List<Transform>();
    public NetworkVariable<Room> m_Game = new NetworkVariable<Room>();
    public List<Text> players = new List<Text>();
    public GameObject prefab;
    private Dictionary<ulong,NetworkObject> obj=new Dictionary<ulong, NetworkObject>();

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
        //Debug.Log("UpdatePlayers");
        for (int i = 0; i < 3; i++)
        {
            players[i].text = "";
        }

        for (int i = 0; i < current.list.Count; i++)
        {
            if (current.list[i].sta == Player.status.Play)
            {
                //Debug.Log("UpdatePlayer:"+ current.list[i].user);
                players[i].text = current.list[i].user;
            }
        }
    }

    public void OnGameValueChanged(Room previous, Room current)
    {
        if (Global.Singleton.net.IsClient)
        {
            UpdatePlayers(current);
        }
    }
    public override void OnNetworkSpawn()
    {

        if (IsClient)
        {
            m_Game.OnValueChanged += OnGameValueChanged;

            JoinGameOwnerServerRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient) m_Game.OnValueChanged -= OnGameValueChanged;
    }

    public void GameShow(NetworkObject no)
    {
        for (int i = 0; i < m_Game.Value.list.Count; i++)
                no.NetworkShow(m_Game.Value.list[i].id);
    }

    public bool IsGameOwner(ulong id)
    {
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

                GameObject ob = GameObject.Instantiate(prefab);//在Active场景创建
                ob.name = p.user;
                SceneManager.MoveGameObjectToScene(ob, gameObject.scene);//迁移到当前场景
                obj.Add(id,ob.GetComponent<NetworkObject>());
                obj[id].SpawnAsPlayerObject(id,true);
                
                obj[id].GetComponent<PlayerControl>().player.Value = p;
                obj[id].GetComponent<PlayerControl>().player.SetDirty(true);
                obj[id].transform.localPosition = list[m_Game.Value.GetIndex(id)].localPosition;

                GameShow(obj[id]);

                return true;
            }
        }

        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinGameOwnerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        if (IsGameOwner(id))
        {
            for (int i = 0; i < m_Game.Value.list.Count; i++)
            {
                if (id == m_Game.Value.list[i].id) continue;
                Global.Singleton.net.SceneManager.ServerSwitchScene(m_Game.Value.list[i].id, "Game_" + m_Game.Value.username);
            }
            Global.Singleton.net.SceneManager.SvrUnloadScene("Room_" + m_Game.Value.username);
            Global.Singleton.lobby.m_list.Value.list.Remove(m_Game.Value);
            Global.Singleton.lobby.m_list.SetDirty(true);
        }
        else
        {
            m_Game.Value.SetStatus(id, Player.status.Play);

            Player p = Global.Singleton.players[id];
            p.SetStatus(Player.status.Play);
            Global.Singleton.players[id] = p;

            GameObject ob = GameObject.Instantiate(prefab);//在Active场景创建
            ob.name = p.user;
            SceneManager.MoveGameObjectToScene(ob, gameObject.scene);//迁移到当前场景
            obj.Add(id, ob.GetComponent<NetworkObject>());
            obj[id].SpawnAsPlayerObject(id,true);
            
            obj[id].GetComponent<PlayerControl>().player.Value = p;
            obj[id].GetComponent<PlayerControl>().player.SetDirty(true);
            obj[id].transform.localPosition = list[m_Game.Value.GetIndex(id)].localPosition;

            GameShow(obj[id]);

            m_Game.SetDirty(true);
        }
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

        obj[id].Despawn();

        if (m_Game.Value.list.Count == 0)
            Global.Singleton.net.SceneManager.SvrUnloadScene("Game_"+ m_Game.Value.username);

        Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Lobby");
    }

    public void OnBtnExit()
    {
        LeaveGameServerRpc();
    }
}
