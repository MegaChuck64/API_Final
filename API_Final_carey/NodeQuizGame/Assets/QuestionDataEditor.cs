using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using SocketIO;

public class GameDataEditor : EditorWindow
{
    string gameDataFilePath = "/StreamingAssets/data.json";
    public QuestionData editorData;
    static private GameObject server;
    static SocketIOComponent socket;

    [MenuItem("Window/Game Data Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(GameDataEditor)).Show();
    }

    void OnGUI()
    {

            //display the data from json
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("editorData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

        //LoadGameData();


        if (GUILayout.Button("Load Question"))
        {
            LoadGameData();
        }


        if (GUILayout.Button("Add Question"))
        {
            SaveGameData();
            SendGameData();
        }

    }

    void LoadGameData()
    {
        string filePath = Application.dataPath + gameDataFilePath;

        if (File.Exists(filePath))
        {
            string gameData = File.ReadAllText(filePath);
            editorData = JsonUtility.FromJson<QuestionData>(gameData);
        }
        else
        {
            editorData = new QuestionData();
        }
    }

    void SaveGameData()
    {
        string jsonObj = JsonUtility.ToJson(editorData);

        string filePath = Application.dataPath + gameDataFilePath;
        File.WriteAllText(filePath, jsonObj);
    }

    void SendGameData()
    {
        string jsonObj = JsonUtility.ToJson(editorData);
        server = GameObject.Find("Server");
        socket = server.GetComponent<SocketIOComponent>();
        socket.Emit("send data", new JSONObject(jsonObj));
        socket.Emit("load data");

    }
}