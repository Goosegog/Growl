using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackImageInGameStartScene : MonoBehaviour
{
    Image self_image;

	void Start ()
    {
        self_image = GetComponent<Image>();
	}

	void Update ()
    {
        self_image.color = new Color(self_image.color.r, self_image.color.g, self_image.color.b, self_image.color.a - 0.01f);
        //Debug.LogError(self_image.color.a);
        if(self_image.color.a <= 0.75)
        {
            Cursor.visible = true;
        }
        if (self_image.color.a <= 0)
        {                      
            Destroy(gameObject);
        }
            
	}
}
