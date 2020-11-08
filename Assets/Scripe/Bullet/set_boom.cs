using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class set_boom : MonoBehaviour
{
    private GameObject boom2;
    private AudioClip boomAudio;
    private void Awake()
    {
        boomAudio = Resources.Load<AudioClip>("Sound/explo");
        selEventSystem.EventIns.EventplayerWin.AddListener(DestroySelf);
        selEventSystem.EventIns.EventplayerDeath.AddListener(DestroySelf);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);   
        if (other.gameObject.layer == 9 || other.gameObject.layer == 11 )
        {
            //粒子效果
            boom2 = Instantiate(Resources.Load<GameObject>("Prefab/explode/setboom"), transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(boomAudio, transform.position);
            gameObject.SetActive(false);
        }
    }
    private void DestroySelf()
    {
        gameObject.SetActive(false);
    }
}
