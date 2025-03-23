

using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using Random = UnityEngine.Random;


public class CandidateGenerator : MonoBehaviour
{
    public string[] FirstNamesList { get; private set; }
    public string[] LastNamesList { get; private set; }
    public string[] JobTitleList { get; private set; }
    public string[] PreviousEmployerList { get; private set; }
    public string[] CollegeList { get; private set; }
    public List<string> SkillsList { get; private set; }

    public int MinAge { get; private set; } = 20;
    public int MaxAge { get; private set; } = 45;

    [SerializeField] int _minPatience = 40;
    [SerializeField] int _maxPatience = 45;

    [SerializeField] GameObject[] _skinList;
    [SerializeField] GameObject[] _eyesList;
    [SerializeField] GameObject[] _mouthList;
    [SerializeField] GameObject[] _headList;
    [SerializeField] GameObject[] _armsList;
    [SerializeField] GameObject[] _torsoList;

    private void Start()
    {
        FirstNamesList = new string[] {"Gabriel", "Lynn", "Liz", "Gabriel", "John", "James", "Jonathan", "Jordan", "Amelia",
            "Ava", "Nova", "James", "Luke", "Noah", "Olivia", "Charlotte", "Liam", "Oliver", "Sophia", "Sofia", "Daniel", "Connor", "Conner",
            "Logan", "Adeline", "Ethan", "Mateo", "Emma", "Amelia", "William", "Bill", "Theo", "Henry", "Lia", "Dalia", "Daniel", "Ezra",
            "Carter", "Jack", "Samuel", "Alex", "Harper","Kai", "Cason", "Sebastian",
            "Michael", "Leo", "Abigail", "Caroline", "Evelyn", "Elliot", "Emily", "Sam", "Maru", "Sebastian", "Leah", "Linus", "Haley", "Morris", "Harvey",
            "Penny", "Shane", "Gus", "Demetrius", "Marnie", "Pam", "Robin", "Vincent" };
        
        LastNamesList = new string[] {"Smith","Baker","Stone","Jones","Williams","Taylor","Brown","Davies","Evans","Thomas","Wilson","Johnson","Roberts",
            "White","Green","Jackson","Carter","Gates","Ross","Mills","Willis","Grant","Cooke","Lane","Hamilton","Moss","McDonald","Potter",
            "Rowe","Carr","Stone","Foster","Barber","Stevenson","Collins","Shaw","Adams","Bird","Reid","Reed","Oliver","Newton","Porter",
            "Holland","Harding","Frost","Slater","Goodwin","Gray","Hunt","Own","Webb","Danner" };

        JobTitleList = new string[] {"Senior operator","Officer of staff","Accountant","CAO","CEO","CAT","Technical Assistant","Sales Coordinator",
            "Chief Synergy Officer","Stakeholder Assesment Analyst","Optimization Facilitator","Logistics Liaison","Resource Orchestrator","Customer Enabler",
            "Antics Strategist","Corporate Engineer" };

        PreviousEmployerList = new string[] {"MacroHard","Floormart","Rainforest","Gaagly","Pear Inc","McBurgers","PriceCo",
            "Discsword", "Unwell Fargot", "Bank of Antartica", "Charlie's Swab Bank", "Nesty", "Specific Mills", "Kelly Logs" };

        CollegeList = new string[] {"Pricetown", "Hardvard", "Stan's Ford", "Half Sail", "Light Brown", "Johns Hopkids", "Fluke", "Dartboard",
            "Another Dame", "Vanderspilt", "Gorgetown", "BLT", "Sale", "Northyeastern", "Cornhole", "Carnivorous Melon" };

        SkillsList = new List<string>() { "Prone to error", "Customer service", "Sales", "Communication", "Active listening", "Atenton to detail",
            "Leadership", "Public Speaking", "Self sufficiency", "Integrity", "Self motivated" }; 
    }


    public Candidate GenerateRandomCandidate()
    {
        string firstName = ChooseRandomElement(FirstNamesList);
        string lastName = ChooseRandomElement(LastNamesList);

        string college = ChooseRandomElement(CollegeList);
        string previousJobTitle = ChooseRandomElement(JobTitleList);
        string previousEmployer = ChooseRandomElement(PreviousEmployerList);

        int skillsAmount = Random.Range(2, 5);
        List<string> skillsToChooseFrom = new List<string>();
        foreach (var skill in SkillsList)
        {
            skillsToChooseFrom.Add(skill);
        }

        List<string> skills = new List<string>();

        for (int i = 0; i<skillsAmount; i++)
        {
            int j = Random.Range(0, skillsToChooseFrom.Count);
            skills.Add(skillsToChooseFrom[j]);
            skillsToChooseFrom.RemoveAt(j);
        }
        

        int age = Random.Range(MinAge, MaxAge+1);
        int patience = Random.Range(_minPatience, _maxPatience + 1);

        int skin = Random.Range(0, _skinList.Length);
        int eyes = Random.Range(0, _eyesList.Length);
        int mouth = Random.Range(0, _mouthList.Length);
        int head = Random.Range(0, _headList.Length);
        int arms = Random.Range(0, _armsList.Length);
        int torso = Random.Range(0, _torsoList.Length);
        

        Candidate candidate = new Candidate(firstName, lastName, college, previousJobTitle, previousEmployer, skills,
        age, patience, skin, eyes, mouth, head, arms, torso);

        return candidate;
    }

    public T ChooseRandomElement<T>(T[] array)
    {
        int index = Random.Range(0, array.Length);
        return array[index];
    }

    public T ChooseRandomElement<T>(List<T> array)
    {
        int index = Random.Range(0, array.Count);
        return array[index];
    }

}
