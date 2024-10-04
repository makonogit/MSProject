using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

//---------------------------------
//担当：菅
//GameUI管理クラス
//---------------------------------

namespace Assets.C_Script.C_GameUI
{

    public class CS_GameUIClass
    {
        //コンストラクタ
        //CS_GameUIClass() {}

        //コンストラクタ(最大リソース値とゲージのサイズでゲージの割合を設定)
        public CS_GameUIClass(float _Maxresource, RectTransform _gageTrans)
        {
            GageRate = _gageTrans.sizeDelta.x / _Maxresource;
            MaxResource = _Maxresource;
        }

        /// <summary>
        /// リザルトUIの表示
        /// </summary>
        /// <param クリアUIかゲームオーバーUI="_GameResultUI"></param>
        public void ViewResultUI(GameObject _GameResultUI)
        {
            //ゲーム内の時間を止める
            Time.timeScale = 0f;
            _GameResultUI.SetActive(true);

        }

        /// <summary>
        /// リソース変化
        /// </summary>
        /// <param プレイヤーの現在リソース="_Resource"></param>
        /// <param 変化量="_ChangeAmount"></param>
        ///  /// <param ゲージのサイズ="_gageTrans"></param>
        public void ResourceChange(ref float _Resource, float _ChangeAmount, RectTransform _gageTrans)
        {
            //リソースの上下限
            bool resourcelimit = (_Resource <= 0 && _ChangeAmount < 0) || (_Resource >= MaxResource && _ChangeAmount > 0);
            if(resourcelimit) { return; }

            _Resource += _ChangeAmount;

            //変化量からゲージの幅を設定
            Vector2 NowSizeDelta = _gageTrans.sizeDelta;
            NowSizeDelta.x += _ChangeAmount * GageRate;
            _gageTrans.sizeDelta = NowSizeDelta;

        }

        //ゲージの増減幅
        private float GageRate = 1.0f;

        //リソース最大値
        private float MaxResource = 1.0f;
    }
}
