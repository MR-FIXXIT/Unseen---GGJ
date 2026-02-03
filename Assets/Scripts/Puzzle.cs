using UnityEngine;

public class Puzzle : Interactable
{
    [SerializeField] private GameObject panel;
    
    private void Start() {
        if (panel != null) panel.SetActive(false);
    }

    public override void Interact(Player player) {
        if (panel != null) { 
            panel.SetActive(!panel.activeSelf);
        }

    }
}
