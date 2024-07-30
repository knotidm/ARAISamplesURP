using System.Linq;
using UnityEngine;
using UnityEngine.XR.AR;

public class Game : SingletonMonoBehaviour<Game>
{
    public Alarm alarm;
    public ARAnchorCreator arAnchorCreator;

    public AudioClip[] numbers;
    public Texture2D[] digits;

    private Door door;
    private ControlPanel controlPanel;

    public bool fruitCodes = false; // specific code for fruit level

    public int[] code; // secret code
    public int codePosition = 0; //e.g. is it the first, second or third digit you are entering

    private int[] numbersArray = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private int[] fruitNumbers = new int[] { 1, 2, 3, 4, 5, 6 };

    private void Start()
    {
        arAnchorCreator.OnFirstAnchorPrefabCreated += SetDoor;
        arAnchorCreator.OnSecondAnchorPrefabCreated += SetControlPanel;
    }

    protected override void OnDestroy()
    {
        arAnchorCreator.OnFirstAnchorPrefabCreated -= SetDoor;
        arAnchorCreator.OnSecondAnchorPrefabCreated -= SetControlPanel;
    }

    private void SetDoor(GameObject gameObject)
    {
        door = gameObject.GetComponent<Door>();
    }

    private void SetControlPanel(GameObject gameObject)
    {
        controlPanel = gameObject.GetComponent<ControlPanel>();
        controlPanel.callback = GotNumber;
        ResetCode(controlPanel);
    }

    // Check if the code is correct and if we have completed the full code
    public (bool correct, bool completed) CheckCode(int digitGuess)
    {
        bool isCorrectGuess = digitGuess == code[codePosition];

        if (isCorrectGuess)
        {
            // turn lights green
            for (int i = 0; i < controlPanel.lights.Length; i++)
            {
                var lightMaterial = controlPanel.lights[i].GetComponent<Renderer>().material;
                lightMaterial.SetColor("_EmissionColor", i <= codePosition ? Color.green : Color.black);
            }

            codePosition++;
            bool everyDigitCorrect = codePosition == code.Length; // every digit correct

            if (everyDigitCorrect)
            {
                codePosition = 0;
                return (correct: true, completed: true);
            }
            else
            {
                return (correct: true, completed: false);
            }
        }
        else //wrong guess
        {
            // turn off lights
            for (int i = 0; i < controlPanel.lights.Length; i++)
            {
                var lightMaterial = controlPanel.lights[i].GetComponent<Renderer>().material;
                lightMaterial.SetColor("_EmissionColor", Color.black);
            }

            codePosition = 0;
            return (correct: false, completed: false);
        }
    }


    // Reset code randomly and set the clue images in the room
    public void ResetCode(ControlPanel panel)
    {
        controlPanel = panel;
        code = new int[] { 0, 0, 0 };

        int[] randomNumbers = GetRandomizedNumbers();

        // Assign the first three random numbers in our randomized array to the three codes and set the clue materials
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = randomNumbers[i];

            if (i >= controlPanel.clue.Length)
                return;

            var clueMaterial = controlPanel.clue[i].GetComponent<Renderer>().material;
            if (fruitCodes)
            {
                // changes the image of the fruit clue
                clueMaterial.mainTextureOffset = new Vector2((code[i] - 1) / 6f, 0);
            }
            else
            {
                clueMaterial.mainTexture = digits[code[i]];
            }
        }
    }

    private int[] GetRandomizedNumbers()
    {
        if (fruitCodes)
        {
            return fruitNumbers.OrderBy((x) => Random.Range(0, 1f)).ToArray();
        }
        else
        {
            return numbersArray.OrderBy((x) => Random.Range(0, 1f)).ToArray();
        }
    }

    //This is called from the panel once a digit has been entered. It gives the predicted number and the probability:
    private void GotNumber(Door room, int number, float probability)
    {
        GetComponent<AudioSource>().PlayOneShot(numbers[number]);
        Debug.Log("Predicted number " + number + "\nProbability " + (probability * 100) + "%");

        //now we need to check if this code is correct:
        (bool correct, bool completed) = CheckCode(number);

        if (!correct)
        {
            //The guess is not correct so sound the alarm:
            door = room;
            Invoke(nameof(SoundAlarm), 0.5f);
        }
        if (completed)
        {
            if (room.doorState == Door.DOOR_STATE.CLOSED)
            {
                door = room;
                Invoke(nameof(PlayMessage), 1f);
            }
            room.OpenDoor();
        }
    }

    //Sound the alarm and reset the code to something else:
    private void SoundAlarm()
    {
        alarm.StartAlarm(controlPanel);
        ResetCode(controlPanel);
    }

    private void PlayMessage()
    {
        if (door.message != null)
        {
            GetComponent<AudioSource>().PlayOneShot(door.message);
        }
    }
}
