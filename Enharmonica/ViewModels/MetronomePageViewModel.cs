using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Enharmonica.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enharmonica.ViewModels
{
    public partial class MetronomePageViewModel : BaseViewModel
    {
        private NotesPlayer _notesPlayer;
        private MIDIGenerator _midiGenerator;
        private DateTime? _previousTap;

        [ObservableProperty]
        private int selectedTempo;

        [ObservableProperty]
        public string playStatus;

        [ObservableProperty]
        public string playButtonColor;

        [ObservableProperty]
        public string tempoExpression;

        [ObservableProperty]
        public bool isPlaying;
        public bool IsNotPlaying => !isPlaying;

        public MetronomePageViewModel()
        {
            _midiGenerator = new MIDIGenerator();
            _notesPlayer = new NotesPlayer();
            SelectedTempo = 120;
            IsPlaying = false;
            UpdateTempoExpression();
            UpdatePlayButtonProperties();
        }

        [RelayCommand]
        public void IncreaseTempo()
        {
            if (SelectedTempo < 240)
            {
                SelectedTempo++;
            }
        }

        [RelayCommand]
        public void DecreaseTempo()
        {
            if (SelectedTempo > 40) // Min tempo limit
            {
                SelectedTempo--;
            }
        }

        [RelayCommand]
        public async void TogglePlay()
        {
            if (IsPlaying)
            {
                // Stop the metronome
                IsPlaying = false;
                UpdatePlayButtonProperties();
                _notesPlayer.StopMetronome();
            }
            else
            {
                // Start the metronome
                IsPlaying = true;
                UpdatePlayButtonProperties();
                await _notesPlayer.PlayMetronome(SelectedTempo); // Use NotesPlayer to play metronome
            }
        }

        [RelayCommand]
        public void TapTempo()
        {
            var now = DateTime.Now;
            
            if (_previousTap == null) // first time tap
            {
                _previousTap = now;
                return;
            }

            var timeDifference= (now - _previousTap.Value).TotalSeconds;
            var tempo = (int)(60 / timeDifference); // calculate tempo based on seconds
            _previousTap = now; // update previous tap

            if (tempo >= 40 && tempo < 240) // ignore if tap is too fast or too slow
            {
                SelectedTempo = tempo;
            }
        }
        private void UpdatePlayButtonProperties()
        {
            PlayStatus = IsPlaying ? "Stop" : "Play";
            PlayButtonColor = IsPlaying ? "#E53935" : "#4CAF50"; // Red for Stop, Green for Play
        }
        private void UpdateTempo()
        {
            if (IsPlaying)
            {
                _notesPlayer.StopMetronome();
                _notesPlayer.PlayMetronome(SelectedTempo);
            }
        }
        private void UpdateTempoExpression()
        {
            TempoExpression = _midiGenerator.GetTempoExpression(SelectedTempo);
        }
        partial void OnSelectedTempoChanged(int value)
        {
            UpdateTempoExpression();
            UpdateTempo();
        }
        partial void OnIsPlayingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotPlaying));
        }
    }
}
