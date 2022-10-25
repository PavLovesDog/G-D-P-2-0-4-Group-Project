using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

namespace MBF
{
    public class GameManager : MonoBehaviour
    {
        public bool gamePaused;
        bool canPauseGame;
        public bool pauseMenuEnabled;
        public bool gameComplete;
        bool canCompleteGame;
        Animator playerAnimator;
        InputHandler inputHandler;
        EventSystem eventSystem;
        EndZoneHandler endZoneHandler;

        //need reference to all UI elements to be able to loop through image components to enable and disable sprites..
        public GameObject mainMenu;
        public GameObject pauseMenu;
        public GameObject creditMenu;
        public GameObject gameCompleteMenu;
        public GameObject healthBar;
        public GameObject warmthMeter;
        public GameObject reticle;

        public GameObject startButton; // Title screen button
        public GameObject resumeButton; // pause screen button
        public GameObject returnButton; // credits screen button
        public GameObject mainMenuButton; // ending screen button

        public bool pauseCameraTransition;
        //float cameraTransitionTimer;

        void Start()
        {
            canPauseGame = true;
            gamePaused = true;
            gameComplete = false;
            canCompleteGame = true;
            pauseMenuEnabled = false;
            pauseCameraTransition = true;
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
            inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<InputHandler>();
            eventSystem = GameObject.FindGameObjectWithTag("Event System").GetComponent<EventSystem>();
            endZoneHandler = FindObjectOfType<EndZoneHandler>();

            //disable all menu options that are NOT the main menu
            DisableImageSprites(pauseMenu);
            DisableText(pauseMenu);

            DisableImageSprites(creditMenu);
            DisableText(creditMenu);

            DisableImageSprites(gameCompleteMenu);
            DisableText(gameCompleteMenu);

            DisableImageSprites(healthBar);
            DisableImageSprites(warmthMeter);
        }

        // Update is called once per frame
        void Update()
        {
            ListenForPauseButton();
            ListenForSceneChanges();
            HandleGameEnding();

            // update value within animator to controll sitting states
            playerAnimator.SetBool("gamePaused", gamePaused);
        }

        public void HandleGameEnding()
        {
            if(gameComplete && canCompleteGame)
            {
                //reset bool
                canCompleteGame = false;

                // - pause game
                gamePaused = true; // stop all funmctions

                // - display the finsih menu screen
                EnableImageSprites(gameCompleteMenu);
                EnableText(gameCompleteMenu);
                eventSystem.SetSelectedGameObject(mainMenuButton); // select menu button

                // - change scene?
            }
        }

        public void ListenForSceneChanges()
        {
            // reset scene/ return to MainMenu
            if(endZoneHandler.reloadScene)
            {
                endZoneHandler.reloadScene = false; // reset bool 
                SceneManager.LoadScene("Test Level2"); // re-load level
            }
        }

        public void ListenForPauseButton()
        {
            // listen for if the pause button is pressed
            if (inputHandler.playerControls.Menu.PauseButton.phase == UnityEngine.InputSystem.InputActionPhase.Performed && canPauseGame)
            {
                canPauseGame = false;
                StartCoroutine(ResetPauseBool());
                Debug.Log("Start button pressed");

                //set the event system to now use the pause menu
                eventSystem.SetSelectedGameObject(resumeButton);

                if (pauseMenuEnabled) //if its paused already, Transition to Unpause
                {
                    DisableImageSprites(pauseMenu);
                    DisableText(pauseMenu);
                    gamePaused = false;
                    pauseMenuEnabled = false;

                    //StartCoroutine(TransitionCamera());
                }
                else // we are pausing the game!
                {
                    EnableImageSprites(pauseMenu);
                    EnableText(pauseMenu);
                    gamePaused = true;
                    pauseMenuEnabled = true;
                    //pauseCameraTransition = true; // change transition type of camera
                }
            }
        }

        #region Disable & Enable sprites and text functions
        //function to disable a passed in gameobjects SPRITES
        // for use on menus and buttons for UI etc.
        public void DisableImageSprites(GameObject menu)
        {
            Image[] sprites = menu.GetComponentsInChildren<Image>();

            foreach (Image sprite in sprites)
            {
                sprite.enabled = false;
            }
        }

        //function to disable a passed in gameobjects TEXT
        // for use on menus and buttons for UI etc.
        public void DisableText(GameObject menu)
        {
            TMP_Text[] texts = menu.GetComponentsInChildren<TMP_Text>();

            foreach (TMP_Text text in texts)
            {
                text.enabled = false;
            }
        }

        //function to enable a passed in gameobjects SPRITES
        // for use on menus and buttons for UI etc.
        public void EnableImageSprites(GameObject menu)
        {
            Image[] sprites = menu.GetComponentsInChildren<Image>();

            foreach (Image sprite in sprites)
            {
                sprite.enabled = true;
            }
        }

        //function to enable a passed in gameobjects TEXT
        // for use on menus and buttons for UI etc.
        public void EnableText(GameObject menu)
        {
            TMP_Text[] texts = menu.GetComponentsInChildren<TMP_Text>();

            foreach (TMP_Text text in texts)
            {
                text.enabled = true;
            }
        }
        #endregion

        //============================BUTTONS===================================
        //function to handlee when the start button is pressed
        public void OnStartButtonPress()
        {
            //game is now unpaused,
            gamePaused = false;
            pauseMenuEnabled = false;
            //hide necessary menu images
            DisableImageSprites(mainMenu);
            DisableText(mainMenu);

            //switch camera lerp style
            StartCoroutine(TransitionCamera());
        }

        public void OnResumeButtonPress()
        {
            gamePaused = false;
            pauseMenuEnabled = false;

            //hide necessary menu images
            DisableImageSprites(pauseMenu);
            DisableText(pauseMenu);
        }

        public void OnReturnButtonPress()
        {
            // disable main menu
            DisableImageSprites(creditMenu);
            DisableText(creditMenu);

            //set the event system to now use the credit menu
            eventSystem.SetSelectedGameObject(startButton);

            // enable credit menu
            EnableImageSprites(mainMenu);
            EnableText(mainMenu);
        }

        public void OnCreditsButtonPress()
        {
            Debug.Log("Clicked CREDITS!");

            // disable main menu
            DisableImageSprites(mainMenu);
            DisableText(mainMenu);

            //set the event system to now use the credit menu
            eventSystem.SetSelectedGameObject(returnButton);

            // enable credit menu
            EnableImageSprites(creditMenu);
            EnableText(creditMenu);
        }

        public void OnMainMenuButtonPress()
        {
            //reset scene? can do that>?
            //SceneManager.LoadScene("Test Level2");
            endZoneHandler.reloadScene = true;

            Debug.Log("pressed MAIN MENU !");
        }

        public void OnQuitButtonPress()
        {
            // quit game logic
            Debug.Log("pressed QUIT !");
            Application.Quit();
        }


        // IENumerators.....
        IEnumerator TransitionCamera()
        {
            yield return new WaitForSeconds(1.25f);
            pauseCameraTransition = false;
        }

        IEnumerator ResetPauseBool()
        {
            //sneakFlag = !sneakFlag;
            yield return new WaitForSeconds(0.25f);
            canPauseGame = true;
        }
    }
}
