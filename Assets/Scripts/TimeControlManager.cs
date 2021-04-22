using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TimeControlManager : MonoBehaviour
{
    //public fields
    public bool isPaused;

    //private fields
    private const float TimeStep = 0.15f;
    [SerializeField]private int currentTimeStep;
    private int selectedTimeStep;
    private float ticktimer;
    private int day;    
    private int month;   
    private int year;

    //Serialized Private Fields
    [SerializeField] private int tick;
    [SerializeField] private int ticksPerDay;
    [SerializeField] private int daysPerMonth;
    [SerializeField] private int monthsPerYear;
    [SerializeField] private int[] timeSteps;

    //UI Elements
    [SerializeField] private TextMeshProUGUI currentDateDisplay;
    [SerializeField] private GameObject pausedIndicator;
    [SerializeField] private GameObject[] speedIndicators;
    [SerializeField] private Toggle pausedToggle;

    //Events
    public delegate void DayUpdateTick();
    public static event DayUpdateTick OnDayTick;
    public delegate void MonthUpdateTick();
    public static event MonthUpdateTick OnMonthTick;
    public delegate void YearUpdateTick();
    public static event YearUpdateTick OnYearTick;

    void Awake()
    {
        tick = 0;
        day = 1;
        month = 1;
        year = 1;
        isPaused = true;
        selectedTimeStep = 0;
        currentTimeStep = timeSteps[selectedTimeStep];
        foreach (GameObject visualizer in speedIndicators)
        {
            visualizer.SetActive(false);
        }
        UpdateTimeVisulaizer(selectedTimeStep);
        UpdateDateUI();
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }
        else
        {
            ticktimer += Time.deltaTime * currentTimeStep;
            if (ticktimer >= TimeStep)
            {
                ticktimer -= TimeStep;
                tick++;
            }
            if (tick >= ticksPerDay)
            {
                tick -= ticksPerDay - 1;
                day++;
                OnDayTick?.Invoke();
                //Debug.Log("Day Update Tick");
            }
            if(day >= daysPerMonth)
            {
                day -= (daysPerMonth - 1);
                month++;
                OnMonthTick?.Invoke();
                //Debug.Log("Month Update Tick");
            }
            if (month >= monthsPerYear)
            {
                month -= (monthsPerYear - 1);
                year++;
                OnYearTick?.Invoke();
                //Debug.Log("Year Update Tick");
            }
        }
        UpdateDateUI();
    }

    public void IncreaseTimeStep()
    {
        if (selectedTimeStep == timeSteps.Length - 1)
        {
            return;
        }
        else
        {
            selectedTimeStep++;
        }
        currentTimeStep = timeSteps[selectedTimeStep];
        Debug.Log("current time step: " + selectedTimeStep);
        UpdateTimeVisulaizer(selectedTimeStep);
    }

    public void DecreaseTimeStep()
    {
        if (selectedTimeStep == 0)
        {
            return;
        }
        else
        {
            selectedTimeStep--;
        }
        currentTimeStep = timeSteps[selectedTimeStep];
        Debug.Log("current time step: " + selectedTimeStep);
        UpdateTimeVisulaizer(selectedTimeStep + 1);
    }

    public void Pause()
    {
        bool justChanged = false;
        if (isPaused)
        {
            if (!justChanged)
            {
                isPaused = false;
                Debug.Log("Game is running");
                justChanged = true;
                UpdateDateUI();
                pausedToggle.graphic.enabled = false;
            }
            else
            {
                return;
            }
        }
        if (!isPaused)
        {
            if (!justChanged)
            {
                isPaused = true;
                Debug.Log("Game is pasued");
                UpdateDateUI();
                pausedToggle.graphic.enabled = true;
            }
            else
            {
                return;
            }
        }
    }

    public void UIPause()
    {
        if (isPaused == true)
        {
            return;
        }
        else
        {
            isPaused = true;
            UpdateDateUI();
            pausedToggle.graphic.enabled = true;
            Debug.Log("Paused");
        }
    }

    void UpdateDateUI()
    {
        currentDateDisplay.text = day.ToString() + "/" + month.ToString() + "/" + year.ToString();
        if (!isPaused)
        {
            pausedIndicator.SetActive(false);
        }
        if (isPaused)
        {
            pausedIndicator.SetActive(true);
        }
    }

    void UpdateTimeVisulaizer(int index)
    {
        if (index > speedIndicators.Length - 1)
        {
            Debug.LogError("Time Step Visulizer out of Range of array");
            return;
        }
        if (index != selectedTimeStep)
        {
            speedIndicators[index].SetActive(false);
        }
        else
        {
            speedIndicators[index].SetActive(true);
        }
    }
}
