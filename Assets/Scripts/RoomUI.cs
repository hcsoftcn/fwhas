using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RoomUI : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void LeaveRoomServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        Global.Singleton.net.SceneManager.SvrUnloadScene("Room_" + Global.Singleton.players[id].user);
    }

    public void OnBtnCancel()
    {
        LeaveRoomServerRpc();
        Global.Singleton.net.SceneManager.ClientSwitchScene("Lobby");
    }
}
