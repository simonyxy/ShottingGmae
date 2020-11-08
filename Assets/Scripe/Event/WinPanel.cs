using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//放在winPanel下，当胜利时去调用方法显示星星
public class WinPanel : MonoBehaviour
{
    /// <summary>
    /// 播放玩UI显示动画后显示星星
    /// </summary>
    public void ShowStarts()
    {
        GameManager.gameManager_ins.WinShowStars();
    }
}
