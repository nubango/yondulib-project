using UnityEngine.UIElements;
using UnityEngine;

namespace Samples.Runtime.Text
{
    [RequireComponent(typeof(UIDocument), typeof(EventSystem))]
    public class TextAssets : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;
        [SerializeField] private StyleSheet styleAsset = default;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            uiDocument.visualTreeAsset = sourceAsset;
        }

        void OnEnable()
        {
            InitializeVisualTree(GetComponent<UIDocument>());
        }

        private void InitializeVisualTree(UIDocument uiDocument)
        {
            VisualElement root = uiDocument.rootVisualElement;
            root.styleSheets.Add(styleAsset);

            root.Query(className: "disableRichText").ForEach(
                (element => (element as Label).enableRichText = false));
        }
    }
}
