using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject[] floorPrefabs; // 創建空陣列 用來存放不同類型地板樣版（Prefabs）
    // 陣列中要裝的元素的資料型態是 GameObject（遊戲物件）
    // floorPrefabs是陣列名稱

    // 生成階梯的方法
    public void SpawnFloor() // 要在Floor模組中做呼叫，所以要設定為public方法
    {
        // 生成的階梯「類型」隨機
        // Random.Range(0, 2) // 隨機回傳0～2的值（不包含2）
        int r = Random.Range(0, floorPrefabs.Length);
        GameObject floor = Instantiate(floorPrefabs[r], transform);
        // 隨機生成一般階梯或尖刺階梯
        // transform：將生成的物件放到此物件底下（子物件）
        // 將隨機生成的子物件 存入 限定變數中資料型態為GameObject的變數中
        // 生成物件函式：Instantiate(要生成的遊戲物件, transform)

        // 生成的階梯「位置」隨機
        floor.transform.position = new Vector3(Random.Range(-3.7f, 3.7f), -6f, 0f); // 讓階梯物件生成的位置隨機
        // floor.transform.position：遊戲物件floor的位置 
    }
}
