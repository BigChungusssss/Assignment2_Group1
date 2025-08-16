using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    private PlayerController player;
    public GameObject cards;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Power Shot when at least 1 card
        // if (player.controls.Player.PowerShot.WasPressedThisFrame() &&
        //     !player.isTransformed && !player.isShrunk &&
        //     player.currentCards > 0)
        // {
        //     player.FirePowerShot();
        // }

        // Rocket Transform when exactly full cards
        if (player.controls.Player.RocketTransform.WasPressedThisFrame() &&
            !player.isShrunk &&
            player.currentCards == player.maxCards)
        {
            player.TransformPlayer();
            cards.SetActive(false);
        }
    }

    
}
