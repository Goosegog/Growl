using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BlackVideoInGameStartScene : MonoBehaviour
{
    public VideoPlayer vp;
    public RawImage ri;

    void Start ()
    {
        vp.loopPointReached += EndOfVideo;
        //Debug.LogError(vp.frameCount);
    }
	
	void Update ()
    {
        Debug.LogError(vp.frameRate);
        if (ri && vp.frameRate >= 20 )
        {
            Destroy(ri.gameObject);
        }
	}

    void EndOfVideo(VideoPlayer _vp)
    {
        vp.Stop();
    }
}
