using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Resart : MonoBehaviour
{
    public Scene scene;
    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    public void Ontrig(string s)
    {
        SceneManager.LoadScene(scene.name);
    }
}
