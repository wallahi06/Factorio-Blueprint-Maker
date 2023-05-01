using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IconSelection : MonoBehaviour
{

    public GameObject scrollContainer;
    public GameObject IconSelectionView;
    public GameObject prefab;

    public int numOfOfObject = 4;
    private float spacing;

    void Start()
    {
        IconSelectionActive();
    }


    void IconSelectionActive()
    {
        IconSelectionView.SetActive(true);
        InitiateIconSelection();

        float width = scrollContainer.GetComponent<RectTransform>().rect.width;

        spacing = width * 0.06f;
        Vector2 newGridSize = new Vector2((width - ((numOfOfObject+1)*spacing))/numOfOfObject, (width - ((numOfOfObject+1) * spacing)) / numOfOfObject);
        Vector2 spacingTest = new Vector2(spacing, spacing);
        scrollContainer.GetComponent<GridLayoutGroup>().cellSize = newGridSize;
        scrollContainer.GetComponent<GridLayoutGroup>().spacing = spacingTest;

        scrollContainer.GetComponent<GridLayoutGroup>().padding = new RectOffset((int)spacing, (int)spacing, (int)spacing, (int)spacing);


    }


    void InitiateIconSelection()
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
                Debug.Log(iconButton.name);
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
