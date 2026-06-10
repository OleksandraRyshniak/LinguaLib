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
            player.Play();
        }
        catch { /* ignore missing audio files */ }
    }

    public async Task ToggleBackgroundMusicAsync(string resourceName)
    {
        if (_bgPlaying)
        {
            _bgPlayer?.Stop();
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
        catch { /* ignore */ }
    }

    public bool IsBackgroundPlaying => _bgPlaying;

    public void StopBackground()
    {
        _bgPlayer?.Stop();
        _bgPlaying = false;
    }
}
