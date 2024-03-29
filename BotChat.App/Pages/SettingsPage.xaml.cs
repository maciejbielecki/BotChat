﻿using BotChat.App.ViewModels;
using BotChat.App.Views;

namespace BotChat.App
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;
        public SettingsPage(SettingsViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            Navigation.Add(new NavigationView(serviceProvider.GetService<HeaderViewModel>()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((Navigation.Children[0] as NavigationView).BindingContext as HeaderViewModel).IsClearButtonVisible = false;
        }

        private void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewModel.LanguageChanged(sender, e);
        }

        private void ModelPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewModel.ModelChanged(sender, e);
        }

        private void PitchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.PitchChanged(sender, e);
        }

        private void VolumeEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.VolumeChanged(sender, e);
        }

        private void EnabledAIVoice_Toggled(object sender, ToggledEventArgs e)
        {
            _viewModel.EnabledAIVoiceToggled(sender, e);
        }

        private void EnabledAutosend_Toggled(object sender, ToggledEventArgs e)
        {
            _viewModel.EnabledAutosendToggled(sender, e);
        }

        private async void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}