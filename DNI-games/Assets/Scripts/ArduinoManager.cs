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
        // FAKE UID TRIGGER (Press 'N' key)
        // ==========================================
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            string fakeUID = "1A2B3C4D"; 
            
            // This will now print the exact scene name Unity sees!
            Debug.Log("Manual NFC Triggered. Current Scene is: [" + currentScene + "]");

            if (currentScene == "SampleScene")
            {
                if (introScreen != null) 
                {
                    SpriteRenderer introSr = introScreen.GetComponent<SpriteRenderer>();
                    if (introSr != null) introSr.enabled = false;
                }

                if (fishSpawner != null) 
                {
                    fishSpawner.ProcessNFCData(fakeUID);
                    Debug.Log("Successfully sent Fake UID to Fish Spawner!");
                }
                else 
                {
                    // ALARM 1
                    Debug.LogError("FISH SPAWNER IS MISSING! Please drag it into the ArduinoManager Inspector.");
                }
            }
            else if (currentScene == "Cooking_Scene")
            {
                if (fruitSpawner != null)
                {
                    fruitSpawner.ActivateFruit(fakeUID);
                    Debug.Log("Successfully sent Fake UID to Fruit Spawner!");
                }
                else
                {
                    // ALARM 2
                    Debug.LogError("FRUIT SPAWNER IS MISSING! Please drag it into the ArduinoManager Inspector.");
                }
            }
            else 
            {
                // ALARM 3
                Debug.LogWarning("SCENE NAME MISMATCH: The script doesn't know what to do in a scene named: " + currentScene);
            }
        }

        // ==========================================
        // SERIAL PORT DATA READING
        // ==========================================

        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            string data = serialPort.ReadLine().Trim();

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
                // Removed the UID check entirely. Starts directly with FORCE.
                else if (data.StartsWith("FORCE:"))
                {
                    fishSpawner.ProcessForceData(data);
                }
                else if (data.StartsWith("ACCEL_Y:"))
                {
                    string valueStr = data.Replace("ACCEL_Y:", "").Trim();

                    if (float.TryParse(valueStr, out float accelY))
                    {
                        fishingController.ProcessAccelY(accelY);
                    }
                }
                else if (data.StartsWith("JOY_Y:"))
                {
                    string valueStr = data.Replace("JOY_Y:", "").Trim();

                    if (int.TryParse(valueStr, out int joyValue))
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
                            seaweedTrigger.Wiggle();
                        }
                    }
                }
                else if (data.StartsWith("BUTTON:"))
                {
                    string valueStr = data.Replace("BUTTON:", "").Trim();

                    if (valueStr == "yes")
                    {
                        overlayFade.ToggleOverlay();
                    }
                }
                else if (data.StartsWith("SCENE:"))
                {
                    string valueStr = data.Replace("SCENE:", "").Trim();

                    if (int.TryParse(valueStr, out int sceneState))
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
                else if (fruitSpawner != null && fruitSpawner.activeFruit != null)
                {
                    fruitSpawner.activeFruit.ProcessInput(data);
                }
            }
        }
        catch (System.TimeoutException)
        {
            // Normal serial timeout
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
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