using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region 记录游戏中的全局变量
//String :  "nowLevel"  ---  当前关卡名字
//Int    :  "totalNum"  ---  星星总数

//回家添加
//Int    :  "name?"（当前关卡名字） --- 当前关卡获得的星星
#endregion



//desc：
//GameManger 游戏管理器 ，因为这里也涉及管理 对象池Pool 和 游戏逻辑 以及 UI 。涉及到unity交互比较多，为了减轻CPU负担，注册进SceneControl的处理过的DontDestroyOnLoad()中
//1.控制生成怪物地点，和每一关生成多少怪物
//2.界面上面的UI 更新和计算
//3.通关后的逻辑

//  通过当前全局变量 控制生成怪物
//1.每一关 生成的数量 每次生成的间隔时间 (所以需要获取当前关卡，根据关卡决定生成时间)
//2.每关 生成怪物的种类 （目前有 普通僵尸 ， Boss ，警车）
//3.怪物的出生地点

//对于关卡思路：
// 1. 出生地点问题 ： 设置每一个关卡的prefab E 里面必须含有每个怪物的  ！！BornPoint！！ 代码动态获得BornPoint
// 2. 获取当前关卡 ， 用数组初始化每一个关卡的生成怪物数量和生成时间
//    写三个协程 通过三种怪物在当前关卡 的 生成时间 和数量 把它们生成
// 3. 通关后 ++currentLevel , 这一关的星星 记录在全局 PlayerPrefs 的 “level + [num] ” 记录里面
//总体的星星数目 totalNum 也增加相应的 数量


//其他逻辑
public class GameManager : MonoBehaviour
{
    //为了让这个东西在切换场景的时候不被消除
    public static GameManager gameManager_ins;
    //怪物对象池
    private EnemyPool enemy_ins;
    private Player player_ins;
    
//-------------------------Logic-----------------------//
    //倒计时
    private int countNum = 60;
    private int counttotal=60; 
    //关卡得分计算
    private int currentScore =0;
    private int currentStarts = 0;
    //事件机制
    private selEventSystem eventIns;
    //声音
    private AudioClip gameOverAudio;
    //游戏预制体
    private GameObject[] Enviroments;
    private GameObject curEnviroment;
    //玩家生成
    private Vector3 playerBornPoint;

    //-------------------------Enemy------------------------//
    //计算怪物在哪生成的
    private int policeBornPoint;
    private int enemyBornPoint;
    private int bossBornPoint;
    private int boomBornPoint;
    private int EnemyBornCount;
    //当前关卡
    private int currentLevel = 0;
    //每一关的怪物生成数量 （ 二维数组 ， [怪物][生成数量]）326
    private int[] enemyCount1 = new int[] { 1, 1, 2, 2, 3, 3, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4 };
    private int[] BossPreCount1 = new int[] { 1, 1, 2, 2, 3, 3, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4, 1, 1, 2, 2, 3, 3, 4, 4 };
    private int[] policePreCount1 = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 2, 2, 2 };

    //生成怪物的时间间隔
    private int[] intervalTime = new int[5] { 5, 4,4, 3 ,2};
    //普通僵尸
    private GameObject enemy;
    //警车
    private GameObject police;
    //326轰炸点
    private GameObject areaBoom;
    //获得出生点位置，出生点设置在其他脚本把
    private Vector3[] bornPoints = new Vector3[4];
    private Vector3[] policeBornPoints = new Vector3[4];
    private Vector3[] bossBornPoints = new Vector3[4];

    private GameObject player;
    private Vector3 levelOffSet = new Vector3(200, 0, 0); 
