using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public Boss boss;

    public HUD hud;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            boss.StartBossCombat();
            hud.EnableBossUI();
        }
    }
}
