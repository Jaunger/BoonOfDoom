// GateController.cs
using UnityEngine;

public class GateController : MonoBehaviour
{
    [Tooltip("How far (units) the gate moves down when opened")]
    public float openOffsetY = 5f;

    private Vector3 closedPosition;
    private bool isOpen = false;

    private void Awake()
    {
        closedPosition = transform.position;
    }

    /// <summary>
    /// Lowers the gate by openOffsetY units on the Y axis.
    /// Subsequent calls do nothing if the gate is already open.
    /// </summary>
    public void Open()
    {
        if (isOpen) return;

        Vector3 newPos = closedPosition;
        newPos.y -= openOffsetY;
        transform.position = newPos;

        isOpen = true;
    }

    /// <summary>
    /// (Optional) Closes the gate by restoring its original position.
    /// </summary>
    public void Close()
    {
        if (!isOpen) return;

        transform.position = closedPosition;
        isOpen = false;
    }
}
