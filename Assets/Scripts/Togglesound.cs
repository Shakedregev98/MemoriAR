using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Togglesound : MonoBehaviour
{
    public Button soundButton;
    private bool isMuted = false;
    public AudioSource ambientSound;
    public Sprite volumeOnSprite;  // Set this in the Inspector
    public Sprite volumeOffSprite; // Set this in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Assuming you want the sound to be on when the game starts
        ambientSound.mute = false;
        soundButton.image.sprite = volumeOnSprite;
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any update logic here if needed
    }

    public void ToggleSound()
    {
        isMuted = !isMuted; // Toggle the mute state

        ambientSound.mute = isMuted; // Apply the mute state to the AudioSource

        // Change the button sprite based on the mute state
        soundButton.image.sprite = isMuted ? volumeOffSprite : volumeOnSprite;
    }
}
