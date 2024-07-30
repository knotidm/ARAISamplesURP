using UnityEngine;

public class Alarm : MonoBehaviour
{
    [SerializeField] private AudioSource alarmAudioSource;

    public Light mainLight;

    // alarm state
    private enum STATE { NORMAL, ALARM };

    private STATE state = STATE.NORMAL;
    private float startTimeOfState = 0;
    private float alarmPeriod = 2f; // number of seconds for alarm
    private Color defaultLightColor;
    private ControlPanel currentPanel;

    // Start is called before the first frame update
    private void Start()
    {
        defaultLightColor = mainLight.color;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == STATE.ALARM)
        {
            float t = Time.time - startTimeOfState;

            if (t < alarmPeriod)
            {
                AnimateAlarm(t);
            }
            else
            {
                StopAlarm();
            }
        }
    }

    public void StartAlarm(ControlPanel panel)
    {
        currentPanel = panel;
        state = STATE.ALARM;
        alarmAudioSource.Play();
        startTimeOfState = Time.time;
    }

    private void AnimateAlarm(float t)
    {
        Color lightColor = new Color(Mathf.Pow(Mathf.Cos(t * Mathf.PI * 4), 2), 0, 0);

        for (int i = 0; i < currentPanel.lights.Length; i++)
        {
            currentPanel.lights[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", lightColor);
        }

        mainLight.color = lightColor;
    }

    private void StopAlarm()
    {
        alarmAudioSource.Stop();
        state = STATE.NORMAL;
        mainLight.color = defaultLightColor;

        for (int i = 0; i < currentPanel.lights.Length; i++)
        {
            currentPanel.lights[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }
}
