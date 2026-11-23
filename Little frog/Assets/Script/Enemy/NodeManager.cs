using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{

    public List<Node> allNodes = new List<Node>();

    public Transform player;

    public LayerMask obstacleLayer;

    public float checkRadius = 5f;


    List<Node> GetNearbyNodes()
    {
        List<Node> nearby = new List<Node>();

        foreach(Node node in allNodes)
        {
            float distance = Vector2.Distance(node.Position, player.position);

            if(distance < checkRadius)
            {
                
                nearby.Add(node);
            }
        }

        return nearby;
    }

    bool HasLineOfSight(Node node)
    {
        Vector2 dir = (Vector2)player.position - node.Position;

        RaycastHit2D hit = Physics2D.Raycast(node.Position, dir.normalized, dir.magnitude, obstacleLayer);

        
        if (hit.collider == null)
        {
            Debug.Log("BUG");
            return true;// bug
        }

        if(hit.collider.CompareTag("Player"))
        {
            Debug.Log("NODE da thay player");
            return true;
        }

        return false;
    }


    public List<Node> GetVisibleNodes()
    {
        List<Node> result = new List<Node>();

        List<Node> nearby = GetNearbyNodes();

        foreach (Node node in nearby)
        {
            

            if (HasLineOfSight(node))
            {
                
                result.Add(node);
            }
        }
    

        return result;
    }


    private void OnDrawGizmos()
    {
        if (player == null || allNodes == null) return;

        Gizmos.color = Color.green;

        foreach (Node node in allNodes)
        {
            Vector2 dir = (Vector2)player.position - node.Position;

            RaycastHit2D hit = Physics2D.Raycast(
                node.Position,
                dir.normalized,
                dir.magnitude,
                obstacleLayer
            );
          
            
            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(node.Position, player.position, Color.green);
            }
            else
            {
                Debug.DrawLine(node.Position, player.position, Color.red);
            }
        }
    }

}
