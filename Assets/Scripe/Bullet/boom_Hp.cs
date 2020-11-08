using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//脚本说明
//暴炸效果里面的碰撞到怪物扣血
public class boom_Hp : MonoBehaviour
{
    private int damageValue = 200;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damageValue, true);
        }
        else if (other.gameObject.layer == 11)
        {
            other.gameObject.GetComponent<Police>().PoliceDestroy();
        }
        else if (other.gameObject.layer == 12)
        {
            other.gameObject.GetComponent<Boss>().TakeDamage(damageValue, true);
        }
        else if (other.gameObject.layer == 10)
        {
            other.gameObject.GetComponent<Player>().TakeDamage(damageValue);
        }
    }
}
