using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

 public partial class Stars
    {
        private class Star
        {
            private enum States
            {
                Birth,
                Life,
                Dead,
                Meteor
            };

            private const int MaxSpeed = 9;

            private CompositeColor _color;
            private int _lifeTime;
            private bool _modulation; //типа поблескивание
            private States _state;
            private int _appearSpeed;
            private int _fadeSpeed;
            private int _modulationTimes;
            private int _brightness;

            public Star(CompositeColor color, bool modulation, int lifeTime, IntPoint position)
            {
                _state = States.Birth;
                _modulationTimes = 0;
                _brightness = 0;

                _color = color;
                Position = position;
                _modulation = modulation && LCRandom.RandomNext(5) == 1;
                _lifeTime = lifeTime;

                _appearSpeed = LCRandom.RandomNext(1, MaxSpeed);
                _fadeSpeed = LCRandom.RandomNext(1, MaxSpeed);
            }

            public IntPoint Position { get; private set; }
            
            public CompositeColor GetColor()
            {
                return CompositeColor.GetAttenuate(_color, _brightness);
            }

            public bool Quant()
            {
                bool res = false;

                switch(_state)
                {
                    case States.Birth:
                        if(_brightness < 255)
                        {
                            _brightness += _appearSpeed;
                        }

                        if(_brightness >= 255)
                        {
                            _state = States.Life;
                            _brightness = 255;
                            _modulationTimes = LCRandom.RandomNext(1, 4) * 2;
                        }

                        if(--_lifeTime <= 0)
                        {
                            _state = States.Dead;
                        }
                        break;

                    case States.Life:
                        if(--_lifeTime <= 0)
                        {
                            _state = States.Dead;
                        }
                        else
                        {
                            if(_modulation && _modulationTimes > 0)
                            {
                                _modulationTimes--;
                                _brightness = (_modulationTimes & 1) == 1 ? (int)(255 * 0.75f) : 255;
                            }
                        }


                        break;

                    case States.Dead:
                        if(_brightness > _fadeSpeed)
                        {
                            _brightness -= _fadeSpeed;
                        }
                        else
                        {
                            _brightness = 0;
                            res = true;
                        }
                        break;

                    case States.Meteor:
                        break;
                }

                return res;
            }
        }
    }