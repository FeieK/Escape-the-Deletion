using UnityEngine;
[RequireComponent (typeof(Animator))]
public class ButtonAnimationController : MonoBehaviour
{
    public bool isSprint = false;
    private void Start()
    {
        Animator animator = GetComponent<Animator> ();
        animator.SetBool("IsSprint", isSprint);
    }
}
