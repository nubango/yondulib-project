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

        // maximo tiempo entre chasquidos para que se contabilicen por separado
        private int maxCountSilenceDetection = 200;
        // minimo tiempo entre chanquidos para que se contabilizen como combo
        private int minCountSilenceBetweenClicks = 10;

        // cuenta de las veces que se ha detectado un posible chasquido
        private uint countFrequencyClickDetected = 0;
        // cuenta las veces que detecta silencio
        private uint countSilenceDetected = 0;

        // minimo de la intensidad total de la muestra para no confundirlo con voces y silbidos
        private int minAllIntensity = 150;

        // numero de chasquidos seguidos
        [HideInInspector]
        public int combo = 0;
        #endregion

        #region PUBLIC_ATTRIBUTES
        // Intensidad minima para considerarse un chasquido
        public int minIntensityClickDetection = 0;

        // minimo y maximo de detecciones seguidas que tiene que haber para que se considere un chasquido
        public int minCountClickDetection = 0;
        public int maxCountClickDetection = 0;
        #endregion

        /*
         * Utilizamos una ventana deslizante de tamaño grande para diferenciar los golpes de los silbidos.
         * Tambien realizamos la suma de todas las frecuencias de la muestra (tamaño maximo de la ventana)
         * para afinar más y evitar confusiones con ciertos silbidos graves.
         * 
         * El algoritmo cuenta las veces que detecta una intensidad que puede ser un chasquido. (+1 cada vez que se ejecuta el método)
         * Cuando el sumatorio esta dentro del umbral y ha habido el suficiente silencio despues, 
         * se confirma que ha habido un chasquido y devuelve true. 
         * 
         * Tambien detecta combos del mismo tipo (dos chasquidos, dos palmadas), utilizando 
         * un umbral de silencios más corto y en el que si se detecta otro chasquido suma uno al combo
         * 
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
                // si el silencio que ha habido y las iteraciones que dura el chasquido
                // estan dentro del umbral significa que ha habido dos chasquidos seguidos
                if (countSilenceDetected * Time.deltaTime > minCountSilenceBetweenClicks * Time.deltaTime &&
                    countSilenceDetected * Time.deltaTime < maxCountSilenceDetection * Time.deltaTime &&
                    countFrequencyClickDetected * Time.deltaTime > minCountClickDetection * Time.deltaTime &&
                    countFrequencyClickDetected * Time.deltaTime < maxCountClickDetection * Time.deltaTime)
                {
                    combo++;
                    countFrequencyClickDetected = 0;
                }

                // contamos cuantas iteraciones dura el chasquido para saber si de verdad es un chasquido
                countFrequencyClickDetected++;
                countSilenceDetected = 0;
            }
            else
                countSilenceDetected++;

            // si las iteraciones que dura el chasquido estan dentro del umbral y ha habido un silencio
            // lo suficientemente largo entonces contabilizamos que ha acabado el combo (combo puede ser de 1,2,3...)
            if (countFrequencyClickDetected * Time.deltaTime > minCountClickDetection * Time.deltaTime &&
                countFrequencyClickDetected * Time.deltaTime < maxCountClickDetection * Time.deltaTime &&
                countSilenceDetected * Time.deltaTime > maxCountSilenceDetection * Time.deltaTime)
            {
                isClick = true;
                countFrequencyClickDetected = 0;
            }
            // si las iteraciones que dura el chasquido estan fuera del umbral de reconocimiento
            // y ha habido un silencio lo suficientemente largo reiniciemos los contadores de combo e iteraciones
            else if ((countFrequencyClickDetected * Time.deltaTime <= minCountClickDetection * Time.deltaTime ||
                      countFrequencyClickDetected * Time.deltaTime >= maxCountClickDetection * Time.deltaTime) &&
                      countSilenceDetected * Time.deltaTime > maxCountSilenceDetection * Time.deltaTime)
            {
                countFrequencyClickDetected = 0;
                combo = 0;
            }

            return isClick;
        }
    }
}