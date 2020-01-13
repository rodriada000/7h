﻿using System;

namespace _7thHeaven.Code
{
    [Serializable]
    public class LaunchSettings
    {
        public bool AutoMountGameDisc { get; set; }
        public bool AutoUnmountGameDisc { get; set; }
        public bool AutoUpdateDiscPath { get; set; }

        public bool Code5Fix { get; set; }
        public bool HighDpiFix { get; set; }
        public bool DisableReunionOnLaunch { get; set; }

        public bool ReverseSpeakers { get; set; }
        public bool LogarithmicVolumeControl { get; set; }
        public Guid SelectedSoundDevice { get; set; }
        public string SelectedMidiDevice { get; set; }
        public int SoundVolume { get; set; }
        
        public int SelectedRenderer { get; set; }
        public bool UseRiva128GraphicsOption { get; set; }
        public bool UseTntGraphicsOption { get; set; }

        public static LaunchSettings DefaultSettings()
        {
            return new LaunchSettings()
            {
                AutoMountGameDisc = true,
                AutoUnmountGameDisc = true,
                AutoUpdateDiscPath = true,
                Code5Fix = false,
                HighDpiFix = false,
                DisableReunionOnLaunch = true,
                SelectedSoundDevice = Guid.Empty,
                ReverseSpeakers = false,
                LogarithmicVolumeControl = false,
                SelectedMidiDevice = "GENERAL_MIDI",
                SelectedRenderer = 3,
                SoundVolume = 100,
                UseRiva128GraphicsOption = false,
                UseTntGraphicsOption = false
            };
        }
    }
}