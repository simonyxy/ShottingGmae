using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//desc:补充子弹的物体
public class ObjXX : MonoBehaviour
{
    Vector3 rota = new Vector3(0, 0.5f, 0);
    AudioClip pickUpAuido;
    private void Awake()
    {
        pickUpAuido = Resources.Load<AudioClip>("Sound/pick");
    }
    void Update()
    {
        transform.Rotate(rota,Space.World);       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10) { 
            selEventSystem.EventIns.EventPickBullet.Invoke();
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(pickUpAuido,transform.position);
        }
    }
}
