using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartParticulars : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public PartButton parent_button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //parent_button.hover_image.enabled = true;
        //gameObject.SetActive(true);
        //transform.position = parent_button.transform.position;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        parent_button.hover_image.enabled = false;
        gameObject.SetActive(false);
    }
    
}
