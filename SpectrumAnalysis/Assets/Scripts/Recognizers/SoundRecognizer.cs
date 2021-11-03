using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PatternRecognizer
{
    public enum EventName { Click, Whistle, Silence, Null }
    public struct SoundEvent
    {
        public SoundEvent(EventName n, float f, float d)
        {
            name = n;
            frequency = f;
            data = d;
        }
        public EventName name { get; }
        public float frequency { get; }
        public float data { get; }
    }
    public abstract class SoundRecognizer
    {
        protected SoundRecognizer(EventName name)
        {
            this.name = name;
            _currentName = EventName.Null;
        }


        #region PROTECTED_ATTRIBUTES
        // nombre del evento que va a reconocer
        protected EventName name;

        // maxima frecuencia del espectro
        protected Note maxFrequency = new Note(-1, -1);

        // intervalo en el cual se reconoce la misma frecuencia (inicializado en el Awake)
        protected float _offsetFrequency = 10.0f;

        // frecuencia mas alta del reconocedor
        protected float _currentFrequency = -1.0f;
        #endregion


        #region PRIVATE_ATTRIBUTES
        // evento reconocido ahora
        private EventName _currentName;
        // datos de reconocimiento
        private float _currentData = 0.0f;

        // contador de silencio
        private int _countNotSoundDetected = 0;
        // flag que se activa cuando se ha reconocido algo para que se cree un solo evento mientas dure el reconocimineto
        private bool _eventRecording = false;

        // contador de evento reconocido (suma mientras dure el reconocimiento)
        private bool _soundRecognize = false;
        #endregion


        #region PROTECTED_METHODS
        // metodo que analiza el espectro y devueve un float en el intervalo 0-1 siendo 0 = no reconocido y 1 = total reconocimiento
        protected abstract float AnalizeSpectrum(float[] array);
        #endregion


        #region PUBLIC_METHODS
        // metodo que devuelve el evento reconocido al llamar a recognize. Es destructivo, una vez le llamas la infomracion se resetea.
        public virtual SoundEvent GetEvent()
        {
            SoundEvent s = new SoundEvent(_currentName, _currentFrequency, _currentData);

            _currentName = EventName.Null;

            return s;
        }

        // devuelve la maxima frecuencia encontrada al analizar el espectro
        //private float GetMaxFrequency() { return maxFrequency.frequency; }

        // trata de reconocer el evento que le corresponda y devuelve el porcentaje de reconocimiento
        public float Recognize(float[] array)
        {
            _currentData = AnalizeSpectrum(array);

            float freq = maxFrequency.frequency;
            if (_currentFrequency == -1)
                _currentFrequency = freq;

            if (_currentData > 0.87f && freq > _currentFrequency - _offsetFrequency && freq < _currentFrequency + _offsetFrequency)
            {
                _soundRecognize = true;
                _countNotSoundDetected = 0;
            }
            else
            {
                _countNotSoundDetected++;
            }
            //Debug.Log(freq);

            if (_countNotSoundDetected > 25)
            {
                _soundRecognize = false;
                _eventRecording = false;
                _currentName = EventName.Silence;
                _currentFrequency = -1;
            }


            if (_soundRecognize && !_eventRecording)
            {
                _countNotSoundDetected = 0;
                _eventRecording = true;

                _currentName = name;

                _currentFrequency = freq;
                Debug.Log(name.ToString() + " " + freq);
            }

            return _currentData;
        }
    }
    #endregion
}