using UnityEngine;

public class gimmick : MonoBehaviour
{
    [SerializeField]
    int dustCount = 5;
    public void OnclearDust()
    {
        dustCount -= 1;
        if (dustCount == 0)
        {
            Destroy(gameObject);
        }
        
    }
}
