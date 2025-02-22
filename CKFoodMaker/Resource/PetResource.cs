using CKFoodMaker.Model.Pet;
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
            { (PetId.PetDog,PetColor.Color_0) ,"茶(Default)" },
            { (PetId.PetDog,PetColor.Color_1) ,"白(Arctic)" },
            { (PetId.PetDog,PetColor.Color_2) ,"暗灰(Ash)" },
            { (PetId.PetDog,PetColor.Color_3) ,"黄(Blonde)" },
            { (PetId.PetDog,PetColor.Color_4) ,"橙(Caramel)" },
            { (PetId.PetDog,PetColor.Color_5) ,"灰(Gray)" },
            { (PetId.PetDog,PetColor.Color_6) ,"黒(Midnight)" },
            { (PetId.PetDog,PetColor.Color_7) ,"桃(Strawberry)" },
            { (PetId.PetCat,PetColor.Color_0) ,"白(Default)" },
            { (PetId.PetCat,PetColor.Color_1) ,"黄(Blonde)" },
            { (PetId.PetCat,PetColor.Color_2) ,"桃(Camel)" },
            { (PetId.PetCat,PetColor.Color_3) ,"橙(Citrus)" },
            { (PetId.PetCat,PetColor.Color_4) ,"紫(Fuschia)" },
            { (PetId.PetCat,PetColor.Color_5) ,"灰(Gray)" },
            { (PetId.PetCat,PetColor.Color_6) ,"青(Stormy)" },
            { (PetId.PetCat,PetColor.Color_7) ,"茶(Tan)" },
            { (PetId.PetBird,PetColor.Color_0) ,"緑(Default)" },
            { (PetId.PetBird,PetColor.Color_1) ,"橙(Amber)" },
            { (PetId.PetBird,PetColor.Color_2) ,"白(Ice)" },
            { (PetId.PetBird,PetColor.Color_3) ,"紫(Grape)" },
            { (PetId.PetBird,PetColor.Color_4) ,"青(Ocean)" },
            { (PetId.PetBird,PetColor.Color_5) ,"桃(Pink)" },
            { (PetId.PetBird,PetColor.Color_6) ,"灰(Stormy)" },
            { (PetId.PetBird,PetColor.Color_7) ,"黄(Sunny)" },
            { (PetId.PetBunny,PetColor.Color_0) ,"白(Dfault)" },
            { (PetId.PetBunny,PetColor.Color_1) ,"卵(Banana)" },
            { (PetId.PetBunny,PetColor.Color_2) ,"黄(Blonde)" },
            { (PetId.PetBunny,PetColor.Color_3) ,"灰(Johnnyh)" },
            { (PetId.PetBunny,PetColor.Color_4) ,"桃(Petunia)" },
            { (PetId.PetBunny,PetColor.Color_5) ,"黒(Slate)" },
            { (PetId.PetBunny,PetColor.Color_6) ,"茶(Tawny)" },
            { (PetId.PetBunny,PetColor.Color_7) ,"橙(Tiger)" },
            { (PetId.PetMoth,PetColor.Color_0) ,"白(Default)" },
            { (PetId.PetMoth,PetColor.Color_1) ,"灰(BlackSesame)" },
            { (PetId.PetMoth,PetColor.Color_2) ,"茶(Chocochip)" },
            { (PetId.PetMoth,PetColor.Color_3) ,"黄(Mango)" },
            { (PetId.PetMoth,PetColor.Color_4) ,"水色(Mint)" },
            { (PetId.PetMoth,PetColor.Color_5) ,"緑(Pistachio)" },
            { (PetId.PetMoth,PetColor.Color_6) ,"桃(Strawberry)" },
            { (PetId.PetMoth,PetColor.Color_7) ,"卵(Vanilla)" },
            { (PetId.PetTardigrade,PetColor.Color_0) ,"緑(Default)" },
            { (PetId.PetTardigrade,PetColor.Color_1) ,"青(Bobo)" },
            { (PetId.PetTardigrade,PetColor.Color_2) ,"灰(Dreamy)" },
            { (PetId.PetTardigrade,PetColor.Color_3) ,"白(Milkman)" },
            { (PetId.PetTardigrade,PetColor.Color_4) ,"赤(Pomegranate)" },
            { (PetId.PetTardigrade,PetColor.Color_5) ,"黒(Shadow)" },
            { (PetId.PetTardigrade,PetColor.Color_6) ,"橙(Tangerine)" },
            { (PetId.PetTardigrade,PetColor.Color_7) ,"桃(Bubblegum)" },
            { (PetId.PetSlimeBlob,PetColor.Color_0) ,"----" },
            { (PetId.PetSlipperySlimeBlob,PetColor.Color_0) ,"----" },
            { (PetId.PetPoisonSlimeBlob,PetColor.Color_0) ,"----" },
            { (PetId.PetLavaSlimeBlob,PetColor.Color_0) ,"----" },
            { (PetId.PetPrinceSlimeBlob,PetColor.Color_0) ,"----" },
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
