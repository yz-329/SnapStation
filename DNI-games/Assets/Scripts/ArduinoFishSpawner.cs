using UnityEngine;
using System.IO.Ports; // Note: You may need to change API Compatibility Level to .NET Framework in Player Settings
using System.Collections.Generic;

public class ArduinoFishSpawner : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbserial-59690940491";
    public int baudRate = 9600;
    
    [Header("Fish Pool")]
    public List<GameObject> fishPool; // Drag your 7 fish objects here

    private SerialPort _serialPort;

    void Start()
    {
        _serialPort = new SerialPort(portName, baudRate);
        _serialPort.ReadTimeout = 50;
        _serialPort.Open();

        // Initial state: Hide all fish
        foreach (var fish in fishPool) fish.SetActive(false);
    }

    void Update()
    {
        if (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
        {
            try
            {
                string data = _serialPort.ReadLine(); // Expected: "UID: 17 E6 xx xx"
                if (data.StartsWith("UID:"))
                {
                    ProcessNFCData(data);
                }
            }
            catch (System.Exception) { /* Timeout or parse error */ }
        }
    }

    void ProcessNFCData(string input)
    {
        string[] parts = input.Replace("UID:", "").Trim().Split(' ');
        
        if (parts.Length > 0)
        {
            // Use the first hex byte as a seed for randomness
            int seed = int.Parse(parts[0], System.Globalization.NumberStyles.HexNumber);
            SpawnGroup(seed);
        }
    }

    void SpawnGroup(int seed)
    {
        // 1. Seed the RNG so the same NFC card always shows the same 4 fish
        Random.InitState(seed);

        // 2. Prepare the list of indices based on your current fish pool
        List<int> indices = new List<int>();
        for (int i = 0; i < fishPool.Count; i++) indices.Add(i);

        // 3. Shuffle the indices
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // 4. Hide every fish in the pool first
        foreach (var fish in fishPool) 
        {
            fish.SetActive(false);
        }

        // 5. Activate exactly 4 fish (or the max available if the pool is smaller)
        int targetCount = Mathf.Min(4, fishPool.Count);
        
        for (int i = 0; i < targetCount; i++)
        {
            int fishIndex = indices[i];
            GameObject selectedFish = fishPool[fishIndex];
            
            selectedFish.SetActive(true);
            
            // MVP: Teleport them to a random spot within your camera bounds
            // Adjust these ranges (-7 to 7, etc.) based on your Scene view
            selectedFish.transform.position = new Vector3(
                Random.Range(-6f, 6f), 
                Random.Range(-4f, 4f), 
                0
            );
        }
    }

    void OnApplicationQuit()
    {
        if (_serialPort != null && _serialPort.IsOpen) _serialPort.Close();
    }
}