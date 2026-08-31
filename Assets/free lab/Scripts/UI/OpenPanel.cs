using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    public void setVisibleToTrue()
    {
        panel.SetActive(true);
    }
    
}
