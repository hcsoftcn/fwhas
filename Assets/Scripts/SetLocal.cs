using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SetLocal : MonoBehaviour
{
    private Locale chineseLocale;
    private Locale englishLocale;
    // Start is called before the first frame update
    void Start()
    {
        var obj = GetComponent<Dropdown>();
        obj.value = Global.Singleton.config.curLocale;
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectChange(Int32 n)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        LocalizationSettings.Instance.SetSelectedLocale(locales[n]);
        Global.Singleton.config.curLocale = n;
    }

}
