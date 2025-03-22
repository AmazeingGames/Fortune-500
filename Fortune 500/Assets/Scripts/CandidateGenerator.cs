using System.Collections.Generic;
using UnityEngine;

public class CandidateGenerator : MonoBehaviour
{
    int _mutationsRange = 4;


    public Candidate GenerateRandomCandidate(int totalMutations)
    {
        Candidate candidate = new Candidate(totalMutations, 0, 0, 0, 0 ,0);
        int mutationsToAssign = totalMutations;
        List<string> keyList = new List<string>(candidate.Properties.Keys);

        while (keyList.Count > 0 && mutationsToAssign >0)
        {
            int randInt= Random.Range(0, keyList.Count);
            mutationsToAssign--;
            MakePropertyMutation(candidate, keyList[randInt]);
            keyList.RemoveAt(randInt);
        }
        Debug.Log(candidate);
        return candidate;
    }
    
    private void MakePropertyMutation(Candidate candidate, string property)
    {
        candidate.Properties[property] = Random.Range(1, _mutationsRange);
    }
    
}
