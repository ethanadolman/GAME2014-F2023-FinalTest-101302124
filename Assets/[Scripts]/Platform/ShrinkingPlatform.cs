using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingPlatform : MonoBehaviour
{
    public float amplitude = 0.5f;  // Adjust the amplitude of the floating motion
    public float frequency = 1f;    // Adjust the frequency of the floating motion
    public float shrinkSpeed = 1.0f; // Adjust the speed of the shrinking
    public float activationDelay = 2.0f; // Delay before the platform starts shrinking after activation
    public float resetDelay = 2.0f; // Delay before the platform resets to its original size after the player leaves
    public AudioClip shrinkSound; // Assign the audio clip in the Unity Editor

    private Vector2 initialPosition;
    private bool activated = false;
    private AudioSource audioSource;
    private float timeSinceLastActivation;

    private void Start()
    {
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (activated)
        {
            FloatAndShrinkPlatform();
        }
        else
        {
            FloatPlatform();
            ResetPlatform();
        }
    }

    void FloatPlatform()
    {
        // Calculate the floating motion using a sine wave
        float yOffset = amplitude * Mathf.Sin(frequency * Time.time);
        transform.position = initialPosition + new Vector2(0f, yOffset);
    }

    void FloatAndShrinkPlatform()
    {
        // Calculate the floating motion using a sine wave
        float yOffset = amplitude * Mathf.Sin(frequency * Time.time);
        transform.position = initialPosition + new Vector2(0f, yOffset);

        // Shrink the platform over time
        Vector3 newScale = transform.localScale;
        newScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
        newScale = Vector3.Max(newScale, Vector3.zero); // Ensure the scale doesn't go negative
        transform.localScale = newScale;

        // Play the shrinking sound
        if (shrinkSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(shrinkSound);
        }

        // You can add additional logic here, like destroying the platform when it's small enough
        if (newScale == Vector3.zero)
        {
            activated = false;
            timeSinceLastActivation = Time.time;
        }
    }

    void ResetPlatform()
    {
        // Reset the platform to its original size after the player leaves
        if (!activated && Time.time - timeSinceLastActivation > resetDelay)
        {
            Vector3 newScale = transform.localScale;
            newScale += Vector3.one * shrinkSpeed * Time.deltaTime; // Adjust the reset speed
            newScale = Vector3.Min(newScale, Vector3.one); // Ensure the scale doesn't exceed the original size
            transform.localScale = newScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !activated)
        {
            activated = true;
            Invoke("ActivatePlatform", activationDelay);
        }
    }

    void ActivatePlatform()
    {
        // Perform any activation logic here
        // For now, we simply start the shrinking process
        activated = true;
    }
}