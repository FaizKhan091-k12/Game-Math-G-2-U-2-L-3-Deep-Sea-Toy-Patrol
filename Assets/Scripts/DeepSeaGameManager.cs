using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class DeepSeaGameManager : MonoBehaviour
    {
        public static DeepSeaGameManager Instance { get; private set; }

        public enum GameScreen
        {
            MainMenu,
            Level1_SortingBay,
            Level2_StickMarkRock,
            Level3_ChartRoom,
            SummaryScreen
        }

        [Header("Screens")]
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject level1Screen;
        [SerializeField] private GameObject level2Screen;
        [SerializeField] private GameObject level3Screen;
        [SerializeField] private GameObject summaryScreen;

        [Header("Top Header Bar")]
        [SerializeField] private GameObject topHeaderBar;
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Image soundIconImage;
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;

        [Header("Screen Transition")]
        [SerializeField] private CanvasGroup screenTransitionGroup;

        private GameScreen currentScreen = GameScreen.MainMenu;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            SetupHeaderButtons();
        }

        private void Start()
        {
            ShowScreen(GameScreen.MainMenu);
            UpdateSoundIcon();
        }

        private void SetupHeaderButtons()
        {
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(() =>
                {
                    SoundManager.Instance?.PlayClick();
                    GoToMainMenu();
                });
            }

            if (soundButton != null)
            {
                soundButton.onClick.AddListener(() =>
                {
                    SoundManager.Instance?.ToggleSound();
                    UpdateSoundIcon();
                });
            }
        }

        public void UpdateSoundIcon()
        {
            if (soundIconImage != null && SoundManager.Instance != null)
            {
                soundIconImage.sprite = SoundManager.Instance.IsMuted ? soundOffSprite : soundOnSprite;
            }
        }

        public void GoToMainMenu() => ShowScreen(GameScreen.MainMenu);
        public void GoToSummary() => ShowScreen(GameScreen.SummaryScreen);

        public void GoToLevel(int levelNumber)
        {
            switch (levelNumber)
            {
                case 1: ShowScreen(GameScreen.Level1_SortingBay); break;
                case 2: ShowScreen(GameScreen.Level2_StickMarkRock); break;
                case 3: ShowScreen(GameScreen.Level3_ChartRoom); break;
                default: ShowScreen(GameScreen.MainMenu); break;
            }
        }

        private void ShowScreen(GameScreen screen)
        {
            currentScreen = screen;

            if (screenTransitionGroup != null)
            {
                screenTransitionGroup.DOKill();
                screenTransitionGroup.alpha = 0f;
                screenTransitionGroup.gameObject.SetActive(true);
                screenTransitionGroup.DOFade(1f, 0.2f).OnComplete(() =>
                {
                    ActivateScreensInternal(screen);
                    screenTransitionGroup.DOFade(0f, 0.25f).OnComplete(() =>
                    {
                        screenTransitionGroup.gameObject.SetActive(false);
                    });
                });
            }
            else
            {
                ActivateScreensInternal(screen);
            }
        }

        private void ActivateScreensInternal(GameScreen screen)
        {
            if (mainMenuScreen != null) mainMenuScreen.SetActive(screen == GameScreen.MainMenu);
            if (level1Screen != null) level1Screen.SetActive(screen == GameScreen.Level1_SortingBay);
            if (level2Screen != null) level2Screen.SetActive(screen == GameScreen.Level2_StickMarkRock);
            if (level3Screen != null) level3Screen.SetActive(screen == GameScreen.Level3_ChartRoom);
            if (summaryScreen != null) summaryScreen.SetActive(screen == GameScreen.SummaryScreen);

            // Update top bar
            if (topHeaderBar != null)
            {
                topHeaderBar.SetActive(screen != GameScreen.MainMenu);
            }

            if (levelTitleText != null)
            {
                switch (screen)
                {
                    case GameScreen.Level1_SortingBay:
                        levelTitleText.text = "Level 1: The Sorting Bay";
                        break;
                    case GameScreen.Level2_StickMarkRock:
                        levelTitleText.text = "Level 2: The Stick-Mark Rock";
                        break;
                    case GameScreen.Level3_ChartRoom:
                        levelTitleText.text = "Level 3: The Chart Room";
                        break;
                    case GameScreen.SummaryScreen:
                        levelTitleText.text = "Patrol Complete!";
                        break;
                    default:
                        levelTitleText.text = "Deep Sea Toy Patrol";
                        break;
                }
            }
        }
    }
}
