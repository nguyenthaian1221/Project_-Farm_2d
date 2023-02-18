using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class DayTimeController : MonoBehaviour
{
    const float SecondsInDay = 86400f;
    public float time;
    [SerializeField] Text TimeDisplay;
    [SerializeField] float TimeScale;
    [SerializeField] float LightTransition = 0.0001f;
    public int hungerUpdaterCounter;
    public int healthUpdaterCounter;
    public int temperatureUpdateCounter;
    public int day;

    private void Start()
    {
        day = 0;
        time = 25200f;
        hungerUpdaterCounter = 0;
        healthUpdaterCounter = 0;
        temperatureUpdateCounter = 0;
        TemperatureController.currentTemperature = 100;
    }
    private float getHours
    {
        get { return time / 3600f; }
    }

    public float GetTime
    {
        get { return time; }
    }


    void Update()
    {
        if (Time.timeScale == 0)
            return;

        //Hunger and temperature indicator counter
        hungerUpdaterCounter += 1;
        temperatureUpdateCounter += 1;
        //Here you can adjust the rate of decrease

        if (hungerUpdaterCounter == 250)
        {
            HungerController.currentHunger -= 1;
            hungerUpdaterCounter = 0;
        }
        //when hunger point or temperature point is lower than 10, health starts to deplete:
        if (HungerController.currentHunger < 10 || TemperatureController.currentTemperature < 10)
        {
            healthUpdaterCounter += 1;
            //Here you can adjust the rate at which your health stats decrease
            if (healthUpdaterCounter == 100)
            {
                HealthController.currentHealth -= 1;
                healthUpdaterCounter = 0;
            }
        }


        //Control and display the time
        time += Time.deltaTime * TimeScale;
        int hours = (int)getHours;
        TimeDisplay.text = hours.ToString("00") + ":00";
        Light2D light = transform.GetComponent<Light2D>();

        //Daylight from 5am to 6pm
        if (time > 25200f && time < 64800f)
        {
            light.intensity = 1f;
            TemperatureController.currentTemperature = 100;
        }


        //It gets darker from 6pm - 5am
        if ((time > 64800f && time < 86400f) || ((time > 0f && time < 18000f)))
        {
            if (light.intensity > 0.3f)
            {
                light.intensity -= LightTransition;
            }
            if (temperatureUpdateCounter > 50)
            {
                TemperatureController.currentTemperature -= 1;
                temperatureUpdateCounter = 0;
            }
        }
        
        //Lights up 5am - 7am
        if (time > 18000f && time < 25200f)
        {
            if (light.intensity < 1f)
                light.intensity += LightTransition;
            if (temperatureUpdateCounter > 50)
            {
                TemperatureController.currentTemperature -= 1;
                temperatureUpdateCounter = 0;
            }
        }

        //Change the date to a new day
        if (time > SecondsInDay)
        {
            time = 0;
            day += 1;
            //daily increase money 
            MoneyController.money += 200;
        }
        //
        if(HealthController.currentHealth < 1)
        {
            Application.LoadLevel(3);
        }
        if(day == 9 && time > 25200f)
        {
            Application.LoadLevel(4);
        }
    }

}
