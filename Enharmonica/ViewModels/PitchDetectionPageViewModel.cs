using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using Enharmonica.Models;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.CoreAudioApi;
using System.Net.Http.Headers;


namespace Enharmonica.ViewModels
{
    public partial class PitchDetectionPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string detectedSong;

        [ObservableProperty]
        private string detectedArtist;

        [ObservableProperty]
        private string detectedDetail;

        [ObservableProperty]
        private bool isLoading;

        private string _apiKey;
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private readonly string audioFilePath = Path.Combine(Environment.GetFolderPath(
                                                             Environment.SpecialFolder.MyDocuments), "audio.wav");
        //private readonly string audioFilePath = Path.Combine(FileSystem.AppDataDirectory, "audio.wav");

        public PitchDetectionPageViewModel()
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(44100, 1);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            LoadApiKey();
        }

        [RelayCommand]
        public async Task Record()
        {
            try
            {
                IsLoading = true;
                if (writer == null)
                {
                    writer = new WaveFileWriter(audioFilePath, waveIn.WaveFormat);
                    waveIn.StartRecording();
                    DetectedSong = "🤫 I'm listening ...";
                    DetectedArtist = string.Empty;
                    DetectedDetail = string.Empty;
                }
                else
                {
                    DetectedSong = "Done listening ...";
                }
            }
            catch (Exception ex)
            {
                DetectedSong = $"StartRecording() function has problem...";
                IsLoading = false; // Hide loader
            }
        }
        [RelayCommand]
        public async Task Stop()
        {
            try
            {
                //ListAudioDevices(); // Debug list of audio Devices
                DetectedSong = "🤔 Thinking REAL hard ...";

                if (waveIn != null)
                {
                    waveIn.StopRecording(); 
                }
                else
                {
                    DetectedSong = "Failed Recording! Stop()->else";
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                DetectedSong = $"STOP Exception: {ex.Message}";
                IsLoading = false; // Hide loader
            }
        }
        private async Task AnalyzeAudioWithAudD(string audioFilePath)
        {
            try
            {
                using var client = new HttpClient();
                var requestContent = new MultipartFormDataContent();
                var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));
                audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
                requestContent.Add(audioContent, "file", Path.GetFileName(audioFilePath));

                // Add API key
                requestContent.Add(new StringContent(_apiKey), "api_token");

                // Send POST request
                var response = await client.PostAsync("https://api.audd.io/", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    DetectedSong = $"HTTP Error: {response.StatusCode}";
                    DetectedArtist = string.Empty;
                    return;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync(); // for debugging
                Debug.WriteLine("API Response: " + jsonResponse);              // for debugging

                // Parse the response
                var apiResponse = JsonSerializer.Deserialize<TrackInfoModel>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                if (apiResponse != null && apiResponse.Status == "success")
                {
                    if (apiResponse.Result != null)
                    {
                        // Extract song details
                        DetectedSong = apiResponse.Result.Title ?? "Unfortunately, never heard of it ...";
                        DetectedArtist = apiResponse.Result.Artist ?? "Who knows who this song belongs to ^_^";
                        var releaseDate = apiResponse.Result.ReleaseDate;
                        var label = apiResponse.Result.Label;
                        var link = apiResponse.Result.SongLink;
                        DetectedDetail = $"Release Date: {releaseDate}\nPublish: {label}\nLink: {link}";
                    }
                    else
                    {
                        // No matches found
                        DetectedSong = "No matches found.";
                        DetectedArtist = string.Empty;
                    }
                }
                else
                {

                    Debug.WriteLine($"File Path: {audioFilePath}");
                    Debug.WriteLine($"File Exists: {File.Exists(audioFilePath)}");
                    var fileInfo = new FileInfo(audioFilePath);
                    Debug.WriteLine($"File Size: {fileInfo.Length} bytes");
                    Debug.WriteLine($"Wave Format: {waveIn.WaveFormat}");

                    // API returned an error
                    DetectedSong = $"API Error: {apiResponse?.Status ?? "Unknown error"}";
                    DetectedArtist = string.Empty;
                }
            }
            catch (Exception ex)
            {
                DetectedSong = $"AnalyzeAudioWithAudD->Exception: {ex.Message}";
                DetectedArtist = string.Empty;
            }
        }
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (writer != null)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }
        private async void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            writer?.Dispose();
            writer = null;

            if (e.Exception != null)
            {
                DetectedSong = "OnRecordingStopped() ERROR!";
                IsLoading = false;
                return;
            }

            // Send audio to Shazam API
            await AnalyzeAudioWithAudD(audioFilePath);
            isLoading = false;
        }
        private void ListAudioDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var device in devices)
            {
                Debug.WriteLine($"Device: {device.FriendlyName}");
            }
        }
        private void LoadApiKey()
        {
            string path = @"C:/Files/AudD_APIKey.txt"; // Full path to the file

            if (File.Exists(path))
            {
                try
                {
                    _apiKey = File.ReadAllText(path).Trim(); // Read and trim whitespace
                    Debug.WriteLine($"API Key successfully loaded: {_apiKey}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading API Key file: {ex.Message}");
                    _apiKey = string.Empty; // Set as empty if an error occurs
                }
            }
            else
            {
                Debug.WriteLine($"API key file not found at: {path}");
                _apiKey = string.Empty; // Set as empty if the file is missing
            }
        }
    }
}
