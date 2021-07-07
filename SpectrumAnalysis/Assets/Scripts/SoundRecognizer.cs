using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectrumAnalyzer
{
    public abstract class SoundRecognizer : MonoBehaviour
    {
        public abstract bool Recognize(float[] array);

        #region SLIDING_WINDOW_REGION
        public struct WindowUnit
        {
            // posicion de la ventana en el array de frecuencias
            public int frequency;
            // suma total de las fecuencias que entran dentro de la ventana
            public float intensity;
            public WindowUnit(int f, float i)
            {
                frequency = f;
                intensity = i;
            }
        }

        /*
         * ventana deslizante: https://stackoverflow.com/questions/8269916/what-is-sliding-window-algorithm-examples
         * **/
        public static WindowUnit SlidingWindow(float[] array, int ancho)
        {
            float total = 0;
            int i = 0;
            // sumaos los "ancho" primeras posiciones del array
            while (i < ancho) total += array[i++];

            WindowUnit max = new WindowUnit(i, total);

            // desplazamos la ventana deslizante quitando a y sumando d
            //[a b c] d e f
            //a [b c d] e f
            while (i < array.Length)
            {
                total += -array[i - ancho] + array[i++];
                if (total > max.intensity)
                {
                    max.frequency = i;
                    max.intensity = total;
                }
            }

            return max;
        }
        #endregion
    }
}