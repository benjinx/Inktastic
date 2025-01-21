using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using BizarreTools;
using DG.Tweening;

public class MagesAudioManager : SingletonMonoBehaviour<MagesAudioManager>
{
    //Dictionary of loaded sounds
    //Method for additivley loading audio sets
    //Support for using Object Pooler to pool 3D audio sources
    //Audio set of music tracks
    //Method for blending music

    public AudioList musicLibrary;

    public List<AudioList> generalSFX;

    public AudioMixer mainMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup dialogueMixer;
    public AudioMixerGroup musicMixer;

    public AudioSource musicSource1;
    public AudioSource musicSource2;

    public float musicTransitionThreshold = 10f;
    public float musicStopFadeoutTime = 4f;
    public float musicStartFadeTime = 3f;

    public ObjectPooler.ObjectPool posSourcePool;

    public enum MixerType
    {
        Main,
        SFX,
        Music,
        Dialogue
    }

    private Dictionary<string, SoundClip> loadedClips = new Dictionary<string, SoundClip>();
    private bool source1Active;
    private bool musicActive;
    private float currentMusicTime;
    private float currentMusicDuration;
    //Pool a generic list of audio source objects and setup the source when you need it
    //use timer component to return object to pool after clip duration
    //Pool list of 2D audio sources 

    new void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        InitializeAudioManager();
    }

    private void Update()
    {
        if (musicActive)
        {
            currentMusicTime += Time.deltaTime;
            if(currentMusicTime >= (currentMusicDuration - musicTransitionThreshold))
            {
                TransitionMusic();
            }
        }
    }

    public void InitializeAudioManager()
    {

        if(ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.InstantiatePool(posSourcePool, this.gameObject);
        }
        else
        {
            Debug.LogError("ERROR! No Object Pooler found! Audio Manager will not function.");
        }

        for (int i = 0; i < generalSFX.Count; i++)
        {
            AppendAudioList(generalSFX[i]);
        }

        InitializeMusic();
    }

    public void InitializeMusic()
    {
        if (musicActive)
        {
            return;
        }

        musicSource1.clip = musicLibrary.GetRandomClip().GetAudioClip();
        musicSource1.Play();
        musicSource2.Stop();
        musicSource2.volume = 0;
        musicSource1.volume = 0;
        musicSource1.DOFade(1, musicStartFadeTime);
        musicActive = true;
        source1Active = true;
        currentMusicTime = 0;
        currentMusicDuration = musicSource1.clip.length;
    }

    public void SetMixerVolume(MixerType mixerType, float _volume)
    {
        switch (mixerType)
        {
            case MixerType.Main:
                mainMixer.SetFloat("mainVol", LinearToDecibel(_volume));
                break;
            case MixerType.Music:
                mainMixer.SetFloat("musicVol", LinearToDecibel(_volume));
                break;
            case MixerType.SFX:
                mainMixer.SetFloat("sfxVol", LinearToDecibel(_volume));
                break;
            case MixerType.Dialogue:
                //TODO: Implement Dialogue mixer
                break;
        }
    }

    float LinearToDecibel(float linear)
    {
        // Handle the case when linear is 0 to avoid taking the log of zero
        if (linear <= 0.0001f)
        {
            return -80.0f; // Minimum dB value, representing silence
        }

        return 20.0f * Mathf.Log10(linear);
    }

    public void TransitionMusic()
    {
        AudioSource newAudio = source1Active ? musicSource2 : musicSource1;
        AudioSource oldAudio = source1Active ? musicSource1 : musicSource2;

        InitializeAudioSource(musicLibrary.GetRandomClip(), newAudio);
        newAudio.volume = 0;
        newAudio.DOFade(1, musicTransitionThreshold);
        oldAudio.DOFade(0, musicTransitionThreshold);
        newAudio.Play();
        source1Active = !source1Active;
        currentMusicTime = 0;
        currentMusicDuration = newAudio.clip.length;

    }

    public void TransitionMusic(SoundClip _music)
    {
        AudioSource newAudio = source1Active ? musicSource2 : musicSource1;
        AudioSource oldAudio = source1Active ? musicSource1 : musicSource2;

        InitializeAudioSource(_music, newAudio);
        newAudio.volume = 0;
        newAudio.DOFade(1, musicTransitionThreshold);
        oldAudio.DOFade(0, musicTransitionThreshold);
        newAudio.Play();
        source1Active = !source1Active;
        currentMusicTime = 0;
        currentMusicDuration = newAudio.clip.length;

    }

    public void StopMusic()
    {
        musicActive = false;
        musicSource1.DOFade(0, musicStopFadeoutTime).onComplete += ForceMusicVolumeZero;
        musicSource2.DOFade(0, musicStopFadeoutTime).onComplete += ForceMusicVolumeZero;
        currentMusicTime = 0;
    }

    public void ForceMusicVolumeZero()
    {
        musicSource1.volume = 0; 
        musicSource2.volume = 0;
    }

    public void InitializeAudioSource(SoundClip _clip, AudioSource _source)
    {
        _source.clip = _clip.GetAudioClip();
        _source.volume = _clip.volume;

        if (_clip.usePitchVariation)
        {
            _source.pitch = Random.Range(_clip.minPitch, _clip.maxPitch);
        }
        else
        {
            _source.pitch = _clip.pitch;
        }
        _source.loop = _clip.loop;

        switch (_clip.audioChannel)
        {
            case SoundClip.AudioChannel.Music:
                _source.outputAudioMixerGroup = musicMixer;
                break;
            case SoundClip.AudioChannel.SFX:
                _source.outputAudioMixerGroup = sfxMixer;
                break;
            case SoundClip.AudioChannel.Dialogue:
                _source.outputAudioMixerGroup = dialogueMixer;
                break;
            default:
                break;
        }
    }

    public void AppendAudioList(AudioList _audioList)
    {
        for (int i = 0; i < _audioList.sounds.Count; i++)
        {
            if (loadedClips.ContainsKey(_audioList.sounds[i].name))
            {
                continue;
            }

            loadedClips.Add(_audioList.sounds[i].name, _audioList.sounds[i]);
        }
    }

    public void PlayClip(string _tag)
    {
        if (loadedClips.ContainsKey(_tag))
        {
            AudioSource source = ObjectPooler.Instance.SpawnFromPool(posSourcePool.tag, this.transform.position, this.transform.rotation).GetComponent<AudioSource>();
            InitializeAudioSource(loadedClips[_tag], source);
            if (!source.loop)
            {
                source.GetComponent<Timer>().ResetTime();
                source.GetComponent<Timer>().duration = source.clip.length;
                source.GetComponent<Timer>().EnableTimer();
            }
            source.spatialBlend = 0f;
            source.Play();
        }
        else
        {
            Debug.LogError("COULD NOT FIND LOADED CLIP: " + _tag);
        }
    }

    public void PlayClipAtPosition(string _tag, Vector3 _pos)
    {
        if (loadedClips.ContainsKey(_tag))
        {
            AudioSource source = ObjectPooler.Instance.SpawnFromPool(posSourcePool.tag, _pos, Quaternion.identity).GetComponent<AudioSource>();
            InitializeAudioSource(loadedClips[_tag], source);
            if (!source.loop)
            {
                source.GetComponent<Timer>().ResetTime();
                source.GetComponent<Timer>().duration = source.clip.length;
                source.GetComponent<Timer>().EnableTimer();
            }
            source.spatialBlend = 1.0f;
            source.Play();
        }
        else
        {
            Debug.LogError("COULD NOT FIND LOADED CLIP: " + _tag);
        }
    }


}
