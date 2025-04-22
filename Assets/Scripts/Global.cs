using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class Global : MonoBehaviour
{
    public Config config;
    public NetworkManager net;
    public Dictionary<ulong,Player> players;//服务端玩家字典
    private static Global instance;
    public LobbyUI lobby;
    // Start is called before the first frame update
    void Start()
    {
        //测试中文编码
        players = new Dictionary<ulong, Player>();
     
        config.curLocale= PlayerPrefs.GetInt("curLocale");
        DontDestroyOnLoad(gameObject);
        net = NetworkManager.Singleton;
        if (config.startType == Config.StartType.Client)
        {
            
            if (!net.IsConnectedClient)
            {
                net.StartClient();
                net.OnClientConnectedCallback += OnClientConnected;
            }
        }
        else
        {
            net.StartServer();
            net.SceneManager.OnServerSceneLoadComplete += OnServerSceneLoadComplete;
            net.SceneManager.SvrLoadScene("Active");//活动空场景，用于创建游戏对象
            net.SceneManager.SvrLoadScene("Main");
            net.SceneManager.SvrLoadScene("Reg");
            net.SceneManager.SvrLoadScene("Lobby");
            net.SceneManager.SetDefaultScene("Main");
            net.OnClientConnectedCallback += OnServerConnected;
            net.OnClientDisconnectCallback += OnServerDisonnected;
        }
    }

    public void OnServerSceneLoadComplete(string scname)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Active"));
        lobby = GameObject.Find("lobby").GetComponent<LobbyUI>();
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
        PlayerPrefs.SetInt("curLocale", config.curLocale);
    }

    public void OnClientConnected(ulong id)
    {
        Debug.LogFormat("OnConnected {0}",id);
    }
    public void OnServerConnected(ulong id)
    {
       
    }

    public void OnServerDisonnected(ulong id)
    {
        if (players.ContainsKey(id))
        {
            Debug.LogFormat("玩家:{0}断开连接", players[id].user);
            players.Remove(id);
        }
    }

    public bool UserHaveLogin(string user)
    {
        foreach (var player in players)
            if (player.Value.user == user) return true;
        return false;
    }
}
