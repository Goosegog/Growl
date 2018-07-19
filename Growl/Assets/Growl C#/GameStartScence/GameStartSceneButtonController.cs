using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartSceneButtonController : MonoBehaviour
{
    // Use this for initialization
    AsyncOperation op;
    public List<RectTransform> allburrons_origin_pos;
    public List<RectTransform> allburrons_end_pos;
    public List<RectTransform> all_buttons;

    private void Awake()
    {
        for(int i = 0; i < allburrons_origin_pos.Count; i++)
        {
            all_buttons[i].position = allburrons_origin_pos[i].position;
        }
    }
    void Start ()
    {
        float time = 0.5f;

        for (int i = 0; i < allburrons_origin_pos.Count; i++)
        {
            int tmp = i;
            Tweener buttonTweener = all_buttons[i].DOMoveX(allburrons_end_pos[i].position.x, time);
                //Debug.Log("..........."+ tmp);
                /*all_buttons[tmp].GetComponent<AudioSource>().Play(); }*/

            //buttonTweener.Pause();
            //buttonTweener.SetAutoKill(false);
            //buttonTweener.Play();
            time += 0.25f;
        }
      
    }
	
            
	void Update ()
    {
        if (op != null)
            Debug.LogError(op.progress);
	}

    public void ClickUnitDesigner()
    {
        StartCoroutine(GotoUnitDesignerScene());

        InfoTransfer.GetInfoTransfer.want_loadingscene_name = "Unit Designer";
        //SceneManager.LoadScene("LoadScence");
        //op = SceneManager.LoadSceneAsync("LoadScence");

    }
    IEnumerator GotoUnitDesignerScene()
    {
        yield return new WaitForSeconds(1f);
        InfoTransfer.GetInfoTransfer.want_loadingscene_name = "Unit Designer";
        SceneManager.LoadSceneAsync("LoadScence");
    }
}
