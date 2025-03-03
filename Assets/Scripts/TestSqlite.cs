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
        connection.CreateTable<TestTable>();

        // 插入数据
        TestTable table = new TestTable
        {
            Name = "Example"
        };
        connection.Insert(table);

        // 查询数据
        var query = connection.Table<TestTable>().Where(v => v.Name == "Example");
        foreach (var item in query)
        {
            Debug.Log(item.Name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TestTable
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
}