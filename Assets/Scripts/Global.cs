using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class Global : MonoBehaviour
{
    public Config config;
    public string curScene="Main";
    private static Global instance;
    // Start is called before the first frame update
    void Start()
    {
        //测试中文编码
        config.curLocale= PlayerPrefs.GetInt("curLocale");
        DontDestroyOnLoad(gameObject);
        if (config.startType == Config.StartType.Client)
        {
            
            if (!NetworkManager.Singleton.IsConnectedClient)
            {              
                NetworkManager.Singleton.StartClient();
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        else
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.SceneManager.SvrLoadScene("Main");
            NetworkManager.Singleton.SceneManager.SvrLoadScene("Reg");
            NetworkManager.Singleton.SceneManager.SvrLoadScene("PlayScene");
            NetworkManager.Singleton.SceneManager.SetDefaultScene("Main");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static Global Singleton
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Global>();
            }
            return instance;
        }
    }


    public void OnDestroy()
    {
        //Debug.Log("OnDestroy");
        PlayerPrefs.SetInt("curLocale", config.curLocale);
    }

    public void OnClientConnected(ulong id)
    {
        Debug.LogFormat("OnConnected {0}",id);
    }

}
