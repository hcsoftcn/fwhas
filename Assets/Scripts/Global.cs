using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Global : MonoBehaviour
{
    public Config config;
    public string curScene="Main";
    private static Global instance;
    // Start is called before the first frame update
    void Start()
    {
        config.curLocale= PlayerPrefs.GetInt("curLocale");
        DontDestroyOnLoad(gameObject);
        if (config.startType == Config.StartType.Client)
        {
            //Debug.Log("Start");
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                NetworkManager.Singleton.StartClient();
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
                NetworkManager.Singleton.SceneManager.VerifySceneBeforeLoading += VerifySceneBeforeLoading;
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
        //Debug.Log("begin wait");
        yield return new WaitForSeconds(waitTime); // 等待指定的秒数
        //Debug.Log("end wait");
        NetworkManager.Singleton.SceneManager.LoadScene("Reg", LoadSceneMode.Additive);
        Debug.Log("loadscene : Reg");
        yield return new WaitForSeconds(waitTime);
        NetworkManager.Singleton.SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
        Debug.Log("loadscene : PlayScene");
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

    void OnClientConnectedCallback(ulong id)
    {
        Debug.LogFormat("OnConnected {0}",id);
    }

    bool VerifySceneBeforeLoading(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
    {
        if(sceneName== curScene) return true;
        else return false;
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(curScene);
        curScene = sceneName;
        Player.Singleton.UpdateScenesServerRpc(NetworkManager.Singleton.LocalClientId);
    }
}
