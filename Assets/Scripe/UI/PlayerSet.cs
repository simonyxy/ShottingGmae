using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//@desc: 玩家的设置 
//包括  :  1.换装 (确认后保存到全局变量PlayerPrefs中))
//         
//     
public class PlayerSet : MonoBehaviour {

	//玩家当前材质id
    private int curMatID = 0;
	private int playerMatID=0;
    private string[] names = new string[] { "Student" , "Sailor", "Fierce", "Spy", "Hawaii", "King" , "Gentleman", "British" , "SportMan","Black","Cook","OldMan" };

    //地图UI
    public GameObject map;
    //设置按钮
    public GameObject setBtn;
    private Animator selfAnim;

    private Button rightBtn;
    private Button leftBtn;
    private Button returnBtn;
    private Button confirmBtn;
    private void Awake()
    {
        rightBtn = transform.Find("Right").GetComponent<Button>();
        leftBtn  = transform.Find("Left").GetComponent<Button>();
        returnBtn =transform.Find("ReturnBtn").GetComponent<Button>();
        confirmBtn=transform.Find("Confirm").GetComponent<Button>();
        rightBtn.onClick.AddListener(delegate
                            {
                                ChangeClothCallBack(1);
                            }
                         );
        leftBtn.onClick.AddListener(delegate
                            {
                                ChangeClothCallBack(-1);
                            }
                         );
        returnBtn.onClick.AddListener(ReturnBtnCallBack);
        confirmBtn.onClick.AddListener(ConfirmBtnCallBack);
        this.transform.Find("Name").GetComponent<Text>().text = names[playerMatID];
        selfAnim = transform.Find("PlayerModel").GetComponent<Animator>();
        selfAnim.SetBool("Dance",true);
    }
    void OnEnable(){
        playerMatID = PlayerPrefs.GetInt("PlayerMatID",0);
        curMatID    = playerMatID;
        transform.Find("PlayerModel").GetComponent<SkinnedMeshRenderer>().material = Resources.Load<Material>("Materials/Player/Player" + playerMatID.ToString()) as Material;
    }


    //换装按钮回调
    public void ChangeClothCallBack(int type){
		curMatID += type ;
		if(curMatID <0 || curMatID > 11){
			curMatID -=type;
			return;
		}
		else{
			this.transform.Find("PlayerModel").GetComponent<SkinnedMeshRenderer>().material = Resources.Load<Material>("Materials/Player/Player" + curMatID.ToString()) as Material;
            this.transform.Find("Name").GetComponent<Text>().text = names[curMatID];
        }
		Debug.Log(curMatID);
	}
	//确认按钮回调
	public void ConfirmBtnCallBack(){
		PlayerPrefs.SetInt("PlayerMatID", curMatID);
		Debug.Log(PlayerPrefs.GetInt("PlayerMatID"));
        if (gameObject.activeInHierarchy)
        { 
            map.SetActive(true);
            gameObject.SetActive(false);
            setBtn.SetActive(true);
            selfAnim.SetBool("Dance",false);
        }
	}
    public void ReturnBtnCallBack()
    {
        map.SetActive(true);
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            setBtn.SetActive(true);
            selfAnim.SetBool("Dance",false);
        }
    }

    public void SetBtnCallBack()
    {
        if (map.activeInHierarchy)
        {
            map.SetActive(false);
            setBtn.SetActive(false);
        }
        gameObject.SetActive(true); 
        selfAnim.SetBool("Dance",true);
    }
}
