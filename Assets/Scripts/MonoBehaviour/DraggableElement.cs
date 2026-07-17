using UnityEngine;

// Attach this small script to ANY draggable object that needs to carry
// element identity information (works alongside the existing Drag & Drop script).
//
// This script does nothing on its own - it's just a data holder that the
// Name Matching Target script will read from when the object is dropped on it.
public class DraggableElement : MonoBehaviour
{
    [Tooltip("The Element ScriptableObject this draggable object represents.")]
    public ElementData assignedElement;
}