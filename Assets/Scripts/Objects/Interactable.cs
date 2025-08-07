using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactText;
    [SerializeField] protected Collider interactCollider;

    protected virtual void Awake()
    {
        if (interactCollider == null)
            interactCollider = GetComponent<Collider>();

    }

    protected virtual void Start()
    {

    }

    public virtual void Interact(PlayerManager player)
    {

        interactCollider.enabled = false;
        player.playerInteractionManager.RemoveInteractable(this);
        PlayerUIManager.instance.popUpManager.CloseAllPopUpWindows();

    }

    public virtual void ReEnableCollider()
    {
        interactCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            if (player.isDead)
                return;

            player.playerInteractionManager.AddInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            player.playerInteractionManager.RemoveInteractable(this);


            PlayerUIManager.instance.popUpManager.CloseAllPopUpWindows();
        }
    }
}
