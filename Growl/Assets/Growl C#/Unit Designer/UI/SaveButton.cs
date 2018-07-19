using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    #region 重要外部引用
    UnitBlueprint unit_blueprint;
    BlueprintButton blueprint_button;
    #endregion
    Button self_button;
    [HideInInspector]
    public Sprite[] sp;
    [HideInInspector]
    public Image self_image;
    Text save_button_text;
    GameObject loading_anim;
    GameObject warning_icon;
    GameObject success_icon;

    void Start ()
    {
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();      
        self_button = GetComponent<Button>();
        sp = Resources.LoadAll<Sprite>("Image/LayerButtonImage");
        self_image = GetComponent<Image>();
       
        foreach (Transform t in GetComponentInChildren<Transform>())
        {
            if(t.name == "ButtonName")
            {
                save_button_text = t.GetComponent<Text>();
            }
            else if (t.name == "SaveLoadingAnim")
            {
                loading_anim = t.gameObject;
            }
            else if (t.name == "WarningIcon")
            {
                warning_icon = t.gameObject;             
            }
            else if(t.name == "SuccessIcon")
            {
                success_icon = t.gameObject;
            }
        }

        self_image.sprite = sp[3];
        loading_anim.SetActive(false);
        warning_icon.SetActive(false);
        success_icon.SetActive(false);
        ChangeColorByUnitLegal();
        //CheckUnitLegalToSetButton();
    }
	

	void Update ()
    {
        ChangeColorByUnitLegal();
    }

    //public void CheckUnitLegalToSetButton()
    //{
    //    if (UnitInfo.GetUnitInfo == null)
    //    {
    //        //说明单例还没有初始化好，直接返回，不影响
    //        return;
    //    }
    //    if (!UnitInfo.GetUnitInfo.legal && self_button.enabled)
    //    {
    //        //说明当前的设计不合法但是开关组件还开着
    //        self_button.enabled = false;
    //        save_button_text.text = "<color=#7B7B7B>SAVE</color>";
    //        return;
    //    }
    //    if(UnitInfo.GetUnitInfo.legal && !self_button.enabled)
    //    {
    //        self_button.enabled = true;
    //        save_button_text.text = "<color=#68DB86>SAVE</color>";
    //    }

    //}
    public void ChickSaveButton()
    {
        if (UnitInfo.GetUnitInfo == null)
        {
            //说明单例还没有初始化好，直接返回，不影响
            return;
        }
        if (!UnitInfo.GetUnitInfo.legal)
        {
            //说明当前的设计不合法            
            warning_icon.SetActive(true);
            StartCoroutine(CloseWariningIcon());
        }
        else
        {
            //开启保存动画：
            loading_anim.SetActive(true);

            //【重要】当前的设计合法，可以被保存，先调用蓝图下的方法生成当前设计的 UnitStruct 并将其添加到用户数据控制器单例的用户数据字段中：
            unit_blueprint.SaveUnitToUserDataControllerUD();

            //之后调用 UserDataController 单例的序列化方法：
            UserDataController.GetSingleton.SaveUserData();
            //调用蓝图脚本的生成新按钮的方法：
            BlueprintButton.GetBlueprintButton.CreatUnitPartLastOne();
            loading_anim.SetActive(false);            
            success_icon.SetActive(true);
            StartCoroutine(CloseSuccessIcon());
        }
    }
     
    IEnumerator CloseWariningIcon()
    {
        yield return new WaitForSeconds(1.5f);
        warning_icon.SetActive(false);
       
    }
    IEnumerator CloseSuccessIcon()
    {
        yield return new WaitForSeconds(1.0f);
        success_icon.SetActive(false);
    }
    void ChangeColorByUnitLegal()
    {
        if (UnitInfo.GetUnitInfo == null)
        {
            return;
        }
        if (!UnitInfo.GetUnitInfo.legal)
        {
            save_button_text.text = "<color=#7B7B7B>SAVE</color>";
            if(self_image.sprite != sp[3]) self_image.sprite = sp[3];
        }
        else
        {
            save_button_text.text = "<color=#68DB86>SAVE</color>";
            if(self_image.sprite != sp[0] && self_image.sprite != sp[1])
            {
                self_image.sprite = sp[0];
            }
        }
    }

    public void OnPointerEnter()
    {
        self_image.sprite = sp[1];
    }
    public void OnPointerExit()
    {
        self_image.sprite = sp[0];
    }
}
