using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExemploAEstrelaValores : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        List<Vector2> vetores = new List<Vector2>();
        for(int i = 1; i <= 7; i++)
        {
            for (int j = 1; j <= 6; j++)
            {
                vetores.Add(new Vector2(i, j));
            }
        }
        Vector2 target = new Vector2(7, 1); ;
        foreach(Vector2 v in vetores)
        {
            print($"Distância de (7, 1) até ({v.x}, {v.y}) é: {Vector2.Distance(target, v)}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
