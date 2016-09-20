using System;
using UnityEngine;

namespace Assets.Scripts
{
    /**
     * This class will look for the presens of the most important elements of this game
     * It will throw errors if any of those are missing during Start()
     */
    [RequireComponent(typeof(GameLogic))]
    class SanityChecker : MonoBehaviour
    {
        void Start()
        {
            bool success = true;

            try
            {
                if (!Constants.gameLogic)
                    throw new NullReferenceException("GameLogic reference missing!");
                if (!Constants.StatusUI)
                    throw new NullReferenceException("StatusUI reference missing!");
                if (!Constants.InteractionUI)
                    throw new NullReferenceException("InteractionUI reference missing!");
                if (!Constants.HUDCanvas)
                    throw new NullReferenceException("HUDCanvas reference missing!");
                if (Constants.gameTimeMultiplier <= 0)
                    throw new ArgumentException("gameTimeMultiplier MUST NOT be 0 or negative!!");

                if (!GameObject.FindGameObjectWithTag("MainCamera"))
                    throw new NullReferenceException("MainCamera missing in scene!");
                if (!GameObject.FindGameObjectWithTag("MainCharacter"))
                    throw new NullReferenceException("MainCharacter missing in scene!");
                if (GameObject.FindGameObjectsWithTag("MainCharacter").Length > 1)
                    throw new System.Exception("More than 1 objects with tag 'MainCharacter' found!");
            }
            catch (System.Exception e)
            {
                success = false;
                Debug.LogError("SanityChecker found an error!" + e);
                throw; // throw further up. This is just for the logging stuff
            }

            if (success)
                Debug.Log("SanitCheck is okay.");
        }
    }
}
