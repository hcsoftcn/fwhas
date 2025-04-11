using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Localization.Components;

public class RoomUI : NetworkBehaviour
{
    public NetworkVariable<Room> m_room = new NetworkVariable<Room>();
    public NetworkVariable<bool> IsRoomHaveOwner = new NetworkVariable<bool>();
    public LocalizeStringEvent m_Localize;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
    }
    public override void OnNetworkSpawn()
    {
        if (Global.Singleton.net.IsClient)
        {
            if (!IsRoomHaveOwner.Value)
                SetRoomOwnerServerRpc();
        }

        if (Global.Singleton.net.IsClient)
        {
            if (m_Localize.StringReference.Arguments == null)
                m_Localize.StringReference.Arguments = new object[] { m_room.Value.username };
            m_Localize.StringReference.StringChanged += OnStringChanged;
            m_Localize.RefreshString();
            m_room.OnValueChanged += OnRoomValueChanged;
        }
    }
    public override void OnNetworkDespawn()
    { 

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
    public void LeaveRoomServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        if (id == m_room.Value.id)
        {
            Global.Singleton.lobby.m_list.Value.list.Remove(m_room.Value);
            Global.Singleton.lobby.m_list.SetDirty(true);
            Global.Singleton.net.SceneManager.SvrUnloadScene("Room_" + Global.Singleton.players[id].user);
        }
        else
        {
            m_room.Value.list.Remove(id);
            m_room.SetDirty(true);
            for (int i=0;i< Global.Singleton.lobby.m_list.Value.list.Count;i++)
            {
                if (Global.Singleton.lobby.m_list.Value.list[i].id == m_room.Value.id)
                {
                    Global.Singleton.lobby.m_list.Value.list[i] = m_room.Value;
                    Global.Singleton.lobby.m_list.SetDirty(true);
                    break;
                }
            }
        }
    }

    public void OnBtnCancel()
    {
        LeaveRoomServerRpc();
        Global.Singleton.net.SceneManager.ClientSwitchScene("Lobby");
    }
}
