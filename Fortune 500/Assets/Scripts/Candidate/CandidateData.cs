using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public struct CandidateData
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string College { get; private set; }
    public string PreviousJobTitle { get; private set; }
    public string PreviousEmployer { get; private set; }
    public List<string> Skills { get; private set; }

    public int Age { get; private set; }
    public int Patience { get; private set; }
    public Sprite Eyes { get; private set; }
    public Sprite Skin { get; private set; }
    public Sprite HairFront { get; private set; }
    public Sprite HairBack { get; private set; }
    public Sprite Nose { get; private set; }
    public Sprite Torso { get; private set; }
    public Sprite Mouth { get; private set; }


    public Vector2 HairFrontOffset { get; private set; }

    public CandidateData(string firstName, string lastName, string college, string previousJobTitle, string previousEmployer, List<string> skills, int age, int patience, Sprite skin, Sprite eyes, Sprite mouth, Sprite hairFront, Sprite hairBack, Sprite nose, Sprite torso, Vector2 hairFrontOffset)
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
        HairFront = hairFront;
        HairBack = hairBack;
        Nose = nose;
        Torso = torso;   

        HairFrontOffset = hairFrontOffset;
    }
}
