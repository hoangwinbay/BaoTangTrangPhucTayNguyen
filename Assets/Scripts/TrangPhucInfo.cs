using UnityEngine;

public class TrangPhucInfo : MonoBehaviour
{
    [TextArea(4, 12)]
    public string moTa;

    [Tooltip("Bảng mô tả 3D gắn tại object này")]
    public GameObject bangMoTa;
}
