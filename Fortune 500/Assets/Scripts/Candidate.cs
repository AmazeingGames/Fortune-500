using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Candidate
{
    public int MutationsNumber { get; private set; }
    public int Eyes { get; private set; }
    public int Head { get; private set; }
    public int Arms { get; private set; }
    public int Torso { get; private set; }
    public int Mouth { get; private set; }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public Dictionary<string, int> Properties { get; private set; }


    public Candidate(int mutationsNumber ,int eyes, int mouth, int arms, int head, int torso)
    {
        MutationsNumber = mutationsNumber;
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

        FirstName = "Gabriel";
        LastName = "Scoccola";
    }

    public override string ToString()
    {
        string outputString = FirstName + " " + LastName + ", mutations Number: " + MutationsNumber + ", ";
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
