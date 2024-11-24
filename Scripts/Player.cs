using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 想要使用SampleScene物件（場景）的話，要引入此命名空間

public class Player : MonoBehaviour // C#檔案名稱要跟裡面的class名稱相同 // 將腳本檔案拉到要應用的物件的Inspector視窗中，就可以將腳本應用在該物件上
{
    // float moveSpeed = 5f;
    // 沒有宣告是什麼方法的話，預設是private方法，無法在Unity裡直接調整值

    // public float moveSpeed = 5f; // transform.Translate()參數要用double資料型態，如果是float，要加寫f
    // 宣告為public方法，就可以在Unity裡直接調整值
    // 在遊戲執行中也可以透過Unity改變數值，但變更不會被保存

    // 加上[SerializeField]可讓private方法也可以在Unity裡直接調整值
    [SerializeField] float moveSpeed = 5f;

    // 目前腳下的階梯
    GameObject currentFloor;

    // 血量
    [SerializeField] int Hp;

    // 遊戲物件 血量條 
    // Unity：將 HpBar物件 設定到 「此處創建的Player物件的Hp Bar」中
    [SerializeField] GameObject HpBar;

    // 分數文字
    [SerializeField] TMPro.TMP_Text scoreText;
    // Text物件，必須引入UnityEngine.UI才能用
    // 新版的Text物件叫TMPro.TMP_Text物件
    // 新版 Unity 改為使用 TextMesh Pro (TMP) 文字

    // 分數
    int score;

    // 距離上次加分 經過的時間（時間越長分數越高）
    float scoreTime;

    // 動畫
    Animator anim;

    // 物件渲染
    SpriteRenderer render;

    // 聲音
    AudioSource deathSound;

    // 重新開始按鈕
    [SerializeField] GameObject replayButton;

    // Start is called before the first frame update
    void Start() // 遊戲初始化，本方法只有在一開始會被執行一次
    {
        // Unity內建的方法
        // 移動物件
        // transform.Translate(0, 1, 0);

        Hp = 10;
        score = 0;
        scoreTime = 0f;
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        deathSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() // 進入遊戲迴圈後，就會被不斷重複執行，直到遊戲結束
    {
        // Time.deltaTime：每次Update方法被呼叫的間隔時間（跑一次GameLoop的時間）
        // transform.Translate(0, 0.1f*Time.deltaTime, 0);
        // 0.1f*1（電腦「一秒」跑一次迴圈）*1（電腦一秒跑「一次」迴圈）＝0.1（每秒動0.1單位）
        // 0.1f*0.5（電腦「0.5秒」跑一次迴圈）*2（電腦一秒跑「2次」迴圈）=0.1（每秒也是動0.1單位，達到控制在不同電腦上速度一致效果）
        // transform.Translate(0, moveSpeed*Time.deltaTime, 0);
        // (x軸, y軸, z軸)

        // 取得輸入、控制遊戲物件
        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) { // 如果玩家按下方向盤右鍵或D鍵
            transform.Translate(moveSpeed*Time.deltaTime, 0 ,0);
            render.flipX = false; // 將SpriteRenderer 的 flipX 功能取消
            anim.SetBool("run", true); // 透過將條件參數run設定為true，來讓觸發變換動畫狀態條件
        }
        else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) { // 如果玩家按下方向盤左鍵或A鍵
            transform.Translate(-moveSpeed*Time.deltaTime,0 ,0);
            render.flipX = true; // 將SpriteRenderer 的 flipX 功能勾選（圖片左右翻轉，人物朝左）
            anim.SetBool("run", true);
        }
        else // 若玩家沒有按下往左或往右
        {
            anim.SetBool("run", false);
        }

