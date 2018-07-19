using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GotoGameStartScene : MonoBehaviour
{
    public VideoPlayer vp;
    AsyncOperation op;

    private void Awake()
    {
        Cursor.visible = false;
    }
    void Start ()
    {
        vp.loopPointReached += EndOfVideo;
    }
	

	void Update ()
    {
        if (op != null) Debug.LogError("jcdajdkl" + op.progress);


    }
    
    void EndOfVideo(VideoPlayer _vp)
    {
        op = SceneManager.LoadSceneAsync("GameStartScene");
    }
    
}
