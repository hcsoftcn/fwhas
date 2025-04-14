using Unity.Netcode;
public struct Player : INetworkSerializable
{
    public enum status
        {
            Idle,//空闲
            InRoom,//在房间里
            Ready,//已经准备好
            Play//正在游戏
        }
    public string user;//用户名
    public status sta;//玩家状态
    public ulong id;//网络id

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (user == null) user = "";
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref sta);
        serializer.SerializeValue(ref user);
    }

    public void SetStatus(status thesta)
    {
        sta = thesta;
    }
}
