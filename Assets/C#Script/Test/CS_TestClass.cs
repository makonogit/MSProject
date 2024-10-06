using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

// namespace について
// なくとも問題ないけど付けとくと、
// クラス名が被っても大丈夫になるので安心
namespace Assets.C_Script.Test
{

    // class に使うアクセス演算子について
    // 
    // public       どこでもアクセス可
    //  
    // internal     同一アセンブリであればアクセス可
    //              (ソリューション外の他プロジェクトは不可)

    public class CS_TestClass // ←MonoBehaviourを継承しない
    {
        private string text;
        
        // コストラクタ = インスタンスが生成された瞬間に実行
        public CS_TestClass() { }

        // 引き数がある場合
        public CS_TestClass(int val, string text)
        {
            this.HP = val;
            this.text = text;
        }

        // デストラクタ = クラスのインスタンスが破棄 (ガベージコレクションに回収)される時に実行
        ~CS_TestClass() { }
        
        // C++と同じく変数としてクラスを持っている時に呼び出せる
        public void Function() {}

        // C++の静的関数と同じくいつでもどこでも呼び出せる。
        public static void Function2() {}   

        // プロパティについて

        // get/setアクセサ
        private bool shotFlag;
        public bool IsShot 
        {
            set {
                shotFlag = value;
            }
            get {
                return false;
            }
        }

        // このように書くこともできる。
        public string Name { get; set; } = "中川";

        // privateを付けることで外からは読み込み専用,書き込み専用として使える
        public int HP { get; private set; } =　10;
        public int MP { private get; set; } =　10;
    }
}
