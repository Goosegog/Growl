using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class iIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public PartButton parent_button;
    void Start ()
    {
		
	}
	
	void Update ()
    {
        //transform.position = parent_button.transform.position;
	}
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        parent_button.hover_image.enabled = true;
        parent_button.part_particulars_UI.SetActive(true);
        parent_button.part_particulars_UI.transform.position = parent_button.transform.position;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //parent_button.hover_image.enabled = false;
        //gameObject.SetActive(false);
    }
}