        UpdateScore();
        // 每跑一次遊戲迴圈，就把跑一次迴圈的時間加給scoreTime
        // 當檢查到scoreTime超過兩秒，分數就加一分，並把scoreTime歸零
        // 更新畫面分數顯示
    }

    // Unity 的事件方法（如 OnCollisionEnter2D 和 OnTriggerEnter2D）需要是類別的成員方法，而不能定義在其他方法內。

    // 剛體：像現實世界物體一樣，有質量，受摩擦力、阻力、地心引力影響
    // 程式判斷碰撞
    // Unity內建方法
    // Sent when an incoming collider makes contact with this object's collider (2D physics only).
    // <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        // 檢查用輸出
        // Debug.Log("撞到了啦");

        // 判斷撞到哪個物體
        // 先在Unity中幫物體加上tag
        if(other.gameObject.tag == "Normal") // 只要玩家物件碰到tag為一般階梯的物件，
        {
            // 輸出碰撞的座標位置
            // Debug.Log(other.contacts[0].point); // 玩家物件碰撞框左下點座標
            // Debug.Log(other.contacts[1].point); // 玩家物件碰撞框右下點座標

            // 輸出碰撞的法向量
            Debug.Log(other.contacts[0].normal); // 玩家物件碰撞框左下點的法向量
            Debug.Log(other.contacts[1].normal); // 玩家物件碰撞框右下點的法向量
            // 碰到階梯上緣時，法向量向上（0.00, 1.00）
            // 碰到階梯右側時，法向量向右（1.00, 0.00）

            // 只有在碰撞到階梯上緣時，才算撞到階梯
            if(other.contacts[0].normal == new Vector2(0f, 1f)) // 如果碰撞到的是階梯上緣
            // Vector2()：2D向量
            {
                Debug.Log("撞到一般階梯");
                currentFloor = other.gameObject; // 就將該階梯物件指定給currnetFloor變數
                ModifyHp(1);
                other.gameObject.GetComponent<AudioSource>().Play();
                // other是被碰撞到的物件 此處為一般階梯
                // 播放該物件 AudioSource component 中的音檔
            }
        }
        else if(other.gameObject.tag == "Nails")
        {
            // 只有在碰撞到階梯上緣時，才算撞到階梯
            if(other.contacts[0].normal == new Vector2(0f, 1f)) // 如果碰撞到的是階梯上緣
            // Vector2()：2D向量
            {
                Debug.Log("撞到尖刺階梯");
                currentFloor = other.gameObject; // 就將該階梯物件指定給currnetFloor變數
                ModifyHp(-3);
                anim.SetTrigger("hurt"); // 透過將條件參數hurt設定為被觸發，來讓觸發變換動畫狀態條件
                other.gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if(other.gameObject.tag == "Ceiling") // 只要玩家物件碰到tag為天花板的物件
        {
            Debug.Log("撞到天花板");
            currentFloor.GetComponent<BoxCollider2D>().enabled = false; // 就將目前腳底下階梯的 BoxCollider2D 勾勾拿掉
            // 碰撞功能也是一個component
            ModifyHp(-3);
            anim.SetTrigger("hurt");
            other.gameObject.GetComponent<AudioSource>().Play();
        }
    }

    // 在Unity中將物件的isTrigger勾起來（偵測用物件 不與他人碰撞）
    // 程式判斷穿過
    // Sent when another object enters a trigger collider attached to this object (2D physics only).
    // <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "DeathLine")
        {
            Debug.Log("你輸了");
            Die();
        }
    }

    // 匯入圖片
    // Unity中操作
        // 新增圖片到既有物件：將圖片拉到已建立物件的Sprite Renderer中
        // 直接用圖片創新物件：將圖片拉到 Hierarchy 或 Scene 中
        // 點擊圖片 Pixels Per Unit 指的是 每幾個像素為一個單位：所以如果數值砍半，圖片佔遊戲畫面空間就會變成兩倍大
        // 如果不希望物體旋轉，就調整 Rigidbody2D 的 Constraint 的 Freeze Rotation
        // 更改背景顏色：在Main Camera物件中修改顏色
        // Order in Layer 可調整物件圖層順序
    
    // 複製階梯（Prefab）（把物件做成樣版）
    // Unity中操作
        // 將物件往下拉到資料夾中，就會做出一個樣版（Prefab）
        // Hierachy 中藍色的物件，代表是根據某個樣版生成的
        // 將資料夾中樣版往上拉到 Hierarchy 或 scene 就可以生成物件
        // 當更動任一個以Perfab生成的物件時，如果想要套用到其他以同樣Perfab生成的物件，就點Overrides 選 Apply all

    // 扣血或回血 方法
    void ModifyHp(int num) // 參數為血量的變動量
    { 
        Hp += num;
        if(Hp > 10)
        {
            Hp = 10;
        }
        else if(Hp <= 0)
        {
            Hp = 0;
            Die();
        }

        UpdateHpBar(); // 更新血量顯示量
    }

    // 製作UI介面
    // Unity操作
        // 於 Hierarchy右鍵點擊UI，選擇要創建的UI類型
        // UI物件都要是Canvas的物件
        // Canvas下的物件都獨立於遊戲，物件顯示都可以覆蓋到畫面上
        // Canvas下的物件被點擊之類的，就會觸發 EventSystem 的事件

    // 更新血量條血量顯示多寡 方法
    void UpdateHpBar()
    {
        for(int i=0; i<HpBar.transform.childCount; i++)  
        // HpBar.transform.childCount：HpBar物件下的子物件數量
        {
            if(i<Hp) { // Hp剩1時，顯示第0個血量物件；Hp剩2時，顯示第0個與第1個血量物件
                HpBar.transform.GetChild(i).gameObject.SetActive(true);
                // HpBar.transform.GetChild(i)：HpBar物件下的第i個子物件
                // SetActive()：是否將物件顯示在畫面上
            }
            else
            {
                HpBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    // 更新分數
    void UpdateScore()
    {
        scoreTime += Time.deltaTime; 
        // Time.deltaTime：每次Update方法被呼叫的間隔時間
        if(scoreTime>2f) // 遊戲每經過超過2秒後
        {
            score++; // 分數+1
            scoreTime = 0f;
            scoreText.text = "Lower Ground" + score.ToString(); // 更新顯示之 分數文字 UI
        }
        
    }

    // 新增動畫
        // Unity操作
        // 在資料夾中新增Animator Controller
        // 在要新增動畫的物件上新增Component，新增Animator
        // 將 Animator Controller 拖曳到 Animator 的 Controller 中
        // 點擊上方window > Animation > Animation 開啟動畫視窗
        // 於 Hierarchy 點選要新增動畫的物件
        // 點擊 動畫視窗 的 Create
        // 新增動畫檔案
        // 將動畫圖片拉入影格
        // 點擊 動畫視窗 右邊三點點 Show Sample Rate 可以看到動畫幀數

        // 點擊上方window > Animation > Animator 可以知道物件動畫狀態
        // 按住option(alt)可以移動
        // Entry是遊戲一開始

        // 動畫室窗中 點擊 create clip可以新增新動畫

        // Animator視窗中 點擊 某個動畫狀態 右鍵 Make Transition 
        // 點擊 要轉換 的 動畫狀態 以連接兩個狀態
        // 點擊 Parameters 頁籤 新增參數
        // 點擊關聯線 右邊Inspector出現 conditions（狀態轉換條件）
        // conditions可以新增剛剛新增的參數
        // Animator視窗中 Parameters 參數 打勾（bool），表示遊戲初始設該參數為true

        // 解決動畫狀態轉換延遲
        // 點擊 Animator 視窗 動畫狀態間 的 關聯線
        // 打開Setting，關掉 Has Exit Time、Fixed Duration、Transiton Duratior設為0

        // 同時播放兩動畫
        // 點擊 Animator 視窗 Layers頁籤
        // 新增圖層
        // 點擊齒輪調權重 調成1 完全顯示
        // 兩個圖層的動畫就可以允許同時被播放

        // 錄製可以記錄物件的變化

    // 新增音效
        // Unity操作
        // 在物件上新增Component Audio Source
        // 在 AudioClip 選擇音效檔案
        // Play On Awake 預設為勾選 表示一開始先播放一次，不要的話就取消（想要採到階梯才播放）

    // 新增遊戲結束後之重新遊戲按鈕
        // Unity操作
        // 在 Hierarchy 視窗新增 UI物件：Button
        // 方法寫完後，到Inspector 之 on Click()裡新增方法（選擇物件 選取掛在該物件下的公開方法）
    
    void Die()
    {
        deathSound.Play();
        Time.timeScale = 0f;
        // Time.timeScale：Unity中時間的縮放比例，預設為1
        // 設為2 遊戲執行速度就會變成2倍
        // 0 就可以凍結遊戲（暫停）
        replayButton.SetActive(true); // 讓重新遊戲按鈕可見
        // 在Unity做的設定都是預設值，遊戲進行中時再利用程式碼控制狀態
    }

    public void Replay() // 本方法會被重新遊玩按鈕物件使用，所以要宣告為公開方法
    {
        Time.timeScale = 1f;
        // 出現重新遊玩按鈕的時候，時間是暫停的，所以點下重新開始按鈕，要讓遊戲繼續進行
        SceneManager.LoadScene("SampleScene"); // 重新載入 SampleScene 場景
    }
}
