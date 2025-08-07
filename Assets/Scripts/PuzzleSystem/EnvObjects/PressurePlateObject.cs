using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    public float requiredMass = 40f;                 // 0 for HeavyImpact-only
    public PlatePuzzleController controller;         // assign in Inspector

    private bool pressed = false;

    /* Weight detection */
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody &&
            other.attachedRigidbody.mass >= requiredMass)
        {
            SetPressed(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody &&
            other.attachedRigidbody.mass >= requiredMass)
        {
            SetPressed(false);
        }
    }

    /* HeavyImpact slam */
    public void OnDamageReceived(TakeDamageEffect hit)
    {
        if (requiredMass == 0f && hit.weaponElement == WeaponElement.HeavyImpact)
            SetPressed(true);
    }

    private void SetPressed(bool state)
    {
        if (pressed == state) return;          // no change
        pressed = state;

        if (controller != null)
            controller.NotifyPlateState(this, pressed);
    }

    public bool IsPressed => pressed;
}
