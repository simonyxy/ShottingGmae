using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boom: MonoBehaviour
{

    private Rigidbody self_rig;
    private GameObject boomObj;
    private GameObject player;
    private AudioClip boomAudio;
    
    void Awake()
    {
        //这里可能会消耗一些性能，因为没生成一个子弹都要去获取player 的rotation，后面优化再注意改动
        //self_rigidbody = this.GetComponent<Rigidbody>();
        self_rig = this.GetComponent<Rigidbody>();
        //player_ins = Player.playerInstance;
        boomAudio = Resources.Load<AudioClip>("Sound/explo");
    }


    private void OnEnable()
    {
        player = GameObject.Find("Player");
        if(player ==null){
            return;
        }
        Invoke("GetD", 2.0f);
        self_rig.AddForce((player.transform.forward)*450);
    }
    void GetD()
    {
        boomObj = Instantiate(Resources.Load<GameObject>("Prefab/explode/1"), transform.position, Quaternion.identity) as GameObject;
        boomObj.GetComponent<BoxCollider>().enabled = true;
        AudioSource.PlayClipAtPoint(boomAudio, transform.position);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        self_rig.velocity = Vector3.zero;
    }
}
