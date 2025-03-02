using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class SetLocal : MonoBehaviour
{
    private Locale chineseLocale;
    private Locale englishLocale;
    // Start is called before the first frame update
    void Start()
    {
        GetAllLocale();
    }

    void GetAllLocale()
    {
        //获取
        var locales = LocalizationSettings.AvailableLocales.Locales;

        for (int i = 0; i < locales.Count; ++i)
        {
            var locale = locales[i];

            //获取语言环境并赋值
            switch (locale.LocaleName)
            {
                case "Chinese (Simplified) (zh-Hans)":
                    chineseLocale = locale;
                    break;
                case "English (en)":
                    englishLocale = locale;
                    break;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectChange(Int32 n)
    {
        if(n==0)
            LocalizationSettings.Instance.SetSelectedLocale(chineseLocale);
        else if(n==1)
            LocalizationSettings.Instance.SetSelectedLocale(englishLocale);
    }

}
