using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    public class Combo
    {
        #region PRIVATE_ATTRIBUTES
        private EventName[] _combo;
        private float[] _frequencies;

        private float offsetFrequency = 0.05f;
        #endregion
        // Debug
        public string _name;

        public Combo(string name, EventName[] namesEvents, float[] frequencies)
        {
            string s = "";
            for (int i = 0; i < namesEvents.Length; i++)
            {
                s += namesEvents[i] + " ";
            }
            Debug.Log("Events: " + s);

            s = "";
            for (int i = 0; i < frequencies.Length; i++)
            {
                s += frequencies[i] + " ";
            }
            Debug.Log("Frecuencias: " + s);

            _name = name;
            _combo = namesEvents;
            if (frequencies == null)
                _frequencies = new float[0];
            else
                _frequencies = frequencies;

            offsetFrequency *= SoundEventManager.Instance.GetResolution();
        }
        // Debug

        public Combo(EventName[] namesEvents, float[] frequencies)
        {
            _combo = namesEvents;
            if (frequencies == null)
                _frequencies = new float[0];
            else
                _frequencies = frequencies;

            offsetFrequency *= SoundEventManager.Instance.GetResolution();
        }

        public bool Recognizer(EventName[] combo, float[] frequencies)
        {
            if (combo.Length != _combo.Length)
                return false;

            if (frequencies.Length != _frequencies.Length)
                return false;

            bool correct = true;
            int i = 0, j = 0;

            while (correct && i < combo.Length)
            {
                if (combo[i] != _combo[i])
                    correct = false;

                if (combo[i] == EventName.Whistle)
                {
                    // comprobamos si la duracion y la frecuencia se salen del intervalo de error
                    if (frequencies[j] > _frequencies[j] + offsetFrequency || frequencies[j] < _frequencies[j] - offsetFrequency)
                    {
                        correct = false;
                    }
                    else
                        j++;
                }
                i++;
            }

            return correct;
        }
    }
}