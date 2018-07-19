using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleController : MonoBehaviour
{
    static ModuleController self;
    public static ModuleController GetModuleController
    {
        get
        {
            return self;
        }
    }

    UnitBlueprint unit_blueprint;

    Dictionary<int, ModuleButton> module_button_dict;
    Transform content;
    Transform module_interface;
    [HideInInspector]
    public ModuleButtonInUnitInfoInterface who_called_ModuleInterface;


    void Start ()
    {
        self = this;
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        module_button_dict = new Dictionary<int, ModuleButton>();
        content = GameObject.Find("UICanvasBase/ModuleInterface/Scroll View/Viewport/Content").transform;
        module_interface = GameObject.Find("UICanvasBase/ModuleInterface").transform;

        LoadConfigurationTableAtSceneStart();
        CloseModuleInterface();

    }
	
	void Update ()
    {
		
	}
    public void LoadConfigurationTableAtSceneStart()
    {
        Sprite[] SP = Resources.LoadAll<Sprite>("Icon/ModuleIcons");

        JsonData itemData = JsonMapper.ToObject(Resources.Load<TextAsset>("ConfigurationTable/ModuleConfigurationTable").text.ToString());
        Debug.LogError(itemData.Count);
        for (int i = 0; i < itemData.Count; ++i)
        {
            int id = int.Parse(itemData[i]["ID"].ToString());
            string name = itemData[i]["Name"].ToString();
            int icon_id = int.Parse(itemData[i]["IconID"].ToString());
            int number_of_use = int.Parse(itemData[i]["NumberOfUse"].ToString());
            string info = itemData[i]["Info"].ToString();

            if (!module_button_dict.ContainsKey(id))
            {
                ModuleButton module_button_temp = Instantiate(Resources.Load<GameObject>("UI/ModuleButton/ModuleButtonDefault"), content).GetComponent<ModuleButton>();
                module_button_temp.Init(SP, id, name, icon_id, number_of_use, info);


                module_button_dict.Add(module_button_temp.ID, module_button_temp);
            }
            else
            {
                Debug.LogError("严重错误：配置表中存在 ID相同 的模块信息！");
            }
        }


    }

    public void OpenModuleInterface(ModuleButtonInUnitInfoInterface click_this)
    {
        who_called_ModuleInterface = click_this;

        module_interface.gameObject.SetActive(true);
        unit_blueprint.menu_open = true;
        
    }

    public void CloseModuleInterface()
    {
        module_interface.gameObject.SetActive(false);
        unit_blueprint.menu_open = false;
    }
    public void DeleteModuleNow()
    {
        who_called_ModuleInterface.Uninstall();
        CloseModuleInterface();
    }
}
