using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;


/*
emitimos un evento al reconocer y otro al dejar de reconocer.
dispara con los dos eventos (aun poniendo un minimo (interactions) en el actionAsset)
 
Investigar sobre como generamos el evento y como escribimos los valores.
 */

/*
 TODO:
 Utilizar tiempo de verdad en vez de contadores++
 */

namespace YonduLib.Recognizers
{
    public enum EventName { Click, Whistle, Silence, Null }


    public abstract class SoundRecognizer
    {
        protected SoundRecognizer(EventName name)
        {
            this.name = name;
        }


        #region ATTRIBUTES
        // nombre del evento que va a reconocer
        protected EventName name;

        // maxima frecuencia del espectro
        protected YonduNote maxFrequency = new YonduNote(-1, -1);

        // intervalo en el cual se reconoce la misma frecuencia (inicializado en el Awake)
        protected float _offsetFrequency = 10.0f;

        // frecuencia mas alta del reconocedor
        protected float _eventFrequency = -1.0f;

        // Representa la duracion de un unico evento. Si una palmada se da muy fuerte
        // el sonido permanece un tiempo y si el valor de este atributo es muy bajo entonces
        // se generarán varios eventos CLICK cuento en realidad solo se ha producido una sola palmada.
        // El valor se inicializa en los contructores de cada reconocedor
        protected int _eventDuration = 100;

        // evento reconocido ahora
        // nivel de reconocimiento
        private float _recognitionLevel = 0.0f;

        // contador de silencio
        private int _countNotSoundDetected = 0;
        // contador de sonido
        private int _countSoundDetected = 0;
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

        // devuelve la maxima frecuencia encontrada al analizar el espectro
        //private float GetMaxFrequency() { return maxFrequency.frequency; }

        // trata de reconocer el evento que le corresponda y devuelve el porcentaje de reconocimiento
        public float Recognize(float[] array)
        {
            _recognitionLevel = AnalizeSpectrum(array);

            // maxFrequency contiene la frecuencia maxima en cada vuelta de bucle
            float freq = maxFrequency.frequency;

            // si eventFrequency es -1 significa que estamos ante la primera vuelta
            // despues de un reconocimiento positivo por lo que asignamos la frecuencia
            // maxima en esta vuelta a la frecuencia del posible evento actual
            if (_eventFrequency == -1)
                _eventFrequency = freq;


            // Analizamos el dato de reconocimiento del reconocedor para generar
            // el evento correspondiente.
            // si el grado de reconocimiento es superior al 87% y si la frecuencia
            // del sonido esta dentro del rango se genera un evento del tipo correspondiente
            if (_recognitionLevel > 0.87f && freq > _eventFrequency - _offsetFrequency
                && freq < _eventFrequency + _offsetFrequency)
            {
                _soundRecognize = true;
                _countNotSoundDetected = 0;
            }
            else
            {
                _countNotSoundDetected++;
            }



            // cuenta el tiempo que esta reconociendo un sonido
            if (_soundRecognize)
            {
                _countSoundDetected++;
            }

            // si el tiempo que ha pasado sin reconocerse ningun sonido es mayor a 25 
            // de resetean los flags y el evento actual se le da el valor de silencio
            if (_soundRecognize && _countNotSoundDetected * Time.deltaTime > 25 * Time.deltaTime)
            {
                _countNotSoundDetected = 0;
                _soundRecognize = false;
                _eventRecording = false;
                _eventFrequency = -1;

                Debug.Log("Silencio");

                // generamos el evento de release 
                //name = EventName.Silence;
                EnqueueEvent(0.0f);

            }
            else if (!_soundRecognize)
                _eventFrequency = -1;


            // Si se lleva reconociendo un sonido mas de 100 entonces se genera otro evento
            if (_countSoundDetected * Time.deltaTime > _eventDuration * Time.deltaTime)
            {
                _countSoundDetected = 0;
                _eventRecording = false;
            }


            // Si se ha reconocido el sonido y no estamos generando el evento ya,
            // se genera el evento correspondiente
            if (_soundRecognize && !_eventRecording)
            {
                _eventRecording = true;

                _eventFrequency = freq;
                Debug.Log(name.ToString() + " " + freq);

                if ((freq < array.Length * 0.6f) && (freq > array.Length * 0.2f))
                {
                    float aux = freq - 0.2f;
                    freq = aux / 0.4f;
                }

                EnqueueEvent(freq == -1 ? 1.0f : freq);
                lastNameEvent = name;
            }

            return _recognitionLevel;
        }

        EventName lastNameEvent = EventName.Null;

        private void EnqueueEvent(float freq)
        {
            // TODO: tal vez habria que tratar el valor de freq para que se adecue a el formato de cada evento
            InputEventPtr eventPtr;
            using (StateEvent.From(YonduLibDevice.YonduDevice.current, out eventPtr))
            {
                string buttonName;
                buttonName = name.ToString().ToLower();
                //if (name != EventName.Silence)
                //    buttonName = name.ToString().ToLower();
                //else
                //    buttonName = lastNameEvent.ToString().ToLower();

                InputControl ic = YonduLibDevice.YonduDevice.current[buttonName];

                //((ButtonControl)ic).WriteValueIntoEvent(freq, eventPtr);


                if (name == EventName.Click)
                    ((ButtonControl)ic).WriteValueIntoEvent(freq, eventPtr);
                //else if (name == EventName.Whistle)
                //{
                //    Vector2 value = new Vector2(1f, freq);
                //    ((StickControl)ic).WriteValueIntoEvent(value, eventPtr);
                //}
                //else if(name == EventName.Silence)
                //{
                //    if (name == EventName.Click)
                //        ((ButtonControl)ic).WriteValueIntoEvent(freq, eventPtr);
                //    else if (name == EventName.Whistle)
                //    {
                //        Vector2 value = new Vector2(freq != 0f? 1f : 0f, freq);
                //        ((StickControl)ic).WriteValueIntoEvent(value, eventPtr);
                //    }
                //}
                InputSystem.QueueEvent(eventPtr);
            }
        }
    }
    #endregion
}