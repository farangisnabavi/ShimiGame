using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class IsFinished : MonoBehaviour
{
    // This script doesn't have anything to do with UI and HUD
    //It
    //only checks wether the level finished or not
    
    //Reciving every Item thata should be cmpleted
    [SerializeField]
    private List<MoleculeCompletionChecker> moleculeCheckers =
        new List<MoleculeCompletionChecker>();

    //Choosing the HUD controller
    [SerializeField] private LevelCompletionController levelCompletionController;

    //Flag
    private bool levelCompleted = false;

    //Checking if level is finished
    private bool IsItFinished(){

        if (moleculeCheckers == null || moleculeCheckers.Count == 0)
        {
            return false;
        }

        foreach (MoleculeCompletionChecker checker in moleculeCheckers)
        {
            if (!checker.IsCompleted)
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
        
        if (IsItFinished())
        {
            levelCompleted = true;

            //Declaring that level ended
            levelCompletionController.CompleteLevel();

        }
    }

}
