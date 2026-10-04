using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Voidless.XRIT
{
public class XRITJoystickDebugger : MonoBehaviour
    {
    [SerializeField] private XRITJoystick _joystick;
    [SerializeField] private TextMeshProUGUI _textMesh;

    /// <summary>Gets joystick property.</summary>
    public XRITJoystick joystick { get { return _joystick; } }

    /// <summary>Gets textMesh property.</summary>
    public TextMeshProUGUI textMesh { get { return _textMesh; } }

    /// <summary>Updates XRITJoystickDebugger's instance at each frame.</summary>
    private void Update()
    {
        if(joystick == null || textMesh == null) return;

        textMesh.text = joystick.ToString();
    }
}
}