using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    PlayerManager player;

    [SerializeField] List<Interactable> interactables;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        interactables = new List<Interactable>();
    }

    private void FixedUpdate()
    {
        if(!PlayerUIManager.instance.menuIsOpen && !PlayerUIManager.instance.popUpIsOpen)
        {
            CheckForInteractable();
        }
    }

    private void CheckForInteractable()
    {
        if (interactables.Count == 0) {
            // No interactables in range, return
            return;
        }

        if (interactables[0] == null) { 
            // Interactable is null, remove it from the list
            interactables.RemoveAt(0);
            return;
        }

        if (interactables[0] != null)
        {
           PlayerUIManager.instance.popUpManager.SendPlayerMessagePopUp(interactables[0].interactText);
        }
    }

    public void Interact()
    {
        PlayerUIManager.instance.popUpManager.CloseAllPopUpWindows();

        if (interactables.Count == 0)
            // No interactables in range, return
            return;
        

        if (interactables[0] != null)
        {
            interactables[0].Interact(player);
            RefreshInteractionList();
        }
    }

    private void RefreshInteractionList()
    {
        for (int i = 0; i < interactables.Count; i++)
        {
            if (interactables[i] == null)
            {
                interactables.RemoveAt(i);
                i--;
            }
        }
    }

    public void AddInteractable(Interactable interactable)
    {
        RefreshInteractionList();

        if (!interactables.Contains(interactable))
        {
            interactables.Add(interactable);
        }
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (interactables.Contains(interactable))
        {
            interactables.Remove(interactable);
        }

        RefreshInteractionList();
    }
}
