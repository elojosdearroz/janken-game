using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClipboardManager : MonoBehaviour
{
    public TextMeshProUGUI idLoby;
    public void CopyClipboard()
    {
        GUIUtility.systemCopyBuffer = idLoby.text;
    }
}
