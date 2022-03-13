using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
IMPORTANTE:
  Lo de los combos seguramente cambie porque al meter los eventos del customedevice 
  dentro del soundrecognizer ya no nos hace falta el metodo getEvent()

  A lo mejor hay que revisar como generamos el evento de silbido por que 
  ahora se genera un evento de silvido al detectar una frecuencia pero si estamos 
  silbando 10 segundos en la misma frecuencia solo se generará el primer evento y luego nada.

  El funcionamiento debe ser como un joystick que se mueve en una sola direccion, 
  puede estar quieto en el centro (no se silba), puede estar arriba (silbido agudo)
  o puede estar abajo (silbido grave) y debe de estar mandando eventos constantemente 
  mientras se detecte dicha frecuencia. 

  Ahora mismo se genera un evento silbido con una deadzon (_offsetFrequency) 
  excesivamente alta; deadzone del 5% de la resolucion, es decir, con resolucion 
  de 256 la deadzone es de 12 por arriba y 12 por abajo que equivale a 24.
 */

/*
Al generar el evento recogemos la mayor cantidad de parámetros y mas arriba decidiran que valor tener en cuenta 
- Silbido: intensidad y frecuencia (TODO: si silbas de seguido mucho tiempo se generan muchos eventos)
- Chasquido: intensidad
*/

namespace PatternRecognizer
{
    /*
     * Clase que se encarga de gestionar los SoundRecognizers y les proporciona el array de frecuencias a cada uno
     * **/
    public class SoundEventManager : MonoBehaviour
    {
        #region UNITY_IMPLEMENTTION
        public GameObject[] medidor;
        public SpectrumAnalyzer analyzer;

        private Vector3[] medidorInitSize;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;

                if (analyzer == null)
                    // TODO: cambiar a un hijo spectrumAnalizer que tenga el componente y asignarselo a esta clase por el editor
                    Debug.LogError("Atributo SpectrumAnalyzer no asignado en el componente SoundEventManager");

                if (_recognizers == null)
                {
                    _recognizers = new List<SoundRecognizer>();

                    // El orden en el que de meten debe ser el mismo del enum COMBONAME
                    // de la clase COMBO para que la creacion de los combos sea correcta
                    _recognizers.Add(new ClickRecognizer(EventName.Click));
                    _recognizers.Add(new WhistleRecognizer(EventName.Whistle));
                }

                if (medidor != null && medidorInitSize == null)
                {
                    visibleMedidor = new bool[medidor.Length];
                    medidorInitSize = new Vector3[medidor.Length];
                    for (int i = 0; i < medidor.Length; i++)
                    {
                        if (medidor[i] != null)
                        {
                            medidorInitSize[i] = medidor[i].transform.localScale;
                            visibleMedidor[i] = true;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            // si no se ha seleccionado ningun dispositivo de entrada no hay sonido que analizar
            if (!analyzer.DeviceSelected)
                return;


            float[] array = analyzer.logSpectrumSpan.ToArray();

            for (int i = 0; i < _recognizers.Count; i++)
            {
                float valueRecognize = _recognizers[i].Recognize(array);

                if (medidor != null && medidor[i] != null)
                {
                    medidor[i].SetActive(visibleMedidor[i]);
                    if (visibleMedidor[i]) updateMedidor(valueRecognize, i);
                }
            }
        }

        #endregion

        #region PUBLIC_METHODS
        //public void AddRecognizer(SoundRecognizer sr) => _soundRecognizers.Add(sr);

        //public bool RemoveRecognizer(SoundRecognizer sr)
        //{
        //    if (_soundRecognizers.Count > 0)
        //        return _soundRecognizers.Remove(sr);

        //    return false;
        //}

        //public List<SoundRecognizer> GetRecognizers() => _soundRecognizers;

        public void SetReconizerDelegate()
        {
        }
        public int GetResolution() => analyzer.resolution;

        public void SetVisibleMedidor(int i, bool b) { visibleMedidor[i] = b; }
        public bool IsVisibleMedidor(int i) { return visibleMedidor[i]; }
        #endregion

        #region PRIVATE_METHODS
        /*
         * Metodo que se encarga de actualizar los medidores graficos de los recognizers
         * **/
        private void updateMedidor(float valueRecognize, int it)
        {
            if (medidor == null || it >= medidor.Length || medidor[it] == null)
                return;

            Image i = medidor[it].GetComponentInChildren<Image>();
            MeshRenderer m = medidor[it].GetComponentInChildren<MeshRenderer>();

            if (i != null)
            {
                i.transform.localScale = new Vector3(medidorInitSize[it].x, medidorInitSize[it].y * valueRecognize, medidorInitSize[it].z);
                i.color = Color.Lerp(Color.red, Color.green, valueRecognize);
            }
            else if (m != null)
            {
                m.transform.localScale = new Vector3(medidorInitSize[it].x, medidorInitSize[it].y * valueRecognize, medidorInitSize[it].z);
                m.material.color = Color.Lerp(Color.red, Color.green, valueRecognize);
            }
        }

        #endregion


        #region PUBLIC_ATTRIBUTES
        public bool[] visibleMedidor = null;
        #endregion

        #region PRIVATE_ATTRIBUTES
        // Lista con los reconocedores que hay
        private List<SoundRecognizer> _recognizers = null;

        #endregion

        #region SINGLETON_REGION
        /*
         * sigleton pattern in unity: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
         * **/
        private static SoundEventManager _instance = null;
        public static SoundEventManager Instance => _instance;
        #endregion
    }
}