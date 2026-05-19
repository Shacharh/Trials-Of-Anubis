using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using IACFPSController.Managers;
using TMPro;
using UnityEngine.Events;

namespace IACFPSController.Managers
{
    [AddComponentMenu("IAC/Axe Game/Managers/Time Manager")]
    public class TimeManager : MonoBehaviour
    {
        private bool gameOver = false;

        [Tooltip("Define the starting time for the game")]
        public enum TimeMode
        {
            CountUp, CountDown, Manual
        };
        public TimeMode timeMode = TimeMode.CountDown;
        public float startTime;
        [HideInInspector] public float timeLeft;
        private TimeSpan timerDisplay;
        public bool displayAsNumbers = false;
        private string displayValue;
        

        [Header("UI Elements")]
        public TMP_Text[] timerTexts;
        public Color warningColor = Color.red;
        private Color originalColor;

        [Header("UI Elements")]
        public float endTimeValue;
        public UnityEvent endTimeEvent;
        
   
        // Start is called before the first frame update
        void Start()
        {
            gameOver = false;
            switch (timeMode)
            {
                case TimeMode.CountUp:
                timeLeft = 0;
                break;

                case TimeMode.CountDown:
                timeLeft = startTime;
                endTimeValue = 0;
                break;

                case TimeMode.Manual:
                timeLeft = 0;
                break;
            }

            if (timerTexts.Length > 0)
            {
                foreach (var timerText in timerTexts)
                {
                    originalColor = timerText.color;
                }
            }

            InvokeRepeating("UpdateTimer", 0f, 1f);
        }

        // Update is called once per frame
        void Update()
        {
            if (gameOver)
            {
                return;
            }

            switch (timeMode)
            {
                case TimeMode.CountUp:
                timeLeft += Time.deltaTime;
                break;

                case TimeMode.CountDown:
                timeLeft -= Time.deltaTime;
                break;

                case TimeMode.Manual:
                break;
            }
        

            if (timeLeft <= 0 && timeMode == TimeMode.CountDown)
            {
               EndGameEvent();
            }
            else if(timeMode != TimeMode.CountDown && timeLeft >= endTimeValue)
            {
                EndGameEvent();
            }
        }

        public void UpdateTimer()
        {
            if (gameOver)
            { return; }

            timerDisplay = TimeSpan.FromSeconds(timeLeft);

            if (timerTexts.Length > 0)
            {
              if(timeMode == TimeMode.CountDown)
              {
                if (timeLeft < 60f)
                {
                    foreach (var timerText in timerTexts)
                    {
                        timerText.color = warningColor;
                    }
                }
                else 
                {
                    foreach (var timerText in timerTexts)
                    {
                        timerText.color = originalColor;
                    }
                }
              }
                GetTimeValues();

                foreach (var timerText in timerTexts)
                {
                    timerText.text = displayValue;
                }
            }
            else
            {
                GetTimeValues();
                Debug.Log(displayValue);
            }
        }

        public void AddTime(float timeToAdd)
        {
            if (gameOver)
            {
                return;
            }

            switch (timeMode)
            {
                case TimeMode.CountUp:
                timeLeft -= timeToAdd;
                break;

                case TimeMode.CountDown:
                timeLeft += timeToAdd;
                break;

                case TimeMode.Manual:
                timeLeft += timeToAdd;
                break;
            }

            UpdateTimer();
        }

        public void EndGameEvent()
        {
            gameOver = true;
            endTimeEvent.Invoke();
        }

        private void GetTimeValues()
        {
            if (displayAsNumbers)
            {
                displayValue = Mathf.CeilToInt(timeLeft).ToString();
            }
            else
            {
                displayValue = timerDisplay.ToString("mm':'ss");
            }
        }

    }
}
