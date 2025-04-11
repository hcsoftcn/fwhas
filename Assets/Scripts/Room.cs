using System.Collections.Generic;
using Unity.Netcode;
public struct Room : INetworkSerializable
{
    public ulong id;//房间号，使用客户端id
    public string username;//房主的用户名
    public int maxcount;//最大人数
    public List<ulong> list;//房间内的其他玩家id
    public List<string> list1;//房间内的其他玩家的username

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
        serializer.SerializeValue(ref id);
        if (username == null) username = "";
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref maxcount);
        if (list == null)
            list = new List<ulong>();
        if (serializer.IsReader)
        {
            list.Clear();
            int count = 0;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                ulong clientid = 0;
                serializer.SerializeValue(ref clientid);
                list.Add(clientid);
            }
        }
        else
        {
            int count = list.Count;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                ulong clientid = list[i];
                serializer.SerializeValue(ref clientid);
            }
        }

        if (list1 == null)
            list1 = new List<string>();
        if (serializer.IsReader)
        {
            list1.Clear();
            int count = 0;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                string user = "";
                serializer.SerializeValue(ref user);
                list1.Add(user);
            }
        }
        else
        {
            int count = list1.Count;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                string user = list1[i];
                serializer.SerializeValue(ref user);
            }
        }
    }

}