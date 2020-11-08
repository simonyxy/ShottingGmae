using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//前言：对象池，游戏管理器，UI界面，这种都是固定存在在每一关的东西，直接写脚本管理整个游戏过程只生成一次！
//      DontDestroyOnLoad虽然可以保留物体，但是我们永远只有两个场景，每次重新进入游戏场景又会重新加载一份出来
//      因此不能在局部场景定义DontDestroyOnLoad ，需要在加载之前再在全局做一个判断，每一个物体给定一个判断符。通过全局变量去生成，不怕被销毁
//      没有就加载一个，有就不加载了.
//写一个控制生成类脚本，加一个全局bool isHave[]，每次重载场景都重载这个控制生成类脚本，查isHave。有就不搞了，没有就搞一搞
//DontDestroyOnLoad方法会重复生成物体，即使 只是在

public class SceneControl : MonoBehaviour
{
    //预制体（不销毁的物体（做成预制体））,外部载入预制体判断
    public GameObject[] DontDestoryObj; 
    //实例对象
    public static SceneControl sceneControlIns;
    //初始化判断符
    private static bool[] isHave;  
    //初始化脚本是否被打开过（打开过就不初始化isHave）
    private static bool isOpen;
    private GameObject clone;//克隆的不销毁物体
     public void Awake()
     {
        sceneControlIns =this;
        if(!isOpen) {
             Debug.Log("ISoPEN:" + isOpen);
            //1.初始化全局变量 clone + obj
            isHave = new bool[DontDestoryObj.Length];
            for(int i =0;i<DontDestoryObj.Length;i++){
                isHave[i] = false;
                Debug.Log("isHAve:" + DontDestoryObj[i].name +"   " +isHave[i] );
            }

        }
        for(int i =0;i<DontDestoryObj.Length;i++){
            if (!isHave[i])
            {
                clone = GameObject.Instantiate(DontDestoryObj[i], transform.position, transform.rotation);
                PlayerPrefs.SetInt("clone"+ DontDestoryObj[i].name,1);
                DontDestroyOnLoad(clone);//切换场景不销毁clone
                isHave[i] = true;
            }
        }
        #region 游戏加载情况说明（isOpen 的用处）
         //除了第一次打开之外，其他情况都需要我们自己手动调用游戏开启逻辑，因为GameManege保存在全局里没有重新加载，里面的方法不会去调用。我们手动调用游戏开启函数
         //如果现在是第一次加载游戏场景 isOpen 在这里为false
         //如果现在是第二此加载场景，isOpen为true ，那么就会走这个逻辑
         #endregion
        if(isOpen){
            GameManager.gameManager_ins.StartGame();
        }
        //最后再把isOpen 设为 true；
        if(!isOpen){
             isOpen = true;
        }
     }
}
