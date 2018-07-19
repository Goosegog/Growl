using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemButtonController : MonoBehaviour
{
    UnitBlueprint unit_blueprint;
    GameObject systemmask;
    GameObject warining_ReturnGameStartSceneButton;
    GameObject warining_QuitGameButton;


    void Start ()
    {
        unit_blueprint = GameObject.Find("DesignPlatform/Unit").GetComponent<UnitBlueprint>();

        systemmask = GameObject.Find("UICanvasTop/SystemMask");
        warining_ReturnGameStartSceneButton = GameObject.Find("UICanvasTop/SystemMask/Menu/ReturnGameStartSceneWarning");
        warining_QuitGameButton = GameObject.Find("UICanvasTop/SystemMask/Menu/QuitGameWarning");

        warining_ReturnGameStartSceneButton.SetActive(false);
        warining_QuitGameButton.SetActive(false);
        systemmask.SetActive(false);
        unit_blueprint.menu_open = false;
    }
	

    public void ClickSystemButton()
    {
        systemmask.SetActive(true);
        unit_blueprint.menu_open = true;
    }
    public void ClickReturnButton()
    {
        systemmask.SetActive(false);
        unit_blueprint.menu_open = false;
    }
    public void CkilckQuitButton()
    {
        warining_QuitGameButton.SetActive(true);
    }
    public void ClickYesInQuitButton()
    {
        Application.Quit();
    }
    public void ClickNoInQuitButton()
    {
        warining_QuitGameButton.SetActive(false);
    }
    public void ClickReturnGameStartScene()
    {
        warining_ReturnGameStartSceneButton.SetActive(true);
    }
    public void ClickYesInReturnGameStartScene()
    {
        InfoTransfer.GetInfoTransfer.want_loadingscene_name = "GameStartScene";
        SceneManager.LoadSceneAsync("LoadScence");
    }
    public void ClickNoInReturnGameStartScene()
    {
        warining_ReturnGameStartSceneButton.SetActive(false);
    }

}
