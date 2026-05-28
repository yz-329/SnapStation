using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;

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
        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            string data = serialPort.ReadLine().Trim();

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "SampleScene")
            {
                if (data.StartsWith("UID:"))
                {
                    if (introScreen != null) 
                    {
                        SpriteRenderer introSr = introScreen.GetComponent<SpriteRenderer>();
                        if (introSr != null)
                        {
                            introSr.enabled = false;
                        }
                    }

                    Debug.Log(data);
                    string uid = data.Replace("UID:", "").Trim();

                    fishSpawner.ProcessNFCData(uid);
                }

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
                if (fruitSpawner != null)
                {
                    // 1. Check for NFC Scan
                    if (data.StartsWith("UID:"))
                    {
                        string uid = data.Replace("UID:", "").Trim();
                        fruitSpawner.ActivateFruit(uid);
                    }
                    // 2. Forward ALL OTHER physical inputs to the active fruit
                    else if (fruitSpawner.activeFruit != null)
                    {
                        fruitSpawner.activeFruit.ProcessInput(data);
                    }
                }
            }
        }
        catch (System.TimeoutException)
        {
            // Normal serial timeout
        }
        catch (System.Exception e)
        {
            // Debug.LogWarning(e.Message);
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