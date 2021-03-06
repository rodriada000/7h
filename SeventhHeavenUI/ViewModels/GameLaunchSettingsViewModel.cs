﻿using _7thHeaven.Code;
using Iros._7th.Workshop;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SeventhHeaven.Classes;
using SeventhHeaven.Windows;
using SeventhHeavenUI;
using SeventhHeavenUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SeventhHeaven.ViewModels
{
    internal enum AudioChannel
    {
        Left,
        Center,
        Right
    }

    internal enum VolumeSlider
    {
        Music,
        Sfx
    }

    class GameLaunchSettingsViewModel : ViewModelBase
    {
        #region Data Members
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private List<string> _defaultControlNames = new List<string>() 
        {
            "1998 KB+Std Gamepad",
            "1998 KB+Swap AB-XO Gamepad",
            "1998 Original",
            "No Numpad+Std Gamepad",
            "No Numpad+Swap AB-XO Gamepad",
            "Steam KB+Std GamePad",
            "Steam KB+Swap AB-XO GamePad",
            "Steam Original",
            "WASD unab0mb's Choice Std",
            "WASD unab0mb's Choice Swap"
        };

        private string _statusMessage;
        private bool _autoMountChecked;
        private bool _autoUnmountChecked;
        private bool _autoUpdatePathChecked;
        private bool _code5FixChecked;
        private bool _highDpiFixChecked;
        private bool _disableReunionChecked;
        private bool _isReunionInstalled;
        private bool _isAutoMountSupported;
        private bool _isReverseSpeakersChecked;
        private bool _isLogVolumeChecked;
        private bool _isRivaOptionChecked;
        private bool _isTntOptionChecked;
        private string _selectedSoundDevice;
        private string _selectedMidiDevice;
        private string _selectedRenderer;
        private int _volumeValue;

        private WaveOut _audioTest;

        private ObservableCollection<ProgramToRunViewModel> _programList;
        private string _newProgramPathText;
        private string _newProgramArgsText;
        private bool _isProgramPopupOpen;
        private int _sfxVolumeValue;
        private bool _isQuarterScreenChecked;
        private bool _isFullScreenChecked;
        private bool _isShowLauncherChecked;
        private string _selectedGameConfigOption;
        private string _importStatusMessage;
        private bool _importButtonIsEnabled;
        private Visibility _importProgressVisibility;
        private double _importProgressValue;
        private VolumeSlider _lastVolumeSliderChanged;

        #endregion


        #region Properties

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsAudioPlaying
        {
            get
            {
                return _audioTest != null && _audioTest.PlaybackState == PlaybackState.Playing;
            }
        }

        public bool IsAudioNotPlaying
        {
            get
            {
                return _audioTest == null;
            }
        }

        public bool IsAutoMountSupported
        {
            get
            {
                return _isAutoMountSupported;
            }
            set
            {
                _isAutoMountSupported = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoMountChecked
        {
            get
            {
                return _autoMountChecked;
            }
            set
            {
                _autoMountChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoUnmountChecked
        {
            get
            {
                return _autoUnmountChecked;
            }
            set
            {
                _autoUnmountChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoUpdatePathChecked
        {
            get
            {
                return _autoUpdatePathChecked;
            }
            set
            {
                _autoUpdatePathChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsShowLauncherChecked
        {
            get
            {
                return _isShowLauncherChecked;
            }
            set
            {
                _isShowLauncherChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasCode5Error
        {
            get
            {
                return Sys.Settings.GameLaunchSettings.HasReceivedCode5Error;
            }
        }

        public bool Code5FixChecked
        {
            get
            {
                return _code5FixChecked;
            }
            set
            {
                _code5FixChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool HighDpiFixChecked
        {
            get
            {
                return _highDpiFixChecked;
            }
            set
            {
                _highDpiFixChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool DisableReunionChecked
        {
            get
            {
                return _disableReunionChecked;
            }
            set
            {
                _disableReunionChecked = value;
                NotifyPropertyChanged();
                ShowWarningMessageAbouReunion();
            }
        }

        public bool IsReunionInstalled
        {
            get
            {
                return _isReunionInstalled;
            }
            set
            {
                _isReunionInstalled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsReverseSpeakersChecked
        {
            get
            {
                return _isReverseSpeakersChecked;
            }
            set
            {
                _isReverseSpeakersChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsLogVolumeChecked
        {
            get
            {
                return _isLogVolumeChecked;
            }
            set
            {
                _isLogVolumeChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsRivaOptionChecked
        {
            get
            {
                return _isRivaOptionChecked;
            }
            set
            {
                _isRivaOptionChecked = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility RivaOptionsVisibility
        {
            get
            {
                if (SelectedRenderer == "Direct3D Hardware Acceleration")
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public bool IsTntOptionChecked
        {
            get
            {
                return _isTntOptionChecked;
            }
            set
            {
                _isTntOptionChecked = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ScreenModesVisibility
        {
            get
            {
                if (SelectedRenderer != "Custom 7H Game Driver")
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public bool IsQuarterScreenChecked
        {
            get
            {
                return _isQuarterScreenChecked;
            }
            set
            {
                _isQuarterScreenChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFullScreenChecked
        {
            get
            {
                return _isFullScreenChecked;
            }
            set
            {
                _isFullScreenChecked = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedSoundDevice
        {
            get
            {
                return _selectedSoundDevice;
            }
            set
            {
                _selectedSoundDevice = value;
                NotifyPropertyChanged();
            }
        }

        public List<string> SoundDevices
        {
            get
            {
                return SoundDeviceGuids?.Keys.ToList();
            }
        }

        public Dictionary<string, Guid> SoundDeviceGuids { get; set; }

        public string SelectedMidiDevice
        {
            get
            {
                return _selectedMidiDevice;
            }
            set
            {
                _selectedMidiDevice = value;
                NotifyPropertyChanged();
            }
        }

        public List<string> MidiDevices
        {
            get
            {
                return MidiDeviceMap?.Keys.ToList();
            }
        }

        public Dictionary<string, string> MidiDeviceMap { get; set; }

        public string SelectedRenderer
        {
            get
            {
                return _selectedRenderer;
            }
            set
            {
                _selectedRenderer = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(RivaOptionsVisibility));
                NotifyPropertyChanged(nameof(ScreenModesVisibility));

                // ensure atleast one radio button is checked when changing renderer
                if (!IsTntOptionChecked && !IsRivaOptionChecked)
                {
                    IsTntOptionChecked = true;
                }

                if (!IsQuarterScreenChecked && !IsFullScreenChecked)
                {
                    IsFullScreenChecked = true;
                }

                ShowWarningMessageAboutRenderer();
            }
        }

        public List<string> AvailableRenderers
        {
            get
            {
                return RendererMap?.Keys.ToList();
            }
        }

        public Dictionary<string, int> RendererMap { get; set; }

        public int MusicVolumeValue
        {
            get
            {
                return _volumeValue;
            }
            set
            {
                _volumeValue = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(MusicVolumeDisplayText));
                LastVolumeSliderChanged = VolumeSlider.Music;

                if (IsAudioPlaying && LastVolumeSliderChanged == VolumeSlider.Music)
                {
                    _audioTest.Volume = (float)_volumeValue / (float)100.0;
                }
            }
        }

        public int SfxVolumeValue
        {
            get
            {
                return _sfxVolumeValue;
            }
            set
            {
                _sfxVolumeValue = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SfxVolumeDisplayText));
                LastVolumeSliderChanged = VolumeSlider.Sfx;

                if (IsAudioPlaying && LastVolumeSliderChanged == VolumeSlider.Sfx)
                {
                    _audioTest.Volume = (float)_sfxVolumeValue / (float)100.0;
                }
            }
        }

        public string SfxVolumeDisplayText
        {
            get
            {
                return $"Volume: {SfxVolumeValue}";
            }
        }

        public string MusicVolumeDisplayText
        {
            get
            {
                return $"Volume: {MusicVolumeValue}";
            }
        }

        public bool IsProgramPopupOpen
        {
            get
            {
                return _isProgramPopupOpen;
            }
            set
            {
                _isProgramPopupOpen = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ProgramToRunViewModel> ProgramList
        {
            get
            {
                if (_programList == null)
                    _programList = new ObservableCollection<ProgramToRunViewModel>();

                return _programList;
            }
            set
            {
                _programList = value;
                NotifyPropertyChanged();
            }
        }

        public string NewProgramPathText
        {
            get
            {
                return _newProgramPathText;
            }
            set
            {
                _newProgramPathText = value;
                NotifyPropertyChanged();
            }
        }

        public string NewProgramArgsText
        {
            get
            {
                return _newProgramArgsText;
            }
            set
            {
                _newProgramArgsText = value;
                NotifyPropertyChanged();
            }
        }

        private bool HasLoaded { get; set; }

        public List<string> InGameConfigOptions
        {
            get
            {
                return InGameConfigurationMap.Keys.ToList();
            }
        }

        public Dictionary<string, string> InGameConfigurationMap { get; set; }

        public string SelectedGameConfigOption
        {
            get
            {
                return _selectedGameConfigOption;
            }
            set
            {
                _selectedGameConfigOption = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsCustomConfigOptionSelected));
            }
        }

        public bool IsCustomConfigOptionSelected
        {
            get
            {
                return !_defaultControlNames.Any(s => s.Equals(SelectedGameConfigOption, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public string ImportStatusMessage
        {
            get
            {
                return _importStatusMessage;
            }
            set
            {
                _importStatusMessage = value;
                NotifyPropertyChanged();
            }
        }

        public bool ImportButtonIsEnabled
        {
            get
            {
                return _importButtonIsEnabled;
            }
            set
            {
                _importButtonIsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ImportProgressVisibility
        {
            get
            {
                return _importProgressVisibility;
            }
            set
            {
                _importProgressVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public double ImportProgressValue
        {
            get
            {
                return _importProgressValue;
            }
            set
            {
                _importProgressValue = value;
                NotifyPropertyChanged();
            }
        }

        public VolumeSlider LastVolumeSliderChanged
        {
            get
            {
                return _lastVolumeSliderChanged;
            }
            set
            {
                _lastVolumeSliderChanged = value;
                NotifyPropertyChanged();
            }
        }

        #endregion


        public GameLaunchSettingsViewModel()
        {
            StatusMessage = "";
            NewProgramPathText = "";
            NewProgramArgsText = "";
            IsProgramPopupOpen = false;
            _audioTest = null;
            HasLoaded = false;
            LastVolumeSliderChanged = VolumeSlider.Music;

            InitImportMovieOption();
            InitInGameConfigOptions();
            InitSoundDevices();
            InitRenderers();
            InitMidiDevices();
            LoadSettings(Sys.Settings.GameLaunchSettings);
        }

        private void InitImportMovieOption()
        {
            ImportProgressVisibility = Visibility.Hidden;
            ImportProgressValue = 0;
            ImportStatusMessage = "";
            ImportButtonIsEnabled = !GameConverter.AllMovieFilesExist(Sys.Settings.MovieFolder);

            Dictionary<string, string[]> missingMovies = GameConverter.GetMissingMovieFiles(Sys.Settings.MovieFolder);

            if (missingMovies.Count > 0)
            {
                List<string> discsToInsert = GetDiscsToInsertForMissingMovies(missingMovies);

                ImportStatusMessage = $"Insert {discsToInsert[0]} and click 'Import'.";
            }
        }

        private List<string> GetDiscsToInsertForMissingMovies(Dictionary<string, string[]> missingMovies)
        {
            List<string> discsToInsert = new List<string>();
            missingMovies.Values.GroupBy(s => s)
                                .Select(g => g.Key)
                                .ToList()
                                .ForEach(s => discsToInsert.AddRange(s)); // add all string[] to one List<string> 

            discsToInsert = discsToInsert.Distinct().OrderBy(s => s).ToList();

            return discsToInsert;
        }

        private void LoadSettings(LaunchSettings launchSettings)
        {
            HasLoaded = false;

            if (Sys.Settings.GameLaunchSettings == null)
            {
                Logger.Warn("No game launcher settings found, initializing to defaults.");
                Sys.Settings.GameLaunchSettings = LaunchSettings.DefaultSettings();
                launchSettings = Sys.Settings.GameLaunchSettings;
            }

            ProgramList = new ObservableCollection<ProgramToRunViewModel>(Sys.Settings.ProgramsToLaunchPrior.Select(s => new ProgramToRunViewModel(s.PathToProgram, s.ProgramArgs)));

            AutoUpdatePathChecked = launchSettings.AutoUpdateDiscPath;
            Code5FixChecked = launchSettings.Code5Fix;
            HighDpiFixChecked = launchSettings.HighDpiFix;

            IsShowLauncherChecked = launchSettings.ShowLauncherWindow;
            SelectedGameConfigOption = InGameConfigurationMap.Where(s => s.Value == launchSettings.InGameConfigOption)
                                                             .Select(c => c.Key)
                                                             .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(SelectedGameConfigOption))
            {
                // default to first option if their previous option is missing
                SelectedGameConfigOption = InGameConfigOptions[0];
            }


            // disable options to auto-mount if user OS does not support it
            IsAutoMountSupported = GameLauncher.OSHasAutoMountSupport();

            if (IsAutoMountSupported)
            {
                AutoMountChecked = launchSettings.AutoMountGameDisc;
                AutoUnmountChecked = launchSettings.AutoUnmountGameDisc;
            }
            else
            {
                AutoMountChecked = false;
                AutoUnmountChecked = false;
            }

            // Have option look unchecked and disabled when user does not have The Reunion installed
            IsReunionInstalled = GameLauncher.IsReunionModInstalled();
            if (IsReunionInstalled)
            {
                DisableReunionChecked = launchSettings.DisableReunionOnLaunch;
            }
            else
            {
                DisableReunionChecked = false;
                IsReunionInstalled = false;
            }


            SelectedSoundDevice = SoundDeviceGuids.Where(s => s.Value == launchSettings.SelectedSoundDevice)
                                                  .Select(s => s.Key)
                                                  .FirstOrDefault();

            // switch back to 'Auto' if device not found
            if (SelectedSoundDevice == null)
            {
                SelectedSoundDevice = SoundDeviceGuids.Where(s => s.Value == Guid.Empty)
                                                      .Select(s => s.Key)
                                                      .FirstOrDefault();
            }

            SelectedMidiDevice = MidiDeviceMap.Where(s => s.Value == launchSettings.SelectedMidiDevice)
                                              .Select(s => s.Key)
                                              .FirstOrDefault();

            MusicVolumeValue = launchSettings.MusicVolume;
            SfxVolumeValue = launchSettings.SfxVolume;
            IsReverseSpeakersChecked = launchSettings.ReverseSpeakers;
            IsLogVolumeChecked = launchSettings.LogarithmicVolumeControl;

            SelectedRenderer = RendererMap.Where(s => s.Value == launchSettings.SelectedRenderer)
                                          .Select(s => s.Key)
                                          .FirstOrDefault();

            IsRivaOptionChecked = launchSettings.UseRiva128GraphicsOption;
            IsTntOptionChecked = launchSettings.UseTntGraphicsOption;
            IsQuarterScreenChecked = launchSettings.QuarterScreenMode;
            IsFullScreenChecked = launchSettings.FullScreenMode;

            HasLoaded = true;
        }

        internal bool SaveSettings()
        {
            try
            {
                Sys.Settings.ProgramsToLaunchPrior = GetUpdatedProgramsToRun();

                Sys.Settings.GameLaunchSettings.AutoMountGameDisc = AutoMountChecked;
                Sys.Settings.GameLaunchSettings.AutoUnmountGameDisc = AutoUnmountChecked;
                Sys.Settings.GameLaunchSettings.AutoUpdateDiscPath = AutoUpdatePathChecked;
                Sys.Settings.GameLaunchSettings.ShowLauncherWindow = IsShowLauncherChecked;

                Sys.Settings.GameLaunchSettings.InGameConfigOption = InGameConfigurationMap[SelectedGameConfigOption];

                Sys.Settings.GameLaunchSettings.Code5Fix = Code5FixChecked;
                Sys.Settings.GameLaunchSettings.HighDpiFix = HighDpiFixChecked;


                IsReunionInstalled = GameLauncher.IsReunionModInstalled();
                if (IsReunionInstalled)
                {
                    Sys.Settings.GameLaunchSettings.DisableReunionOnLaunch = DisableReunionChecked;
                }
                else
                {
                    Sys.Settings.GameLaunchSettings.DisableReunionOnLaunch = true; // always have this set to true when Reunion is not installed in case user later installs it
                }


                Sys.Settings.GameLaunchSettings.SelectedSoundDevice = SoundDeviceGuids[SelectedSoundDevice];
                Sys.Settings.GameLaunchSettings.SelectedMidiDevice = MidiDeviceMap[SelectedMidiDevice];
                Sys.Settings.GameLaunchSettings.MusicVolume = MusicVolumeValue;
                Sys.Settings.GameLaunchSettings.SfxVolume = SfxVolumeValue;
                Sys.Settings.GameLaunchSettings.ReverseSpeakers = IsReverseSpeakersChecked;
                Sys.Settings.GameLaunchSettings.LogarithmicVolumeControl = IsLogVolumeChecked;

                Sys.Settings.GameLaunchSettings.SelectedRenderer = RendererMap[SelectedRenderer];
                Sys.Settings.GameLaunchSettings.UseRiva128GraphicsOption = IsRivaOptionChecked;
                Sys.Settings.GameLaunchSettings.UseTntGraphicsOption = IsTntOptionChecked;
                Sys.Settings.GameLaunchSettings.QuarterScreenMode = IsQuarterScreenChecked;
                Sys.Settings.GameLaunchSettings.FullScreenMode = IsFullScreenChecked;

                Sys.Save();

                Sys.Message(new WMessage("Game Launcher settings updated!"));
                return true;
            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to save launch settings: {e.Message}";
                Logger.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Copies ff7input.cfg from FF7 game folder to ./Resources/Controls/ folder with the given <paramref name="customFileName"/>
        /// </summary>
        /// <param name="forceCopy"> copies ff7input.cfg if it already exists; overwriting the current custom.cfg </param>
        public bool CopyInputCfgToCustomCfg(bool forceCopy, string customFileName)
        {
            string pathToCustomCfg = Path.Combine(Sys.PathToControlsFolder, customFileName);
            string pathToInputCfg = Path.Combine(Path.GetDirectoryName(Sys.Settings.FF7Exe), "ff7input.cfg");

            Directory.CreateDirectory(Sys.PathToControlsFolder);

            if (!File.Exists(pathToCustomCfg) || forceCopy)
            {
                if (File.Exists(pathToInputCfg))
                {
                    try
                    {
                        File.Copy(pathToInputCfg, pathToCustomCfg, true);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        StatusMessage = $"Error copying ff7input.cfg: {ex.Message}";
                    }
                }
                else
                {
                    StatusMessage = $"No ff7input.cfg found at {pathToInputCfg}";
                    Logger.Warn(StatusMessage);
                }
            }
            else
            {
                StatusMessage = $"{customFileName} already exists at {Sys.PathToControlsFolder}";
                Logger.Warn(StatusMessage);
            }

            return false;
        }

        private void InitInGameConfigOptions()
        {
            Dictionary<string, string> configOptions = new Dictionary<string, string>();

            if (!Directory.Exists(Sys.PathToControlsFolder))
            {
                Logger.Warn($"Controls folder missing. creating {Sys.PathToControlsFolder}");
                Directory.CreateDirectory(Sys.PathToControlsFolder);
            }

            foreach (string filePath in Directory.GetFiles(Sys.PathToControlsFolder, "*.cfg").OrderBy(s => s))
            {
                FileInfo info = new FileInfo(filePath);
                configOptions.Add(Path.GetFileNameWithoutExtension(filePath), info.Name);
            }

            InGameConfigurationMap = configOptions;
            NotifyPropertyChanged(nameof(InGameConfigOptions));
        }

        private void InitSoundDevices()
        {
            var deviceGuids = new Dictionary<string, Guid>();

            deviceGuids.Add("Auto-Switch (Windows Default)", Guid.Empty);

            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                WaveOutCapabilities caps;

                try
                {
                    caps = WaveOut.GetCapabilities(n);
                }
                catch (Exception e)
                {
                    Logger.Warn(e);
                    continue;
                }

                if (caps.ProductGuid == Guid.Empty)
                    continue;

                // reference: https://stackoverflow.com/questions/1449162/get-the-full-name-of-a-wavein-device
                MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    if (device.FriendlyName.StartsWith(caps.ProductName) && !deviceGuids.ContainsKey(device.FriendlyName))
                    {
                        Guid audioGuid = new Guid(device.Properties[PropertyKeys.PKEY_AudioEndpoint_GUID].Value as string);
                        deviceGuids.Add(device.FriendlyName, audioGuid);
                        break;
                    }
                }
            }

            SoundDeviceGuids = deviceGuids;
            NotifyPropertyChanged(nameof(SoundDevices));
        }

        private void InitMidiDevices()
        {
            MidiDeviceMap = new Dictionary<string, string>()
            {
                { "General MIDI", "GENERAL_MIDI" },
                { "Soundfont MIDI (Creative AWE32/AWE64)", "SOUNDFONT_MIDI" },
                { "Yamaha XG MIDI", "YAMAHA_XG" }
            };
        }

        private void InitRenderers()
        {
            RendererMap = new Dictionary<string, int>()
            {
                { "Software Renderer", 0 },
                { "Direct3D Hardware Acceleration", 1 },
                { "Custom 7H Game Driver", 3 }
            };
        }

        /// <summary>
        /// Plays audio_test_file.ogg based on launch settings for testing audio
        /// </summary>
        /// <param name="channel"> Where audio will play from: Left,Center,or Right channel</param>
        internal void TestAudio(AudioChannel channel)
        {
            string pathToTestFile = Path.Combine(Sys._7HFolder, "Resources", "audio_test_file.ogg");


            if (_audioTest == null)
            {
                // reference: https://markheath.net/post/handling-multi-channel-audio-in-naudio
                // reference: https://stackoverflow.com/questions/22248138/play-sound-on-specific-channel-with-naudio?rq=1

                // input 0 - audio test .ogg
                // input 1 - silenced wave provider to play silent audio
                NAudio.Vorbis.VorbisWaveReader waveReader = new NAudio.Vorbis.VorbisWaveReader(pathToTestFile);
                MultiplexingWaveProvider waveProvider = new MultiplexingWaveProvider(new List<IWaveProvider>() { waveReader, new SilenceWaveProvider(waveReader.WaveFormat) }, 2);

                int leftChannel = 0;
                int rightChannel = 1;

                if (IsReverseSpeakersChecked)
                {
                    leftChannel = 1;
                    rightChannel = 0;
                }

                if (channel == AudioChannel.Left)
                {
                    // note that the wave reader has 2 input channels so we must route both input channels of the reader to the output channel
                    waveProvider.ConnectInputToOutput(0, leftChannel);
                    waveProvider.ConnectInputToOutput(1, leftChannel);

                    waveProvider.ConnectInputToOutput(2, rightChannel);
                    waveProvider.ConnectInputToOutput(3, rightChannel);
                }
                else if (channel == AudioChannel.Right)
                {
                    waveProvider.ConnectInputToOutput(0, rightChannel);
                    waveProvider.ConnectInputToOutput(1, rightChannel);

                    waveProvider.ConnectInputToOutput(2, leftChannel);
                    waveProvider.ConnectInputToOutput(3, leftChannel);
                }
                else
                {
                    waveProvider.ConnectInputToOutput(0, leftChannel);
                    waveProvider.ConnectInputToOutput(1, leftChannel);

                    waveProvider.ConnectInputToOutput(0, rightChannel);
                    waveProvider.ConnectInputToOutput(1, rightChannel);
                }

                float outVolume = (float)MusicVolumeValue / (float)100.0;
                if (LastVolumeSliderChanged == VolumeSlider.Sfx)
                {
                    outVolume = (float)SfxVolumeValue / (float)100.0;
                }

                _audioTest = new WaveOut
                {
                    DeviceNumber = GetSelectedSoundDeviceNumber(),
                    Volume = outVolume
                };

                _audioTest.Init(waveProvider);

                _audioTest.PlaybackStopped += AudioTest_PlaybackStopped;
                _audioTest.Play();

                NotifyPropertyChanged(nameof(IsAudioPlaying));
                NotifyPropertyChanged(nameof(IsAudioNotPlaying));
            }
        }

        private int GetSelectedSoundDeviceNumber()
        {
            Guid selectedGuid = SoundDeviceGuids[SelectedSoundDevice];

            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);

                MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    if (device.FriendlyName.StartsWith(caps.ProductName))
                    {
                        Guid soundGuid = new Guid(device.Properties[PropertyKeys.PKEY_AudioEndpoint_GUID].Value as string);

                        if (soundGuid == selectedGuid)
                        {
                            return n;
                        }
                    }
                }
            }

            return -1; // if device not found return default device which is -1
        }

        private void AudioTest_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (_audioTest != null)
            {
                _audioTest.Stop();
                _audioTest.PlaybackStopped -= AudioTest_PlaybackStopped;
                _audioTest = null;

                NotifyPropertyChanged(nameof(IsAudioPlaying));
                NotifyPropertyChanged(nameof(IsAudioNotPlaying));
            }
        }

        /// <summary>
        /// Shows warning message dialog to user if <see cref="SelectedRenderer"/> is not set to "Custom 7H Game Driver"
        /// </summary>
        private void ShowWarningMessageAboutRenderer()
        {
            if (!HasLoaded)
            {
                // prevent warning from showing when loading settings
                return;
            }

            if (!SelectedRenderer.Equals("Custom 7H Game Driver", StringComparison.InvariantCultureIgnoreCase))
            {
                MessageDialogWindow.Show("Choosing any other option besides 'Custom 7H Game Driver' will cause mods installed with 7H not to work anymore!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowWarningMessageAbouReunion()
        {
            if (!HasLoaded)
            {
                // prevent warning from showing when loading settings
                return;
            }

            if (GameLauncher.IsReunionModInstalled() && !DisableReunionChecked)
            {
                string warningMsg = "Reunion R06 and newer, even when disabled in Options.ini, forces a custom game driver to load when you run FF7. This conflicts with 7th Heaven's game driver, breaks your graphics settings, and you will experience problems.\n\nIf you wish to play Reunion, do so using a compatible modded version built for 7th Heaven.\n\nAre you sure?";
                var result = MessageDialogWindow.Show(warningMsg, "You should leave this setting ON!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result.Result == MessageBoxResult.No)
                {
                    // re-enable option as user selected 'No' in warning message so reverting option
                    DisableReunionChecked = true;
                }
            }
        }

        internal void EditSelectedProgram(ProgramToRunViewModel selected)
        {
            IsProgramPopupOpen = true;
            NewProgramPathText = selected.ProgramPath;
            NewProgramArgsText = selected.ProgramArguments ?? "";
        }

        internal void AddNewProgram()
        {
            IsProgramPopupOpen = true;
        }

        /// <summary>
        /// Adds or Edits program to run and closes programs popup
        /// </summary>
        internal bool SaveProgramToRun()
        {
            if (!File.Exists(NewProgramPathText))
            {
                StatusMessage = "Program to run not found";
                return false;
            }

            if (!ProgramList.Any(s => s.ProgramPath == NewProgramPathText))
            {
                ProgramList.Add(new ProgramToRunViewModel(NewProgramPathText, NewProgramArgsText));
            }
            else
            {
                ProgramToRunViewModel toEdit = ProgramList.FirstOrDefault(s => s.ProgramPath == NewProgramPathText);
                toEdit.ProgramArguments = NewProgramArgsText;
            }

            CloseProgramPopup();
            return true;
        }

        internal void CloseProgramPopup()
        {
            IsProgramPopupOpen = false;
            NewProgramPathText = "";
            NewProgramArgsText = "";
        }

        internal void RemoveSelectedProgram(ProgramToRunViewModel selected)
        {
            ProgramList.Remove(selected);
        }

        /// <summary>
        /// Returns list of <see cref="ProgramLaunchInfo"/> objects based on the current input in <see cref="ProgramList"/>
        /// </summary>
        private List<ProgramLaunchInfo> GetUpdatedProgramsToRun()
        {
            List<ProgramLaunchInfo> updatedPrograms = new List<ProgramLaunchInfo>();

            foreach (ProgramToRunViewModel item in ProgramList.ToList())
            {
                ProgramLaunchInfo existingProg = Sys.Settings.ProgramsToLaunchPrior.FirstOrDefault(s => s.PathToProgram == item.ProgramPath);

                if (existingProg == null)
                {
                    existingProg = new ProgramLaunchInfo() { PathToProgram = item.ProgramPath, ProgramArgs = item.ProgramArguments };
                }
                else
                {
                    existingProg.ProgramArgs = item.ProgramArguments;
                }

                updatedPrograms.Add(existingProg);
            }

            return updatedPrograms;
        }

        internal void ImportMissingMovies()
        {
            string warningMessage = $"This will copy missing movie files from your game discs to your movie path in 'General Settings'.\n\nThis process will overwrite any movie files already in {Sys.Settings.MovieFolder}.\n\nDo you want to proceed?";
            MessageDialogViewModel result = MessageDialogWindow.Show(warningMessage, "Import Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result.Result == MessageBoxResult.No)
            {
                return;
            }

            ImportButtonIsEnabled = false;
            ImportProgressVisibility = Visibility.Visible;

            bool cancelProcess = false;
            int totalFiles = 0;
            int filesCopied = 0;

            Task importTask = Task.Factory.StartNew(() =>
            {
                Dictionary<string, string[]> missingMovies = GameConverter.GetMissingMovieFiles(Sys.Settings.MovieFolder);
                List<string> discsToInsert = GetDiscsToInsertForMissingMovies(missingMovies);

                totalFiles = missingMovies.Count;

                foreach (string disc in discsToInsert)
                {
                    List<string> driveLetters;

                    do
                    {
                        SetImportStatus($"Looking for {disc} ...");
                        driveLetters = GameLauncher.GetDriveLetters(disc);

                        if (driveLetters.Count == 0)
                        {
                            SetImportStatus($"Insert {disc} to continue.");

                            App.Current.Dispatcher.Invoke(() =>
                            {
                                string discNotFoundMessage = $"Please insert {disc} to continue copying missing movie files.\n\nClick 'Cancel' to stop the process.";
                                MessageDialogViewModel insertDiscResult = MessageDialogWindow.Show(discNotFoundMessage, "Insert Disc", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                                cancelProcess = (insertDiscResult.Result == MessageBoxResult.Cancel);
                            });
                        }

                        if (cancelProcess)
                        {
                            return;
                        }

                    } while (driveLetters.Count == 0);

                    SetImportStatus($"Found {disc} at {string.Join("  ", driveLetters)} ...");

                    // loop over missing files on the found disc and copy to data/movies destination
                    foreach (string movieFile in missingMovies.Where(kv => kv.Value.Any(s => s.Equals(disc, StringComparison.InvariantCultureIgnoreCase)))
                                                              .Select(kv => kv.Key))
                    {

                        foreach (string drive in driveLetters)
                        {
                            string fullTargetPath = Path.Combine(Sys.Settings.MovieFolder, movieFile);
                            string sourceFilePath = Path.Combine(drive, "ff7", "movies", movieFile);

                            if (File.Exists(sourceFilePath))
                            {
                                if (File.Exists(fullTargetPath))
                                {
                                    SetImportStatus($"Overwriting {movieFile} ...");
                                }
                                else
                                {
                                    SetImportStatus($"Copying {movieFile} ...");
                                }

                                File.Copy(sourceFilePath, fullTargetPath, true);
                                filesCopied++;
                                UpdateImportProgress(filesCopied, totalFiles);
                                break;
                            }
                            else
                            {
                                SetImportStatus($"Failed to find {movieFile} at {sourceFilePath}");
                            }
                        }
                    }
                }
            });

            importTask.ContinueWith((taskResult) =>
            {
                if (taskResult.IsFaulted)
                {
                    Logger.Error(taskResult.Exception);
                    SetImportStatus($"An error occurred while copying movies: {taskResult.Exception.GetBaseException().Message}");
                }
                else if (cancelProcess)
                {
                    InitImportMovieOption();
                }
                else
                {
                    if (filesCopied == totalFiles)
                    {
                        SetImportStatus("Successfully copied movies.");
                    }
                    else
                    {
                        SetImportStatus("Finished copying movies. Some movies failed to copy. Check app log for details.");
                    }

                    ImportButtonIsEnabled = !GameConverter.AllMovieFilesExist(Sys.Settings.MovieFolder);
                    ImportProgressValue = 0;
                    ImportProgressVisibility = Visibility.Hidden;
                }
            });

        }

        private void SetImportStatus(string message)
        {
            ImportStatusMessage = message;
            Logger.Info(ImportStatusMessage);
            App.ForceUpdateUI();
        }

        private void UpdateImportProgress(int copiedCount, int totalCount)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (totalCount == 0)
                {
                    ImportProgressValue = 0;
                }
                else
                {
                    ImportProgressValue = ((double)copiedCount / (double)totalCount) * 100;
                }
                App.ForceUpdateUI();
            });
        }

        internal void ChangeProgramOrder(ProgramToRunViewModel program, int delta)
        {
            int currentIndex = ProgramList.IndexOf(program);
            int targetIndex = currentIndex + delta;

            if (targetIndex == currentIndex || targetIndex < 0 || targetIndex >= ProgramList.Count)
            {
                return;
            }

            ProgramList.Move(currentIndex, targetIndex);
        }

        internal void ResetToDefaults()
        {
            Logger.Info("Resetting game launcher settings to defaults.");
            LoadSettings(LaunchSettings.DefaultSettings());
        }

        internal void SaveNewCustomControl()
        {
            bool isValid = true;
            string title = "Save Control Configuration";
            string prompt = "Import current controls from game and save as...";
            string controlName;
            string pathToFile;

            do
            {
                isValid = true;
                InputTextWindow inputBox = new InputTextWindow(title, prompt);
                inputBox.ViewModel.MaxCharLength = 24;

                bool? dialogResult = inputBox.ShowDialog();
                if (!dialogResult.GetValueOrDefault(false))
                {
                    return;
                }

                controlName = inputBox.ViewModel.TextInput;

                if (string.IsNullOrEmpty(controlName))
                {
                    isValid = false;
                    MessageDialogWindow.Show("Control name is empty.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                if (Path.GetInvalidFileNameChars().Any(c => controlName.Contains(c)))
                {
                    isValid = false;
                    MessageDialogWindow.Show("Control name contains invalid characters.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                // construct path and check if already exists if name is valid
                if (isValid)
                {
                    if (controlName.EndsWith(".cfg"))
                    {
                        controlName = controlName.Substring(0, controlName.Length - 4);
                    }

                    pathToFile = Path.Combine(Sys.PathToControlsFolder, $"{controlName}.cfg");

                    if (File.Exists(pathToFile))
                    {
                        isValid = false;
                        MessageDialogWindow.Show("Custom controls with that name already exist.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            } while (!isValid);

            try
            {
                CopyInputCfgToCustomCfg(true, $"{controlName}.cfg");
                InitInGameConfigOptions();
                SelectedGameConfigOption = controlName;
                StatusMessage = $"Successfully created custom controls.";

            }
            catch (Exception e)
            {
                Logger.Error(e);
                StatusMessage = $"Failed to create custom controls: {e.Message}";
            }
        }

        internal void DeleteSelectedCustomControl()
        {
            if (!IsCustomConfigOptionSelected)
            {
                return;
            }

            string fileName = $"{SelectedGameConfigOption}.cfg";
            string pathToFile = Path.Combine(Sys.PathToControlsFolder, fileName);

            if (!File.Exists(pathToFile))
            {
                Logger.Warn($"Can not delete custom contols: no file found at {pathToFile}");
            }

            try
            {
                File.Delete(pathToFile);
                InitInGameConfigOptions();
                SelectedGameConfigOption = InGameConfigurationMap.Keys.ToArray()[0];
                StatusMessage = $"Successfully deleted custom controls {fileName}.";
            }
            catch (Exception e)
            {
                Logger.Error(e);
                StatusMessage = $"Failed to delete custom controls: {e.Message}";
            }
        }
    }
}
