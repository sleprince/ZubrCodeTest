using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class Sorting : MonoBehaviour
{
    public string maddison = "Maddison Albert";
    public string hester = "Hester Blankenship";
    public string saara = "Saara Browne";
    public string libbie = "Libbie Case";
    public string dominick = "Dominick Howells";
    public string abida = "Abida Hayden";
    public string giorgia = "Giorgia Cotton";
    public string rita = "Rita Smyth";
    public string rania = "Rania Wolf";
    public string zainab = "Zainab Sheldon";

    // list to hold all the names
    public List<string> allNamesList = new List<string>();


    private void Awake()
    {
        CollectNamesUsingReflection();
    }

    private void CollectNamesUsingReflection()
    {
        //get all the fields (variables) in the class
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        //loop through each field in the class
        foreach (FieldInfo field in fields)
        {
            //check if the field is a string
            if (field.FieldType == typeof(string))
            {
                //if it is a string, take the value from this field and add it to the list
                allNamesList.Add((string)field.GetValue(this));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            LastNameDescending();


    }

    //method to split a full name and return the last name
    private string GetLastName(string fullName)
    {
        string[] parts = fullName.Split(' ');
        return parts[parts.Length - 1];
    }

    void LastNameDescending()
    {

        {
            List<string> lastNameDescending = new List<string>(allNamesList);
            lastNameDescending.Sort((a, b) => string.Compare(GetLastName(b), GetLastName(a)));

            //print the sorted names to console with function name
            PrintSortedResults("LastNameDescending", lastNameDescending);
        }

    }

    void FirstNameDescending()
    {

    }
    void LastNameAscending()
    {

    }
    void FirstNameAscending()
    {

    }

    //method to print sorted results to console
    private void PrintSortedResults(string functionName, List<string> sortedResults)
    {
        Debug.Log(functionName + ":");
        foreach (string name in sortedResults)
        {
            Debug.Log(name);
        }
    }

}
