using UnityEngine;

public class WallCollision : MonoBehaviour
{
    public Entity.WallSide wallSide;
    public float slideDelay;

    public Entity entity;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) entity.ToutchWall(slideDelay, wallSide, true);
        if (collision.CompareTag("Border")) entity.ToutchBorder(wallSide, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) entity.ToutchWall(0, wallSide, false);
        if (collision.CompareTag("Border")) entity.ToutchBorder(wallSide, false);
    }
}
