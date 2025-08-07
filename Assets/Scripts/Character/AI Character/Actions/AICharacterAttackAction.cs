using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Actions/Attack Action")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] public string attackAnimation; 

    [Header("Combat Action")]
    public AICharacterAttackAction comboAction;

    [Header("Combat Action")]
    [SerializeField] AttackType attackType;
    public int attackWeight = 50;
    // attack can be repeated 
    public float actionRecoveryTime = 1.5f;
    public float maxAttackAngle = -35f;
    public float minAttackAngle = 35f;
    public float maxAttackDistance = 2f;
    public float minAttackDistance = 0f;

    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.aiCharacterAnimationManager.PlayerTargetActionAnimation(attackAnimation, true);
    }
}
