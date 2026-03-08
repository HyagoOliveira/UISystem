using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    public abstract class AbstractMainMenuOptionsScreen : MonoBehaviour
    {
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        protected abstract void Method();
    }
}