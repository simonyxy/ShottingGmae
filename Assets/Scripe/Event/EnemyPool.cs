using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//@desc : 怪物和怪物得分特效对象池
//        把所有的怪物相关物体都放在这个池里生成，因为调用unity交互比较多。所以在整个游戏过程只被生成一次。在SceneControl脚本里面注册进处理过的DontDestroyOnLoad()里面

//说明 []enemy          ：游戏中的怪物相关的物体
//     enemyPool        ：存放对象的怪物对象池（二维链表）【每个怪物占一层】
//     []currentIndex   ：取出怪物对象当前的索引

//注意： 放回对象池的对象只需要 SetActive（false）即可
//       同理，重新取出时记得注册这个怪物的transform
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool enemysPoolInstance;      //怪物池实例，被其他脚本调用方便
    private GameObject[] enemy ;                        //怪物perfabs


    private int pooledAmount = 5;                   //怪物池初始大小
    private bool lockPoolSize = false;               //是否锁定怪物池大小
    private List<List<GameObject>> enemyPool;        //怪物池
    private List<GameObject> enemyObjects;           //怪物池链表
    private List<GameObject> policeObjects;          //怪物池链表
    private List<GameObject> enemyScoreObjects;      //怪物得分显示
    private List<GameObject> policeScoreObjects;     //警车得分显示
    private List<GameObject> bossObjects;            //Boss怪物
    private List<GameObject> bossScoreObjects;      //Boss怪物的得分
    private List<GameObject> bossAttackObjects;         //Boss攻击物体
    private List<GameObject> areaBoomObjects;       //轰炸区对象
    private int[] currentIndex ;                    //当前指向链表位置索引

    private void Awake()
    {
        enemysPoolInstance = this;                     //把本对象作为实例
        // enemyPool = new List<GameObject>()[4];
        enemy = new GameObject[8];
        enemy[0] = Resources.Load("Prefab/Enemy/Enemy")  as GameObject;
        enemy[1] = Resources.Load("Prefab/Enemy/Police") as GameObject;
        enemy[2] = Resources.Load("Prefab/Enemy/EnemyScore")  as GameObject;
        enemy[3] = Resources.Load("Prefab/Enemy/PoliceScore") as GameObject;
        enemy[4] = Resources.Load("Prefab/Enemy/Boss")   as GameObject;
        enemy[5] = Resources.Load("Prefab/Enemy/BossScore") as GameObject;
        enemy[6] = Resources.Load("Prefab/Enemy/AreaBoom") as GameObject;
        enemy[7] = Resources.Load("Prefab/Bullet/BossAttack") as GameObject;

        currentIndex = new int[enemy.Length];
    }
    void Start()
    {
        enemyObjects = new List<GameObject>();         //初始化链表
        policeObjects = new List<GameObject>();
        enemyScoreObjects = new List<GameObject>();
        policeScoreObjects = new List<GameObject>();
        bossObjects = new List<GameObject>();
        bossScoreObjects = new List<GameObject>();
        areaBoomObjects = new List<GameObject>();
        bossAttackObjects = new List<GameObject>();
        enemyPool =new List<List<GameObject>>(){enemyObjects,policeObjects,enemyScoreObjects,policeScoreObjects, bossObjects, bossScoreObjects,areaBoomObjects , bossAttackObjects };
        InitPool();
    }
    //初始化对象池对象
    private void InitPool(){
        for(int i = 0 ; i < enemyPool.Count;i++)
        {
            for (int j = 0; j < pooledAmount; j++)
            {
                GameObject obj  = Instantiate(enemy[i], this.transform);    //创建怪物对象
                obj.SetActive(false);                       //设置怪物无效
                enemyPool[i].Add(obj);                     //把怪物添加到链表（对象池）中
            }
        }
    }
 
    //方法，取对象
    public GameObject GetEnemyObject()                 //获取对象池中可以使用的怪物。
    {
        for (int i = 0; i < enemyPool[0].Count; ++i)   //把对象池遍历一遍
        {
            //这里简单优化了一下，每一次遍历都是从上一次被使用的怪物的下一个，而不是每次遍历从0开始。
            //例如上一次获取了第4个怪物，currentIndex就为5，这里从索引5开始遍历，这是一种贪心算法。
            int temI = (currentIndex[0] + i) % enemyPool[0].Count;
            if (!enemyPool[0][temI].activeInHierarchy) //判断该怪物是否在场景中激活。
            {
                currentIndex[0] = (temI + 1) % enemyPool[0].Count;
                return enemyPool[0][temI];             //找到没有被激活的怪物并返回
            }
        }
        //如果遍历完一遍怪物库发现没有可以用的，执行下面
        if (!lockPoolSize)                               //如果没有锁定对象池大小，创建怪物并添加到对象池中。
        {
            GameObject obj = Instantiate(enemy[0]);
            obj.transform.parent = this.transform;       //生成到buttlepool下
            enemyPool[0].Add(obj);
            return obj;
        }
        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }
    public GameObject GetPoliceObject()                 //获取对象池中可以使用的怪物。
    {
        for (int i = 0; i < enemyPool[1].Count; ++i)   //把对象池遍历一遍
        {
            //这里简单优化了一下，每一次遍历都是从上一次被使用的怪物的下一个，而不是每次遍历从0开始。
            //例如上一次获取了第4个怪物，currentIndex就为5，这里从索引5开始遍历，这是一种贪心算法。
            int temI = (currentIndex[1] + i) % enemyPool[1].Count;
            if (!enemyPool[1][temI].activeInHierarchy) //判断该怪物是否在场景中激活。
            {
                currentIndex[1] = (temI + 1) % enemyPool[1].Count;
                return enemyPool[1][temI];             //找到没有被激活的怪物并返回
            }
        }
        //如果遍历完一遍怪物库发现没有可以用的，执行下面
        if (!lockPoolSize)                               //如果没有锁定对象池大小，创建怪物并添加到对象池中。
        {
            GameObject obj = Instantiate(enemy[1]);
            obj.transform.parent = this.transform;       //生成到buttlepool下
            enemyPool[1].Add(obj);
            return obj;
        }
        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }
    public GameObject GetEnemyScoreObject()                 //获取对象池中可以使用的怪物。
    {
        for (int i = 0; i < enemyPool[2].Count; ++i)   //把对象池遍历一遍
        {
            //这里简单优化了一下，每一次遍历都是从上一次被使用的怪物的下一个，而不是每次遍历从0开始。
            //例如上一次获取了第4个怪物，currentIndex就为5，这里从索引5开始遍历，这是一种贪心算法。
            int temI = (currentIndex[2] + i) % enemyPool[2].Count;
            if (!enemyPool[2][temI].activeInHierarchy) //判断该怪物是否在场景中激活。
            {
                currentIndex[2] = (temI + 1) % enemyPool[2].Count;
                return enemyPool[2][temI];             //找到没有被激活的怪物并返回
            }
        }
        //如果遍历完一遍怪物库发现没有可以用的，执行下面
        if (!lockPoolSize)                               //如果没有锁定对象池大小，创建怪物并添加到对象池中。
        {
            GameObject obj = Instantiate(enemy[2]);
            obj.transform.parent = this.transform;       //生成到buttlepool下
            enemyPool[2].Add(obj);
            return obj;
        }
        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }

    public GameObject GetPoliceScoreObject()                 //获取对象池中可以使用的怪物。
    {
        for (int i = 0; i < enemyPool[3].Count; ++i)   //把对象池遍历一遍
        {
            //这里简单优化了一下，每一次遍历都是从上一次被使用的怪物的下一个，而不是每次遍历从0开始。
            //例如上一次获取了第4个怪物，currentIndex就为5，这里从索引5开始遍历，这是一种贪心算法。
            int temI = (currentIndex[3] + i) % enemyPool[3].Count;
            if (!enemyPool[3][temI].activeInHierarchy) //判断该怪物是否在场景中激活。
            {
                currentIndex[3] = (temI + 1) % enemyPool[3].Count;
                return enemyPool[3][temI];             //找到没有被激活的怪物并返回
            }
        }
        //如果遍历完一遍怪物库发现没有可以用的，执行下面
        if (!lockPoolSize)                               //如果没有锁定对象池大小，创建怪物并添加到对象池中。
        {
            GameObject obj = Instantiate(enemy[3]);
            obj.transform.parent = this.transform;       //生成到buttlepool下
            enemyPool[3].Add(obj);
            return obj;
        }
        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }
    public GameObject GetBossObject()
    {
        for (int i = 0; i < enemyPool[4].Count; ++i)
        {
            int temI = (currentIndex[4] + i) % enemyPool[4].Count;
            if (!enemyPool[4][temI].activeInHierarchy)
            {
                currentIndex[4] = (temI + 1) % enemyPool[4].Count;
                return enemyPool[4][temI];
            }
        }
        if (!lockPoolSize)
        {
            GameObject obj = Instantiate(enemy[4]);
            obj.transform.parent = this.transform;
            enemyPool[4].Add(obj);
            return obj;
        }
        return null;
    }
    public GameObject GetBossScoreObject()
    {
        for (int i = 0; i < enemyPool[5].Count; ++i)
        {
            int temI = (currentIndex[5] + i) % enemyPool[5].Count;
            if (!enemyPool[5][temI].activeInHierarchy)
            {
                currentIndex[5] = (temI + 1) % enemyPool[5].Count;
                return enemyPool[5][temI];
            }
        }
        if (!lockPoolSize)
        {
            GameObject obj = Instantiate(enemy[5]);
            obj.transform.parent = this.transform;
            enemyPool[5].Add(obj);
            return obj;
        }
        return null;
    }
    public GameObject GetAreaBoomObject()
    {
        for (int i = 0; i < enemyPool[6].Count; ++i)
        {
            int temI = (currentIndex[6] + i) % enemyPool[6].Count;
            if (!enemyPool[6][temI].activeInHierarchy)
            {
                currentIndex[6] = (temI + 1) % enemyPool[6].Count;
                return enemyPool[6][temI];
            }
        }
        if (!lockPoolSize)
        {
            GameObject obj = Instantiate(enemy[6]);
            obj.transform.parent = this.transform;
            enemyPool[6].Add(obj);
            return obj;
        }
        return null;
    }
    public GameObject GetBossAttack()
    {
        for (int i = 0; i < enemyPool[7].Count; ++i)
        {
            int temI = (currentIndex[7] + i) % enemyPool[7].Count;
            if (!enemyPool[7][temI].activeInHierarchy)
            {
                currentIndex[7] = (temI + 1) % enemyPool[7].Count;
                return enemyPool[7][temI];
            }
        }
        if (!lockPoolSize)
        {
            GameObject obj = Instantiate(enemy[7]);
            obj.transform.parent = this.transform;
            enemyPool[7].Add(obj);
            return obj;
        }
        return null;
    }
    
}
