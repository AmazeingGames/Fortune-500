using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using Unity.VisualScripting;

public class RestrictionHandler : MonoBehaviour
{
    CandidateGenerator _candidateGenerator;


    Dictionary<string, Func<Candidate, bool>> restrictions = new Dictionary<string, Func<Candidate, bool>>()
    {
        ["Age must be above 24"] = (candidate => candidate.Age > 24),
        ["Age must be below 35"] = (candidate => candidate.Age < 35),
        ["Age can't be 38"] = (candidate => candidate.Age != 38),
        ["First name must start with letter E"] = (candidate => candidate.FirstName[0]=='E'),
        ["Last name can't contain letter S"] = (candidate => !candidate.LastName.ToLower().Contains('s')),
        ["Must be self motivated or Self sufficient"] = 
            (candidate => candidate.Skills.Contains("Self motivated") || candidate.Skills.Contains("Self sufficiency")),
        ["Must not be Bill Smith name. Fuck that guy"] = (candidate => candidate.FirstName != "Bill" || candidate.LastName != "Smith"),

    };

    public List<Tuple<string, Func<Candidate, bool>>> GenerateRestrictions()
    {
        List<Tuple<string, Func<Candidate, bool>>> output = new List<Tuple<string, Func<Candidate, bool>>>();

        int age = Random.Range(_candidateGenerator.MinAge, _candidateGenerator.MaxAge + 1);
        int ageRestrictionType = Random.Range(0, 3);
        switch (ageRestrictionType)
        {
            case 0:
                output.Add(new Tuple<string, Func<Candidate, bool>>("Age must be above " + age, candidate => candidate.Age > age));
                break;
            case 1:
                output.Add(new Tuple<string, Func<Candidate, bool>>("Age must be below " + age, candidate => candidate.Age < age));
                break;
            case 2:
                output.Add(new Tuple<string, Func<Candidate, bool>>("Age can't be " + age, candidate => candidate.Age != age));
                break;
        }

        int nameRestrictionType = Random.Range(0, 3);
        string commonLetters = "alsd";
        char letter = commonLetters[Random.Range(0, commonLetters.Length)];
        switch (nameRestrictionType)
        {
            case 0:
                output.Add(new Tuple<string, Func<Candidate, bool>>("First name must start with letter " + Char.ToUpper(letter),
                    candidate => candidate.FirstName.ToLower()[0] == letter));
                break;
            case 1:
                output.Add(new Tuple<string, Func<Candidate, bool>>("Last name can't contain letter " + Char.ToUpper(letter),
                    candidate => !candidate.LastName.ToLower().Contains(letter)));
                break;
            case 2:
                string firstName = _candidateGenerator.ChooseRandomElement(_candidateGenerator.FirstNamesList);
                string lastName = _candidateGenerator.ChooseRandomElement(_candidateGenerator.LastNamesList);
                output.Add(new Tuple<string, Func<Candidate, bool>>("Must not be " + firstName + " " + lastName + ". Fuck them",
                    candidate => candidate.FirstName != firstName || candidate.LastName != lastName));
                break;
        }

        int skillRestrictionType = Random.Range(0, 2);
        string skill = _candidateGenerator.ChooseRandomElement(_candidateGenerator.SkillsList);
        switch (skillRestrictionType)
        {
            case 0:
                output.Add(new Tuple<string, Func<Candidate, bool>>("Must have this skill: " + skill,
                    candidate => candidate.Skills.Contains(skill)));
                break;
            case 1:
                output.Add(new Tuple<string, Func<Candidate, bool>>("Must not have this skill: " + skill,
                    candidate => !candidate.Skills.Contains(skill)));
                break;
        }
        return output;
    }

    private void Awake()
    {
        _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();
    }


    public Tuple<string, Func<Candidate, bool>> GetRandomRestriction()
    {
        
        int i = Random.Range(0, restrictions.Count);
        return new Tuple<string, Func<Candidate, bool>>(restrictions.ElementAt(i).Key, restrictions.ElementAt(i).Value);

    }
}
