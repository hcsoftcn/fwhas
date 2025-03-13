using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //测试中文编码
        //if (IsServer)
            //GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBtnCancel()
    {
        //Global.Singleton.SwitchScene("Main");
    }
}
