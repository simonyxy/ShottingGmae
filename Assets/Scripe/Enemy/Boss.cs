using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss : MonoBehaviour
{
    //325
    //寻路
    private NavMeshAgent Enemy_Nav_Agent;
    //玩家物体
    private GameObject player;

    //怪物血量
    private int totalHp = 700;
    //当前血量和攻击力
    private int currentHp;
    //自身动画组件
    private Animator selfAnim;
    //攻击距离 
    private int attackDistance = 10;
    //获得加分特效
    private GameObject enemyScore;
    //private BulletsPool bulletPool_Ins;
    //当前状态
    private bool isAttack=false;
    private GameObject bullet;
    //该怪物类型
    private int Enemytype = 2;
    private Transform ShootPoint;

    //326
    private EnemyPool enemyPoolIns;
    private selEventSystem selEventSystemIns;
    private GameManager GMIns;
    private void OnEnable()
    {
        selfAnim = gameObject.GetComponent<Animator>();
        Enemy_Nav_Agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        selfAnim.SetBool("isAttack", false);
        //初始化血量
        Enemy_Nav_Agent.SetDestination(player.transform.position);
        currentHp = totalHp;
        //开始寻路
        Enemy_Nav_Agent.isStopped = false;
        currentHp = totalHp;
        this.gameObject.layer = 12;
        //重置状态
        isAttack = false;
    }
    void Start()
    {
        ShootPoint = transform.GetChild(2);
        enemyPoolIns = EnemyPool.enemysPoolInstance;
        selEventSystemIns = selEventSystem.EventIns;
        GMIns = GameManager.gameManager_ins;
        //注册玩家死亡和胜利事件
        selEventSystemIns.EventplayerDeath.AddListener(PlayerDeath);
        selEventSystemIns.EventplayerWin.AddListener(PlayerWin);
    }
    void Update()
    {
       if (currentHp > 0)
        {
            if (isAttack)
            {
                transform.LookAt(player.transform);
            }
            if (Time.frameCount % 25 ==0 )
            {
                if (!isAttack)
                {
                    //怪物走的慢，设置时间延长给cpu缓缓
                    Enemy_Nav_Agent.SetDestination(player.transform.position);
                    StageChange();
                }
            }
        }
    }
    public void TakeDamage(int damageValue, bool isboom)
    {
        //射击类的和炸弹类的撞击效果是不同的
        if (isboom)
        {
            gameObject.transform.position -= gameObject.transform.forward * 2;
        }
        else
        {
            gameObject.transform.position += Player.playerInstance.transform.forward;
        }
        if (currentHp > 0)
        {
            //受到伤害
            currentHp -= damageValue;
        }
        if (currentHp <= 0)//怪物死亡
        {
            Death();
        }

    }
    private void Death()
    {
        //修正血条为0 
        currentHp = 0;
        //播放死亡动画
        selfAnim.SetBool("EneDeath", true);
        //停止寻路
        Enemy_Nav_Agent.isStopped = true;
        //设置层级让子弹无法达到
        this.gameObject.layer = 0;
        Invoke("AddPoint", 1f);
        Invoke("Destroy", 2f);
    }

    private void BossAttack()
    {
        transform.LookAt(player.transform);                     //面向玩家
        bullet = enemyPoolIns.GetBossAttack();              //需要添加子弹
        bullet.transform.position = ShootPoint.position;
        bullet.GetComponent<BossAttack>().SetParent(gameObject);
        bullet.SetActive(true);
    }

    private void AddPoint()
    {
        enemyScore = enemyPoolIns.GetBossScoreObject();
        enemyScore.transform.position = gameObject.transform.position + (gameObject.transform.up * 3);
        enemyScore.SetActive(true);
    }
    private void Destroy()
    {
        //死亡 加分
        GMIns.EnemyDeath(Enemytype);
        enemyScore.SetActive(false);
        CancelInvoke("BossAttack");
        this.gameObject.SetActive(false);
    }
    void StageChange()
    {
        //如果怪物距离小于攻击距离
        if (Vector3.Distance(this.transform.position, player.transform.position) <= attackDistance)
        {
            //1.发射子弹，每帧转向玩家
            isAttack = true;
            selfAnim.SetBool("isAttack", isAttack);
            Enemy_Nav_Agent.isStopped = true;
            InvokeRepeating("BossAttack", 2f,2.4f);
        }
    }
    void PlayerDeath()
    {
        CancelInvoke("BossAttack");
        gameObject.SetActive(false);
    }
    void PlayerWin()
    {
        CancelInvoke("BossAttack");
        gameObject.SetActive(false);
    }

}
