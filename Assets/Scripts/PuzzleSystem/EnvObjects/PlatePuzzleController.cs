using UnityEngine;
using UnityEngine.Events;

public class PlatePuzzleController : MonoBehaviour
{
    public PressurePlate plateA;          // drag Plate A
    public PressurePlate plateB;          // drag Plate B
    public UnityEvent onBothPressed;      // door.Open
    public UnityEvent onAnyReleased;      // door.Close

    private void Awake()
    {
        plateA.controller = this;
        plateB.controller = this;
    }

    public void NotifyPlateState(PressurePlate plate, bool pressed)
    {
        if (plateA.IsPressed && plateB.IsPressed)
            onBothPressed.Invoke();
        else
            onAnyReleased.Invoke();
    }
}
