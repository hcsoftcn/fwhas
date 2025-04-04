using System.Collections.Generic;
using Unity.Netcode;

public struct RoomList : INetworkSerializable
{
    public List<Room> list;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (list == null)
            list = new List<Room>();

        if (serializer.IsReader)
        {
            list.Clear();
            int count = 0;
            serializer.SerializeValue(ref count);
            
            for (int i = 0; i < count; i++)
            {
                Room room = new Room();
                room.NetworkSerialize(serializer);
                list.Add(room);
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
}