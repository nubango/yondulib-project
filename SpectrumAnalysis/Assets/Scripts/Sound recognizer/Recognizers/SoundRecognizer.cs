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
            _currentEventName = EventName.Null;
        }


        #region PROTECTED_ATTRIBUTES
        // nombre del evento que va a reconocer
        protected EventName name;

        // maxima frecuencia del espectro
        protected Note maxFrequency = new Note(-1, -1);

        // intervalo en el cual se reconoce la misma frecuencia (inicializado en el Awake)
        protected float _offsetFrequency = 10.0f;

        // frecuencia mas alta del reconocedor
        protected float _currentEventFrequency = -1.0f;
        #endregion


        #region PRIVATE_ATTRIBUTES
        // evento reconocido ahora
        private EventName _currentEventName;
        // nivel de reconocimiento
        private float _recognitionLevel = 0.0f;

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
            SoundEvent s = new SoundEvent(_currentEventName, _currentEventFrequency, _recognitionLevel);

            _currentEventName = EventName.Null;

            return s;
        }

        // devuelve la maxima frecuencia encontrada al analizar el espectro
        //private float GetMaxFrequency() { return maxFrequency.frequency; }

        // trata de reconocer el evento que le corresponda y devuelve el porcentaje de reconocimiento
        public float Recognize(float[] array)
        {
            _recognitionLevel = AnalizeSpectrum(array);

            float freq = maxFrequency.frequency;
            if (_currentEventFrequency == -1)
                _currentEventFrequency = freq;




            // Analizamos el dato de reconocimiento del reconocedor para generar
            // el evento correspondiente.
            // si el grado de reconocimiento es superior al 87% y si la frecuencia
            // del sonido esta dentro del rango se genera un evento del tipo correspondiente
            if (_recognitionLevel > 0.87f && freq > _currentEventFrequency - _offsetFrequency 
                && freq < _currentEventFrequency + _offsetFrequency)
            {
                _soundRecognize = true;
                _countNotSoundDetected = 0;
            }
            else
            {
                _countNotSoundDetected++;
            }
            //Debug.Log(freq);




            // si el tiempo que ha pasado sin reconocerse ningun sonido es mayor a 25,
            // de resetean los flags y el evento actual se le da el valor de silencio
            if (_countNotSoundDetected * Time.deltaTime > 25 * Time.deltaTime)
            {
                _soundRecognize = false;
                _eventRecording = false;
                _currentEventName = EventName.Silence;
                _currentEventFrequency = -1;
            }



            // Si se ha reconocido el sonido y no estamos generando el evento ya,
            // se genera el evento correspondiente
            if (_soundRecognize && !_eventRecording)
            {
                _countNotSoundDetected = 0;
                _eventRecording = true;

                _currentEventName = name;

                // AQUI DEBERIA IR LA GENERACION DE UN EVENTO DE TIPO "name"
                /*
                https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Events.html
                InputEventPtr eventPtr;
                using (StateEvent.From(myDevice, out eventPtr))
                {
                    ((AxisControl) myDevice["myControl"]).WriteValueIntoEvent(0.5f, eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
                */

                _currentEventFrequency = freq;
                Debug.Log(name.ToString() + " " + freq);
            }

            return _recognitionLevel;
        }
    }
    #endregion
}