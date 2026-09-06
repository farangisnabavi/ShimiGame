using System.Collections.Generic;
using UnityEngine;

public class CategoryLevelCompletion : MonoBehaviour
{

    // Getting categorizng finished
    [SerializeField] private List<CategoryCompletionChecker> completionCheckers = new List<CategoryCompletionChecker>();

    //Getting levelCompletionController
    [SerializeField] private LevelCompletionController levelCompletionController;

    //flag
    private bool levelCompleted = false;

    //checking if the level is finished
    private bool IsLevelFinished()
    {

        foreach (CategoryCompletionChecker checker in completionCheckers)
        {

            if (!checker.IsCompleted)
            {
                return false;
            }

        }

        return true;

    }

    //ckecking level is finished 
    private void Update()
    {
        if (levelCompleted)
        {
            return;
        }

        if (IsLevelFinished())
        {
            levelCompleted = true;
            levelCompletionController.CompleteLevel();
        }

    }

}
