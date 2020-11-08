using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// desc : 大地图的选择 
/// 
/// 1.大地图是否可以选择的要求 ： 玩家星星总和是否达到 要求
/// ###需要存储的数据（全局）：
///    * 玩家每一关星星的数量（这个数量影响下一关能否开启 ， 以及level界面星星个数的显示（更新） 名称为 "level + num"）
///    * 星星的总和 totalNum
///    * 关卡的名字 （更加载相关）
///
///
/// 整体思路：
/// 1.每一个 Map 设置一个starNum 。当全局变量星星总数（totalNum） 
/// 2. bool isSelect 。玩家星星总数超过了 这个starNum ，isSelect = true
/// 3. 写一个方法注册进 Map的点击 事件 （Selected），当isSelect ，则点击后显示关卡选择界面
/// </summary>
public class MapSelect : MonoBehaviour
{
    public int starsNum ;
    private bool isSelect = false;

    private GameObject locks;
    private GameObject stars;

    public GameObject panel;
    private GameObject map;
    private Button returnBtn;
    public int mapID;
    //当前一共的星星数量
    private int totalNum;
    private int totalNumLevel = 14;
    private void Awake(){
        locks = transform.Find("Image/lock").gameObject;
        stars = transform.Find("Image/scorces").gameObject;
        map   = transform.parent.gameObject;
        returnBtn = panel.transform.Find("ReturnBtn").GetComponent<Button>();
        returnBtn.onClick.AddListener(ReturnBtnCallBack);
    }
    private void Start()
    {
        Debug.Log("当前场景的星星总数"+PlayerPrefs.GetInt("totalNum", 0) );
        //所有的星星数量
        if (PlayerPrefs.GetInt("totalNum", 0) >= starsNum)
        {
            isSelect = true;
        }
        if (isSelect)
        {
            locks.SetActive(false);
            stars.SetActive(true);
            ShowStarsNum(mapID);
            //todo :显示 星星数目的text
        }
    }
    /// <summary>
    /// 鼠标点机
    /// </summary>
    public void Selected()
    {
        if (isSelect)
        {
            panel.SetActive(true );
            map.SetActive(false);
            //todo :选中时需要 记录当前选择的是哪个map
            
        }
    }

    void ShowStarsNum(int mapID){
        for(int i = 1;i<=14;i++){
            if(PlayerPrefs.GetInt("level"+(i + i*mapID).ToString())!= 0)
            {
                Debug.Log("关卡："+ (i+totalNumLevel*mapID) +"的星星数量为" +PlayerPrefs.GetInt((i+totalNumLevel*mapID).ToString()  ));
                totalNum += PlayerPrefs.GetInt((i+ totalNumLevel * mapID).ToString());
            }
            else{     
                stars.GetComponent<Text>().text = string.Concat(totalNum.ToString()," / 42" );
                break;
            }
        }
    }

    void ReturnBtnCallBack(){
        panel.SetActive(false);
        map.SetActive(true);
    }
}
