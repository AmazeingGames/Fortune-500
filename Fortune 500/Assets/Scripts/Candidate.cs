using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Candidate
{
    
    public int Eyes { get; private set; }
    public int Head { get; private set; }
    public int Arms { get; private set; }
    public int Torso { get; private set; }
    public int Mouth { get; private set; }

    public Dictionary<string, int> Properties { get; private set; }


    public Candidate(int eyes, int mouth, int arms, int head, int torso)
    {
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
        string outputString = "";
        foreach (var item in Properties)
        {
            outputString += item.Key;
            outputString += " ";
            outputString += item.Value;
            outputString += " ";
        }
        return outputString;
    }

}
