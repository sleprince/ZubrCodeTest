using UnityEngine;
using System.Reflection; //added, needed for reflection
using System.Collections.Generic; //needed for lists
using System.Linq; //needed for LINQ sorting methods

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

    //list to hold all the names
    public List<Person> allNamesList = new List<Person>();

    //struct to separate fist and last names, but also keep them grouped together as part of a person object
    public struct Person
    {
        public string FirstName;
        public string LastName;

        //constructor for each new person object, to split a full name and return first and last name
        public Person(string fullName)
        {
            //split the full name based on spaces
            var parts = fullName.Split(' ');

            //assign the first part to FirstName
            FirstName = parts[0];

            //assign second part to LastName
            LastName = parts[1];
        }

        //override ToString to display person as "FirstName LastName" when printed, otherwise it would just say "Sorting+Person"
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }


    private void Awake()
    {
        CollectNamesUsingReflection();
    }

    /// <summary>
    /// This method uses reflection to collect all string fields into a list. 
    /// The choice to use reflection, instead of directly creating a list from the strings 
    /// was made as an exercise to learn about reflection and out of curiosity of the possibility 
    /// of automating the process without manually entering the names again.
    /// </summary>
    
    private void CollectNamesUsingReflection()
    {
        //get all the fields (variables) in the class, BindingFlags.Instance important for it to work
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        //loop through each field in the class
        foreach (FieldInfo field in fields)
        {
            //check if the field is a string
            if (field.FieldType == typeof(string))
            {
                //if it is a string, take the value from this field
                string fullName = (string)field.GetValue(this);

                //add it to the list as a new person object
                allNamesList.Add(new Person(fullName));
            }
        }
    }

    private void Update()
    {

        //arrow key checks for the 4 sorting operations
        if (Input.GetKeyDown(KeyCode.DownArrow))
            LastNameDescending();
        if (Input.GetKeyDown(KeyCode.UpArrow))
            LastNameAscending();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            FirstNameDescending();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            FirstNameAscending();

    }

    //methods to sort names in various orders
    void LastNameDescending()
    {
        //sorts the allNamesList based on each person's LastName. 
        //uses a lambda expression (p => p.LastName) to select the LastName property by which to sort.
        //orders in descending order and converts the output to a list.
        var sortedList = allNamesList.OrderByDescending(p => p.LastName).ToList();

        //sends the sorted list to the PrintSortedResults method indicating the used sorting method.
        PrintSortedResults("LastNameDescending", sortedList);
    }

    void FirstNameDescending()
    {
        //OrderByDescending needed for descending order
        var sortedList = allNamesList.OrderByDescending(p => p.FirstName).ToList();
        PrintSortedResults("FirstNameDescending", sortedList);
    }

    void LastNameAscending()
    {
        //OrderBy is ascending order
        var sortedList = allNamesList.OrderBy(p => p.LastName).ToList();
        PrintSortedResults("LastNameAscending", sortedList);
    }

    void FirstNameAscending()
    {
        var sortedList = allNamesList.OrderBy(p => p.FirstName).ToList();
        PrintSortedResults("FirstNameAscending", sortedList);
    }

    //method to print sorted results to console
    private void PrintSortedResults(string functionName, List<Person> sortedResults)
    {
        //so we know which function was called
        Debug.Log(functionName + ":");
        foreach (Person person in sortedResults)
        {
            //normally ToString() is called automatically on objects, here our overridden version is used for the person
            Debug.Log(person);
        }
    }

}
