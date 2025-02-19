using CKFoodMaker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKFoodMaker.Resource
{
    public class PetResource
    {
        //todo KeyをIdとBattleTypeの組み合わせにする
        public static IReadOnlyDictionary<int, string> Talents { get; } = new Dictionary<int, string>
        {
            {0, "近接攻撃スピード"},
            {1, "遠距離攻撃スピード"},
            {2, "クリティカルヒット確率"},
            {3, "クリティカルヒットダメージ"},
            {4, "物理近接ダメージ"},
            {5, "物理遠距離ダメージ"},
            {6, "ボスへの与ダメージ"},
            {7, "ヒット時にダメージが3倍になる確率"},
            {8, "ヒット時に燃焼ダメージ"},
            {9, "ヒットで敵の回復力を75%減少させる毒を与える確率"},
            {10, "近接ヒットで相手を気絶させる確率"},
            {11, "相手の移動スピードを4秒間8.0%低下させる"},
            {12, "光度"},
            {13, "青色の光度"},
            {14, "相手の気絶時間が増加"},
            {15, "気絶している相手へのダメージ"},
            {16, "移動スピード"},
            {17, "ヒット時に敵がすべりやすくなる確率"},
            {18, "相手の残り体力に応じてダメージが最大"},
            {19, "ヒット時に相手の燃焼効果を消費し即座に合計燃焼ダメージを与える確率"},
            {20, "1体の敵を発射物が貫通"},
            {21, "移動不可および気絶時間減少"},
            {22, "ヒット時にマナを得られる確率"},
        };

        public static IReadOnlyDictionary<(PetId petType, PetColor color), string> ColorDict
            = new Dictionary<(PetId petType, PetColor color), string>
        {
            { (PetId.PetDog,PetColor.Color_1) ,"茶" },
            { (PetId.PetDog,PetColor.Color_2) ,"白" },
            { (PetId.PetDog,PetColor.Color_3) ,"暗灰" },
            { (PetId.PetDog,PetColor.Color_4) ,"黄" },
            { (PetId.PetDog,PetColor.Color_5) ,"橙" },
            { (PetId.PetDog,PetColor.Color_6) ,"灰" },
            { (PetId.PetDog,PetColor.Color_7) ,"黒" },
            { (PetId.PetDog,PetColor.Color_8) ,"桃" },
            { (PetId.PetCat,PetColor.Color_1) ,"白" },
            { (PetId.PetCat,PetColor.Color_2) ,"黄" },
            { (PetId.PetCat,PetColor.Color_3) ,"桃" },
            { (PetId.PetCat,PetColor.Color_4) ,"橙" },
            { (PetId.PetCat,PetColor.Color_5) ,"紫" },
            { (PetId.PetCat,PetColor.Color_6) ,"灰" },
            { (PetId.PetCat,PetColor.Color_7) ,"青" },
            { (PetId.PetCat,PetColor.Color_8) ,"茶" },
            { (PetId.PetBird,PetColor.Color_1) ,"緑" },
            { (PetId.PetBird,PetColor.Color_2) ,"橙" },
            { (PetId.PetBird,PetColor.Color_3) ,"白" },
            { (PetId.PetBird,PetColor.Color_4) ,"紫" },
            { (PetId.PetBird,PetColor.Color_5) ,"青" },
            { (PetId.PetBird,PetColor.Color_6) ,"桃" },
            { (PetId.PetBird,PetColor.Color_7) ,"灰" },
            { (PetId.PetBird,PetColor.Color_8) ,"黄" },
            { (PetId.PetBunny,PetColor.Color_1) ,"白" },
            { (PetId.PetBunny,PetColor.Color_2) ,"卵" },
            { (PetId.PetBunny,PetColor.Color_3) ,"黄" },
            { (PetId.PetBunny,PetColor.Color_4) ,"灰" },
            { (PetId.PetBunny,PetColor.Color_5) ,"桃" },
            { (PetId.PetBunny,PetColor.Color_6) ,"黒" },
            { (PetId.PetBunny,PetColor.Color_7) ,"茶" },
            { (PetId.PetBunny,PetColor.Color_8) ,"橙" },
            { (PetId.PetMoth,PetColor.Color_1) ,"白" },
            { (PetId.PetMoth,PetColor.Color_2) ,"灰" },
            { (PetId.PetMoth,PetColor.Color_3) ,"茶" },
            { (PetId.PetMoth,PetColor.Color_4) ,"黄" },
            { (PetId.PetMoth,PetColor.Color_5) ,"青" },
            { (PetId.PetMoth,PetColor.Color_6) ,"緑" },
            { (PetId.PetMoth,PetColor.Color_7) ,"桃" },
            { (PetId.PetMoth,PetColor.Color_8) ,"卵" },
            { (PetId.PetTardigrade,PetColor.Color_1) ,"緑" },
            { (PetId.PetTardigrade,PetColor.Color_2) ,"青" },
            { (PetId.PetTardigrade,PetColor.Color_3) ,"灰" },
            { (PetId.PetTardigrade,PetColor.Color_4) ,"白" },
            { (PetId.PetTardigrade,PetColor.Color_5) ,"赤" },
            { (PetId.PetTardigrade,PetColor.Color_6) ,"黒" },
            { (PetId.PetTardigrade,PetColor.Color_7) ,"橙" },
            { (PetId.PetTardigrade,PetColor.Color_8) ,"桃" },
            { (PetId.PetSlimeBlob,PetColor.Color_1) ,"----" },
            { (PetId.PetSlipperySlimeBlob,PetColor.Color_1) ,"----" },
            { (PetId.PetPoisonSlimeBlob,PetColor.Color_1) ,"----" },
            { (PetId.PetLavaSlimeBlob,PetColor.Color_1) ,"----" },
            { (PetId.PetPrinceSlimeBlob,PetColor.Color_1) ,"----" },
        };

        public static IReadOnlyDictionary<PetId, PetBattleType> BattleType
            = new Dictionary<PetId, PetBattleType>
            {
                { PetId.PetDog, PetBattleType.Melee},
                { PetId.PetCat, PetBattleType.Range},
                { PetId.PetBird, PetBattleType.Buff},
                { PetId.PetSlimeBlob, PetBattleType.Melee},
                { PetId.PetBunny, PetBattleType.Range},
                { PetId.PetSlipperySlimeBlob, PetBattleType.Melee},
                { PetId.PetPoisonSlimeBlob, PetBattleType.Melee},
                { PetId.PetLavaSlimeBlob, PetBattleType.Melee},
                { PetId.PetPrinceSlimeBlob, PetBattleType.Melee},
                { PetId.PetMoth, PetBattleType.Buff},
                { PetId.PetTardigrade, PetBattleType.Melee},
            };
    }
}
