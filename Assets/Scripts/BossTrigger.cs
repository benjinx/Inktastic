using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public Boss boss;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            boss.StartBossCombat();
        }
    }
}
