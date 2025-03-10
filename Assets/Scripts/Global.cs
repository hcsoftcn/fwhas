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
        config.curLocale= PlayerPrefs.GetInt("curLocale");
        DontDestroyOnLoad(gameObject);
        if (config.startType == Config.StartType.Client)
        {
            
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.Log("Start");
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.StartClient();
                
                //NetworkManager.Singleton.SceneManager.VerifySceneBeforeLoading += VerifySceneBeforeLoading;
            }
        }
        else
        {
            NetworkManager.Singleton.StartServer();
            Queue<string> scenes = new Queue<string>();
            scenes.Enqueue("Main");
            scenes.Enqueue("Reg");
            scenes.Enqueue("PlayScene");
            NetworkManager.Singleton.SceneManager.SvrLoadScenes(scenes);
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
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(curScene));
    }
}
