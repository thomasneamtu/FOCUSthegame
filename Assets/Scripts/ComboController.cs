using UnityEngine;

public class ComboController : MonoBehaviour
{
    [SerializeField] Animator charAnim;
    private string[] comboSequence = new string[] { "H", "J", "K", "L" };
    private int comboIndex = 0;
    private float inputTimer = 0f;
    public float maxInputDelay = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputTimer = Time.deltaTime;

        if(inputTimer > maxInputDelay)
        {
            comboIndex = 0;
            inputTimer = 0f;
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            CheckCombo("H");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            CheckCombo("J");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            CheckCombo("K");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CheckCombo("L");
        }
    }

    void CheckCombo(string key)
    {
        if(comboSequence[comboIndex] == key)
        {
            inputTimer = 0f;
            comboIndex++;

            charAnim.SetTrigger($"Move{comboIndex}");

            if(comboIndex >= comboSequence.Length)
            {
                Debug.Log("COMBO SUCCESS!");
                comboIndex = 0; 
            }
            else
            {
                Debug.Log("Wrong Input, Combo Reset");
                comboIndex = 0;
                inputTimer = 0f;
            }
        }    
    }
}
