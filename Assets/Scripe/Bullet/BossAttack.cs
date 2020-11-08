using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private Rigidbody self_rig;
    private GameObject boom2;
    private int power = 400;
    private int damage = 50;
    private GameObject player;
    private AudioClip boomAudio;
    private Vector3 dir;
    private  static GameObject parent;

    void Awake()
    {
        self_rig = this.GetComponent<Rigidbody>();
        boomAudio = Resources.Load<AudioClip>("Sound/explo");
    }
    void OnEnable()
    {
        if (parent != null)
        {
            self_rig.AddForce((parent.transform.forward) * power);
        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.layer == 10)
        {
            other.GetComponent<Player>().TakeDamage(damage); 
            //粒子效果
            boom2 = Instantiate(Resources.Load<GameObject>("Prefab/explode/roketboom"), transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(boomAudio, transform.position);
            gameObject.SetActive(false);
        }
        else if(other.gameObject.layer == 8)
        { 
            boom2 = Instantiate(Resources.Load<GameObject>("Prefab/explode/roketboom"), transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(boomAudio, transform.position);
            gameObject.SetActive(false);
        }
        
    }
   
    private void OnDisable()
    {
        //把速度设为0
        self_rig.velocity = Vector3.zero;
    }
    public void SetParent(GameObject par)
    {
        parent = par;
    }
}
