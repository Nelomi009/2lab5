using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2lab2 {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      this.Hide();
      Form2 task1Form = new Form2();
      task1Form.ShowDialog();
      this.Show();
    }

    private void button1_Click_1(object sender, EventArgs e) {
      this.Hide();
      Form2 task1Form = new Form2();
      task1Form.ShowDialog();
      this.Show();
    }

    private void button2_Click(object sender, EventArgs e) {
      this.Hide();
      task2 task2Form = new task2();
      task2Form.ShowDialog();
      this.Show();
    }

    private void button3_Click(object sender, EventArgs e) {
      this.Hide();
      task3 task3Form = new task3();
      task3Form.ShowDialog();
      this.Show();
    }

    private void label1_Click(object sender, EventArgs e) {

    }

    private void button4_Click(object sender, EventArgs e) {
      this.Hide();
      task4 task4Form = new task4();
      task4Form.ShowDialog();
      this.Show();
    }
  }
}