using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Global : NetworkBehaviour
{
    public Config config;
    private static Global instance;
    // Start is called before the first frame update
    void Start()
    {
        config.curLocale= PlayerPrefs.GetInt("curLocale");
        DontDestroyOnLoad(gameObject);
        if (config.startType == Config.StartType.Client)
        {
            Debug.Log("Start");
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                NetworkManager.Singleton.StartClient();
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            }
        }
        else
        {
            NetworkManager.Singleton.StartServer();
            SceneEventProgressStatus sta=NetworkManager.Singleton.SceneManager.LoadScene("Main", LoadSceneMode.Additive);
            Debug.Log("loadscene : Main");
            StartCoroutine(WaitAndPrint(3.0f));
            
            
        }
    }

    IEnumerator WaitAndPrint(float waitTime)
    {
        Debug.Log("begin wait");
        yield return new WaitForSeconds(waitTime); // 等待指定的秒数
        Debug.Log("end wait");
        NetworkManager.Singleton.SceneManager.LoadScene("Reg", LoadSceneMode.Additive);
        Debug.Log("loadscene : Reg");
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

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("OnNetworkDespawn");
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.Shutdown();

        base.OnNetworkDespawn();
    }

    public override void OnDestroy()
    {
        Debug.Log("OnDestroy");
        PlayerPrefs.SetInt("curLocale", config.curLocale);
        base.OnDestroy();
    }

    void OnClientConnectedCallback(ulong id)
    {
        Debug.LogFormat("OnConnected {0}",id);
    }
}
