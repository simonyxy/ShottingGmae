using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biggun : MonoBehaviour
{
    public  static Biggun Biggun_ins;
    private void Awake()
    {
        Biggun_ins = this;
    }


    public void ChangeMateria(string m)
    {
        this.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Gun/" + m ) ;
    }
}
