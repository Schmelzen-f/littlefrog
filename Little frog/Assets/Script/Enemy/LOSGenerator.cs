using System.Collections.Generic;
using UnityEngine;

public class LOSGenerator : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleLayer;

    public float radius = 3f;     // khoảng cách tạo vòng tròn quanh player
    public int pointCount = 16;   // số điểm

    public List<Vector2> GeneratePoints()
    {
        List<Vector2> validPoints = new List<Vector2>();

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2 / pointCount;

            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 point = (Vector2)player.position + dir * radius;

            // Check line of sight từ point → player
            Vector2 directionToPlayer = (Vector2)player.position - point;

            RaycastHit2D hit = Physics2D.Raycast(point, directionToPlayer.normalized, directionToPlayer.magnitude, obstacleLayer);

            if (hit.collider == null)
            {
                // Không bị cản → point này nhìn thấy player
                validPoints.Add(point);
            }

         


        }

        return validPoints;
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2 / pointCount;

            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 point = (Vector2)player.position + dir * radius;

            Gizmos.DrawSphere(point, 0.1f);
        }
    }


}
