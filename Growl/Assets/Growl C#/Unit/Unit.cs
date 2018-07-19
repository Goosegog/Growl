using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    List<string> all_part_prefab_name;
    List<FitInfo> fit_info;//储存顺序是 零件ID，零件次序ID，节点ID，节点连接的零件ID，节点连接的零件的次序ID，节点连接的节点的ID


    public void Init()
    {
        all_part_prefab_name = new List<string>();
        fit_info = new List<FitInfo>();
    }
	
	void Update ()
    {
		
	}

    public void Fit()
    {

    }
}
