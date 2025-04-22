using System.Collections.Generic;
using Unity.Netcode;
public struct Room : INetworkSerializable
{
    public ulong id;//房间号，使用客户端id
    public string username;//房主的用户名
    public int maxcount;//最大人数
    public List<Player> list;//房间内的其他玩家id

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
        serializer.SerializeValue(ref id);
        if (username == null) username = "";
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref maxcount);
        if (list == null)
            list = new List<Player>();
        if (serializer.IsReader)
        {
            list.Clear();
            int count = 0;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                Player player=new Player();
                player.NetworkSerialize(serializer);
                list.Add(player);
            }
        }
        else
        {
            int count = list.Count;
            serializer.SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                list[i].NetworkSerialize(serializer);
            }
        }

    }
    public void SetStatus(ulong id,Player.status thesta)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].id == id)
            {
                Player p = list[i];
                p.SetStatus(thesta);
                list[i] = p;
                break;
            }
        }
    }

    public int GetIndex(ulong id)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].id == id)
            {
                return i;
            }
        }
        return -1;
    }
}