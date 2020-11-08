using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Police : MonoBehaviour
{
    //寻路
    private NavMeshAgent Police_Nav_Agent;
    //玩家物体
    private GameObject player;
    private Player player_ins;
    private GameObject boom;
    private GameObject policeScore;
    private AudioClip policeAudio;
    private AudioClip boomAudio;
    private void Awake()
    {
        policeAudio = Resources.Load<AudioClip>("Sound/Police");       
        boomAudio = Resources.Load<AudioClip>("Sound/explo");       
        selEventSystem.EventIns.EventplayerDeath.AddListener(PlayerDeath);
        selEventSystem.EventIns.EventplayerWin.AddListener(PlayerWin);
        Police_Nav_Agent = this.GetComponent<NavMeshAgent>();
    }
    void OnEnable(){
        player = GameObject.FindGameObjectWithTag("Player");
        player_ins = player.GetComponent<Player>();
        if (Police_Nav_Agent != null){
            Police_Nav_Agent.isStopped =false;
        }
        AudioSource.PlayClipAtPoint(policeAudio, transform.position);
    }
    

    // Update is called once per frame
    void Update()
    { 
        //if (Time.frameCount % 2 == 0)
        //{
            if (player_ins.currentHp <= 0 )
            {
                Police_Nav_Agent.isStopped = true;
                gameObject.SetActive(false);
            }
            else
            {
                Police_Nav_Agent.SetDestination(player.transform.position);
            }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name != "boom(Clone)")
        {
            PoliceDestroy();
        }
    }
    public void PoliceDestroy()
    {
        boom = Instantiate(Resources.Load<GameObject>("Prefab/explode/policeboom"), transform.position, Quaternion.identity) as GameObject;
        AddPoint();
        Invoke("DestroyPoint", 2f);
        gameObject.SetActive(false);
    }
    private void AddPoint()
    {
        //声音
        AudioSource.PlayClipAtPoint(boomAudio, transform.position);
        //生成分数特效
        policeScore = EnemyPool.enemysPoolInstance.GetPoliceScoreObject();
        policeScore.transform.position = gameObject.transform.position + (gameObject.transform.up * 3);
        policeScore.SetActive(true);

        //给玩家加分
        GameManager.gameManager_ins.EnemyDeath(1);
    }
    private void DestroyPoint()
    {
        policeScore.SetActive(false);
    }
    //注册全局事件，玩家获得胜利后

    void PlayerDeath(){
        gameObject.SetActive(false);
    }
    void PlayerWin(){
        gameObject.SetActive(false);
    }
    
}
