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
        private int minIntensityClapDetection = 40;

        // minimo y maximo de detecciones seguidas que tiene que haber para que se considere un chasquido
        private int minCountClapDetection = 3;
        private int maxCountClapDetection = 18;

        // suma de todas las intensidades por encima del umbral de intensidad (minIntensityClapDetection)
        private int countDetected = 0;
        private int totalCountDetected = 0;
        public int totalCountDetectedDebug = 0;

        // timer que gestiona el tiempo que hay entre un chasquido y otro
        private float elapsedTime = 0;
        private float timeBetweenClapping = 0.5f;
        #endregion

        /*
         * Cuenta las veces que detecta un chasquido. Cuando la cuenta esta por encima del umbral, 
         * detectaria que ha habido un chasquido y devolveria true. El resto del tiempo devuelve false.
         * 
         * Al final hacemos un timer para evitar dobles positivos que se dan cuando hay un chasquido muy fuerte 
         * y puede detectar dos chasquidos cuando en verdad solo ha habido uno
         * **/
        public override bool Recognize(float[] array)
        {
            bool isClick = false;
            elapsedTime += Time.deltaTime;

            // ventana deslizante con tamaño de ventana grande
            WindowUnit max = SlidingWindow(array, windowSizeBig);

            // Si la intensidad supera un limite asumimos que estamos oyendo una palmada
            if (max.intensity > minIntensityClapDetection)
            {
                // contamos cuantas iteraciones dura la palmada para saber si de verdad es una palmada
                countDetected++;
                totalCountDetected++;
            }
            else
                countDetected = 0;

            if (countDetected == 0 && totalCountDetected > minCountClapDetection && totalCountDetected < maxCountClapDetection)
            {
                if (elapsedTime > timeBetweenClapping)
                {
                    totalCountDetectedDebug = totalCountDetected;
                    elapsedTime = 0;
                    isClick = true;
                    totalCountDetected = 0;
                }
            }
            else if (countDetected == 0 && totalCountDetected > maxCountClapDetection)
                totalCountDetected = 0;

            return isClick;
        }
    }
}