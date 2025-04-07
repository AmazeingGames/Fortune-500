using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using Unity.VisualScripting;
using static DayManager;

public class RestrictionHandler : MonoBehaviour
{
    const string CommonLetters = "alsd";

    [SerializeField] Sprite ageIcon;
    [SerializeField] Sprite nameIcon;
    [SerializeField] Sprite skillsIcon;
    [SerializeField] Sprite collegeIcon;
    [SerializeField] Sprite jobTitleIcon;
    [SerializeField] Sprite previousEmployerIcon;

    public static List<Sprite> icons;

    public enum RestrictionType { Age, Skills, Name, College, JobTitle, PreviousEmployer }
    public static List<RestrictionData> Restrictions = new();
    private void OnEnable()
        => DayManager.DayStateChangeEventHandler += HandleChangeDayState;

    private void OnDisable()
        => DayManager.DayStateChangeEventHandler -= HandleChangeDayState;

    private void Awake()
    {
        icons = new List<Sprite>() { ageIcon, nameIcon, skillsIcon, collegeIcon, jobTitleIcon, previousEmployerIcon };
    }

    void HandleChangeDayState(object sender, DayStateChangeEventArgs e)
    {
        switch (e.myDayState)
        {
            case DayState.StartDay:
                GenerateRestrictions();
            break;
        }
    }

    enum NumericalRestriction { Above, Below, Not }
    enum NameRestriction { ContainLetter, StartWithLetter, BeName }

    public void GenerateRestrictions()
    {
        Restrictions.Clear();

        // Age Restriction
        int randomAge = Random.Range(CandidateGenerator.Instance.MinAge, CandidateGenerator.Instance.MaxAge + 1);
        int randomRestriction = Random.Range(0, Enum.GetNames(typeof(NumericalRestriction)).Length - 1);
        NumericalRestriction myAgeRestriction = (NumericalRestriction)randomRestriction;

        RestrictionData ageRestrictionData = myAgeRestriction switch
        {
            NumericalRestriction.Above => new RestrictionData($"Age must be above {randomAge}", candidate => candidate.Value.Age > randomAge - 7),
            NumericalRestriction.Below => new RestrictionData($"Age must be below {randomAge}", candidate => candidate.Value.Age < randomAge + 7 ),
            NumericalRestriction.Not   => new RestrictionData($"Age can't be {randomAge}",      candidate => candidate.Value.Age != randomAge),
            _ => throw new ArgumentException("Age restriction not handled by switch expression."),
        };

        ageRestrictionData.Initialize(RestrictionType.Age, ageIcon);
        Restrictions.Add(ageRestrictionData);

        // Name Restriciton
        int nameRestrictionType = Random.Range(0, 3);
        char randomLetter = CommonLetters[Random.Range(0, CommonLetters.Length)];
        int randomNameRestriction = Random.Range(0, Enum.GetNames(typeof(NameRestriction)).Length - 1);
        NameRestriction myNameRestriction = (NameRestriction)randomRestriction;
        RestrictionData nameRestrictionData;

        switch (myNameRestriction)
        {
            case NameRestriction.StartWithLetter:
                nameRestrictionData = new RestrictionData("First name must not start with letter " + Char.ToUpper(randomLetter), candidate => candidate.Value.FirstName.ToLower()[0] != randomLetter);
            break;

            case NameRestriction.ContainLetter:
                nameRestrictionData = new RestrictionData("Last name can't contain letter " + Char.ToUpper(randomLetter), candidate => !candidate.Value.LastName.ToLower().Contains(randomLetter));
            break;

            case NameRestriction.BeName:
                string firstName = CandidateGenerator.ChooseRandomElement(CandidateGenerator.Instance.FirstNamesList);
                string lastName = CandidateGenerator.ChooseRandomElement(CandidateGenerator.Instance.LastNamesList);
                nameRestrictionData = new RestrictionData($"Must not be {firstName} {lastName}. Fuck them", candidate => candidate.Value.FirstName != firstName || candidate.Value.LastName != lastName);
            break;

            default:
                throw new ArgumentException("Name restriction not handled by switch statement.");
        }
        nameRestrictionData.Initialize(RestrictionType.Name, nameIcon);
        Restrictions.Add(nameRestrictionData);

        // Skill Restriction
        bool mustHaveSkill = Random.Range(0, 2) == 0;
        string skill = CandidateGenerator.ChooseRandomElement(CandidateGenerator.Instance.SkillsList);

        RestrictionData skillRestriction = mustHaveSkill
            ? new RestrictionData("Must have this skill: " + skill, candidate => candidate.Value.Skills.Contains(skill))
            : new RestrictionData("Must not have this skill: " + skill, candidate => !candidate.Value.Skills.Contains(skill));

        skillRestriction.Initialize(RestrictionType.Skills, skillsIcon);
        Restrictions.Add(skillRestriction);

        // College Restriction
        string college = CandidateGenerator.ChooseRandomElement(CandidateGenerator.Instance.CollegeList);
        RestrictionData collegeRestriction = new RestrictionData("Must not be a graduate from " + college, candidate => candidate.Value.College != college);
        collegeRestriction.Initialize(RestrictionType.College, collegeIcon);
        Restrictions.Add(collegeRestriction);

        // Job Restriction
        string jobTitle = CandidateGenerator.ChooseRandomElement(CandidateGenerator.Instance.JobTitleList);
        RestrictionData jobTitleRestriction = new RestrictionData("Must not have previous experience as " + jobTitle, candidate => candidate.Value.PreviousJobTitle != jobTitle);
        jobTitleRestriction.Initialize(RestrictionType.JobTitle, jobTitleIcon);
        Restrictions.Add(jobTitleRestriction);

        // Previous Employee Restriction
        string previousEmployer = CandidateGenerator.ChooseRandomElement(CandidateGenerator.Instance.PreviousEmployerList);
        RestrictionData previousEmployerRestriction = new RestrictionData("Must not have worked for " + previousEmployer, candidate => candidate.Value.PreviousEmployer != previousEmployer);
        previousEmployerRestriction.Initialize(RestrictionType.PreviousEmployer, previousEmployerIcon);
        Restrictions.Add(previousEmployerRestriction);
    }

}

[Serializable]
public class RestrictionData
{
    public readonly Predicate<CandidateData?> restriction;
    public readonly string description;
    public RestrictionHandler.RestrictionType myRestrictionType;
    public Sprite icon;
    bool hasInitialized = false;

    public RestrictionData(string description, Predicate<CandidateData?> restriction)
    {
        this.description = description;
        this.restriction = restriction;
    }

    public void Initialize(RestrictionHandler.RestrictionType myRestrictionType, Sprite icon)
    {
        if (hasInitialized)
            throw new Exception("We have already initialized class");

        hasInitialized = true;
        this.myRestrictionType = myRestrictionType;
        this.icon = icon;
    }
}
