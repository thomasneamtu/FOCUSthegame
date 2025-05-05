using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FocusMode : MonoBehaviour
{
    [SerializeField] private float focusBuildRate = 20f;
    [SerializeField] private float focusMeter = 0f;
    [SerializeField] private float focusDuration = 10f;
    [SerializeField] private PlayerCombat playerHealth;

    [Header("UI Settings")]
    [SerializeField] private Image focusRadialImage;
    [SerializeField] private TMP_Text focusText;
    [SerializeField] private Image keycodeImage;
    [SerializeField] private GameObject focusEffect;
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
        if (focusMeter < 100f)
        {
            focusMeter += focusBuildRate * Time.deltaTime;

            if (focusMeter >= 100f)
            {
                focusText.color = Color.white; // glow effect?
                keycodeImage.color = Color.white;
            }

            if (focusMeter >= 100f)
            {
                focusMeter = 100f;
                isFocusReady = true;
                Debug.Log("FOCUS. Ready!");
            }

            UpdateFocusUI();

        }
    }


    private void UpdateFocusUI()
    {
        float fill = Mathf.Clamp01(focusMeter / 100f);
        focusRadialImage.fillAmount = fill;
    }

    private void FocusActivate()
    {

        if(isFocusReady &&  Input.GetKey(KeyCode.Y))
        {
            isFocusActive = true;
            isFocusReady = false;
            focusTimer = focusDuration;

            focusRadialImage.fillAmount = 0f;
            focusText.color = Color.gray;
            keycodeImage.color = Color.gray;
            focusEffect.SetActive(true);

            playerHealth.health = 5;
            Debug.Log("FOCUS. Activated!");

        }
    }

    private void FocusDeactivate()
    {
        isFocusActive = false;
        focusMeter = 0f;
        isFocusReady = false;

        focusEffect.SetActive(false);

       

        Debug.Log("FOCUS. Lost!");
    }

    private void FocusDurationCountdown()
    {
        focusTimer -= Time.deltaTime;

        // could have UI meter respond here, giving clear indicator that focusmode is done.

        if (focusTimer <= 0f)
        {
            FocusDeactivate();
        }
    }

}
