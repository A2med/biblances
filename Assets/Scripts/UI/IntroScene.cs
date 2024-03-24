using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public float WaitTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delay());
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(WaitTime);
        SceneManager.LoadScene(1);
    }
}
