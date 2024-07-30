using UnityEngine;

public class Door : SingletonMonoBehaviour<Door>
{
    public GameObject door, door2;
    public AudioClip message;

    public enum DOOR_STATE { OPEN, CLOSED, OPENING, CLOSING };
    public DOOR_STATE doorState = DOOR_STATE.CLOSED;

    // door state (position/opening time/ width)
    private float startTimeOfDoorState = 0;
    private Vector3 doorClosedPosition;
    private Vector3 door2ClosedPosition;
    private float timeOfDoorOpening = 0.5f;
    private float doorWidth = 4f;

    private void Start()
    {
        if (door) doorClosedPosition = door.transform.position;
        if (door2) door2ClosedPosition = door2.transform.position;
    }

    public void OpenDoor()
    {
        if (!door) return;
        if (doorState == DOOR_STATE.CLOSED)
        {
            doorState = DOOR_STATE.OPENING;
            startTimeOfDoorState = Time.time;
            door.GetComponent<AudioSource>().Play();
        }
        if (doorState == DOOR_STATE.OPEN)
        {
            doorState = DOOR_STATE.CLOSING;
            startTimeOfDoorState = Time.time;
            door.GetComponent<AudioSource>().Play();
        }
    }

    private void Update()
    {
        float time = (Time.time - startTimeOfDoorState) / timeOfDoorOpening;

        if (doorState == DOOR_STATE.OPENING)
        {
            OpeningDoor(time);
        }
        if (doorState == DOOR_STATE.CLOSING)
        {
            ClosingDoor(time);
        }
    }

    private void OpeningDoor(float time)
    {
        if (!door && !door2) return;

        door.transform.position = doorClosedPosition + door.transform.right * Mathf.Min(1, time) * doorWidth;
        door2.transform.position = door2ClosedPosition - door2.transform.right * Mathf.Min(1, time) * doorWidth;

        if (time > 1)
        {
            doorState = DOOR_STATE.OPEN;
        }
    }

    private void ClosingDoor(float time)
    {
        if (!door || !door2) return;

        door.transform.position = doorClosedPosition + door.transform.right * Mathf.Max(0, 1 - time) * doorWidth;
        door2.transform.position = doorClosedPosition - door2.transform.right * Mathf.Max(0, 1 - time) * doorWidth;

        if (time > 1)
        {
            doorState = DOOR_STATE.CLOSED;
        }
    }
}
