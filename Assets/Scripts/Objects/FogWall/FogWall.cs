using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FogWall : Interactable
{
    [Header("Fog Wall")]
    [SerializeField] GameObject[] fogWallObjects;

    [Header("Fog Wall Collider")]
    [SerializeField] Collider fogWallCollider;

    [Header("ID")]
    public int fogWallID;
    public int bossId;

    [Header("Sound")]
    private AudioSource fogWallAudioSource;
    [SerializeField] AudioClip fogWallSound;

    [Header("Active")]
    [SerializeField] private bool _isActive;

  
    public bool isActive
    {
        get { return _isActive; }
        set
        {
            gameObject.SetActive(value);
            _isActive = value;
            if (value)
            {
                foreach (GameObject fogWall in fogWallObjects)
                {
                    fogWall.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject fogWall in fogWallObjects)
                {
                    fogWall.SetActive(false);
                }
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        fogWallAudioSource = gameObject.GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        isActive = false;
        WorldObjectManager.instance.AddFogWallToList(this);
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        Quaternion targetRot = Quaternion.Euler(0,90, 0);
        player.transform.rotation = targetRot;

        AllowPlayerThroughFogWallColliders(player);

        player.playerAnimatorManager.PlayerTargetActionAnimation("Pass_Throught_Fog_01", true);
        AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(bossId);

        if (boss != null)
            boss.WakeBoss();

        ReEnableCollider();
    }

    private void AllowPlayerThroughFogWallColliders(PlayerManager player)
    {
        if (fogWallSound != null)
            fogWallAudioSource.PlayOneShot(fogWallSound);

        if (player != null)
            StartCoroutine(disableCollisionForTime(player));

        
    }

    private IEnumerator disableCollisionForTime(PlayerManager player)
    {
        Physics.IgnoreCollision(player.characterController, fogWallCollider, true);

        yield return new WaitForSeconds(3);

        Physics.IgnoreCollision(player.characterController, fogWallCollider, false);
    }
}
