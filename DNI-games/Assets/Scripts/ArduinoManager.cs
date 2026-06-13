using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class ArduinoManager : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbserial-59690940491";
    public int baudRate = 9600;

    [Header("References")]
    public FishSpawner fishSpawner;
    public FishingController fishingController;
    public HookMovement hookMovement;
    public OverlayFade overlayFade;
    public SeaweedTrigger seaweedTrigger;
    public SceneSwitcher sceneSwitcher;
    public FruitSpawner fruitSpawner;
    public GameObject introScreen;

    private SerialPort serialPort;

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
        string currentScene = SceneManager.GetActiveScene().name;

        // ==========================================
        // KEYBOARD DEBUG OVERRIDES (For Testing)
        // ==========================================
        if (Keyboard.current != null)
        {
            // Universal NFC/UID key (N)
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                string fakeUID = "1A2B3C4D"; 
                Debug.Log("Manual NFC Triggered. Current Scene is: [" + currentScene + "]");

                if (currentScene == "SampleScene")
                {
                    if (introScreen != null) 
                    {
                        SpriteRenderer introSr = introScreen.GetComponent<SpriteRenderer>();
                        if (introSr != null) introSr.enabled = false;
                    }

                    if (fishSpawner != null) fishSpawner.ProcessNFCData(fakeUID);
                    else Debug.LogError("FISH SPAWNER IS MISSING in Inspector!");
                }
                else if (currentScene == "Cooking_Scene")
                {
                    if (fruitSpawner != null) fruitSpawner.ActivateFruit(fakeUID);
                    else Debug.LogError("FRUIT SPAWNER IS MISSING in Inspector!");
                }
            }

            // Scene-Specific Keyboard Emulators
            if (currentScene == "SampleScene")
            {
                if (Keyboard.current.aKey.wasPressedThisFrame) ProcessSerialLine("ACCEL_Y: 10.0", currentScene);
                else if (Keyboard.current.jKey.wasPressedThisFrame) ProcessSerialLine("JOY_Y: 4095", currentScene);
                else if (Keyboard.current.fKey.wasPressedThisFrame) ProcessSerialLine("FORCE: 100", currentScene);
                else if (Keyboard.current.bKey.wasPressedThisFrame) ProcessSerialLine("BUTTON: yes", currentScene);
                else if (Keyboard.current.xKey.wasPressedThisFrame) ProcessSerialLine("FLEX: 500", currentScene);
            }
        }

        // ==========================================
        // HARDWARE SERIAL PORT READING
        // ==========================================
        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            // Process batches to clean buffers instantly without frame stuttering
            int maxLinesPerFrame = 30; 
            int linesRead = 0;

            while (serialPort.BytesToRead > 0 && linesRead < maxLinesPerFrame)
            {
                string data = serialPort.ReadLine().Trim();
                linesRead++;

                if (!string.IsNullOrEmpty(data))
                {
                    ProcessSerialLine(data, currentScene);
                }
            }
        }
        catch (System.TimeoutException)
        {
            // Normal serial timeout
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Serial read error: " + e.Message);
        }
    }

    // ==========================================
    // CENTRAL LOGIC PARSER
    // ==========================================
    void ProcessSerialLine(string data, string currentScene)
    {
        if (currentScene == "SampleScene")
        {
            if (data.StartsWith("UID:"))
            {
                string uidStr = data.Replace("UID:", "").Trim();
                if (fishSpawner != null) 
                {
                    fishSpawner.ProcessNFCData(uidStr);
                    Debug.Log("Real NFC Scanned: " + uidStr);
                }
            }
            else if (data.StartsWith("FORCE:"))
            {
                if (fishSpawner != null) fishSpawner.ProcessForceData(data);
            }
            else if (data.StartsWith("ACCEL_Y:"))
            {
                string valueStr = data.Replace("ACCEL_Y:", "").Trim();
                if (fishingController != null && float.TryParse(valueStr, out float accelY))
                {
                    fishingController.ProcessAccelY(accelY);
                }
            }
            else if (data.StartsWith("JOY_Y:"))
            {
                string valueStr = data.Replace("JOY_Y:", "").Trim();
                if (hookMovement != null && int.TryParse(valueStr, out int joyValue))
                {
                    hookMovement.ProcessJoystick(joyValue);
                }
            }
            else if (data.StartsWith("FLEX:"))
            {
                string valueStr = data.Replace("FLEX:", "").Trim();
                if (int.TryParse(valueStr, out int flexValue))
                {
                    if (flexValue < 800 || flexValue > 3000)
                    {
                        Debug.Log($"Seaweed Triggered! Flex: {flexValue}");
                        if (seaweedTrigger != null) seaweedTrigger.Wiggle();
                    }
                }
            }
            else if (data.StartsWith("BUTTON:"))
            {
                string valueStr = data.Replace("BUTTON:", "").Trim();
                if (valueStr == "yes" && overlayFade != null)
                {
                    overlayFade.ToggleOverlay();
                }
            }
            else if (data.StartsWith("SCENE:"))
            {
                string valueStr = data.Replace("SCENE:", "").Trim();
                if (sceneSwitcher != null && int.TryParse(valueStr, out int sceneState))
                {
                    sceneSwitcher.ProcessSceneSwitch(sceneState);
                }
            }
        }
        else if (currentScene == "Cooking_Scene")
        {
            if (data.StartsWith("UID:"))
            {
                string uidStr = data.Replace("UID:", "").Trim();
                if (fruitSpawner != null) 
                {
                    fruitSpawner.ActivateFruit(uidStr);
                    Debug.Log("Real NFC Scanned for Cooking: " + uidStr);
                }
            }
            else if (fruitSpawner != null)
            {
                FoodController activeFood = fruitSpawner.activeFruit;
                if (activeFood == null)
                {
                    activeFood = Object.FindFirstObjectByType<FoodController>();
                }

                if (activeFood != null)
                {
                    activeFood.ProcessInput(data);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}