using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//desc :关卡的选择 和 加载

//整体
//1.关卡是否打开的计算：
//     上一关的获得的星星数量 >= 1 
//     

public class LevelSelect : MonoBehaviour
{
    private bool isSelect = false;
    public Sprite levelBG;
    private Image image;
    private Button btnSelf;
    //前一关
    private int preLevelNum;
    //显示的星星
    private int starNum;
    private GameObject[] stars;

    //325 是否加载异步加载场景
    private static bool  isFirstOpen = false ;
    private void Awake()
    {   
        btnSelf =this.GetComponent<Button>();
        stars = new GameObject[3];
        preLevelNum = int.Parse(gameObject.name) - 1;
        image = GetComponent<Image>();
        for(int i = 0 ;i<3;i++){
            stars[i] = transform.Find("star"+i.ToString()).gameObject;
        }
    }
    private void Start()
    {
        btnSelf.onClick.AddListener(SelectedCallBack);
        if(transform.parent.GetChild(0).name == gameObject.name)
        {
            //初始化第1关
            isSelect = true;
        }
        else{
            //假如前一关通过
            if(PlayerPrefs.GetInt(preLevelNum.ToString())> 0 ){
                isSelect =true;
            }
        }

        if (isSelect)
        {
            image.overrideSprite = levelBG;
            transform.Find("Text").gameObject.SetActive(true);
            //获得当前关卡的星星数目
            starNum = PlayerPrefs.GetInt(gameObject.name);
            Debug.Log(starNum);
            if(starNum>0){
                for(int i =0 ; i<starNum ;i++){
                    stars[i].SetActive(true);
                }
            }
        }
    }

//方法，选中方法回调
    public void SelectedCallBack()
    {
        if (isSelect)
        {
            //实例化游戏场景,把当前关卡名字存进去
            PlayerPrefs.SetString("nowLevel",gameObject.name);
            //选中时初始化这一关的星星数量
            PlayerPrefs.GetInt(gameObject.name, 0);
            //加载场景
            Debug.Log("当前选择场景为"   + gameObject.name);
            Debug.Log("这个场景的星星数" + PlayerPrefs.GetInt(gameObject.name));
            //第一次加载的时候，因为需要加载预制体所以，给他一个缓冲界面
            if (!isFirstOpen)
            {
                isFirstOpen = true;
                SceneManager.LoadScene(2);
            }
            else
            {
                SceneManager.LoadSceneAsync(1);
            }
        }
    }
}
