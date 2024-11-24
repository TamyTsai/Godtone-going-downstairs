using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    [SerializeField] float moveSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // 先於Unity創建空物件 來管理生成階梯
        // 於 Hierarchy 視窗中，將要被管理的物件（子物件），拉進空物件（父物件）中

        // 移動階梯
        transform.Translate(0, moveSpeed*Time.deltaTime, 0);

        // 判斷如果階梯移動到超出視窗，就刪除，並生成新階梯
        if(transform.position.y > 6f)
        {
            Destroy(gameObject); // 刪除階梯
            transform.parent.GetComponent<FloorManager>().SpawnFloor(); // 呼叫生成階梯方法
            // transform.parent：代表FloorManager物件（Floor的父物件）
            // GetComponent<FloorManager>()：取得FloorManager物件下的Component，如此才能使用裡面的公開方法
        }
    }

}
