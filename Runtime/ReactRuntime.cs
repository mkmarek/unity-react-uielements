using ChakraHost.Hosting;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityReactUIElements;

namespace UnityReactUIElements
{
    public class ReactRuntime : MonoBehaviour
    {
        [SerializeField]
        private JSFileObject _root;

        [SerializeField]
        private PanelRenderer renderer;

        private JsModuleRuntime runtime;

        // Start is called before the first frame update
        void Start()
        {
            this.runtime = new JsModuleRuntime();

            runtime.RunModule(_root, renderer);
        }

        private void OnDestroy()
        {
            runtime.Dispose();
        }
    }
}