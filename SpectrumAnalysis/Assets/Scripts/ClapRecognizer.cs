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
        #region PRIVATE_ATTRIBUTES
        // Tamaño de la ventana en la ventana deslizante (grande para detectar golpes ~20% de la resolucion)
        private int windowSizeBig = 100;
        
        // Intensidad minima para considerarse una palmada
        private int minIntensityClapDetection = 40;

        // minimo de detecciones seguidas que tiene que haber para que se considere una palmada
        private int minCountClapDetection = 25;
        private int maxCountClapDetection = 70;

        // minimo de silencios que tiene que hber despues de un chasquido para que se considere como tal
        private int minCountSilenceDetection = 18;

        // cuenta de las veces que se ha detectado un posible chasquido
        public int countFrequencyClickDetected = 0;
        // cuenta las veces que detecta silencio
        public int countSilenceDetected = 0;

        // minimo de la intensidad total de la muestra para no confundirlo con voces y silbidos
        private int minAllIntensity = 150;
        #endregion

        /*
         * Cuenta las veces que detecta una intensidad que puede ser una palmada (+1 cada vez que se ejecuta el método).
         * Cuando la cuenta esta dentro del umbral, detectaria que ha habido un palmada y devolveria true. 
         * El resto del tiempo devuelve false.
         * **/
        public override bool Recognize(float[] array)
        {
            bool isClick = false;

            // ventana deslizante con tamaño de ventana grande
            WindowUnit max = SlidingWindow(array, windowSizeBig);
            WindowUnit allFrequencies = SlidingWindow(array, array.Length - 1);

            // Si la intensidad supera un limite asumimos que estamos oyendo una palmada
            if (max.intensity > minIntensityClapDetection)
            {
                // contamos cuantas iteraciones dura la palmada para saber si de verdad es una palmada
                countFrequencyClickDetected++;
                countSilenceDetected = 0;
            }
            else
                countSilenceDetected++;

            if (countFrequencyClickDetected > minCountClapDetection && countFrequencyClickDetected < maxCountClapDetection
                && countSilenceDetected > minCountSilenceDetection)
            {
                isClick = true;
                countFrequencyClickDetected = 0;
            }
            else if ((countFrequencyClickDetected < minCountClapDetection || countFrequencyClickDetected > maxCountClapDetection) && 
                countSilenceDetected > minCountSilenceDetection)
            {
                countFrequencyClickDetected = 0;
            }


            return isClick;
        }
    }
}