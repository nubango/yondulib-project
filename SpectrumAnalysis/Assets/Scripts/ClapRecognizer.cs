using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Clase que reconoce cuando se ha dado una palmada
 * **/

namespace SpectrumAnalyzer
{
    public class ClapRecognizer : SoundRecognizer
    {
        #region UNITY_REGION

        #region public_attributes
        [Tooltip("Limite de volumen a partir del cual empieza a analizar la entrada")]
        public int limit;
        [Tooltip("Tamaño de la ventana en la ventana deslizante")]
        public int windowSize;
        #endregion

        #region private_attributes
        private int maxRangeFrequency = 450;
        private int minRangeFrequency = 550;

        private int maxRangeIntensity = 40;
        private int minRangeIntensity = 70;
        #endregion

        #region private_methods
        private void Start()
        {

        }
        #endregion
        #endregion

        private bool Analyze(WindowUnit max)
        {
            return max.intensity > minRangeIntensity && max.intensity < maxRangeIntensity &&
                max.frequency > minRangeFrequency && max.frequency < maxRangeFrequency;
        }

        public override bool Recognize(float[] array)
        {
            WindowUnit max = SlidingWindow(array, windowSize);

            if (max.intensity > limit)
            {
                Debug.Log((int)max.frequency);
            }

            return Analyze(max);
        }
    }
}