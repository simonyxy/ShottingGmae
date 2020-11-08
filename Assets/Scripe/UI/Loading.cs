using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Loading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("GameLoading", 3.5f);
    }
    void GameLoading()
    {
        SceneManager.LoadSceneAsync(1);
    }


}
