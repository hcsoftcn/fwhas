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
    public List<Text> list=new List<Text>();
    //private ClientRpcParams clientRpcParams;
    // Start is called before the first frame update
    void Start()
    {
        //clientRpcParams = new ClientRpcParams();
        //clientRpcParams.Send = new ClientRpcSendParams();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRoom(Room current)
    {
        for (int i = 0; i < 3; i++)
            list[i].text = "";

        for (int i = 0; i < current.list1.Count; i++)
            list[i].text = current.list1[i];
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
                 JoinRoomOwnerServerRpc();
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
    public void JoinRoomOwnerServerRpc(ServerRpcParams serverRpcParams = default)
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
                if(id!= m_room.Value.list[i])
                      Global.Singleton.net.SceneManager.ServerSwitchScene(m_room.Value.list[i],"Lobby");
            }
            Global.Singleton.net.SceneManager.SvrUnloadScene("Room_" + Global.Singleton.players[id].user);
            Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Lobby");
        }
        else
        {
            m_room.Value.list.Remove(id);
            m_room.Value.list1.Remove(Global.Singleton.players[id].user);
            m_room.SetDirty(true);
            //ulong []targs= new ulong[m_room.Value.list.Count];
            //for (int i = 0; i < m_room.Value.list.Count; i++)
            //    targs[i] = m_room.Value.list[i];
            //clientRpcParams.Send.TargetClientIds = targs;

            for (int i=0;i< Global.Singleton.lobby.m_list.Value.list.Count;i++)
            {
                if (Global.Singleton.lobby.m_list.Value.list[i].id == m_room.Value.id)
                {
                    Global.Singleton.lobby.m_list.Value.list[i] = m_room.Value;
                    Global.Singleton.lobby.m_list.SetDirty(true);
                    break;
                }
            }
            
            //UpdateRoomClientRpc(m_room.Value, clientRpcParams);
            Global.Singleton.net.SceneManager.ServerSwitchScene(id, "Lobby");
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void UpdateRoomClientRpc(Room room,ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("UpdateRoomClientRpc");
        UpdateRoom(room);
    }
    public void OnBtnCancel()
    {
        LeaveRoomServerRpc();
    }
}
