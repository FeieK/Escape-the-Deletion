using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool doTimeTick = false;
    public static FinalTime time;

    public TextMeshProUGUI timeText;

    [Space(10)]

    public int currentSceneIndex;

    public SaveSystem saveData;


    private int tick;

    private void Awake()
    {
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            time = new FinalTime();
            SaveSystem.Load();
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Update()
    {
        if (doTimeTick)
        {
            tick = (int)(Time.deltaTime * 1000);
            time.minSecond += tick;
            time.second += time.minSecond / 1000;
            time.minSecond %= 1000;

            time.minute += time.second / 60;
            time.second %= 60;

            time.hour += time.minute / 60;
            time.minute %= 60;
        }

        string milSec = time.minSecond.ToString();
        string sec = time.second.ToString();
        string min = time.minute.ToString();
        string hour = time.hour.ToString();
        if (time.minSecond < 100)
        {
            milSec = $"0{time.minSecond}";
        }
        if (time.minSecond < 10)
        {
            milSec = $"0{time.minSecond}";
        }
        if (time.second < 10)
        {
            sec = $"0{time.second}";
        }
        if (time.minute < 10)
        {
            min = $"0{time.minute}";
        }

        if (timeText == null)
        {
            GameObject timerObj = GameObject.FindGameObjectWithTag("Timer");
            if (timerObj != null)
            {
                timeText = timerObj.GetComponent<TextMeshProUGUI>();
            }
        }
        if (timeText != null)
        {
            timeText.text = $"{hour}:{min}:{sec}:{milSec}";
        }
    }
    public static void NextScene()
    {
        instance.currentSceneIndex++;
        try
        {
            SceneManager.GetSceneByBuildIndex(instance.currentSceneIndex);
        }
        catch {
            instance.currentSceneIndex = 0;
        }
        SceneManager.LoadScene(instance.currentSceneIndex);

    }

    [Serializable]
    public class FinalTime
    {
        public int hour;
        public int minute;
        public int second;
        public int minSecond;

        public FinalTime(int hour, int minute, int second, int minSecond)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.minSecond = minSecond;
        }
        public FinalTime()
        {
            this.hour = 0;
            this.minute = 0;
            this.second = 0;
            this.minSecond = 0;
        }

        public static bool IsNewRecord(FinalTime timeA, FinalTime timeB)
        {
            int tickA = GetTick(timeA);
            int tickB = GetTick(timeB);

            return tickA < tickB || tickB == 0;
        }
        public static int GetTick(FinalTime time)
        {
            int tick = 0;
            tick += time.minSecond;
            tick += time.second * 1000;
            tick += time.minute * 60 * 1000;
            tick += time.hour * 60 * 60 * 1000;
            return tick;
        }
        public static FinalTime GetTime(int tick)
        {
            FinalTime newTime = new FinalTime();
            newTime.minSecond += tick;
            newTime.second += newTime.minSecond / 1000;
            newTime.minSecond %= 1000;

            newTime.minute += newTime.second / 60;
            newTime.second %= 60;

            newTime.hour += time.minute / 60;
            newTime.minute %= 60;
            return newTime;
        }
    }
}
