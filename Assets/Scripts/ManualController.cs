using FishNet.Object;
using TMPro;
using UnityEngine;
public class ManualController : NetworkBehaviour
{
    [Header("Manual Setup")]
    [Tooltip("Drag the 3D book model here. It must be a child of the personal camera.")]
    public GameObject manualObject;
    private TextMeshProUGUI helpText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameObject objetoTexto = GameObject.Find("Manual");
        helpText = objetoTexto.GetComponent<TextMeshProUGUI>();
        
        if (manualObject != null)
        {
            manualObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!base.IsOwner) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (manualObject != null && !manualObject.activeSelf)
            {
                helpText.text = "[Q] Quit";
                manualObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (manualObject != null && manualObject.activeSelf)
            {
                helpText.text = "[R] Manual";
                manualObject.SetActive(false);
            }
        }
    }
}