using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    private Transform player;
    //public float speed = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("không tìm thấy player");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (player != null && Input.GetKey(KeyCode.E))
        {
            /*transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );*/
        }

    }
    


  


}
