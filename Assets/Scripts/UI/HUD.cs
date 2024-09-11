using UnityEngine;
using UnityEngine.UIElements;

using Entities;

namespace UI
{

    /// <summary>
    /// Manages the HUD that displays the player's score.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [HideInInspector] public Entity player;
        private VisualElement root;
        private Label score;

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            score = root.Q<Label>("score");
        }

        void Update()
        {
            score.text = player?.score.ToString() ?? "0";
        }
    }
}