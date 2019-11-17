using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using RiftLibrary;
using MemoryLib;
using Ini;

namespace ProjectPhale
{
    public partial class ProjectPhale : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int extraInfo);

        [DllImport("user32.dll")]
        static extern short MapVirtualKey(int wCode, int wMapType);

        bool keypresswindow = false;
        Player pc;
        Target tar;
        int myselfptr;
        double xpStart;
        int kinahStart;
        int kills;
        int tarID;
        int tarLastKillID;
        DateTime xpStartTime;
        DateTime xpCurTime;
        TimeSpan elapsed;
        int resthp;
        int restmana;
        int healhp;
        int pothp;
        int potmp;
        int ignorelevel;
        int ignoretime;
        bool ishealer;
        bool isranged;
        bool isresting=false;
        int rangedist;
        List<string> preattacks = new List<string>();
        List<string> attacks = new List<string>();
        List<string> ignorelist = new List<string>();
        string keyloot;
        string keyrest;
        string keyhppot;
        string keymppot;
        string keytarget;
        string keyself;
        string keyturn;

        int findcounter;
        bool stopattack = true;

        public ProjectPhale()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception)
            {
                MessageBox.Show("Rift process not found! Exiting");
                Environment.Exit(1);
            }
        }



        public void timer1_Tick(object sender, EventArgs e) //200ms refresh
        {
            double xpperhr;
            double pcXP;
            double totalSeconds;
            float pcx = pc.X;
            float pcy = pc.Y;
            float pcz = pc.X;
            int pcMaxXP;

            label1.Text = "X: " + string.Format("{0:f3}",pcx);
            label2.Text = "Y: " + string.Format("{0:f3}",pcy);
            label3.Text = "Z: " + string.Format("{0:f3}",(pcz + 1.1)); //note the 1.1 added

            
            /*if (ignorelist.Contains(tar.Name) == true)//ignore button
            {
                btnuningnore.Enabled = true;
                btnignore.Enabled = false;

            }
            if (ignorelist.Contains(tar.Name) == false)
            {
                btnuningnore.Enabled = false;
                btnignore.Enabled = true;

            }
            if (tar.Name != "")
            {

                if (tar.Attitude == eAttitude.Hostile) lbltarget.ForeColor = Color.Red;
                if (tar.Attitude == eAttitude.Friendly) lbltarget.ForeColor = Color.Teal;
                if (tar.Attitude == eAttitude.Passive) lbltarget.ForeColor = Color.WhiteSmoke;
                if (tar.Attitude == eAttitude.Utility) lbltarget.ForeColor = Color.Green;
                
                lbltarget.Text = tar.Name + "(" + tar.Level + ")";
            }
            else lbltarget.Text = "";
            tarhealth.Maximum = 100;
            tarhealth.Value = tar.Health;
            */
            
            if(tar.ID == 0)
            {
                 tarhealth.CreateGraphics().DrawString("No Target", new Font("Arial", (float)8.25, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(tarhealth.Width / 2 - 25, tarhealth.Height / 2 - 7));
            }
            else tarhealth.CreateGraphics().DrawString(tarhealth.Value.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(tarhealth.Width / 2 - 15, tarhealth.Height / 2 - 7));

            elapsed = DateTime.Now - xpStartTime;
            elapsedLabel.Text = "Running\n" + (elapsed.ToString()).Substring(0,8);
            elapsed = DateTime.Now - xpCurTime;
            
            if (tar.HP_Current == 0 && tarLastKillID != tarID)
            {
                kills += 1;
                tarLastKillID = tarID;
                tarID = 0;
                killLabel.Text = "Kills: " + kills;
            }
            
            //pc.Update();
            healthbar.Maximum = pc.HP_Max;
            healthbar.Value = (int)pc.HP_Current;
            healthbar.CreateGraphics().DrawString(pc.HP_Current + "/" + pc.HP_Max, new Font("Arial", (float)8.25, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(healthbar.Width / 2 - 30, healthbar.Height / 2 - 7));
            manabar.Maximum = pc.MP_Max;
            manabar.Value = (int)pc.MP_Current;
            manabar.CreateGraphics().DrawString(pc.MP_Current + "/" + pc.MP_Max, new Font("Arial", (float)8.25, FontStyle.Bold), Brushes.WhiteSmoke, new PointF(manabar.Width / 2 - 30, manabar.Height / 2 - 7));
            displayrange();
            
            if (elapsed.TotalSeconds > 10) //updates screen stats
            {  
            //pcXP = pc.XP;
            //pcMaxXP = pc.MaxXP;
            //playerExp.Text = "Exp: " + string.Format("{0:f3}", ((pcXP / pcMaxXP) * 100)) + "%";//string.Format("{0:n0}", pcXP) + "/" + string.Format("{0:n0}", pcMaxXP);
            //playerProg.Maximum = pcMaxXP;
            //playerProg.Value = (int)pcXP;
            xpCurTime = DateTime.Now;
            elapsed = xpCurTime - xpStartTime;
            totalSeconds = elapsed.TotalSeconds;
            //xpperhr = Math.Round((pcXP - xpStart) * 3600 / totalSeconds / 1000, 2);
            //lblxpgain.Text = "XP: " + (pcXP - xpStart);
            //expHRLabel.Text = "Exp/Hr: " + string.Format("{0:n2}", xpperhr) + "k";
            //kinahLabel.Text = "Kinah " + (pc.Kinah - kinahStart);
            killLabel.Text = "Kills: " + kills;        
            }
        }


        private void resetAll_Click(object sender, EventArgs e)
        {
            //clearAll();
        } 

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab1 = new AboutBox1();
            ab1.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings f2 = new settings();
            f2.Show();

        }

        public void loadsettings()
        {
            IniFile ini = new IniFile(Environment.CurrentDirectory + "\\settings.ini");
            
            resthp = Int32.Parse(ini.IniReadValue("limits", "RestHP").TrimEnd(' '));
            restmana = Int32.Parse(ini.IniReadValue("limits", "RestMana").TrimEnd(' '));
            healhp = Int32.Parse(ini.IniReadValue("limits", "HealHP").TrimEnd(' '));
            pothp = Int32.Parse(ini.IniReadValue("limits", "PotHP").TrimEnd(' '));
            potmp = Int32.Parse(ini.IniReadValue("limits", "PotMP").TrimEnd(' '));
            ignorelevel = Int32.Parse(ini.IniReadValue("limits", "IgnoreLevel").TrimEnd(' '));
            ignoretime = Int32.Parse(ini.IniReadValue("limits", "IgnoreTime").TrimEnd(' '));

            ishealer = Convert.ToBoolean(ini.IniReadValue("character", "Healer").TrimEnd(' '));
            isranged = Convert.ToBoolean(ini.IniReadValue("character", "Ranged").TrimEnd(' '));
            rangedist = Int32.Parse(ini.IniReadValue("character", "RangeDist").TrimEnd(' '));
            
            string pretemp = ini.IniReadValue("preattacks", "PreAttacks");
            if (pretemp.Contains('\0').ToString() == "True")
            {
                pretemp = pretemp.Substring(0, pretemp.LastIndexOf('\0') - 0);
            }
            string[] listpreattack = pretemp.Split('|');
            preattacks.AddRange(listpreattack);
            
            string attacktemp = ini.IniReadValue("attacks", "Attacks");
            if (attacktemp.Contains('\0').ToString() == "True")
            {
                attacktemp = attacktemp.Substring(0, attacktemp.LastIndexOf('\0') - 0);
            }
            string[] listattack = attacktemp.Split('|');
            attacks.AddRange(listattack);
         
            keyloot = Convert.ToString(ini.IniReadValue("keybinds", "LootBtn"));
            keyrest = Convert.ToString(ini.IniReadValue("keybinds", "RestBtn"));
            keyhppot = Convert.ToString(ini.IniReadValue("keybinds", "Healthpot"));
            keymppot = Convert.ToString(ini.IniReadValue("keybinds", "Manapot"));
            keytarget = Convert.ToString(ini.IniReadValue("keybinds", "TargetBtn"));
            keyself = Convert.ToString(ini.IniReadValue("keybinds", "SelfTarget"));
            keyturn = Convert.ToString(ini.IniReadValue("keybinds", "TurnAround"));
        }


        /*private void findmob()
        {
            //tab around, push E
            while (tar.Type != eType.AttackableNPC && tar.Name == "" && (tar.Health <= 100) && ignorelist.Contains(tar.Name) && tar.Level <= ignorelevel)
            {
                if (isresting == false)
                {

                    if (findpcstance() == eStance.Resting) keyenumerator("w"); //get up
                    //if (tar.PtrTarget == myselfptr) return; //if something is attacking me
                    if (pc.Name == tar.Name) keyenumerator("ESC");
                    findcounter++;
                    if (findcounter > 4) //180 degree turn
                    {
                        keyenumerator(keyturn);
                        findcounter = 0;
                    }
                    PauseForMilliSeconds(2000);
                    keyenumerator(keytarget); //tab
                    
                    if (btnstop.Visible == false) return;
                }
               
            }
        }

        public void usehppot()
        {  
            if (((Convert.ToDouble(pc.Health) / Convert.ToDouble(pc.MaxHealth)) * 100) < pothp)
            {
                lblstatus.Text = "Status: Using HP Pot";
                keyenumerator(keyhppot);
                PauseForMilliSeconds(500);
            }
        }*/

        private void displayrange()
        {
            double distance;
            distance = pc.Distance2D(tar) - 1;
            lbldistance.Text = "Range: " + string.Format("{0:f2}",distance);

        }

        private void mainattackloop()
        {
            do
            {
                foreach (string item in attacks)
                {
                    
                    //usehppot();
                    if (tar.Stance == eStance.Dead)//dead
                    break;
                    
                    if (tar.Name == "")
                    break; //exits the attack loop
                    
                    if (stopattack == true)
                    return;
                    
                    lblstatus.Text = "Status: Attacking..";
                    string[] parseitem = item.Split(':');
                    string key;
                    double delayholder;
                    int delay;
                    key = parseitem[0];
                    if (parseitem[1] != "0")
                    {
                        delayholder = Convert.ToDouble(parseitem[1]) * 1000;
                        delay = Convert.ToInt32(delayholder);
                    }
                    else { delay = 50; }  //no delay
                   
                    if (tar.Stance == eStance.Dead || tar.IsDead == true) break;
                    //usehppot();
                    keyenumerator(key);
                    PauseForMilliSeconds(delay);
                }
                if (btnstop.Visible == false) return;
            }while (tar.IsDead != true || tar.Name == "" || tar.Health != 0);
        }

        private void attackmob()
        {
            if (tar.Type == eType.AttackableNPC)//can I fight it?
            {            
                lblstatus.Text = "Status: Pre-Attacking..";
                //start preattacks

                //LOOP this until mob targets me
                foreach (string item in preattacks)
                {
                    //usehppot();
                    string[] parseitem = item.Split(':');
                    string key;
                    double delayholder;
                    int delay;
                    if (stopattack == true || tar.Name == "") return;
                    key = parseitem[0];
                    delayholder = Convert.ToDouble(parseitem[1]) * 1000;
                    delay = Convert.ToInt32(delayholder); //optimize this
                    keyenumerator(key);
                    PauseForMilliSeconds(delay);
                }
                //start attack loop until mob is dead          
                mainattackloop();
            }
        }

        private void combat()
        {
            if (btnstop.Visible == false) return;
           
            while ((tar.IsDead == true || tar.Health == 0) && tar.Name != "")
            {
                lblstatus.Text = "Status: Looting..";
                keyenumerator(keyloot);
                PauseForMilliSeconds(2000);
            }
            if (btnstop.Visible == true) rest();//check to see if need rest
            if (tar.IsDead != true)
            {
                attackmob();
            }
            
            lblstatus.Text = "Status: Searching..";
            //if (btnstop.Visible == true) findmob();
            
            if (btnstop.Visible == true) attackmob();                     
            
            if (btnstop.Visible == true) combat();
            //TODO if nothing else then do waypoints
        }
        public void getpcptr()
        {
            lblstatus.Text = "Status: Getting Player Entity";
            while (tar.Name != pc.Name)
            {
                keyenumerator(keyself);
                PauseForMilliSeconds(1000);
            }
            
            myselfptr = tar.PtrEntity; //gets pc entity ptr
            if(tar.Name == pc.Name)
            keyenumerator("ESC");
            PauseForMilliSeconds(1000);
        }

        /*public eStance findpcstance()
        {
            eStance mystance = (eStance)Memory.ReadInt(Process.handle, (myselfptr + 0x20C)); //use pc entity
            //MessageBox.Show(myselfptr.ToString() + " stance: " + mystance);
            return mystance;
        }*/

        /*private void rest()
        { 
            double perresthp,perrestmana;
            perresthp = (Convert.ToDouble(pc.Health) / Convert.ToDouble(pc.MaxHealth)) * 100;
            perrestmana = (Convert.ToDouble(pc.MP) / Convert.ToDouble(pc.MaxMP)) * 100;         
            
            if (pc.Name == tar.Name) keyenumerator("ESC");
            
            if ((perresthp <= resthp || perrestmana <= restmana) && tar.Name == "")
            {
                lblstatus.Text = "Status: Resting..";
                isresting = true;
                while (perresthp < 100 && tar.Name == "")
                {                  
                    if (findpcstance() == eStance.Normal || findpcstance() == eStance.Combat) keyenumerator(keyrest);
                    perresthp = (Convert.ToDouble(pc.Health) / Convert.ToDouble(pc.MaxHealth)) * 100;
                    perrestmana = (Convert.ToDouble(pc.MP) / Convert.ToDouble(pc.MaxMP)) * 100;
                    if (btnstop.Visible == false) { isresting = false; return; } //you clicked stop button
                    PauseForMilliSeconds(2000);
                }  //bug on mana
                lblstatus.Text = "Status: Done Resting..";
                if (findpcstance() == eStance.Resting) keyenumerator(keyrest);//get up
                PauseForMilliSeconds(1500);
                isresting = false;
            }
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            //getpcptr();
            stopattack = false;
            button1.Visible = false;
            btnstop.Visible = true;
            combat();
        }      
        private void btnstop_Click_1(object sender, EventArgs e)
        {
            
            lblstatus.Text = "Status: Stopped";
            stopattack = true;
            btnstop.Visible = false;
            button1.Visible = true;
        }
        private void keyenumerator(string key)
        {
            if (key.Contains('\0').ToString() == "True")
            {
                key = key.Substring(0, key.LastIndexOf('\0') - 0);
            }
            switch (key)
            {
                case "F1":
                    keybd_event((int)Keys.F1, (byte)MapVirtualKey((int)Keys.F1, 0), 0, 0); //  Down
                    PauseForMilliSeconds(100);
                    keybd_event((int)Keys.F1, (byte)MapVirtualKey((int)Keys.F1, 0), 2, 0); //  Up 
                    break;
                case "a":
                    keybd_event((int)Keys.A, (byte)MapVirtualKey((int)Keys.A, 0), 0, 0); //  Down
                    PauseForMilliSeconds(100);
                    keybd_event((int)Keys.A, (byte)MapVirtualKey((int)Keys.A, 0), 2, 0); //  Up 
                    break;
                case "b":
                    keybd_event((int)Keys.B, (byte)MapVirtualKey((int)Keys.B, 0), 0, 0); //  Down
                    PauseForMilliSeconds(100);
                    keybd_event((int)Keys.B, (byte)MapVirtualKey((int)Keys.B, 0), 2, 0); //  Up 
                    break;
                case "c":
                    keybd_event((int)Keys.C, (byte)MapVirtualKey((int)Keys.C, 0), 0, 0); //  Down
                    PauseForMilliSeconds(100);
                    keybd_event((int)Keys.C, (byte)MapVirtualKey((int)Keys.C, 0), 2, 0); //  Up 
                    break;
                case "d":
                    keybd_event((int)Keys.D, (byte)MapVirtualKey((int)Keys.D, 0), 0, 0); //  Down
                    PauseForMilliSeconds(100);
                    keybd_event((int)Keys.D, (byte)MapVirtualKey((int)Keys.D, 0), 2, 0); //  Up 
                    break;
                case "e":
                    keybd_event((int)Keys.E, (byte)MapVirtualKey((int)Keys.E, 0), 0, 0); //  Down
                    PauseForMilliSeconds(100);
                    keybd_event((int)Keys.E, (byte)MapVirtualKey((int)Keys.E, 0), 2, 0); //  Up 
                break;
                case "f":
                keybd_event((int)Keys.F, (byte)MapVirtualKey((int)Keys.F, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.F, (byte)MapVirtualKey((int)Keys.F, 0), 2, 0); //  Up 
                break;
                case "g":
                keybd_event((int)Keys.G, (byte)MapVirtualKey((int)Keys.G, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.G, (byte)MapVirtualKey((int)Keys.G, 0), 2, 0); //  Up 
                break;
                case "h":
                keybd_event((int)Keys.H, (byte)MapVirtualKey((int)Keys.H, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.H, (byte)MapVirtualKey((int)Keys.H, 0), 2, 0); //  Up 
                break;
                case "i":
                keybd_event((int)Keys.I, (byte)MapVirtualKey((int)Keys.I, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.I, (byte)MapVirtualKey((int)Keys.I, 0), 2, 0); //  Up 
                break;
                case "j":
                keybd_event((int)Keys.J, (byte)MapVirtualKey((int)Keys.J, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.J, (byte)MapVirtualKey((int)Keys.J, 0), 2, 0); //  Up 
                break;
                case "k":
                keybd_event((int)Keys.K, (byte)MapVirtualKey((int)Keys.K, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.K, (byte)MapVirtualKey((int)Keys.K, 0), 2, 0); //  Up 
                break;
                case "l":
                keybd_event((int)Keys.L, (byte)MapVirtualKey((int)Keys.L, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.L, (byte)MapVirtualKey((int)Keys.L, 0), 2, 0); //  Up 
                break;
                case "m":
                keybd_event((int)Keys.M, (byte)MapVirtualKey((int)Keys.M, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.M, (byte)MapVirtualKey((int)Keys.M, 0), 2, 0); //  Up 
                break;
                case "n":
                keybd_event((int)Keys.N, (byte)MapVirtualKey((int)Keys.N, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.N, (byte)MapVirtualKey((int)Keys.N, 0), 2, 0); //  Up 
                break;
                case "o":
                keybd_event((int)Keys.O, (byte)MapVirtualKey((int)Keys.O, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.O, (byte)MapVirtualKey((int)Keys.O, 0), 2, 0); //  Up 
                break;
                case "p":
                keybd_event((int)Keys.P, (byte)MapVirtualKey((int)Keys.P, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.P, (byte)MapVirtualKey((int)Keys.P, 0), 2, 0); //  Up 
                break;
                case "q":
                keybd_event((int)Keys.Q, (byte)MapVirtualKey((int)Keys.Q, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Q, (byte)MapVirtualKey((int)Keys.Q, 0), 2, 0); //  Up 
                break;
                case "r":
                keybd_event((int)Keys.R, (byte)MapVirtualKey((int)Keys.R, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.R, (byte)MapVirtualKey((int)Keys.R, 0), 2, 0); //  Up 
                break;
                case "s":
                keybd_event((int)Keys.S, (byte)MapVirtualKey((int)Keys.S, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.S, (byte)MapVirtualKey((int)Keys.S, 0), 2, 0); //  Up 
                break;
                case "t":
                keybd_event((int)Keys.T, (byte)MapVirtualKey((int)Keys.T, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.T, (byte)MapVirtualKey((int)Keys.T, 0), 2, 0); //  Up 
                break;
                case "u":
                keybd_event((int)Keys.U, (byte)MapVirtualKey((int)Keys.U, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.U, (byte)MapVirtualKey((int)Keys.U, 0), 2, 0); //  Up 
                break;
                case "v":
                keybd_event((int)Keys.V, (byte)MapVirtualKey((int)Keys.V, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.V, (byte)MapVirtualKey((int)Keys.V, 0), 2, 0); //  Up 
                break;
                case "w":
                keybd_event((int)Keys.W, (byte)MapVirtualKey((int)Keys.W, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.W, (byte)MapVirtualKey((int)Keys.W, 0), 2, 0); //  Up 
                break;
                case "x":
                keybd_event((int)Keys.X, (byte)MapVirtualKey((int)Keys.X, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.X, (byte)MapVirtualKey((int)Keys.X, 0), 2, 0); //  Up 
                break;
                case "y":
                keybd_event((int)Keys.Y, (byte)MapVirtualKey((int)Keys.Y, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Y, (byte)MapVirtualKey((int)Keys.Y, 0), 2, 0); //  Up 
                break;
                case "z":
                keybd_event((int)Keys.Z, (byte)MapVirtualKey((int)Keys.Z, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Z, (byte)MapVirtualKey((int)Keys.Z, 0), 2, 0); //  Up 
                break;
                case "ESC":
                keybd_event((int)Keys.Escape, (byte)MapVirtualKey((int)Keys.Escape, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Escape, (byte)MapVirtualKey((int)Keys.Escape, 0), 2, 0); //  Up 
                break;
                case "1":
                keybd_event((int)Keys.D1, (byte)MapVirtualKey((int)Keys.D1, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D1, (byte)MapVirtualKey((int)Keys.D1, 0), 2, 0); //  Up 
                break;
                case "2":
                keybd_event((int)Keys.D2, (byte)MapVirtualKey((int)Keys.D2, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D2, (byte)MapVirtualKey((int)Keys.D2, 0), 2, 0); //  Up 
                break;
                case "3":
                keybd_event((int)Keys.D3, (byte)MapVirtualKey((int)Keys.D3, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D3, (byte)MapVirtualKey((int)Keys.D3, 0), 2, 0); //  Up 
                break;
                case "4":
                keybd_event((int)Keys.D4, (byte)MapVirtualKey((int)Keys.D4, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D4, (byte)MapVirtualKey((int)Keys.D4, 0), 2, 0); //  Up 
                break;
                case "5":
                keybd_event((int)Keys.D5, (byte)MapVirtualKey((int)Keys.D5, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D5, (byte)MapVirtualKey((int)Keys.D5, 0), 2, 0); //  Up 
                break;
                case "6":
                keybd_event((int)Keys.D6, (byte)MapVirtualKey((int)Keys.D6, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D6, (byte)MapVirtualKey((int)Keys.D6, 0), 2, 0); //  Up 
                break;
                case "7":
                keybd_event((int)Keys.D7, (byte)MapVirtualKey((int)Keys.D7, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D7, (byte)MapVirtualKey((int)Keys.D7, 0), 2, 0); //  Up 
                break;
                case "8":
                keybd_event((int)Keys.D8, (byte)MapVirtualKey((int)Keys.D8, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D8, (byte)MapVirtualKey((int)Keys.D8, 0), 2, 0); //  Up 
                break;
                case "9":
                keybd_event((int)Keys.D9, (byte)MapVirtualKey((int)Keys.D9, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D9, (byte)MapVirtualKey((int)Keys.D9, 0), 2, 0); //  Up 
                break;
                case "0":
                keybd_event((int)Keys.D0, (byte)MapVirtualKey((int)Keys.D0, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D0, (byte)MapVirtualKey((int)Keys.D0, 0), 2, 0); //  Up 
                break;
                case "-":
                keybd_event((int)Keys.OemMinus, (byte)MapVirtualKey((int)Keys.OemMinus, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.OemMinus, (byte)MapVirtualKey((int)Keys.OemMinus, 0), 2, 0); //  Up 
                break;
                case "=":
                keybd_event((int)Keys.Oemplus, (byte)MapVirtualKey((int)Keys.Oemplus, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Oemplus, (byte)MapVirtualKey((int)Keys.Oemplus, 0), 2, 0); //  Up 
                break;
                case ",":
                keybd_event((int)Keys.Oemcomma, (byte)MapVirtualKey((int)Keys.Oemcomma, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Oemcomma, (byte)MapVirtualKey((int)Keys.Oemcomma, 0), 2, 0); //  Up 
                break;             
                case "TAB":
                keybd_event((int)Keys.Tab, (byte)MapVirtualKey((int)Keys.Tab, 0), 0, 0); //  Down
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Tab, (byte)MapVirtualKey((int)Keys.Tab, 0), 2, 0); //  Up 
                break;

                case "Alt,1":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Alt
                PauseForMilliSeconds(100); 
                keybd_event((int)Keys.D1, (byte)MapVirtualKey((int)Keys.D1, 0), 0, 0); //  Down
                keybd_event((int)Keys.D1, (byte)MapVirtualKey((int)Keys.D1, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100); 
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Alt
                break;
                case "Alt,2":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Alt
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D2, (byte)MapVirtualKey((int)Keys.D2, 0), 0, 0); //  Down
                keybd_event((int)Keys.D2, (byte)MapVirtualKey((int)Keys.D2, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Alt
                break;
                case "Alt,3":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Alt
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D3, (byte)MapVirtualKey((int)Keys.D3, 0), 0, 0); //  Down
                keybd_event((int)Keys.D3, (byte)MapVirtualKey((int)Keys.D3, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,4":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D4, (byte)MapVirtualKey((int)Keys.D4, 0), 0, 0); //  Down
                keybd_event((int)Keys.D4, (byte)MapVirtualKey((int)Keys.D4, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,5":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D5, (byte)MapVirtualKey((int)Keys.D5, 0), 0, 0); //  Down
                keybd_event((int)Keys.D5, (byte)MapVirtualKey((int)Keys.D5, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,6":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D6, (byte)MapVirtualKey((int)Keys.D6, 0), 0, 0); //  Down
                keybd_event((int)Keys.D6, (byte)MapVirtualKey((int)Keys.D6, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,7":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D7, (byte)MapVirtualKey((int)Keys.D7, 0), 0, 0); //  Down
                keybd_event((int)Keys.D7, (byte)MapVirtualKey((int)Keys.D7, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,8":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D8, (byte)MapVirtualKey((int)Keys.D8, 0), 0, 0); //  Down
                keybd_event((int)Keys.D8, (byte)MapVirtualKey((int)Keys.D8, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,9":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D9, (byte)MapVirtualKey((int)Keys.D9, 0), 0, 0); //  Down
                keybd_event((int)Keys.D9, (byte)MapVirtualKey((int)Keys.D9, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Menu
                break;
                case "Alt,0":
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 0, 0); //  Menu
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D0, (byte)MapVirtualKey((int)Keys.D0, 0), 0, 0); //  Down
                keybd_event((int)Keys.D0, (byte)MapVirtualKey((int)Keys.D0, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.Menu, (byte)MapVirtualKey((int)Keys.Menu, 0), 2, 0); //  Alt
                break;
                case "Ctrl,1":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  Alt
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D1, (byte)MapVirtualKey((int)Keys.D1, 0), 0, 0); //  Down
                keybd_event((int)Keys.D1, (byte)MapVirtualKey((int)Keys.D1, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  Alt
                break;
                case "Ctrl,2":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  Alt
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D2, (byte)MapVirtualKey((int)Keys.D2, 0), 0, 0); //  Down
                keybd_event((int)Keys.D2, (byte)MapVirtualKey((int)Keys.D2, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  Alt
                break;
                case "Ctrl,3":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  Alt
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D3, (byte)MapVirtualKey((int)Keys.D3, 0), 0, 0); //  Down
                keybd_event((int)Keys.D3, (byte)MapVirtualKey((int)Keys.D3, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,4":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D4, (byte)MapVirtualKey((int)Keys.D4, 0), 0, 0); //  Down
                keybd_event((int)Keys.D4, (byte)MapVirtualKey((int)Keys.D4, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,5":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D5, (byte)MapVirtualKey((int)Keys.D5, 0), 0, 0); //  Down
                keybd_event((int)Keys.D5, (byte)MapVirtualKey((int)Keys.D5, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,6":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D6, (byte)MapVirtualKey((int)Keys.D6, 0), 0, 0); //  Down
                keybd_event((int)Keys.D6, (byte)MapVirtualKey((int)Keys.D6, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,7":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D7, (byte)MapVirtualKey((int)Keys.D7, 0), 0, 0); //  Down
                keybd_event((int)Keys.D7, (byte)MapVirtualKey((int)Keys.D7, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,8":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D8, (byte)MapVirtualKey((int)Keys.D8, 0), 0, 0); //  Down
                keybd_event((int)Keys.D8, (byte)MapVirtualKey((int)Keys.D8, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,9":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D9, (byte)MapVirtualKey((int)Keys.D9, 0), 0, 0); //  Down
                keybd_event((int)Keys.D9, (byte)MapVirtualKey((int)Keys.D9, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  ControlKey
                break;
                case "Ctrl,0":
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 0, 0); //  ControlKey
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.D0, (byte)MapVirtualKey((int)Keys.D0, 0), 0, 0); //  Down
                keybd_event((int)Keys.D0, (byte)MapVirtualKey((int)Keys.D0, 0), 2, 0); //  Up 
                PauseForMilliSeconds(100);
                keybd_event((int)Keys.ControlKey, (byte)MapVirtualKey((int)Keys.ControlKey, 0), 2, 0); //  Alt
                break;
            }
            if (listkeypress.Items.Count >= 19) listkeypress.Items.Clear();
            listkeypress.Items.Add(key);
        }
        public static DateTime PauseForMilliSeconds(int MilliSecondsToPauseFor)
        {

            if (MilliSecondsToPauseFor < 0) MilliSecondsToPauseFor = 50;
            
                System.DateTime ThisMoment = System.DateTime.Now;
                System.TimeSpan duration = new System.TimeSpan(0, 0, 0, 0, MilliSecondsToPauseFor);
                System.DateTime AfterWards = ThisMoment.Add(duration);


                while (AfterWards >= ThisMoment)
                {
                    System.Windows.Forms.Application.DoEvents();
                    ThisMoment = System.DateTime.Now;
                }

            
            return System.DateTime.Now;
        }

        private void btnignore_Click(object sender, EventArgs e)
        {
            if (tar.Name != "")
            {
                ignorelist.Add(tar.Name);
            }
        }
        private void btnuningnore_Click(object sender, EventArgs e)
        {
            if (tar.Name != "")
            {
                ignorelist.Remove(tar.Name);
            }
        }
        private void keypressWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (keypresswindow == false)
            {
                this.Width = 566;
                keypresswindow = true;
            }
            else
            {
                this.Width = 414;        //414normal
                keypresswindow = false;
            }
        }

        private void ProjectPhale_Load(object sender, EventArgs e)
        {

        }   

    }
}
