using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//全局事件系统声明

public class selEventSystem : MonoBehaviour
{
   public static selEventSystem EventIns;
   //游戏开始事件
   [HideInInspector]
   public UnityEvent EventGameStart;
   //玩家死亡事件
    [HideInInspector]
    public UnityEvent EventplayerDeath;
    //重玩按钮触发事件
    [HideInInspector]
    public UnityEvent EventRetryBtn;
    [HideInInspector]
    public UnityEvent EventplayerWin;
    [HideInInspector]
    public UnityEvent EventHomeBtn;
    [HideInInspector]
    public UnityEvent EventPickBullet;
    void Awake(){
        EventIns =this;
    }

}
