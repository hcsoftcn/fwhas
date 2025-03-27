using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainUI : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnExit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        EditorApplication.isPaused = false;
        EditorApplication.isPlaying = false;
        #endif
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("Main UI Spawned");
    }
    public void OnBtnReg()
    {
        //测试中文编码
        //SceneManager.LoadScene("Reg");
        //Debug.Log("OnBtnReg");
        //Global.Singleton.SwitchScene("Reg");
        NetworkManager.Singleton.SceneManager.ClientSwitchScene("Reg");
    }

    public void OnBtnLogin()
    {
        //Debug.Log("OnBtnLogin");
        //Global.Singleton.SwitchScene("PlayScene");
    }
}
