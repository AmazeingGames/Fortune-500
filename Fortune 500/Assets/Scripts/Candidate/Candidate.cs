using JetBrains.Annotations;
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

    public CandidateData? CandidateData { get; private set; }

    public void Init(CandidateData? candidateData)
    {
        CandidateData = candidateData;

        eyes.sprite = candidateData.Value.Eyes;
        nose.sprite = candidateData.Value.Nose;
        mouth.sprite = candidateData.Value.Mouth;
        hairFront.sprite = candidateData.Value.HairFront;
        float hairFrontZ = hairFront.transform.localPosition.z;
        hairFront.transform.localPosition = (Vector2.zero + candidateData.Value.HairFrontOffset);
        hairFront.transform.localPosition = new Vector3(hairFront.transform.localPosition.x, hairFront.transform.localPosition.y, hairFrontZ);
        hairBack.sprite = candidateData.Value.HairBack;
        skin.sprite = candidateData.Value.Skin;
        torso.sprite = candidateData.Value.Torso;
    }
}
