using UnityEngine;

public class FocusMode : MonoBehaviour
{
    [SerializeField] private float focusBuildRate = 20f;
    [SerializeField] private float focusMeter = 0f;
    [SerializeField] private float focusDuration = 10f;

    public bool isFocusActive { get; private set; } = false;    
    public bool isFocusReady = false;
    private float focusTimer = 0f;

    private PlayerCombat playerCombat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFocusActive)
        {
            BuildFocusMeter();
            FocusActivate();
        }
        else
        {
            FocusDurationCountdown();
        }
    }

   private void BuildFocusMeter()
    {
        if(focusMeter < 100f)
        {
            focusMeter += focusBuildRate * Time.deltaTime;

            if(focusMeter >= 100f)
            {
                focusMeter = 100f;
                isFocusReady = true;
                Debug.Log("FOCUS. Ready!");

                //Animation on the UI Meter
                 
            }
        }
    }


    private void FocusActivate()
    {

        if(isFocusReady &&  Input.GetKey(KeyCode.Y))
        {
            isFocusActive = true;
            isFocusReady = false;
            focusTimer = focusDuration;

            Debug.Log("FOCUS. Activated!");

        }
    }

    private void FocusDeactivate()
    {
        isFocusActive = false;
        focusMeter = 0f;

        Debug.Log("FOCUS. Lost!");
    }

    private void FocusDurationCountdown()
    {
        focusTimer -= Time.deltaTime;

        if (focusTimer <= 0f)
        {
            FocusDeactivate();
        }
    }

}
