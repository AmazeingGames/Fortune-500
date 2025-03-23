using UnityEngine;

public class Candidate : MonoBehaviour
{
    [SerializeField] SpriteRenderer eyes;
    [SerializeField] SpriteRenderer nose;
    [SerializeField] SpriteRenderer mouth;
    [SerializeField] SpriteRenderer hairFront;
    [SerializeField] SpriteRenderer hairBack;
    [SerializeField] SpriteRenderer skin;

    public CandidateData CandidateData { get; private set; }

    public void Init(CandidateData candidateData)
    {
        CandidateData = candidateData;

        eyes.sprite = candidateData.Eyes;
        nose.sprite = candidateData.Nose;
        mouth.sprite = candidateData.Mouth;
        hairFront.sprite = candidateData.HairFront;
        hairBack.sprite = candidateData.Eyes;
        skin.sprite = candidateData.Eyes;
    }
}
