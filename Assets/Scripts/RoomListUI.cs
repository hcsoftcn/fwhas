using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class RoomListUI : MonoBehaviour
{
    public GameObject prefab;
    public Transform parent;
    public GameObject selectbarprefab;
    public RectTransform selectbar;
    public int selectIndex;
    private int maxcount;
    private Vector3 pos;
    private Vector3 scale;

    // Start is called before the first frame update
    void Awake()
    {
        selectIndex = -1;
        maxcount = 0;
        pos = new Vector3(-109,-17,0);
        scale = new Vector3(1, 1, 1);
    }

    public void UpdateView(RoomList list)
    {
        for (int j = 0; j < parent.childCount; j++)
        {
            Destroy(parent.GetChild(j).gameObject);
        }

        if (list.list == null|| list.list.Count==0)
        {
            selectIndex = -1;
            maxcount = 0;
            
            return;
        }

        maxcount = list.list.Count;
        if (selectIndex > maxcount - 1) selectIndex = maxcount - 1;
        if (selectIndex < 0) selectIndex = 0;

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

            if(selectIndex==i)
            {
                selectbar = GameObject.Instantiate(selectbarprefab).GetComponent<RectTransform>();
                selectbar.SetParent(obj.transform);
                selectbar.localPosition = pos;
                selectbar.localScale = scale;
            }
            i++;
        }
    }

    void UpdateSelectBar()
    {
        if (parent.childCount == 0) return;
        if (selectIndex > maxcount - 1) selectIndex = maxcount - 1;
        if (selectIndex < 0) selectIndex = 0;
        selectbar.SetParent(parent.GetChild(selectIndex));
        selectbar.localPosition = pos;
        selectbar.localScale = scale;
    }
    public void OnListItemClicked(int index)
    {
        //Debug.LogFormat("OnListItemClicked:{0}", index);
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
