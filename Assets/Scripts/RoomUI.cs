using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Localization.Components;
using System;

public class RoomUI : NetworkBehaviour
{
    public NetworkVariable<Room> m_room = new NetworkVariable<Room>();
    public NetworkVariable<bool> IsRoomHaveOwner = new NetworkVariable<bool>();
    public LocalizeStringEvent m_Localize;
    public List<Text> list=new List<Text>();
    public List<LocalizeStringEvent> list1 = new List<LocalizeStringEvent>();
    public Toggle ready;
    public Button start;

    private Msgbox msgbox;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MsgBox");
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3(0, 0, 0);
        instance.transform.rotation = Quaternion.identity;
        msgbox = instance.GetComponent<Msgbox>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRoom(Room current)
    {
        for (int i = 0; i < 3; i++)
        {
            list[i].text = "";
            list1[i].SetEntry("room.notready");
        }
 
        for (int i = 0; i < current.list.Count; i++)
        {
            list[i].text = current.list[i].user;
            if (current.list[i].sta == Player.status.Ready)
            {
                list1[i].SetEntry("room.ready");
            }
        }
        if (current.id == Global.Singleton.net.LocalClientId)
            start.gameObject.SetActive(true);
    }
    void OnStringChanged(string updatedText)
    {
        m_Localize.GetComponent<Text>().text = updatedText;
    }
    public void OnRoomValueChanged(Room previous, Room current)
    {
        if (Global.Singleton.net.IsClient)
        {
            m_Localize.StringReference.Arguments[0]= current.username;
            m_Localize.RefreshString();
            UpdateRoom(current);
        }
    }
    public override void OnNetworkSpawn()
    {
        if (Global.Singleton.net.IsClient)
        {
            if (m_Localize.StringReference.Arguments == null)
                m_Localize.StringReference.Arguments = new object[] { m_room.Value.username };
            m_Localize.StringReference.StringChanged += OnStringChanged;
            m_Localize.RefreshString();
            m_room.OnValueChanged += OnRoomValueChanged;

            if (!IsRoomHaveOwner.Value)
                 SetRoomOwnerServerRpc();
            else
                 JoinRoomServerRpc();
        }
    }
    public override void OnNetworkDespawn()
    {
        m_Localize.StringReference.StringChanged -= OnStringChanged;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRoomOwnerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        foreach(Room r in Global.Singleton.lobby.m_list.Value.list)
        {
            if (r.id == id)
            {
                m_room.Value = r;
                m_room.SetDirty(true);
                IsRoomHaveOwner.Value = true;
                break;
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinRoomServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        foreach (Room r in Global.Singleton.lobby.m_list.Value.list)
        {
            if (r.id == m_room.Value.id)
            {
                m_room.Value = r;
                m_room.SetDirty(true);
                
                break;
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveRoomServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        
        if (id == m_room.Value.id)
        {
            Global.Singleton.lobby.m_list.Value.list.Remove(m_room.Value);
            Global.Singleton.lobby.m_list.SetDirty(true);
            for (int i = 0; i < m_room.Value.list.Count; i++)
            {
                if (id != m_room.Value.list[i].id)
                {
                    Player  p = Global.Singleton.players[id];
                    p.SetStatus(Player.status.Idle);
                    Global.Singleton.players[id] = p;
                    Global.Singleton.net.SceneManager.ServerSwitchScene(m_room.Value.list[i].id, "Lobby");
                }
            }
            Global.Singleton.net.SceneManager.SvrUnloadScene("Room_" + Global.Singleton.players[id].user);
            Player p1 = Global.Singleton.players[id];
            p1.SetStatus(Player.status.Idle);
            Global.Singleton.players[id] = p1;
            Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Lobby");
        }
        else
        {
            m_room.Value.list.Remove(Global.Singleton.players[id]);
            m_room.SetDirty(true);
            Global.Singleton.lobby.m_list.Value.UpdateRoom(m_room.Value);
            Global.Singleton.lobby.m_list.SetDirty(true);
            Player p = Global.Singleton.players[id];
            p.SetStatus(Player.status.Idle);
            Global.Singleton.players[id] = p;
            Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Lobby");
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetStatusServerRpc(bool isOn,ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        if (!isOn)
        {
            Player p = Global.Singleton.players[id];
            p.SetStatus(Player.status.InRoom);
            Global.Singleton.players[id] = p;
            m_room.Value.SetStatus(id,Player.status.InRoom);
            Global.Singleton.lobby.m_list.Value.UpdateRoom(m_room.Value);
        }
        else
        {
            Player p = Global.Singleton.players[id];
            p.SetStatus(Player.status.Ready);
            Global.Singleton.players[id] = p;
            m_room.Value.SetStatus(id, Player.status.Ready);
            Global.Singleton.lobby.m_list.Value.UpdateRoom(m_room.Value);
        }
        
        m_room.SetDirty(true);
        Global.Singleton.lobby.m_list.SetDirty(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        Global.Singleton.net.SceneManager.SvrCreateAndMergeScene(id, "Game_" + Global.Singleton.players[id].user, "Game", new Vector3(0, 0, 0));
    }
    public void OnBtnCancel()
    {
        LeaveRoomServerRpc();
    }

    public void OnStatusChanged()
    {
        SetStatusServerRpc(ready.isOn);
    }

    public bool IsAllReady()
    {
        for (int i = 0; i < m_room.Value.list.Count; i++)
        {
            if (m_room.Value.list[i].sta != Player.status.Ready)
            {
                return false;
            }
        }
        return true;
    }
    public void OnBtnStart()
    {
        if (!IsAllReady())
            msgbox.Show("room.notallready");
        else
            StartGameServerRpc();
    }
}
