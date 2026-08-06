using UnityEngine;
using System.Collections.Generic;

public class NameMatchLevelCompletion : MonoBehaviour
{

    //Taking items from inspector
    [SerializeField] private List<NameMatchTarget> targets = new List<NameMatchTarget>();

    //conecting to level completion checker
    [SerializeField] private LevelCompletionController levelCompletionController;

    //flag 
    private bool levelCompleted = false;

    private bool AreAllTargetsCompleted()
    {
        //checking all of items to be right
        foreach (NameMatchTarget target in targets)
        {
            Debug.Log(
        target.gameObject.name +
        " | LastMatchWasCorrect = " +
        target.LastMatchWasCorrect
    );

            if (target.LastMatchWasCorrect != true)
            {
                return false;
            }
        }

        return true;
    }


    private void Update()
    {
        if (levelCompleted)
        {
            return;
        }

        if (AreAllTargetsCompleted())
        {
            levelCompleted = true;
            levelCompletionController.CompleteLevel();
        }
    }


}
