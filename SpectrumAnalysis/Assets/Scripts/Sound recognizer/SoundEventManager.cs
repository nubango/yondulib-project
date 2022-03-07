using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 Clase combo:
    - diccionario que contiene el combo (no se de que tipo van a ser pero tienen que entrar los tipos de sibidos diferentes -> guardar frecuencia y duracion de alguna manera y distinguir los silbidos entre ellos y entre palmas y chasquidos) 

(igual esta clase no se tiene que usar) Clase WhistleFrequencyRecognizer: cada vez que se llame al metodo recognize hay que saignar los valores frequency y frequencyDuration que queramos identificar


Esta clase identifica los combos y gestiona cuando ha acabado un combo y cuando no (el tiempo que hay silencio)

El metodo de la clase combo recibe un atributo ComboName y otro WhistleData (si no es silbido los valores son negarivos)
Cada combo se encarga de saber si la secuencia que lleva es la correcta o no. Si se pasa un ComboName de tipo Silence entonces se devuelve true o false (si coincide o
no) y se reinicia todo.

(Como est� hecho lo de los silbidos no me acaba de convencer porque para detectar un ascenso o descenso de frecuencia no se si como lo planteo va a funcionar)

 */


/*
Combo:
 silbar -> moverse hacia la derecha
 silbar+chasquido -> cambio de sentido
 chasquido -> saltar
 chasquido+chasquido -> accionX
 silbido ascendente -> accionY (whistlerecognizer guarda la frecuencia con mayor intensidad de la muestra)

Efecto visual para feedback:
 crear una clase que tenga un metodo que admita por par�metro un float (0-1) y que lo tranforme en un cambio visual en el atributo visual de la clase (asignado desde el editor?).
 Estos objetos se asignan en el editor al SoundEventManager, uno para cada recognizer
 */


namespace PatternRecognizer
{
    /*
     * Clase que se encarga de gestionar los SoundRecognizers y les proporciona el array de frecuencias a cada uno
     * **/
    public class SoundEventManager : MonoBehaviour
    {
        #region UNITY_REGION
        public GameObject[] medidor;

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
                
                if (_analyzer == null)
                    // TODO: cambiar a un hijo spectrumAnalizer que tenga el componente y asignarselo a esta clase por el editor
                    _analyzer = GetComponent<SpectrumAnalyzer>();
                    //_analyzer = GetComponent<Expample_libsoundio.SpectrumAnalyzer>();

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
                _combos = new List<Combo>();
                _currentEventsNames = new List<EventName>();
                _currentEventsFrequencies = new List<float>();
                _countSilenceDetected = new int[_recognizers.Count];

            }
        }

        private void Update()
        {
            // si no se ha seleccionado ningun dispositivo de entrada no hay sonido que analizar
            if (!_analyzer.DeviceSelected)
                return;


            float[] array = _analyzer.logSpectrumSpan.ToArray();

            int minCountSilence = _countSilenceDetected[0];

            for (int i = 0; i < _recognizers.Count; i++)
            {
                float valueRecognize = _recognizers[i].Recognize(array);


                RecordCombo(i);
                minCountSilence = minCountSilence > 0 && _countSilenceDetected[i] < minCountSilence ? _countSilenceDetected[i] : minCountSilence;


                if (medidor != null && medidor[i] != null)
                {
                    medidor[i].SetActive(visibleMedidor[i]);
                    if (visibleMedidor[i]) updateMedidor(valueRecognize, i);
                }
            }


            if (minCountSilence * Time.deltaTime > _minSilenceToCombo * Time.deltaTime && _currentEventsNames.Count > 0)
            {
                if (isRecordCombo)
                    _combos.Add(new Combo((++comboID).ToString(), _currentEventsNames.ToArray(), _currentEventsFrequencies.ToArray()));
                else
                    // comparar el combo con los combos que ya hay
                    foreach (Combo c in _combos)
                    {
                        Debug.Log(c._name + " -> " + c.Recognizer(_currentEventsNames.ToArray(), _currentEventsFrequencies.ToArray()));
                    }

                _currentEventsNames.Clear();
                _currentEventsFrequencies.Clear();
            }
        }

        //debug
        int comboID = 0;
        //debug

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
        public int GetResolution() => _analyzer.resolution;

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

        /*
         * Metodo que se encarga de guardar cada uno de los eventos para la posterior creacion del combo
         * **/
        private void RecordCombo(int it)
        {
            SoundEvent ev = _recognizers[it].GetEvent();
            if (ev.name != EventName.Null && ev.name != EventName.Silence)
            {
                _countSilenceDetected[it] = 0;
                _currentEventsNames.Add(ev.name);
                _currentEventsFrequencies.Add(ev.frequency);
            }
            else if (ev.name == EventName.Silence)
            {
                _countSilenceDetected[it]++;
            }
        }

        /*
        metodo en cada recognizer que gestiones la salida de recognize(...) y que inicialice los dos atributos(name y data) y que haya otros dos metodos que devuelvan 
        dichos atributos, respectivamente, y que sean metodos destructivos, es decir, que cuando se llamen, devuelven la informacion y ponen a null los atributos.Mientras 
        no reconozca nada estaran a null pero cuando reconoce el evento lo mete en el atributo. Esta estructura obliga a llamar en cada vuelta a los metodos ya que si no se
        llaman es posible que se pierdan eventos porque en cada tick se sobreescriben los datos del tick anterior.

        La creacion del combo la hace SoundEventManager y es el que junta la salida de todos los recognizers (teniendo en cuenta el tiempo y eso)

        */
        #endregion


        #region PUBLIC_ATTRIBUTES
        // flag para grabar los combos
        public bool isRecordCombo = false;
        public bool[] visibleMedidor = null;
        #endregion
        
        #region PRIVATE_ATTRIBUTES
        // Clase que contiene el array con el sonido de entrada
        private SpectrumAnalyzer _analyzer = null;
        //private Expample_libsoundio.SpectrumAnalyzer _analyzer = null;

        // Lista con los reconocedores que hay
        private List<SoundRecognizer> _recognizers = null;

        // lista que guarda los combos grabados
        private List<Combo> _combos;

        // lista de los items del combo que se esta creando en este momento
        private List<EventName> _currentEventsNames;

        // lista de los datos del combo que se esta creando en este momento
        private List<float> _currentEventsFrequencies;

        // contador de silencio por cada recognizer
        int[] _countSilenceDetected;

        // minimo silencio que tiene que haber para que el combo se de por acabado
        int _minSilenceToCombo = 100;
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