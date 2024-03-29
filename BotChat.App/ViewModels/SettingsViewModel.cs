﻿using BotChat.App.Services;
using ChatGPT;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BotChat.App.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ISpeechService _speechService;
        private readonly IUserService _userService;
        private readonly IChatGPTService _chatGPTService;
        [ObservableProperty]
        string selectedLanguage;

        [ObservableProperty]
        string selectedModel;

        [ObservableProperty]
        bool isEnabledAIVoice;

        [ObservableProperty]
        bool isEnabledAutosend;

        [ObservableProperty]
        string pitch;

        [ObservableProperty]
        string volume;

        [ObservableProperty]
        ObservableCollection<string> languages;

        [ObservableProperty]
        ObservableCollection<string> models;

        public SettingsViewModel(ISpeechService speechService, IUserService userService, IChatGPTService chatGPTService)
        {
            _speechService = speechService;
            _userService = userService;
            _chatGPTService = chatGPTService;
            Initialize();
        }

        public async void Initialize()
        {
            var test = await _speechService.GetLocales();
            Models = new(_chatGPTService.Models.Select(d => d.Id).OrderBy(d => d));
            Languages = new((await _speechService.GetLocales()).Select(l => l.Name).OrderBy(l => l));
            SelectedLanguage = string.IsNullOrEmpty(_userService.Settings.Language) ? Languages.FirstOrDefault() : _userService.Settings.Language;
            SelectedModel = string.IsNullOrEmpty(_userService.Settings.ChatGPTAIModel) ? "text-davinci-003" : _userService.Settings.ChatGPTAIModel;
            IsEnabledAIVoice = _userService.Settings.IsEnabledAIVoice;
            IsEnabledAutosend = _userService.Settings.IsEnabledAutosend;
            Pitch = _userService.Settings.SpeechOptionsPitch.ToString().Replace(".", ",");
            Volume = _userService.Settings.SpeechOptionsVolume.ToString().Replace(".", ",");
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            _userService.SetLanguage((string)picker.SelectedItem);
        }

        public void ModelChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            _userService.SetChatGPTAIModel((string)picker.SelectedItem);
        }

        public void PitchChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            var valueText = string.IsNullOrEmpty(entry.Text) ? "0" : entry.Text.Last() == '.' ? $"{entry.Text}0".Replace(".", ",") : entry.Text.Replace(".", ",");
            var value = double.Parse(valueText);

            if (value > 2) Pitch = "2";
            if (value < 0) Pitch = "0";

            value = value >= 0 && value <= 2
                ? value
                : value < 0 ? 0 : 2;

            _userService.SetSpeechOptionsPitch(value);
        }

        public void VolumeChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            var valueText = string.IsNullOrEmpty(entry.Text) ? "0" : entry.Text.Last() == '.' ? $"{entry.Text}0".Replace(".", ",") : entry.Text.Replace(".", ",");
            var value = double.Parse(valueText);

            if (value > 1) Volume = "1";
            if (value < 0) Volume = "0";

            value = value >= 0 && value <= 1
                ? value
                : value < 0 ? 0 : 1;

            _userService.SetSpeechOptionsVolume(value);
        }

        public void EnabledAIVoiceToggled(object sender, ToggledEventArgs e)
        {
            var toggle = (Switch)sender;
            _userService.SetIsEnabledAIVoice(toggle.IsToggled);
        }

        public void EnabledAutosendToggled(object sender, ToggledEventArgs e)
        {
            var toggle = (Switch)sender;
            _userService.SetIsEnabledAutosend(toggle.IsToggled);
        }
    }
}
