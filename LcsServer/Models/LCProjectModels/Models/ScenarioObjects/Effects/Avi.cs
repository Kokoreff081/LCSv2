using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;
using NLog;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class Avi : BaseMediaEffect, IDisposable
    {
        private VideoCapture _capture;

        public string FilePath { get; private set; }

        /// <summary>
        /// Темп
        /// </summary>
        public float VideoRate { get; private set; }

        /// <summary>
        /// Номер последнего кадра
        /// </summary>
        public int VideoMaxFrame { get; private set; }

        /// <summary>
        /// Продолжительность видео-файла
        /// </summary>
        public float VideoDuration { get; private set; }

        public int VideoWidth { get; private set; }

        public int VideoHeight { get; private set; }

        private int _fromFrame;
        private int _toFrame;
        private bool _fitToRaster = true;

        private Mat _mat;
        
        private readonly byte[] _colorBytes = new byte[3];

        /// <summary>
        /// Изменение ОТ какого кадра вести расчет
        /// </summary>
        public event EventHandler FromFrameChanged;

        /// <summary>
        /// Изменение ДО какого кадра вести расчет 
        /// </summary>
        public event EventHandler ToFrameChanged;

        public event EventHandler FitToRasterChanged;

        public Avi() : base(string.Empty) { }

        public Avi(string resourcesPath) : base(resourcesPath)
        {
        }

        // ~Avi() {
        //     Dispose(false);
        // }

        public  Avi(int id, string name, int parentId, string fileName, int fromFrame, int toFrame, long totalTicks, long riseTime, long fadeTime, bool fitToRaster, float dimmingLevel) :this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            FileName = fileName;
            FromFrame = fromFrame;
            ToFrame = toFrame;
            TotalTicks = totalTicks;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            FitToRaster = fitToRaster;
            DimmingLevel = dimmingLevel;
        }

        /// <summary>
        /// Категория
        /// </summary>
        public override Category Category => Category.Media;

        /// <summary>
        /// Тип
        /// </summary>
        public override Enum Type => MediaTypes.Avi;
        
        /// <summary>
        /// С какого кадра проигрывать
        /// </summary>
        [SaveLoad]
        public int FromFrame
        {
            get => _fromFrame;
            set
            {
                if (_fromFrame == value)
                {
                    return;
                }

                _fromFrame = value;
                FromFrameChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// До какого кадра проигрывать
        /// </summary>
        [SaveLoad]
        public int ToFrame
        {
            get => _toFrame;
            set
            {
                if (_toFrame == value)
                {
                    return;
                }

                _toFrame = value;
                ToFrameChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool FitToRaster
        {
            get => _fitToRaster;
            set
            {
                if (_fitToRaster == value)
                {
                    return;
                }

                _fitToRaster = value;
                FitToRasterChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            return new Avi(ResourcesPath)
            {
                Parent = parent,
                FileName = FileName,
                TotalTicks = TotalTicks,
                FromFrame = _fromFrame,
                ToFrame = _toFrame,
                FitToRaster = _fitToRaster,
            };
        }
        
        protected override void OnFileNameChanged()
        {
            string fullFileName = Path.Combine(ResourcesPath, FileName);
            if (FilePath == fullFileName)
            {
                return;
            }

            FileChanged = true;
            FilePath = fullFileName;
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseAndFadeLevel, bool isDefault)
        {
            if (FileChanged || _capture == null)
            {
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        _capture?.Dispose();
                        FilePath = string.Empty;
                    }
                    else
                    {
                        _capture?.Dispose();
                        _capture = new VideoCapture(FilePath);

                        if (_capture == null)
                        {
                            return false;
                        }

                        VideoRate = (float)_capture.Get(Emgu.CV.CvEnum.CapProp.Fps);
                        double frames = _capture.Get(Emgu.CV.CvEnum.CapProp.FrameCount);
                        VideoMaxFrame = Math.Max(0, (int)Math.Floor(frames) - 1);
                        VideoDuration = VideoRate == 0 ? 0 : (float)Math.Round(VideoMaxFrame / VideoRate, 1);
                        VideoHeight = _capture.Height;
                        VideoWidth = _capture.Width;
                    }
                }
                finally
                {
                    FileChanged = false;
                }
            }

            if (_capture == null || VideoMaxFrame <= 0 || !(VideoRate > 0))
            {
                return false;
            }

            float stepTime = (_toFrame - _fromFrame) / VideoRate;
            float tickFloat = ticks / 1000f;
            tickFloat %= stepTime;

            int frameNumber = (int)Math.Floor(tickFloat * VideoRate) + FromFrame;

            if (frameNumber >= ToFrame - 1)
            {
                return false;
            }

            try
            {
                _capture.Set(Emgu.CV.CvEnum.CapProp.PosFrames, frameNumber);

                _mat = _capture.QueryFrame();
                if (_mat == null)
                {
                    return false;
                }

                if (isDefault || (_fitToRaster && _mat.Height != frame.Height && _mat.Width != frame.Width))
                {
                    CvInvoke.Resize(_mat, _mat, new Size(frame.Width, frame.Height));
                }

                FillFrame(frame, _mat, riseAndFadeLevel);
                return true;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error($"AVI effect error {e.Message}");
                return false;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void FillFrame(EffectFrame frame, Mat resultImage, float riseOrFadeLevel)
        {
            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    // TODO цвета могут быть не BGR!! 
                    Marshal.Copy(
                        resultImage.DataPointer +
                        ((point.Y % resultImage.Height * resultImage.Cols) + point.X % resultImage.Width) *
                        resultImage.ElementSize, _colorBytes, 0, 3);
                    frame.Colors[point] = new CompositeColor(
                        (byte)(_colorBytes[2] * riseOrFadeLevel),
                        (byte)(_colorBytes[1] * riseOrFadeLevel),
                        (byte)(_colorBytes[0] * riseOrFadeLevel));
                }
            }
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _capture?.Dispose();
                _mat?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }