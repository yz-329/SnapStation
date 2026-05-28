using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbserial-59690940491";
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

            Debug.Log(data);

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
        // Prevent reloading same scene
        if (sceneState == currentSceneState)
            return;

        currentSceneState = sceneState;

        Debug.Log("Switching Scene: " + sceneState);

        switch (sceneState)
        {
            case 0:
                SceneManager.LoadScene("IntroScreen");
                break;

            case 1:
                SceneManager.LoadScene("SampleScene");
                break;

            case 2:
                SceneManager.LoadScene("Cooking_Scene");
                break;
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