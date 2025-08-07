using UnityEngine;

public class AIFrankCharacterManager : AIBossCharacterManager
{
   [HideInInspector] public AIFrankSoundFXManager frankSoundFXManager;


    protected override void Awake()
    {
        base.Awake();
        frankSoundFXManager = GetComponent<AIFrankSoundFXManager>();
    }

    

}
