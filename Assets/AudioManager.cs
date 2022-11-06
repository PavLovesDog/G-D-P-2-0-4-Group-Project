using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class AudioManager : MonoBehaviour
    {
        GameManager gameManager;

        // References to all audio played in-game
        public AudioSource audioSource; // place to play from
        public AudioSource projectileHitSource;
        public AudioSource physicalHitSource;
        public AudioSource companionHitSource;
        public AudioSource wolfPursuit;
        public AudioSource menuMusicSource;
        public AudioSource overworldSource;

        public AudioClip footsteps; // sound to play
        public AudioClip jump;
        public AudioClip land;
        public AudioClip throwItem;
        public AudioClip impact;
        public AudioClip impact_2;
        public AudioClip angryWolf;
        public AudioClip wolfDeath;
        public AudioClip menuMusic;
        public AudioClip overworldSound;

        public float volumeDecreaseAmount = 0.1f;

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Function to call when another script needs to play a sound
        // takes in the audio play source, an audioclip and volume as float value
        public void PlayAudio(AudioSource source, AudioClip sound, float volume)
        {
            source.clip = sound;
            source.pitch = Random.Range(0.75f, 1f);
            source.volume = volume;
            source.PlayOneShot(sound);
        }

        public void StopSound(AudioSource source)
        {
            source.Stop();
        }

        public void IncreaseAudioSourceVolume(AudioSource source, float volume)
        {
            if(gameManager.startMusic)
            {
                source.Play();
                gameManager.startMusic = false;
            }

            source.volume += volumeDecreaseAmount * Time.deltaTime;
            source.volume = Mathf.Clamp(source.volume, 0f, volume); // clamp it

            if(source.volume >= 0.1f)
            {

                if (source.volume >= volume)
                    gameManager.increaseMusic = false;
            }
        }

        public void DecreaseAudioSourceVolume(AudioSource source)
        {
            source.volume -= volumeDecreaseAmount * Time.deltaTime;

            if(source.volume <= 0)
            {
                source.Stop();
                gameManager.decreaseMusic = false;
            }
        }

        //// function to Stop the game music and atmosphere sounds
        //public void StopOverworldAudio()
        //{
        //    overworldMusic.Stop();
        //    atmosphereSound.Stop();
        //}
        //
        //// function to Play the game music and atmosphere sounds
        //public void PlayOverworldAudio()
        //{
        //    overworldMusic.Play();
        //    atmosphereSound.Play();
        //}
    }
}
