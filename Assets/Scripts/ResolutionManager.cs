using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolution;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    void Start()
    {
        resolution = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolution.Length; ++i)
        {
            if (resolution[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolution[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; ++i)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + "hz";
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
}
