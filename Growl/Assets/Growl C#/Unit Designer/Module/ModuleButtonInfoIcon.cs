using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModuleButtonInfoIcon : MonoBehaviour
{
    [HideInInspector]
    public ModuleButton self_parent;

    public void Init(ModuleButton _sel_parent)
    {
        self_parent = _sel_parent;
    }

    public void PointerEnter()
    {
        Debug.LogError("OnPointerEnter");
        self_parent.info.SetActive(true);
    }
    public void PointerExit()
    {
        Debug.LogError("OnPointerExit");
        self_parent.info.SetActive(false);
    }
}
