using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectrumAnalyzer
{
    public class ClickRecognizer : SoundRecognizer
    {
        #region PRIVATE_ATTRIBUTES
        // Tamaño de la ventana en la ventana deslizante (grande para detectar golpes ~20% de la resolucion)
        private int windowSizeBig = 100;

        // Intensidad minima para considerarse un chasquido
        private int minIntensityClapDetection = 30;

        // minimo y maximo de detecciones seguidas que tiene que haber para que se considere un chasquido
        private int minCountClickDetection = 2;
        private int maxCountClickDetection = 18;

        // minimo de silencios que tiene que hber despues de un chasquido para que se considere como tal
        private int minCountSilenceDetection = 18;

        // cuenta de las veces que se ha detectado un posible chasquido
        private int countFrequencyClickDetected = 0;
        // cuenta las veces que detecta silencio
        private int countSilenceDetected = 0;

        // minimo de la intensidad total de la muestra para no confundirlo con voces y silbidos
        private int minAllIntensity = 150;


        #endregion

        /*
         * Cuenta las veces que detecta una intensidad que puede ser un chasquido.(+1 cada vez que se ejecuta el método)
         * Cuando la cuenta esta dentro del umbral, detectaria que ha habido un chasquido y devolveria true. 
         * El resto del tiempo devuelve false.
         * **/

        public override bool Recognize(float[] array)
        {
            bool isClick = false;

            // ventana deslizante con tamaño de ventana grande
            WindowUnit max = SlidingWindow(array, windowSizeBig);
            WindowUnit allFrequencies = SlidingWindow(array, array.Length - 1);

            // Si la intensidad supera un limite asumimos que estamos oyendo un chasquido
            if (max.intensity > minIntensityClapDetection && allFrequencies.intensity > minAllIntensity)
            {
                // contamos cuantas iteraciones dura el chasquido para saber si de verdad es un chasquido
                countFrequencyClickDetected++;
                countSilenceDetected = 0;
            }
            else
                countSilenceDetected++;

            if (countFrequencyClickDetected > minCountClickDetection && countFrequencyClickDetected < maxCountClickDetection
                && countSilenceDetected > minCountSilenceDetection)
            {
                isClick = true;
                countFrequencyClickDetected = 0;
            }
            else if ((countFrequencyClickDetected < minCountClickDetection || countFrequencyClickDetected > maxCountClickDetection) && 
                countSilenceDetected > minCountSilenceDetection)
            {
                countFrequencyClickDetected = 0;
            }


            return isClick;
        }
    }
}