using UnityEngine;
using UnityEngine.XR.AR;

/*
 *  This class controls your player. It deals with user inputs such as mouse and keyboard.
 *  It also controls the game state.
 */

public class Game : SingletonMonoBehaviour<Game>
{
    public Alarm alarm;
    public ARAnchorCreator arAnchorCreator;

    public AudioClip[] numbers;
    public Texture2D[] digits;
    public Camera playerCamera;
    public Texture2D circleCursor;

    private Room currentRoom;
    private Panel currentPanel;

    private void Start()
    {
        arAnchorCreator.OnFirstAnchorPrefabCreated += SetRoom;
        arAnchorCreator.OnSecondAnchorPrefabCreated += SetPanel;
    }

    protected override void OnDestroy()
    {
        arAnchorCreator.OnFirstAnchorPrefabCreated -= SetRoom;
        arAnchorCreator.OnSecondAnchorPrefabCreated -= SetPanel;
    }
    private void SetRoom(GameObject gameObject)
    {
        currentRoom = gameObject.GetComponent<Room>();
    }

    private void SetPanel(GameObject gameObject)
    {
        currentPanel = gameObject.GetComponent<Panel>();
        currentPanel.callback = GotNumber;
        currentRoom.ResetCode(currentPanel);
    }

    //This is called from the panel once a digit has been entered. It gives the predicted number and the probability:
    private void GotNumber(Room room, int n, float probability)
    {
        GetComponent<AudioSource>().PlayOneShot(numbers[n]);
        Debug.Log("Predicted number " + n + "\nProbability " + (probability * 100) + "%");

        //now we need to check if this code is correct:
        (bool correct, bool completed) = room.CheckCode(n);
        if (!correct)
        {
            //The guess is not correct so sound the alarm:
            currentRoom = room;
            Invoke(nameof(SoundAlarm), 0.5f);
        }
        if (completed)
        {
            if (room.doorState == Room.DOOR_STATE.CLOSED)
            {
                currentRoom = room;
                Invoke(nameof(PlayMessage), 1f);
            }
            room.OpenDoor();
        }
    }

    //Sound the alarm and reset the code to something else:
    private void SoundAlarm()
    {
        alarm.StartAlarm(currentPanel);
        currentRoom.ResetCode(currentPanel);
    }

    private void PlayMessage()
    {
        if (currentRoom.message != null)
        {
            GetComponent<AudioSource>().PlayOneShot(currentRoom.message);
        }
    }
}
