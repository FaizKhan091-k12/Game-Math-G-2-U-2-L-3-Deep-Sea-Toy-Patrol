using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    [ExecuteAlways]
    public class DeepSeaSceneBuilder : MonoBehaviour
    {
        [Header("Auto Build On Start")]
        [SerializeField] private bool buildOnStart = true;

        private void Start()
        {
            if (Application.isPlaying && buildOnStart)
            {
                if (FindFirstObjectByType<DeepSeaGameManager>() == null)
                {
                    BuildCompleteGameScene();
                }
            }
        }

        [ContextMenu("Build Complete Game Scene")]
        public void BuildCompleteGameScene()
        {
            Debug.Log("[DeepSeaSceneBuilder] Building Complete Deep Sea Toy Patrol Game Scene...");

            // 1. Camera setup
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.backgroundColor = new Color(0.08f, 0.42f, 0.68f, 1f);
                mainCam.orthographic = true;
                mainCam.orthographicSize = 5f;
            }

            // 2. Audio & SoundManager
            GameObject soundObj = GameObject.Find("SoundManager");
            if (soundObj == null)
            {
                soundObj = new GameObject("SoundManager");
            }
            SoundManager soundManager = soundObj.GetComponent<SoundManager>();
            if (soundManager == null)
            {
                soundManager = soundObj.AddComponent<SoundManager>();
            }

            // 3. Canvas & UI
            Canvas canvas = FindFirstObjectByType<Canvas>();
            GameObject canvasObj;
            if (canvas == null)
            {
                canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            else
            {
                canvasObj = canvas.gameObject;
            }

            // EventSystem
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            // Load Sprites
            Sprite bgSprite = LoadSprite("bg_deep_sea");
            Sprite titleLogoSprite = LoadSprite("title_logo");
            Sprite bigPlayBtnSprite = LoadSprite("btn_big_play");
            Sprite panelDiagSprite = LoadSprite("panel_solid_dialogue");
            Sprite panelCardSprite = LoadSprite("panel_solid_card");

            Sprite captainIdle = LoadSprite("captain_finn_idle");
            Sprite captainCheer = LoadSprite("captain_finn_cheer");
            Sprite subSprite = LoadSprite("yellow_submarine");
            Sprite octopusSprite = LoadSprite("pink_octopus");
            Sprite fishSprite = LoadSprite("tropical_fish");
            Sprite redCarSprite = LoadSprite("toy_red_car");
            Sprite blueBoatSprite = LoadSprite("toy_blue_boat");
            Sprite greenPlaneSprite = LoadSprite("toy_green_plane");
            Sprite starfishBtnSprite = LoadSprite("starfish_button");
            Sprite shipWheelSprite = LoadSprite("ship_wheel");
            Sprite goldBadgeSprite = LoadSprite("gold_star_badge");
            Sprite rockTabletSprite = LoadSprite("rock_tablet");
            Sprite waveIcon = LoadSprite("icon_wave");
            Sprite homeIcon = LoadSprite("icon_home");
            Sprite replayIcon = LoadSprite("icon_replay");
            Sprite soundOnIcon = LoadSprite("icon_sound_on");
            Sprite soundOffIcon = LoadSprite("icon_sound_off");
            Sprite starIcon = LoadSprite("icon_star");
            Sprite needleSprite = LoadSprite("wheel_needle");
            Sprite laserGlowSprite = LoadSprite("laser_glow");
            Sprite tallyMarkSprite = LoadSprite("tally_mark");
            Sprite tallySlashSprite = LoadSprite("tally_slash");
            TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

            // 4. Background
            GameObject bgObj = GetOrCreateChild(canvasObj, "Background");
            bgObj.transform.SetAsFirstSibling();
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            StretchFull(bgRect);
            Image bgImg = GetOrAddComponent<Image>(bgObj);
            bgImg.sprite = bgSprite;
            bgImg.color = Color.white;
            bgImg.raycastTarget = false;

            // Bubble particle system
            GameObject bubbleObj = GetOrCreateChild(canvasObj, "BubbleParticles");
            RectTransform bubbleRect = bubbleObj.GetComponent<RectTransform>();
            StretchFull(bubbleRect);
            GetOrAddComponent<BubbleParticleSystem>(bubbleObj);

            // 5. Game Manager Root
            GameObject gmObj = GetOrCreateChild(canvasObj, "GameManagerRoot");
            DeepSeaGameManager gameManager = GetOrAddComponent<DeepSeaGameManager>(gmObj);

            // 6. Dialogue / Captain Banner (Solid Opaque High-Contrast Background)
            GameObject dialogueObj = GetOrCreateChild(canvasObj, "CaptainDialogueBanner");
            RectTransform diagRect = dialogueObj.GetComponent<RectTransform>();
            diagRect.anchorMin = new Vector2(0.5f, 0f);
            diagRect.anchorMax = new Vector2(0.5f, 0f);
            diagRect.pivot = new Vector2(0.5f, 0f);
            diagRect.anchoredPosition = new Vector2(0f, 20f);
            diagRect.sizeDelta = new Vector2(1450f, 170f);

            // Solid 100% Opaque Dialog Background (Clean Standard Image without ProceduralImage shader conflict)
            Image diagBg = EnsureStandardImage(dialogueObj);
            diagBg.sprite = panelDiagSprite;
            diagBg.color = Color.white;
            diagBg.type = Image.Type.Simple;
            diagBg.preserveAspect = false;

            // Captain Avatar
            GameObject avatarObj = GetOrCreateChild(dialogueObj, "CaptainAvatar");
            RectTransform avatarRect = avatarObj.GetComponent<RectTransform>();
            avatarRect.anchorMin = new Vector2(0f, 0.5f);
            avatarRect.anchorMax = new Vector2(0f, 0.5f);
            avatarRect.pivot = new Vector2(0.5f, 0.5f);
            avatarRect.anchoredPosition = new Vector2(95f, 5f);
            avatarRect.sizeDelta = new Vector2(165f, 165f);
            Image avatarImg = GetOrAddComponent<Image>(avatarObj);
            avatarImg.sprite = captainIdle;
            avatarImg.preserveAspect = true;

            // Dialogue Text
            GameObject textObj = GetOrCreateChild(dialogueObj, "DialogueText");
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.offsetMin = new Vector2(195f, 15f);
            textRect.offsetMax = new Vector2(-80f, -15f);
            TextMeshProUGUI diagTMP = GetOrAddComponent<TextMeshProUGUI>(textObj);
            if (fontAsset != null) diagTMP.font = fontAsset;
            diagTMP.fontSize = 28;
            diagTMP.fontStyle = FontStyles.Bold;
            diagTMP.color = Color.white;
            diagTMP.textWrappingMode = TextWrappingModes.Normal;
            diagTMP.alignment = TextAlignmentOptions.MidlineLeft;
            diagTMP.text = "Welcome to Deep Sea Toy Patrol!";

            // Repeat speech button
            GameObject repeatBtnObj = GetOrCreateChild(dialogueObj, "RepeatSpeechBtn");
            RectTransform repeatRect = repeatBtnObj.GetComponent<RectTransform>();
            repeatRect.anchorMin = new Vector2(1f, 0.5f);
            repeatRect.anchorMax = new Vector2(1f, 0.5f);
            repeatRect.pivot = new Vector2(0.5f, 0.5f);
            repeatRect.anchoredPosition = new Vector2(-40f, 0f);
            repeatRect.sizeDelta = new Vector2(55f, 55f);
            Image repeatImg = GetOrAddComponent<Image>(repeatBtnObj);
            repeatImg.sprite = soundOnIcon;
            Button repeatBtn = GetOrAddComponent<Button>(repeatBtnObj);
            GetOrAddComponent<JuicyButton>(repeatBtnObj);

            DialogueManager diagManager = GetOrAddComponent<DialogueManager>(dialogueObj);
            SetPrivateField(diagManager, "captainAvatar", avatarImg);
            SetPrivateField(diagManager, "dialogueText", diagTMP);
            SetPrivateField(diagManager, "dialogueContainer", diagRect);
            SetPrivateField(diagManager, "repeatSpeechButton", repeatBtn);
            SetPrivateField(diagManager, "captainIdleSprite", captainIdle);
            SetPrivateField(diagManager, "captainCheerSprite", captainCheer);

            // 7. Top Header Bar
            GameObject topBarObj = GetOrCreateChild(canvasObj, "TopHeaderBar");
            RectTransform topBarRect = topBarObj.GetComponent<RectTransform>();
            topBarRect.anchorMin = new Vector2(0.5f, 1f);
            topBarRect.anchorMax = new Vector2(0.5f, 1f);
            topBarRect.pivot = new Vector2(0.5f, 1f);
            topBarRect.anchoredPosition = new Vector2(0f, -15f);
            topBarRect.sizeDelta = new Vector2(1300f, 75f);
            ProceduralImage topBarBg = SetupProceduralImage(topBarObj, new Color(0.04f, 0.18f, 0.36f, 0.95f), new Vector4(20f, 20f, 20f, 20f));

            // Home Button
            GameObject homeBtnObj = GetOrCreateChild(topBarObj, "HomeButton");
            RectTransform homeRect = homeBtnObj.GetComponent<RectTransform>();
            homeRect.anchorMin = new Vector2(0f, 0.5f);
            homeRect.anchorMax = new Vector2(0f, 0.5f);
            homeRect.pivot = new Vector2(0.5f, 0.5f);
            homeRect.anchoredPosition = new Vector2(45f, 0f);
            homeRect.sizeDelta = new Vector2(55f, 55f);
            Image homeImg = GetOrAddComponent<Image>(homeBtnObj);
            homeImg.sprite = homeIcon;
            Button homeBtn = GetOrAddComponent<Button>(homeBtnObj);
            GetOrAddComponent<JuicyButton>(homeBtnObj);

            // Title Text
            GameObject titleObj = GetOrCreateChild(topBarObj, "LevelTitleText");
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(100f, 0f);
            titleRect.offsetMax = new Vector2(-100f, 0f);
            TextMeshProUGUI titleTMP = GetOrAddComponent<TextMeshProUGUI>(titleObj);
            if (fontAsset != null) titleTMP.font = fontAsset;
            titleTMP.fontSize = 32;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(1f, 0.85f, 0.25f, 1f);
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.text = "Deep Sea Toy Patrol";

            // Sound Button
            GameObject soundBtnObj = GetOrCreateChild(topBarObj, "SoundButton");
            RectTransform soundRect = soundBtnObj.GetComponent<RectTransform>();
            soundRect.anchorMin = new Vector2(1f, 0.5f);
            soundRect.anchorMax = new Vector2(1f, 0.5f);
            soundRect.pivot = new Vector2(0.5f, 0.5f);
            soundRect.anchoredPosition = new Vector2(-45f, 0f);
            soundRect.sizeDelta = new Vector2(55f, 55f);
            Image soundImg = GetOrAddComponent<Image>(soundBtnObj);
            soundImg.sprite = soundOnIcon;
            Button soundBtn = GetOrAddComponent<Button>(soundBtnObj);
            GetOrAddComponent<JuicyButton>(soundBtnObj);

            // Edovu Logo Watermark
            Transform existingLogo = canvasObj.transform.Find("Edovu Logo ");
            if (existingLogo == null)
            {
                GameObject edovuPrefab = Resources.Load<GameObject>("Edovu Logo ");
                if (edovuPrefab == null) edovuPrefab = UnityEditorAssetLoader.LoadPrefab("Assets/Edovu Logo .prefab");
                if (edovuPrefab != null)
                {
                    GameObject logoInst = Object.Instantiate(edovuPrefab, canvasObj.transform);
                    logoInst.name = "Edovu Logo ";
                    logoInst.transform.SetAsLastSibling();
                }
            }

            // 8. SCREENS ROOT
            GameObject screensRoot = GetOrCreateChild(canvasObj, "ScreensRoot");
            RectTransform screensRootRect = screensRoot.GetComponent<RectTransform>();
            StretchFull(screensRootRect);

            // ==========================================
            // SCREEN 0: MAIN MENU
            // ==========================================
            GameObject menuScreen = GetOrCreateChild(screensRoot, "Screen_MainMenu");
            StretchFull(menuScreen.GetComponent<RectTransform>());

            // Remove legacy GameTitle if present
            Transform legacyTitle = menuScreen.transform.Find("GameTitle");
            if (legacyTitle != null) Object.DestroyImmediate(legacyTitle.gameObject);

            // Deluxe 3D Cartoon Title Logo Image (Playful, theme-accurate, transparent)
            GameObject menuLogoObj = GetOrCreateChild(menuScreen, "TitleLogoImage");
            RectTransform menuLogoRect = menuLogoObj.GetComponent<RectTransform>();
            menuLogoRect.anchorMin = new Vector2(0.5f, 1f);
            menuLogoRect.anchorMax = new Vector2(0.5f, 1f);
            menuLogoRect.pivot = new Vector2(0.5f, 1f);
            menuLogoRect.anchoredPosition = new Vector2(0f, -25f);
            menuLogoRect.sizeDelta = new Vector2(760f, 282f);

            Image menuLogoImg = EnsureStandardImage(menuLogoObj);
            menuLogoImg.sprite = titleLogoSprite;
            menuLogoImg.preserveAspect = true;
            menuLogoImg.raycastTarget = false;

            // Remove legacy subtitle / level bar / start patrol button if they existed
            Transform legacySub = menuScreen.transform.Find("GameSubtitle");
            if (legacySub != null) Object.DestroyImmediate(legacySub.gameObject);
            Transform legacyBar = menuScreen.transform.Find("LevelSelectBar");
            if (legacyBar != null) Object.DestroyImmediate(legacyBar.gameObject);
            Transform legacyStart = menuScreen.transform.Find("StartPatrolButton");
            if (legacyStart != null) Object.DestroyImmediate(legacyStart.gameObject);

            // Yellow Submarine in Main Menu (Left Side)
            GameObject subObj = GetOrCreateChild(menuScreen, "SubmarineDecor");
            RectTransform subRect = subObj.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.20f, 0.46f);
            subRect.anchorMax = new Vector2(0.20f, 0.46f);
            subRect.pivot = new Vector2(0.5f, 0.5f);
            subRect.anchoredPosition = Vector2.zero;
            subRect.sizeDelta = new Vector2(340f, 340f);
            Image subImg = EnsureStandardImage(subObj);
            subImg.sprite = subSprite;
            subImg.preserveAspect = true;

            // Big Juicy Play Button (DEAD CENTER OF THE SCREEN)
            GameObject playBtnObj = GetOrCreateChild(menuScreen, "BigPlayButton");
            RectTransform playBtnRect = playBtnObj.GetComponent<RectTransform>();
            playBtnRect.anchorMin = new Vector2(0.5f, 0.44f);
            playBtnRect.anchorMax = new Vector2(0.5f, 0.44f);
            playBtnRect.pivot = new Vector2(0.5f, 0.5f);
            playBtnRect.anchoredPosition = Vector2.zero;
            playBtnRect.sizeDelta = new Vector2(440f, 190f);
            Image playBtnImg = EnsureStandardImage(playBtnObj);
            playBtnImg.sprite = bigPlayBtnSprite;
            playBtnImg.preserveAspect = true;
            Button playBtn = GetOrAddComponent<Button>(playBtnObj);
            GetOrAddComponent<JuicyButton>(playBtnObj);

            // Friendly Sea Friend Decor on Right Side (Balances Submarine)
            GameObject rightDecor = GetOrCreateChild(menuScreen, "RightSeaFriendDecor");
            RectTransform rdRect = rightDecor.GetComponent<RectTransform>();
            rdRect.anchorMin = new Vector2(0.80f, 0.46f);
            rdRect.anchorMax = new Vector2(0.80f, 0.46f);
            rdRect.pivot = new Vector2(0.5f, 0.5f);
            rdRect.anchoredPosition = Vector2.zero;
            rdRect.sizeDelta = new Vector2(280f, 280f);
            Image rdImg = EnsureStandardImage(rightDecor);
            rdImg.sprite = octopusSprite;
            rdImg.preserveAspect = true;

            MainMenuController menuCtrl = GetOrAddComponent<MainMenuController>(menuScreen);
            SetPrivateField(menuCtrl, "playButton", playBtn);
            SetPrivateField(menuCtrl, "playButtonTransform", playBtnRect);
            SetPrivateField(menuCtrl, "yellowSubmarineTransform", subRect);
            SetPrivateField(menuCtrl, "titleLogoTransform", menuLogoRect);

            // ==========================================
            // SCREEN 1: LEVEL 1 (SORTING BAY)
            // ==========================================
            GameObject lvl1Screen = GetOrCreateChild(screensRoot, "Screen_Level1");
            StretchFull(lvl1Screen.GetComponent<RectTransform>());

            // 1. Question Banner (Placed prominently above the Octopuses)
            GameObject qBannerObj = GetOrCreateChild(lvl1Screen, "QuestionBanner_Lvl1");
            RectTransform qBannerRect = qBannerObj.GetComponent<RectTransform>();
            qBannerRect.anchorMin = new Vector2(0.5f, 0.85f);
            qBannerRect.anchorMax = new Vector2(0.5f, 0.85f);
            qBannerRect.pivot = new Vector2(0.5f, 0.5f);
            qBannerRect.anchoredPosition = Vector2.zero;
            qBannerRect.sizeDelta = new Vector2(1180f, 66f);

            Image qBannerBg = EnsureStandardImage(qBannerObj);
            qBannerBg.sprite = panelDiagSprite;
            qBannerBg.color = Color.white;
            qBannerBg.preserveAspect = false;

            GameObject qTextObj = GetOrCreateChild(qBannerObj, "Text");
            StretchFull(qTextObj.GetComponent<RectTransform>());
            TextMeshProUGUI qTMP = GetOrAddComponent<TextMeshProUGUI>(qTextObj);
            if (fontAsset != null) qTMP.font = fontAsset;
            qTMP.fontSize = 27;
            qTMP.fontStyle = FontStyles.Bold;
            qTMP.color = new Color(1f, 0.90f, 0.35f, 1f); // Radiant gold
            qTMP.alignment = TextAlignmentOptions.Center;
            qTMP.text = "Which question will give different answers we can sort and count?";

            // 2. Octopuses Bay Container
            GameObject octoBay = GetOrCreateChild(lvl1Screen, "OctopusesBay");
            RectTransform octoBayRect = octoBay.GetComponent<RectTransform>();
            octoBayRect.anchorMin = new Vector2(0.5f, 0.68f);
            octoBayRect.anchorMax = new Vector2(0.5f, 0.68f);
            octoBayRect.pivot = new Vector2(0.5f, 0.5f);
            octoBayRect.anchoredPosition = Vector2.zero;
            octoBayRect.sizeDelta = new Vector2(1200f, 210f);

            List<Image> octoImages = new List<Image>();
            List<Image> heldToys = new List<Image>();
            List<GameObject> yesBubbles = new List<GameObject>();

            float[] octoXPositions = new float[] { -440f, -220f, 0f, 220f, 440f };
            for (int i = 0; i < 5; i++)
            {
                GameObject octoObj = GetOrCreateChild(octoBay, $"Octopus_{i}");
                RectTransform oRect = octoObj.GetComponent<RectTransform>();
                oRect.anchoredPosition = new Vector2(octoXPositions[i], 0f);
                oRect.sizeDelta = new Vector2(190f, 190f);
                Image oImg = GetOrAddComponent<Image>(octoObj);
                oImg.sprite = octopusSprite;
                oImg.preserveAspect = true;
                octoImages.Add(oImg);

                // Held Toy (hidden initially)
                GameObject toyObj = GetOrCreateChild(octoObj, "HeldToy");
                RectTransform tRect = toyObj.GetComponent<RectTransform>();
                tRect.anchoredPosition = new Vector2(0f, 85f);
                tRect.sizeDelta = new Vector2(100f, 100f);
                Image tImg = GetOrAddComponent<Image>(toyObj);
                tImg.preserveAspect = true;
                toyObj.SetActive(false);
                heldToys.Add(tImg);

                // Yes Speech Bubble (for wrong answer reaction)
                GameObject yesObj = GetOrCreateChild(octoObj, "YesBubble");
                RectTransform yRect = yesObj.GetComponent<RectTransform>();
                yRect.anchoredPosition = new Vector2(0f, 105f);
                yRect.sizeDelta = new Vector2(90f, 55f);
                ProceduralImage yBg = SetupProceduralImage(yesObj, new Color(0.2f, 0.7f, 0.95f, 1.0f), new Vector4(16f, 16f, 16f, 16f));
                GameObject yText = GetOrCreateChild(yesObj, "Text");
                StretchFull(yText.GetComponent<RectTransform>());
                TextMeshProUGUI yTMP = GetOrAddComponent<TextMeshProUGUI>(yText);
                if (fontAsset != null) yTMP.font = fontAsset;
                yTMP.fontSize = 20;
                yTMP.fontStyle = FontStyles.Bold;
                yTMP.color = Color.white;
                yTMP.alignment = TextAlignmentOptions.Center;
                yTMP.text = "YES!";
                yesObj.SetActive(false);
                yesBubbles.Add(yesObj);
            }

            // 3. Designated Drop Slot (Clear, visible space to drop the selected question bubble)
            Sprite dropZoneSprite = LoadSprite("slot_drop_zone");

            GameObject dropSlotObj = GetOrCreateChild(lvl1Screen, "DropTargetSlot");
            RectTransform dropSlotRect = dropSlotObj.GetComponent<RectTransform>();
            dropSlotRect.anchorMin = new Vector2(0.5f, 0.49f);
            dropSlotRect.anchorMax = new Vector2(0.5f, 0.49f);
            dropSlotRect.pivot = new Vector2(0.5f, 0.5f);
            dropSlotRect.anchoredPosition = Vector2.zero;
            dropSlotRect.sizeDelta = new Vector2(620f, 100f);

            Image dropSlotImg = EnsureStandardImage(dropSlotObj);
            dropSlotImg.sprite = dropZoneSprite != null ? dropZoneSprite : panelCardSprite;
            dropSlotImg.color = Color.white;
            dropSlotImg.preserveAspect = false;

            GameObject dropLabelObj = GetOrCreateChild(dropSlotObj, "SlotLabel");
            RectTransform dlRect = dropLabelObj.GetComponent<RectTransform>();
            dlRect.anchorMin = new Vector2(0f, 0.36f);
            dlRect.anchorMax = new Vector2(1f, 0.96f);
            dlRect.offsetMin = Vector2.zero;
            dlRect.offsetMax = Vector2.zero;
            TextMeshProUGUI dlTMP = GetOrAddComponent<TextMeshProUGUI>(dropLabelObj);
            if (fontAsset != null) dlTMP.font = fontAsset;
            dlTMP.fontSize = 22;
            dlTMP.fontStyle = FontStyles.Bold;
            dlTMP.color = new Color(1f, 0.91f, 0.42f, 1f);
            dlTMP.alignment = TextAlignmentOptions.Center;
            dlTMP.text = "DROP SELECTED QUESTION HERE";

            GameObject dropSubObj = GetOrCreateChild(dropSlotObj, "SlotSubLabel");
            RectTransform dsRect = dropSubObj.GetComponent<RectTransform>();
            dsRect.anchorMin = new Vector2(0f, 0.06f);
            dsRect.anchorMax = new Vector2(1f, 0.40f);
            dsRect.offsetMin = Vector2.zero;
            dsRect.offsetMax = Vector2.zero;
            TextMeshProUGUI dsTMP = GetOrAddComponent<TextMeshProUGUI>(dropSubObj);
            if (fontAsset != null) dsTMP.font = fontAsset;
            dsTMP.fontSize = 15;
            dsTMP.fontStyle = FontStyles.Normal;
            dsTMP.color = new Color(0.70f, 0.93f, 1f, 1f);
            dsTMP.alignment = TextAlignmentOptions.Center;
            dsTMP.text = "Drag Bubble A or B up into this box";

            // 4. Questions Container (Bubble A vs Bubble B)
            GameObject questionsArea = GetOrCreateChild(lvl1Screen, "QuestionsArea");
            RectTransform qAreaRect = questionsArea.GetComponent<RectTransform>();
            qAreaRect.anchorMin = new Vector2(0.5f, 0.30f);
            qAreaRect.anchorMax = new Vector2(0.5f, 0.30f);
            qAreaRect.pivot = new Vector2(0.5f, 0.5f);
            qAreaRect.anchoredPosition = Vector2.zero;
            qAreaRect.sizeDelta = new Vector2(1100f, 150f);

            // Bubble A (Solid Opaque Card, Drag-Only)
            DraggableBubble bubbleA = CreateQuestionBubble(
                questionsArea,
                "BubbleA_ToyQuestion",
                new Vector2(-300f, 0f),
                "Bubble A:\n\"Which toy do you like best?\"",
                "Drag up into Drop Box",
                new Sprite[] { redCarSprite, blueBoatSprite, greenPlaneSprite },
                DraggableBubble.BubbleType.ToyQuestion_A,
                fontAsset,
                panelCardSprite
            );

            // Bubble B (Solid Opaque Card, Drag-Only)
            DraggableBubble bubbleB = CreateQuestionBubble(
                questionsArea,
                "BubbleB_OceanQuestion",
                new Vector2(300f, 0f),
                "Bubble B:\n\"Do you live in the ocean?\"",
                "Drag up into Drop Box",
                new Sprite[] { waveIcon },
                DraggableBubble.BubbleType.OceanQuestion_B,
                fontAsset,
                panelCardSprite
            );

            // 5. Results summary card
            GameObject resultsCard = GetOrCreateChild(lvl1Screen, "ResultsSummaryCard");
            RectTransform resRect = resultsCard.GetComponent<RectTransform>();
            resRect.anchorMin = new Vector2(0.5f, 0.49f);
            resRect.anchorMax = new Vector2(0.5f, 0.49f);
            resRect.pivot = new Vector2(0.5f, 0.5f);
            resRect.anchoredPosition = Vector2.zero;
            resRect.sizeDelta = new Vector2(780f, 120f);
            Image resBg = EnsureStandardImage(resultsCard);
            resBg.sprite = panelDiagSprite;
            resBg.color = Color.white;
            resBg.preserveAspect = false;

            GameObject resTextObj = GetOrCreateChild(resultsCard, "SummaryText");
            StretchFull(resTextObj.GetComponent<RectTransform>());
            TextMeshProUGUI resTMP = GetOrAddComponent<TextMeshProUGUI>(resTextObj);
            if (fontAsset != null) resTMP.font = fontAsset;
            resTMP.fontSize = 26;
            resTMP.fontStyle = FontStyles.Bold;
            resTMP.color = Color.white;
            resTMP.alignment = TextAlignmentOptions.Center;
            resTMP.text = "Votes Counted:\nRed Cars: 2    Blue Boats: 2    Green Planes: 1";
            resultsCard.SetActive(false);

            // Next Level Button
            Button lvl1NextBtn = CreateBigActionButton(lvl1Screen, "NextButton_Lvl1", "Continue to Level 2 ->", new Vector2(0.5f, 0.33f), fontAsset);

            Level1Controller lvl1Ctrl = GetOrAddComponent<Level1Controller>(lvl1Screen);
            SetPrivateField(lvl1Ctrl, "octopusesContainer", octoBayRect);
            SetPrivateField(lvl1Ctrl, "octopusImages", octoImages);
            SetPrivateField(lvl1Ctrl, "heldToyImages", heldToys);
            SetPrivateField(lvl1Ctrl, "yesBubbles", yesBubbles);
            SetPrivateField(lvl1Ctrl, "redCarSprite", redCarSprite);
            SetPrivateField(lvl1Ctrl, "blueBoatSprite", blueBoatSprite);
            SetPrivateField(lvl1Ctrl, "greenPlaneSprite", greenPlaneSprite);
            SetPrivateField(lvl1Ctrl, "bubbleA", bubbleA);
            SetPrivateField(lvl1Ctrl, "bubbleB", bubbleB);
            SetPrivateField(lvl1Ctrl, "dropTargetArea", dropSlotRect);
            SetPrivateField(lvl1Ctrl, "dropTargetVisual", dropSlotObj);
            SetPrivateField(lvl1Ctrl, "dropTargetLabel", dlTMP);
            SetPrivateField(lvl1Ctrl, "resultsCard", resRect);
            SetPrivateField(lvl1Ctrl, "nextLevelButton", lvl1NextBtn);

            // ==========================================
            // SCREEN 2: LEVEL 2 (STICK-MARK ROCK)
            // ==========================================
            GameObject lvl2Screen = GetOrCreateChild(screensRoot, "Screen_Level2");
            StretchFull(lvl2Screen.GetComponent<RectTransform>());

            // Stone Tablet
            GameObject stoneObj = GetOrCreateChild(lvl2Screen, "StoneTablet");
            RectTransform stoneRect = stoneObj.GetComponent<RectTransform>();
            stoneRect.anchorMin = new Vector2(0.5f, 0.61f);
            stoneRect.anchorMax = new Vector2(0.5f, 0.61f);
            stoneRect.pivot = new Vector2(0.5f, 0.5f);
            stoneRect.anchoredPosition = Vector2.zero;
            stoneRect.sizeDelta = new Vector2(720f, 400f);
            Image stoneImg = GetOrAddComponent<Image>(stoneObj);
            stoneImg.sprite = rockTabletSprite;
            stoneImg.preserveAspect = true;

            // Car carved header above stone
            GameObject carCarvedObj = GetOrCreateChild(stoneObj, "CarHeaderIcon");
            RectTransform carCarvedRect = carCarvedObj.GetComponent<RectTransform>();
            carCarvedRect.anchoredPosition = new Vector2(0f, 130f);
            carCarvedRect.sizeDelta = new Vector2(110f, 110f);
            Image carCarvedImg = GetOrAddComponent<Image>(carCarvedObj);
            carCarvedImg.sprite = redCarSprite;
            carCarvedImg.preserveAspect = true;

            // Tally Marks Container
            GameObject marksContainer = GetOrCreateChild(stoneObj, "MarksContainer");
            RectTransform marksRect = marksContainer.GetComponent<RectTransform>();
            marksRect.anchoredPosition = new Vector2(0f, -20f);
            marksRect.sizeDelta = new Vector2(560f, 180f);

            // Group 1: 4 straight marks + 1 diagonal slash
            GameObject group1Obj = GetOrCreateChild(marksContainer, "Group_1");
            RectTransform g1Rect = group1Obj.GetComponent<RectTransform>();
            g1Rect.anchoredPosition = new Vector2(-120f, 0f);
            g1Rect.sizeDelta = new Vector2(240f, 160f);

            List<Image> straightMarksList = new List<Image>();
            float[] g1Offsets = new float[] { -68f, -22f, 22f, 68f };
            for (int i = 0; i < 4; i++)
            {
                GameObject smObj = GetOrCreateChild(group1Obj, $"StraightMark_{i}");
                RectTransform smRect = smObj.GetComponent<RectTransform>();
                smRect.anchoredPosition = new Vector2(g1Offsets[i], 0f);
                smRect.sizeDelta = new Vector2(32f, 130f);
                Image smImg = GetOrAddComponent<Image>(smObj);
                smImg.sprite = tallyMarkSprite;
                smObj.SetActive(false);
                straightMarksList.Add(smImg);
            }

            // Diagonal slash mark
            GameObject slashObj = GetOrCreateChild(group1Obj, "DiagonalSlash");
            RectTransform slashRect = slashObj.GetComponent<RectTransform>();
            slashRect.anchoredPosition = Vector2.zero;
            slashRect.sizeDelta = new Vector2(190f, 150f);
            Image slashImg = GetOrAddComponent<Image>(slashObj);
            slashImg.sprite = tallySlashSprite;
            slashObj.SetActive(false);

            // Group 2: 3 straight marks
            GameObject group2Obj = GetOrCreateChild(marksContainer, "Group_2");
            RectTransform g2Rect = group2Obj.GetComponent<RectTransform>();
            g2Rect.anchoredPosition = new Vector2(140f, 0f);
            g2Rect.sizeDelta = new Vector2(180f, 160f);

            float[] g2Offsets = new float[] { -45f, 0f, 45f };
            for (int i = 0; i < 3; i++)
            {
                GameObject smObj = GetOrCreateChild(group2Obj, $"StraightMark_{4 + i}");
                RectTransform smRect = smObj.GetComponent<RectTransform>();
                smRect.anchoredPosition = new Vector2(g2Offsets[i], 0f);
                smRect.sizeDelta = new Vector2(32f, 130f);
                Image smImg = GetOrAddComponent<Image>(smObj);
                smImg.sprite = tallyMarkSprite;
                smObj.SetActive(false);
                straightMarksList.Add(smImg);
            }

            // Fish queue
            GameObject fishQueueObj = GetOrCreateChild(lvl2Screen, "FishQueueArea");
            RectTransform fishQRect = fishQueueObj.GetComponent<RectTransform>();
            fishQRect.anchorMin = new Vector2(0.5f, 0.35f);
            fishQRect.anchorMax = new Vector2(0.5f, 0.35f);
            fishQRect.pivot = new Vector2(0.5f, 0.5f);
            fishQRect.anchoredPosition = Vector2.zero;
            fishQRect.sizeDelta = new Vector2(1100f, 120f);

            List<RectTransform> fishRects = new List<RectTransform>();
            float startFishX = -460f;
            float stepFishX = 130f;
            for (int i = 0; i < 8; i++)
            {
                GameObject fObj = GetOrCreateChild(fishQueueObj, $"VoterFish_{i}");
                RectTransform fRect = fObj.GetComponent<RectTransform>();
                fRect.anchoredPosition = new Vector2(startFishX + (i * stepFishX), 0f);
                fRect.sizeDelta = new Vector2(110f, 110f);
                Image fImg = GetOrAddComponent<Image>(fObj);
                fImg.sprite = fishSprite;
                fImg.preserveAspect = true;

                // Little car held by fish
                GameObject fcObj = GetOrCreateChild(fObj, "HeldCar");
                RectTransform fcRect = fcObj.GetComponent<RectTransform>();
                fcRect.anchoredPosition = new Vector2(25f, -20f);
                fcRect.sizeDelta = new Vector2(50f, 50f);
                Image fcImg = GetOrAddComponent<Image>(fcObj);
                fcImg.sprite = redCarSprite;
                fcImg.preserveAspect = true;

                fObj.SetActive(false);
                fishRects.Add(fRect);
            }

            // Laser Button & Cannon
            GameObject laserBtnObj = GetOrCreateChild(lvl2Screen, "LaserToolButton");
            RectTransform lBtnRect = laserBtnObj.GetComponent<RectTransform>();
            lBtnRect.anchorMin = new Vector2(0.12f, 0.62f);
            lBtnRect.anchorMax = new Vector2(0.12f, 0.62f);
            lBtnRect.pivot = new Vector2(0.5f, 0.5f);
            lBtnRect.anchoredPosition = Vector2.zero;
                lBtnRect.sizeDelta = new Vector2(190f, 110f);
            ProceduralImage lBtnBg = SetupProceduralImage(laserBtnObj, new Color(0.85f, 0.15f, 0.25f, 1.0f), new Vector4(20f, 20f, 20f, 20f));
            Button laserBtn = GetOrAddComponent<Button>(laserBtnObj);
            GetOrAddComponent<JuicyButton>(laserBtnObj);

            GameObject lBtnText = GetOrCreateChild(laserBtnObj, "Label");
            StretchFull(lBtnText.GetComponent<RectTransform>());
            TextMeshProUGUI lTMP = GetOrAddComponent<TextMeshProUGUI>(lBtnText);
            if (fontAsset != null) lTMP.font = fontAsset;
            lTMP.fontSize = 24;
            lTMP.fontStyle = FontStyles.Bold;
            lTMP.color = Color.white;
            lTMP.alignment = TextAlignmentOptions.Center;
            lTMP.text = "TAP LASER!\n(Draw Mark)";

            // Laser beam effect
            GameObject beamObj = GetOrCreateChild(lvl2Screen, "LaserBeamEffect");
            RectTransform beamRect = beamObj.GetComponent<RectTransform>();
            beamRect.anchorMin = new Vector2(0.18f, 0.62f);
            beamRect.anchorMax = new Vector2(0.55f, 0.62f);
            beamRect.anchoredPosition = Vector2.zero;
            beamRect.sizeDelta = new Vector2(0f, 30f);
            Image beamImg = GetOrAddComponent<Image>(beamObj);
            beamImg.sprite = laserGlowSprite;
            beamObj.SetActive(false);

            // Chart display below
            GameObject chartRowObj = GetOrCreateChild(lvl2Screen, "ChartDisplayRow");
            RectTransform cRowRect = chartRowObj.GetComponent<RectTransform>();
            cRowRect.anchorMin = new Vector2(0.5f, 0.22f);
            cRowRect.anchorMax = new Vector2(0.5f, 0.22f);
            cRowRect.pivot = new Vector2(0.5f, 0.5f);
            cRowRect.anchoredPosition = Vector2.zero;
            cRowRect.sizeDelta = new Vector2(500f, 65f);
            ProceduralImage cRowBg = SetupProceduralImage(chartRowObj, new Color(0.04f, 0.22f, 0.42f, 1.0f), new Vector4(16f, 16f, 16f, 16f));

            GameObject cRowText = GetOrCreateChild(chartRowObj, "Label");
            StretchFull(cRowText.GetComponent<RectTransform>());
            TextMeshProUGUI cRowTMP = GetOrAddComponent<TextMeshProUGUI>(cRowText);
            if (fontAsset != null) cRowTMP.font = fontAsset;
            cRowTMP.fontSize = 26;
            cRowTMP.fontStyle = FontStyles.Bold;
            cRowTMP.color = Color.white;
            cRowTMP.alignment = TextAlignmentOptions.Center;
            cRowTMP.text = "Red Cars Chart Count: ";

            GameObject cCountObj = GetOrCreateChild(chartRowObj, "CountValue");
            RectTransform cCountRect = cCountObj.GetComponent<RectTransform>();
            cCountRect.anchorMin = new Vector2(1f, 0.5f);
            cCountRect.anchorMax = new Vector2(1f, 0.5f);
            cCountRect.pivot = new Vector2(0.5f, 0.5f);
            cCountRect.anchoredPosition = new Vector2(-50f, 0f);
            cCountRect.sizeDelta = new Vector2(60f, 50f);
            TextMeshProUGUI cCountTMP = GetOrAddComponent<TextMeshProUGUI>(cCountObj);
            if (fontAsset != null) cCountTMP.font = fontAsset;
            cCountTMP.fontSize = 32;
            cCountTMP.fontStyle = FontStyles.Bold;
            cCountTMP.color = new Color(1f, 0.85f, 0.2f, 1f);
            cCountTMP.alignment = TextAlignmentOptions.Center;
            cCountTMP.text = "?";

            Button lvl2NextBtn = CreateBigActionButton(lvl2Screen, "NextButton_Lvl2", "Continue to Level 3 ->", new Vector2(0f, 0.17f), fontAsset);

            Level2Controller lvl2Ctrl = GetOrAddComponent<Level2Controller>(lvl2Screen);
            SetPrivateField(lvl2Ctrl, "straightMarks", straightMarksList);
            SetPrivateField(lvl2Ctrl, "diagonalSlashMark", slashImg);
            SetPrivateField(lvl2Ctrl, "stoneTablet", stoneRect);
            SetPrivateField(lvl2Ctrl, "fishQueue", fishRects);
            SetPrivateField(lvl2Ctrl, "laserButton", laserBtn);
            SetPrivateField(lvl2Ctrl, "laserBeamEffect", beamImg);
            SetPrivateField(lvl2Ctrl, "chartCountText", cCountTMP);
            SetPrivateField(lvl2Ctrl, "chartRow", cRowRect);
            SetPrivateField(lvl2Ctrl, "nextLevelButton", lvl2NextBtn);

            // ==========================================
            // SCREEN 3: LEVEL 3 (CHART ROOM)
            // ==========================================
            GameObject lvl3Screen = GetOrCreateChild(screensRoot, "Screen_Level3");
            StretchFull(lvl3Screen.GetComponent<RectTransform>());

            // Frequency Table Slate
            GameObject tableSlateObj = GetOrCreateChild(lvl3Screen, "TableSlate");
            RectTransform tableSlateRect = tableSlateObj.GetComponent<RectTransform>();
            tableSlateRect.anchorMin = new Vector2(0.35f, 0.52f);
            tableSlateRect.anchorMax = new Vector2(0.35f, 0.52f);
            tableSlateRect.pivot = new Vector2(0.5f, 0.5f);
            tableSlateRect.anchoredPosition = Vector2.zero;
            tableSlateRect.sizeDelta = new Vector2(620f, 400f);
            ProceduralImage tsBg = SetupProceduralImage(tableSlateObj, new Color(0.05f, 0.2f, 0.38f, 1.0f), new Vector4(24f, 24f, 24f, 24f));

            // Table Header
            GameObject tHeader = GetOrCreateChild(tableSlateObj, "HeaderRow");
            RectTransform thRect = tHeader.GetComponent<RectTransform>();
            thRect.anchoredPosition = new Vector2(0f, 140f);
            thRect.sizeDelta = new Vector2(560f, 50f);
            TextMeshProUGUI thTMP = GetOrAddComponent<TextMeshProUGUI>(tHeader);
            if (fontAsset != null) thTMP.font = fontAsset;
            thTMP.fontSize = 24;
            thTMP.fontStyle = FontStyles.Bold;
            thTMP.color = new Color(0.9f, 0.95f, 1f, 1f);
            thTMP.alignment = TextAlignmentOptions.Center;
            thTMP.text = "TOY                    TOTAL VOTES";

            // 3 Rows
            List<RectTransform> rowsList = new List<RectTransform>();
            List<Button> rowButtons = new List<Button>();
            List<Image> rowHighlights = new List<Image>();

            string[] toyNames = new string[] { "Red Cars", "Blue Boats", "Green Planes" };
            int[] toyVotes = new int[] { 8, 3, 5 };
            Sprite[] rowIcons = new Sprite[] { redCarSprite, blueBoatSprite, greenPlaneSprite };
            float[] rowYPositions = new float[] { 65f, -15f, -95f };

            for (int i = 0; i < 3; i++)
            {
                GameObject rObj = GetOrCreateChild(tableSlateObj, $"Row_{toyNames[i]}");
                RectTransform rRect = rObj.GetComponent<RectTransform>();
                rRect.anchoredPosition = new Vector2(0f, rowYPositions[i]);
                rRect.sizeDelta = new Vector2(560f, 65f);
                ProceduralImage rBg = SetupProceduralImage(rObj, new Color(0.08f, 0.32f, 0.58f, 1.0f), new Vector4(14f, 14f, 14f, 14f));
                Button rBtn = GetOrAddComponent<Button>(rObj);
                GetOrAddComponent<JuicyButton>(rObj);

                // Row Icon
                GameObject riObj = GetOrCreateChild(rObj, "Icon");
                RectTransform riRect = riObj.GetComponent<RectTransform>();
                riRect.anchorMin = new Vector2(0f, 0.5f);
                riRect.anchorMax = new Vector2(0f, 0.5f);
                riRect.anchoredPosition = new Vector2(50f, 0f);
                riRect.sizeDelta = new Vector2(50f, 50f);
                Image riImg = GetOrAddComponent<Image>(riObj);
                riImg.sprite = rowIcons[i];
                riImg.preserveAspect = true;

                // Row Text
                GameObject rtObj = GetOrCreateChild(rObj, "NameText");
                RectTransform rtRect = rtObj.GetComponent<RectTransform>();
                rtRect.anchoredPosition = new Vector2(170f, 0f);
                rtRect.sizeDelta = new Vector2(200f, 50f);
                TextMeshProUGUI rtTMP = GetOrAddComponent<TextMeshProUGUI>(rtObj);
                if (fontAsset != null) rtTMP.font = fontAsset;
                rtTMP.fontSize = 26;
                rtTMP.fontStyle = FontStyles.Bold;
                rtTMP.color = Color.white;
                rtTMP.alignment = TextAlignmentOptions.MidlineLeft;
                rtTMP.text = toyNames[i];

                // Vote count badge
                GameObject rvObj = GetOrCreateChild(rObj, "VoteBadge");
                RectTransform rvRect = rvObj.GetComponent<RectTransform>();
                rvRect.anchorMin = new Vector2(1f, 0.5f);
                rvRect.anchorMax = new Vector2(1f, 0.5f);
                rvRect.anchoredPosition = new Vector2(-60f, 0f);
                rvRect.sizeDelta = new Vector2(60f, 48f);
                ProceduralImage rvBg = SetupProceduralImage(rvObj, new Color(0.95f, 0.75f, 0.15f, 1f), new Vector4(12f, 12f, 12f, 12f));
                GameObject rvtObj = GetOrCreateChild(rvObj, "Text");
                StretchFull(rvtObj.GetComponent<RectTransform>());
                TextMeshProUGUI rvtTMP = GetOrAddComponent<TextMeshProUGUI>(rvtObj);
                if (fontAsset != null) rvtTMP.font = fontAsset;
                rvtTMP.fontSize = 28;
                rvtTMP.fontStyle = FontStyles.Bold;
                rvtTMP.color = new Color(0.35f, 0.15f, 0f, 1f);
                rvtTMP.alignment = TextAlignmentOptions.Center;
                rvtTMP.text = toyVotes[i].ToString();

                // Highlight overlay
                GameObject rhObj = GetOrCreateChild(rObj, "HighlightOverlay");
                StretchFull(rhObj.GetComponent<RectTransform>());
                ProceduralImage rhImg = SetupProceduralImage(rhObj, new Color(1f, 0.9f, 0.2f, 0.35f), new Vector4(14f, 14f, 14f, 14f));
                rhObj.SetActive(false);

                rowsList.Add(rRect);
                rowButtons.Add(rBtn);
                rowHighlights.Add(rhImg);
            }

            // Ship Steering Wheel Area
            GameObject wheelArea = GetOrCreateChild(lvl3Screen, "WheelArea");
            RectTransform wAreaRect = wheelArea.GetComponent<RectTransform>();
            wAreaRect.anchorMin = new Vector2(0.74f, 0.52f);
            wAreaRect.anchorMax = new Vector2(0.74f, 0.52f);
            wAreaRect.pivot = new Vector2(0.5f, 0.5f);
            wAreaRect.anchoredPosition = Vector2.zero;
            wAreaRect.sizeDelta = new Vector2(360f, 360f);

            // Wheel transform
            GameObject wheelImgObj = GetOrCreateChild(wheelArea, "ShipWheel");
            RectTransform wImgRect = wheelImgObj.GetComponent<RectTransform>();
            wImgRect.anchoredPosition = Vector2.zero;
            wImgRect.sizeDelta = new Vector2(320f, 320f);
            Image wImg = GetOrAddComponent<Image>(wheelImgObj);
            wImg.sprite = shipWheelSprite;
            wImg.preserveAspect = true;

            // Needle Pointer
            GameObject needleObj = GetOrCreateChild(wheelArea, "PointerNeedle");
            RectTransform needleRect = needleObj.GetComponent<RectTransform>();
            needleRect.anchoredPosition = new Vector2(-155f, 0f);
            needleRect.sizeDelta = new Vector2(100f, 50f);
            needleRect.localEulerAngles = new Vector3(0f, 0f, 180f);
            Image nImg = GetOrAddComponent<Image>(needleObj);
            nImg.sprite = needleSprite;
            nImg.preserveAspect = true;

            // Rotate controls & Confirm Button
            GameObject wheelCtrlRow = GetOrCreateChild(lvl3Screen, "WheelControlRow");
            RectTransform wcRowRect = wheelCtrlRow.GetComponent<RectTransform>();
            wcRowRect.anchorMin = new Vector2(0.74f, 0.25f);
            wcRowRect.anchorMax = new Vector2(0.74f, 0.25f);
            wcRowRect.pivot = new Vector2(0.5f, 0.5f);
            wcRowRect.anchoredPosition = Vector2.zero;
            wcRowRect.sizeDelta = new Vector2(340f, 65f);

            Button rotUpBtn = CreateSmallIconButton(wheelCtrlRow, "RotUpBtn", "^", new Vector2(-110f, 0f), fontAsset);
            Button rotDownBtn = CreateSmallIconButton(wheelCtrlRow, "RotDownBtn", "v", new Vector2(-40f, 0f), fontAsset);
            Button confirmBtn = CreateSmallActionButton(wheelCtrlRow, "ConfirmBtn", "LOCK IN", new Vector2(75f, 0f), fontAsset);

            Button lvl3NextBtn = CreateBigActionButton(lvl3Screen, "NextButton_Lvl3", "Finish Patrol! View Award", new Vector2(0f, 0.17f), fontAsset);

            Level3Controller lvl3Ctrl = GetOrAddComponent<Level3Controller>(lvl3Screen);
            SetPrivateField(lvl3Ctrl, "shipWheelTransform", wImgRect);
            SetPrivateField(lvl3Ctrl, "pointerNeedle", needleRect);
            SetPrivateField(lvl3Ctrl, "rotateClockwiseBtn", rotDownBtn);
            SetPrivateField(lvl3Ctrl, "rotateCounterBtn", rotUpBtn);
            SetPrivateField(lvl3Ctrl, "lockRowButton", confirmBtn);
            SetPrivateField(lvl3Ctrl, "tableRows", rowsList);
            SetPrivateField(lvl3Ctrl, "rowSelectButtons", rowButtons);
            SetPrivateField(lvl3Ctrl, "rowHighlightOverlays", rowHighlights);
            SetPrivateField(lvl3Ctrl, "continueToSummaryButton", lvl3NextBtn);

            // ==========================================
            // SCREEN 4: SUMMARY (PATROL COMPLETE!)
            // ==========================================
            GameObject sumScreen = GetOrCreateChild(screensRoot, "Screen_Summary");
            StretchFull(sumScreen.GetComponent<RectTransform>());

            // Title
            GameObject sumTitleObj = GetOrCreateChild(sumScreen, "SummaryTitle");
            RectTransform sumTitleRect = sumTitleObj.GetComponent<RectTransform>();
            sumTitleRect.anchorMin = new Vector2(0.5f, 1f);
            sumTitleRect.anchorMax = new Vector2(0.5f, 1f);
            sumTitleRect.pivot = new Vector2(0.5f, 1f);
            sumTitleRect.anchoredPosition = new Vector2(0f, -80f);
            sumTitleRect.sizeDelta = new Vector2(1000f, 90f);
            TextMeshProUGUI sumTMP = GetOrAddComponent<TextMeshProUGUI>(sumTitleObj);
            if (fontAsset != null) sumTMP.font = fontAsset;
            sumTMP.fontSize = 52;
            sumTMP.fontStyle = FontStyles.Bold;
            sumTMP.color = new Color(1f, 0.88f, 0.2f, 1f);
            sumTMP.alignment = TextAlignmentOptions.Center;
            sumTMP.text = "PATROL COMPLETE!";

            // Dropping Gold Badge
            GameObject badgeObj = GetOrCreateChild(sumScreen, "GoldStarBadge");
            RectTransform badgeRect = badgeObj.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(0.5f, 0.55f);
            badgeRect.anchorMax = new Vector2(0.5f, 0.55f);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.anchoredPosition = Vector2.zero;
            badgeRect.sizeDelta = new Vector2(340f, 340f);
            Image badgeImg = GetOrAddComponent<Image>(badgeObj);
            badgeImg.sprite = goldBadgeSprite;
            badgeImg.preserveAspect = true;

            // Happy Swimming Animals in celebration
            List<RectTransform> celebAnimals = new List<RectTransform>();
            float[] celebX = new float[] { -480f, -320f, 320f, 480f };
            Sprite[] celebSprites = new Sprite[] { octopusSprite, fishSprite, octopusSprite, fishSprite };
            for (int i = 0; i < 4; i++)
            {
                GameObject caObj = GetOrCreateChild(sumScreen, $"CelebrationAnimal_{i}");
                RectTransform caRect = caObj.GetComponent<RectTransform>();
                caRect.anchorMin = new Vector2(0.5f, 0.52f);
                caRect.anchorMax = new Vector2(0.5f, 0.52f);
                caRect.pivot = new Vector2(0.5f, 0.5f);
                caRect.anchoredPosition = new Vector2(celebX[i], 0f);
                caRect.sizeDelta = new Vector2(170f, 170f);
                Image caImg = GetOrAddComponent<Image>(caObj);
                caImg.sprite = celebSprites[i];
                caImg.preserveAspect = true;
                celebAnimals.Add(caRect);
            }

            // Buttons: Play Again & Main Menu
            GameObject sumBtnsRow = GetOrCreateChild(sumScreen, "SummaryButtonsRow");
            RectTransform sbRowRect = sumBtnsRow.GetComponent<RectTransform>();
            sbRowRect.anchorMin = new Vector2(0.5f, 0.22f);
            sbRowRect.anchorMax = new Vector2(0.5f, 0.22f);
            sbRowRect.pivot = new Vector2(0.5f, 0.5f);
            sbRowRect.anchoredPosition = Vector2.zero;
            sbRowRect.sizeDelta = new Vector2(620f, 80f);

            Button playAgainBtn = CreateSummaryButton(sumBtnsRow, "PlayAgainBtn", "Play Again", new Vector2(-150f, 0f), new Color(0.18f, 0.72f, 0.35f, 1f), fontAsset);
            Button menuBtn = CreateSummaryButton(sumBtnsRow, "MainMenuBtn", "Main Menu", new Vector2(150f, 0f), new Color(0.12f, 0.52f, 0.85f, 1f), fontAsset);

            SummaryController sumCtrl = GetOrAddComponent<SummaryController>(sumScreen);
            SetPrivateField(sumCtrl, "goldBadgeTransform", badgeRect);
            SetPrivateField(sumCtrl, "celebrationAnimals", celebAnimals);
            SetPrivateField(sumCtrl, "playAgainButton", playAgainBtn);
            SetPrivateField(sumCtrl, "mainMenuButton", menuBtn);

            // Screen Transition CanvasGroup
            GameObject transObj = GetOrCreateChild(canvasObj, "ScreenTransitionOverlay");
            transObj.transform.SetAsLastSibling();
            StretchFull(transObj.GetComponent<RectTransform>());
            Image transImg = GetOrAddComponent<Image>(transObj);
            transImg.color = new Color(0.04f, 0.25f, 0.45f, 1f);
            transImg.raycastTarget = false;
            CanvasGroup transCG = GetOrAddComponent<CanvasGroup>(transObj);
            transCG.alpha = 0f;
            transObj.SetActive(false);

            // Link DeepSeaGameManager
            SetPrivateField(gameManager, "mainMenuScreen", menuScreen);
            SetPrivateField(gameManager, "level1Screen", lvl1Screen);
            SetPrivateField(gameManager, "level2Screen", lvl2Screen);
            SetPrivateField(gameManager, "level3Screen", lvl3Screen);
            SetPrivateField(gameManager, "summaryScreen", sumScreen);
            SetPrivateField(gameManager, "topHeaderBar", topBarObj);
            SetPrivateField(gameManager, "levelTitleText", titleTMP);
            SetPrivateField(gameManager, "homeButton", homeBtn);
            SetPrivateField(gameManager, "soundButton", soundBtn);
            SetPrivateField(gameManager, "soundIconImage", soundImg);
            SetPrivateField(gameManager, "soundOnSprite", soundOnIcon);
            SetPrivateField(gameManager, "soundOffSprite", soundOffIcon);
            SetPrivateField(gameManager, "screenTransitionGroup", transCG);

            Debug.Log("[DeepSeaSceneBuilder] Complete Deep Sea Toy Patrol Game Scene built successfully!");
        }

        private static Sprite LoadSprite(string spriteName)
        {
            Sprite s = Resources.Load<Sprite>($"Art/Sprites/{spriteName}");
            if (s == null)
            {
                s = UnityEditorAssetLoader.LoadSprite($"Assets/Art/Sprites/{spriteName}.png");
            }
            return s;
        }

        private static DraggableBubble CreateQuestionBubble(GameObject parent, string name, Vector2 pos, string questionText, string dragHelp, Sprite[] previewIcons, DraggableBubble.BubbleType type, TMP_FontAsset font, Sprite cardSprite)
        {
            GameObject bubbleObj = GetOrCreateChild(parent, name);
            RectTransform rect = bubbleObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(490f, 170f);

            // Solid 100% Opaque Card Background (Standard Image, NO ProceduralImage)
            Image bg = EnsureStandardImage(bubbleObj);
            bg.sprite = cardSprite;
            bg.color = Color.white;
            bg.preserveAspect = false;

            // Remove Button or JuicyButton if present (drag only!)
            Button existingBtn = bubbleObj.GetComponent<Button>();
            if (existingBtn != null) Object.DestroyImmediate(existingBtn);
            JuicyButton existingJuicy = bubbleObj.GetComponent<JuicyButton>();
            if (existingJuicy != null) Object.DestroyImmediate(existingJuicy);

            // Question Text (Dark navy, bold, crisp!)
            GameObject textObj = GetOrCreateChild(bubbleObj, "QuestionText");
            RectTransform tRect = textObj.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0f, 0.45f);
            tRect.anchorMax = new Vector2(1f, 1f);
            tRect.offsetMin = new Vector2(20f, 0f);
            tRect.offsetMax = new Vector2(-20f, -12f);
            TextMeshProUGUI tmp = GetOrAddComponent<TextMeshProUGUI>(textObj);
            if (font != null) tmp.font = font;
            tmp.fontSize = 24;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = new Color(0.08f, 0.2f, 0.38f, 1f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = questionText;

            // Icons row
            GameObject iconsObj = GetOrCreateChild(bubbleObj, "IconsRow");
            RectTransform iRect = iconsObj.GetComponent<RectTransform>();
            iRect.anchorMin = new Vector2(0.5f, 0.25f);
            iRect.anchorMax = new Vector2(0.5f, 0.25f);
            iRect.sizeDelta = new Vector2(300f, 48f);
            iRect.anchoredPosition = Vector2.zero;

            float startX = -(previewIcons.Length - 1) * 35f;
            for (int i = 0; i < previewIcons.Length; i++)
            {
                if (previewIcons[i] == null) continue;
                GameObject iconObj = GetOrCreateChild(iconsObj, $"Icon_{i}");
                RectTransform ir = iconObj.GetComponent<RectTransform>();
                ir.anchoredPosition = new Vector2(startX + (i * 70f), 0f);
                ir.sizeDelta = new Vector2(48f, 48f);
                Image img = GetOrAddComponent<Image>(iconObj);
                img.sprite = previewIcons[i];
                img.preserveAspect = true;
            }

            // Drag prompt label
            GameObject hintObj = GetOrCreateChild(bubbleObj, "DragHint");
            RectTransform hRect = hintObj.GetComponent<RectTransform>();
            hRect.anchorMin = new Vector2(0.5f, 0f);
            hRect.anchorMax = new Vector2(0.5f, 0f);
            hRect.pivot = new Vector2(0.5f, 0f);
            hRect.anchoredPosition = new Vector2(0f, 8f);
            hRect.sizeDelta = new Vector2(300f, 25f);
            TextMeshProUGUI hTmp = GetOrAddComponent<TextMeshProUGUI>(hintObj);
            if (font != null) hTmp.font = font;
            hTmp.fontSize = 17;
            hTmp.fontStyle = FontStyles.Bold;
            hTmp.color = new Color(0.1f, 0.45f, 0.8f, 1f);
            hTmp.alignment = TextAlignmentOptions.Center;
            hTmp.text = dragHelp;

            DraggableBubble db = GetOrAddComponent<DraggableBubble>(bubbleObj);
            SetPrivateField(db, "bubbleType", type);
            return db;
        }

        private static Button CreateBigActionButton(GameObject parent, string name, string label, Vector2 anchor, TMP_FontAsset font)
        {
            GameObject btnObj = GetOrCreateChild(parent, name);
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(460f, 84f);
            ProceduralImage bg = SetupProceduralImage(btnObj, new Color(0.18f, 0.72f, 0.35f, 1.0f), new Vector4(22f, 22f, 22f, 22f));
            Button btn = GetOrAddComponent<Button>(btnObj);
            GetOrAddComponent<JuicyButton>(btnObj);

            GameObject textObj = GetOrCreateChild(btnObj, "Label");
            StretchFull(textObj.GetComponent<RectTransform>());
            TextMeshProUGUI tmp = GetOrAddComponent<TextMeshProUGUI>(textObj);
            if (font != null) tmp.font = font;
            tmp.fontSize = 28;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = label;

            return btn;
        }

        private static Button CreateSmallIconButton(GameObject parent, string name, string iconText, Vector2 pos, TMP_FontAsset font)
        {
            GameObject btnObj = GetOrCreateChild(parent, name);
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(55f, 55f);
            ProceduralImage bg = SetupProceduralImage(btnObj, new Color(0.08f, 0.32f, 0.58f, 1.0f), new Vector4(14f, 14f, 14f, 14f));
            Button btn = GetOrAddComponent<Button>(btnObj);
            GetOrAddComponent<JuicyButton>(btnObj);

            GameObject tObj = GetOrCreateChild(btnObj, "Text");
            StretchFull(tObj.GetComponent<RectTransform>());
            TextMeshProUGUI tmp = GetOrAddComponent<TextMeshProUGUI>(tObj);
            if (font != null) tmp.font = font;
            tmp.fontSize = 24;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = iconText;
            return btn;
        }

        private static Button CreateSmallActionButton(GameObject parent, string name, string label, Vector2 pos, TMP_FontAsset font)
        {
            GameObject btnObj = GetOrCreateChild(parent, name);
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(160f, 55f);
            ProceduralImage bg = SetupProceduralImage(btnObj, new Color(0.92f, 0.65f, 0.12f, 1.0f), new Vector4(14f, 14f, 14f, 14f));
            Button btn = GetOrAddComponent<Button>(btnObj);
            GetOrAddComponent<JuicyButton>(btnObj);

            GameObject tObj = GetOrCreateChild(btnObj, "Text");
            StretchFull(tObj.GetComponent<RectTransform>());
            TextMeshProUGUI tmp = GetOrAddComponent<TextMeshProUGUI>(tObj);
            if (font != null) tmp.font = font;
            tmp.fontSize = 22;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = new Color(0.25f, 0.1f, 0f, 1f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = label;
            return btn;
        }

        private static Button CreateSummaryButton(GameObject parent, string name, string label, Vector2 pos, Color col, TMP_FontAsset font)
        {
            GameObject btnObj = GetOrCreateChild(parent, name);
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(260f, 70f);
            ProceduralImage bg = SetupProceduralImage(btnObj, col, new Vector4(18f, 18f, 18f, 18f));
            Button btn = GetOrAddComponent<Button>(btnObj);
            GetOrAddComponent<JuicyButton>(btnObj);

            GameObject tObj = GetOrCreateChild(btnObj, "Text");
            StretchFull(tObj.GetComponent<RectTransform>());
            TextMeshProUGUI tmp = GetOrAddComponent<TextMeshProUGUI>(tObj);
            if (font != null) tmp.font = font;
            tmp.fontSize = 26;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = label;
            return btn;
        }

        private static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static GameObject GetOrCreateChild(GameObject parent, string name)
        {
            Transform existing = parent.transform.Find(name);
            if (existing != null) return existing.gameObject;

            GameObject child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            child.AddComponent<RectTransform>();
            return child;
        }

        private static Image EnsureStandardImage(GameObject obj)
        {
            ProceduralImage pImg = obj.GetComponent<ProceduralImage>();
            if (pImg != null) Object.DestroyImmediate(pImg);
            ProceduralImageModifier mod = obj.GetComponent<ProceduralImageModifier>();
            if (mod != null) Object.DestroyImmediate(mod);
            return GetOrAddComponent<Image>(obj);
        }

        private static ProceduralImage SetupProceduralImage(GameObject obj, Color color, Vector4 radius)
        {
            ProceduralImage pImg = GetOrAddComponent<ProceduralImage>(obj);
            pImg.color = color;
            pImg.BorderWidth = 0f;
            pImg.FalloffDistance = 1f;
            FreeModifier mod = GetOrAddComponent<FreeModifier>(obj);
            mod.Radius = radius;
            return pImg;
        }

        private static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp == null) comp = obj.AddComponent<T>();
            return comp;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            if (target == null) return;
            System.Reflection.FieldInfo field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
    }

    public static class UnityEditorAssetLoader
    {
        public static Sprite LoadSprite(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
#else
            return null;
#endif
        }

        public static GameObject LoadPrefab(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
#else
            return null;
#endif
        }
    }
}
