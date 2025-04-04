using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListUI : MonoBehaviour
{
    public GameObject prefab;
    public Transform parent;
    public Transform selectbar;
    public int selectIndex;
    private int oldindex;
    private int maxcount;
    // Start is called before the first frame update
    void Start()
    {
        selectIndex = 0;
        maxcount = 0;
    }

    public void UpdateView(RoomList list)
    {
        if (list.list == null) return;
        int i = 0;
        foreach(Room room in list.list)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = i.ToString();
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            Text t = obj.GetComponent<Text>();
            t.text = room.username+"的房间("+room.list.Count+"/"+room.maxcount+")";
            Button btn = obj.GetComponent<Button>();
            int index = i;
            btn.onClick.AddListener(() => OnListItemClicked(index));
            i++;
        }
        maxcount = i;
        UpdateSelectBar();
    }

    void UpdateSelectBar()
    {
        if (oldindex != selectIndex)
        {
            selectbar.GetComponent<RectTransform>().localPosition = new Vector3(0, (selectIndex + 1) * -32 - 8, 0);
            oldindex = selectIndex;
        }
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
        if (Input.GetKeyDown(KeyCode.W))   selectIndex--;
        if (Input.GetKeyDown(KeyCode.S))   selectIndex++;
        if (selectIndex < 0) selectIndex = 0;
        if (selectIndex > maxcount - 1) selectIndex = maxcount - 1;

        UpdateSelectBar();
    }
}
