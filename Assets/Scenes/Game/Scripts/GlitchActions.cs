using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class GlitchAction : MonoBehaviour
{
    public Volume invertedColorsVolume;
    public float invertColorsSwitchSpeed;
    public GameObject GapFillersObj;

    GameObject playerObj;
    Player playerScript;

    InvertState invertingState;

    public enum Action
    {
        INVERT_CONTROLLS,
        INVERT_COLORS,
        FLIP_CAMERA,
        HIDE_GAPS
    }

    enum InvertState
    {
        NORMAL,
        TO_INVERT,
        INVERTED,
        TO_NORMAL
    }


    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null )
        {
            playerScript = playerObj.GetComponent<Player>();
            if (playerScript == null)
            {
                Debug.LogError("PlayerObj does not contain component \"Player\" ");
            }
        }
    }
    public void PreformAction(Action action)
    {
        switch (action)
        {
            case Action.INVERT_CONTROLLS:
                {
                    playerScript.invertControlls = true;
                    break;
                }
            case Action.INVERT_COLORS:
                {
                    if (invertingState != InvertState.TO_INVERT && invertingState != InvertState.INVERTED)
                    {
                        invertingState = InvertState.TO_INVERT;
                        StartCoroutine(VolumeWeightTransmission(1));
                    }
                    break;
                }
            case Action.FLIP_CAMERA:
                {
                    Camera.main.gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
                    break;
                }
            case Action.HIDE_GAPS:
                {
                    GapFillersObj.SetActive(true);
                    break;
                }
        }
        StartCoroutine(DelayedRestoreAction(action, 10));
    }

    public void RestoreAction(Action action)
    {
        switch (action)
        {
            case Action.INVERT_CONTROLLS:
                {
                    playerScript.invertControlls = false;
                    break;
                }
            case Action.INVERT_COLORS:
                {
                    if (invertingState != InvertState.TO_NORMAL && invertingState != InvertState.NORMAL)
                    {
                        invertingState = InvertState.TO_NORMAL;
                        StartCoroutine(VolumeWeightTransmission(0));
                    }
                    break;
                }
            case Action.FLIP_CAMERA:
                {
                    Camera.main.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                }
            case Action.HIDE_GAPS:
                {
                    GapFillersObj.SetActive(false);
                    break;
                }
        }
    }

    IEnumerator VolumeWeightTransmission(float target)
    {
        if (invertingState == InvertState.TO_INVERT)
        {
            float progress = 0;
            while (progress < 1)
            {
                invertedColorsVolume.weight = progress;
                progress = Mathf.Clamp(progress + invertColorsSwitchSpeed, 0, 1);
                yield return new WaitForFixedUpdate();
            }
            invertingState = InvertState.INVERTED;
        }
        if (invertingState == InvertState.TO_NORMAL)
        {
            float progress = 1;
            while (progress > 0)
            {
                invertedColorsVolume.weight = progress;
                progress = Mathf.Clamp(progress - invertColorsSwitchSpeed, 0, 1);
                yield return new WaitForFixedUpdate();
            }
            invertingState = InvertState.NORMAL;
        }
    }

    IEnumerator DelayedRestoreAction(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RestoreAction(action);
    }
}
