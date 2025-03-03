using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class TestSqlite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.streamingAssetsPath + "/data.db";
        SQLiteConnection connection = new SQLiteConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // 创建表
        //connection.CreateTable<TestTable>();

        // 插入数据
        UserTbl table = new UserTbl
        {
            UserName = "Example",
            Password = "password",
            RegDate = System.DateTime.Now
        };
        connection.Insert(table);

        // 查询数据
        var query = connection.Table<UserTbl>().Where(v => v.UserName == "Example");
        foreach (var item in query)
        {
            Debug.Log(item.UserName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class UserTbl
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public System.DateTime RegDate { get; set; }
}