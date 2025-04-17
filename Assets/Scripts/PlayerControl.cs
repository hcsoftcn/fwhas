using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerControl : NetworkBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (IsLocalPlayer)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            moveDirection = new Vector3(horizontal, vertical, 0);
            if (moveDirection != Vector3.zero)
            {
                offset = moveDirection * moveSpeed * Time.deltaTime;
                Debug.LogFormat("Move :{0}", offset);
                MoveServerRpc(offset);
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void MoveServerRpc(Vector3 offset,ServerRpcParams serverRpcParams = default)
    {
        //ulong id = serverRpcParams.Receive.SenderClientId;
        //if (!Global.Singleton.net.ConnectedClients.ContainsKey(id)) return;
        transform.Translate(offset);
        Debug.LogFormat("Move :{0}", offset);
    }
}
