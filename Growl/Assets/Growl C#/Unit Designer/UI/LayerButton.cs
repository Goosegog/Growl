using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayerButton : MonoBehaviour
{
    [HideInInspector]
    public Sprite[] sp;
    [HideInInspector]
    public Image self_image;
    [HideInInspector]
    public bool pressed = false;

    void Start ()
    {
        sp = Resources.LoadAll<Sprite>("Image/LayerButtonImage");
        self_image = GetComponent<Image>();
        self_image.sprite = sp[0];
    }

    public void OnPointerEnter()
    {
        if (!pressed) self_image.sprite = sp[1];
    }
    public void OnPointerExit()
    {
        if (!pressed) self_image.sprite = sp[0];
    }




}
