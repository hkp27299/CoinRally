using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CarModelSelection : MonoBehaviour
{
    public GameObject[] CarModels;
    public GameObject CarCanvas;
    int childCount;
    int carModelCount = 0;

    public static int selectedCarModelCount=0;

    // Start is called before the first frame update
    void Start()
    {
        childCount = CarModels.Length;
        selectedCarModelCount = 0;
        CarSelect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CarSelect()
    {
        
        foreach (GameObject obj in CarModels)
            obj.SetActive(false);
        CarModels[0].SetActive(true);
        CarCanvas.transform.GetChild(1).gameObject.SetActive(false);
        CarCanvas.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void Next()
    {
        CarModels[carModelCount].SetActive(false);
        carModelCount = (carModelCount + 1) % childCount;
        CarModels[carModelCount].SetActive(true);
        selectedCarModelCount = carModelCount;

        if(carModelCount == childCount - 1)
        {
            CarCanvas.transform.GetChild(1).gameObject.SetActive(true);
            CarCanvas.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            CarCanvas.transform.GetChild(1).gameObject.SetActive(true);
            CarCanvas.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void Previous()
    {
        CarModels[carModelCount].SetActive(false);
        carModelCount--;
        if(carModelCount<0){
            carModelCount=7;
        }
        CarModels[carModelCount].SetActive(true);

        if (carModelCount <= 0)
        {
            CarCanvas.transform.GetChild(1).gameObject.SetActive(false);
            CarCanvas.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            CarCanvas.transform.GetChild(1).gameObject.SetActive(true);
            CarCanvas.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void FinalSelectedModel()
    {
        selectedCarModelCount = carModelCount;
        CarModels[carModelCount].SetActive(false);
        ChangeScene("StartingPoint");
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
