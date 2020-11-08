using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roketBoom : MonoBehaviour
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
        else if (other.gameObject.layer ==12)
        {
            other.gameObject.GetComponent<Boss>().TakeDamage(damageValue, true);
        }
    }
}
