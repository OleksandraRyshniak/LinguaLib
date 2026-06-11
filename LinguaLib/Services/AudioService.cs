using Plugin.Maui.Audio;

namespace LinguaLib.Services;

public class AudioService
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _bgPlayer;
    private bool _bgPlaying;

    public AudioService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public async Task PlaySoundAsync(string resourceName)
    {
        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(resourceName);
            var player = _audioManager.CreatePlayer(stream);
            player.PlaybackEnded += (s, e) => player.Dispose();
            player.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PlaySoundAsync error: {ex.Message}");
        }
    }

    public async Task ToggleBackgroundMusicAsync(string resourceName)
    {
        if (_bgPlaying)
        {
            _bgPlayer?.Stop();
            _bgPlayer?.Dispose();
            _bgPlayer = null;
            _bgPlaying = false;
            return;
        }

        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(resourceName);
            _bgPlayer = _audioManager.CreatePlayer(stream);
            _bgPlayer.Loop = true;
            _bgPlayer.Play();
            _bgPlaying = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ToggleBackgroundMusicAsync error: {ex.Message}");
        }
    }

    public bool IsBackgroundPlaying => _bgPlaying;

    public void StopBackground()
    {
        _bgPlayer?.Stop();
        _bgPlayer?.Dispose();
        _bgPlayer = null;
        _bgPlaying = false;
    }
}