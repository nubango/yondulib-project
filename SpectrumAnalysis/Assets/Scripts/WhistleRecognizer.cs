using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    public class WhistleRecognizer : SoundRecognizer
    {
        #region PRIVATE_ATTRIBUTES
        private int windowSize = 20;
        private int minIntensityClickDetection = 0;
        private int minAllIntensity = 0;
        #endregion

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
        public override bool Recognize(float[] array)
        {
            bool isClick = false;

            // ventana deslizante con tamaño de ventana grande
            WindowUnit max = SlidingWindow(array, windowSize);
            WindowUnit allFrequencies = SlidingWindow(array, array.Length - 1);

            Debug.Log("max - " + max.intensity + " all - " + allFrequencies.intensity);

            // Si la intensidad supera un limite asumimos que estamos oyendo un chasquido
            if (max.intensity > minIntensityClickDetection && allFrequencies.intensity > minAllIntensity)
            {

            }

            return isClick;
        }
    }
}