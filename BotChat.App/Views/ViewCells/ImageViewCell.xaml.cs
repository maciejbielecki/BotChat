using BotChat.App.ViewModels;
using ChatGPT.Models;

namespace BotChat.App.Views.ViewCells;

public partial class ImageViewCell : ViewCell
{
    public static readonly BindableProperty TextProperty =
   BindableProperty.Create("Text", typeof(string), typeof(MessageViewCell), "");

    public static readonly BindableProperty TypeProperty =
   BindableProperty.Create("Type", typeof(ChatType), typeof(MessageViewCell));

    public static readonly BindableProperty DateProperty =
   BindableProperty.Create("Date", typeof(DateTime), typeof(MessageViewCell));

    public static readonly BindableProperty IsHumanProperty =
  BindableProperty.Create("IsHuman", typeof(bool), typeof(MessageViewCell));

    public static readonly BindableProperty IsAIProperty =
  BindableProperty.Create("IsAI", typeof(bool), typeof(MessageViewCell));

    public bool IsHuman
    {
        get { return (bool)GetValue(IsHumanProperty); }
        set { SetValue(IsHumanProperty, value); }
    }

    public bool IsAI
    {
        get { return (bool)GetValue(IsAIProperty); }
        set { SetValue(IsAIProperty, value); }
    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public ChatType Type
    {
        get { return (ChatType)GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }

    public DateTime Date
    {
        get { return (DateTime)GetValue(DateProperty); }
        set { SetValue(DateProperty, value); }
    }

    public ImageViewCell()
    {
        InitializeComponent();
        MainVSL.Padding = new Thickness(5, 15, 0, 15);
    }

    private void VolumeOffBtn_Clicked(object sender, EventArgs e)
    {
        (Shell.Current.CurrentPage.BindingContext as MainViewModel).VolumeOff();
    }

    private void VolumeUpBtn_Clicked(object sender, EventArgs e)
    {
        var imageButton = (ImageButton)sender;
        var vsl = imageButton.Parent.Parent as VerticalStackLayout;
        var hsl = vsl.Children.FirstOrDefault() as HorizontalStackLayout;
        var text = (hsl.Last() as Label).Text;
        (Shell.Current.CurrentPage.BindingContext as MainViewModel).VolumeUp(text);
    }

    private async void ShareContentBtn_Clicked(object sender, EventArgs e)
    {
        var imageButton = (ImageButton)sender;
        var vsl = imageButton.Parent.Parent as VerticalStackLayout;
        var hsl = vsl.Children.FirstOrDefault() as HorizontalStackLayout;
        var url = (hsl.Last() as Image).Source.ToString().Replace("Uri: ", string.Empty);
#if ANDROID
        using (HttpClient client = new HttpClient())
        {
            using (var response = await client.GetAsync(url))
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                    string fileName = $"generated_image_{Guid.NewGuid().ToString("N")}.png";
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream outputFileStream = new FileStream(filePath, FileMode.Create))
                    {
                        stream.CopyTo(outputFileStream);
                        var rof = new ShareFile(filePath);
                        await Share.RequestAsync(new ShareFileRequest() { File = rof, Title = fileName });
                    }
                }
            }
        }
#endif
    }

    private async void CopyContentBtn_Clicked(object sender, EventArgs e)
    {
        var imageButton = (ImageButton)sender;
        var vsl = imageButton.Parent.Parent as VerticalStackLayout;
        var hsl = vsl.Children.FirstOrDefault() as HorizontalStackLayout;
        var url = (hsl.Last() as Image).Source.ToString().Replace("Uri: ", string.Empty);

#if ANDROID
        if (await Shell.Current.CurrentPage.DisplayAlert(string.Empty, "Would you like to download this image?", "Yes", "No"))
        {
            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                        string filePath = System.IO.Path.Combine(path, $"generated_image_{Guid.NewGuid().ToString("N")}.png");
                        using (FileStream outputFileStream = new FileStream(filePath, FileMode.Create))
                        {
                            stream.CopyTo(outputFileStream);
                            await Shell.Current.CurrentPage.DisplayAlert(string.Empty, "Image saved in downloads folder", "Ok");
                        }
                    }
                }
            }
        }
#endif
    }
}