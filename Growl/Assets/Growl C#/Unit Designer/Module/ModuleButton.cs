using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleButton : MonoBehaviour
{
    [HideInInspector]
    public Sprite[] sp;
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public string module_name;
    [HideInInspector]
    public Image self_icon;
    [HideInInspector]
    public int self_iconID_in_sprite;
    [HideInInspector]
    public int number_of_use;
    [HideInInspector]
    public int use_power;

    Text self_name_text;
    [HideInInspector]
    public GameObject info;
    [HideInInspector]
    public ModuleButtonInfoIcon self_i_icon;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void Init(Sprite[] _sp, int _ID, string _module_name, int _icon_ID, int _number_of_use, string _info)
    {
        ID = _ID;
        module_name = _module_name;
        number_of_use = _number_of_use;
        sp = _sp;

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "Icon")
            {
                self_icon = t.GetComponent<Image>();
                self_iconID_in_sprite = _icon_ID;
                self_icon.sprite = _sp[_icon_ID];
            }
            else if (t.name == "Name")
            {
                self_name_text = t.GetComponent<Text>();
                self_name_text.text = module_name;
            }
            else if(t.name == "iIcon")
            {
                //这个时候 iIcon 上的 ModuleButtonInfoIcon 脚本还没有实例化完，因此开一个协程等他初始化完再给其字段赋值
                StartCoroutine(InitModuleButtonInfoIcon(t));
            }
            else if(t.name == "Info")
            {
                info = t.gameObject;

                foreach(Transform tt in t.GetComponentsInChildren<Transform>())
                {
                    if (tt.name == "NameText")
                    {
                        tt.GetComponent<Text>().text = _module_name;
                    }
                    else if(tt.name == "NumberOfUseText")
                    {
                        if (_number_of_use == -999)
                        {
                            tt.GetComponent<Text>().text = "辅助模块";
                        }
                        else
                        {
                            tt.GetComponent<Text>().text = "可用次数  " + _number_of_use;
                        }
                        
                    }
                    else if(tt.name == "InfoText")
                    {
                        tt.GetComponent<Text>().text = _info;
                    }
                }
            }
        }

        info.SetActive(false);

    }

    IEnumerator InitModuleButtonInfoIcon(Transform t)
    {
        while (!t.GetComponent<ModuleButtonInfoIcon>())
        {
            yield return new WaitForEndOfFrame();
        }

        self_i_icon = t.GetComponent<ModuleButtonInfoIcon>();
        self_i_icon.Init(this);

    }

    public void ClickButton()
    {
        ModuleButtonInUnitInfoInterface temp = ModuleController.GetModuleController.who_called_ModuleInterface;

        temp.Install(self_icon.sprite, self_iconID_in_sprite, self_name_text.text);

        ModuleController.GetModuleController.CloseModuleInterface();
    }

    
}
