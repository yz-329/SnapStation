using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbmodem1201";
    public int baudRate = 9600;

    private SerialPort serialPort;

    private int currentSceneState = -1;

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

            // Debug.Log(data);

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
            Debug.LogWarning(e.Message);
        }
    }

    public void ProcessSceneSwitch(int sceneState)
    {
        // If the same cartridge is pressed again (spring mechanism pops it out)
        if (sceneState == currentSceneState)
        {
            // If we are already on the Intro screen (0), just ignore it so it doesn't reload infinitely
            if (currentSceneState == 0) 
                return;

            Debug.Log("Cartridge popped out! Returning to Intro.");
            
            // Reset internal state to Intro
            currentSceneState = 0; 
            SceneManager.LoadScene("IntroScreen");
            return;
        }

        // Otherwise, load the new scene normally
        currentSceneState = sceneState;

        Debug.Log("Switching Scene: " + sceneState);

        switch (sceneState)
        {
            case 0:
                SceneManager.LoadScene("IntroScreen");
                break;

            case 1:
                SceneManager.LoadScene("Cooking_Scene");
                break;

            case 2:
                SceneManager.LoadScene("SampleScene");
                break;
        }
    }    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}