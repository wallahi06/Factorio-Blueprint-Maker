// include necessary libaries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;


namespace JsonParser
{

    public class LoadBlueprint : MonoBehaviour
    {
        
        private MainMenuHandler mainmenu;

        // The LoadObjects and parent
        [SerializeField] private GameObject childObjectPrefab;
        [SerializeField] private Transform panelTransform;
        [SerializeField] private Transform IconObjectContainer;

        GameObject childObject;

        // List of files in the blueprint directory
        FileInfo[] files;

        public Blueprint blueprint = new Blueprint();


        [SerializeField] private Button deleteBlueprintButton;
        [SerializeField] private Button editBlueprintEditorButton;
        [SerializeField] private Button copyToClipboardButton;


        private string label;
        private string description;
        private int icon_index;

        private GameObject IconSlot;



        private void Start()
        {
            mainmenu = GetComponent<MainMenuHandler>();
        }

        public void setDescription(string get_description)
        {
            description = get_description;
        }

        public void setLabel(string get_label)
        {
            label = get_label;
        }

        // changes the blueprint information of the editBlueprint in the blueprint file (filePath) to the new_label and new_description
        public void editBlueprintFile(Blueprint editBlueprint, string new_label, string new_description, string filePath)
        {
            // changes the editBlueprint information to the new values
            editBlueprint.blueprintInformation.label = new_label;
            editBlueprint.blueprintInformation.description = new_description;

            // writes the new blueprint information to the file
            string blueprint_result = JsonConvert.SerializeObject(editBlueprint, Formatting.Indented);
            File.WriteAllText(filePath, blueprint_result);

            // renames the blueprint file and .meta file to the label
            string new_filePath = Path.Combine(Application.dataPath, $"blueprints/{label}.json");
            File.Move(filePath, new_filePath);

            if (File.Exists($"{filePath}.meta"))
            {
                string new_metaFilePath = Path.Combine(Application.dataPath, $"blueprints/{label}.json.meta");
                File.Move($"{filePath}.meta", new_metaFilePath);
            }


            // update the loader
            get_blueprints();
        }
         

        // deleted the loadObject and the file in the FilePath
        public void deleteBlueprintFile(GameObject loadObject, string FilePath)
        {
            Destroy(loadObject);

            File.Delete(FilePath);
            File.Delete($"{FilePath}.meta");

            get_blueprints();

        }



        // Creates loading obejcts
        public void get_blueprints()
        {
            childObjectPrefab.SetActive(false);

            // Stores all files in the blueprint directory
            string path = Path.Combine(Application.dataPath, "blueprints");
            DirectoryInfo dir = new DirectoryInfo(path);
            files = dir.GetFiles();


            // Deletes prevoius child objects
            foreach (Transform child in panelTransform)
            {
                Destroy(child.gameObject);
            }

            // add new objects
            foreach (FileInfo file in files)
            {
                if (file.Extension == ".json")
                {

                    // read from json file
                    string jsonContent = File.ReadAllText(file.FullName);
                    blueprint = JsonConvert.DeserializeObject<Blueprint>(jsonContent);


                    // add all necessary components of the loading object
                    childObject = Instantiate(childObjectPrefab, panelTransform);
                    childObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = blueprint.blueprintInformation.label;
                    childObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = blueprint.blueprintInformation.description;


                    // check is any icon is accessible
                    if (blueprint.blueprintInformation.icons.Count > 0)
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        byte[] imageData = File.ReadAllBytes($"{Application.dataPath}/Resources/InventoryIcons/{blueprint.blueprintInformation.icons[0].signal.name}.png");
                        texture.LoadImage(imageData);

                        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                        childObject.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().color = Color.white;
                        childObject.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().sprite = newSprite;

                    } else
                    {
                        Debug.Log("No icon was selected");
                    }

                    // delete button
                    Button deleteButton = childObject.transform.GetChild(2).GetComponent<Button>();
                    deleteButton.onClick.RemoveAllListeners();
                    deleteButton.onClick.AddListener(() =>
                    {

                        mainmenu.blurImage.SetActive(true);
                        mainmenu.DeleteBlueprintPopup.SetActive(true);

                        deleteBlueprintButton.onClick.RemoveAllListeners();
                        deleteBlueprintButton.onClick.AddListener(() =>
                        {
                            deleteBlueprintFile(childObject, file.FullName);

                            mainmenu.blurImage.SetActive(false);
                            mainmenu.DeleteBlueprintPopup.SetActive(false);
                        });

                    });


                    // copy to clipboard button
                    Button copyToClipboard = childObject.transform.GetChild(4).GetComponent<Button>();
                    copyToClipboard.onClick.RemoveAllListeners();
                    copyToClipboard.onClick.AddListener(() =>
                    {
                        mainmenu.blurImage.SetActive(true);
                        mainmenu.copystringPopup.SetActive(true);

                        copyToClipboardButton.onClick.RemoveAllListeners();
                        copyToClipboardButton.onClick.AddListener(() =>
                        {
                            Debug.Log(file.FullName);       //  encoded json string in the future
                            mainmenu.blurImage.SetActive(false);
                            mainmenu.copystringPopup.SetActive(false);
                        });

                    });


                    // edit selected blueprint 
                    Button editBlueprint = childObject.transform.GetChild(5).GetComponent<Button>();
                    editBlueprint.onClick.RemoveAllListeners();
                    editBlueprint.onClick.AddListener(() =>
                    {
                        mainmenu.blurImage.SetActive(true);
                        mainmenu.LoadBlueprintEditor.SetActive(true);

                        editBlueprintEditorButton.onClick.RemoveAllListeners();
                        editBlueprintEditorButton.onClick.AddListener(() =>
                        {
                            editBlueprintFile(blueprint, label, description, file.FullName);
                            mainmenu.blurImage.SetActive(false);
                            mainmenu.LoadBlueprintEditor.SetActive(false);

                        });
                    });

                    childObject.SetActive(true);

                }
            }

            foreach (Transform child in panelTransform)
            {
                Button selectLoading = child.GetComponent<Button>();
                selectLoading.onClick.RemoveAllListeners();
                selectLoading.onClick.AddListener(() =>
                {
                    foreach (Transform t in panelTransform)
                    {
                        if (t != child)
                        {
                            t.GetComponent<Image>().color = new Color(0.736f, 0.736f, 0.736f);
                        }
                        else
                        {
                            t.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
                            //selected_object = t.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
                            Debug.Log(t.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text);
                        }
                    }
                });
            }


        }



    }
}
