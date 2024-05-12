using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraSensitivityController : MonoBehaviour
{
    public Slider sensitivitySlider;
    public CinemachineFreeLook freeLookCamera;

    // Default sensitivity values for X and Y axes
    private float defaultXAxisSpeed;
    private float defaultYAxisSpeed;

    private const string sensitivityKey = "CameraSensitivityMultiplier";

    private void Start()
    {
        // Save default sensitivity values
        defaultXAxisSpeed = freeLookCamera.m_XAxis.m_MaxSpeed;
        defaultYAxisSpeed = freeLookCamera.m_YAxis.m_MaxSpeed;

        // Load sensitivity multiplier from PlayerPrefs or use default value if not set
        float sensitivityMultiplier = PlayerPrefs.GetFloat(sensitivityKey, 1f);
        sensitivitySlider.value = sensitivityMultiplier;

        // Initialize sensitivity based on slider value
        UpdateSensitivity();
    }

    public void ChangeSensitivity()
    {
        // Update sensitivity based on slider value
        UpdateSensitivity();

        // Save sensitivity multiplier to PlayerPrefs
        PlayerPrefs.SetFloat(sensitivityKey, sensitivitySlider.value);
        PlayerPrefs.Save();
    }

    private void UpdateSensitivity()
    {
        // Apply sensitivity multiplier to X and Y axes
        freeLookCamera.m_XAxis.m_MaxSpeed = defaultXAxisSpeed * sensitivitySlider.value;
        freeLookCamera.m_YAxis.m_MaxSpeed = defaultYAxisSpeed * sensitivitySlider.value;
    }
}