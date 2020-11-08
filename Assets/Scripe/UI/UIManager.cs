using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //动态设置该组件的Camera
   Canvas s ;
   GameObject winPanel;
   GameObject losePanel;
   void Awake(){
       winPanel = gameObject.transform.Find("winPanel").gameObject;
       losePanel=gameObject.transform.Find("losePanel").gameObject;
   }
   void Start(){
       selEventSystem.EventIns.EventGameStart.AddListener(GameStartListener);
   }
   public void RetryWinCallBack(){
       winPanel.SetActive(false);
       GameManager.gameManager_ins.Retry();
   }

   public void RetryLoseCallBack(){
       losePanel.SetActive(false);
       GameManager.gameManager_ins.Retry();
   }

   public void HomeWinCallBack(){
       winPanel.SetActive(false);
       GameManager.gameManager_ins.Home();
   }

   public void HomeLoseCallBack(){
       losePanel.SetActive(false);
       GameManager.gameManager_ins.Home();
   }

   private void GameStartListener(){
       s = gameObject.GetComponent<Canvas>();
       s.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
   }
}
