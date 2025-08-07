using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;
    public Transform cameraPivotTransform;
    public float cameraPivotYOffeset = 1.5f;

    [Header("Camera Settingss")]
    [SerializeField] private float cameraSmoothSpeed = 1;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30; //Lowest point to look down
    [SerializeField] float maximumPivot = 60; //max point to look up
    [SerializeField] float cameraCollisionRadius = 0.2f; //max point to look up
    [SerializeField] LayerMask collideWithLayers; //max point to look up

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZposition;
    private float targetCameraZPosition;

    [Header("Lock On")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minViewableAngle = -50;
    [SerializeField] float maxViewableAngle = 50;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] float lockedCameraSpeed = 1f;
    [SerializeField] float unlockedCameraHeight = 1.6f;
    [SerializeField] float lockedCameraHeight = 2.0f;
    private Coroutine cameraLockOnHeightCoroutine;
    private List<CharacterManager> availableTargets = new();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;

    [Header("Ranged Aim")]
    private Transform targetTransformWhileAiming;
    public Vector3 aimDirection;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZposition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollision();
        }
    }

    private void HandleFollowTarget()
    {
        if (player.isAiming)
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.playerCombatManager.lockOnTransform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }
        else
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }
    
    }

    private void HandleRotations()
    {
        if (player.isAiming)
        {
            HandleAimRotations();
        }
        else
        {
            HandleStandardRotations();
        }
    }

    private void HandleAimRotations()
    {
        if (!player.playerLocomitionManager.isGrounded)
            player.isAiming = false;

        if (player.isPerformingAction)
            return;

        aimDirection = cameraObject.transform.forward.normalized;

        Vector3 cameraRotationY = Vector3.zero;
        Vector3 cameraRotationX = Vector3.zero;

        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        cameraRotationY.y = leftAndRightLookAngle;
        Debug.Log($"Left and Right Look Angle: {leftAndRightLookAngle}");
        cameraRotationX.x = upAndDownLookAngle;

        cameraObject.transform.eulerAngles = new Vector3(upAndDownLookAngle, leftAndRightLookAngle, 0);

    }

    private void HandleStandardRotations()
    {
        // if locked on -> force rotation toward target
        if (player.isLockedOn)
        {
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        // rotate regulary
        else
        {
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            //  ROTATE left and right 
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // ROTATE up and down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }

    private void HandleCollision()
    {
        targetCameraZPosition = cameraZposition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        //  CHECK IF OBJECT IN FRONT OF CAMERA 
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        if (player.isAiming)
        {
            cameraObjectPosition.z = 0;
            cameraObject.transform.localPosition = cameraObjectPosition;
            return;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    public void HandleLocatingLockOnTargets()
    {
        float shortDistance = Mathf.Infinity;
        float shortDistanceOfRightTarget = Mathf.Infinity;
        float shortDistanceOfLeftTarget = -Mathf.Infinity;
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                if (lockOnTarget.isDead)
                    continue;

                if (lockOnTarget.transform.root == player.transform.root)
                    continue;

                if (distanceFromTarget > lockOnRadius)
                    continue;

                if (viewableAngle > minViewableAngle && viewableAngle < maxViewableAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(player.characterCombatManager.lockOnTransform.position,
                        lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnvLayers()))
                    {
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);
                    }
                }

            }
        }

        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortDistance)
                {
                    shortDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if (player.isLockedOn)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[k] == player.playerCombatManager.currentTarget)
                        continue;

                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortDistanceOfLeftTarget)
                    {
                        shortDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    }
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortDistanceOfRightTarget)
                    {
                        shortDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[k];
                    }
                }
            }
            else
            {
                ClearLockOnTargets();
                player.isLockedOn = true;
            }
        }
    }

    public void SetLockedCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
            yield return null;

        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.isLockedOn = true;
        }

        yield return null;
    }

    public IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, lockedCameraSpeed);
                    cameraPivotTransform.transform.localRotation =
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, lockedCameraSpeed);
                    cameraPivotTransform.transform.localRotation =
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
            }

            yield return null;
        }

        if (player != null)
        {
            if (player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.SetLocalPositionAndRotation(newLockedCameraHeight, Quaternion.Euler(0, 0, 0));
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }
}
