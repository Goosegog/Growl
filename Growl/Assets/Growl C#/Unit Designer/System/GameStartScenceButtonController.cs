using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartScenceButtonController : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public void ClickUnitDesignerButton()
    {
        //保存需要加载的目标场景
        //Globe.nextSceneName = "Scene";

        SceneManager.LoadScene("Loading");
    }
    
}
