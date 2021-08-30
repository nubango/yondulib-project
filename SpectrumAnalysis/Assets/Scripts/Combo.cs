using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    public enum ComboName { Clap, Click, Whistle, Silence }
    public struct WhistleData
    {
        public WhistleData(uint frequency, uint duration)
        {
            f = frequency;
            d = duration;
        }

        public double f { get; }
        public double d { get; }
    }
    public class Combo
    {
        #region PRIVATE_ATTRIBUTES
        private ComboName[] _combo;
        private WhistleData[] _data;

        private float offsetDuration = 0.5f;
        private float offsetFrequency = 0.01f;
        private uint maxWrongRangeFrequency;
        #endregion

        public Combo(ComboName[] combo, WhistleData[] data)
        {
            _combo = combo;
            _data = data;

            offsetFrequency *= SoundEventManager.Instance.GetResolution();
        }

        public bool Recognizer(ComboName[] combo, WhistleData[] data)
        {
            if (combo.Length != _combo.Length)
                return false;

            if (data.Length != _data.Length)
                return false;

            bool correct = true;
            int i = 0, j = 0;

            while (correct && i < combo.Length)
            {
                if (combo[i] != _combo[i])
                    correct = false;

                if (combo[i] == ComboName.Whistle)
                {
                    // comprobamos si la duracion y la frecuencia se salen del intervalo de error
                    if ((data[j].d > _data[j].d + (_data[j].d * 0.1) || data[j].d < _data[j].d - (_data[j].d * 0.1) )&&
                        (data[j].f > _data[j].f + offsetFrequency || data[j].f < _data[j].f - offsetFrequency))
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