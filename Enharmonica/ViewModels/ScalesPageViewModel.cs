using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Enharmonica.Models;
using Enharmonica.Services;
using NAudio.Midi;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Enharmonica.ViewModels
{
    public partial class ScalesPageViewModel : BaseViewModel
    {
        private ScaleModel _scaleModel;
        private readonly MIDIGenerator _midiGenerator;
        private readonly FetchDetailService _fetchDetailService;

        [ObservableProperty]
        private string scaleName;

        [ObservableProperty]
        private string scaleImagePath;

        [ObservableProperty]
        private string selectedRootNote;

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private string selectedScaleType;

        [ObservableProperty]
        private string selectedInstrument;

        [ObservableProperty]
        private string explanation;

        [ObservableProperty]
        private ObservableCollection<string> rootNotes;

        [ObservableProperty]
        private ObservableCollection<string> scaleTypes;

        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        private ObservableCollection<string> instrumentList;

        public ScalesPageViewModel()
        {
            _midiGenerator = new MIDIGenerator();
            _fetchDetailService = new FetchDetailService();

            RootNotes = new ObservableCollection<string>(_midiGenerator.GetRootNotes());
            Categories = new ObservableCollection<string>(_midiGenerator.GetCategories());
            ScaleTypes = new ObservableCollection<string>();
            InstrumentList = new ObservableCollection<string>(_midiGenerator.GetInstrumentList());

            SelectedRootNote = RootNotes.FirstOrDefault();
            SelectedCategory = Categories.FirstOrDefault();
            SelectedInstrument = InstrumentList.FirstOrDefault();

            UpdateScaleTypes(SelectedCategory);
        }

        [RelayCommand]
        public void GenerateScale()
        {
            // Generate scale model
            var notes = _midiGenerator.GetScaleNotes(SelectedRootNote, SelectedScaleType);
            var instrument = _midiGenerator.GetMidiInstrument(SelectedInstrument);
            _scaleModel = new ScaleModel(SelectedRootNote, SelectedScaleType, notes, instrument);

            // Update scale name for label
            ScaleName = $"{SelectedRootNote} {SelectedScaleType} Scale:";
            var PATH = @"C:/Users/pangz/Documents/MSSA/MAUI_Programs/Enharmonica/Resources/Images";

            // Determine image path
            var note = $"{SelectedRootNote.Replace("#", "sharp").ToLower()}";
            var jpgName = $"{note}_{SelectedScaleType.Replace(" ", "_").ToLower()}";
            ScaleImagePath = $"{PATH}/Scales/{note}/{jpgName}.jpg"; 
            //Debug.WriteLine($"Scale generated: {ScaleName}");
            //Debug.WriteLine($"Image path: {ScaleImagePath}");

            // GetExplanation from API Call
            FetchExplanation();
        }
        [RelayCommand]
        public void PlayScale()
        {
            if (_scaleModel == null || _scaleModel.Notes == null || _scaleModel.Notes.Count == 0)
            {
                Debug.WriteLine("No valid scale generated.");
                return;
            }

            NotesPlayer notePlayer = new NotesPlayer();
            notePlayer.PlayScales(_scaleModel); // Play scale with velocity 70, duration 500ms
        }
        partial void OnSelectedCategoryChanged(string value)
        {
            // Update Scale Types dynamically based on selected category
            UpdateScaleTypes(value);
        }
        private void UpdateScaleTypes(string category)
        {
            ScaleTypes.Clear();

            var types = _midiGenerator.GetScaleTypes(category);

            foreach (var type in types)
            {
                ScaleTypes.Add(type);
            }

            // Reset the selected scale type
            SelectedScaleType = ScaleTypes.FirstOrDefault();
        }
        private async Task FetchExplanation()
        {
            var keywords = "";
            if (SelectedCategory == "Major-Minor")
            {
                keywords = $"{SelectedScaleType} scale".Replace("-", " ");
            }
            else
            {
                keywords = $"{SelectedCategory} {SelectedScaleType} scale".Replace("-", " ");
            }
            
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
