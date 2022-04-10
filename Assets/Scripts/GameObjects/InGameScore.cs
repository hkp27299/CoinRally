using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameScore : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    public void UpdateScoreTXT(float score)
    {
        textMesh.text = "Coins : "+score.ToString();
    }

}