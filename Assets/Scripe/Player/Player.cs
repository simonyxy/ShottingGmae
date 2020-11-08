using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Player : MonoBehaviour
{

    //----------------------------------*物理相关-------------------------------------
    public static Player playerInstance;
    //控制移动的水平和垂直分量
    private float horizontalFactor;
    private float verticalFactor;    
    //玩家移动速度
    private float moveSpeed=8;
    //计算玩家的旋转
    private Quaternion rotaDirection;
    //玩家移动方向
    private Vector3 moveDirection;

    //----------------------------------自身组件---------------------------------------   
    //自身刚体  
    private Rigidbody selfRigidbody;
    //自身动画组件
    private Animator selfAnimator;    
    //枪的材质
    private Material Gun_M;    
    //枪
    private Biggun biggun;
    //子弹
    private GameObject bullet;

    //----------------------------------武器相关----------------------------------------   
    //获得对象池 
    private BulletsPool bulletPool_Ins;            //子弹对象池脚本
    private PlayerAttack normalattack_Ins;         //获得射击脚本(射线检测的攻击)
    [HideInInspector]
    public Transform shotTransform;                //子弹发射的初始化位置
    private float BoomAttackInterval = 0.4f;       //每次发射子弹事件间隔
    private float BoomAttackTimer =0;              //下一次发射子弹的时间
    private int[] bullet_number = new int[5];      //子弹数量
    public bool BoomAttackColdState=false;                //攻击冷却
    public GunEnum gunType = GunEnum.normal;              //声明当前玩家武器类型
    private Vector3 set_offset = new Vector3(0, -0.98f, 0); //定时炸弹初始化时候的位置偏移

    //----------------------------------玩家状态 以及UI---------------------------------
    //玩家的血条
    internal int totalHP = 100;
    //当前的血量
    internal int currentHp;
    //玩家的血条UI
    private GameObject selfHpbar;
    private Image hpBar;
    private float hpHeight = 2.5f;
    //玩家的武器显示
    private Text gunnameText;
    //玩家皮肤
    private SkinnedMeshRenderer playerMesh;
    //是否奔跑
    public bool isRun = false;
    //奔跑特效
    private TrailRenderer selTrail;
    //--------------------------------音效-----------------------------------------------
    private AudioClip attackAudio;
    private AudioClip roketAudio;
    private AudioClip deathAudio;
    private AudioClip hitAudio;
    //技能特效
    private LineRenderer skillEffect;

    private void Awake()
    {
        playerInstance   = this;
        bulletPool_Ins = BulletsPool.bulletsPoolInstance;
        selfHpbar = Instantiate(Resources.Load<GameObject>("Prefab/HpCanvas"), this.transform.position + hpHeight * Vector3.up, Quaternion.identity) as GameObject;
        //初始化子弹
        gunType = GunEnum.normal;
        gunnameText = selfHpbar.transform.Find("Text").GetComponent<Text>();
        shotTransform = gameObject.transform.Find("FirePoint").transform;
        //初始化音效
        attackAudio = Resources.Load<AudioClip>("Sound/gunshot");
        roketAudio = Resources.Load<AudioClip>("Sound/roket");
        deathAudio = Resources.Load<AudioClip>("Sound/game_over");
        hitAudio = Resources.Load<AudioClip>("Sound/Hit");
        playerMesh = transform.Find("Mesh").GetComponent<SkinnedMeshRenderer>();
        playerMesh.material = Resources.Load<Material>("Materials/Player/Player"+PlayerPrefs.GetInt("PlayerMatID",0).ToString());
        //初始化特效
        //skillEffect = transform.Find("LinRenderer").GetComponent<LineRenderer>();
    }
    void Start()
    {
        for (int i = 0; i < bullet_number.Length; i++)
        {
            bullet_number[i] = bulletPool_Ins.InitBulletNum(gunType);
            gunType++;
        }
        //自身刚体组件
        selfRigidbody = this.GetComponent<Rigidbody>();
        //自身动画
        selfAnimator = this.GetComponent<Animator>();
        normalattack_Ins = PlayerAttack.normal_attack_Instance;
        gunType = bulletPool_Ins.nowType;
        //血量初始化
        currentHp = totalHP;
        //奔跑特效
        selTrail = gameObject.GetComponent<TrailRenderer>();
        selTrail.enabled = false;
        //枪
        biggun =Biggun.Biggun_ins;
        //血条
        hpBar = selfHpbar.transform.Find("Hpbar").GetComponent<Image>();

        selEventSystem.EventIns.EventplayerWin.AddListener(StopWalk);
        selEventSystem.EventIns.EventPickBullet.AddListener(PickupBullet);
    }

    //捡到弹药包后
    private void PickupBullet()
    {
        if(gunType != GunEnum.normal)
        {
            //先清空当前武器的子弹数量
            bullet_number[gunType.GetHashCode()] =0;
        }
        int weaponType = Random.Range(1, 5);
        gunType = bulletPool_Ins.RandomChange(weaponType);
        bullet_number[weaponType] = 10;
        //显示UI
        gunnameText.text = bulletPool_Ins.InitGunName(gunType) + " " + bullet_number[gunType.GetHashCode()];
    }
    void Update()
    {
        #region 炸弹攻击的计时器
        //判断语句，普通攻击 间隔计时器计时
        if (BoomAttackColdState == true)
        {
            BoomAttackTimer += Time.deltaTime;
            if (BoomAttackTimer >= BoomAttackInterval)
            {
                //退出冷却
                BoomAttackColdState = false;
                //计时器时间归零
                BoomAttackTimer = 0;
            }
        }
        #endregion
        if (gunType != GunEnum.normal && gunType != GunEnum.shutgun)
        {
            if (!isRun)
            {
                if (BoomAttackColdState == false && Input.GetKey(KeyCode.F))               //可以发射子弹时间
                {
                    if (bullet_number[gunType.GetHashCode()] != 0)
                    {
                        if (gunType == GunEnum.roket)
                        {
                            AudioSource.PlayClipAtPoint(roketAudio, shotTransform.position);
                        }
                        BoomAttackColdState = true;
                        //获取对象池中的子弹
                        //325
                        //GameObject bullet = bulletPool_Ins.GetPooledObject(gunType);
                        bullet = bulletPool_Ins.GetPooledObject(gunType);
                        selfAnimator.SetInteger("WeaponType_int", 4);
                        selfAnimator.SetBool("Walkable", horizontalFactor != 0 || verticalFactor != 0);
                        if (bullet != null)                  //不为空时执行
                        {
                            bullet.SetActive(true);         //激活子弹并初始化子弹的位置
                            if (gunType == GunEnum.set)
                            {
                                bullet.transform.position = shotTransform.position + set_offset;
                            }
                            else { bullet.transform.position = shotTransform.position; }
                        }
                        //动画
                        Invoke("StopShotAnim", 0.2f);
                        //减少子弹池子弹
                        DecreasBullet(gunType);
                    }
                }
            }
        }
        else if (gunType == GunEnum.normal)
        {
            if (!isRun)
            {
                if (BoomAttackColdState == false && Input.GetKey(KeyCode.F))
                {
                    if (bullet_number[gunType.GetHashCode()] != 0)
                    {
                        selfAnimator.SetInteger("WeaponType_int", 4);
                        AudioSource.PlayClipAtPoint(attackAudio, shotTransform.position);
                        normalattack_Ins.PlayNormalAttack();
                        BoomAttackColdState = true;
                        //取消动画效果
                        Invoke("StopShotAnim", 0.2f);
                    }
                }
            }
        }
        else if(gunType == GunEnum.shutgun)
        {
            if (!isRun)
            {
                if (BoomAttackColdState == false && Input.GetKey(KeyCode.F))
                {
                    if (bullet_number[gunType.GetHashCode()] != 0)
                    {
                        selfAnimator.SetInteger("WeaponType_int", 4);
                        normalattack_Ins.PlayShutAttack();
                        BoomAttackColdState = true;
                        //取消动画效果
                        Invoke("StopShotAnim", 0.2f);
                        AudioSource.PlayClipAtPoint(attackAudio, shotTransform.position);
                        DecreasBullet(gunType);
                        //gunnameText.text = bulletPool_Ins.InitGunName(gunType) + " " + bullet_number[gunType.GetHashCode()];
                    }
                }
            }

        }
        #region 切换武器按钮被弃用，改为直接显示，保留用来开启
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    //调用对象池里的方法切换武器
        //    gunType = bulletPool_Ins.BulletChange(0);
        //    BoomAttackInterval = bulletPool_Ins.GetWeaponColdTime(gunType);
        //    BoomAttackColdState = false;
        //    biggun.ChangeMateria(gunType.ToString());
        //    gunnameText.text = bulletPool_Ins.InitGunName(gunType) + " " + bullet_number[gunType.GetHashCode()];
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    //调用对象池里的方法切换武器
        //    gunType = bulletPool_Ins.BulletChange(1);
        //    BoomAttackInterval = bulletPool_Ins.GetWeaponColdTime(gunType);
        //    biggun.ChangeMateria(gunType.ToString());
        //    BoomAttackColdState = false;
        //    if (gunType == GunEnum.normal)
        //        gunnameText.text = bulletPool_Ins.InitGunName(gunType);
        //    else
        //    {
        //        gunnameText.text = bulletPool_Ins.InitGunName(gunType) + " " + bullet_number[gunType.GetHashCode()];
        //    }
        //}
        #endregion

    }
    private void FixedUpdate()
    {
        if (currentHp > 0)
        {
            //血条跟随
            selfHpbar.transform.position = this.transform.position + hpHeight * Vector3.up;
            //获得unity轴
            horizontalFactor = Input.GetAxis("Horizontal");
            verticalFactor = Input.GetAxis("Vertical");
            //Debug.Log(horizontalFactor + "= horizontalFactor " + verticalFactor + "= verticalFactor");
            //如果水平分量或者竖直方向不为0，调用移动函数
            if (horizontalFactor != 0 || verticalFactor != 0)
            {
                //移动
                Move();
            }
            else
            {
                selfAnimator.SetBool("Walkable", horizontalFactor != 0 || verticalFactor != 0);
                selfAnimator.SetBool("Run", horizontalFactor != 0 || verticalFactor != 0);
            }
            //满足后面条件时候为TURE不满足为False
        }
    }
    private void Move()
    {
        //玩家移动方向
        moveDirection = new Vector3(horizontalFactor, 0, verticalFactor);
        rotaDirection = Quaternion.LookRotation(moveDirection);
        selfRigidbody.MoveRotation(rotaDirection);
        //归一化（模取1）
        moveDirection.Normalize();
        if (Input.GetKey(KeyCode.Q) )
        {
            //移动，（使用物理引擎，而不是改动位置，这样发生碰撞什么的更准确）
            //当前位置+ （速度大小*速度方向*速度更新时间）
            selfRigidbody.MovePosition(this.transform.position + (moveSpeed  *1.5f* moveDirection * Time.fixedDeltaTime));
            if (isRun == false)
            {
                selTrail.enabled = true;
                isRun = true;
                selfAnimator.SetBool("Run", isRun);
            }
        }
        else
        { 
            //移动，（使用物理引擎，而不是改动位置，这样发生碰撞什么的更准确）
            //当前位置+ （速度大小*速度方向*速度更新时间）
            selfRigidbody.MovePosition(this.transform.position + (moveSpeed * moveDirection * Time.fixedDeltaTime));
            //rotaDirection = Quaternion.LookRotation(moveDirection);
            //selfRigidbody.MoveRotation(rotaDirection);
            if (isRun == true)
            {
                selTrail.enabled = false;
                isRun = false;
                selfAnimator.SetBool("Run", isRun);
            }
            //把动画从Idle切换为Move
            selfAnimator.SetBool("Walkable", horizontalFactor != 0 || verticalFactor != 0);
        }
       
    }
    private void StopShotAnim()
    {
        selfAnimator.SetInteger("WeaponType_int", 0);
    }

    //被攻击的时候
    public void TakeDamage(int damageValue)
    {
        //首先要收到伤害，怪物要存活
        if (currentHp > 0)
        {
            AudioSource.PlayClipAtPoint(hitAudio, transform.position);
            //收到伤害，血量减少
            currentHp -= damageValue;
            //如果血量小于等于0
            if (currentHp <= 0)
            {
                PlayerDeath();
            }
            //修改血量值
            hpBar.fillAmount = currentHp / (float)totalHP;
        }
    }
    //减少子弹数量
    void DecreasBullet(GunEnum gunType)
    {
        bullet_number[gunType.GetHashCode()] -= 1;
        Debug.Log(bullet_number[gunType.GetHashCode()]);
        //如果当前武器子弹用完了
        if (bullet_number[gunType.GetHashCode()] == 0)
        {
            Debug.Log("子弹没了，进来");
            this.gunType = GunEnum.normal; 
            gunnameText.text = bulletPool_Ins.InitGunName(this.gunType);
        }
        else
        {
            gunnameText.text = bulletPool_Ins.InitGunName(gunType) + " " + bullet_number[gunType.GetHashCode()];
        }
    }

    void PlayerDeath(){
        //因为程序中血量不可能为负数
        //修正血量为0
        currentHp = 0;
        //播放死亡动画和音效
        selfAnimator.SetBool("Death_b",true);
        AudioSource.PlayClipAtPoint(deathAudio,transform.position);
        //设置Collider
        gameObject.GetComponent<Collider>().enabled=false;
        //销毁血条
        Destroy(hpBar);
        //触发事件
        selEventSystem.EventIns.EventplayerDeath.Invoke();
    }
    //胜利后玩家无法移动
    void StopWalk(){
        currentHp = 0;
    }
}
