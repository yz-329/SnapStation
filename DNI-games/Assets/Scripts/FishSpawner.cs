using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Pool")]
    public List<GameObject> fishPool;

    void Start()
    {
        foreach (var fish in fishPool)
        {
            fish.SetActive(false);
        }
    }

    // =========================
    // FORCE SENSOR
    // =========================
    public void ProcessForceData(string input)
    {
        string valueStr = input.Replace("FORCE:", "").Trim();

        if (int.TryParse(valueStr, out int forceValue))
        {
            Debug.Log("Force Value: " + forceValue);

            if (forceValue > 60)
            {
                DisturbFish();
            }
        }
    }

    void DisturbFish()
    {
        foreach (var fish in fishPool)
        {
            if (fish.activeSelf)
            {
                RandomFishMovement2 moveScript =
                    fish.GetComponent<RandomFishMovement2>();

                if (moveScript != null)
                {
                    moveScript.ApplyBoost(1.0f);
                }
            }
        }
    }

    // =========================
    // NFC
    // =========================
    public void ProcessNFCData(string input)
    {
        string[] parts =
            input.Replace("UID:", "").Trim().Split(' ');

        if (parts.Length > 0)
        {
            int seed = int.Parse(
                parts[0],
                System.Globalization.NumberStyles.HexNumber
            );

            SpawnGroup(seed);
        }
    }

    void SpawnGroup(int seed)
    {
        Random.InitState(seed);

        List<int> indices = new List<int>();

        for (int i = 0; i < fishPool.Count; i++)
        {
            indices.Add(i);
        }

        // Shuffle
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];

            int randomIndex =
                Random.Range(i, indices.Count);

            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // Hide all fish
        foreach (var fish in fishPool)
        {
            fish.SetActive(false);
        }

        // Activate 4 fish
        int targetCount = Mathf.Min(4, fishPool.Count);

        for (int i = 0; i < targetCount; i++)
        {
            int fishIndex = indices[i];

            GameObject selectedFish = fishPool[fishIndex];

            selectedFish.SetActive(true);

            selectedFish.transform.position =
                new Vector3(
                    Random.Range(-6f, 6f),
                    Random.Range(-4f, 4f),
                    0
                );
        }
    }
}