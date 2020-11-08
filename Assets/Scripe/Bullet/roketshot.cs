using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roketshot : MonoBehaviour
{
    //需要注意的是，粒子效果里面的Play On Away 不能确认
    private Rigidbody self_rig;
    private GameObject boom2;
    private int power = 900;
    private GameObject player;
    private AudioClip boomAudio;

    void Awake()
    {
        self_rig  = this.GetComponent<Rigidbody>();
        boomAudio = Resources.Load<AudioClip>("Sound/explo");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            //粒子效果
            boom2 = Instantiate(Resources.Load<GameObject>("Prefab/explode/roketboom"), transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(boomAudio, transform.position);
            gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    private void OnEnable()
    {
        player = GameObject.Find("Player");
        self_rig.AddForce((player.transform.forward) * power);
    }
    private void OnDisable()
    {
        //把速度设为0
        self_rig.velocity = Vector3.zero;
    }
}
