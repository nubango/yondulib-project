using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YonduLib.Recognizers;

/*
IMPORTANTE:

  El funcionamiento debe ser como un joystick que se mueve en una sola direccion, 
  puede estar quieto en el centro (no se silba), puede estar arriba (silbido agudo)
  o puede estar abajo (silbido grave) y debe de estar mandando eventos constantemente 
  mientras se detecte dicha frecuencia. 
 */

/*
Al generar el evento recogemos la mayor cantidad de parámetros y mas arriba decidiran que valor tener en cuenta 
- Silbido: intensidad y frecuencia (TODO: si silbas de seguido mucho tiempo se generan muchos eventos)
- Chasquido: intensidad
*/

/*
 SoundEventManager tambien se encarga de añadir un dispositivo al inputSystem de tipo MyDevice. 
 Ahora lo añadimos de forma manual desde el editor. Si no lo añadimos da error al intentar crear el evento correspondiente
 */

namespace YonduLib
{
    /*
     * Clase que se encarga de gestionar los SoundRecognizers y les proporciona el array de frecuencias a cada uno
     * **/
    public class SoundEventManager : MonoBehaviour
    {
        #region UNITY_IMPLEMENTTION

        // ----------- ATTRIBUTES ---------- //

        public SpectrumAnalyzer analyzer;


        // ------------ METHODS ------------ //

        // TODO: refactorizar para que añadir otro supuesto recognizer distinto sea facil y no haya que añadir mucho codigo
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                TransferInfo();
                DestroyImmediate(this.gameObject);
                return;
            }

            _instance = this;

            InitInformation();
            
            DontDestroyOnLoad(this);
        }

        private void TransferInfo()
        {
            _instance.analyzer = analyzer;
            _instance.medidorPrefab = medidorPrefab;
            _instance.canvas = canvas;
        }

        private void InitInformation()
        {
            //CustomeDevice.MyDevice.Initialize();

            if (analyzer == null)
                Debug.LogError("Atributo SpectrumAnalyzer no asignado en el componente SoundEventManager");

            if (_recognizers == null)
            {
                _recognizers = new List<SoundRecognizer>();

                // El orden en el que de meten debe ser el mismo del enum COMBONAME
                // de la clase COMBO para que la creacion de los combos sea correcta
                _recognizers.Add(new ClickRecognizer(EventName.Click, analyzer.resolution));
                _recognizers.Add(new WhistleRecognizer(EventName.Whistle, analyzer.resolution));
            }

            if (_medidor == null && _medidorInitSize == null)
            {
                int numRecognizers = _recognizers.Count;

                //_selector = Instantiate(selectorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                //_selector.transform.SetParent(canvas.transform, false);
                //_selector.gameObject.SetActive(false);


                _medidor = new Medidor[numRecognizers];
                _medidorInitSize = new Vector3[numRecognizers];

                for (int i = 0; i < numRecognizers; i++)
                {
                    _medidor[i] = Instantiate(medidorPrefab, new Vector3(i * 100, 0, 0), Quaternion.identity);
                    _medidor[i].transform.SetParent(canvas.transform, false);
                    _medidor[i].gameObject.SetActive(activeMedidores);

                    _medidorInitSize[i] = _medidor[i].transform.localScale;
                }
            }
        }

        private void Update()
        {
            // si no se ha seleccionado ningun dispositivo de entrada no hay sonido que analizar
            if (!analyzer.DeviceSelected)
                return;


            float[] array = analyzer.logSpectrumSpan.ToArray();

            //_selector.SetActive(activeSelector);

            for (int i = 0; i < _recognizers.Count; i++)
            {
                float valueRecognize = _recognizers[i].Recognize(array);

                if (_medidor != null && _medidor[i] != null)
                {
                    _medidor[i].gameObject.SetActive(activeMedidores);
                    if (activeMedidores) updateMedidor(valueRecognize, i);
                }
            }
        }

        #endregion


        #region GRAPHIC_REGION

        // ----------- ATTRIBUTES ---------- //

        public Medidor medidorPrefab;
        public Canvas canvas;
        public bool activeMedidores;

        private Medidor[] _medidor = null;
        private Vector3[] _medidorInitSize;

        // ------------ METHODS ------------ //

        /*
         * Metodo que se encarga de actualizar los medidores graficos de los recognizers
         * **/
        private void updateMedidor(float valueRecognize, int it)
        {
            if (_medidor == null || it >= _medidor.Length || _medidor[it] == null)
                return;

            Image i = _medidor[it].Green.GetComponentInChildren<Image>();
            MeshRenderer m = _medidor[it].Green.GetComponentInChildren<MeshRenderer>();

            if (i != null)
            {
                i.transform.localScale = new Vector3(_medidorInitSize[it].x, _medidorInitSize[it].y * valueRecognize, _medidorInitSize[it].z);
                i.color = Color.Lerp(Color.red, Color.green, valueRecognize);
            }
            else if (m != null)
            {
                m.transform.localScale = new Vector3(_medidorInitSize[it].x, _medidorInitSize[it].y * valueRecognize, _medidorInitSize[it].z);
                m.material.color = Color.Lerp(Color.red, Color.green, valueRecognize);
            }
        }
        #endregion


        #region RECOGNIZER_REGION

        // ----------- ATTRIBUTES ---------- //

        // Lista con los reconocedores que hay
        private List<SoundRecognizer> _recognizers = null;


        // ------------ METHODS ------------ //

        /*
         * Devuelve la resolucion a la que se está captando el sonido
         * **/
        public int GetResolution() => analyzer.resolution;



        /*
        TODO:
        que los medidores se hagan de forma automatica, al añadir un roundRecognizer 
        automaticamente se genera una barra verde para mostrar el feedback de ese reconocedor
         */

        //public void AddRecognizer(SoundRecognizer sr) => _soundRecognizers.Add(sr);

        //public bool RemoveRecognizer(SoundRecognizer sr)
        //{
        //    if (_soundRecognizers.Count > 0)
        //        return _soundRecognizers.Remove(sr);

        //    return false;
        //}

        //public List<SoundRecognizer> GetRecognizers() => _soundRecognizers;

        //public void SetReconizerDelegate()
        //{
        //}

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