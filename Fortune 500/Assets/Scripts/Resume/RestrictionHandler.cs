using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using Unity.VisualScripting;

public class RestrictionHandler : Singleton<RestrictionHandler>
{
    CandidateGenerator _candidateGenerator;
    const string CommonLetters = "alsd";

    public enum RestrictionType { Age, Skills, Name, College, JobTitle, PreviousEmployer }
    public List<Restriction> Restrictions = new();
    private void Awake()
    {
        base.Awake();
        _candidateGenerator = FindAnyObjectByType<CandidateGenerator>();
    }
    private void OnEnable()
        => GameManager.GameActionEventHandler += HandleGameAction;

    private void OnDisable()
        => GameManager.GameActionEventHandler -= HandleGameAction;

    void HandleGameAction(object sender, GameActionEventArgs e)
    {
        switch (e.gameAction)
        {
            case GameManager.GameAction.StartWork:
                GenerateRestrictions();
            break;
        }
    }

    public void GenerateRestrictions()
    {
        Debug.Log("Generate Restrictions");
        List<Restriction> output = new();

        int age = Random.Range(_candidateGenerator.MinAge, _candidateGenerator.MaxAge + 1);
        int ageRestrictionType = Random.Range(0, 3);
        Restriction ageRestriction = ageRestrictionType switch
        {
            0 => new Restriction("Age must be above " + age, candidate => candidate.Age > age - 7),
            1 => new Restriction("Age must be below " + age, candidate => candidate.Age < age + 7 ),
            2 => new Restriction("Age can't be " + age, candidate => candidate.Age != age),
            _ => null,
        };

        ageRestriction.Init(RestrictionType.Age);
        output.Add(ageRestriction);

        int nameRestrictionType = Random.Range(0, 3);
        char randomCommonLetter = CommonLetters[Random.Range(0, CommonLetters.Length)];
        Restriction nameRestriction;
        switch (nameRestrictionType)
        {
            case 0:
                nameRestriction = new Restriction("First name must not start with letter " + Char.ToUpper(randomCommonLetter), candidate => candidate.FirstName.ToLower()[0] != randomCommonLetter);
                break;
            case 1:
                nameRestriction = new Restriction("Last name can't contain letter " + Char.ToUpper(randomCommonLetter), candidate => !candidate.LastName.ToLower().Contains(randomCommonLetter));
                break;
            case 2:
                string firstName = CandidateGenerator.ChooseRandomElement(_candidateGenerator.FirstNamesList);
                string lastName = CandidateGenerator.ChooseRandomElement(_candidateGenerator.LastNamesList);
                nameRestriction = new Restriction($"Must not be {firstName} {lastName}. Fuck them", candidate => candidate.FirstName != firstName || candidate.LastName != lastName);
            break;
            default:
                nameRestriction = null;
            break;
        }
        nameRestriction.Init(RestrictionType.Name);
        output.Add(nameRestriction);

        int skillRestrictionType = Random.Range(0, 2);
        string skill = CandidateGenerator.ChooseRandomElement(_candidateGenerator.SkillsList);
        Restriction skillRestriction = skillRestrictionType switch
        {
            0 => new Restriction("Must have this skill: " + skill, candidate => candidate.Skills.Contains(skill)),
            1 => new Restriction("Must not have this skill: " + skill, candidate => !candidate.Skills.Contains(skill)),
            _ => null,
        };
        skillRestriction.Init(RestrictionType.Skills);
        output.Add(skillRestriction);

        
        string college = CandidateGenerator.ChooseRandomElement(_candidateGenerator.CollegeList);
        Restriction collegeRestriction = new Restriction("Must not be a graduate from " + college, candidate => candidate.College != college);
        collegeRestriction.Init(RestrictionType.College);
        output.Add(collegeRestriction);

        string jobTitle = CandidateGenerator.ChooseRandomElement(_candidateGenerator.JobTitleList);
        Restriction jobTitleRestriction = new Restriction("Must not have previous experience as " + jobTitle, candidate => candidate.PreviousJobTitle != jobTitle);
        jobTitleRestriction.Init(RestrictionType.JobTitle);
        output.Add(jobTitleRestriction);

        string previousEmployer = CandidateGenerator.ChooseRandomElement(_candidateGenerator.PreviousEmployerList);
        Restriction previousEmployerRestriction = new Restriction("Must not have worked for " + previousEmployer, candidate => candidate.PreviousEmployer != previousEmployer);
        previousEmployerRestriction.Init(RestrictionType.PreviousEmployer);
        output.Add(previousEmployerRestriction);

        Restrictions = output;
    }

    public Restriction GetRandomRestriction()
        => Restrictions[Random.Range(0, Restrictions.Count)];

}

[Serializable]
public class Restriction
{
    public readonly Predicate<CandidateData> restriction;
    public readonly string description;
    public RestrictionHandler.RestrictionType myRestrictionType;
    bool hasInitialized = false;

    public Restriction(string description, Predicate<CandidateData> restriction)
    {
        this.description = description;
        this.restriction = restriction;
    }

    public void Init(RestrictionHandler.RestrictionType myRestrictionType)
    {
        if (hasInitialized)
            throw new Exception("We have already initialized class");

        hasInitialized = true;
        this.myRestrictionType = myRestrictionType;
    }
}
