using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[SerializeField]
public class SerializeUnitFromDesignMode : MonoBehaviour
{
    //这是一个实验脚本，没有作用

//===============================================================================================
    UnitBlueprint unit_blueprint;

    void Start ()
    {
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();
    }
	

	void Update ()
    {
		
	}

    void Save2Json()
    {
        //Application.persistentDataPath

        //先调用设计蓝图里的方法更新每个合法零件的次序ID：
        unit_blueprint.SetPartOrderID();

        foreach(var p in unit_blueprint.all_legal_parts)
        {
            switch ((int)p.part_type)
            {
                case 0:
                    var PP = p.GetComponent<PowerPart>();
                    JsonData data = new JsonData();
                    data["name"] = "Yang";
                    data["info"] = new JsonData();

                    var info1 = new JsonData();
                    info1["type"] = "student";
                    data["info"].Add(info1);
                    string json_ = JsonMapper.ToJson(data);
                    //File s = new File("");
                    //JsonWriter js = new JsonWriter();

                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
        

        JsonData unit_data;

        List<string> all_part_prefab_name;
        List<FitInfo> fit_info;//储存顺序是 零件ID，零件次序ID，节点ID，节点连接的零件ID，节点连接的零件的次序ID，节点连接的节点的ID
    }    
}
