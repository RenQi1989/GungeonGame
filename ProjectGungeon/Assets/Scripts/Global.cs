using UnityEngine;

public class Global
{
    // 保存重新开始游戏的数据
    public static void RestartData()
    {
        Player.HP = 3;
        Time.timeScale = 1; // 时间继续流逝
    }

}
