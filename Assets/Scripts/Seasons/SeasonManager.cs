using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Weather
{
    SUNNY,
    RAINY,
    SNOWY
}

public enum Season
{
    SUMMER,
    RAINYSEASON,
    WINTER
}

public class SeasonManager : MonoBehaviour
{
    [SerializeField]
    private float timeMultiplier;

    [SerializeField]
    private float startHour;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private Light sunLight;

    [SerializeField]
    private List<Light> nightLights;

    [SerializeField]
    private float sunriseHour;

    [SerializeField]
    private float sunsetHour;

    [SerializeField]
    private Color dayAmbientLight;

    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;

    [SerializeField]
    private float maxSunLightIntensity;

    [SerializeField]
    private Light moonLight;

    [SerializeField]
    private float maxMoonLightIntensity;

    private DateTime currentTime;

    private DateTime originalStartTime;

    private TimeSpan sunriseTime;

    private TimeSpan sunsetTime;

    float normalminintensity = 0.1f;
    float normalmaxintensity = 0.75f;

    float rainyminintensity = 0.05f;
    float rainymaxintensity = 0.375f;

    [SerializeField]
    private Weather CurrentWeather;

    [SerializeField]
    private Material snowMat;
    [SerializeField]
    private float SnowAmmount;

    public static SeasonManager instance;

    public bool isinroom;

    public new GameObject light;

    public ParticleSystem Rain;
    public ParticleSystem Snow;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentTime = currentTime = new DateTime((long)(DateTime.UtcNow.AddYears(-2021).Ticks * timeMultiplier));

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        isinroom = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
       // RotateSun(); //2022/9/19 cai
        UpdateLightSettings();
        if(GameManager.instance!=null && GameManager.instance.mycontroller!=null)
        {
            Rain.transform.position = GameManager.instance.mycontroller.transform.position + new Vector3(0, 50, 0);
            Snow.transform.position = GameManager.instance.mycontroller.transform.position + new Vector3(0, 50, 0);
        }

    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);//2022/09/12 cai disable night mode 360
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 0, (float)percentage);//2022/09/12 cai disable night mode 360
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    float timezone = -1;

    private void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));

        float minintensity = normalminintensity;
        float maxintensity = normalmaxintensity;


        if (currentTime.Month <= 4)
        {
            if(currentTime.Hour > 18 && currentTime.Hour <= 24 && timezone!=0)
            {
                if(UnityEngine.Random.Range(0,100)<90)
                {
                    CurrentWeather = Weather.SUNNY;
                }
                else
                {
                    CurrentWeather = Weather.RAINY;
                }

                timezone = 0;
            }
            else if (currentTime.Hour > 12 && currentTime.Hour <= 18 && timezone != 1)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SUNNY;
                }
                else
                {
                    CurrentWeather = Weather.RAINY;
                }
                timezone = 1;
            }
            else if (currentTime.Hour > 6 && currentTime.Hour <= 12 && timezone != 2)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SUNNY;
                }
                else
                {
                    CurrentWeather = Weather.RAINY;
                }
                timezone = 2;
            }
            else if (currentTime.Hour > 0 && currentTime.Hour <= 6 && timezone != 3)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SUNNY;
                }
                else
                {
                    CurrentWeather = Weather.RAINY;
                }
                timezone = 3;
            }
        }
        else if (currentTime.Month <= 8)
        {
            if (currentTime.Hour > 18 && currentTime.Hour <= 24 && timezone != 0)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.RAINY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }

                timezone = 0;
            }
            else if (currentTime.Hour > 12 && currentTime.Hour <= 18 && timezone != 1)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.RAINY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }
                timezone = 1;
            }
            else if (currentTime.Hour > 6 && currentTime.Hour <= 12 && timezone != 2)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.RAINY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }
                timezone = 2;
            }
            else if (currentTime.Hour > 0 && currentTime.Hour <= 6 && timezone != 3)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.RAINY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }
                timezone = 3;
            }
        }
        else
        {
            if (currentTime.Hour > 18 && currentTime.Hour <= 24 && timezone != 0)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SNOWY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }

                timezone = 0;
            }
            else if (currentTime.Hour > 12 && currentTime.Hour <= 18 && timezone != 1)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SNOWY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }
                timezone = 1;
            }
            else if (currentTime.Hour > 6 && currentTime.Hour <= 12 && timezone != 2)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SNOWY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }
                timezone = 2;
            }
            else if (currentTime.Hour > 0 && currentTime.Hour <= 6 && timezone != 3)
            {
                if (UnityEngine.Random.Range(0, 100) < 90)
                {
                    CurrentWeather = Weather.SNOWY;
                }
                else
                {
                    CurrentWeather = Weather.SUNNY;
                }
                timezone = 3;
            }
        }


        if(CurrentWeather==Weather.RAINY)
        {
            minintensity = rainyminintensity;
            maxintensity = rainymaxintensity;
            sunLight.shadows = LightShadows.None;
            sunLight.intensity /= 2;
            SnowAmmount -= Time.deltaTime/ 36000 * timeMultiplier;
            if(!isinroom)
            {

            }
            Rain.gameObject.SetActive(true);
            Snow.gameObject.SetActive(false);
        }
        else if(CurrentWeather == Weather.SUNNY)
        {
            sunLight.shadows = LightShadows.Hard;
            SnowAmmount -= Time.deltaTime/ 36000 * timeMultiplier;
            Rain.gameObject.SetActive(false);
            Snow.gameObject.SetActive(false);
        }
        else
        {
            sunLight.shadows = LightShadows.Hard;
            SnowAmmount += Time.deltaTime/36000*timeMultiplier;
            Rain.gameObject.SetActive(false);
            Snow.gameObject.SetActive(true);
        }

        if (isinroom)
        {
            Rain.gameObject.SetActive(false);
            Snow.gameObject.SetActive(false);
        }
        
        SnowAmmount = Mathf.Clamp(SnowAmmount, 0, 2);
        snowMat.SetFloat("_Intensity",SnowAmmount);
        if (!isinroom)
            RenderSettings.ambientIntensity = Mathf.Clamp(sunLight.intensity + moonLight.intensity, minintensity, maxintensity);
        else
            RenderSettings.ambientIntensity = 1;
        if (RenderSettings.ambientIntensity < 0.25f && !isinroom)
            light.SetActive(true);
        else
            light.SetActive(false);
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }

    public void LightOn(bool on)
    {
        isinroom = on;
    }
}