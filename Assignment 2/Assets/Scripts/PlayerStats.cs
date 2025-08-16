using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public Text text;

    public GameObject temporaryUIText;   // For level up
    public TextMeshProUGUI healthText;  // For health increase
    public GameObject temporaryUIText3;  // For attack power increase

    public Gun currentGun; // Assign this from wherever you equip the gun


    public float displayTime = 5f;

    public float level = 1;
    public float attackPower = 10f;
    public float Health = 1000f;

    public float GetBaseAttackPower()
    {
        return attackPower;
    }



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        Debug.Log("Attack Power: " + attackPower);
        Debug.Log("Health Power: " + Health);
        UpdateHealth();
    }

    public void ApplyPowerUp(string type)
    {
        switch (type)
        {
            case "Level":
                IncreaseLevel();
                break;

            default:
                Debug.LogWarning("Unknown power-up type: " + type);
                break;
        }
    }

    private IEnumerator ShowTemporaryText(GameObject textObject)
    {
        textObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        textObject.SetActive(false);
    }

    public void IncreaseLevel()
    {
        level++;
        UpdateAttackPower();
        
        UpdateLevelDisplay();

        if (temporaryUIText != null)
        {
            StartCoroutine(ShowTemporaryText(temporaryUIText));
        }
    }

    public void UpdateLevelDisplay()
    {
        if (text != null)
        {
            text.text = level.ToString();
        }
    }

    public void UpdateAttackPower()
    {
        attackPower += 10;

        if (temporaryUIText3 != null)
        {
            StartCoroutine(ShowTemporaryText(temporaryUIText3));
        }
    }

    public void UpdateHealth()
    {
        // Ensure Health doesn't go below 0
        Health = Mathf.Max(Health, 0f);

        // Update the TextMeshProUGUI display
        if (healthText != null)
            healthText.text = "HP:"+ Mathf.CeilToInt(Health); // Show as integer
    }



    
    
}

