using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;


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
public class Signal { 
    public string type { get; set; }
    public string name { get; set; }
}


// Icon class, import the signal and sets an index
public class Icon  {
    public Signal signal { get; set; }
    public int index { get; set; }
}


// Blueprint class that stores all the information
public class BlueprintInformation {
    public string label { get; set; }
    public string description { get; set; }
    public List<Entity> entities { get; set; }
    public List<Icon> icons { get; set; }
}


    // Finished blueprint class
public class Blueprint {
        public BlueprintInformation blueprintInformation { get; set; }  
    }


public class CreateBlueprint : MonoBehaviour    {

        private string description;
        private string label;

        private int entity_number = 1;
        private string name = "test";
        private int x = 1;
        private int y = 1;
        private int direction = 1;

        private Blueprint blueprint;
        private Signal signal;

        private BlueprintInformation blueprintInformation = new BlueprintInformation()
        {
            entities = new List<Entity>(),
            icons = new List<Icon>()
        };


        // gets the description and label from input fields
        public void setDescription(string get_description)
        {
            description = get_description;
        }


        public void setLabel(string get_label)
        {
            label = get_label;
        }

        // function that creates the JSON file
        public void createBlueprintJSON()
        {

            string path = Path.Combine(Application.dataPath, "blueprints/");
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                if (Path.GetFileNameWithoutExtension(file.FullName) != label)
                {
                    if (label != null && description != null)
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

                    } else
                    {
                        Debug.Log("make sure that no field is blank");
                    }


                } else
                {
                    Debug.Log(label + " already exists");
                }
            } 
        }


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


            string blueprint_result = JsonConvert.SerializeObject(blueprint, Formatting.Indented);
            File.WriteAllText(filePath, blueprint_result);
        }


        // adds the entity to the blueprint JSON file
        public void createEntity()
        {
            Entity entity = new Entity()
            {
                entity_number = entity_number,
                name = name,
                position = new Position()
                {
                    x = x,
                    y = y
                },
                direction = direction
            };

            blueprintInformation.entities.Add(entity);
            writeToFile();

        }


        // on call, it adds a icon 
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


        public int icon_index = 1;

        [SerializeField] private GameObject childObjectPrefab;
        [SerializeField] private Transform panelTransform;
        GameObject childObject;

        public GameObject iconSelectionView;
        [SerializeField] private Button exitIconSelectionButton;


        public void icon_selection_1()
        {
            iconSelectionView.SetActive(true);
            icon_index = 1;
            setIconView();
        }

        public void icon_selection_2()
        {
            iconSelectionView.SetActive(true);
            icon_index = 2;
            setIconView();
        }

        public void icon_selection_3()
        {
            iconSelectionView.SetActive(true);
            icon_index = 3;
            setIconView();
        }

        public void icon_selection_4()
        {
            iconSelectionView.SetActive(true);
            icon_index = 4;
            setIconView();
        }

        void setIconView()
        {

            if (blueprintInformation.icons.Count > 0)
            {
                for (int i = 0; i < blueprintInformation.icons.Count; i++)
                {
                    if (icon_index == blueprintInformation.icons[i].index)
                    {
                        Debug.Log("Adding the new icon because icon index: " + i + " is already imported");
                        // load the new icon to the icon index
                    }
                }
            } else
            {
                foreach (Transform child in panelTransform)
                {
                    Button test = child.transform.GetComponent<Button>();
                    test.onClick.RemoveAllListeners();
                    test.onClick.AddListener(() =>
                    {
                        addIcon(child.name, icon_index);
                        iconSelectionView.SetActive(false);
                        Debug.Log("Adding " + child.name + " to the icon index: " + icon_index);

                    });
                }
            }

            Button exitButton = exitIconSelectionButton.transform.GetComponent<Button>();
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() =>
            {
                iconSelectionView.SetActive(false);
            });

        }


        void Start() {

            iconSelectionView.SetActive(false);
            childObjectPrefab.SetActive(false);

            string path = Path.Combine(Application.dataPath, "Resources/InventoryIcons/");
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                if (file.Extension == ".png")
                {
                    childObject = Instantiate(childObjectPrefab, panelTransform);
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