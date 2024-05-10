using UnityEngine;
using UnityEngine.UI;

public class CameraInverter : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook freeLookCamera;
    public Toggle invertXToggle;
    public Toggle invertYToggle;

    private const string invertXPlayerPrefsKey = "InvertX";
    private const string invertYPlayerPrefsKey = "InvertY";

    private void Start()
    {
        // Load previous settings from PlayerPrefs
        bool invertX = PlayerPrefs.GetInt(invertXPlayerPrefsKey, 0) == 1;
        bool invertY = PlayerPrefs.GetInt(invertYPlayerPrefsKey, 0) == 1;

        // Set toggles based on loaded settings
        invertXToggle.isOn = invertX;
        invertYToggle.isOn = invertY;

        // Apply inversion settings to the camera
        freeLookCamera.m_XAxis.m_InvertInput = invertX;
        freeLookCamera.m_YAxis.m_InvertInput = !invertY;
    }

    public void ToggleInvertX(Toggle value)
    {
        // Set inversion setting for X axis
        freeLookCamera.m_XAxis.m_InvertInput = value.isOn;
        PlayerPrefs.SetInt(invertXPlayerPrefsKey, value.isOn ? 1 : 0);
    }

    public void ToggleInvertY(Toggle value)
    {
        // Set inversion setting for Y axis
        freeLookCamera.m_YAxis.m_InvertInput = !value.isOn;
        PlayerPrefs.SetInt(invertYPlayerPrefsKey, value.isOn ? 1 : 0);
    }
}