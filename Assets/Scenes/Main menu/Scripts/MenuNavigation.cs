using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public static MenuNavigation instance { get; private set; }

    public GameObject mainMenuScreen;
    public GameObject settingsScreen;

    public GameObject quitWidget;
    public GameObject DeleteWidget;

    public Animator cameraAnimator;

    public TMP_Dropdown screenResDropdown;
    public Toggle fullscreenToggle;

    public Transform playerTransform;
    public float playerSlerp;
    public Transform playerTarget;

    private bool shouldFullScreen = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        shouldFullScreen = fullscreenToggle.isOn;

        float x = Screen.currentResolution.width;
        float y = Screen.currentResolution.height;

        if (!screenResDropdown.options.Any(option => option.text == $"{x}x{y}"))
        {
            List<string> list = new List<string>();

            list.Add($"{x}x{y}");

            screenResDropdown.AddOptions(list);

            List<TMP_Dropdown.OptionData> sortedOptions = screenResDropdown.options
                .OrderBy(option => option.text)
                .ToList();

            sortedOptions.Reverse();

            screenResDropdown.options = sortedOptions;
            screenResDropdown.RefreshShownValue();
        }

        int index = screenResDropdown.options.FindIndex(option => option.text == $"{x}x{y}");
        screenResDropdown.SetValueWithoutNotify(index);

        settingsScreen.SetActive(false);

    }

    private void Update()
    {
        float slerpPos = Mathf.Lerp(playerTransform.position.x, playerTarget.position.x, playerSlerp);

        playerTransform.position = new(slerpPos, playerTransform.position.y, playerTransform.position.z);
    }


    public void ClickStart()
    {
        GameManager.NextScene();
        GameManager.time = new GameManager.FinalTime();
    }
    

    public void ClickQuit()
    {
        quitWidget.SetActive(true);
    }

    public void ClickQuitYes()
    {
        Application.Quit();
    }

    public void ClickQuitNo()
    {
        quitWidget.SetActive(false);
    }
    public void ClickDeleteYes()
    {
        DeleteWidget.SetActive(false);
        SaveSystem.Delete();
    }

    public void ClickDeleteNo()
    {

    }
    public void ClickSettings()
    {
        settingsScreen.SetActive(true);
        cameraAnimator.SetTrigger("MenuToSettings");
    }

    public void ClickCloseSettings()
    {
        mainMenuScreen.SetActive(true);
        cameraAnimator.SetTrigger("SettingsToMenu");
    }


    public void ToggleFullscreen()
    {
        shouldFullScreen = fullscreenToggle.isOn;
        Screen.fullScreen = shouldFullScreen;
    }

    public void DropdownResolution()
    {
        int index = screenResDropdown.value;
        string resString = screenResDropdown.options[index].text;
        string[] resStringArray = resString.Split('x');

        int x = int.Parse(resStringArray[0]);
        int y = int.Parse(resStringArray[1]);

        Screen.SetResolution(x, y, shouldFullScreen);

    }


    public static void HideMainMenuScreen()
    {
        instance.mainMenuScreen.SetActive(false);
    }

    public static void HideSettingsScreen()
    {
        instance.settingsScreen.SetActive(false);
    }
}
