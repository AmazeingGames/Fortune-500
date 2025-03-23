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
    const string CommonLetters = "alsd";

    /*public Dictionary<string, Predicate<CandidateData>> Restrictions { get; private set; } = new()
    {
        ["Age must be above 24"] = (candidate => candidate.Age > 24),
        ["Age must be below 35"] = (candidate => candidate.Age < 35),
        ["Age can't be 38"] = (candidate => candidate.Age != 38),
        ["First name must start with letter E"] = (candidate => candidate.FirstName[0]=='E'),
        ["Last name can't contain letter S"] = (candidate => !candidate.LastName.ToLower().Contains('s')),
        ["Must be self motivated or Self sufficient"] = 
            (candidate => candidate.Skills.Contains("Self motivated") || candidate.Skills.Contains("Self sufficiency")),
        ["Must not be Bill Smith name. Fuck that guy"] = (candidate => candidate.FirstName != "Bill" || candidate.LastName != "Smith"),
    };*/

    public List<Restriction> Restrictions = new();
    private void Awake()
        => _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();

    private void OnEnable()
        => GameManager.GameActionEventHandler += HandleGameAction;

    private void OnDisable()
        => GameManager.GameActionEventHandler -= HandleGameAction;

    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameManager.GameAction.None:
                break;
            case GameManager.GameAction.EnterMainMenu:
                break;
            case GameManager.GameAction.PlayGame:
                break;
            case GameManager.GameAction.StartDay:
                Restrictions = GenerateRestrictions();
                break;
            case GameManager.GameAction.PauseGame:
                break;
            case GameManager.GameAction.ResumeGame:
                break;
            case GameManager.GameAction.RestartDay:
                break;
            case GameManager.GameAction.LoadNextDay:
                break;
            case GameManager.GameAction.CompleteLevel:
                break;
            case GameManager.GameAction.BeatGame:
                break;
        }
    }

    public List<Restriction> GenerateRestrictions()
    {
        List<Restriction> output = new();

        int age = Random.Range(_candidateGenerator.MinAge, _candidateGenerator.MaxAge + 1);
        int ageRestrictionType = Random.Range(0, 3);
        Restriction ageRestriction = ageRestrictionType switch
        {
            0 => new Restriction("Age must be above " + age, candidate => candidate.Age > age),
            1 => new Restriction("Age must be below " + age, candidate => candidate.Age < age),
            2 => new Restriction("Age can't be " + age, candidate => candidate.Age != age),
            _ => null,
        };
        output.Add(ageRestriction);

        int nameRestrictionType = Random.Range(0, 3);
        char randomCommonLetter = CommonLetters[Random.Range(0, CommonLetters.Length)];
        switch (nameRestrictionType)
        {
            case 0:
                output.Add(new Restriction("First name must start with letter " + Char.ToUpper(randomCommonLetter),
                    candidate => candidate.FirstName.ToLower()[0] == randomCommonLetter));
                break;
            case 1:
                output.Add(new Restriction("Last name can't contain letter " + Char.ToUpper(randomCommonLetter),
                    candidate => !candidate.LastName.ToLower().Contains(randomCommonLetter)));
                break;
            case 2:
                string firstName = CandidateGenerator.ChooseRandomElement(_candidateGenerator.FirstNamesList);
                string lastName = CandidateGenerator.ChooseRandomElement(_candidateGenerator.LastNamesList);
                output.Add(new Restriction("Must not be " + firstName + " " + lastName + ". Fuck them",
                    candidate => candidate.FirstName != firstName || candidate.LastName != lastName));
            break;
        }

        int skillRestrictionType = Random.Range(0, 2);
        string skill = CandidateGenerator.ChooseRandomElement(_candidateGenerator.SkillsList);
        Restriction skillRestriction = skillRestrictionType switch
        {
            0 => new Restriction("Must have this skill: " + skill, candidate => candidate.Skills.Contains(skill)),
            1 => new Restriction("Must not have this skill: " + skill, candidate => !candidate.Skills.Contains(skill)),
            _ => null,
        };
        output.Add(skillRestriction);
        return output;
    }

    public Restriction GetRandomRestriction()
        => Restrictions[Random.Range(0, Restrictions.Count)];
  
}

public class Restriction
{
    public readonly Predicate<CandidateData> restriction;
    public readonly string description;

    public Restriction(string description, Predicate<CandidateData> restriction)
    {
        this.description = description;
        this.restriction = restriction;
    }
}
