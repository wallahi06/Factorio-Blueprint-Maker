// include libraries
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


    public GameObject scrollContainer;
    public GameObject prefab;
    public GameObject IconSlots;
    public GameObject labelInputField;
    public GameObject descriptionInputField;

    private MainMenuHandler mainMenu;

    private Blueprint blueprint;


    // Start is called before the first frame update
    void Start()
    {

        mainMenu = GetComponent<MainMenuHandler>();

    }


    private int icon_index;

    public void createBlueprintStart()
    {

        createBlueprintObject();

        foreach (Transform child in IconSlots.transform)
        {
            Button iconSlotButton = child.GetComponent<Button>();
            iconSlotButton.onClick.RemoveAllListeners();
            iconSlotButton.onClick.AddListener(() =>
            {
                if (iconSlotButton.name == "Slot 1")
                {
                    icon_index = 1;
                }
                else if (iconSlotButton.name == "Slot 2")
                {
                    icon_index = 2;
                }
                else if (iconSlotButton.name == "Slot 3")
                {
                    icon_index = 3;
                }
                else if (iconSlotButton.name == "Slot 4")
                {
                    icon_index = 4;
                }

                mainMenu.IconSelectionView.SetActive(true);
                setIcon();
            });
        }
    }


    public int numOfOfObject = 4;
    private float spacing;

    void setIcon()
    {
        // get the width of the scroller container
        float width = scrollContainer.GetComponent<RectTransform>().rect.width;

        // set the spacing to 6% of the total width
        spacing = width * 0.06f; 

        Vector2 newGridSize = new Vector2((width - ((numOfOfObject + 1) * spacing)) / numOfOfObject, (width - ((numOfOfObject + 1) * spacing)) / numOfOfObject);

        scrollContainer.GetComponent<GridLayoutGroup>().cellSize = newGridSize;
        scrollContainer.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacing, spacing);

        scrollContainer.GetComponent<GridLayoutGroup>().padding = new RectOffset((int)spacing, (int)spacing, (int)spacing, (int)spacing);


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
                bool indexExists = blueprint.blueprintInformation.icons.Exists(existingIcon => existingIcon.index == icon_index);
                if (indexExists)
                {
                    for (int i = 0; i <  blueprint.blueprintInformation.icons.Count; i++)
                    {
                        if (icon_index == blueprint.blueprintInformation.icons[i].index)
                        {
                            blueprint.blueprintInformation.icons[i].index = icon_index;
                            blueprint.blueprintInformation.icons[i].signal.name = iconButton.name;
                            blueprint.blueprintInformation.icons[i].signal.type = "item";
                        }
                    }
                } else
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

                    blueprint.blueprintInformation.icons.Add(icon);
                }

                Sprite newSprite = Resources.Load<Sprite>($"InventoryIcons/{iconButton.name}");

                IconSlots.transform.GetChild(icon_index - 1).GetComponent<Image>().sprite = newSprite;
                IconSlots.transform.GetChild(icon_index - 1).GetComponent<Image>().color = new Color(1, 1, 1);

                mainMenu.IconSelectionView.SetActive(false);
            });
        }

        Button exitButton = mainMenu.IconSelectionView.transform.Find("Exit").GetComponent<Button>();
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() =>
        {
            mainMenu.IconSelectionView.SetActive(false);
        });

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


    private string label;
    private string description;

    public void loadBlueprintInformation()
    {
        // sets the label and descriptino to the values of the input fields
        label = labelInputField.GetComponent<TMP_InputField>().text;
        description = descriptionInputField.GetComponent<TMP_InputField>().text;


        string filePath = Path.Combine(Application.streamingAssetsPath, $"blueprints/{label}.json");
        bool fileExists;

        if (File.Exists(filePath))
        {
            fileExists = true;
            Debug.Log("The File already exists");
        } else
        {
            fileExists = false;
        }

        

        // check if the input fields are empty
        if (!string.IsNullOrEmpty(labelInputField.GetComponent<TMP_InputField>().text) && !string.IsNullOrEmpty(descriptionInputField.GetComponent<TMP_InputField>().text))
        {
            if (!fileExists)
            {
                blueprint.blueprintInformation.label = label;
                blueprint.blueprintInformation.description = description;

                resetCreateFileView();
                writeToFile();
            }
        } else
        {
            Debug.Log("some input fields may be empty");
        }
    }


    // write the new blueprint to a blueprint file
    public void writeToFile()
    {
        string folderPath = Application.streamingAssetsPath + "/blueprints/";
        string fileName = $"{blueprint.blueprintInformation.label}.json";

        // check is the folder already exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string blueprintFile = JsonConvert.SerializeObject(blueprint, Formatting.Indented);

        File.WriteAllText(folderPath + fileName, blueprintFile);

    }


    public void resetCreateFileView()
    {
        labelInputField.GetComponent<TMP_InputField>().text = null;
        descriptionInputField.GetComponent<TMP_InputField>().text = null;
    }
}
