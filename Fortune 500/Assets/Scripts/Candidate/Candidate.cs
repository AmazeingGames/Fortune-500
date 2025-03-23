using UnityEngine;

public class Candidate : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] SpriteRenderer eyes;
    [SerializeField] SpriteRenderer nose;
    [SerializeField] SpriteRenderer mouth;
    [SerializeField] SpriteRenderer hairFront;
    [SerializeField] SpriteRenderer hairBack;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] SpriteRenderer torso;

    public CandidateData CandidateData { get; private set; }

    public void Init(CandidateData candidateData)
    {
        CandidateData = candidateData;

        eyes.sprite = candidateData.Eyes;
        nose.sprite = candidateData.Nose;
        mouth.sprite = candidateData.Mouth;
        hairFront.sprite = candidateData.HairFront;
        float hairFrontZ = hairFront.transform.localPosition.z;
        hairFront.transform.localPosition = (Vector2.zero + candidateData.HairFrontOffset);
        hairFront.transform.localPosition = new Vector3(hairFront.transform.localPosition.x, hairFront.transform.localPosition.y, hairFrontZ);
        hairBack.sprite = candidateData.HairBack;
        skin.sprite = candidateData.Skin;
        torso.sprite = candidateData.Torso;
    }
}
