using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicatorBehaviour : MonoBehaviour {

    private MyContext Context = MyContext.Current;
    private Vector3 initialLocalScale;
    private Vector3 gameRunningLocalScale;
    private Vector3 gameOverLocalScale;

    void OnMouseDown()
    {
        if (Context.GameOver)
        {
            Context.ResetGame();
        }
    }

    // Use this for initialization
    void Start () {
        //MyContext.Current.ColorIndicator = this;
        initialLocalScale = transform.localScale;
        gameRunningLocalScale = initialLocalScale;
        gameOverLocalScale = new Vector3(initialLocalScale.x, initialLocalScale.y * 2.0F, initialLocalScale.z);

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 baseSize = Context.GameOver ? gameOverLocalScale : gameRunningLocalScale;

		if(Context.IsCurrentPlayerWhite)
        {
            GetComponent<Renderer>().material.color = new Color(1.0F, 1.0F, 1.0F); //C#
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(0.0F, 0.0F, 0.0F); //C#
        }

        transform.localScale = baseSize;
    }
}
