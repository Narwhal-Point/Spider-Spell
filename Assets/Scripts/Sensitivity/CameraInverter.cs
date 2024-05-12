using UnityEngine;
using UnityEngine.UI;

public class CameraInverter : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook playerCamera;
    public Cinemachine.CinemachineFreeLook aimCamera;
    public Toggle invertXToggle;
    public Toggle invertYToggle;

    private const string invertXPlayerPrefsKey = "InvertX";
    private const string invertYPlayerPrefsKey = "InvertY";

    private void Start()
    {
        // playerCamera = GetComponent<Cinemachine.CinemachineFreeLook>();
        
        // Load previous settings from PlayerPrefs
        bool invertX = PlayerPrefs.GetInt(invertXPlayerPrefsKey, 0) == 1;
        bool invertY = PlayerPrefs.GetInt(invertYPlayerPrefsKey, 0) == 1;

        // Set toggles based on loaded settings
        invertXToggle.isOn = invertX;
        invertYToggle.isOn = invertY;

        // Apply inversion settings to the camera
        playerCamera.m_XAxis.m_InvertInput = invertX;
        playerCamera.m_YAxis.m_InvertInput = !invertY;
        aimCamera.m_XAxis.m_InvertInput = invertX;
        aimCamera.m_YAxis.m_InvertInput = !invertY;
    }

    public void ToggleInvertX(Toggle value)
    {
        // Set inversion setting for X axis
        playerCamera.m_XAxis.m_InvertInput = value.isOn;
        PlayerPrefs.SetInt(invertXPlayerPrefsKey, value.isOn ? 1 : 0);
    }

    public void ToggleInvertY(Toggle value)
    {
        // Set inversion setting for Y axis
        playerCamera.m_YAxis.m_InvertInput = !value.isOn;
        PlayerPrefs.SetInt(invertYPlayerPrefsKey, value.isOn ? 1 : 0);
    }
}