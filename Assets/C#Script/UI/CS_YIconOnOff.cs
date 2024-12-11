//-------------------------------
// クラス名 :CS_BillBoardArea
// 内容     :ビルボード
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.C_Script.UI
{
    public class CS_YIconOnOff:MonoBehaviour
    {
        [SerializeField]    
        private SpriteRenderer sprite;
        [SerializeField]
        private float radius = 1f;    
        [SerializeField]
        private Transform player;

        private void Start()
        {
            if (!TryGetComponent(out sprite)) Debug.LogError("null spriteRenderer");
            if (sprite != null) sprite.enabled = false;
            if (player == null) Debug.LogError("変数にプレイヤーを設定してください。");
        }

        private void Update()
        {
            sprite.enabled = isNear();
        }

        private bool isNear()
        {
            if (player == null) return false;
            float sqrRadius = radius * radius;
            Vector3 vector = player.transform.position - transform.position;
            if (vector.sqrMagnitude > sqrRadius) return false;
            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position,radius);
        }

#endif // UNITY_EDITOR
    }
}
