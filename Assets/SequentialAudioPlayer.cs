using System.Collections;
using UnityEngine;

public class SequentialAudioPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    public float crossfadeDuration = 1.0f; // Set the duration of the crossfade
    public bool loopSecondClip = true; // Set this to true if you want the second clip to loop
    private AudioSource audioSource;
    private int currentIndex = 0;
    private float crossfadeTimer = 0.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Check if there are audio clips assigned
        if (audioClips.Length > 0)
        {
            // Start playing the first audio clip
            audioSource.clip = audioClips[currentIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No audio clips assigned.");
        }
    }

    void Update()
    {
        // Check if the current clip has finished playing
        if (!audioSource.isPlaying)
        {
            // Move to the next clip
            currentIndex = (currentIndex + 1) % audioClips.Length;

            // If it's the second clip and looping is enabled, play it in a loop
            if (currentIndex == 1 && loopSecondClip)
            {
                audioSource.clip = audioClips[currentIndex];
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                // Crossfade to the next audio clip
                StartCoroutine(CrossfadeToNextClip());
            }
        }
    }

    IEnumerator CrossfadeToNextClip()
    {
        // Set up variables for crossfade
        AudioSource nextAudioSource = gameObject.AddComponent<AudioSource>();
        nextAudioSource.clip = audioClips[currentIndex];
        nextAudioSource.volume = 0.0f;
        nextAudioSource.Play();

        while (crossfadeTimer < crossfadeDuration)
        {
            // Adjust the volume of the current and next clips over time
            audioSource.volume = Mathf.Lerp(1.0f, 0.0f, crossfadeTimer / crossfadeDuration);
            nextAudioSource.volume = Mathf.Lerp(0.0f, 1.0f, crossfadeTimer / crossfadeDuration);
            crossfadeTimer += Time.deltaTime;

            yield return null;
        }

        // Destroy the temporary AudioSource
        Destroy(nextAudioSource);

        // Reset variables for the next iteration
        crossfadeTimer = 0.0f;

        // Play the next audio clip
        audioSource.clip = audioClips[currentIndex];
        audioSource.volume = 1.0f;
        audioSource.loop = false;
        audioSource.Play();
    }
}
