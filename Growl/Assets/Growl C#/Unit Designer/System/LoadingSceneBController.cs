using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneBController : MonoBehaviour
{
    public RawImage raw_image;
    public Text loading_value;
    public Image loading_image;
    int max_index;
    float loading_value_atstart;
    float _value;

    private void Awake()
    {
        //一进入场景，就先设置进度条数值：
        loading_value_atstart = InfoTransfer.GetInfoTransfer.loading_value_now;
        _value = loading_value_atstart;
        if (_value <= 50) _value = 50;
        loading_image.transform.rotation = InfoTransfer.GetInfoTransfer.loading_anim_rotation_state;
        SetLoadingValue(loading_value_atstart);
        StartCoroutine(WaitForLoading());
    }

    void Start ()
    {
		
	}
	

	void Update ()
    {
        if (_value < 100)
        {
            SetLoadingValue(++_value);
        }
    }

    IEnumerator WaitForLoading()
    {
        while (BlueprintButton.GetBlueprintButton == null)
        {
            yield return new WaitForEndOfFrame();
        }
        while (UserDataController.GetSingleton == null)
        {
            yield return null;
        }

        max_index = UserDataController.GetSingleton.UD.user_blueprints.Count;

        while (!BlueprintButton.GetBlueprintButton.load_user_data_over)
        {
            //_value = loading_value_atstart + (BlueprintButton.GetBlueprintButton.end_index / max_index) * 50;
            //SetLoadingValue(_value);
            yield return new WaitForEndOfFrame();
        }

        _value = 100;
        SetLoadingValue(_value);
        yield return new WaitForSeconds(2f);
        Destroy(raw_image.gameObject);
        Destroy(loading_value);
        Destroy(loading_image);
        Destroy(gameObject);
    }
    void SetLoadingValue(float value)
    {
        loading_value.text = String.Format("{0}{1}", value.ToString(), "%");
    }
}
