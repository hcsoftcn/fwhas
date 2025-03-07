using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    private static Player instance;
    public static Player Singleton
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScenesServerRpc(ulong id)
    {
        //NetworkManager.Singleton.SceneManager.SynchronizeNetworkObjects(id);
        Debug.Log("UpdateScenesServerRpc");
    }

}
