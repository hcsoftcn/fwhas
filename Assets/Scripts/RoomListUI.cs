using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class RoomListUI : MonoBehaviour
{
    public GameObject prefab;
    public Transform parent;
    public Transform selectbar;
    public int selectIndex;
    private int maxcount;
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        selectIndex = 0;
        maxcount = 0;
        pos = new Vector3(0,0,0);
    }

    public void UpdateView(RoomList list)
    {
        if (list.list == null)
        {
            UpdateSelectBar();
            return;
        }

        for (int j=0;j< parent.childCount;j++)
        {
            Destroy(parent.GetChild(j).gameObject);
        }
        
        int i = 0;

        foreach (Room room in list.list)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = i.ToString();
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            LocalizeStringEvent ev = obj.GetComponent<LocalizeStringEvent>();
            if (ev.StringReference.Arguments == null)
                ev.StringReference.Arguments = new object[] { room.username,room.list.Count,room.maxcount };
            ev.RefreshString();
            ev.GetComponent<Text>().text = ev.StringReference.GetLocalizedString();
            Button btn = obj.GetComponent<Button>();
            int index = i;
            btn.onClick.AddListener(() => OnListItemClicked(index));
            i++;
        }
        maxcount = i;
        if (selectIndex > maxcount - 1) selectIndex = maxcount - 1;
        if (selectIndex < 0) selectIndex = 0;
        UpdateSelectBar();
    }

    void UpdateSelectBar()
    {
        pos.y = (selectIndex + 1) * -32.0f - 8.0f;
        selectbar.GetComponent<RectTransform>().localPosition = pos;
    }
    public void OnListItemClicked(int index)
    {
        Debug.LogFormat("OnListItemClicked:{0}", index);
        selectIndex = index;
        UpdateSelectBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectIndex--;
            if (selectIndex < 0) selectIndex = 0;
            UpdateSelectBar();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            selectIndex++;
            if (selectIndex > maxcount - 1) selectIndex = maxcount - 1;
            UpdateSelectBar();
        }
    }
}
