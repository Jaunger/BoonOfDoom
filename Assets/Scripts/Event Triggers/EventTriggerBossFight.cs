using UnityEngine;

public class EventTriggerBossFight : MonoBehaviour
{
    [SerializeField] int bossID;

    private void OnTriggerEnter(Collider other)
    {
        AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(bossID);

        if (boss != null)
        {
            if (boss.hasBeenDefeated)
                return; // Boss is already defeated, do nothing

            boss.WakeBoss();
            this.gameObject.SetActive(false);
        }
    }
}
