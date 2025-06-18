using UnityEngine;

public class PlayerQuestHandler : MonoBehaviour
{
    public int mintCount = 0;

    public bool bookDestroyed = false;

    //Luna
    public int flowerCount = 0; 
    public bool wreathCompleted = false;

    //Lendo
    public int sweepCount = 0;
    public bool yardCleaned = false;

    //Pip
    public int chickenCount = 0;
    public bool chickensCaught = false;

    //Harn
    public int woodCount = 0;
    public bool woodCollected = false;

    //Neko
    public int catTrackCount = 0;
    public bool catFound = false;

    //Fina
    public bool flourCollected = false;

    //Oren
    public int boxSortedCount = 0;
    public bool boxesSorted = false;

    //Serel
    public bool stickFound = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mintCount++;
            Debug.Log($"Đã thu thập {mintCount} nhánh bạc hà.");
            if (mintCount >= 4)
            {
                QuestManager.Instance.CompleteQuest("Hái 4 nhánh bạc hà quanh giếng làng để giúp Lys pha thuốc hồi phục cho trẻ em.");
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            bookDestroyed = true;
            Debug.Log("Đã phá hủy cuốn sách.");
            QuestManager.Instance.CompleteQuest("Hủy diệt cuốn sách trong Hầm mộ Rỗng để giúp Orlin tổ chức lễ truyền rèn.");
        }

        if (Input.GetKeyDown(KeyCode.F)) // Giả định nhấn phím F để thu thập hoa
        {
            flowerCount++;
            Debug.Log($"Đã thu thập {flowerCount} loại hoa.");
            if (flowerCount == 3 && !wreathCompleted)
            {
                wreathCompleted = true;
                QuestManager.Instance.CompleteQuest("Tìm 3 loại hoa quanh làng để Yuna kết vòng hoa.");
            }
        }

        if (Input.GetKeyDown(KeyCode.L)) // Giả định phím L để test quét sân
        {
            sweepCount++;
            Debug.Log($"Đã quét sạch {sweepCount} góc sân.");

            if (sweepCount == 3 && !yardCleaned)
            {
                yardCleaned = true;
                QuestManager.Instance.CompleteQuest("Cảm ơn! Giờ thì sân làng sạch sẽ rồi.");
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) // Giả định nhấn phím C để bắt gà (test nhanh)
        {
            chickenCount++;
            Debug.Log($"Đã bắt được {chickenCount} con gà.");

            if (chickenCount == 3 && !chickensCaught)
            {
                chickensCaught = true;
                QuestManager.Instance.CompleteQuest("Bắt giúp em 3 con gà quanh sân.");
            }
        }

        if (Input.GetKeyDown(KeyCode.G)) // Giả định phím G để nhặt gỗ
        {
            woodCount++;
            Debug.Log($"Đã nhặt {woodCount} mảnh gỗ.");

            if (woodCount == 5 && !woodCollected)
            {
                woodCollected = true;
                QuestManager.Instance.CompleteQuest("Giúp tôi nhặt 5 mảnh gỗ quanh xưởng để sửa lại ghế.");
            }
        }

        if (Input.GetKeyDown(KeyCode.K)) // Giả định phím K để thu thập dấu chân mèo
        {
            catTrackCount++;
            Debug.Log($"Đã tìm thấy {catTrackCount} dấu chân mèo.");

            if (catTrackCount == 3 && !catFound)
            {
                catFound = true;
                QuestManager.Instance.CompleteQuest("Giúp tôi tìm dấu chân mèo quanh làng nhé.");
            }
        }

        if (Input.GetKeyDown(KeyCode.F1)) // Phím giả định để test lấy bao bột
        {
            if (!flourCollected)
            {
                flourCollected = true;
                Debug.Log("Đã lấy bao bột cho Fina.");
                QuestManager.Instance.CompleteQuest("Cháu có thể lấy giúp cô bao bột trong kho không?");
            }
        }

        if (Input.GetKeyDown(KeyCode.O)) // Phím O để test sắp xếp thùng
        {
            boxSortedCount++;
            Debug.Log($"Đã sắp xếp {boxSortedCount} thùng hàng.");

            if (boxSortedCount == 4 && !boxesSorted)
            {
                boxesSorted = true;
                QuestManager.Instance.CompleteQuest("Mấy thùng hàng bị xáo trộn rồi, xếp lại giúp tôi nhé!");
            }
        }

        if (Input.GetKeyDown(KeyCode.I)) // Phím I để test nhặt gậy
        {
            if (!stickFound)
            {
                stickFound = true;
                Debug.Log("Đã tìm thấy cây gậy cho bà Serel.");
                QuestManager.Instance.CompleteQuest("Giúp bà tìm cây gậy gỗ để chống khi đi lại.");
            }
        }
    }

