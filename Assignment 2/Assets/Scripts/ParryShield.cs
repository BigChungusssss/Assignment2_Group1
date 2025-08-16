using UnityEngine;

public class ParryShield : MonoBehaviour
{
    public PlayerController player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        gameObject.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Parry"))
        {
            player.SuccessfulParry(other.gameObject);
        }
    }
}
