using UnityEngine;
using UnityEngine.Localization.Components;

public class Msgbox : MonoBehaviour
{
    public LocalizeStringEvent m_Localize;
    // Start is called before the first frame update
    void Start()
    {
        //m_Localize=transform.Find("msg").GetComponent<LocalizeStringEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(string msg)
    {
        m_Localize.SetEntry(msg);
        gameObject.SetActive(true);
    }
    public void OnBtnOK()
    {
        gameObject.SetActive(false);
    }
}
