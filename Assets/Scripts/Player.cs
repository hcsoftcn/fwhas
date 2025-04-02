public class Player
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
}
