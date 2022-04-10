using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public float score = 0;

    public float roadSpeed;

    public float minimumObjectDistance = 1;

    [Range(-1, -0.005f)]
    public float carTurningSpeed = -0.15f;

    public CarMovement car;
    public RoadBehavior roadBehavior;
    public GameObject[] carModels;
    public GameObstacle gameObstaclePrefab;
    public GamePoint gamePointPrefab;

    public List<GameObject> activeObjects = new List<GameObject>();

    public ScoreScreen scoreScreen;

    public InGameScore inGameScoreScreen;

    public TextMeshProUGUI nameText, scoreText ,inGameScore;

    public Transform leaderboardContent;

    private GameObject leaderBoardBox;

    public Canvas mainCanvas, leaderboardCanvas;

    public void StartGame()
    {
        //car.StartEngine();


        if (!PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore",0);            
        }
        foreach (GameObject obj in carModels){
            obj.SetActive(false);
        }
        new WaitForSeconds(1f);
        carModels[CarModelSelection.selectedCarModelCount].SetActive(true);
        car = carModels[CarModelSelection.selectedCarModelCount].GetComponent<CarMovement>();
        FindObjectOfType<SoundManager>().attach(carModels[CarModelSelection.selectedCarModelCount]);
        inGameScoreScreen.gameObject.SetActive(true);
        StartCoroutine(CarVoice());
        roadBehavior.roadSpeed = roadSpeed;
        car.roadXScale = roadBehavior.transform.localScale.z - 4f;
        car.turnSpeed = carTurningSpeed;
        StartCoroutine(SpawnObjectsRoutine());
        scoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";
        nameText.text = FirebaseManager.CurrentUser.DisplayName;
        inGameScoreScreen.UpdateScoreTXT(0);
    }

    public void StopGame()
    {
        Application.Quit();
    }
    public async void OpenLeaderBoards()
    {
        mainCanvas.gameObject.SetActive(false);
        leaderboardCanvas.gameObject.SetActive(true);
        
        leaderBoardBox = leaderboardContent.GetChild(1).gameObject;
        leaderboardContent.GetChild(1).gameObject.SetActive(false);

        var allData = await FirebaseManager.Instance.GetAllUserLeaderBoards();
        if (allData == null)
        {
            Debug.Log("all data is null");
            return;
        }
        Debug.Log("All Data" + JsonUtility.ToJson(allData));


        foreach (var data in allData)
        {
            GameObject box = Instantiate(leaderBoardBox, leaderboardContent);
            box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = data.Key;
            box.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = data.Value.ToString();
            box.SetActive(true);
        }

        for (int i = 2; i < leaderboardContent.childCount; i++)
        {
            leaderboardContent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = (i - 1).ToString();
        }
    }

    public void CloseLeaderboards()
    {
        mainCanvas.gameObject.SetActive(true);
        leaderboardCanvas.gameObject.SetActive(false);
        
        for (int i = 2; i < leaderboardContent.childCount; i++)
        {
            Destroy(leaderboardContent.GetChild(i).gameObject);
        }
    }
    
    public void GameOver()
    {
        roadBehavior.roadSpeed = 0;
        roadBehavior.sideSpeed = 0;
        car.turnSpeed=0;
        inGameScoreScreen.gameObject.SetActive(false);
        StopAllCoroutines();
        FindObjectOfType<SoundManager>().Stop("CarVoice");

        foreach (GameObject active in activeObjects)
        {
            Destroy(active);
        }

        scoreScreen.gameObject.SetActive(true);
        scoreScreen.UpdateScoreTXT(score);
    }

    public void SpawnObjects(GameObject pointPrefab, GameObject obstaclePrefab)
    {
        Vector3[] positions = new Vector3[2];

        positions = GetRandomPositions();

        if (positions[0].y > 0 && positions[1].y > 0)
        {
            pointPrefab.transform.position = positions[0];
            obstaclePrefab.transform.position = positions[1];

            GameObject point = Instantiate(pointPrefab);
            GameObject obstacle = Instantiate(obstaclePrefab);

            activeObjects.Add(point);
            activeObjects.Add(obstacle);
        }
    }

    public void AddPoint()
    {
        FindObjectOfType<SoundManager>().Play("Coin");
        
        score += 1;
        inGameScoreScreen.UpdateScoreTXT(score);
        if(score%10==0){
            roadBehavior.roadSpeed +=1.02f*(roadBehavior.roadSpeed);
            roadBehavior.sideSpeed +=1.02f*(roadBehavior.sideSpeed);
            foreach(GameObject active in activeObjects){
                if(active.GetComponent<GameObstacle>()!=null){
                    active.GetComponent<GameObstacle>().updateSpeed();
                }
                else if(active.GetComponent<GamePoint>()!=null){
                    active.GetComponent<GamePoint>().updateSpeed();
                }
            }
        }
        
    }


    public Vector3[] GetRandomPositions()
    {
        float randomX0 = Random.Range(-2.5f, 2.5f);
        float randomX1 = Random.Range(-2.5f, 2.5f);

        if (Mathf.Abs(randomX0 - randomX1) > minimumObjectDistance)
        {
            return new Vector3[] { new Vector3(randomX0, 0.5f, 100), new Vector3(randomX1, 0.1f, 100) };
        }
        else
        {
            return new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
        }
    }

    public IEnumerator SpawnObjectsRoutine()
    {
        while (true)
        {
            SpawnObjects(gamePointPrefab.gameObject, gameObstaclePrefab.gameObject);
            yield return new WaitForSeconds(0.8f);
        }
    }

    public IEnumerator CarVoice()
    {
        while (true)
        {
            FindObjectOfType<SoundManager>().Play("CarVoice");
            yield return new WaitForSeconds(6);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log("SceneChanged");
        StartCoroutine(LoadYourAsyncScene(sceneName));
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("SceneChanged");
    }
}
