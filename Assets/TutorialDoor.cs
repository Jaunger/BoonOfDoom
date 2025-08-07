using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;  
    private bool isOpen = false;

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        animator.SetTrigger("Open");            
        GetComponent<Collider>().enabled = false; 
    }
}
