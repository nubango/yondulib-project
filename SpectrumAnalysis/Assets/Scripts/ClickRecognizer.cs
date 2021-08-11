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
     * minAllIntensity = 100;
     * 
     * -PALMADAS-
     * minIntensityClickDetection = 40; 
     * minCountClickDetection = 10;
     * maxCountClickDetection = 100;
     * minAllIntensity = 140;
     * **/

    class ClickRecognizer : SoundRecognizer
    {
        #region PRIVATE_ATTRIBUTES
        // Tamaño de la ventana en la ventana deslizante (grande para detectar golpes ~20% de la resolucion)
        private int windowSize = 100;

        // minimo tiempo entre chanquidos para que se contabilizen como combo
        private readonly int minCountSilenceBetweenClicks = 10;

        // cuenta de las veces que se ha detectado un posible chasquido
        private uint countFrequencyClickDetected = 0;
        // cuenta las veces que detecta silencio
        private uint countSilenceDetected = 0;

        // minimo de la intensidad total de la muestra para no confundirlo con voces y silbidos
        private int _minAllIntensity = 0;
        // Intensidad minima para considerarse un chasquido
        private int _minIntensityClickDetection = 0;

        //// Frecuencia para distinguir si es palmada o chasquido
        //public int minFrequency = 0;
        //public int maxFrequency = 0;

        // minimo y maximo de detecciones seguidas que tiene que haber para que se considere un chasquido
        private int _minCountClickDetection = 0;
        private int _maxCountClickDetection = 0;
        #endregion

        public string _name;

        #region CONSTRUCTOR
        public ClickRecognizer(string name, int minAllIntensity, int minIntensityClickDetection, int minCountClickDetection, int maxCountClickDetection)
        {
            _name = name;
            _minAllIntensity = minAllIntensity;
            _minIntensityClickDetection = minIntensityClickDetection;
            _minCountClickDetection = minCountClickDetection;
            _maxCountClickDetection = maxCountClickDetection;
        }
        #endregion

        #region RECOGNIZE_METHODS
        /*
         * Utilizamos una ventana deslizante de tamaño grande para diferenciar los golpes de los silbidos.
         * Tambien realizamos la suma de todas las frecuencias de la muestra (tamaño maximo de la ventana)
         * para afinar más y evitar confusiones con ciertos silbidos graves.
         * 
         * El algoritmo cuenta las veces que detecta una intensidad que puede ser un chasquido. (+1 cada vez que se ejecuta el método)
         * Cuando el sumatorio esta dentro del umbral y ha habido el suficiente silencio despues, 
         * se confirma que ha habido un chasquido y devuelve true. 
         * 
         * El resto del tiempo devuelve false.
         * **/
        public override bool Recognize(float[] array)
        {
            bool isClick = false;

            // ventana deslizante con tamaño de ventana grande
            Utils.WindowUnit max = Utils.SlidingWindow(array, windowSize);
            Utils.WindowUnit allFrequencies = Utils.SlidingWindow(array, array.Length - 1);

            // Si la intensidad supera un limite asumimos que estamos oyendo un chasquido
            if (max.intensity > _minIntensityClickDetection && allFrequencies.intensity > _minAllIntensity)
            //&& max.frequency > minFrequency && max.frequency < maxFrequency)
            {
                //Debug.Log(max.frequency);
                // contamos cuantas iteraciones dura el chasquido para saber si de verdad es un chasquido
                countFrequencyClickDetected++;
                countSilenceDetected = 0;
            }
            else
                countSilenceDetected++;

            // si las iteraciones que dura el chasquido estan dentro del umbral y ha habido un silencio
            // lo suficientemente largo entonces contabilizamos que ha acabado el chasquido
            if (countFrequencyClickDetected * Time.deltaTime > _minCountClickDetection * Time.deltaTime &&
                countFrequencyClickDetected * Time.deltaTime < _maxCountClickDetection * Time.deltaTime &&
                countSilenceDetected * Time.deltaTime > minCountSilenceBetweenClicks * Time.deltaTime)
            {
                isClick = true;
                countFrequencyClickDetected = 0;
            }
            // si las iteraciones que dura el chasquido estan fuera del umbral de reconocimiento
            // y ha habido un silencio lo suficientemente largo reiniciamos iteraciones
            else if (countSilenceDetected * Time.deltaTime > minCountSilenceBetweenClicks * Time.deltaTime)
            {
                countFrequencyClickDetected = 0;
            }

            return isClick;
        }
        #endregion
    }
}