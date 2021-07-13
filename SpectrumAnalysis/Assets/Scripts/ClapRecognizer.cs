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
        private int minCountClapDetection = 20;

        // suma de todas las intensidades por encima del umbral de intensidad (minIntensityClapDetection)
        private int countDetected = 0;

        // timer que gestiona el tiempo que hay entre una palmada y otra
        private float elapsedTime = 0;
        private float timeBetweenClapping = 0.5f;
        #endregion

        /*
         * Cuenta las veces que detecta una palmada. Cuando la cuenta esta por encima del umbral, 
         * detectaria que ha habido una palmada y devolveria true. El resto del tiempo devuelve false.
         * 
         * Al final hacemos un timer para evitar dobles positivos que se dan cuando hay una palmada muy fuerte 
         * y puede detectar dos palmadas cuando en verdad solo ha habido una
         * **/
        public override bool Recognize(float[] array)
        {
            bool isClap = false;
            elapsedTime += Time.deltaTime;

            // ventana deslizante con tamaño de ventana grande
            WindowUnit max = SlidingWindow(array, windowSizeBig);

            // Si la intensidad supera un limite asumimos que estamos oyendo una palmada
            if (max.intensity > minIntensityClapDetection)
                // contamos cuantas iteraciones dura la palmada para saber si de verdad es una palmada
                countDetected++;
            else
                countDetected = 0;

            if(countDetected > minCountClapDetection)
            {
                if (elapsedTime > timeBetweenClapping)
                {
                    elapsedTime = 0;
                    isClap = true;
                    countDetected = 0;
                }
            }

            return isClap;
        }
    }
}