using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour
{
    private static Global instance;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
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

    public void ChangeColor(Text text)
    {
        text.color = Color.blue;
    }
}
