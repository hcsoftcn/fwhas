using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerControl : NetworkBehaviour
{
    public NetworkVariable<Player> player = new NetworkVariable<Player>(); 
    private float moveSpeed = 5f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 offset;

    // Start is called before the first frame update

    public override void OnNetworkSpawn()
    {
        if(IsClient) player.OnValueChanged += OnPlayerValueChanged;
        if (IsLocalPlayer)
        {
            UpdatePlayerServerRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient) player.OnValueChanged -= OnPlayerValueChanged;
    }
    void Start()
    {
        
    }
    void OnPlayerValueChanged(Player oldp, Player newp)
    {
        GetComponentInChildren<TextMeshPro>().text = newp.user;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            moveDirection = new Vector3(horizontal, vertical, 0);
            if (moveDirection != Vector3.zero)
            {
                offset = moveDirection * moveSpeed * Time.deltaTime;
                //Debug.LogFormat("Move :{0}", offset);
                MoveServerRpc(offset);
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void MoveServerRpc(Vector3 offset,ServerRpcParams serverRpcParams = default)
    {
        transform.Translate(offset);
    }
    [ServerRpc(RequireOwnership = true)]
    public void UpdatePlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(1f);
        player.SetDirty(true);
    }
}
