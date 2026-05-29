using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    // 1. THIS MAKES IT A SINGLETON THAT SURVIVES SCENE CHANGES
    public static SceneSwitcher instance;

    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbmodem1201";
    public int baudRate = 9600;

    private SerialPort serialPort;
    private int currentSceneState = -1;
    
    // 2. COOLDOWN TOGGLE
    private bool isSwitching = false;

    void Awake()
    {
        // Enforce the Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy this when changing scenes!
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates in new scenes
            return;
        }
    }

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 50;

        try
        {
            serialPort.Open();
            Debug.Log("Serial Port Opened");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            string data = serialPort.ReadLine().Trim();

            // =========================
            // SCENE SWITCHING
            // =========================
            if (data.StartsWith("SCENE:"))
            {
                string valueStr = data.Replace("SCENE:", "").Trim();

                if (int.TryParse(valueStr, out int sceneState))
                {
                    ProcessSceneSwitch(sceneState);
                }
            }
        }
        catch (System.TimeoutException)
        {
            // Normal timeout
        }
        catch (System.Exception e)
        {
            // Debug.LogWarning(e.Message);
        }
    }

    public void ProcessSceneSwitch(int sceneState)
    {
        // If we are currently in the middle of a scene transition, ignore Arduino spam
        if (isSwitching) return;

        // If the SAME cartridge is pressed again (spring mechanism pops it out)
        if (sceneState == currentSceneState)
        {
            if (currentSceneState == 0) return;

            Debug.Log("Cartridge popped out! Returning to Intro.");
            StartCoroutine(LoadSceneWithCooldown(0, "IntroScreen"));
            return;
        }

        // Otherwise, load the new scene normally
        switch (sceneState)
        {
            case 0:
                StartCoroutine(LoadSceneWithCooldown(0, "IntroScreen"));
                break;
            case 1:
                StartCoroutine(LoadSceneWithCooldown(1, "Cooking_Scene"));
                break;
            case 2:
                StartCoroutine(LoadSceneWithCooldown(2, "SampleScene"));
                break;
        }
    }

    // 3. THIS COROUTINE HANDLES THE DELAY
    IEnumerator LoadSceneWithCooldown(int newState, string sceneName)
    {
        isSwitching = true; // Lock out new inputs
        currentSceneState = newState;

        Debug.Log("Switching Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);

        // Wait 2 seconds before accepting new scene switch commands
        // (Increase this number if your Arduino is super spammy)
        yield return new WaitForSeconds(2.0f); 

        isSwitching = false; // Unlock inputs
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}