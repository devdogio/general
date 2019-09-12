using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace Devdog.General
{
    public partial class AudioManager : ManagerBase<AudioManager>
    {
        [Header("Settings")]
        public int reserveAudioSources = 8;
        public AudioMixerGroup audioMixerGroup;

        private static List<AudioSource> _audioSources;
        private static GameObject _audioSourceGameObject;

        private static Queue<AudioClipInfo> _audioQueue = new Queue<AudioClipInfo>();

        protected override void Awake()
        {
            base.Awake();

            StartCoroutine(WaitFramesAndEnable(5));
            enabled = false; // Set to enabled at start, initialize, then enable (to avoid playing sound during initialization)

            _audioQueue = new Queue<AudioClipInfo>(reserveAudioSources);
            CreateAudioSourcePool();
        }

        protected override void Start()
        {
            base.Start();
        }

        // Empty method to show enable / disable icons in Unity inspector.
        private void OnEnable(){ }

        private IEnumerator WaitFramesAndEnable(int frames)
        {
            for(int i = 0; i < frames; i++)
            {
                yield return null;
            }

            enabled = true;
        }

        private void CreateAudioSourcePool()
        {
            _audioSources = new List<AudioSource>(reserveAudioSources);

            _audioSourceGameObject = new GameObject("_AudioSources");
            _audioSourceGameObject.transform.SetParent(transform);
            _audioSourceGameObject.transform.localPosition = Vector3.zero;

            for (int i = 0; i < _audioSources.Count; i++)
            {
                _audioSources.Add(CreateNewAudioSource());
            }
        }

        private static AudioSource CreateNewAudioSource() {
            var src = _audioSourceGameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = instance.audioMixerGroup;
            return src;
        }

        protected void Update()
        {
            if (_audioQueue.Count > 0)
            {
                var source = GetNextAudioSource();
                var clip = _audioQueue.Dequeue();

                source.Play(clip);
            }
        }

        private static AudioSource GetNextAudioSource()
        {
            foreach (var audioSource in _audioSources)
            {
                if (audioSource.isPlaying == false)
                    return audioSource;
            }

            DevdogLogger.LogWarning("All sources taken, creating new on the fly... Consider increasing reserved audio sources");
            
            var src = CreateNewAudioSource();
            _audioSources.Add(src);
            return src;
        }


        /// <summary>
        /// Plays an audio clip, only use this for the UI, it is not pooled so performance isn't superb.
        /// </summary>
        public static void AudioPlayOneShot(AudioClipInfo clip)
        {
            if (clip == null || clip.audioClip == null)
            {
                return;
            }

            if (instance == null)
            {
                DevdogLogger.LogWarning("AudioManager not found, yet trying to play an audio clip....");
            }

            if (_audioQueue.Any(o => o.audioClip == clip.audioClip) == false)
            {
                _audioQueue.Enqueue(clip);
            }
        }
    }
}