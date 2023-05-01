/*// include libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using TMPro;


// Class with information about the position of the entity
public class Position
{
    public int x, y;
}


// Entity class that stores information about places entities
public class Entity
{
    public int entity_number { get; set; }
    public string name { get; set; }
    public Position position { get; set; }
    public int direction { get; set; }
}


// belongs to icon class and sets information about the icon
public class Signal
{
    public string type { get; set; }
    public string name { get; set; }
}


// Icon class, import the signal and sets an index
public class Icon
{
    public Signal signal { get; set; }
    public int index { get; set; }
}


// Blueprint class that stores all the information
public class BlueprintInformation
{
    public string label { get; set; }
    public string description { get; set; }
    public List<Entity> entities { get; set; }
    public List<Icon> icons { get; set; }
}


// Finished blueprint class
public class Blueprint
{
    public BlueprintInformation blueprintInformation { get; set; }
}


public class CreateBlueprint : MonoBehaviour
{

    Blueprint blueprint;

    private MainMenuHandler mainMenu;

    [SerializeField] public GameObject labelInputField;
    [SerializeField] public GameObject descriptionInputField;
    [SerializeField] public GameObject IconSlots;
    [SerializeField] public Transform IconObjectContainer;

    public GameObject scrollContainer;
    public GameObject IconSelectionView;
    public GameObject prefab;

    public int numOfOfObject = 4;
    private float spacing;
    private string label;
    private string description;
    private int icon_index;

    private GameObject childObject;


    // run on start
    void Start()
    {
        mainMenu = GetComponent<MainMenuHandler>();

        IconSelectionView.SetActive(false);
        InitiateIconSelection();

        float width = scrollContainer.GetComponent<RectTransform>().rect.width;

        spacing = width * 0.06f;
        Vector2 newGridSize = new Vector2((width - ((numOfOfObject + 1) * spacing)) / numOfOfObject, (width - ((numOfOfObject + 1) * spacing)) / numOfOfObject);
        Vector2 spacingTest = new Vector2(spacing, spacing);
        scrollContainer.GetComponent<GridLayoutGroup>().cellSize = newGridSize;
        scrollContainer.GetComponent<GridLayoutGroup>().spacing = spacingTest;

        scrollContainer.GetComponent<GridLayoutGroup>().padding = new RectOffset((int)spacing, (int)spacing, (int)spacing, (int)spacing);
    }


    // runs when the user presses the create blueprint button in the main menu
    public void blueprintCreateStart()
    {
        // create a new blueprint object
        createBlueprintObject();

        // get all the icon slot buttons and set the icon index
        foreach (Transform child in IconSlots.transform)
        {
            Button iconSlotButton = child.GetComponent<Button>();
            iconSlotButton.onClick.RemoveAllListeners();
            iconSlotButton.onClick.AddListener(() =>
            {
                if (iconSlotButton.name == "IconSlot 1")
                {
                    icon_index = 1;
                }
                else if (iconSlotButton.name == "IconSlot 2")
                {
                    icon_index = 2;
                }
                else if (iconSlotButton.name == "IconSlot 3")
                {
                    icon_index = 3;
                }
                else if (iconSlotButton.name == "IconSlot 4")
                {
                    icon_index = 4;
                }

                IconSelectionView.SetActive(true);

            });
        }
    }


    // creates an empty blueprint object that will later store information about the blueprint
    public void createBlueprintObject()
    {

        // sets all blueprint information parameters
        blueprint = new Blueprint()
        {
            blueprintInformation = new BlueprintInformation()
            {

                label = "",
                description = "",
                entities = new List<Entity>(),
                icons = new List<Icon>()
            }
        };
    }


    public void loadBlueprintInformation()
    {
        // sets the label and description to the values of the input fields
        label = labelInputField.GetComponent<TMP_InputField>().text;
        description = descriptionInputField.GetComponent<TMP_InputField>().text;

        // get all the files in the blueprint folder to check if the file already exists
        string path = Path.Combine(Application.dataPath, "blueprints/");
         DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] files = dir.GetFiles();
        bool exists = false;

        // iterates over all the files that has the extensino .json
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension == ".json")
            {
                // check if the file already exists
                if (Path.GetFileNameWithoutExtension(files[i].FullName) == label)
                {
                    exists = true;
                }
            } 
        }

        // if the file doesent exist, move on
        if (!exists)
        {
            // check if the input fields are empty or if the file already exists
            if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(description))
            {
                blueprint.blueprintInformation.label = label;
                blueprint.blueprintInformation.description = description;

                writeToFile();
                resetCreateFileView();

            }
            else
            {
                Debug.Log("There's empty fields");
            }
        } else
        {
            Debug.Log("The file already exists");
            resetCreateFileView();
        }     
    }


   


    // writes the blueprint to a blueprint file
    public void writeToFile()
    {

        // path were we store the blueprint file
        string folderPath = Path.Combine(Application.dataPath, "blueprints");
        string filePath = Path.Combine(folderPath, $"{label}.json");

        // checks if file exists, if not create one
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        // write the blueprintInformation to JSON file
        string blueprint_result = JsonConvert.SerializeObject(blueprint, Formatting.Indented);
        File.WriteAllText(filePath, blueprint_result);

    }


    // resets all the input fields and icon slots when file is created
    public void resetCreateFileView()
    {

        // reset all the input fields
        labelInputField.GetComponent<TMP_InputField>().text = "";
        descriptionInputField.GetComponent<TMP_InputField>().text = "";

        // reset the blueprintInformation parameters
        blueprint.blueprintInformation.label = null;
        blueprint.blueprintInformation.description = null;
        blueprint.blueprintInformation.icons = new List<Icon>();
        blueprint.blueprintInformation.entities = new List<Entity>();

        // iterates over all the icon slots and resets them
        for (int i = 0; i < IconSlots.transform.childCount; i++)
        {
            IconSlots.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = null;
            IconSlots.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(0.1921569f, 0.1921569f, 0.1921569f);
        }

        // reset the Icon placeholder
        IconSlots.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = null;
        IconSlots.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);

    }


    // gets all the files from the icon dictory and adds an object for each file
    public void InitiateIconSelection()
    {
        string path = "InventoryIcons";
        Object[] icons = Resources.LoadAll(path, typeof(Texture2D));

        foreach (Object icon in icons)
        {
            GameObject instance = Instantiate(prefab, scrollContainer.transform);
            instance.name = icon.name;
            instance.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(icon as Texture2D, new Rect(0, 0, (icon as Texture2D).width, (icon as Texture2D).height), Vector2.zero);

            Button iconButton = instance.GetComponent<Button>();
            iconButton.onClick.RemoveAllListeners();
            iconButton.onClick.AddListener(() =>
            {
                Icon icon = new Icon()
                {
                    signal = new Signal()
                    {
                        type = "item",
                        name = iconButton.name
                    },
                    index = icon_index
                };

                // add the icon to the blueprint
                blueprint.blueprintInformation.icons.Add(icon);

                Debug.Log(iconButton.name);

                IconSelectionView.SetActive(false);
            });
         

            Button exitButton = IconSelectionView.transform.Find("Exit").GetComponent<Button>();
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() =>
            {
                IconSelectionView.SetActive(false);
            });


        }

    }
}
*/