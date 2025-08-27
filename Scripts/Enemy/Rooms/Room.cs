using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> enemiesInRoom;
    public RoomState roomState;

    public List<GameObject> neighbourRooms;
    public List<Room> neighbourRoomsScripts;
    public List<Transform> neighbourRoomsPoints;

    public bool debugDrawEnabled = true; // Toggle for debug drawing

    private Collider2D roomCollider;

    private void Start()
    {
        roomCollider = GetComponent<Collider2D>();
        enemiesInRoom = new List<GameObject>();

        roomState = RoomState.Empty;

        InitNeighbours();
    }

    private void Update()
    {
        if (enemiesInRoom.Count > 0 && roomState != RoomState.Suspicious && roomState != RoomState.ToSearch)
        {
            roomState = RoomState.Secured;
        }
        else if (roomState == RoomState.Suspicious)
        {
            roomState = RoomState.Suspicious;
        }
        else if (roomState == RoomState.ToSearch) {
            roomState = RoomState.ToSearch;
        }
        else
        {
            roomState = RoomState.Empty;
        }
    }

    private void InitNeighbours()
    {
        neighbourRoomsScripts = new List<Room>();
        neighbourRoomsPoints = new List<Transform>();

        foreach (GameObject roomObject in neighbourRooms) {
            neighbourRoomsScripts.Add(roomObject.GetComponent<Room>());
            foreach (Transform t in roomObject.transform)
            {
                neighbourRoomsPoints.Add(t);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().currRoom = gameObject.GetComponent<Room>();
        enemiesInRoom.Add(collision.gameObject);

        if (roomState == RoomState.Suspicious) {
            BeginSearch(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        enemiesInRoom.Remove(collision.gameObject);
    }


    private void BeginSearch(GameObject enemyObject)  
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        foreach (Room room in neighbourRoomsScripts) {
            room.roomState = RoomState.ToSearch;
        }

        foreach (Transform point in neighbourRoomsPoints) {
            enemy.tempPatrolPoints.Add(point);
        }

        ShuffleList(enemy.tempPatrolPoints);

        roomState = RoomState.ToSearch;
        foreach (Transform t in gameObject.transform)
        {
            enemy.tempPatrolPoints.Add(t);
        }

        enemy.EnterSearchMode();
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void AlertRoom()
    {
        foreach (GameObject enemy in enemiesInRoom)
        {
            enemy.GetComponent<Enemy>().GetAlerted();
        }

        roomState = RoomState.Suspicious;
    }

   

    #region Debug
    private void OnDrawGizmos()
    {
        if (debugDrawEnabled)
        {
            if (roomCollider != null)
            {
                if (roomState == RoomState.Suspicious)
                {
                    Gizmos.color = Color.red;
                }
                else if (roomState == RoomState.Secured)
                {
                    Gizmos.color = Color.green;
                }
                else if (roomState == RoomState.ToSearch)
                {
                    Gizmos.color = Color.yellow;
                }
                else if (roomState == RoomState.Empty) 
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawWireCube(roomCollider.bounds.center, roomCollider.bounds.size);
            }
        }
    }
    #endregion
}
