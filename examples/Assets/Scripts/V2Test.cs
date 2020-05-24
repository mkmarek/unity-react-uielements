using Unity.UIElements.Runtime;
using UnityEngine;
using UnityReactUIElements;

public class V2Test : PanelRenderer
{
    [SerializeField]
    private JSFileObject _root;

    private JsModuleRuntime runtime;

    // Start is called before the first frame update
    void Start()
    {
        this.runtime = new JsModuleRuntime();

        runtime.RunModule(_root, this);
    }

    private void OnDestroy()
    {
        runtime.Dispose();
    }
}