//----------------------UI -----------------------------------//
    //显示积分和关卡的UI界面
    private GameObject UICanvas;
    private Text levelText;
    private Text scoresText;
    private Text showScores;
    //倒计时
    private Text countDown;
    //获取胜利和失败界面UI
    private GameObject finalCanvas;
    private GameObject winPanel;
    private GameObject losePanel;
    private GameObject pausePanel;
    private GameObject[] starts = new GameObject[3] ;
    private AudioClip warningAudio;
    private Animator countDownAnim;
    private void Awake()
    {
        //防止单例销毁脚本放在SceneContol
        gameManager_ins = this;
        //获取对象池
        enemy_ins = EnemyPool.enemysPoolInstance;
        //事件机制
        eventIns = selEventSystem.EventIns;
        Enviroments = new GameObject[5];
        //初始化关卡怪物
        for(int i =0;i<= 4; i++)
        {
            Enviroments[i] = Resources.Load<GameObject>("Prefab/E/" +(i+1).ToString());
        }
        EnemyBornCount = bornPoints.Length;
        //I初始化
        finalCanvas = GameObject.Find("FinalCanvas(Clone)");
        winPanel  = finalCanvas.transform.Find("winPanel").gameObject;
        losePanel = finalCanvas.transform.Find("losePanel").gameObject;
        countDownAnim = finalCanvas.transform.Find("CountDownWarnPanel").GetComponent<Animator>();
        //胜利后的分数特效
        showScores = finalCanvas.transform.Find("winPanel/showScores").GetComponent<Text>();
        for (int i =0;i < 3 ;i++){
            starts[i] = winPanel.transform.Find(i.ToString()).gameObject ;
        }
        UICanvas = GameObject.Find("UICanvas(Clone)");
        //初始化level 
        levelText = UICanvas.transform.Find("levelText").GetComponent<Text>();
        //初始化倒计时
        countDown = UICanvas.transform.Find("countDown").GetComponent<Text>();
        //计分的面板
        scoresText = UICanvas.transform.Find("Scores").GetComponent<Text>();
        //声音
        gameOverAudio = Resources.Load<AudioClip>("Sound/game_over");
        warningAudio = Resources.Load<AudioClip>("Sound/Warning");
        //添加事件
        eventIns.EventRetryBtn.AddListener(ResetStars);
        eventIns.EventplayerDeath.AddListener(LoseLevel);
        eventIns.EventHomeBtn.AddListener(ResetStars);
    }
    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        //设置当前关卡
        currentLevel = int.Parse(PlayerPrefs.GetString("nowLevel")) ; 
        SetLevel(currentLevel);

        //先加载场景
        curEnviroment = GameObject.Instantiate(Enviroments[currentLevel %5]);
        player = GameObject.Find("Player");
        playerBornPoint = curEnviroment.transform.Find("PlayerBornPoint").position;
        player.transform.position = playerBornPoint;
        //触发事件
        eventIns.EventGameStart.Invoke();
        //重设人物血量
        player_ins = player.GetComponent<Player>();
        player_ins.currentHp = Player.playerInstance.totalHP;
        player_ins.gunType = GunEnum.normal;
        player_ins.isRun = false;
        player_ins.BoomAttackColdState = false;
        //打开暂停按钮界面
        finalCanvas.SetActive(true);
        UICanvas.SetActive(true);
        countDownAnim.SetBool("CountDownWarn", false);
        //初始化出生点位置
        for(int i = 0; i < bornPoints.Length; i++)
        {
            bornPoints[i] = curEnviroment.transform.Find("BornPoint" + i.ToString()).position;
        }
        for (int i = 0; i < policeBornPoints.Length; i++) 
        {
            policeBornPoints[i] = curEnviroment.transform.Find("carBornPoint" + i.ToString()).position;
        }
        for(int i = 0; i < bossBornPoints.Length; i++)
        {
            bossBornPoints[i] = curEnviroment.transform.Find("BossBornPoint" + i.ToString()).position;
        }
        //初始化分数
        currentScore = 0;
        scoresText.text = currentScore.ToString();
        //生成怪物
        InvokeRepeating("CreatEnemy", 3f, intervalTime[currentLevel/10]);
        InvokeRepeating("CreatPolice", 3f, intervalTime[currentLevel / 10]);
        InvokeRepeating("CountDown", 1f, 1f);
    }
    void SetLevel(int currentLevel)
    {
        levelText.text = levelText.text = "Level " + (currentLevel+1).ToString();
    }
    //倒计时
    private void CountDown()
    {
        countNum--;
        if(countDown ==null){
        }
        countDown.text = countNum.ToString();
        if(countNum == 0)
        {
            //取得关卡胜利
            WinLevel();
        }
        if(countNum == 30)
        {   //326
            CreatBoss();
            countDownAnim.SetBool("CountDownWarn", true);
            StartCoroutine("WaningAlarm");
            //326
            InvokeRepeating("AreaBoom",5f, 7f);
        }
    }

    //输了之后回调
    public void LoseLevel()
    {
        AudioSource.PlayClipAtPoint(gameOverAudio, transform.position);
        losePanel.SetActive(true);
        CancelInvoke("CreatPolice");
        CancelInvoke("CountDown");
        CancelInvoke("CreatEnemy");
        CancelInvoke("AreaBoom");
        StopAllCoroutines();
    }
    //-----------------关卡怪物生成--------------------------/
    //326
    private void AreaBoom()
    {
        StartCoroutine("AreaBoomIE");
    }
    IEnumerator AreaBoomIE()
    {
        for (int i = 0; i < 2; i++)
        {
            areaBoom = enemy_ins.GetAreaBoomObject();
            areaBoom.transform.position = player.transform.position;
            areaBoom.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }
    }
    private void CreatPolice()
    {
        for (int i = 0; i < policePreCount1[currentLevel-1]; i++)
        {
            policeBornPoint = (policeBornPoint + 1) % EnemyBornCount;
            police = enemy_ins.GetPoliceObject();
            police.transform.position = policeBornPoints[policeBornPoint];
            police.SetActive(true);
        }

    }
    //方法，生成怪物
    private void CreatEnemy()
    {
        for (int i = 0; i < enemyCount1[currentLevel-1]; i++)
        {
            enemyBornPoint = (enemyBornPoint + 1) % EnemyBornCount;
            enemy = enemy_ins.GetEnemyObject();
            enemy.transform.position = bornPoints[i];
            enemy.SetActive(true);
        }
    }

    //326 
    private void CreatBoss()
    {
        for (int i = 0; i < BossPreCount1[currentLevel-1]; i++)
        {
            bossBornPoint = (bossBornPoint + 1) % EnemyBornCount;
            enemy = enemy_ins.GetBossObject();
            enemy.transform.position = bornPoints[i];
            enemy.SetActive(true);
        }
    }
    //*****************************计算得分*****************************
    public void EnemyDeath(int Enemytype){
        if(Enemytype == 0 ){
            currentScore += 3000;
        }
        else if(Enemytype == 1){
            currentScore += 5000;
        }
        else{
            currentScore += 10000;
        }
        scoresText.text = currentScore.ToString();
    }
    //-------------------------------胜利相关----------------------
    private void WinLevel()
    {
        CancelInvoke("CreatPolice");
        CancelInvoke("CountDown");
        CancelInvoke("CreatEnemy");
        CancelInvoke("AreaBoom");
        StopAllCoroutines();
        //判断当前得分是否可以过关
        if (currentScore < 0)//改成当前关卡最小得分
        {
            //算是输了
            losePanel.SetActive(true);
        }
        //如果过关了就调用这个
        else
        {
            eventIns.EventplayerWin.Invoke();
            winPanel.SetActive(true);
        }
    }

