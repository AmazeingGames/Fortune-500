using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Candidate
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string College { get; private set; }
    public string PreviousJobTitle { get; private set; }
    public string PreviousEmployer { get; private set; }
    public List<string> Skills { get; private set; }

    public int Age { get; private set; }
    public int Patience { get; private set; }
    public int Eyes { get; private set; }
    public int Skin { get; private set; }
    public int Head { get; private set; }
    public int Arms { get; private set; }
    public int Torso { get; private set; }
    public int Mouth { get; private set; }

    

    public Dictionary<string, int> Properties { get; private set; }


    public Candidate(string firstName, string lastName, string college, string previousJobTitle, string previousEmployer, List<string> skills,
        int age, int patience, int skin, int eyes, int mouth, int head, int arms, int torso)
    {
        FirstName = firstName;
        LastName = lastName;
        College = college;
        PreviousJobTitle = previousJobTitle;
        PreviousEmployer = previousEmployer;
        Skills = skills;

        Age = age;
        Patience = patience;
        Skin = skin;
        Eyes = eyes;
        Mouth = mouth;
        Head = head;
        Arms = arms;
        Torso = torso;
        

        Properties = new Dictionary<string, int>()
        {
            ["eyes"] = eyes,
            ["arms"] = arms,
            ["head"] = head,
            ["torso"] = torso,
            ["mouth"] = mouth,
        };
    }

    public override string ToString()
    {
        string outputString = FirstName + " " + LastName + ", ";
        foreach (var item in Properties)
        {
            outputString += item.Key;
            outputString += ": ";
            outputString += item.Value;
            outputString += ", ";
        }
        return outputString;
    }

}
