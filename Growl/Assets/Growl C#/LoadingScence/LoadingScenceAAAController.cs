using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScenceAAAController : MonoBehaviour
{
    int tool_frame_count = 0;

    string scene_name;
    public Text loading_value;
    public Image loading_image;
    AsyncOperation asyncop;
    int value;
    bool b = true;

    private void Awake()
    {
        Cursor.visible = false;
    }

    void Start()
    {
        scene_name = InfoTransfer.GetInfoTransfer.want_loadingscene_name;
        value = 0;
        //StartCoroutine(LoadAsync(/*scene_name*/));
        //StartCoroutine(LoadAsync());
        asyncop = SceneManager.LoadSceneAsync(scene_name);
        //op = SceneManager.LoadSceneAsync(InfoTransfer.GetInfoTransfer.want_loadingscene_name);
        //op.allowSceneActivation = false;
        //Debug.LogError("op.progress = " + op.progress);
    }


    void Update()
    {
        //tool_frame_count++;
        //if (tool_frame_count == 120)
        //{
        //    StartCoroutine(LoadAsync(/*scene_name*/));
        //    //asyncop = SceneManager.LoadSceneAsync(InfoTransfer.GetInfoTransfer.want_loadingscene_name);
        //}
        //if (asyncop != null)
        //{
        //    Debug.LogError("asyncop.progress = " + asyncop.progress);
        //    SetLoadingValue();
        //}

        SetLoadingValue();

    }

    IEnumerator LoadAsync()
    {

        //yield return null;

        asyncop = SceneManager.LoadSceneAsync(scene_name);
        Debug.LogError("异步加载开始");
        asyncop.allowSceneActivation = false;

        while (asyncop.progress < 1.0f)
        {
            //lp = (int)(op.progress * 50);
            //SetLoadingValue(lp);
            //InfoTransfer.GetInfoTransfer.loading_value_now = lp;
            //InfoTransfer.GetInfoTransfer.loading_anim_rotation_state = loading_image.transform.rotation;
            //yield return new WaitForEndOfFrame();

            Debug.LogError("asyncop.progress = " + asyncop.progress);
            if (asyncop.progress < 0.9f)
            {
                value = (int)(asyncop.progress * 50);
            }
            else
            {
                value = 50;
            }

            InfoTransfer.GetInfoTransfer.loading_value_now = value;
            InfoTransfer.GetInfoTransfer.loading_anim_rotation_state = loading_image.transform.rotation;

            loading_value.text = value.ToString() + "%";

            if (value == 50) asyncop.allowSceneActivation = true;

            yield return new WaitForEndOfFrame();
        }

        InfoTransfer.GetInfoTransfer.loading_value_now = value;
        InfoTransfer.GetInfoTransfer.loading_anim_rotation_state = loading_image.transform.rotation;

    }
    void SetLoadingValue()
    {
        //Debug.LogError("op.progress = " + op.progress);
        //if(asyncop.progress < 0.9f)
        //{
        //    value = (int)(asyncop.progress * 50);
        //}
        //else
        //{
        //    value = 50;
        //}

        
        if (value <= 49)
        {
            value++;
        }
        else
        {
            value = 50;
        }

        InfoTransfer.GetInfoTransfer.loading_value_now = value;
        InfoTransfer.GetInfoTransfer.loading_anim_rotation_state = loading_image.transform.rotation;

        //if (++value <= 50) loading_value.text = (++value).ToString() + "%";
        loading_value.text = value.ToString() + "%";

        if(value == 50) asyncop.allowSceneActivation = true;
       
        

    }
}
