using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        public enum MenuWindow
        {
            Main,
            LevelSelect,
            Options,
            Credits
        }

        [SerializeField]
        private SerializableDictionary<MenuWindow, GameObject> windows;

        [SerializeField]
        private Button newGameButton;
        [SerializeField]
        private Button levelSelectButton;
        [SerializeField]
        private Button optionsButton;
        [SerializeField]
        private Button creditsButton;
        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private Button[] backButtons;

        private void Awake()
        {
            exitButton.onClick.AddListener(Application.Quit);
            levelSelectButton.onClick.AddListener(() => DisplayWindow(MenuWindow.LevelSelect));
            optionsButton.onClick.AddListener(() => DisplayWindow(MenuWindow.Options));
            creditsButton.onClick.AddListener(() => DisplayWindow(MenuWindow.Credits));
            newGameButton.onClick.AddListener(NewGame);
            foreach(Button button in backButtons)
            {
                button.onClick.AddListener(() => DisplayWindow(MenuWindow.Main));
            }
            DisplayWindow(MenuWindow.Main);
        }

        private void Start()
        {
            
        }

        private void DisplayWindow(MenuWindow windoToDisplay)
        {
            foreach (SerializableDictionary<MenuWindow, GameObject>.Pair window in windows)
            {
                if (window.Key == windoToDisplay)
                {
                    window.Value.SetActive(true);
                }
                else
                {
                    window.Value.SetActive(false);
                }
            }
        }

        private void NewGame()
        {
            SceneLoadingManager.Instance.StartLevel(0, true);
        }
    }
}
