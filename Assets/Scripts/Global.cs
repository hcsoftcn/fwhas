using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    public Config config;
    private static Global instance;
    // Start is called before the first frame update
    void Start()
    {
        config.curLocale= PlayerPrefs.GetInt("curLocale");
        DontDestroyOnLoad(gameObject);
        if (config.startType == Config.StartType.Client)
            SceneManager.LoadScene("Main");
        else if(config.startType == Config.StartType.Host)
        {
            NetworkManager.Singleton.StartHost();
            SceneManager.LoadScene("Main");
        }
        else
            NetworkManager.Singleton.StartServer();
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

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("curLocale", config.curLocale);
    }
}
