using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResumeDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _randomPINfield;
    [SerializeField] Sprite[] _resumeSprites;
    [SerializeField] Image _resumeImage;

    [SerializeField] TextMeshProUGUI _nameField;
    [SerializeField] TextMeshProUGUI _ageField;
    [SerializeField] TextMeshProUGUI _universityField;
    [SerializeField] TextMeshProUGUI _workExperienceField;
    [SerializeField] TextMeshProUGUI _skillsField;

    public void DisplayCandidate(CandidateData candidate)
    {
        int randomPIN = Random.Range(10000, 99999);
        _randomPINfield.text = randomPIN.ToString();
        _nameField.text = candidate.FirstName + " " + candidate.LastName;
        _ageField.text = candidate.Age.ToString();
        _universityField.text = candidate.College;
        _workExperienceField.text = candidate.PreviousJobTitle + " at " + candidate.PreviousEmployer;

        
        int paperType = Random.Range(0, 11);
        {
            if (paperType == 10) _resumeImage.sprite = _resumeSprites[2];
            else if (paperType == 9) _resumeImage.sprite = _resumeSprites[1];
            else _resumeImage.sprite = _resumeSprites[0];
        }

        string skillsField = "";
        for (int i = 0; i < candidate.Skills.Count; i++)
        {
            skillsField += candidate.Skills[i];
            if (i != candidate.Skills.Count-1) skillsField += ", ";
            else skillsField += ".";
        }
        _skillsField.text = skillsField;
    }
}
