using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



    public class MainMenuHandler : MonoBehaviour
    {

        // views
        public GameObject createBlueprintView;
        public GameObject loadBlueprintView;
        public GameObject optionsMenuView;
        public GameObject mainMenuView;

        // Popup views 
        public GameObject DeleteBlueprintPopup;
        public GameObject copystringPopup;
        public GameObject LoadBlueprintEditor;
        public GameObject IconSelectionView;

        // Exit buttons
        private Button exitDeletePopup;
        private Button exitCopystringPopup;
        private Button exitLoadBlueprintPopup;
        private Button exitIconSelectionPopup;

        // Blur
        public GameObject blurImage;

        void Start()
        {
            mainmenuPage();

            DeleteBlueprintPopup.SetActive(false);
            copystringPopup.SetActive(false);
            LoadBlueprintEditor.SetActive(false);
            IconSelectionView.SetActive(false);
            blurImage.SetActive(false);

            exitDeletePopup = DeleteBlueprintPopup.transform.Find("Exit").GetComponent<Button>();
            exitCopystringPopup = copystringPopup.transform.Find("Exit").GetComponent<Button>();
            exitLoadBlueprintPopup = LoadBlueprintEditor.transform.Find("Exit").GetComponent<Button>();
            exitIconSelectionPopup = IconSelectionView.transform.Find("Exit").GetComponent<Button>();

        }

    void Update()
    {
        exitDeletePopup.onClick.RemoveAllListeners();
        exitDeletePopup.onClick.AddListener(() =>
        {
            DeleteBlueprintPopup.SetActive(false);
            blurImage.SetActive(false);
        });

        exitCopystringPopup.onClick.RemoveAllListeners();
        exitCopystringPopup.onClick.AddListener(() =>
        {
            copystringPopup.SetActive(false);
            blurImage.SetActive(false);
        });

        exitLoadBlueprintPopup.onClick.RemoveAllListeners();
        exitLoadBlueprintPopup.onClick.AddListener(() =>
        {
            LoadBlueprintEditor.SetActive(false);
            blurImage.SetActive(false);
        });

        exitIconSelectionPopup.onClick.RemoveAllListeners();
        exitIconSelectionPopup.onClick.AddListener(() =>
        {
            IconSelectionView.SetActive(false);
            blurImage.SetActive(false);
        });

    }


    //activate the createBlueprint page
    public void createBlueprintPage()
        {
            mainMenuView.SetActive(false);
            loadBlueprintView.SetActive(false);
            optionsMenuView.SetActive(false);
            createBlueprintView.SetActive(true);
        }

        public void loadBlueprintPage()
        {
            mainMenuView.SetActive(false);
            createBlueprintView.SetActive(false);
            optionsMenuView.SetActive(false);
            loadBlueprintView.SetActive(true);
        }

        public void optionspage()
        {
            mainMenuView.SetActive(false);
            createBlueprintView.SetActive(false);
            loadBlueprintView.SetActive(false);
            optionsMenuView.SetActive(true);

        }

        //activate the mainMenu page
        public void mainmenuPage()
        {
            createBlueprintView.SetActive(false);
            loadBlueprintView.SetActive(false);
            optionsMenuView.SetActive(false);
            mainMenuView.SetActive(true);
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


    
}