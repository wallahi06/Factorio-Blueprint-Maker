using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuHandler : MonoBehaviour
{

    // pages
    public GameObject createBlueprint;
    public GameObject loadBlueprint;
    public GameObject optionsMenu;
    public GameObject mainMenu;


    //activate the createBlueprint page
    public void createBlueprintPage()
    {
        mainMenu.SetActive(false);
        loadBlueprint.SetActive(false);
        optionsMenu.SetActive(false);
        createBlueprint.SetActive(true);
    }

    public void loadBlueprintPage()
    {
        mainMenu.SetActive(false);
        createBlueprint.SetActive(false);
        optionsMenu.SetActive(false);
        loadBlueprint.SetActive(true);
    }

    public void optionspage()
    {
        mainMenu.SetActive(false);
        createBlueprint.SetActive(false);
        loadBlueprint.SetActive(false);
        optionsMenu.SetActive(true);

    }

    //activate the mainMenu page
    public void mainMenuPage()
    {
        createBlueprint.SetActive(false);
        loadBlueprint.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }


    //create new JSON and load the JSON
    public void loadBlueprintScene()
    {
        SceneManager.LoadScene("blueprintBuilder");
    }

    public void loadMainMenuScene()
    {
        SceneManager.LoadScene("mainMenu");
    }

    // on start run this
    void Start()
    {
        mainMenuPage();

    }
}
