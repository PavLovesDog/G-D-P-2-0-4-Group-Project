using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MBF
{
    public class GameManager : MonoBehaviour
    {
        public bool gamePaused;
        bool canPauseGame;
        public bool pauseMenuEnabled;
        Animator playerAnimator;
        InputHandler inputHandler;
        EventSystem eventSystem;

        //need reference to all UI elements to be able to loop through image components to enable and disable sprites..
        public GameObject mainMenu;
        public GameObject pauseMenu;
        public GameObject creditMenu;
        public GameObject healthBar;
        public GameObject warmthMeter;
        public GameObject reticle;

        public GameObject startButton;
        public GameObject resumeButton;
        public GameObject returnButton;

        public bool pauseCameraTransition;
        //float cameraTransitionTimer;

        void Start()
        {
            canPauseGame = true;
            gamePaused = true;
            pauseMenuEnabled = false;
            pauseCameraTransition = true;
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
            inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<InputHandler>();
            eventSystem = GameObject.FindGameObjectWithTag("Event System").GetComponent<EventSystem>();

            //disable all menu options that are NOT the main menu
            DisableImageSprites(pauseMenu);
            DisableText(pauseMenu);

            DisableImageSprites(creditMenu);
            DisableText(creditMenu);

            DisableImageSprites(healthBar);
            DisableImageSprites(warmthMeter);
        }

        // Update is called once per frame
        void Update()
        {
            ListenForPauseButton();

            // update value within animator to controll sitting states
            playerAnimator.SetBool("gamePaused", gamePaused);
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
            Debug.Log("pressed MAIN MENU !");
        }

        public void OnQuitButtonPress()
        {
            // quit game logic
            Debug.Log("pressed QUIT !");
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
