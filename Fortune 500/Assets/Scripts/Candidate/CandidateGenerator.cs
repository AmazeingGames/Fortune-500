
using System;
using System.Collections.Generic;

using UnityEngine;

using Random = UnityEngine.Random;

public class CandidateGenerator : MonoBehaviour
{
    [SerializeField] TextAsset firstNamesFile;
    [SerializeField] TextAsset lastNamesFile;
    [SerializeField] TextAsset jobTitleFile;
    [SerializeField] TextAsset previousEmployerFile;
    [SerializeField] TextAsset collegeFile;
    [SerializeField] TextAsset skillsFile;

    public List<string> FirstNamesList { get; private set; }
    public List<string> LastNamesList { get; private set; }
    public List<string> JobTitleList { get; private set; }
    public List<string> PreviousEmployerList { get; private set; }
    public List<string> CollegeList { get; private set; }
    public List<string> SkillsList { get; private set; }

    public int MinAge { get; private set; } = 20;
    public int MaxAge { get; private set; } = 45;

    [SerializeField] int _minPatience = 40;
    [SerializeField] int _maxPatience = 45;

    [SerializeField] Sprite[] _skinList;
    [SerializeField] Sprite[] _eyesList;
    [SerializeField] Sprite[] _mouthList;
    [SerializeField] Sprite[] _hairFrontList;
    [SerializeField] Sprite[] _hairBackList;
    [SerializeField] Sprite[] _noseList;
    [SerializeField] Sprite[] _torsoList;

    [Header("Offset")]
    [SerializeField] Vector2[] _hairFrontOffset;

    private void OnValidate()
    {
        FirstNamesList = FillList(firstNamesFile);
        LastNamesList = FillList(lastNamesFile);
        JobTitleList = FillList(jobTitleFile);
        PreviousEmployerList = FillList(previousEmployerFile);
        CollegeList = FillList(collegeFile);
        SkillsList = FillList(skillsFile);
    }

    List<string> FillList(TextAsset textAsset)
        => textAsset ? new(textAsset.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)) : null;

    public CandidateData GenerateRandomCandidate()
    {
        string firstName = ChooseRandomElement(FirstNamesList);
        string lastName = ChooseRandomElement(LastNamesList);

        string college = ChooseRandomElement(CollegeList);
        string previousJobTitle = ChooseRandomElement(JobTitleList);
        string previousEmployer = ChooseRandomElement(PreviousEmployerList);

        int skillsAmount = Random.Range(2, 5);
        List<string> skillsToChooseFrom = new List<string>();

        List<string> skills = new List<string>();
        skillsToChooseFrom.AddRange(SkillsList);

        for (int i = 0; i < skillsAmount; i++)
        {
            int j = Random.Range(0, skillsToChooseFrom.Count);
            skills.Add(skillsToChooseFrom[j]);
            skillsToChooseFrom.RemoveAt(j);
        }

        int age = Random.Range(MinAge, MaxAge + 1);
        int patience = Random.Range(_minPatience, _maxPatience + 1);

        Sprite skin = _skinList[Random.Range(0, _skinList.Length)];
        Sprite eyes = _eyesList[Random.Range(0, _eyesList.Length)];
        Sprite mouth = _mouthList[Random.Range(0, _mouthList.Length)];
        int randomHair = Random.Range(0, _hairFrontList.Length);
        Sprite hairFront = _hairFrontList[randomHair];
        Vector2 hairFrontOffset = _hairFrontOffset[randomHair];
        Sprite hairBack = _hairBackList[Random.Range(0, _hairBackList.Length)];
        Sprite nose = _noseList[Random.Range(0, _noseList.Length)];
        Sprite torso = _torsoList[Random.Range(0, _torsoList.Length)];

        return new CandidateData(firstName, lastName, college, previousJobTitle, previousEmployer, skills, age, patience, skin, eyes, mouth, hairFront, hairBack, nose, torso, hairFrontOffset);  
    }

    public static T ChooseRandomElement<T>(List<T> list)
            => list[Random.Range(0, list.Count)];
}