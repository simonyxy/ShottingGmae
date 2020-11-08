using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMove : MonoBehaviour
{
    //摄像机跟随物体
    public Transform followTarget;
    //与物体的相对距离
    //private Vector3 realativePosition;
    private Vector3 forcePosition = new Vector3(0, 16.8f, -10.3f);
    //void Start()
    //{
        //摄像机和物体的相对距离（摄像机位置-跟随目标位置）
        //realativePosition = this.transform.position - followTarget.position;
    //}

    // Update is called once per frame
    private void LateUpdate()
    {
        //更新摄像机位置
        this.transform.position = followTarget.position + forcePosition;
    }
}
