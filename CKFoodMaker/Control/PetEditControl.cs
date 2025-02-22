using CKFoodMaker.Model;
using CKFoodMaker.Model.Pet;
using CKFoodMaker.Resource;
using System.Data;
using System.Text;

namespace CKFoodMaker.Control
{
    public partial class PetEditControl : UserControl
    {
        public PetEditControl()
        {
            InitializeComponent();
            InitControl();
        }

        private Item _petItem = Item.Default;
        public Item PetItem
        {
            get
            {
                _petItem = GetParameters();
                return _petItem;
            }
            set
            {
                _petItem = value;
                LoadPet(value);
            }
        }

        public void InitControl()
        {
            var petKinds = Enum.GetNames<PetId>();
            petKindComboBox.Items.AddRange(petKinds);
            var petColors = Enum.GetNames<PetColor>();
            petColorComboBox.Items.AddRange(petColors);
        }

        private static readonly IEnumerable<int> allPetIds = Enum.GetValues(typeof(PetId)).Cast<int>();

        public void LoadPet(Item petItem)
        {
            // 選択されたアイテムがペットである場合のみ処理を行う
            if (allPetIds.Contains(_petItem.Info.objectID))
            {
                int objectId = petItem.Info.objectID;
                PetId petId = (PetId)objectId;
                petKindComboBox.SelectedIndex = Array.IndexOf(
                    Enum.GetValues(typeof(PetId)).Cast<int>().ToArray(), objectId);
                petExpNumeric.Value = petItem.Info.amount;
                BattleTypeLabel.Text = PetResource.BattleType[petId].ToString();


                petItem.Aux.GetPetData(out var name, out var color, out List<PetTalent> talents);
                petColorComboBox.SelectedIndex = color;
                petNameTextBox.Text = name;

                LoadPetTalents(talents);
            }
            else
            {
                ResetPetTab();
            }
            
        }

        private void LoadPetTalents(List<PetTalent> talents)
        {
            var talentContrls = petSkillTableLayoutPanel.Controls.Cast<PetTalentControl>()
                .OrderBy(control => control.SlotNo)
                .ToList();

            for (int i = 0; i < 9; i++)
            {
                talentContrls[i].Talent = talents[i];
            }
        }

        public void ResetPetTab()
        {
            petColorComboBox.SelectedIndex = -1;
            petExpNumeric.Value = 0;
            petNameTextBox.Text = string.Empty;

            var petControls = petSkillTableLayoutPanel.Controls.Cast<PetTalentControl>()
                .OrderBy(control => control.SlotNo)
                .ToList();
            foreach (var control in petControls)
            {
                control.Reset();
            }
        }

        private void petKindComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            petColorComboBox.Items.Clear();

            var allPetType = Enum.GetValues<PetId>();
            var petTabDic = Enumerable.Range(0, allPetType.Length - 1)
                .Select(i => (i, allPetType[i]))
                .ToDictionary();
            var colorDic = Enumerable.Range(0, Enum.GetValues<PetColor>().Length - 1)
                .Select(i => (i, Enum.GetValues<PetColor>()[i]))
                .ToDictionary();
            //多色ペット判定
            if (new int[] { 0, 1, 2, 4, 9, 10 }.Contains(petKindComboBox.SelectedIndex))
            {
                foreach (var color in Enum.GetValues<PetColor>())
                {
                    petColorComboBox.Items.Add(PetResource.ColorDict[(allPetType[petKindComboBox.SelectedIndex], color)]);
                }
            }
            else
            {
                // スライム系
                petColorComboBox.Items.Add(PetResource.ColorDict[(allPetType[petKindComboBox.SelectedIndex], PetColor.Color_0)]);
            }
            petColorComboBox.SelectedIndex = 0;
        }

        private List<PetTalent> GeneratePetTalentLists()
        {
            var talents = petSkillTableLayoutPanel.Controls.Cast<PetTalentControl>()
                .OrderBy(control => control.SlotNo)
                .Select(control => control.Talent)
                .ToList();

            return talents;
        }

        private Item GetParameters()
        {
            var allPetTypes = Enum.GetValues<PetId>();
            var allPetColors = (PetColor[])Enum.GetValues(typeof(PetColor));

            //元アイテムがペットでない場合はauxIndexはデフォルト値で与える
            int auxIndex = 0;

            ItemInfo itemInfo = new(objectID: (int)allPetTypes[petKindComboBox.SelectedIndex],
                amount: (int)petExpNumeric.Value,
                variation: 0);
            string objectName = Enum.GetNames(typeof(PetId))[petKindComboBox.SelectedIndex];
            var auxData = _petItem.Aux;
            auxData.AuxPrefabManager?.UpdatePet(
                petNameTextBox.Text,
                allPetColors[petColorComboBox.SelectedIndex],
                GeneratePetTalentLists());

            return new Item(itemInfo, objectName, auxData);
        }

        private void petNameTextBox_TextChanged(object sender, EventArgs e)
        {
            string text = petNameTextBox.Text;
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            // ペット名が64バイトを超えたら溢れた分を削除する
            if (bytes.Length > 64)
            {
                while (Encoding.UTF8.GetByteCount(text) > 64)
                {
                    text = text.Substring(0, text.Length - 1);
                }
                petNameTextBox.Text = text;
                petNameTextBox.SelectionStart = text.Length; // キャレット位置を末尾に設定
            }
        }
    }
}
