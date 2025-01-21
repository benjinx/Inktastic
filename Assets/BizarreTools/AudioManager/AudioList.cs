using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSet", menuName = "Data/AudioSet", order = 1)]
public class AudioList : ScriptableObject
{

    #region Fields

    public List<SoundClip> sounds;

    private SoundClip lastClip;

    public SoundClip GetSoundClipByName(string _name)
    {
        SoundClip[] clipArray = sounds.ToArray();

        if (sounds == null || clipArray.Length == 0)
        {
            Debug.LogError("No audio clips available.");
            return null;
        }

        for (int i = 0; i < clipArray.Length; i++)
        {
            if (clipArray[i].name == _name)
            {
                return clipArray[i];
            }
        }

        return null;

    }

    public SoundClip GetRandomClip()
    {
        SoundClip[] clipArray = sounds.ToArray();

        if (sounds == null || clipArray.Length == 0)
        {
            Debug.LogError("No audio clips available.");
            return null;
        }

        // Perform Fisher-Yates shuffle
        System.Random rng = new System.Random();
        for (int i = clipArray.Length - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(i + 1);
            SoundClip temp = clipArray[i];
            clipArray[i] = clipArray[randomIndex];
            clipArray[randomIndex] = temp;
        }

        SoundClip selectedClip;
        do
        {
            selectedClip = clipArray[rng.Next(clipArray.Length)];
        } while (selectedClip == lastClip && clipArray.Length > 1);

        lastClip = selectedClip;
        return selectedClip;
    }
    #endregion

}
