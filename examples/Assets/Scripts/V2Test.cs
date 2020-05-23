using UnityEngine;
using UnityReactUIElements;

public class V2Test : MonoBehaviour
{
    [SerializeField]
    private JSFileObject _root;

    private JsModuleRuntime runtime;

    // Start is called before the first frame update
    void Start()
    {
        this.runtime = new JsModuleRuntime();

        runtime.RunModule(_root);
    }

    private void OnDestroy()
    {
        runtime.Dispose();
    }
}
