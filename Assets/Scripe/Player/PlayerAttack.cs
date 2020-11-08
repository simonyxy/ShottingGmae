using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//@desc: 游戏攻击分两种： 1. 射线检测
//                        2. 从对象池实例化出子弹
//这个脚本写的是用射线检测的攻击
public class PlayerAttack : MonoBehaviour
{
    private Transform gunBarreEnd;
    public LineRenderer normal_layser;
    //发射的层标记
    private int AttackHitMask;
    //散弹枪发射射线
    private LineRenderer[] shut_laysers = new LineRenderer [3];
    //test 
    private Vector3 dirz = new Vector3(0.1f,0,-0.15f);
    //射线的碰撞信息
    private RaycastHit attackHitInfo;
    private RaycastHit[] shutHitInfo = new RaycastHit[3];
    private Ray[] testRay = new Ray[3];
    private Vector3[] Raypos = new Vector3[3];
    public static PlayerAttack normal_attack_Instance;
    //默认的攻击力
    private int damageValue = 50;

    //打到物体的效果
    private GameObject hitParticle;
    private void Awake()
    {
        //射线碰撞的层标记，就是可以和哪个Layer碰撞
        AttackHitMask += 1 << LayerMask.NameToLayer("E");
        AttackHitMask += 1 << LayerMask.NameToLayer("Enemy");
        AttackHitMask += 1 << LayerMask.NameToLayer("Boss");
        AttackHitMask += 1 << LayerMask.NameToLayer("police");
        normal_attack_Instance = this;
        for(int i =0; i < shut_laysers.Length; i++)
        {
            shut_laysers[i] = GameObject.Find("line"+i.ToString()).GetComponent<LineRenderer>();
        }
    }
    private void Start(){
        //初始化
        gunBarreEnd = Player.playerInstance.shotTransform;
    }
    public void PlayNormalAttack()
    {
        if(shut_laysers[0].enabled == true)
        {
            for(int i =0; i < shut_laysers.Length; i++)
            {
                shut_laysers[i].enabled = false;
            }
        }
        if (normal_layser.enabled == false) normal_layser.enabled = true;
        //激活激光
        normal_layser.gameObject.SetActive(true);
        //设置激光渲染七点
        normal_layser.SetPosition(0, gunBarreEnd.position);
        //以枪口位置为七点，枪口方向为正方向，产生一条3D射线
        Ray testRay = new Ray(gunBarreEnd.position, gunBarreEnd.forward);
        //发射射线，记录碰撞结果和碰撞信息
        bool hitResult = Physics.Raycast(testRay, out attackHitInfo, 100, AttackHitMask);

        //如果射线和指定的层级物体发生碰撞
        if (hitResult)
        {
            //设置激光渲染器的重点
            normal_layser.SetPosition(1, attackHitInfo.point);
            hitParticle =  Instantiate(Resources.Load<GameObject>("Prefab/explode/HitParticle"), attackHitInfo.point,Quaternion.identity) as GameObject;
            if(attackHitInfo.transform.gameObject.layer == 9)
            {
                
                attackHitInfo.transform.GetComponent<Enemy>().TakeDamage(damageValue, false);
              
            }
            else if (attackHitInfo.transform.gameObject.layer == 12)
            {
                attackHitInfo.transform.GetComponent<Boss>().TakeDamage(damageValue, false);
            }
        }
        //否则给定一个默认的终点
        else  
        {
            normal_layser.SetPosition(1, (gunBarreEnd.position + gunBarreEnd.forward * 100));
        }
        Invoke("ResetAttack", 0.1f);
    }

    public void PlayShutAttack()
    {
        if (normal_layser.enabled == true) normal_layser.enabled = false;

        if (shut_laysers[0].enabled == false)
        {
            for(int i = 0; i < shut_laysers.Length; i++)
            {   
                shut_laysers[i].enabled = true;
            }
        }
        //激活激光
        normal_layser.gameObject.SetActive(true);
        //以枪口位置为七点，枪口方向为正方向，产生三条3D射线
        //激光发射方向
        Raypos[0] = gunBarreEnd.forward;
        Raypos[1] = gunBarreEnd.forward + dirz;
        Raypos[2] = gunBarreEnd.forward - dirz;

        for (int i = 0; i < shut_laysers.Length; i++)
        {
            //用来检测的碰撞射线
            testRay[i] = new Ray(gunBarreEnd.position, Raypos[i]);
            //LineRender射线 起点
            shut_laysers[i].SetPosition(0, gunBarreEnd.position);
        }

        //发射射线，记录碰撞结果和碰撞信息
        for(int i  = 0; i< shutHitInfo.Length;i++)
        {
            if(Physics.Raycast(testRay[i], out shutHitInfo[i], 10, AttackHitMask))
            {
                //设置激光渲染器的重点
                shut_laysers[i].SetPosition(1, shutHitInfo[i].point);
                hitParticle = Instantiate(Resources.Load<GameObject>("Prefab/explode/HitParticle"), shutHitInfo[i].point, Quaternion.identity) as GameObject;
                if (shutHitInfo[i].transform.gameObject.layer == 9)
                {
                    //敌人收到伤害
                    shutHitInfo[i].transform.GetComponent<Enemy>().TakeDamage(damageValue,false);
                }
                else if(shutHitInfo[i].transform.gameObject.layer == 12)
                {
                    attackHitInfo.transform.GetComponent<Boss>().TakeDamage(damageValue, false);
                }
            }
            else
            {
                shut_laysers[i].SetPosition(1, (gunBarreEnd.position + Raypos[i].normalized * 10));
            }
        }
        Invoke("ResetAttack", 0.1f);
    }

    private void ResetAttack()
    {
        normal_layser.gameObject.SetActive(false);
    }
}
