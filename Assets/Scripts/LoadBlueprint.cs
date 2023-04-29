// include necessary libaries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;


namespace JsonParser
{

    public class LoadBlueprint : MonoBehaviour
    {

        private MainMenuHandler mainmenu;
        private CreateBlueprint blueprintCreater;

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

        [SerializeField] public GameObject labelInputField;
        [SerializeField] public GameObject descriptionInputField;


        private string label;
        private string description;
        private int icon_index;

        public GameObject IconSlot;



        private void Start()
        {
            mainmenu = GetComponent<MainMenuHandler>();
            blueprintCreater = GetComponent<CreateBlueprint>();
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
            string new_filePath = Path.Combine(Application.dataPath, $"blueprints/{new_label}.json");
            File.Move(filePath, new_filePath);

            if (File.Exists($"{filePath}.meta"))
            {
                string new_metaFilePath = Path.Combine(Application.dataPath, $"blueprints/{new_label}.json.meta");
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

                    childObject.transform.Find("Label").GetComponent<TMPro.TextMeshProUGUI>().text = blueprint.blueprintInformation.label;
                    childObject.transform.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().text = blueprint.blueprintInformation.description;


                    // check if any icon is accessible
                    if (blueprint.blueprintInformation.icons.Count > 0)
                    {
                        Sprite iconPlaceholderSprite = Resources.Load<Sprite>($"InventoryIcons/{blueprint.blueprintInformation.icons[0].signal.name}");
                        childObject.transform.Find("IconPlaceholder").transform.GetChild(0).GetComponent<Image>().color = Color.white;
                        childObject.transform.Find("IconPlaceholder").transform.GetChild(0).GetComponent<Image>().sprite = iconPlaceholderSprite;
                    } 
                    if (blueprint.blueprintInformation.icons.Count == 0)
                    {
                        Sprite iconPlaceholderSprite = Resources.Load<Sprite>("Icons/No-selected-icon");
                        childObject.transform.Find("IconPlaceholder").transform.GetChild(0).GetComponent<Image>().color = Color.white;
                        childObject.transform.Find("IconPlaceholder").transform.GetChild(0).GetComponent<Image>().sprite = iconPlaceholderSprite;
                    }


                    // delete button
                    Button deleteButton = childObject.transform.Find("DeleteBlueprint").GetComponent<Button>();
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
                    Button copyToClipboard = childObject.transform.Find("Copystring").GetComponent<Button>();
                    copyToClipboard.onClick.RemoveAllListeners();
                    copyToClipboard.onClick.AddListener(() =>
                    {
                        mainmenu.blurImage.SetActive(true);
                        mainmenu.copystringPopup.SetActive(true);

                        copyToClipboardButton.onClick.RemoveAllListeners();
                        copyToClipboardButton.onClick.AddListener(() =>
                        {
                            Debug.Log(file.FullName);       
                            mainmenu.blurImage.SetActive(false);
                            mainmenu.copystringPopup.SetActive(false);
                        });

                    });


                    // edit selected blueprint 
                    Button editBlueprint = childObject.transform.Find("EditBlueprint").GetComponent<Button>();
                    editBlueprint.onClick.RemoveAllListeners();
                    editBlueprint.onClick.AddListener(() =>
                    {

                        // activates blurimage and the load blueprint editor popup
                        mainmenu.blurImage.SetActive(true);
                        mainmenu.LoadBlueprintEditor.SetActive(true);

                        // sets the label and description to the parent of the button
                        label = editBlueprint.transform.parent.Find("Label").GetComponent<TMPro.TextMeshProUGUI>().text;
                        description = editBlueprint.transform.parent.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().text;

                        // sets the input fields to the label and description
                        labelInputField.GetComponent<TMP_InputField>().text = label;
                        descriptionInputField.GetComponent<TMP_InputField>().text = description;

                        // listens for the edit button
                        editBlueprintEditorButton.onClick.RemoveAllListeners();
                        editBlueprintEditorButton.onClick.AddListener(() =>
                        {
                            bool fileExists = false;

                            // iterate over all the files and check if the same name already exists
                            for (int i = 0; i< files.Length; i++)
                            {
                                if (files[i].Extension == ".json")
                                {
                                    if (labelInputField.GetComponent<TMP_InputField>().text == Path.GetFileNameWithoutExtension(files[i].FullName))
                                    {
                                        fileExists = true;
                                        break;
                                    } 
                                }
                            }

                            if (fileExists)
                            {
                                Debug.Log("File already exists");
                            }
                            // if the name doesen't exit, edit the file
                            else
                            {
                                // check for empty input fields
                                if (!string.IsNullOrEmpty(descriptionInputField.GetComponent<TMP_InputField>().text) && !string.IsNullOrEmpty(labelInputField.GetComponent<TMP_InputField>().text))
                                {
                                    editBlueprintFile(blueprint, labelInputField.GetComponent<TMP_InputField>().text, descriptionInputField.GetComponent<TMP_InputField>().text, file.FullName);
                                    mainmenu.blurImage.SetActive(false);
                                    mainmenu.LoadBlueprintEditor.SetActive(false);
                                }
                                else
                                {
                                    Debug.Log("There's empty fields");
                                }
                            }
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
                            string selected_object = t.Find("Label").GetComponent<TMPro.TextMeshProUGUI>().text;
                            Debug.Log(selected_object);
                        }
                    }
                });
            }


        }

    }
}