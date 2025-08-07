using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Ground & Jumping")]
    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 1;
    [SerializeField] protected Vector3 yVelocty;
    [SerializeField] protected float groundedYVelocity = -20;
    [SerializeField] protected float fallStartYVelocity = -5;
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTImer = 0;

    [Header("Flags")]
    public bool isRolling = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool isGrounded = true;
    public bool canRun = true;
    public bool canRoll = true;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();

        if (character.characterLocomotionManager.isGrounded)
        {
            //  Not attempting to jump or move upward
            if (yVelocty.y < 0)
            {
                inAirTImer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocty.y = groundedYVelocity;
            }
        }
        else
        {
            if (!character.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocty.y = fallStartYVelocity;
            }

            inAirTImer += Time.deltaTime;

            character.animator.SetFloat("inAirTimer", inAirTImer);

            yVelocty.y += gravityForce * Time.deltaTime;
        }

        character.characterController.Move(yVelocty * Time.deltaTime);

    }

    protected void HandleGroundCheck()
    {

        //if ( character is AIBossCharacterManager)
        //    isGrounded = true;
        //else
            character.characterLocomotionManager.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);

    }

    //    protected void OnDrawGizmos()
    //    {
    //        Gizmos.DrawSphere(character.transform.position,groundCheckSphereRadius);    
    //    }

    public void EnableCanRoate()
    {
        canRotate = true;
    }

    public void DisableCanRoate()
    {
        canRotate = false;
    }
}
