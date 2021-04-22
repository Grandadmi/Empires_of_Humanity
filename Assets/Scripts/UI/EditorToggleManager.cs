using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToggleManager : MonoBehaviour
{
    public int arrayPos;

    public WorldEditor editorObject;

    public void SelectResource()
    {
        editorObject.SelectResource(arrayPos);
    }

}
