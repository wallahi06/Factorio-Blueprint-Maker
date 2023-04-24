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

        public string selected_object;


        public GameObject deletePopup;
        public GameObject copyToClipboardView;
        public GameObject blurImage;
        public GameObject editBlueprintView;

        // The LoadObjects and parent
        [SerializeField] private GameObject childObjectPrefab;
        [SerializeField] private Transform panelTransform;
        GameObject childObject;

        // List of files in the blueprint directory
        FileInfo[] files;

        public Blueprint blueprint = new Blueprint();


        [SerializeField] private Button testButton;
        [SerializeField] private Button exitDeleteButton;

        [SerializeField] private Button exitCopyToClipboard;
        [SerializeField] private Button copyToClipboardButton;

        [SerializeField] private Button exitBLueprintEditor;
        [SerializeField] private Button editBlueprintEditor;


        private string label;
        private string description;

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

                    
                    Texture2D texture = new Texture2D(2, 2);
                    byte[] imageData = File.ReadAllBytes($"{Application.dataPath}/Resources/InventoryIcons/{blueprint.blueprintInformation.icons[0].signal.name}.png");
                    texture.LoadImage(imageData);
                    
                    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    childObject.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().sprite = newSprite;

                    

                    // delete button
                    Button deleteButton = childObject.transform.GetChild(2).GetComponent<Button>();
                    deleteButton.onClick.RemoveAllListeners();
                    deleteButton.onClick.AddListener(() =>
                    {

                        blurImage.SetActive(true);
                        deletePopup.SetActive(true);

                        testButton.onClick.RemoveAllListeners();
                        testButton.onClick.AddListener(() =>
                        {
                            deleteBlueprintFile(childObject, file.FullName);

                            blurImage.SetActive(false);
                            deletePopup.SetActive(false);
                        });

                        exitDeleteButton.onClick.RemoveAllListeners();
                        exitDeleteButton.onClick.AddListener(() =>
                        {
                            blurImage.SetActive(false);
                            deletePopup.SetActive(false);
                        });

                    });


                    // copy to clipboard button
                    Button copyToClipboard = childObject.transform.GetChild(4).GetComponent<Button>();
                    copyToClipboard.onClick.RemoveAllListeners();
                    copyToClipboard.onClick.AddListener(() =>
                    {
                        blurImage.SetActive(true);
                        copyToClipboardView.SetActive(true);

                        exitCopyToClipboard.onClick.RemoveAllListeners();
                        exitCopyToClipboard.onClick.AddListener(() =>
                        {
                            blurImage.SetActive(false);
                            copyToClipboardView.SetActive(false);
                        });

                        copyToClipboardButton.onClick.RemoveAllListeners();
                        copyToClipboardButton.onClick.AddListener(() =>
                        {
                            Debug.Log(file.FullName);       //  encoded json string in the future
                            blurImage.SetActive(false);
                            copyToClipboardView.SetActive(false);
                        });

                    });


                    // edit selected blueprint 
                    Button editBlueprint = childObject.transform.GetChild(5).GetComponent<Button>();
                    editBlueprint.onClick.RemoveAllListeners();
                    editBlueprint.onClick.AddListener(() =>
                    {
                        blurImage.SetActive(true);
                        editBlueprintView.SetActive(true);

                        exitBLueprintEditor.onClick.RemoveAllListeners();
                        exitBLueprintEditor.onClick.AddListener(() =>
                        {
                            blurImage.SetActive(false);
                            editBlueprintView.SetActive(false);
                        });

                        editBlueprintEditor.onClick.RemoveAllListeners();
                        editBlueprintEditor.onClick.AddListener(() =>
                        {
                            editBlueprintFile(blueprint, label, description, file.FullName);
                            blurImage.SetActive(false);
                            editBlueprintView.SetActive(false);

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
                            selected_object = t.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
                            Debug.Log(selected_object);
                        }
                    }
                });
            }
        }


        void Start()
        {
            blurImage.SetActive(false);
            deletePopup.SetActive(false);
            copyToClipboardView.SetActive(false);
            editBlueprintView.SetActive(false);
        }
    }
}
