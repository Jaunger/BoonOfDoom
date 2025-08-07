using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Loading Screen")]
    public bool isPerformingLoadingOperation = false;

    [Header("DEBUG MENU")]
    //[SerializeField] bool despawnCharacters = false; //  TODO: delete later

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;
    private Coroutine spawnAllCharacters;
    private Coroutine despawnAllCharacters;
    private Coroutine resetAllCharacters;

    [Header("Beacon Prefab")]
    public GameObject beaconPrefab;

    [Header("Bosses")]
    [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

    [Header("Patrol Paths")]
    [SerializeField] List<AIPatrolPath> aIPatrolPaths = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public void AddCharacterToSpawnedCharacterList(AICharacterManager aICharacter)
    {
        if (spawnedInCharacters.Contains(aICharacter))
            return;

        if (aICharacter != null)
        {
            spawnedInCharacters.Add(aICharacter);

            AIBossCharacterManager bossCharacterManager = aICharacter as AIBossCharacterManager;

            if (bossCharacterManager != null)
            {
                if (spawnedInBosses.Contains(bossCharacterManager))
                    return;

                spawnedInBosses.Add(bossCharacterManager);
            }
        }
    }

    public AIBossCharacterManager GetBossCharacterByID(int bossID)
    {
        return spawnedInBosses.FirstOrDefault(b => b.bossID == bossID);
    }

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        aiCharacterSpawners.Add(aiCharacterSpawner);
        aiCharacterSpawner.AttemptToSpawnCharacter();
    }

    public void SpawnAllCharacters()
    {
        isPerformingLoadingOperation = true;

        if (SpawnAllCharactersCoroutine() != null)
            StopCoroutine(SpawnAllCharactersCoroutine());

        spawnAllCharacters = StartCoroutine(SpawnAllCharactersCoroutine());
    }

    private IEnumerator SpawnAllCharactersCoroutine()
    {
        for (int i = 0; aiCharacterSpawners.Count > i; i++)
        {
            yield return new WaitForFixedUpdate();

            aiCharacterSpawners[i].AttemptToSpawnCharacter();

            yield return null;
        }

        isPerformingLoadingOperation = false;


        yield return null;
    }

    public void ResetAllCharacters()
    {
        isPerformingLoadingOperation = true;

        if (ResetAllCharactersCoroutine() != null)
            StopCoroutine(ResetAllCharactersCoroutine());

        resetAllCharacters = StartCoroutine(ResetAllCharactersCoroutine());
    }

    private IEnumerator ResetAllCharactersCoroutine()
    {
        for (int i = 0; aiCharacterSpawners.Count > i; i++)
        {
            yield return new WaitForFixedUpdate();

            aiCharacterSpawners[i].ResetCharacter();

            yield return null;
        }

        isPerformingLoadingOperation = false;

        //for (int i = 0; aiCharacterSpawners.Count > i; i++)
        //{
        //    if (aiCharacterSpawners[i].instantiateGameObject != null)
        //    {
        //        spawnedInCharacters.Add(aiCharacterSpawners[i].instantiateGameObject);
        //    }

        //    yield return null;
        //}

        yield return null;
    }

    private void DespawnAllCharacters()
    {
        isPerformingLoadingOperation = true;

        if (DespawnAllCharactersCoroutine() != null)
            StopCoroutine(DespawnAllCharactersCoroutine());

        despawnAllCharacters = StartCoroutine(DespawnAllCharactersCoroutine());

    }

    private IEnumerator DespawnAllCharactersCoroutine()
    {
        for (int i = 0; spawnedInCharacters.Count > i; i++)
        {
            yield return new WaitForFixedUpdate();

            Destroy(spawnedInCharacters[i]);

            yield return null;
        }

        spawnedInCharacters.Clear();
        isPerformingLoadingOperation = false;

        yield return null;
    }

    private void DisableAllCharacters()
    {

    }

    public void DisableAllBossFights()
    {
        for (int i = 0; spawnedInBosses.Count > i; i++)
        {
            if (spawnedInBosses[i] == null)
                continue;

            spawnedInBosses[i].bossFightIsActive = false;
        }
    }

    public void AddPatrolPathToList(AIPatrolPath aIPatrolPath)
    {
        if (aIPatrolPaths.Contains(aIPatrolPath))
            return;

        aIPatrolPaths.Add(aIPatrolPath);
    }

    public AIPatrolPath GetAIPatrolPath(int patrolPathID)
    {
        AIPatrolPath aIPatrolPath = null;
        for (int i = 0; i < aIPatrolPaths.Count; i++)
        {
            if (aIPatrolPaths[i].patrolPathID == patrolPathID)
            {
                aIPatrolPath = aIPatrolPaths[i];
            }
        }
        return aIPatrolPath;
    }
}
