using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClip
{

    [Tooltip("Friendly music name")]
    public string name;

    [Tooltip("Audio Clip to load")]
    public AudioClip clip;

    public bool useVariants;

    public AudioClip[] clipVariants;

    [Tooltip("Volume of clip")]
    [Range(0f, 2f)]
    public float volume = 1f;

    [Tooltip("Pitch of clip")]
    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool usePitchVariation;
    public float minPitch = 1f;
    public float maxPitch = 1.5f;

    [Tooltip("Loop toggle")]
    public bool loop;

    [HideInInspector()]
    public AudioSource source;

    [HideInInspector()]
    public List<AudioSource> pooledSources = new List<AudioSource>();

    [Tooltip("Audio Mixer Group")]
    public AudioChannel audioChannel;

    private AudioClip lastClip;

    public enum AudioChannel
    {
        Music,
        SFX,
        Dialogue
    }

    public AudioClip GetAudioClip()
    {
        AudioClip loadedClip = null;

        if (useVariants)
        {
            loadedClip = GetRandomVariant();
        }
        else
        {
            loadedClip = clip;
        }

        return loadedClip;
    }

    public AudioClip GetRandomVariant()
    {
        if (clipVariants == null || clipVariants.Length == 0)
        {
            Debug.LogError("No audio clips available.");
            return null;
        }

        // Perform Fisher-Yates shuffle
        System.Random rng = new System.Random();
        for (int i = clipVariants.Length - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(i + 1);
            AudioClip temp = clipVariants[i];
            clipVariants[i] = clipVariants[randomIndex];
            clipVariants[randomIndex] = temp;
        }

        AudioClip selectedClip;
        do
        {
            selectedClip = clipVariants[rng.Next(clipVariants.Length)];
        } while (selectedClip == lastClip && clipVariants.Length > 1);

        lastClip = selectedClip;
        return selectedClip;
    }
}

