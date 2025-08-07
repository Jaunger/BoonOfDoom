using System.Collections.Generic;
using UnityEngine;

public class EventTriggerWakeNearbyCharacters : MonoBehaviour
{
    [SerializeField] float awakenRadius = 8;

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = Object.FindFirstObjectByType<PlayerManager>();
        if (player == null)
        {
            Debug.LogError("PlayerManager not found in the scene.");
            return;
        }

        //todo check for friendly

        //todo check for sneaky/hidden

        Collider[] creaturesInRadius = Physics.OverlapSphere(transform.position, awakenRadius, WorldUtilityManager.instance.GetCharacterLayers());
        List<AICharacterManager> awakeCreatures = new();

        for (int i = 0; i < creaturesInRadius.Length; i++)
        {
            AICharacterManager aiCharacterManager = creaturesInRadius[i].GetComponentInParent<AICharacterManager>();

            if (aiCharacterManager == null)
                continue;

            if (aiCharacterManager.isDead)
                continue;

            if (aiCharacterManager.isAwake)
                continue;

            if (!awakeCreatures.Contains(aiCharacterManager))
                awakeCreatures.Add(aiCharacterManager);
        }


        for (int i = 0; i < awakeCreatures.Count; i++)
        {
            awakeCreatures[i].aiCharacterCombatManager.SetTarget(player);
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, awakenRadius);
    }
}
