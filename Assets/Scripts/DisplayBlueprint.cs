using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JsonParser
{


    public class DisplayBlueprint : MonoBehaviour
    {


        [SerializeField] public TMPro.TextMeshProUGUI test;
        LoadBlueprint fuck = new LoadBlueprint();

        void Start()
        {
            Debug.Log(fuck.selected_object);
        }

    }
}