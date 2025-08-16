using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Popup Text")]
    public PopupTextController popupTextController;

    public static bool GameIsOver = false;

    [Header("Health UI")]
    public GameObject healthUI;
    public Image heartTemplate;

    [Header("Attack UI")]
    public GameObject attackUI;
    public Image attackTemplate;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    private List<Image> hearts = new List<Image>();
    private List<Image> attacks = new List<Image>();

    private float lastHealth;
    private float lastAttack;

    void Start()
    {
        heartTemplate.gameObject.SetActive(false);
        attackTemplate.gameObject.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);

        if (PlayerStats.Instance != null)
        {
            lastHealth = PlayerStats.Instance.Health;
            lastAttack = PlayerStats.Instance.attackPower;
            DrawHealth((int)lastHealth);
            DrawAttack((int)lastAttack);
        }
    }

    void Update()
    {
        if (PlayerStats.Instance == null) return;

        float currentHealth = PlayerStats.Instance.Health;
        float currentAttack = PlayerStats.Instance.attackPower;

        if (currentHealth != lastHealth)
        {
            DrawHealth((int)currentHealth);
            lastHealth = currentHealth;

            if (popupTextController != null)
                popupTextController.ShowText("DAMAGE!", Color.red);

            if (currentHealth <= 0f)
            {
                ShowGameOver();
            }
        }

        if (currentAttack != lastAttack)
        {
            DrawAttack((int)currentAttack);
            lastAttack = currentAttack;

            if (popupTextController != null)
                popupTextController.ShowText("ATTACK UP!", Color.yellow);
        }
    }

    void DrawHealth(int health)
    {
        foreach (var h in hearts) Destroy(h.gameObject);
        hearts.Clear();

        for (int i = 0; i < health; i++)
        {
            Image newHeart = Instantiate(heartTemplate, healthUI.transform);
            newHeart.gameObject.SetActive(true);
            hearts.Add(newHeart);
        }
    }

    void DrawAttack(int attack)
    {
        foreach (var a in attacks) Destroy(a.gameObject);
        attacks.Clear();

        for (int i = 0; i < attack; i++)
        {
            Image newAtk = Instantiate(attackTemplate, attackUI.transform);
            newAtk.gameObject.SetActive(true);
            attacks.Add(newAtk);
        }
    }

    void ShowGameOver()
    {
        GameIsOver = true; // <--- add this
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }
}
