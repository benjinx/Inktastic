using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
  
    #region Fields

    [Tooltip("Retain this manager through later scenes (use if you want music fading.)")]
    public bool dontDestroyOnLoad = true;

    public AudioMixerGroup masterMixer, musicMixer, sfxMixer;


    #endregion

    #region Properties

    //[HideInInspector]
    public AudioList currentAudioSet;

    private SoundClip currentMusicPlaying;

    public List<SoundClip> currentSoundList = new List<SoundClip>();

    private List<SoundClip> currentMusicList = new List<SoundClip>();

    #endregion

    #region Unity Methods

    new void Awake()
    {
        base.Awake();

        if (dontDestroyOnLoad && Instance == this)
        {
            DontDestroyOnLoad(this);
        }

    }


    #endregion

    #region Implementation Methods



    /// <summary>Loads new set of audio(Music and SFX) into the manager, fades music if applicable (think cassette tape).
    /// </summary>
    /// <param name="_audioSet"> Audio Set to Load </param>
    /// <param name="fadeMusic"> Fade Music On Swap </param>
    public void LoadAudioSet(AudioList _audioList, bool fadeMusic)
    {
        SoundClip previousMusic = currentMusicPlaying;

        if (_audioList == null)
        {
            Debug.LogError("No Audio List Loaded!");
            return;
        }

        currentAudioSet = _audioList;

        ClearMusicList();



      /*  if (_audioList.music.Count > 0)
        {
            foreach (SoundClip sound in _audioList.music)
            {
                currentMusicList.Add(sound);
            }

            ShuffleSoundList(currentMusicList.ToArray());
        }*/

        //add exception for old music
        ClearAudioSources(previousMusic);
        ClearSoundList();
        PopulateSoundList();
        CreateAudioSources();


        SoundClip[] musicList = currentMusicList.ToArray();

        if (fadeMusic)
        {
            TransitionSound(previousMusic, musicList[0]);
        }

        PlayMusic();

    }




    /// <summary>Loads the music specified in the Audio Set.
    /// </summary>
    public void PlayMusic()
    {

        if (!(currentMusicList.Count > 0))
        {
            return;
        }

        SoundClip[] musicList = currentMusicList.ToArray();
        currentMusicPlaying = musicList[0];

        Play(musicList[0].name);
    }



    /// <summary>Plays specific sound in Audio Set by friendly name.
    /// </summary>
    /// <param name="_name"> SoundClip to Load </param>
    public void Play(string _name)
    {
        //needs to determine if its pooled, only find it once then ask the object pooler to pick one and play

        SoundClip s = null;

        foreach (SoundClip sound in currentSoundList)
        {
            if (sound.name == _name)
            {
                s = sound;
            }
        }

        if (s == null)
        {
            Debug.LogWarning("SoundClip: " + _name + " not found!");
            return;
        }


     /*   if (s.poolAudio)
        {
            AudioSource source = s.GetRandomSource();
            source.clip = s.GetRandomVariant();
            source.Play();
            return;

        }*/

        GetNewAudioSourceVariant(s);
        s.source.Play();
        //Debug.Log("playing " + s.source.clip.name);
    }

    /// <summary>Stops specific sound in Audio Set by friendly name.
    /// </summary>
    /// <param name="_name"> SoundClip to stop </param>
    public void StopSound(string _name)
    {
        SoundClip s = null;

        foreach (SoundClip sound in currentSoundList)
        {
            if (sound.name == _name)
            {
                s = sound;
            }
        }

        if (s == null)
        {
            Debug.LogWarning("SoundClip: " + _name + " not found!");
            return;
        }

/*
        if (s.poolAudio)
        {
            foreach (AudioSource source in s.pooledSources)
            {
                source.Stop();
            }

        }*/

        s.source.Stop();
    }


    /// <summary>Fades two sounds together in and out.
    /// </summary>
    /// <param name="sound1"> SoundClip to fade out </param>
    /// <param name="sound2"> SoundClip to fade in </param>
    public void TransitionSound(SoundClip sound1, SoundClip sound2)
    {
        if (sound1 == null || sound2 == null)
        {
            return;
        }

        GetAudioSourceWithSound(sound1).DOFade(0, 2f);
        GetAudioSourceWithSound(sound2).volume = 0;
        GetAudioSourceWithSound(sound2).DOFade(sound2.volume, 2f);
    }


    /// <summary>Plays specific sound in Audio Set by friendly name in 3D space, if its already flagged as 3D it will use the object already made, if not it will create a temporary one.
    /// </summary>
    /// <param name="_name"> SoundClip to Load </param>
    /// <param name="position"> position to play in </param>
    public void PlayAtPosition(string _name, Vector3 position)
    {
        //find the object created for sound, if there isnt one then make one 

        SoundClip s = new SoundClip();

        foreach (SoundClip sound in currentSoundList)
        {
            if (sound.name == _name)
            {
                s = sound;
            }
        }

        if (s == null)
        {
            Debug.LogWarning("SoundClip: " + _name + " not found!");
            return;
        }

   /*     if (s.threeDAudio)
        {
            if (s.source.gameObject != this.gameObject)
            {
                s.source.transform.position = position;
                s.source.gameObject.SetActive(true);
                s.source.Play();
                GetNewAudioSourceVariant(s);
            }
        }
        else
        {
            //make temporary 3D game object for this one use
            AudioSource oldSource = s.source;
            var obj = new GameObject();
            SetupAudioObject(s, obj);
            s.source.gameObject.SetActive(true);
            s.source.Play();
            s.source.volume = s.volume;
            GetNewAudioSourceVariant(s);
            //add a destroy timer to the new object so it doesnt crowd
            s.source = oldSource;
            StartCoroutine(TimeDestroyObject(obj, s.clip.length + 2.0f));
        }*/

    }


    #endregion

    #region Utility Methods

    /// <summary>Clears the current list of music and prepares it for a new one.
    /// </summary>
    public void ClearMusicList()
    {
        if (currentMusicList != null)
        {
            currentMusicList.Clear();
        }
    }





    /// <summary>Creates list of all sounds loaded from the audio set (Music and SFX).
    /// </summary>
    public void PopulateSoundList()
    {
        if (currentAudioSet != null)
        {
            List<SoundClip> allSounds = new List<SoundClip>();

        /*    foreach (SoundClip sfx in currentAudioSet.sfx)
            {
                allSounds.Add(sfx);
            }
            foreach (SoundClip music in currentAudioSet.music)
            {
                allSounds.Add(music);
            }*/

            foreach (SoundClip sound in allSounds)
            {
                currentSoundList.Add(sound);
            }

        }
        else
        {
            Debug.LogError("No AudioSet Loaded!");
            return;
        }


    }

    /// <summary>Clears total sound list and prepares it for a new one.
    /// </summary>
    public void ClearSoundList()
    {
        currentSoundList.Clear();
    }

    /// <summary>Creates Audio Source components for each sound loaded in the Audio Set.
    /// </summary>
    public void CreateAudioSources()
    {
        foreach (SoundClip sound in currentSoundList)
        {
            //if pool audio OR 3d audio
          /*  if (sound.poolAudio)
            {
                sound.pooledSources.Clear();

                Debug.Log("sound needs to be pooled");

                //create new parent with children of pooled audio
                GameObject parent = new GameObject();
                parent.transform.parent = this.transform;
                parent.transform.name = sound.name;

                for (int i = 0; i < sound.poolSize; i++)
                {
                    GameObject child = new GameObject();
                    child.transform.parent = parent.transform;
                    child.AddComponent<AudioSource>();
                    child.name = sound.name + " " + i;
                    sound.pooledSources.Add(child.GetComponent<AudioSource>());

                    SetupAudioSource(child.GetComponent<AudioSource>(), sound);
                    sound.source = child.GetComponent<AudioSource>();
                    //sound.source = child.GetComponent<AudioSource>();
                }


            }
            else
            {
                if (sound.threeDAudio)
                {
                    SetupAudioObject(sound, new GameObject());
                }
                else
                {
                    //create children

                    GameObject child = new GameObject();
                    child.transform.parent = this.transform;
                    child.AddComponent<AudioSource>();
                    child.name = sound.name;
                    sound.source = child.GetComponent<AudioSource>();

                    SetupAudioSource(sound.source, sound);
                }

            }*/



        }

    }



    /// <summary>Takes in a GameObject and sets it up to play audio.
    /// </summary>
    /// <param name="sound"> SoundClip for the audio source</param>
    /// <param name="obj"> GameObject to set up</param>
    public void SetupAudioObject(SoundClip sound, GameObject obj)
    {
        // GameObject audioObj = new GameObject();

        obj.AddComponent<AudioSource>();

        obj.SetActive(false);

        AudioSource source = obj.GetComponent<AudioSource>();

        sound.source = source;

        SetupAudioSource(source, sound);

        obj.name = sound.name;
    }

    /// <summary>Takes in an AudioSource and sets it up based off of a SoundClip class.
    /// </summary>
    /// <param name="source"> AudioSource to set up</param>
    /// <param name="sound"> SoundClip for the audio source</param>
    public void SetupAudioSource(AudioSource source, SoundClip sound)
    {
        //Debug.Log("set up audio source for: " + sound.clip.name);
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;

        if (musicMixer && sfxMixer != null)
        {
            if (sound.audioChannel == SoundClip.AudioChannel.Music)
            {
                source.outputAudioMixerGroup = musicMixer;
            }
            else
            {
                source.outputAudioMixerGroup = sfxMixer;
            }
        }

     /*   if (sound.threeDAudio)
        {
            source.spatialBlend = 1.0f;
        }*/
    }

    /// <summary>Reseeds the audio source with a new sound variant
    /// </summary>
    /// <param name="_name"> SoundClip with source to reference </param>
    public void GetNewAudioSourceVariant(SoundClip sound)
    {
        if (sound.source != null)
        {
            sound.source.clip = sound.GetRandomVariant();
            //Debug.Log("new clip is " + sound.clip);
        }
    }

    /// <summary>Clears audio source components to prepare for transition with an optional exception of a specific sound.
    /// </summary>
    /// <param name="exception"> SoundClip to not get destroyed right away (destroys in 10 seconds, leave null if not applicable) </param>
    public void ClearAudioSources(SoundClip exception)
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();

        foreach (AudioSource source in sources)
        {
            if (exception != null)
            {
                if (exception.source != source)
                {
                    Destroy(source.gameObject);
                }
                else
                {
                    StartCoroutine(TimeDestroySource(exception.source, 10f));
                    //time destroy source
                }
            }
            else
            {
                Destroy(source.gameObject);
            }

        }
    }

    /// <summary>
    /// find a specific audio source currently attached to this object based on a sound object.
    /// </summary>
    /// <param name="sound">SoundClip to match to audio source</param>
    /// <returns></returns>
    public AudioSource GetAudioSourceWithSound(SoundClip sound)
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();

        AudioSource returnSource = null;

        foreach (AudioSource src in sources)
        {
            if (src.clip == sound.clip)
            {
                returnSource = src;
            }
        }

        return returnSource;

    }

    public SoundClip GetSoundByName(string _name)
    {
        SoundClip s = null;

        foreach (SoundClip sound in currentSoundList)
        {
            if (sound.name == _name)
            {
                s = sound;
            }
        }

        if (s == null)
        {
            Debug.LogWarning("SoundClip: " + _name + " not found!");
            return null;
        }

        return s;
    }

    /// <summary>Destroys an audio source after a specific time.
    /// </summary>
    /// <param name="source"> Source to not get destroyed right away</param>
    /// <param name="time"> Time to delay</param>
    private IEnumerator TimeDestroySource(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(source);
    }

    private IEnumerator TimeDestroyObject(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(obj);
    }

    /// <summary>Shuffles array of sounds using Fisher Yates allegorithm.
    /// </summary>
    /// <param name="_sounds"> Array to shuffle </param>
    public void ShuffleSoundList(SoundClip[] _sounds)
    {

        // Loops through array
        for (int i = _sounds.Length - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = UnityEngine.Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            SoundClip temp = _sounds[i];

            // Swap the new and old values
            _sounds[i] = _sounds[rnd];
            _sounds[rnd] = temp;
        }

        // Print
        for (int i = 0; i < _sounds.Length; i++)
        {
            Debug.Log(_sounds[i]);
        }
    }

    #endregion
}
