using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    /*
     * minIntensityDetection = 7;
     * maxAllIntensity = 80;
     * minAllIntensity = 20;
     * 
     * **/
    public class WhistleRecognizer : SoundRecognizer
    {
        #region PRIVATE_ATTRIBUTES
        private int windowSizeBig = 100;
        private int windowSizeSmall = 20;

        // cuenta de las veces que se detecta una frecuencia y un silencio respectivamente
        private uint countFrecuencyDetected = 0;
        private uint countSilenceDetected = 0;

        // rango de error al comprobar la duracion y la frecuencia del silbido 
        private float offsetDuration = 0.1f;
        private float offsetFrequency;
        #endregion

        #region PUBLIC_ATTRIBUTES
        // minima intensidad que tiene que detectar para que contabilice como que ha habido un silbido
        public int minIntensityDetection = 0;

        // maxima y minima intensidad de toda la muestra. Valores acordes con los de un silbido
        public int maxAllIntensity = 0;
        public int minAllIntensity = 0;

        // Frecuencia que tiene que ser identificada y su duracion
        public uint frequency;
        public uint frequencyDuration;
        #endregion


        public int debugFrequency;

        /*
         * Primera criba en la intensidad. Establecemos un valor maximo y minimo (en ambos tamaños de ventana)
         * 
         * Identificacion de la frecuencia. El resultado final es que se "grabara" el silbido y hay que identificar 
         * y guardar el rango de frecuencias donde se encuentra. El método Recognize(...) lo que hace es identificar si esta dentro de ese rango.
         * Tambien guardar el tiempo que dura la frecuencia (tambien en forma de rango para tener menor tasa de fallo)
         * 
         * Tambien identificar ascendentes y descendentes. De esta forma, cuando se cambie de frecuencia, se guarda esa frecuencia 
         * y el tiempo que ha durado y se pone a contar el tiempo de la nueva frecuancia.
         * **/
        private void Start()
        {
            offsetFrequency = 0.01f * SoundEventManager.Instance.GetResolution();
        }

        public override bool Recognize(float[] array)
        {
            bool isClick = false;

            // ventana deslizante con tamaño de ventana grande
            WindowUnit maxBig = SlidingWindow(array, windowSizeBig);
            WindowUnit allFrequencies = SlidingWindow(array, array.Length - 1);

            //Debug.Log((int)max.intensity);
            //Debug.Log((int)maxBig.intensity + " - " + (int)allFrequencies.intensity);
            // Si la intensidad supera un limite asumimos que estamos oyendo un chasquido
            if (maxBig.intensity > minIntensityDetection && allFrequencies.intensity > minAllIntensity && allFrequencies.intensity < maxAllIntensity)
            {
                WindowUnit maxSmall = SlidingWindow(array, windowSizeSmall);

                // Comprobamos que fecuencia del input esta dentro del rango de la fecuencia buscada. Si no, reiniciamos el contador
                if (maxSmall.frequency < (frequency + offsetFrequency) && maxSmall.frequency > (frequency - offsetFrequency))
                {
                    countFrecuencyDetected++;
                    debugFrequency = maxSmall.frequency;
                }
                else
                    countFrecuencyDetected = 0;

                countSilenceDetected = 0;
            }
            else if (maxBig.intensity < minIntensityDetection && allFrequencies.intensity < minAllIntensity)
            {
                countSilenceDetected++;
                countFrecuencyDetected = 0;
            }

            // Comprobamos que la duracion de la frecuencia esta dentro del rango establecido
            if(countFrecuencyDetected < (frequencyDuration + (frequencyDuration*offsetDuration)) && 
               countFrecuencyDetected > (frequencyDuration - (frequencyDuration * offsetDuration)))
            {
                isClick = true;
            }


            return isClick;
        }
    }
}