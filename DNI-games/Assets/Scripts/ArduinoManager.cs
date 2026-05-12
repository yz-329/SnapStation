using UnityEngine;
using System.IO.Ports;

public class ArduinoManager : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "/dev/tty.usbserial-59690940491";
    public int baudRate = 9600;

    [Header("References")]
    public FishSpawner fishSpawner;
    public FishingController fishingController;
    public HookMovement hookMovement;

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

            Debug.Log(data);

            // =========================
            // NFC
            // =========================
            if (data.StartsWith("UID:"))
            {
                fishSpawner.ProcessNFCData(data);
            }

            // =========================
            // FORCE SENSOR
            // =========================
            else if (data.StartsWith("FORCE:"))
            {
                fishSpawner.ProcessForceData(data);
            }

            // =========================
            // CAST ACTION
            // =========================
            else if (data.StartsWith("ACCEL_Y:"))
            {
                string valueStr = data.Replace("ACCEL_Y:", "").Trim();

                if (float.TryParse(valueStr, out float accelY))
                {
                    fishingController.ProcessAccelY(accelY);
                }
            }

            // =========================
            // JOYSTICK REELING
            // =========================
            else if (data.StartsWith("JOY_Y:"))
            {
                string valueStr = data.Replace("JOY_Y:", "").Trim();

                if (int.TryParse(valueStr, out int joyValue))
                {
                    hookMovement.ProcessJoystick(joyValue);
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