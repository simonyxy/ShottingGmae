using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//desc:为什么要分开单独写一个新的UI界面的事件，是因为我发现，暂停按钮点击的时候，panel动画没播放完游戏就暂停了，单独写一个处理，让动画放完再暂停游戏
public class PausePanel : MonoBehaviour
{
    private Animator self_anim;
    private GameObject pauseBtn;
    private void Awake(){
        self_anim = gameObject.GetComponent<Animator>();
        pauseBtn  = gameObject.transform.Find("PauseBtn").gameObject;
        pauseBtn.GetComponent<Button>().onClick.AddListener(PauseCallBack);
    }

    public void RetryCallBack()
    {
        Time.timeScale = 1;
        self_anim.SetBool("isPause", false);
        GameManager.gameManager_ins.Retry();
    }

    public void HomeCallBack()
    {
        Time.timeScale = 1;
        self_anim.SetBool("isPause", false);
        GameManager.gameManager_ins.Home();
    }
    //方法，暂停按钮点击事件相关 
    public void PauseCallBack(){
        //播放动画
        pauseBtn.SetActive(false);
        self_anim.SetBool("isPause",true);
    }
    public void PauseAnimEndCallBack(){
        //2.暂停
        Time.timeScale = 0; 
    }
    //方法，继续按钮点击事件相关
    public void ResumeCallBack(){
        Time.timeScale = 1;
        self_anim.SetBool("isPause",false);
    }
    public void ResumeAnimEndCallBack(){
        pauseBtn.SetActive(true);
    }


}
