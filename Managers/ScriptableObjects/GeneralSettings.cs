using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace Devdog.General
{
    [CreateAssetMenu(menuName = "Devdog/General Settings")]
    public class GeneralSettings : ScriptableObject
    {
        [Header("Cursor")]
        [Category("Cursor")]
        public CursorIcon defaultCursor;

        [Header("Triggers")]
        [Category("Triggers")]
        public float triggerUseDistance = 10f;

        [Header("Logging")]
        [Category("Logging")]
        public DevdogLogger.LogType minimalLogType;

        // [Header("Audio")]
        // [Category("Audio")]
        // public int reserveAudioSources = 8;
        // public AudioMixerGroup audioMixerGroup;

        [Category("Editor & Testing")]
        [Header("Testing")]
        public bool useExceptionsForAssertions = false;
    }
}
