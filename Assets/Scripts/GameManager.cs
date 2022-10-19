using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        //need reference to all UI elements to be able to loop through image components to enable and disable sprites..
        public GameObject mainMenu;
        public GameObject pauseMenu;
        public GameObject healthBar;
        public GameObject warmthMeter;
        public GameObject reticle;

        void Start()
        {
            canPauseGame = true;
            gamePaused = true;
            pauseMenuEnabled = false;
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
            inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<InputHandler>();

            //disable all menu options that are NOT the main menu
            DisableImageSprites(pauseMenu);
            DisableText(pauseMenu);
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

                if (pauseMenuEnabled)
                {
                    DisableImageSprites(pauseMenu);
                    DisableText(pauseMenu);
                    gamePaused = false;
                    pauseMenuEnabled = false;
                }
                else
                {
                    EnableImageSprites(pauseMenu);
                    EnableText(pauseMenu);
                    gamePaused = true;
                    pauseMenuEnabled = true;
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
        }

        IEnumerator ResetPauseBool()
        {
            //sneakFlag = !sneakFlag;
            yield return new WaitForSeconds(0.25f);
            canPauseGame = true;
        }
    }
}
