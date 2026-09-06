using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Open()
    {
        panel.SetActive(true);
    }
}