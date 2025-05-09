using UnityEngine;


public class StickCanvasToWall : MonoBehaviour
{
    public Transform wall;  // 拖你的墙体进来
    public float offsetDistance = 0.1f; // 距离墙0.1米浮出
   // 可调节的Y轴偏移量，确保Canvas在墙上合适高度
    public float yOffset = 2.0f;

    void Start()
    {
        if (wall != null)
        {
            // 获取墙体的世界位置
            Vector3 wallPosition = wall.position;

            // 调整y坐标，避免Canvas卡在地板上
           // 确保yOffset可以真正影响位置
            wallPosition.y += yOffset;
            // 位置：墙的位置+墙前方方向偏移
            transform.position = wallPosition + wall.forward * offsetDistance;

            // 旋转：对齐墙体
            transform.rotation = wall.rotation;

            // 缩放：适配到小的世界空间尺寸
            transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);  // 可以根据需求调整
        }
        else
        {
            Debug.LogWarning("请把墙体拖进 StickCanvasToWall 脚本的 Wall属性！");
        }
    }
}
