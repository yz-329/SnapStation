using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FruitSpawner : MonoBehaviour
{
    public List<FoodController> fruits;
    public FoodController activeFruit; 
    
    // NEW: Track where we are in the list
    private int currentIndex = 0; 

    public void ActivateFruit(string uid)
    {
        Debug.Log("UID received: " + uid);
        string cleanUid = uid.Replace(" ", "").Trim();
        int seed = int.Parse(cleanUid, System.Globalization.NumberStyles.HexNumber);
        Random.InitState(seed);

        // Set the initial index based on the scan
        currentIndex = Random.Range(0, fruits.Count);

        foreach (var fruit in fruits)
            fruit.gameObject.SetActive(false);

        activeFruit = fruits[currentIndex];
        activeFruit.gameObject.SetActive(true); 

        StartCoroutine(InitFruit(activeFruit, currentIndex, uid));
    }

    // NEW: Call this to go to the next fruit without scanning
    public void SpawnNextFruit()
    {
        // Turn off the old fruit
        if (activeFruit != null)
            activeFruit.gameObject.SetActive(false);

        // Move to the next index. The '%' symbol makes it loop back to 0 if we hit the end of the list!
        currentIndex = (currentIndex + 1) % fruits.Count;
        
        activeFruit = fruits[currentIndex];
        activeFruit.gameObject.SetActive(true); 

        // Pass a fake UID so ProcessInput triggers the basket drop logic
        StartCoroutine(InitFruit(activeFruit, currentIndex, "AUTO_LOOP"));
    }

    IEnumerator InitFruit(FoodController fruit, int index, string uid)
    {
        yield return null; 
        fruit.ResetState();
        fruit.ProcessInput("UID:" + uid);
        Debug.Log("Activated fruit index: " + index);
    }
}