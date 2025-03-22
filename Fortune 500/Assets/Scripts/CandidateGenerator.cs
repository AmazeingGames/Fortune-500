using System.Collections.Generic;
using UnityEngine;

public class CandidateGenerator : MonoBehaviour
{
    int _range = 4;

    void Start()
    {
        Candidate candidate = GenerateRandomCandidate(2);
        Debug.Log(candidate);
    }

    public Candidate GenerateRandomCandidate(int totalMutations)
    {
        Candidate candidate = new Candidate(0, 0, 0, 0 ,0);
        int mutationsToAssign = totalMutations;
        List<string> keyList = new List<string>(candidate.Properties.Keys);

        while (keyList.Count > 0 && mutationsToAssign >0)
        {
            int randInt= Random.Range(0, keyList.Count);
            mutationsToAssign--;
            MakePropertyMutation(candidate, keyList[randInt]);
            keyList.RemoveAt(randInt);
        }

        return candidate;
    }
    
    private void MakePropertyMutation(Candidate candidate, string property)
    {
        candidate.Properties[property] = Random.Range(1, _range);
    }
    
}
