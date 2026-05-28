using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FruitSpawner : MonoBehaviour
{
    public List<FoodController> fruits;
    
    // Track the active fruit so the ArduinoManager can send inputs to it
    public FoodController activeFruit; 

    public void ActivateFruit(string uid)
    {
        Debug.Log("UID received: " + uid);

        string cleanUid = uid.Replace(" ", "").Trim();
        int seed = int.Parse(cleanUid, System.Globalization.NumberStyles.HexNumber);
        Random.InitState(seed);

        int index = Random.Range(0, fruits.Count);

        // Turn everything off
        foreach (var fruit in fruits)
            fruit.gameObject.SetActive(false);

        // Assign the new active fruit and TURN IT ON
        activeFruit = fruits[index];
        activeFruit.gameObject.SetActive(true); 

        // IMPORTANT: wait 1 frame before resetting
        StartCoroutine(InitFruit(activeFruit, index, uid));
    }

    IEnumerator InitFruit(FoodController fruit, int index, string uid)
    {
        yield return null; // wait Unity activation

        fruit.ResetState();
        
        // Pass the UID string down to trigger the visual setup in the basket
        fruit.ProcessInput("UID:" + uid);

        Debug.Log("Activated fruit index: " + index);
    }
}