    public void CollectMint()
    {
        mintCount++;
        Debug.Log($"Đã thu thập {mintCount} nhánh bạc hà.");
        if (mintCount >= 4)
        {
            QuestManager.Instance.CompleteQuest("Hái 4 nhánh bạc hà quanh giếng làng để giúp Lys pha thuốc hồi phục cho trẻ em.");
        }
    }

    public void DestroyBook()
    {
        bookDestroyed = true;
        Debug.Log("Đã phá hủy cuốn sách.");
        QuestManager.Instance.CompleteQuest("Hủy diệt cuốn sách trong Hầm mộ Rỗng để giúp Orlin tổ chức lễ truyền rèn.");
    }

    public void CollectFlower()
    {
        flowerCount++;
        Debug.Log($"Đã thu thập {flowerCount} loại hoa.");
        if (flowerCount == 3 && !wreathCompleted)
        {
            wreathCompleted = true;
            QuestManager.Instance.CompleteQuest("Tìm 3 loại hoa quanh làng để Yuna kết vòng hoa.");
        }
    }

    public void SweepYard()
    {
        sweepCount++;
        Debug.Log($"Đã quét sạch {sweepCount} góc sân.");

        if (sweepCount == 3 && !yardCleaned)
        {
            yardCleaned = true;
            QuestManager.Instance.CompleteQuest("Cảm ơn! Giờ thì sân làng sạch sẽ rồi.");
        }
    }

    public void CatchChicken()
    {
        chickenCount++;
        Debug.Log($"Đã bắt được {chickenCount} con gà.");

        if (chickenCount == 3 && !chickensCaught)
        {
            chickensCaught = true;
            QuestManager.Instance.CompleteQuest("Bắt giúp em 3 con gà quanh sân.");
        }
    }

    public void CollectWood()
    {
        woodCount++;
        Debug.Log($"Đã nhặt {woodCount} mảnh gỗ.");

        if (woodCount == 5 && !woodCollected)
        {
            woodCollected = true;
            QuestManager.Instance.CompleteQuest("Giúp tôi nhặt 5 mảnh gỗ quanh xưởng để sửa lại ghế.");
        }
    }

    public void TrackCat()
    {
        catTrackCount++;
        Debug.Log($"Đã tìm thấy {catTrackCount} dấu chân mèo.");

        if (catTrackCount == 3 && !catFound)
        {
            catFound = true;
            QuestManager.Instance.CompleteQuest("Giúp tôi tìm dấu chân mèo quanh làng nhé.");
        }
    }

    public void CollectFlour()
    {
        if (!flourCollected)
        {
            flourCollected = true;
            Debug.Log("Đã lấy bao bột cho Fina.");
            QuestManager.Instance.CompleteQuest("Cháu có thể lấy giúp cô bao bột trong kho không?");
        }
    }

    public void SortBox()
    {
        boxSortedCount++;
        Debug.Log($"Đã sắp xếp {boxSortedCount} thùng hàng.");

        if (boxSortedCount == 4 && !boxesSorted)
        {
            boxesSorted = true;
            QuestManager.Instance.CompleteQuest("Mấy thùng hàng bị xáo trộn rồi, xếp lại giúp tôi nhé!");
        }
    }

    public void FindStick()
    {
        if (!stickFound)
        {
            stickFound = true;
            Debug.Log("Đã tìm thấy cây gậy cho bà Serel.");
            QuestManager.Instance.CompleteQuest("Giúp bà tìm cây gậy gỗ để chống khi đi lại.");
        }
    }
}