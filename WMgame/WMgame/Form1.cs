using System.Configuration;

namespace WMgame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainGame main_=new MainGame();
            main_.Show();
            //this.Close();
        }
    }
}
