using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class Texture : BaseMediaEffect, IDisposable
    {
        public const int SpeedMax = 1000;

        private bool _isStretched;
        private bool _isFloatingAngle;
        private int _angle;
        
        private int _randomAngle;
        private bool _cycleIdOdd;
        
        private int _speed;

        private Mat _originImage;
        private Mat _fittedImage;
        private Mat _defaultImage;

        private readonly Random _random;

        private int X, Y;

        private readonly byte[] _colorBytes = new byte[3];
        public event EventHandler SpeedChanged;
        public event EventHandler AngleChanged;
        public event EventHandler IsFloatingAngleChanged;
        public event EventHandler IsStretchedChanged;

        public Texture() : this(string.Empty) { }

        public Texture(string resourcesPath) : base(resourcesPath)
        {
            _speed = 1;
            _random = new Random();
            _randomAngle = _random.Next(0, 359);
        }

        public Texture(int id, string name, int parentId, int angle, string fileName, long totalTicks,
            bool isFloatingAngle, bool isStretched, int speed, long riseTime, long fadeTime,
            float dimmingLevel) : this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            Angle = angle;
            FileName = fileName;
            TotalTicks = totalTicks;
            IsFloatingAngle = isFloatingAngle;
            IsStretched = isStretched;
            Speed = speed;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
        }

        public override Category Category => Category.Media;
        public override Enum Type => MediaTypes.Texture;

        [SaveLoad]
        public int Speed
        {
            get => _speed;
            set
            {
                if (_speed == value)
                {
                    return;
                }

                _speed = value;
                SpeedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public int Angle
        {
            get => _angle;
            set
            {
                if (_angle == value)
                {
                    return;
                }

                _angle = value;
                OnFileNameChanged();
                AngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public bool IsFloatingAngle
        {
            get => _isFloatingAngle;
            set
            {
                if (_isFloatingAngle == value)
                {
                    return;
                }

                _isFloatingAngle = value;
                IsFloatingAngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public bool IsStretched
        {
            get => _isStretched;
            set
            {
                if (_isStretched == value)
                {
                    return;
                }

                _isStretched = value;
                IsStretchedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var clone = new Texture(ResourcesPath) {
                Parent = parent,
                _angle = Angle,
                _speed = Speed,
                _isFloatingAngle = _isFloatingAngle,
                Name = Name,
                TotalTicks = TotalTicks,
                FileName = FileName,
                _isStretched = _isStretched
            };
            return clone;
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            if (_originImage == null)
            {
                return false;
            }

            Mat result = new Mat();
            if (isDefault)
            {
                if (_defaultImage == null)
                {
                    _defaultImage = new Mat();
                    CvInvoke.Resize(_originImage, _defaultImage, new Size(frame.Width, frame.Height));
                }

                _defaultImage.CopyTo(result);
            }
            else if (_isStretched)
            {
                if (_fittedImage == null)
                {
                    _fittedImage = new Mat();
                    CvInvoke.Resize(_originImage, _fittedImage, new Size(frame.Width, frame.Height));
                }

                _fittedImage.CopyTo(result);
            }
            else
            {
                _originImage.CopyTo(result);
            }

            FillFrame(frame, result, riseOrFadeLevel, ticks);
            return true;
        }

        protected override void OnFileNameChanged()
        {
            if (File.Exists(FullFilePath))
            {
                _originImage = CvInvoke.Imread(FullFilePath);
                // if (_angle != 0)
                // {
                //     PointF center = new PointF(_originImage.Width / 2.0f, _originImage.Height / 2.0f);
                //     RotatedRect destRect = new RotatedRect(center, _originImage.Size, _angle);
                //     Mat result = new Mat();
                //     RotateImage(_originImage, result, destRect);
                //     result.CopyTo(_originImage);
                // }

                _fittedImage?.Dispose();
                _fittedImage = null;
                _defaultImage?.Dispose();
                _defaultImage = null;
            }
            else
            {
                _originImage?.Dispose();
                _originImage = null;
                _fittedImage?.Dispose();
                _fittedImage = null;
                _defaultImage?.Dispose();
                _defaultImage = null;
            }
        }

        private static void RotateImage(Mat src, Mat destination, RotatedRect destRect)
        {
            PointF[] destCorners = destRect.GetVertices();
            PointF[] srcCorners = {
                new PointF(0F, src.Height),
                new PointF(0F, 0F),
                new PointF(src.Width, 0F),
                new PointF(src.Width, src.Height)
            };

            using Mat aft = CvInvoke.GetAffineTransform(srcCorners, destCorners);
            using Mat warpResult = new Mat(src.Size, src.Depth, src.NumberOfChannels);
            CvInvoke.WarpAffine(src, warpResult, aft, destination.Size);
            warpResult.CopyTo(destination);
        }

        private void FillFrame(EffectFrame frame, Mat resultImage, float riseOrFadeLevel, long ticks)
        {
            float radians;
            if (_isFloatingAngle && _speed > 0)
            {
                bool odd = ticks / (TotalTicks / _speed) % 2 == 0;
                if (odd != _cycleIdOdd)
                {
                   _cycleIdOdd = odd;
                   _randomAngle += _random.Next(90, 270);
                }

                radians = _randomAngle.ToRadians();
            }
            else
            {
                radians = Angle.ToRadians();
            }

            float percent = ticks * _speed / (float)TotalTicks;

            X = (int)(MathF.Cos(radians) * percent * resultImage.Width);
            while (X < 0)
            {
                X += resultImage.Width;
            }

            Y = (int)(Math.Sin(radians) * percent * resultImage.Height);
            while (Y < 0)
            {
                Y += resultImage.Height;
            }
            
            Debug.WriteLine($"{X} {Y} {percent}");
            
            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    // TODO цвета могут быть не BGR!! 
                    Marshal.Copy(
                        resultImage.DataPointer +
                        ((point.Y + Y) % resultImage.Height * resultImage.Cols + (point.X + X) % resultImage.Width) *
                        resultImage.ElementSize, _colorBytes, 0, 3);
                    frame.Colors[point] = new CompositeColor(
                        (byte)(_colorBytes[2] * riseOrFadeLevel),
                        (byte)(_colorBytes[1] * riseOrFadeLevel),
                        (byte)(_colorBytes[0] * riseOrFadeLevel));
                }
            }
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _originImage?.Dispose();
                _fittedImage?.Dispose();
                _defaultImage?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Texture()
        {
            Dispose(false);
        }
    }