using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    /*
     * Clase que tiene un método que devuelde la frecuencia del silbido y su duracion
     * **/
    class WhistleRecognizer : MonoBehaviour
    {
        #region PRIVATE_ATTRIBUTES
        private int windowSizeBig = 100;
        private int windowSizeSmall = 20;

        // cuenta de las veces que se detecta una frecuencia
        private uint countFrequencyDetected = 0;
        // cuenta las veces que la frecuencia no es la buscada (se usa porque al silbar no siempre aguantamos la misma frecuencia)
        private uint countWrongFrequencyDetected = 0;
        private uint maxWrongRangeFrequency = 20;

        private uint minDurationFrequency = 40;

        // rango de error al comprobar la frecuencia del silbido 
        private float offsetFrequency = 0.01f;
        // minima intensidad que tiene que detectar para que contabilice como que ha habido un silbido
        private int minIntensityDetection = 15;

        // maxima y minima intensidad de toda la muestra. Valores acordes con los de un silbido
        private int maxAllIntensity = 80;
        private int minAllIntensity = 20;

        // frecuencia actual que está sonando
        private int currentFrequency = 0;
        #endregion

        #region RECOGNIZE_METHODS
        public Utils.Pair<int, uint> Recognize(float[] array)
        {
            Utils.Pair<int, uint> res = null;
            // ventana deslizante con tamaño de ventana grande
            Utils.WindowUnit maxBigSize = Utils.SlidingWindowMax(array, windowSizeBig);
            Utils.WindowUnit allFrequencies = Utils.SlidingWindowMax(array, array.Length - 1);

            // Si la intensidad esta entre cierto rango, entonces estamos oyendo un posible silbido
            if (maxBigSize.intensity > minIntensityDetection && allFrequencies.intensity > minAllIntensity
                && allFrequencies.intensity < maxAllIntensity)
            {
                //Debug.Log("all = " + allFrequencies.intensity + " - big = " + maxBigSize.intensity);
                // pasamos una ventana mas pequeña para identificar la frecuencia exacta
                Utils.WindowUnit maxSmallSize = Utils.SlidingWindowMax(array, windowSizeSmall);

                //Comprobamos si la frecuencia escuchada es la misma que la anterior o si es una nueva
                //Hay un margen de error para mejorar la precision
                //Si detecta que ya no está silbando en la frecuencia anterior la devuelve (si es lo suficientemente larga)
                if ((maxSmallSize.frequency < (currentFrequency + offsetFrequency)
                     && maxSmallSize.frequency > (currentFrequency - offsetFrequency)))
                {
                    countFrequencyDetected++;
                    countWrongFrequencyDetected = 0;
                }
                else if (countWrongFrequencyDetected * Time.deltaTime < maxWrongRangeFrequency * Time.deltaTime)
                {
                    countWrongFrequencyDetected++;
                    countFrequencyDetected++;
                }
                else
                {
                    // si la frecuencia detectada es lo suficientemente larga entonces la devuelve
                    if (countFrequencyDetected * Time.deltaTime > minDurationFrequency * Time.deltaTime && currentFrequency > 0)
                    {
                        res = new Utils.Pair<int, uint>(currentFrequency, countFrequencyDetected);
                    }

                    countFrequencyDetected = 0;
                    countWrongFrequencyDetected = 0;

                    currentFrequency = maxSmallSize.frequency;
                    countFrequencyDetected++;
                }
            }
            // si la intensidad es demasiado baja reiniciamos contadores y contabilizamos como silencio
            else if (maxBigSize.intensity < minIntensityDetection && allFrequencies.intensity < minAllIntensity)
            {
                if (countFrequencyDetected * Time.deltaTime > minDurationFrequency * Time.deltaTime && currentFrequency > 0)
                {
                    res = new Utils.Pair<int, uint>(currentFrequency, countFrequencyDetected);
                }

                countFrequencyDetected = 0;
                countWrongFrequencyDetected = 0;
            }

            return res;
        }
        #endregion
    }
}