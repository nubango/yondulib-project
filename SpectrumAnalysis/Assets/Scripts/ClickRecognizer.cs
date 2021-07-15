using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    /*
     * Clase que sirve para detectar palmadas y chasquidos en funcion de los valores que se pongan a los atributos publicos
     * -CHASQUIDOS DE DEDOS-
     * minIntensityClickDetection = 30; 
     * minCountClickDetection = 2;
     * maxCountClickDetection = 18;
     * 
     * -PALMADAS-
     * minIntensityClickDetection = 40; 
     * minCountClickDetection = 25;
     * maxCountClickDetection = 100;
     * **/

    public class ClickRecognizer : SoundRecognizer
    {
        #region PRIVATE_ATTRIBUTES
        // Tamaño de la ventana en la ventana deslizante (grande para detectar golpes ~20% de la resolucion)
        private int windowSizeBig = 100;

        // minimo de silencios que tiene que haber despues de un chasquido para que se considere como tal
        private int minCountSilenceDetection = 200;
        // maximo de silencios que tiene que haber para que se considere que ha habido dos chasquidos seguidos
        private int maxCountSilenceBetweenClicks = 10;

        // cuenta de las veces que se ha detectado un posible chasquido
        private int countFrequencyClickDetected = 0;
        // cuenta las veces que detecta silencio
        private int countSilenceDetected = 0;

        // minimo de la intensidad total de la muestra para no confundirlo con voces y silbidos
        private int minAllIntensity = 150;


        #endregion

        #region PUBLIC_ATTRIBUTES
        // Intensidad minima para considerarse un chasquido
        public int minIntensityClickDetection = 0;

        // minimo y maximo de detecciones seguidas que tiene que haber para que se considere un chasquido
        public int minCountClickDetection = 0;
        public int maxCountClickDetection = 0;
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
            if (max.intensity > minIntensityClickDetection && allFrequencies.intensity > minAllIntensity)
            {
                // contamos cuantas iteraciones dura el chasquido para saber si de verdad es un chasquido
                countFrequencyClickDetected++;
                // si el silencio que ha habido esta dentro del umbral significa que ha habido dos chasquidos seguidos con lo cual no tiene que contabilizarlo
                if (countSilenceDetected * Time.deltaTime > maxCountSilenceBetweenClicks * Time.deltaTime &&
                    countSilenceDetected * Time.deltaTime < minCountSilenceDetection * Time.deltaTime)
                    countFrequencyClickDetected += maxCountClickDetection;

                countSilenceDetected = 0;
            }
            else
                countSilenceDetected++;

            if (countFrequencyClickDetected * Time.deltaTime > minCountClickDetection * Time.deltaTime &&
                countFrequencyClickDetected * Time.deltaTime < maxCountClickDetection * Time.deltaTime &&
                countSilenceDetected * Time.deltaTime > minCountSilenceDetection * Time.deltaTime)
            {
                isClick = true;
                countFrequencyClickDetected = 0;
            }
            else if ((countFrequencyClickDetected <= minCountClickDetection || countFrequencyClickDetected >= maxCountClickDetection) &&
                      countSilenceDetected > minCountSilenceDetection)
            {
                countFrequencyClickDetected = 0;
            }

            return isClick;
        }
    }
}