using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBoom : MonoBehaviour {

    private GameObject boom;
    private AudioClip boomAudio;
    private AudioClip EnemyAudio;
    private void Awake()
    {
        boomAudio = Resources.Load<AudioClip>("Sound/explo");
        EnemyAudio = Resources.Load<AudioClip>("Sound/Enemy02");

    }
    private void OnEnable()
    {
        AudioSource.PlayClipAtPoint(EnemyAudio, transform.position);
        Invoke("CreatBoom",0.8f);
    }
    void CreatBoom()
    {
        boom = Instantiate(Resources.Load<GameObject>("Prefab/explode/policeboom"), transform.position, Quaternion.identity) as GameObject;
        AudioSource.PlayClipAtPoint(boomAudio, transform.position);
        gameObject.SetActive(false);
    }
}
