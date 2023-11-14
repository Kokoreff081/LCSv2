using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;
using NAudio.Dsp;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class SoundStaticColor : BaseAudioEffect
    {
        private CompositeColor _backgroundColor;
        private Complex[] _fft;
        private int _lenght = -1;
        private float multipier;

        public event EventHandler BackgroundColorChanged;

        public SoundStaticColor()
        {
            _backgroundColor = CompositeColor.WhiteColor;
        }

        public SoundStaticColor(int id, string name, int parentId, CompositeColor background, long totalTicks,
            long riseTime, long fadeTime, float dimmingLevel)
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            BackgroundColor = background;
            TotalTicks = totalTicks;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
        }

        public override Category Category => Category.Sound;
        public override Enum Type => SoundTypes.Color;

        public CompositeColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor == value) return;

                _backgroundColor = value;
                BackgroundColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void Save(string projectFolderPath)
        {

        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var clone = new SoundStaticColor
            {
                Parent = parent,
                BackgroundColor = _backgroundColor,
                TotalTicks = TotalTicks,
                Name = Name
            };


            return clone;
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            // if (Capture.CaptureState == CaptureState.Stopped)
            // {
            //     Capture.StartRecording();
            // }
            
            if (Buffer == null || Buffer.FloatBuffer.Length == 0)
            {
                return false;
            }

            if (Buffer.FloatBuffer.Length / 8 != _lenght)
            {
                _lenght = Buffer.FloatBuffer.Length / 8;
                _fft = new Complex[_lenght];
            }
                
            for (int i = 0; i < _fft.Length; i++)
            {
                _fft[i].Y = 0;
                _fft[i].X = Buffer.FloatBuffer[i];
            }

            FastFourierTransform.FFT(true, 8, _fft);

            multipier = 0;
            for (int i = 0; i < _fft.Length; i++)
            {
                multipier = Math.Max(multipier, MathF.Sqrt(_fft[i].X*_fft[i].X + _fft[i].Y*_fft[i].Y));
            }

            frame.FillByColor(_backgroundColor * riseOrFadeLevel * multipier);
            return true;
        }
    }