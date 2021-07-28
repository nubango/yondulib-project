using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    /* 
     * Identificacion de la frecuencia. El resultado final es que se "grabara" el silbido (otra clase por hacer) y hay que identificar 
     * y guardar la frecuencia. El método Recognize(...) lo que hace es identificar si se silba en la frecuencia grabada.
     * Tambien guardar el tiempo que dura la frecuencia (tanto la frecuencia como la duracion tienen un margen de error)
     * 
     * **/
    public class WhistleFrequencyRecognizer : SoundRecognizer
    {
        #region PRIVATE_ATTRIBUTES
        private int windowSizeBig = 100;
        private int windowSizeSmall = 20;

        // cuenta de las veces que se detecta una frecuencia
        private uint countFrequencyDetected = 0;
        // cuenta las veces que la frecuencia no es la buscada (se usa porque al silbar no siempre aguantamos la misma frecuencia)
        private uint countWrogFrequencyDetected = 0;
        private uint maxWrongRangeFrequency;

        // rango de error al comprobar la duracion y la frecuencia del silbido 
        private float offsetDuration = 0.5f;
        private float offsetFrequency = 0.01f;
        // minima intensidad que tiene que detectar para que contabilice como que ha habido un silbido
        private int minIntensityDetection = 15;

        // maxima y minima intensidad de toda la muestra. Valores acordes con los de un silbido
        private int maxAllIntensity = 80;
        private int minAllIntensity = 20;
        #endregion

        #region PUBLIC_ATTRIBUTES
        /*
         * frequency = 250
         * frequencyDuration = 100
         * 
         * **/
        // Frecuencia que tiene que ser identificada y su duracion
        public uint frequency;
        public uint frequencyDuration;

        // debug
        public int debugFrequency;
        // debug
        #endregion

        #region UNITY_REGION
        private void Start()
        {
            offsetFrequency *= SoundEventManager.Instance.GetResolution();
            maxWrongRangeFrequency = offsetDuration * frequencyDuration > 1000 ? 1000 : (uint)(offsetDuration * frequencyDuration);
        }
        #endregion

        #region RECOGNIZER_METHODS
        /*Primera criba en la intensidad. Establecemos un valor maximo y minimo (en ambos tamaños de ventana)**/
        public override bool Recognize(float[] array)
        {
            bool isClick = false;

            // ventana deslizante con tamaño de ventana grande
            Utils.WindowUnit maxBigSize = Utils.SlidingWindow(array, windowSizeBig);
            Utils.WindowUnit allFrequencies = Utils.SlidingWindow(array, array.Length - 1);

            // Si la intensidad esta entre cierto rango, entonces estamos oyendo un posible silbido
            if (maxBigSize.intensity > minIntensityDetection && allFrequencies.intensity > minAllIntensity
                && allFrequencies.intensity < maxAllIntensity)
            {
                // pasamos una ventana mas pequeña para identificar la frecuencia exacta
                Utils.WindowUnit maxSmallSize = Utils.SlidingWindow(array, windowSizeSmall);

                //debug
                debugFrequency = maxSmallSize.frequency;
                //debug

                // Comprobamos que fecuencia del input esta dentro del rango de la fecuencia buscada
                if ((maxSmallSize.frequency < (frequency + offsetFrequency) && maxSmallSize.frequency > (frequency - offsetFrequency)))
                {
                    countFrequencyDetected++;
                    countWrogFrequencyDetected = 0;
                }
                // no detecta la frecuencia pero hay cierto margen de error al detectar la frecuencia porque el silbido tiene fluctuaciones
                else if (countWrogFrequencyDetected * Time.deltaTime < maxWrongRangeFrequency * Time.deltaTime)
                {
                    countWrogFrequencyDetected++;
                    countFrequencyDetected++;
                }
                // si supera el margen de error entonces asumimos que no está silbando en la frecuencia adecuada
                else
                {
                    countFrequencyDetected = 0;
                    countWrogFrequencyDetected = 0;
                }
            }
            // si la intensidad es demasiado baja reiniciamos contadores y contabilizamos como silencio
            else if (maxBigSize.intensity < minIntensityDetection && allFrequencies.intensity < minAllIntensity)
            {
                countFrequencyDetected = 0;
                countWrogFrequencyDetected = 0;
            }

            // Comprobamos que la duracion de la frecuencia esta dentro del rango establecido
            if (countFrequencyDetected * Time.deltaTime > (frequencyDuration) * Time.deltaTime)
            {
                isClick = true;
                countFrequencyDetected = 0;
            }


            return isClick;
        }
        #endregion
    }
}