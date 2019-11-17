using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ini;

namespace AngelBot
{
    public partial class settings : Form
    {
        public settings()
        {
            InitializeComponent();
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void loadsettings()
        {
            IniFile ini = new IniFile(Environment.CurrentDirectory + "\\settings.ini");
            //Tab1
            textBox1.Text = ini.IniReadValue("limits", "RestHP");
            textBox2.Text = ini.IniReadValue("limits", "RestMana");
            textBox3.Text = ini.IniReadValue("limits", "HealHP");
            textBox4.Text = ini.IniReadValue("limits", "PotHP");
            textBox5.Text = ini.IniReadValue("limits", "PotMP");
            textBox6.Text = ini.IniReadValue("limits", "IgnoreLevel");
            textBox7.Text = ini.IniReadValue("limits", "IgnoreTime");
            //Tab2
            if ("True" == ini.IniReadValue("character", "Healer"))
            { checkBox1.Checked = true; }
            else { checkBox1.Checked = false; }
            
            if ("True" == ini.IniReadValue("character", "Ranged"))
            { checkBox2.Checked = true; }
            else { checkBox2.Checked = false; }
            textBox8.Text = ini.IniReadValue("character", "RangeDist");
            //Tab3
            string pretemp = ini.IniReadValue("preattacks", "PreAttacks");
            if (pretemp.Contains('\0').ToString() == "True")
            {
                pretemp = pretemp.Substring(0, pretemp.LastIndexOf('\0') - 0);
            }
            string[] listpreattack = pretemp.Split('|');
            listBox1.Items.AddRange(listpreattack);
            //Tab4
                string attacktemp = ini.IniReadValue("attacks", "Attacks");
                if (attacktemp.Contains('\0').ToString() == "True")
                {
                    attacktemp = attacktemp.Substring(0, attacktemp.LastIndexOf('\0') - 0);
                }
                string[] listattack = attacktemp.Split('|');
                listBox2.Items.AddRange(listattack);
            //Tab5

            //Tab6
                txtloot.Text = ini.IniReadValue("keybinds", "LootBtn");
                txtrest.Text = ini.IniReadValue("keybinds", "RestBtn");
                txthppot.Text = ini.IniReadValue("keybinds", "Healthpot");
                txtmppot.Text = ini.IniReadValue("keybinds", "Manapot");
                txttarget.Text = ini.IniReadValue("keybinds", "TargetBtn");
                txtself.Text = ini.IniReadValue("keybinds", "SelfTarget");
                txtturn.Text = ini.IniReadValue("keybinds", "TurnAround");
           
        }
       
             

        
        
        private void settings_Load(object sender, EventArgs e)
        {
            loadsettings();
        }

        

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string butt = (string)listBox2.SelectedItem;;
            if (butt != null)
            {
                string[] theitem = butt.Split(':');
                textBox11.Text = theitem[0];
                textBox12.Text = theitem[1];
            }
            button12.Enabled = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string butt = (string)listBox1.SelectedItem;
            if (butt != null)
            {
                string[] theitem = butt.Split(':');
                textBox9.Text = theitem[0];
                textBox10.Text = theitem[1];
            }
            button11.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
            int i = this.listBox2.SelectedIndex;
            object o = this.listBox2.SelectedItem;

            if (i > 0)
            {
                this.listBox2.Items.RemoveAt(i);
                this.listBox2.Items.Insert(i - 1, o);
                this.listBox2.SelectedIndex = i - 1;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int i = this.listBox2.SelectedIndex;
            object o = this.listBox2.SelectedItem;

            if (i < this.listBox2.Items.Count - 1)
            {
                this.listBox2.Items.RemoveAt(i);
                this.listBox2.Items.Insert(i + 1, o);
                this.listBox2.SelectedIndex = i + 1;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = this.listBox1.SelectedIndex;
            object o = this.listBox1.SelectedItem;

            if (i > 0)
            {
                this.listBox1.Items.RemoveAt(i);
                this.listBox1.Items.Insert(i - 1, o);
                this.listBox1.SelectedIndex = i - 1;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int i = this.listBox1.SelectedIndex;
            object o = this.listBox1.SelectedItem;

            if (i < this.listBox1.Items.Count - 1)
            {
                this.listBox1.Items.RemoveAt(i);
                this.listBox1.Items.Insert(i + 1, o);
                this.listBox1.SelectedIndex = i + 1;
            }
        }

        private void savesettings()
        {
            IniFile ini = new IniFile(Environment.CurrentDirectory + "\\settings.ini");

            ini.IniWriteValue("limits", "RestHP", textBox1.Text);
            ini.IniWriteValue("limits", "RestMana", textBox2.Text);
            ini.IniWriteValue("limits", "HealHP", textBox3.Text);
            ini.IniWriteValue("limits", "PotHP", textBox4.Text);
            ini.IniWriteValue("limits", "PotMP", textBox5.Text);
            ini.IniWriteValue("limits", "IgnoreLevel", textBox6.Text);
            ini.IniWriteValue("limits", "IgnoreTime", textBox7.Text);
            
            if (checkBox1.Checked == true)
            {
                ini.IniWriteValue("character", "Healer","True");
            }
            else{ini.IniWriteValue("character", "Healer","False");}
            
            if (checkBox2.Checked == true) 
            {
                ini.IniWriteValue("character", "Ranged", "True");
            }
            else {ini.IniWriteValue("character", "Ranged", "False");}
            
            ini.IniWriteValue("character", "RangeDist", textBox8.Text);
            
            string listpreattack="";
            foreach (object item in listBox1.Items)
            {
                listpreattack = listpreattack + item.ToString().TrimEnd(null) + "|";
            }
            listpreattack = listpreattack.TrimEnd('|');
            ini.IniWriteValue("preattacks", "PreAttacks", listpreattack);

            string listattacks="";
            foreach (object item in listBox2.Items)
            {
                listattacks = listattacks + Convert.ToString(item).TrimEnd(null) + "|";
            }
            listattacks = listattacks.TrimEnd('|');
            ini.IniWriteValue("attacks", "Attacks", listattacks);

              ini.IniWriteValue("keybinds", "LootBtn",txtloot.Text);
              ini.IniWriteValue("keybinds", "RestBtn", txtrest.Text);
              ini.IniWriteValue("keybinds", "Healthpot", txthppot.Text);
              ini.IniWriteValue("keybinds", "Manapot", txtmppot.Text);
              ini.IniWriteValue("keybinds", "TargetBtn", txttarget.Text);
              ini.IniWriteValue("keybinds", "SelfTarget", txtself.Text);
              ini.IniWriteValue("keybinds", "TurnAround", txtturn.Text);
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            savesettings(); //writes to ini
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            f1.loadsettings(); //loads new settings into RAM
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newitem = textBox9.Text + ":" + textBox10.Text;
            listBox1.Items.Add(newitem);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string newitem = textBox11.Text + ":" + textBox12.Text;
            listBox2.Items.Add(newitem);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.SelectedItem);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            f1.Opacity = (Convert.ToDouble(textBox13.Text) / 100);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int i = this.listBox1.SelectedIndex;
            object o = this.listBox1.SelectedItem;
            object newobj = textBox9.Text + ":" + textBox10.Text;
 
            if (i < this.listBox1.Items.Count - 1)
            {
                this.listBox1.Items[i] = newobj;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int i = this.listBox2.SelectedIndex;
            object o = this.listBox2.SelectedItem;
            object newobj = textBox11.Text + ":" + textBox12.Text;

            if (i < this.listBox2.Items.Count - 1)
            {
                this.listBox2.Items[i] = newobj;
            }
        }

        
        
    }
}
