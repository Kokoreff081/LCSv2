using NAudio.Wave;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public abstract class BaseAudioEffect: Effect, IDisposable
{
    public WaveBuffer Buffer;
    // protected readonly WasapiLoopbackCapture Capture = new WasapiLoopbackCapture();


    protected BaseAudioEffect()
    {
        // Capture.DataAvailable += WriteBuffer;
        // Capture.StartRecording();
    }

    // private void WriteBuffer(object sender, WaveInEventArgs e)
    // {
    //     // if (!Active)
    //     // {
    //     //     Capture.StopRecording();
    //     //     Buffer = null;
    //     // }
    //     
    //     Buffer = new WaveBuffer(e.Buffer);
    // }

    private void ReleaseUnmanagedResources()
    {
        // TODO release unmanaged resources here
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            // Capture.DataAvailable -= WriteBuffer;
            // Capture?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BaseAudioEffect()
    {
        Dispose(false);
    }
}