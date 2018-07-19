using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform self_RT;
    [HideInInspector]
    public GameObject UIunit;//储存这个按钮所绑定的 unit_part 的实例，这个实例由 BlueprintButton 脚本创建
    Transform content;//所有 UnitButton 被本脚本创建后需要挂在这个物体下面进行自动位置排布
    [HideInInspector]
    public UnitStruct self_unit_struct;

    UnitBlueprint unit_blueprint;

    Text name_text;
    Image ban_image;
    iIcon self_i_icon;
    [HideInInspector]
    public Image hover_image;
    [HideInInspector]
    public GameObject part_particulars_UI;

    public bool Init(GameObject _UIunit)
    {
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == "HoverImage")
            {
                hover_image = t.GetComponent<Image>();

            }
            else if (t.name == "BanImage")
            {
                ban_image = t.GetComponent<Image>();
            }
            else if (t.name == "UnitName")
            {
                name_text = t.GetComponent<Text>();
            }
            else if (t.name == "iIcon")
            {
                //self_i_icon = t.GetComponent<iIcon>();                             
            }
        }

        self_RT = transform as RectTransform;
        string unit_name = _UIunit.name;
        name = unit_name + "Button";
        name_text.text = unit_name;
        UIunit = _UIunit;

        //找到 UnitInterfaceContent ：
        //content = GameObject.Find("UICanvasBase/UnitInterfaceGround/UnitInterfaceBackGround/UnitInterface/Viewport/UnitInterfaceContent").transform;

        UIunit.transform.position = transform.position;
        UIunit.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        UIunit.gameObject.AddComponent<UIPartBehaviour>();
        hover_image.enabled = false;

        return true;
    }

	void Update ()
    {
        SetUIunitPos();

    }
    void SetUIunitPos()
    {
        UIunit.transform.position = transform.position;
    }

    public void ClickButton()
    {
        //先清空设计平台：
        Debug.LogError("UnitButton 即将清空设计平台");
        unit_blueprint.CleanAllPart();

        StartCoroutine(CreatUnit());
           
    }

    IEnumerator CreatUnit()
    {
        while (!unit_blueprint.clean_all_part_complete)
        {
            Debug.LogError("没有清理完成，协程 CreatUnit() 还在等待");
            yield return new WaitForEndOfFrame();
        }
        Debug.LogError("清理完成");

        Dictionary<PartSerializeStruct, Part> temp_dict = new Dictionary<PartSerializeStruct, Part>();

        //根据当前unit结构体里的信息，先把需要用到的零件都实例化到设计平台的原位置下：           
        foreach (var pss in self_unit_struct.all_part_serialize_struct)
        {
            switch (pss.part_type_int)
            {
                case 0:
                    GameObject power_part_prefab = Instantiate(Resources.Load<GameObject>("Part/PowerPart/" + pss.part_prefab_name));
                    power_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                    power_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                    //Destroy(power_part_prefab.GetComponent<PowerPart>());
                    //Destroy(power_part_prefab.GetComponent<BoxCollider>());
                    //Destroy(power_part_prefab.GetComponent<Rigidbody>());

                    power_part_prefab.transform.SetParent(unit_blueprint.transform);

                    temp_dict.Add(pss, power_part_prefab.GetComponent<Part>());
                    power_part_prefab.GetComponent<PowerPart>().Init();//手动初始化，因为Start执行顺序有问题

                    break;
                case 1:
                    GameObject leg_part_prefab = Instantiate(Resources.Load<GameObject>("Part/MovementPart/LegPart/" + pss.part_prefab_name));
                    leg_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                    leg_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                    //Destroy(leg_part_prefab.GetComponent<PowerPart>());
                    //Destroy(leg_part_prefab.GetComponent<BoxCollider>());
                    //Destroy(leg_part_prefab.GetComponent<Rigidbody>());

                    leg_part_prefab.transform.SetParent(unit_blueprint.transform);

                    temp_dict.Add(pss, leg_part_prefab.GetComponent<Part>());
                    leg_part_prefab.GetComponent<LegPart>().Init();//手动初始化，因为Start执行顺序有问题

                    break;
                case 2:
                    GameObject ctrl_part_prefab = Instantiate(Resources.Load<GameObject>("Part/ControlPart/" + pss.part_prefab_name));
                    ctrl_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                    ctrl_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                    //Destroy(ctrl_part_prefab.GetComponent<PowerPart>());
                    //Destroy(ctrl_part_prefab.GetComponent<BoxCollider>());
                    //Destroy(ctrl_part_prefab.GetComponent<Rigidbody>());

                    ctrl_part_prefab.transform.SetParent(unit_blueprint.transform);

                    temp_dict.Add(pss, ctrl_part_prefab.GetComponent<Part>());
                    ctrl_part_prefab.GetComponent<ControlPart>().Init();//手动初始化，因为Start执行顺序有问题

                    break;
                case 3:
                    GameObject E_part_prefab = Instantiate(Resources.Load<GameObject>("Part/EquipmentPart/" + pss.part_prefab_name));
                    E_part_prefab.transform.position = new Vector3(pss.pos[0], pss.pos[1], pss.pos[2]);
                    E_part_prefab.transform.rotation = Quaternion.Euler(pss.rotation[0], pss.rotation[1], pss.rotation[2]);

                    //Destroy(E_part_prefab.GetComponent<PowerPart>());
                    //Destroy(E_part_prefab.GetComponent<BoxCollider>());
                    //Destroy(E_part_prefab.GetComponent<Rigidbody>());

                    E_part_prefab.transform.SetParent(unit_blueprint.transform);

                    temp_dict.Add(pss, E_part_prefab.GetComponent<Part>());
                    E_part_prefab.GetComponent<EquipmentPart>().Init();//手动初始化，因为Start执行顺序有问题

                    break;
            }

        }

        //上面的操作跑完后，零件只是被实例化然后恢复了位置信息，互相的节点连接关系和设计面板信息还没有恢复：
        // 恢复连接关系：
        foreach (var k_PSS in temp_dict)
        {
            foreach (var k_MPS in k_PSS.Key.MP_struct_dict)
            {
                foreach (MountPoint mp in k_PSS.Value.mount_points)
                {
                    if (mp.ID == k_MPS.Key)
                    {
                        foreach (Part p in temp_dict.Values)
                        {
                            if (p.ID == k_MPS.Value.link_info[0] && p.orderID == k_MPS.Value.link_info[1])
                            {
                                //从这个设计所有用到的零件里翻找，直到找到与存档里 k_MPS.Value.link_info 所储存信息完全相符的那个零件的实例：
                                //进行连接：
                                mp.Link2MountPoint(p.mount_points[k_MPS.Value.link_info[2]]);
                                break;
                            }
                        }

                    }

                }
            }
        }
        //分别恢复父子列表中的关系：
        foreach (var k_PSS in temp_dict)
        {
            foreach (Part p in temp_dict.Values)
            {
                if (k_PSS.Key.parent_part[0] == -999) continue;
                if (p.ID == k_PSS.Key.parent_part[0] && p.orderID == k_PSS.Key.parent_part[1])
                {
                    //从这个设计所有用到的零件里遍历，直到找到与存档里 k_PSS.Key.parent_part 所储存信息完全相符的那个零件的实例，就是当前零件的父级别零件：
                    k_PSS.Value.parent_part = p;
                    continue;
                }
                //如果当前被遍历的零件不是  k_PSS.Value 的父级零件，就看看他是不是其的子级零件：
                foreach (int[] cp in k_PSS.Key.child_part)
                {
                    if (p.ID == cp[0] && p.orderID == cp[1])
                    {
                        k_PSS.Value.child_part.Add(p);
                        break;
                    }
                }
            }

        }
        //恢复辅助模块安装信息：
        Sprite[] module_sprite = Resources.LoadAll<Sprite>("Icon/ModuleIcons");
        for (int i = 0; i < self_unit_struct.all_modules_savedata.Count; i++)
        {
            ModuleButtonInUnitInfoInterface temp = UnitInfo.GetUnitInfo.modules_script[i];
            ModuleSaveData now_data = self_unit_struct.all_modules_savedata[i];
            temp.installed = true;
            temp.click_text.gameObject.SetActive(false);
            temp.icon.sprite = module_sprite[now_data.self_iconID_in_sprite];
            temp.icon.gameObject.SetActive(true);
            temp.self_iconID_in_sprite = now_data.self_iconID_in_sprite;
            temp.name_text.text = now_data.name;
            temp.name_text.gameObject.SetActive(true);

        }

        Debug.LogError("实例化完成，位置信息恢复");

        //恢复设计面板名字信息:

        UnitInfo.GetUnitInfo.unit_name_text.GetComponent<InputField>().text = self_unit_struct.unit_name;
        Debug.LogError("self_unit_struct.unit_name = " + self_unit_struct.unit_name);

        //开放所有零件的 deserialize_over 为 true
        foreach (Part part in temp_dict.Values)
        {
            unit_blueprint.AddPart2Unit(part);
        }
        foreach (Part part in temp_dict.Values)
        {
            part.deserialize_over = true;
        }

        hover_image.enabled = false;
        BlueprintButton.GetBlueprintButton.ClickCloseUnitInterfaceButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover_image.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hover_image.enabled = false;
    }
}
