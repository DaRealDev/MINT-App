using UnityEngine;
using ArduinoBluetoothAPI;

public class BluetoothCommunication : MonoBehaviour
{
    private BluetoothHelper bt;

    /// <summary>
    /// This method automatically connects to the HC-05 module when the application starts.
    /// </summary>
    private void Start()
    {
        ReceiveData();
        Connect();
    }

    /// <summary>
    /// This method creates a connection between the HC-055 module and the current device.
    /// </summary>
    public void Connect()
    {
        try
        {
            bt = BluetoothHelper.GetInstance("Regenfallrohr");
            bt.OnConnected += OnConnected;
            bt.OnConnectionFailed += OnConnectionFailed;
            bt.setTerminatorBasedStream("\n");

            if (bt.isDevicePaired())
            {
                bt.Connect();
            }
        }
        catch (System.Exception)
        {
            print("Could not connect!");
        }
    }

    /// <summary>
    /// This method is called when the connection process has been successfully finished.
    /// It makes this device listen to the bluetooth module.
    /// </summary>
    void OnConnected()
    {
        bt.StartListening();
        // make microcontroller know that it is connected to this mobile device now
        SendData("openBT");
    }

    /// <summary>
    /// This method sends data to the bluetooth module.
    /// </summary>
    /// <param name="data"> the data that is supposed to be sent </param>
    public void SendData(string data)
    {
        if (bt != null && data != null && data != "")
        {
            bt.SendData(data);
        }
    }

    /// <summary>
    /// This method is called every frame and makes this device wait for available data.
    /// </summary>
    public void ReceiveData()
    {
        if (bt.Available)
        {
            string msg = bt.Read();
            string[] data = msg.Split(';');

            // extract values from received string
            float temperature = (float)System.Convert.ToDouble(data[0].Replace("temp;", ""));
            float humidity    = (float)System.Convert.ToDouble(data[1].Replace("hum;",  ""));
            float voltage     = (float)System.Convert.ToDouble(data[2].Replace("volt;", ""));

            // get current time
            System.DateTime now = System.DateTime.Now; // temporarily

            // add values to their corresponding statistics
            UIManager.GetInstance().GetTemperatureStatistics().AddPoint(now, temperature);
            UIManager.GetInstance().GetHumidityStatistics()   .AddPoint(now, humidity);
            UIManager.GetInstance().GetVoltageStatistics()    .AddPoint(now, voltage);
        }
    }

    /// <summary>
    /// This is called when no connection could be built up.
    /// </summary>
    void OnConnectionFailed()
    {
        print("Failed!");
    }

    private void Update()
    {
        if (bt != null)
        {
            ReceiveData();
        }
    }

    /// <summary>
    /// This is called when the application is closed. Afterwards, the the device is disconnected.
    /// </summary>
    void OnDestroy()
    {
        if (bt != null)
        {
            // make microcontroller know that it is not connected to this mobile device an more
            SendData("closeBT");
            bt.Disconnect();
        }
    }
}
