using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IncubationManager : MonoBehaviour
{
    //The list of eggs currently being incubated
    public static List<EggIncubationSaveState> eggIncubating = new List<EggIncubationSaveState>();

    //How long it takes indays for an egg to hatch
    public const int daysToIncubate = 3;

    public List<Incubator> incubators;

    public static UnityEvent onEggUpdate = new UnityEvent();

    //Call the functions required when the player is in the scene
    private void OnEnable()
    {
        //Assign eachIncubator an id
        RegisterIncubators();
        //Load up the incubator information
        LoadIncubatorData();
        //Load the data on every update
        onEggUpdate.AddListener(LoadIncubatorData);
    }

    private void OnDestroy()
    {
        onEggUpdate.RemoveListener(LoadIncubatorData);
    }

    //Function to call on clock update
    public static void UpdateEggs()
    {
        //Eggs must be incubating
        if (eggIncubating.Count == 0) return;

        foreach(EggIncubationSaveState egg in eggIncubating.ToList())
        {
            //Update the egg
            egg.Tick();
            onEggUpdate?.Invoke();
            if(egg.timeToIncubate <= 0)
            {
                eggIncubating.Remove(egg);

                //Handle Chicken spawning here
                Debug.Log("A new chick is born!");
            }
        }
    }

    //Assign an ID to each incubator
    void RegisterIncubators()
    {
        for(int i = 0; i < incubators.Count; i++)
        {
            incubators[i].incubationID = i;
        }
    }

    void LoadIncubatorData()
    {
        if (eggIncubating.Count == 0) return;

        foreach(EggIncubationSaveState egg in eggIncubating)
        {
            //Get the incubator to load
            Incubator incubatorToLoad = incubators[egg.incubatorID];

            bool isIncubating = true;
            //Check if the egg is hatching/has hatched
            if(egg.timeToIncubate <= 0)
            {
                isIncubating = false;
            }

            incubatorToLoad.SetIncubationState(isIncubating, egg.timeToIncubate);

        }
    }
}
