using System;
using System.Collections.Generic;
using UnityEngine;
using Lasp;
using UnityEngine.UI;

/*
 Clase combo:
    - diccionario que contiene el combo (no se de que tipo van a ser pero tienen que entrar los tipos de sibidos diferentes -> guardar frecuencia y duracion de alguna manera y distinguir los silbidos entre ellos y entre palmas y chasquidos) 

(igual esta clase no se tiene que usar) Clase WhistleFrequencyRecognizer: cada vez que se llame al metodo recognize hay que saignar los valores frequency y frequencyDuration que queramos identificar


Esta clase identifica los combos y gestiona cuando ha acabado un combo y cuando no (el tiempo que hay silencio)

El metodo de la clase combo recibe un atributo ComboName y otro WhistleData (si no es silbido los valores son negarivos)
Cada combo se encarga de saber si la secuencia que lleva es la correcta o no. Si se pasa un ComboName de tipo Silence entonces se devuelve true o false (si coincide o
no) y se reinicia todo.

(Como está hecho lo de los silbidos no me acaba de convencer porque para detectar un ascenso o descenso de frecuencia no se si como lo planteo va a funcionar)

 */


/*
Combo:
 silbar -> moverse hacia la derecha
 silbar+chasquido -> cambio de sentido
 chasquido -> saltar
 chasquido+chasquido -> accionX
 silbido ascendente -> accionY (whistlerecognizer guarda la frecuencia con mayor intensidad de la muestra)

Efecto visual para feedback:
 crear una clase que tenga un metodo que admita por parámetro un float (0-1) y que lo tranforme en un cambio visual en el atributo visual de la clase (asignado desde el editor?).
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
        public Image medidorClick;
        public Image medidorWhistle;

        private Vector3 medidorInitClickSize;
        private Vector3 medidorInitWhistleSize;

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
                    _analyzer = GetComponent<SpectrumAnalyzer>();

                if (_clickRecognizer == null || _whistleRecognizer == null)
                {
                    _clickRecognizer = new ClickRecognizer();
                    _whistleRecognizer = new WhistleRecognizer();

                    medidorInitClickSize = medidorClick.transform.localScale;
                    medidorInitWhistleSize = medidorWhistle.transform.localScale;

                    /*
                    _combos = new List<Combo>();

                    List<ComboName> combo1 = new List<ComboName>();
                    combo1.Add(ComboName.Click);
                    combo1.Add(ComboName.Click);
                    _combos.Add(new Combo("Combo1", combo1.ToArray(), null));

                    List<ComboName> combo2 = new List<ComboName>();
                    combo2.Add(ComboName.Click);
                    combo2.Add(ComboName.Clap);
                    _combos.Add(new Combo("Combo2", combo2.ToArray(), null));

                    List<ComboName> combo3 = new List<ComboName>();
                    combo3.Add(ComboName.Clap);
                    combo3.Add(ComboName.Clap);
                    _combos.Add(new Combo("Combo3", combo2.ToArray(), null));
                    */
                }
            }
        }

        private void Update()
        {
            float valeuClick = _clickRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());
            float valeuWhistle = _whistleRecognizer.Recognize(_analyzer.logSpectrumSpan.ToArray());

            updateMedidor(valeuClick, valeuWhistle);
        }

        Vector3 velocity = Vector3.zero;


        private void updateMedidor(float c, float w)
        {
            if (medidorClick != null)
            {
                //medidorClick.transform.localScale = Vector3.SmoothDamp(medidorClick.transform.localScale, new Vector3(medidorInitClickSize.x, medidorInitClickSize.y * c, medidorInitClickSize.z), ref velocity, 0.5f);
                medidorClick.transform.localScale = new Vector3(medidorInitClickSize.x, medidorInitClickSize.y * c, medidorInitClickSize.z);
                medidorClick.color = Color.Lerp(Color.red, Color.green, c);
            }

            if (medidorWhistle != null)
            {
                //medidorWhistle.transform.localScale = Vector3.SmoothDamp(medidorWhistle.transform.localScale, new Vector3(medidorInitWhistleSize.x, medidorInitWhistleSize.y * w, medidorInitWhistleSize.z), ref velocity, 0.5f);
                medidorWhistle.transform.localScale = new Vector3(medidorInitWhistleSize.x, medidorInitWhistleSize.y * w, medidorInitWhistleSize.z);
                medidorWhistle.color = Color.Lerp(Color.red, Color.green, w);
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
        public int GetResolution() => _analyzer.resolution;
        #endregion

        #region PRIVATE_METHODS

        //private void CompareCombos(Pair<ComboName[], WhistleData[]> combo)
        //{
        //    if (combo == null)
        //        return;

        //    foreach (Combo c in _combos)
        //    {
        //        Debug.Log(c._name + " -> " + c.Recognizer(combo.First, combo.Second));
        //    }
        //}

        #endregion

        #region SINGLETON_REGION
        /*
         * sigleton pattern in unity: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
         * **/
        private static SoundEventManager _instance = null;
        public static SoundEventManager Instance => _instance;
        #endregion

        #region PRIVATE_ATTRIBUTES
        // Clase que contiene el array con el sonido de entrada
        private SpectrumAnalyzer _analyzer = null;

        private ClickRecognizer _clickRecognizer;
        private WhistleRecognizer _whistleRecognizer;

        //private List<Combo> _combos;
        #endregion
    }
}