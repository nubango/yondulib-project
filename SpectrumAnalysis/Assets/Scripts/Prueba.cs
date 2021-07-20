using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lasp;
/*
 *RESOLUCION:512
 * VENTANA DE 20 [~4% de la resolucion]
 *  POSICION EN EL ARRAY
 *  -silbidos: entre 200 y 300
 *  -palmas y chasquidos > 300 (400 y pico)
 *  -golpes graves: 20 ~ 200 (entre 100 y 170)
 *  
 *  INTENSIDAD 
 *  -silbidos:
 *      -graves y agudos: 5~8
 *      -medios: 9~13
 *  -palmas y golpes: 7,8,9 y 10 
 *  
 * VENTANA DE 100 [~20% de la resolucion]
 *  POSICION EN EL ARRAY
 *  -palmas ~ 300
 *  -chasquidos ~ 500
 *  
 *  INTENSIDAD 
 *  -silbidos:
 *      -graves, medios y agudos: (<25)~40 (sobre 25-27)
 *  -palmas, golpes y chasquidos: 40~70 
 *  
 * Nº de detecciones por acción
 *  -palmadas: 30~70 (media de 40)
 *  -chasquidos: 3~18 (media 5-7)
 * SUMA DE LAS FRECUENCIAS
 *  -chasquidos: 100~374
 *  
 * **/


/* Clase pair que usamos para guardar internamente los diferentes tramos en los que dividimos el espectro
 * https://stackoverflow.com/questions/166089/what-is-c-sharp-analog-of-c-stdpair*/
public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};

public class Prueba : MonoBehaviour
{
    #region UNITY_REGION

    #region public_attributes
    [Tooltip("Objeto a mover")]
    public GameObject _target;
    [Tooltip("Limite de volumen a partir del cual empieza a analizar la entrada")]
    public int limit;
    [Tooltip("Tamaño de la ventana en la ventana deslizante")]
    public int windowSize;
    [Tooltip("Array de [0]inicio-[1]fin que representan las regiones en las que se divide el buffer del input \n\n{valores entre 0(inicio del buffer)-100(fin del buffer)} ")]
    public int[] regionsInSpactrumData;
    #endregion

    #region private_attributes
    SpectrumAnalyzer _analyzer;
    Pair<int, int>[] _regions;
    #endregion

    #region private_methods
    private void Start()
    {
        if (_analyzer == null) _analyzer = GetComponent<SpectrumAnalyzer>();

        if (regionsInSpactrumData.Length < 2)
            Debug.LogError("regionsInSpactrumData is empty,  please insert at least two items");
        _regions = new Pair<int, int>[regionsInSpactrumData.Length / 2];
        int j = 0;
        for (int i = 0; i < regionsInSpactrumData.Length; i += 2)
        {
            _regions[j] = new Pair<int, int>(regionsInSpactrumData[i] * _analyzer.resolution / 100, regionsInSpactrumData[i + 1] * _analyzer.resolution / 100);
            j++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        maxStruct max = SlidingWindow(_analyzer.logSpectrumSpan.ToArray(), windowSize);

        //Debug.Log(max.total);
        if (max.total > limit)
        {
            Debug.Log((int)max.total);

            //for (int i = 0; i < _regions.Length; i++)
            //{
            //    if (max.i > _regions[i].First && max.i < _regions[i].Second)
            //    {
            //        Debug.Log(i);
            //    }
            //}
        }
    }
    #endregion

    #endregion

    #region SLIDING_WINDOW_REGION
    private struct maxStruct
    {
        // posicion de la ventana en el array de frecuencias
        public int i;
        // suma total de las fecuencias que entran dentro de la ventana
        public float total;
        public maxStruct(int i, float total)
        {
            this.i = i;
            this.total = total;
        }
    }

    /*
     * ventana deslizante: https://stackoverflow.com/questions/8269916/what-is-sliding-window-algorithm-examples
     * **/
    private maxStruct SlidingWindow(float[] array, int ancho)
    {
        float total = 0;
        int i = 0;
        // sumaos los "ancho" primeras posiciones del array
        while (i < ancho) total += array[i++];

        maxStruct max = new maxStruct(i, total);

        // desplazamos la ventana deslizante quitando a y sumando d
        //[a b c] d e f
        //a [b c d] e f
        while (i < array.Length)
        {
            total += -array[i - ancho] + array[i++];
            if (total > max.total)
            {
                max.i = i;
                max.total = total;
            }
        }

        return max;
    }
    #endregion
}

