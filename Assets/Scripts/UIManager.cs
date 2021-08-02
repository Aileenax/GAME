using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private const string VolumeSliderValue = "VOLUME_SLIDER_VALUE";

    [SerializeField] private GameObject _settingsMenuPanel;

    // Pause panel is used to stop player from interacting with the grid when looking at settings
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Slider _volumeSlider;

    public void SettingsButtonClicked()
    {
        if (_settingsMenuPanel.activeInHierarchy)
        {
            _pausePanel.SetActive(false);
            _settingsMenuPanel.SetActive(false);
        }
        else
        {
            _pausePanel.SetActive(true);
            _volumeSlider.value = PlayerPrefs.GetFloat(VolumeSliderValue);
            _settingsMenuPanel.SetActive(true);
        }
    }

    public void OnSliderValueChanged()
    {
        PlayerPrefs.SetFloat(VolumeSliderValue, _volumeSlider.value);
    }
}
