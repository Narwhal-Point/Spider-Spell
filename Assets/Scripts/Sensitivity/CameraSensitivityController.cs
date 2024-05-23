using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraSensitivityController : MonoBehaviour
{
    public Slider sensitivitySliderX;
    public Slider sensitivitySliderY;
    public CinemachineFreeLook freeLookCamera;

    // Default sensitivity values for X and Y axes
    private float defaultXAxisSpeed;
    private float defaultYAxisSpeed;

    private const string sensitivityKeyX = "CameraSensitivityMultiplierX";
    private const string sensitivityKeyY = "CameraSensitivityMultiplierY";

    private void Start()
    {
        // Save default sensitivity values
        defaultXAxisSpeed = freeLookCamera.m_XAxis.m_MaxSpeed;
        defaultYAxisSpeed = freeLookCamera.m_YAxis.m_MaxSpeed;

        // Load sensitivity multiplier from PlayerPrefs or use default value if not set
        float sensitivityMultiplierX = PlayerPrefs.GetFloat(sensitivityKeyX, 1f);
        float sensitivityMultiplierY = PlayerPrefs.GetFloat(sensitivityKeyY, 1f);
        sensitivitySliderX.value = sensitivityMultiplierX;
        sensitivitySliderY.value = sensitivityMultiplierY;

        // Initialize sensitivity based on slider value
        UpdateSensitivityX();
        UpdateSensitivityX();
    }

    public void ChangeSensitivityX()
    {
        // Update sensitivity based on slider value
        UpdateSensitivityX();

        // Save sensitivity multiplier to PlayerPrefs
        PlayerPrefs.SetFloat(sensitivityKeyX, sensitivitySliderX.value);
        PlayerPrefs.Save();
    }
    
    public void ChangeSensitivityY()
    {
        // Update sensitivity based on slider value
        UpdateSensitivityY();

        // Save sensitivity multiplier to PlayerPrefs
        PlayerPrefs.SetFloat(sensitivityKeyY, sensitivitySliderY.value);
        PlayerPrefs.Save();
    }

    private void UpdateSensitivityX()
    {
        // Apply sensitivity multiplier to X and Y axes
        freeLookCamera.m_XAxis.m_MaxSpeed = defaultXAxisSpeed * sensitivitySliderX.value;
    }
    
    private void UpdateSensitivityY()
    {
        // Apply sensitivity multiplier to X and Y axes
        freeLookCamera.m_YAxis.m_MaxSpeed = defaultYAxisSpeed * sensitivitySliderY.value;
    }
}