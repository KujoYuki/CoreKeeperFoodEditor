using CKFoodMaker.Model;
using CKFoodMaker.Resource;

namespace CKFoodMaker.Control
{
    public partial class PetTalentControl : UserControl
    {
        public PetTalentControl()
        {
            InitializeComponent();
            LoadTalentList();
        }

        private int _slotNo;
        public int SlotNo
        {
            get { return _slotNo; }
            set
            {
                _slotNo = value;
                petTalentCheckBox.Text = $"スキル {value + 1}";
            }
        }

        private PetTalent _petTalent = PetTalent.Default;
        public PetTalent Talent
        {
            get
            {
                return _petTalent;
            }
            set
            {
                petTalentComboBox.SelectedIndex = value.Talent;
                petTalentCheckBox.Checked = value.Points is 1;
            }
        }

        public void LoadTalentList()
        {
            foreach (var item in StaticResource.PetTalents)
            {
                petTalentComboBox.Items.Add($"{item.Key}：{item.Value}");
            }
        }

        public void Reset()
        {
            petTalentCheckBox.Checked = false;
            petTalentComboBox.SelectedIndex = -1;
        }

        private void petTalentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _petTalent.Points = petTalentCheckBox.Checked ? 1 : 0;
        }

        private void petTalentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _petTalent.Talent = petTalentComboBox.SelectedIndex;
        }
    }
}
