using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//枚举一个武器结构，玩家更改武器时用来调用
public enum GunEnum
{
    normal,    //普通
    shutgun,   //散弹枪
    boom,      //手榴弹
    set,       //陷阱
    roket,     //火箭筒  
}


//声明一个武器类，把各种武器都声明成武器类的一个对象
public class SortOfFire
{
    //各自的子弹prefab和保存他们对象的列表
    public GameObject bullet;
    public List<GameObject> bulletObj;
    //当前指向链表位置索引
    public int currentIndex;
    //发射间隔时间
    public float shotting_time;
    //子弹的攻击力
    public int damage_Hp;
    //子弹最大数量
    internal int max_bullet;
    //名字
    internal string name;
    
    //构造方法
    public SortOfFire(float shotting_time,int damage_Hp,int max_bullet,string name ) {
        this.damage_Hp = damage_Hp;
        this.shotting_time = shotting_time;
        this.max_bullet = max_bullet;
        this.name = name;
        bulletObj = new List<GameObject>();
        currentIndex = 0;
    }
}
public class BulletsPool : MonoBehaviour
{
    private bool lockPoolSize = false;                    //是否锁定子弹池大小
    private  int pooledAmount = 5;                        //子弹池初始大小

    //生成类对象
    public SortOfFire boomObj  = new SortOfFire(0.7f,150,0, "Grenade");                          //手榴弹
    public SortOfFire setObj   = new SortOfFire(0.2f,150,0, "Trap");                             //陷阱
    public SortOfFire roketObj = new SortOfFire(0.5f,300,0, "Roket");                            //火箭筒 
    public SortOfFire normal   = new SortOfFire(0.5f,50,-1,"Pistol");                            //手枪
    public SortOfFire shutgun  = new SortOfFire(0.5f,50, 0, "ShotGun");                          //散弹枪

    //声明子弹类型，在外部把他们初始化
    public GameObject boom;
    public GameObject sett;
    public GameObject roket;

    //初始化当前武器类型和武器类
    SortOfFire nowbulletType;                                  
    public GunEnum nowType;
    public static BulletsPool bulletsPoolInstance;      //子弹池实例
    void Awake()
    {
        //初始化武器类型
        nowType = GunEnum.normal;
        boomObj.bullet  = boom;
        setObj.bullet   = sett;
        roketObj.bullet = roket;
        bulletsPoolInstance = this;                     //把本对象作为实例

         //初始化子弹
        InitBullet(boomObj);
        InitBullet(setObj);
        InitBullet(roketObj);
    }
    public GameObject GetPooledObject(GunEnum Sort)                 //获取对象池中可以使用的子弹。
    {
        nowbulletType = GetWeaponType(Sort);
        for (int i = 0; i < nowbulletType.bulletObj.Count; ++i)   //把对象池遍历一遍
        {
            //这里简单优化了一下，每一次遍历都是从上一次被使用的子弹的下一个，而不是每次遍历从0开始。
            //例如上一次获取了第4个子弹，currentIndex就为5，这里从索引5开始遍历，这是一种贪心算法。
            int temI = (nowbulletType.currentIndex + i) % nowbulletType.bulletObj.Count;
            if (!nowbulletType.bulletObj[temI].activeInHierarchy) //判断该子弹是否在场景中激活。
             {
                nowbulletType.currentIndex = (temI + 1) % nowbulletType.bulletObj.Count;
                return nowbulletType.bulletObj[temI];             //找到没有被激活的子弹并返回
             }
        }
        //如果遍历完一遍子弹库发现没有可以用的，执行下面
        if (!lockPoolSize)                               //如果没有锁定对象池大小，创建子弹并添加到对象池中。
        { 
            GameObject obj = Instantiate(nowbulletType.bullet);
            obj.transform.parent = this.transform;       //生成到buttlepool下
            nowbulletType.bulletObj.Add(obj);
            return obj;
        }
        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }


     
   void InitBullet(SortOfFire bulletSort)
   {
    for (int i = 0; i < pooledAmount; ++i)
      {
        GameObject obj = Instantiate(bulletSort.bullet,this.transform);   //创建子弹对象
        obj.SetActive(false);                              //设置子弹无效
        bulletSort.bulletObj.Add(obj);                     //把子弹添加到链表（对象池）中
     }
   }

    //切换武器的方法(Player脚本调用)
    public GunEnum BulletChange(int i)
    {
        if(i == 0)
        {
            if (nowType ==GunEnum.roket) return nowType;
            nowType++;

            return nowType;
        }
        else 
        {
            if (nowType == GunEnum.normal) return nowType;
            nowType--;
            
            return nowType;
        }
    }
    public GunEnum RandomChange(int type)
    {
        nowType = GunEnum.normal + type;
        return nowType;
    }
    private SortOfFire GetWeaponType(GunEnum Sort)
    {
        if (Sort == GunEnum.boom) return boomObj;
        else if (Sort == GunEnum.roket) return roketObj;
        else if (Sort == GunEnum.set) return setObj;
        else if (Sort == GunEnum.normal) return normal;
        else if (Sort == GunEnum.shutgun) return shutgun;
        else return null;
    }

    public float GetWeaponColdTime(GunEnum Sort)
    {
        if (Sort == GunEnum.boom) return boomObj.shotting_time;
        else if (Sort == GunEnum.roket) return roketObj.shotting_time;
        else if (Sort == GunEnum.set) return setObj.shotting_time;
        return 0.2f;
    }
    public int InitBulletNum(GunEnum Type)
    {
        return GetWeaponType(Type).max_bullet;
    }
    public string InitGunName(GunEnum Type)
    {
        return GetWeaponType(Type).name;
    }


}
