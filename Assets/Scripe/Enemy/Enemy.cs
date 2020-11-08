using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //寻路
    private NavMeshAgent Enemy_Nav_Agent;
    //玩家物体
    private GameObject player;

    //怪物血量
    private int totalHp = 100;
    //当前血量和攻击力
    private int currentHp;
    private int damage_Hp = 10;
    //自身动画组件
    private Animator selfAnim; 
    //攻击距离 
    private int attackDistance=1;
    //获得加分特效
    private GameObject enemyScore;

    //该怪物类型
    private int Enemytype =0;

    private void OnEnable()
    {   
        selfAnim = this.GetComponent<Animator>();
        Enemy_Nav_Agent = this.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        //初始化血量
        Enemy_Nav_Agent.SetDestination(player.transform.position);
        currentHp = totalHp;
        //开始寻路
        Enemy_Nav_Agent.isStopped = false;
        currentHp = totalHp;
        this.gameObject.layer = 9;
    }
    void Start(){
        //注册玩家死亡和胜利事件
        selEventSystem.EventIns.EventplayerDeath.AddListener(PlayerDeath);
        selEventSystem.EventIns.EventplayerWin.AddListener(PlayerWin);
    }
    void Update()
    {
        if (currentHp > 0)
        {
            if (Time.frameCount % 50 == 0)
            {
                //怪物走的慢，设置时间延长给cpu缓缓
                Enemy_Nav_Agent.SetDestination(player.transform.position);
            }
            if(Time.frameCount % 50 == 0)
            {
                EnemyAttack();
            }
        }
    }
    public void TakeDamage(int damageValue,bool isboom)
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
        if(currentHp>0) 
        {
            //受到伤害
            currentHp -= damageValue;
        }
        if (currentHp <= 0)//怪物死亡
        {
            Death();
        }
        
    }
    private void Death(){
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
    
    
    private void AddPoint( )
    {
        enemyScore = EnemyPool.enemysPoolInstance.GetEnemyScoreObject();
        enemyScore.transform.position = gameObject.transform.position + (gameObject.transform.up*3);
        enemyScore.SetActive(true);
    }
    private void Destroy()
    {
        //死亡 加分
        GameManager.gameManager_ins.EnemyDeath(Enemytype);
        enemyScore.SetActive(false);
        this.gameObject.SetActive(false);
    }
    void EnemyAttack()
    {
        //如果怪物距离小于攻击距离
        if(Vector3.Distance(this.transform.position,player.transform.position)<= attackDistance)
        {
            //玩家收到伤害
            player.GetComponent<Player>().TakeDamage(damage_Hp);
            
        }
        if (player.GetComponent<Player>().currentHp <= 0)
        {
            PlayerDeath();
        }
    }
    void PlayerDeath(){
        //停止寻路
        // Enemy_Nav_Agent.isStopped = true;
        //怪物动画更改
        // selfAnim.SetBool("PlayerDeath", true);
        //以后补充
        gameObject.SetActive(false);
    }
    void PlayerWin(){
        gameObject.SetActive(false);
    }
}
