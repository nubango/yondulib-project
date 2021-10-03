using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternRecognizer
{
    public class WhistleRecognizer2 : MonoBehaviour
    {

        /*
         * ventana deslizante: https://stackoverflow.com/questions/8269916/what-is-sliding-window-algorithm-examples
         * **/
        /* IDEAS:
         * Para ello analizamos el espectro de frecuencias y con el patron de ventana deslizante
         * hayando el desfase entre la maxima y minima frecuencia. Si el desfase es mayor que un valor 
         * entonces supondremos que es un silbido. 
         *      para conseguir esto analizamos el caso base de la primera ventana hayando el maximo y el minimo
         *      para calcular la diferencia que sera la maxima hasta ahora.
         *      
         *      para calcular los maximos y minimos al mover la ventana usamos colas de prioridad para tener 
         *      todos los elementos ordenados y que el primer elemento sea el maximo o el minimo. 
         *      
         *      Quitar un elemento de la cola de prioridad que no sea el primero es complicado pero en realidad 
         *      lo unico que nos importa es el primer elemento de cada cola por lo que cuando se mueva la ventana 
         *      lo unico que tenemos que hacer es saber si el elemento que sale de la ventana es un maximo o un minimo. 
         *      Si estamos en este supuesto, lo que tenemos que hacer es sacar ese elemento de la cola e introducir el nuevo.
         *      
         *      Si el elemento que sale de la ventana no es ni maximo ni minimo no nos importa por lo que no lo sacamos y lo unico que hacemos es
         *      meter el nuevo elemento ya que el antiguo no interfiere en nada.
         * 
         * Devolvemos un valor entre 0 y 1, siendo 1 deteccion silbido al 100%, 0 no deteccion silbido y 
         * 0.5 posible deteccion de silbido al 50%
         * 
         * 
         * cosas utiles y como hacerlo:
         *      fijarse en la cantidad de picos que hay. En un silbido deberia de haber pocos picos. Puede darse el supuesto de que haya un poco de ruido, por lo que 
         *      aqui entraria la diferencia entre maximos y minimos. Tambien hay que ver que los picos en un silbido estan en la parte central del espectro. Estos tres 
         *      parámetros deberian usarse para elaborar el indice de acierto del audio escuchado en relacion con si lo que suena es un silbido o no.
         * 
         * **/

        /* 
         * Lo que queremos conseguir con este método es saber si se ha silbado o no. 
         * 
         * Para ello analizaremos tres parametros. Cada uno de ellos aportará un valor maximo de 0.33 aproximadamente, por lo que la suma de los tres dara como maximo 1. 
         *    En primer lugar el numero de picos que contiene el array. Cuantos más picos contenga menos probabilidades hay de que sea un silbido.
         *    
         *    El segundo factor es la amplitud de esos picos. Si los picos son muy pronunciados, la amplitud es grande, entonces hay mas probabilidades de que sea un
         *    silbido.
         *    
         *    El tercer parámetro es la posicion de la frecuencia. Un silbido no llega a notas graves ni agudas, se queda en el centro, mas especificamente entre el primer
         *    tercio y la mitad del array (1/3 - 1/2)  {---[--]-----}
         * 
         * **/
        public float factorScaleWindow = 0.2f;
        public float maxDiffValue = 0.2f;

        public class Note : System.IComparable
        {
            public float intensity;
            public int frequency;

            public Note(float i, int f)
            {
                intensity = i;
                frequency = f;
            }

            public int CompareTo(object obj)
            {
                int val = 0;
                if (((Note)obj).intensity < intensity) val = 1;
                else if (((Note)obj).intensity > intensity) val = -1;

                return val;
            }
        }

        public float WhistleIdentifier(float[] array)
        {

            #region factor 1: numero de picos
            /*
             * cuenta los picos que hay y despues saca el porcentaje de afinidad con el silbido (pocos picos -> muy afin con silbido)
             * lo multiplicamos por 0.33 porque hay otros dos factores que sumados conforman el 100% 
             * **/
            int countFrecActivas = 0;
            float last = 0.01f;
            for (int j = 0; j < array.Length; j++)
            {
                if (array[j] > last && j + 1 < array.Length && array[j] > array[j + 1])
                    countFrecActivas++;
                last = array[j];
            }

            // si no hay sonido (countFrecActivas = 0) -> factor = 0
            // si hay sonido (countFrecActivas > 0) -> countFrecActivas = 1 => factor = 0,333
            //                                      -> countFrecActivas = array.Length => factor = 0,000001
            // ((-x/(length/4)) + 1) * 0.33 (length/4 => ¿1 de cada 4 son picos?-> 0% afinidad)
            float factor1 = countFrecActivas == 0 ? 0 : (-((float)countFrecActivas / (array.Length / 6)) + 1) * 0.33f;

            //Debug.Log(factor1);
            #endregion


            #region factor 2 y 3: diferencia de entre intensidad max-min y estudio posicion de la frecuencia
            int ancho = (int)(factorScaleWindow * array.Length);

            int i = 0;
            Note maxi = new Note(array[0], i);
            Note min = new Note(array[0], i);

            Utils.PriorityQueue<Note> pqMaxs = new Utils.PriorityQueue<Note>(true);
            Utils.PriorityQueue<Note> pqMins = new Utils.PriorityQueue<Note>();

            // buscamos las frecuencias maxima y minima de la ventana para saber cual es la diferencia
            do
            {
                if (array[i] > maxi.intensity)
                {
                    maxi.intensity = array[i];
                    maxi.frequency = i;
                }
                else if (array[i] < min.intensity) min.intensity = array[i];
            } while (i++ < ancho);

            Note maxDiff = new Note(maxi.intensity - min.intensity, maxi.frequency);

            // metemos el maximo y el minimo en las colas de prioridad
            pqMaxs.Enqueue(maxi);
            pqMins.Enqueue(min);

            // calculamos la diferencia de intensidad de todo el espectro
            //[a b c] d e f
            //a [b c d] e f
            while (i < array.Length)
            {
                if (array[i] > maxi.intensity)
                {
                    maxi.intensity = array[i];
                    maxi.frequency = i;
                }
                // si el elemento que sale es el maximo o el minimo lo sacamos de la cola
                if (pqMaxs.Peek().intensity == array[i - ancho])
                    pqMaxs.Dequeue();
                else if (pqMins.Peek().intensity == array[i - ancho])
                    pqMins.Dequeue();

                // introducimos el nuevo elemento a la cola
                pqMaxs.Enqueue(new Note(array[i], i));
                pqMins.Enqueue(new Note(array[i], i));

                // comprobamos si la dif de esta ventana supera a la dif maxima hasta ahora
                float aux = pqMaxs.Peek().intensity - pqMins.Peek().intensity;
                if (aux > maxDiff.intensity)
                {
                    maxDiff.intensity = aux;
                    maxDiff.frequency = pqMaxs.Peek().frequency;
                }

                i++;
            }

            // los valores de los elementos del array estan entre 0 y 1 por lo que el valor de maxDiff.intensity oscila mas o menos entre 0 y 0.5.
            // Este valor lo multiplicamos por 0.6 para obtener unos valores entre 0 y 0.33 aproximadamente.
            float factor2 = maxDiff.intensity * 0.6f;

            /*
             * factor3 analiza la posicion en frecuencia del valor mas alto del array. El valor normal de un silbido esta comprendido entre 1/3 y 1/2 del array, por lo que
             * los valores entre ese rango obtendrán 0.33. Los valores que estan por encima o por debajo de ese intervalo obtienen una puntuacion entre 0.15 y 0, siendo
             * 0.15 el valor mas proximo al intervalo y 0 el valor mas alejado.
             * 
             * **/
            float factor3 = 0;
            if (maxDiff.frequency == 0) { }
            else if ((maxDiff.frequency < array.Length * 0.5f) && (maxDiff.frequency > array.Length * 0.33f)) factor3 = 0.33f;
            else if (maxDiff.frequency > array.Length * 0.5f)
                factor3 = ((-(maxDiff.frequency - (0.5f * array.Length)) / array.Length) + 1) * 0.15f;
            else if (maxDiff.frequency < array.Length * 0.33f)
                factor3 = (maxDiff.frequency / array.Length) * 0.15f;

            #endregion


            return factor1 + factor2 + factor3;
        }
    }
}