using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

public class Network : MonoBehaviour
{
    static SocketIOComponent socket;

    public DataController dataController;

    public List<QuestionData> questionList = new List<QuestionData>();

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);


        socket = GetComponent<SocketIOComponent>();
        socket.On("connected", OnConnect);
        socket.On("addQuestion", AddQuestion);
        socket.On("initQuestions", InitQuestions);



    }

    // Tells us we are connected
    void OnConnect(SocketIOEvent e)
    {
        Debug.Log("We are Connected - " + e.data);
        socket.Emit("connected");
    }


    void AddQuestion(SocketIOEvent e)
    {
        questionList.Add(JsonUtility.FromJson<QuestionData>(e.data.ToString()));
        Debug.Log("Added question... " + e.data.ToString());
    }

    void InitQuestions(SocketIOEvent e)
    {
        dataController.GetCurrentRoundData().questions = questionList.ToArray();

        SceneManager.LoadScene ("MenuScreen");

    }

    void OnDisconnect(SocketIOEvent e)
    {
        Debug.Log("Disconnected: " + e.data);

        //var id = e.data["id"].ToString();

    }
}