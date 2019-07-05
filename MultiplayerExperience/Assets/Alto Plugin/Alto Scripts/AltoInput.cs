using UnityEngine;
using AltoDriver;

namespace Alto
{
public class AltoInput : MonoBehaviour
{

    // Singelton pattern
    static AltoInput instance;
    static AltoInput Instance
    {
        get
        {
            if (instance == null)
            {

                instance = FindObjectOfType<AltoInput>();
                if (instance == null) { instance = new GameObject(typeof(AltoInput).Name).AddComponent<AltoInput>(); }
                DontDestroyOnLoad(instance);
                }
            return instance;
        }
    }

    static AltoConfig altoConfig;
    static AltoConfig AltoConfigInstance
        {
            get
            {
                if(altoConfig == null)
                {
                    altoConfig = FindObjectOfType<AltoConfig>();
                    if(altoConfig == null) { altoConfig = new GameObject(typeof(AltoConfig).Name).AddComponent<AltoConfig>(); }
                    DontDestroyOnLoad(altoConfig);
                }
                return altoConfig;
            }
        }

    public static float GetDirection()
    {

        return Instance.GetDirectionInternal();
    }

    public static float GetMagnitudeNormalised()
    {

        return Instance.GetMagnitudeNormalisedInternal();
    }

    public static bool GetOn()
    {
        return Instance.GetOnInternal();
    }

    public static bool IsPresent()
    {
        return Instance.IsPresentInternal();
    }

    public static void SendHapticCommand(string state)
    {
        Instance.SendHapticCommandInternal(state, 9);
    }

    public static void SendHapticCommand(string state, int intensity)
    {
        Instance.SendHapticCommandInternal(state, intensity);
    }

    public static string CreateNewHapticState(string name, int motor, int min, int max, bool continuous)
    {
        return Instance.CreateNewHapticStateInternal(name, motor, min, max, continuous);
    }

    public static void ResetAlto()
    {
        Instance.ResetAltoInternal();
    }

    //Config getters
    public static int GetConfigInt(string section, string key)
    {
        float fValue = AltoConfigInstance.GetValue(section, key);
        return (int)fValue;
    }

    public static float GetConfigFloat(string section, string key)
    {
        return AltoConfigInstance.GetValue(section, key);
    }

    public static string GetConfigString(string section, string key)
    {
        return AltoConfigInstance.GetSetting(section, key);
    }

    private Alto_Driver driver;
    private string pID;
    private string vID;

    void Start()
    {
        string vID = AltoConfigInstance.GetSetting("Alto", "VID");
        string pID = AltoConfigInstance.GetSetting("Alto", "PID");
        Debug.Log("vID:" + vID + ", pID:" + pID);
        driver = new Alto_Driver(vID, pID);
            Debug.Log("Alto driver created");
        //for moving across the ground
        driver.CreateHapticState("Ground", Alto_Driver.MotorName.MotorOne, 0, 175, Alto_Driver.MotorActivationType.Continuous);
        //for when in the air
        driver.CreateHapticState("Hover", Alto_Driver.MotorName.MotorTwo, 25, 160, Alto_Driver.MotorActivationType.Continuous);
        //for when on water
        driver.CreateHapticState("Water", Alto_Driver.MotorName.MotorOne, 0, 200, Alto_Driver.MotorActivationType.Continuous);
        //for when collided with object or land with jetpack
        driver.CreateHapticState("Impact", Alto_Driver.MotorName.MotorTwo, 150, 1000, Alto_Driver.MotorActivationType.Continuous);
        //for when Alto represents a vehicle (steam engine style)
        driver.CreateHapticState("Vehicle", Alto_Driver.MotorName.MotorOne, 0, 200, Alto_Driver.MotorActivationType.Continuous);
        //for when stopping Alto haptics
        driver.CreateHapticState("StopMotor1", Alto_Driver.MotorName.MotorOne, 0, 0, Alto_Driver.MotorActivationType.Discrete);
        driver.CreateHapticState("StopMotor2", Alto_Driver.MotorName.MotorTwo, 0, 0, Alto_Driver.MotorActivationType.Discrete);
    }

    void OnApplicationQuit()
    {
        driver.Shutdown();
    }

    private void ResetAltoInternal()
    {
        driver.ResetDriver();
    }

    private float GetDirectionInternal()
    {
        return driver.data.direction;
    }

    private float GetMagnitudeNormalisedInternal()
    {
        return Mathf.Clamp(driver.data.magnitude / 100f, 0, 1);
    }

    private bool GetOnInternal()
    {
        return driver.data.on;
    }

    bool IsPresentInternal()
    {
        return driver.data.valid;
    }

    private void SendHapticCommandInternal(string state, int intensity)
    {
        driver.sendHapticCommand(state, intensity);
    }

    private string CreateNewHapticStateInternal(string name, int motor, int min, int max, bool continuous)
    {
        Alto_Driver.MotorName motorChoice;
        if (motor > 1)
        {
            motorChoice = Alto_Driver.MotorName.MotorTwo;
        }
        else
        {
            motorChoice = Alto_Driver.MotorName.MotorOne;
        }
        Alto_Driver.MotorActivationType motorActivationChoice;
        if (continuous)
        {
            motorActivationChoice = Alto_Driver.MotorActivationType.Continuous;
        }
        else
        {
            motorActivationChoice = Alto_Driver.MotorActivationType.Discrete;
        }
        driver.CreateHapticState(name, motorChoice, min, max, motorActivationChoice);
        return name;
    }
    }
}
