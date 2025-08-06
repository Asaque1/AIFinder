using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField]
    Animator ani;

   
    public void OnClose()
    {
        ani.SetBool("isopen",false);
    }
    public void OnOpen()
    {
        ani.SetBool("isopen",true);
    }

}
