using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Danh sách node hàng xóm (kéo th? trong Inspector)
    public List<Node> neighbors = new List<Node>();

 

    // Tr? v? v? trí node (cho ti?n dùng)
    public Vector2 Position
    {
        get { return transform.position; }
    }

    // Debug v? ???ng n?i gi?a các node
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (Node neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}
