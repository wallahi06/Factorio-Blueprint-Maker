// include libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using TMPro;


namespace JsonParser
{


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


    public class CreateBlueprint : MonoBehaviour    {

        private string description;
        private string label;
        private int icon_index;

        [SerializeField] public GameObject labelInputField;
        [SerializeField] public GameObject descriptionInputField;
        [SerializeField] private GameObject IconObjectPrefab;
        [SerializeField] private Transform IconObjectContainer;

        GameObject childObject;
        public GameObject IconSlots;


        private MainMenuHandler mainmenu;

        private Blueprint blueprint;
        private Signal signal;

        private BlueprintInformation blueprintInformation = new BlueprintInformation()
        {
            entities = new List<Entity>(),
            icons = new List<Icon>()
        };


        void Start()
        {
            IconObjectPrefab.SetActive(false);
            mainmenu = GetComponent<MainMenuHandler>();

            initiateIconInventory();
         }


        // create blueprint object
        public void createBlueprintObject()
        {
            // sets all blueprint information parameters
            blueprint = new Blueprint()
            {
                blueprintInformation = new BlueprintInformation()
                {
                    label = label,
                    description = description,
                    entities = blueprintInformation.entities,
                    icons = blueprintInformation.icons
                }
            };

            writeToFile();
        }


        // adds the entity to the blueprint JSON file
        public void createEntity()
        {
            Entity entity = new Entity()
            {
                entity_number = 1,
                name = "transport-belt",
                position = new Position()
                {
                    x = 1,
                    y = 1
                },
                direction = 1
            };

            blueprintInformation.entities.Add(entity);
            writeToFile();
        }


        // creates Icon and import icon parameter 
        public void addIcon(string icon_name, int icon_index)
        {

            Icon icon = new Icon()
            {
                signal = new Signal()
                {
                    type = "item",
                    name = icon_name
                },
                index = icon_index
            };

            blueprintInformation.icons.Add(icon);
        }


        // function that creates the JSON file
        public void createBlueprintJSON()
        {

            // sets the label and description to the value of the input fields
            label = labelInputField.GetComponent<TMP_InputField>().text;
            description = descriptionInputField.GetComponent<TMP_InputField>().text;

            // list all blueprint files in the blueprints folder
            string path = Path.Combine(Application.dataPath, "blueprints/");
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();


            // check so the input fields are not empty or null
            if (!string.IsNullOrEmpty(descriptionInputField.GetComponent<TMP_InputField>().text) || !string.IsNullOrEmpty(labelInputField.GetComponent<TMP_InputField>().text)) {

                // check if there are any files, if not just create one
                if (files.Length > 0)   {

                    foreach (FileInfo file in files)    {

                        // check if the blueprint file that is created already exists, if not create it
                        if (Path.GetFileNameWithoutExtension(file.FullName) != label)   {
                            createBlueprintObject();

                        } else  {
                            Debug.Log(label + " already exists");
                        }
                    }
                } else  {
                    createBlueprintObject();
                }
            } else  {
                Debug.Log("There are empty field!");
            }

            resetCreateFileView();
        }

    
        // resets all the input fields and icon slots when file is created
        public void resetCreateFileView() 
        {

            // reset all the input fields
            labelInputField.GetComponent<TMP_InputField>().text = "";
            descriptionInputField.GetComponent<TMP_InputField>().text = "";

            // reset the blueprintInformation parameters
            blueprintInformation.label = null;
            blueprintInformation.description = null;
            blueprintInformation.icons = new List<Icon>();
            blueprintInformation.entities = new List<Entity>();
            
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
        

        // writes blueprint to the JSON file
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


        public void icon_selection_1()  {
            icon_index = 1;
            setIconView();
        }

        public void icon_selection_2()  {
            icon_index = 2;
            setIconView();
        }

        public void icon_selection_3()  {
            icon_index = 3;
            setIconView();
        }

        public void icon_selection_4()  {
            icon_index = 4;
            setIconView();
        }


        void setIconView()
        {

            mainmenu.IconSelectionView.SetActive(true);
        
            foreach (Transform child in IconObjectContainer)
            {
                Button test1 = child.transform.GetComponent<Button>();
                test1.onClick.RemoveAllListeners();
                test1.onClick.AddListener(() =>
                {

                    if (blueprintInformation.icons.Count > 0)
                    {
                        bool found = false;
                        for (int i = 0; i < blueprintInformation.icons.Count; i++)
                        {
                            if (icon_index == blueprintInformation.icons[i].index)
                            {
                                blueprintInformation.icons[i].signal.name = child.name;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            addIcon(child.name, icon_index);
                        }
                    }
                    else
                    {
                        addIcon(child.name, icon_index);
                    }

                    Texture2D texture = new Texture2D(2, 2);
                    byte[] imageData = File.ReadAllBytes($"{Application.dataPath}/Resources/InventoryIcons/{child.name}.png");
                    texture.LoadImage(imageData);

                    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    IconSlots.transform.GetChild(icon_index - 1).transform.GetChild(0).GetComponent<Image>().sprite = newSprite;
                    IconSlots.transform.GetChild(icon_index - 1).transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    IconSlots.transform.GetChild(icon_index - 1).name = child.name;

                    Texture2D texture2 = new Texture2D(2, 2);
                    byte[] imageData2 = File.ReadAllBytes($"{Application.dataPath}/Resources/InventoryIcons/{IconSlots.transform.GetChild(0).name}.png");
                    texture2.LoadImage(imageData2);

                    Sprite newSprite2 = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), Vector2.one * 0.5f);

                    IconSlots.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    IconSlots.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = newSprite2;

                    mainmenu.IconSelectionView.SetActive(false);

                });
            }
        }


        // gets all the files from the icon dictory and adds an object for each file
        void initiateIconInventory()
        {
            // stores all the files in "files"
            string path = Path.Combine(Application.dataPath, "Resources/InventoryIcons/");
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            // iterates over all the files
            foreach (FileInfo file in files)
            {
                if (file.Extension == ".png")
                {
                    childObject = Instantiate(IconObjectPrefab, IconObjectContainer);
                    childObject.SetActive(true);
                    childObject.name = Path.GetFileNameWithoutExtension(file.FullName);

                    Texture2D texture = new Texture2D(2, 2);
                    byte[] imageData = File.ReadAllBytes(file.FullName);
                    texture.LoadImage(imageData);

                    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    Image imageComponent = childObject.transform.GetChild(0).GetComponent<Image>();

                    imageComponent.sprite = newSprite;

                }
            }
        }

    }

}
