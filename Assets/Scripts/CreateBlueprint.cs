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



        // ADD  CHECKS IF FIELDS ARE EMPTY, MAYBE A POPUP



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
            writeToFile();

        }




    }

}