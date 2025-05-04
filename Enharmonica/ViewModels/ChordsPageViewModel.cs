using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Enharmonica.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Enharmonica.Services;

namespace Enharmonica.ViewModels
{
    public partial class ChordsPageViewModel : BaseViewModel
    {
        private readonly MIDIGenerator _midiGenerator;
        private ChordModel _chordModel;
        private readonly FetchDetailService _fetchDetailService;

        [ObservableProperty]
        private string selectedRootNote;

        [ObservableProperty]
        private string selectedChordType;

        [ObservableProperty]
        private string selectedPlaybackType;

        [ObservableProperty]
        private string selectedInstrument;

        [ObservableProperty]
        private string chordName;

        [ObservableProperty]
        private string chordImagePath;

        [ObservableProperty]
        private ObservableCollection<string> rootNotes;

        [ObservableProperty]
        private ObservableCollection<string> playbackTypes;

        [ObservableProperty]
        private ObservableCollection<string> chordTypes;

        [ObservableProperty]
        private ObservableCollection<string> instrumentList;

        [ObservableProperty]
        private string explanation;

        public ChordsPageViewModel()
        {
            _midiGenerator = new MIDIGenerator();
            _fetchDetailService = new FetchDetailService();

            RootNotes = new ObservableCollection<string>(_midiGenerator.GetRootNotes());
            ChordTypes = new ObservableCollection<string>(_midiGenerator.GetChordTypes());
            PlaybackTypes = new ObservableCollection<string>(_midiGenerator.GetPlaybackTypes());
            InstrumentList = new ObservableCollection<string>(_midiGenerator.GetInstrumentList());

            SelectedRootNote = RootNotes.FirstOrDefault();
            SelectedChordType = ChordTypes.FirstOrDefault();
            SelectedPlaybackType = PlaybackTypes.FirstOrDefault();
            SelectedInstrument = InstrumentList.FirstOrDefault();
        }

        [RelayCommand]
        public void GenerateChord()
        {
            if (string.IsNullOrEmpty(SelectedRootNote) || string.IsNullOrEmpty(SelectedChordType))
            {
                Debug.WriteLine("Root note or chord type is not selected.");
                return;
            }

            // Generate scale model
            var notes = _midiGenerator.GetChordNotes(SelectedRootNote, SelectedChordType);
            var instrument = _midiGenerator.GetMidiInstrument(SelectedInstrument);
            _chordModel = new ChordModel(SelectedRootNote, SelectedChordType, notes, instrument);

            // Update scale name for label
            ChordName = $"{SelectedRootNote} {SelectedChordType} Chord:";
            var PATH = @"C:/Users/pangz/Documents/MSSA/MAUI_Programs/Enharmonica/Resources/Images";


            // Determine image path
            var note = $"{SelectedRootNote.Replace("#", "sharp").ToLower()}";
            var jpgName = $"{note}_{SelectedChordType.Replace(" ", "_").ToLower()}";
            ChordImagePath = $"{PATH}/Chords/{note}/{jpgName}.jpg"; // Example path format
            //Debug.WriteLine($"Displaying: {ChordImagePath}");
            FetchExplanation();
        }

        [RelayCommand]
        public void PlayChord() // Play a chord: Notes are played simultaneously
        {
            if (_chordModel == null || _chordModel.Notes == null || _chordModel.Notes.Count == 0)
            {
                Debug.WriteLine("No valid scale generated.");
                return;
            }

            NotesPlayer notePlayer = new NotesPlayer();
            if (SelectedPlaybackType == "Arpeggio")
            {
                notePlayer.PlayArpeggio(_chordModel); // Play scale with velocity 70, duration 500ms
            }
            else
            {
                notePlayer.PlayChords(_chordModel); // Play scale with velocity 70, duration 500ms
            }
        }
        private async Task FetchExplanation()
        {
            var keywords = $"{SelectedChordType} chord";
            //Debug.WriteLine($"keywords: {keywords}");

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                Explanation = $"Getting more details for {keywords}";
                Explanation = await _fetchDetailService.GetExplanation(keywords);
            }
            else
            {
                Debug.WriteLine("--- Invalid Keywords! ---");
                Explanation = string.Empty;
            }
        }
    }
}
