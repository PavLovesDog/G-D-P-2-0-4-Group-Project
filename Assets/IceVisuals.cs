using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MBF
{
    public class IceVisuals : MonoBehaviour
    {
        PlayerStats playerStats;
        public Image iceImage;
        public float alpha;

        void Start()
        {
            playerStats = FindObjectOfType<PlayerStats>();
            iceImage = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (playerStats.currentColdAmount < 65f)
            {
                alpha = Mathf.InverseLerp(0.65f, 0, playerStats.currentColdAmount / 100);

                //link image alpha to playerstats curreent cold amount
                iceImage.color = new Color(iceImage.color.r, iceImage.color.g, iceImage.color.b, alpha);
            }
        }
    }
}