//---------------------------UI相关-----------------------------//
    public void WinShowStars()
    {   
        //重置倒计时时间
        countNum = 60;
        //计算保存数据
        SaveData();
        //获得星星
        currentStarts= PlayerPrefs.GetInt(PlayerPrefs.GetString("nowLevel"));
        StartCoroutine("ShowStarts");
        StartCoroutine("ShowScores");
    }
    //使用协程做星星的特效展示
    IEnumerator ShowStarts(){
        for(int i = 0;i<currentStarts ;i++){
            yield return new WaitForSeconds(0.5f);
                starts[i].SetActive(true);
        }
    }
    IEnumerator ShowScores()
    {
        int showScores = 0;
        for (showScores = 0; showScores <= currentScore; showScores = showScores + 1000)
        {
            yield return new WaitForSeconds(0.03f);
            this.showScores.text = showScores.ToString();
        }
    }
    IEnumerator WaningAlarm()
    {
        for(int i = 0;i< 2; i++)
        {
            AudioSource.PlayClipAtPoint(warningAudio,playerBornPoint);
            yield return new WaitForSeconds(1.5f);
        }
    }
    private void SaveData(){
        currentStarts= PlayerPrefs.GetInt(PlayerPrefs.GetString("nowLevel"),0);
        //判断多少颗星星,协程
        if(currentScore>= 100000 )
        {
            if(currentStarts < 3){
                PlayerPrefs.SetInt(PlayerPrefs.GetString("nowLevel"),3);
            }
        }
        else if(currentScore>=50000 &&currentScore < 100000)
        {
            if(currentStarts < 2){
                PlayerPrefs.SetInt(PlayerPrefs.GetString("nowLevel"),2);
            }
        }
        else{
            if(currentStarts<1){
            PlayerPrefs.SetInt(PlayerPrefs.GetString("nowLevel"),1);
            }
        }
        //判断场景中的星星是否数量改变，改变全局数据. 总星星 = 总星星 + 这关卡获得的星星 - 原本就储存的关卡星星
        if(currentStarts < PlayerPrefs.GetInt(PlayerPrefs.GetString("nowLevel") )){
            int totaStarts = PlayerPrefs.GetInt("totalNum") + PlayerPrefs.GetInt(PlayerPrefs.GetString("nowLevel"))- currentStarts;
            PlayerPrefs.SetInt("totalNum",totaStarts);
        }
    }

    //----------------------------------------------------------------------------

    //------------------------------UI点击相关----------------------------------------
    //方法，按钮Retry注册 
    public void Retry(){
        eventIns.EventplayerDeath.Invoke();
        losePanel.SetActive(false);
        //重设人物血量
        Player.playerInstance.currentHp = Player.playerInstance.totalHP;
        countNum = counttotal;
        //触发事件
        eventIns.EventRetryBtn.Invoke();
        //curEnviroment.SetActive(false);
        //重新加载当前场景
        SceneManager.LoadScene(1);
    }
    //EventRetryBtn的注册事件
    void ResetStars(){
        for(int i = 0 ;i<currentStarts;i++ ){
            starts[i].SetActive(false);
        }
    }
    //方法，按钮返回主界面注册
    public void Home()
    {
        //触发死亡事件让怪物都消失
        eventIns.EventplayerDeath.Invoke();
        losePanel.SetActive(false);
        //重设倒计时
        countNum = counttotal;
        //重设人物血量
        Player.playerInstance.currentHp =Player.playerInstance.totalHP;
        //触发事件
        eventIns.EventRetryBtn.Invoke();
        //关闭暂停按钮界面
        Invoke("ClosefinalCanvas",1f);
        UICanvas.SetActive(false);
        //curEnviroment.SetActive(false);
        //重新加载Home场景        
        SceneManager.LoadScene(0);
    }
    private void ClosefinalCanvas()
    {
        finalCanvas.SetActive(false);
    }
    //------------------------------------------------------------------------------------
}
 