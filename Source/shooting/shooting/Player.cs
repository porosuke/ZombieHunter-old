using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using static System.Windows.Forms.LinkLabel;
using static shooting.Main;
using System.Runtime.Remoting.Services;
using System.Threading;
using System.IO;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace shooting
{
    internal class Player
    {
        //リスト
        private List<clsBullet_P> Bullets_P = new List<clsBullet_P>();
        //インスタンス作成
        private Move move = new Move();
        private clsBullet_P shot;
        //座標
        public PointF my, center;
        //速度
        public int speed;
        //回転角度
        public float angle;
        //死んだか
        public bool dead;
        //画像
        public Image Maptip,img,m_b,s_b,death;
        //体力
        public int health, max_health, before_health;
        //弾
        public int hg_b, hg_mag, ar_b, ar_mag, sg_b, sg_mag, sr_b, sr_mag;
        //武器の記録
        public String before_weapon = "knife";
        public bool damage_flag = false;
        //音
        SoundPlayer cut,down,AR_release,HG_set,HG_shot,MAG_release,MAG_set,Out,SG_pomp,SG_shot,SR_cock,SR_reload,SR_set,SR_shot;
        Stream strm;
        //アニメーションタイマー
        private int Ani_time,change_time;
        private int health_timer = 0;
        //タイマー
        private int time = 0;
        private const int hg_reload_interval = 10;
        private const int sg_reload_interval = 20;
        private const int ar_reload_interval = 30;
        private const int sr_reload_interval = 40;
        
        //bool
        public static bool reload_now,shot_now,change_now;
        //武器の情報
        private const int mag_hg_b = 18;
        private const int mag_ar_b = 30;
        private const int mag_sg_b = 7;
        private const int mag_sr_b = 10;
        private const int max_hg_b = 50;
        private const int max_ar_b = 100;
        private const int max_sg_b = 30;
        private const int max_sr_b = 20;
        private int dummy = 1;

        private List<position> AR_changeL = new List<position>();
        private List<position> AR_reloadL = new List<position>();
        private List<position> AR_shotL = new List<position>();
        private List<position> HG_changeL = new List<position>();
        private List<position> HG_shotL = new List<position>();
        private List<position> KN_changeL = new List<position>();
        private List<position> KN_shotL = new List<position>();
        private List<position> SG_changeL = new List<position>();
        private List<position> SG_reloadL = new List<position>();
        private List<position> SG_shotL = new List<position>();
        private List<position> SR_changeL = new List<position>();
        private List<position> SR_reloadL = new List<position>();
        private List<position> SR_shotL = new List<position>();


        public struct position
        {
            public Image image;
            public float x;
            public float y;
        }

        public Player(float x, float y, int s, float a, int h)
        {
            my = new PointF(x, y);
            speed = s;
            angle = a;
            health = h;
            max_health = h;
            dead = false;

            hg_b = 10;
            ar_b = 10;
            sg_b = 10;
            sr_b = 10;

            hg_mag = 5;
            ar_mag = 3;
            sg_mag = 7;
            sr_mag = 2;

            strm = Properties.Resources.cut;
            cut = new SoundPlayer(strm);
            strm = Properties.Resources.down;
            down = new SoundPlayer(strm);
            strm = Properties.Resources.ARrelease;
            AR_release = new SoundPlayer(strm);
            strm = Properties.Resources.HGset;
            HG_set = new SoundPlayer(strm);
            strm = Properties.Resources.HGshot;
            HG_shot = new SoundPlayer(strm);
            strm = Properties.Resources.Magrelease;
            MAG_release = new SoundPlayer(strm);
            strm = Properties.Resources.Magset;
            MAG_set = new SoundPlayer(strm);
            strm = Properties.Resources.outofbullets;
            Out = new SoundPlayer(strm);
            strm = Properties.Resources.SGpomp;
            SG_pomp = new SoundPlayer(strm);
            strm = Properties.Resources.SGshot;
            SG_shot = new SoundPlayer(strm);
            strm = Properties.Resources.SRcock;
            SR_cock = new SoundPlayer(strm);
            strm = Properties.Resources.SRreload;
            SR_reload = new SoundPlayer(strm);
            strm = Properties.Resources.SRset;
            SR_set = new SoundPlayer(strm);
            strm = Properties.Resources.SRshot;
            SR_shot = new SoundPlayer(strm);

            Maptip = Properties.Resources.player_maptip;
            m_b = Division(1268,240,28,28);
            s_b = Division(1303, 242, 18, 18);
            death = Division(1321, 525, 107, 105);
            img = Division(1,0,100,100);

            //normalはshotに入れておこう
            Set(Division(767, 239, 101, 203), 0, 99 / 2, AR_shotL);
            Set(Division(980, 239, 101, 228), 0, 125 / 2, AR_shotL);
            Set(Division(1084, 239, 101, 204), 0, 99 / 2, AR_shotL);

            Set(Division(873, 239, 101, 203), 0, 99 / 2, AR_reloadL);

            Set(Division(514, 239, 125, 193), 0, 88 / 2, AR_changeL);
            Set(Division(643, 239, 123, 190), 0, 83 / 2 , AR_changeL);

            Set(Division(205, 239, 100, 183), 0, 71 / 2, HG_shotL);
            Set(Division(307, 239, 100, 216), 0, 115 / 2, HG_shotL);
            Set(Division(410, 239, 100, 181), 0, 80 / 2 , HG_shotL);

            Set(Division(0, 239, 100, 181), 0, 80 / 2, HG_changeL);
            Set(Division(102, 239, 100, 172), 0, 70 / 2, HG_changeL);


            Set(Division(100, 0, 108, 158), 0, 54 / 2, KN_changeL);
            Set(Division(965, 0, 135, 188), 0, 87 / 2, KN_changeL);
            Set(Division(1100, 0, 164, 163), 0, 55 / 2, KN_changeL);
            Set(Division(1267, 0, 168, 122), 0, 17 / 2, KN_changeL);

            Set(Division(101, 0, 107, 156), 0, 54 / 2, KN_shotL);
            Set(Division(208, 0, 106, 179), 0, 75 / 2, KN_shotL);
            Set(Division(318, 0, 120, 182), 0, 80 / 2, KN_shotL);
            Set(Division(440, 0, 143, 172), 0, 69 / 2, KN_shotL);
            Set(Division(585, 0, 180, 150), 0, 47 / 2, KN_shotL);
            Set(Division(768, 0, 196, 118), 0, 14 / 2, KN_shotL);

            Set(Division(237, 526, 102, 190), 0, 89 / 2, SG_shotL);
            Set(Division(444, 526, 102, 216), 0, 115 / 2, SG_shotL);
            Set(Division(546, 526, 102, 191), 0, 89 / 2, SG_shotL);

            Set(Division(341, 526, 101, 191), 0, 88 / 2, SG_reloadL);

            Set(Division(1, 526, 122, 175), 0, 73 / 2, SG_changeL);
            Set(Division(127, 526, 106, 184), 0, 82 / 2, SG_changeL);

            Set(Division(909, 526, 100, 213), 0, 111 / 2, SR_shotL);
            Set(Division(1114, 526, 100, 240), 0, 137 / 2, SR_shotL);
            Set(Division(1218, 526, 100, 215), 0, 111 / 2, SR_shotL);

            Set(Division(1013, 526, 100, 213), 0, 111 / 2, SR_reloadL);

            Set(Division(650, 526, 135, 198), 0, 94 / 2, SR_changeL);
            Set(Division(790, 526, 113, 211), 0, 107 / 2, SR_changeL);
        }
        private Bitmap Division(int x, int y,int width, int height)
        {//画像切り出し
            Rectangle sourceRectange = new Rectangle(new Point(x, y), new Size(width, height));
            Bitmap bitmap1 = new Bitmap((int)(width * 0.5), (int)(height * 0.5));
            Graphics graphics = Graphics.FromImage(bitmap1);
            graphics.DrawImage(Maptip, new Rectangle(0, 0,(int)(width * 0.5), (int)(height * 0.5)), sourceRectange, GraphicsUnit.Pixel);
            graphics.Dispose();
            return bitmap1;
        }
        private void Set(Image img, float gapx, float gapy,List<position> posi)
        {
            position p = new position()
            {
                image = img,
                x = gapx,
                y = gapy
            };
            posi.Add(p);
        }
        public void Draw(Graphics g)
        {
            foreach (clsBullet_P bullet_P in Bullets_P)
            {
                bullet_P.draw(g);
            }

            if (!dead)
            {
                g.ResetTransform();
                g.TranslateTransform(-(my.X + img.Width / 2), -(my.Y + img.Width / 2));
                g.RotateTransform(angle, MatrixOrder.Append);
                g.TranslateTransform(my.X + img.Width / 2, my.Y + img.Width / 2, MatrixOrder.Append);
                if (before_health == health)
                {
                    if (change_now)
                    {
                        switch (Key.weapon)
                        {
                            case "knife":
                                g.DrawImage(KN_changeL[change_time].image, my.X - KN_changeL[change_time].x, my.Y - KN_changeL[change_time].y);
                                break;
                            case "handgun":
                                g.DrawImage(HG_changeL[change_time].image, my.X - HG_changeL[change_time].x, my.Y - HG_changeL[change_time].y);
                                break;
                            case "shotgun":
                                g.DrawImage(SG_changeL[Ani_time].image, my.X - SG_changeL[Ani_time].x, my.Y - SG_changeL[Ani_time].y);
                                break;
                            case "carbine":
                                g.DrawImage(AR_changeL[Ani_time].image, my.X - AR_changeL[Ani_time].x, my.Y - AR_changeL[Ani_time].y);
                                break;
                            case "sniper":
                                g.DrawImage(SR_changeL[Ani_time].image, my.X - SR_changeL[Ani_time].x, my.Y - SR_changeL[Ani_time].y);
                                break;
                        }
                    }
                    else if (reload_now)
                    {
                        switch (Key.weapon)
                        {
                            case "knife":
                                g.DrawImage(KN_shotL[0].image, my.X - KN_shotL[0].x, my.Y - KN_shotL[0].y);
                                break;
                            case "handgun":
                                g.DrawImage(HG_shotL[0].image, my.X - HG_shotL[0].x, my.Y - HG_shotL[0].y);
                                break;
                            case "shotgun":
                                g.DrawImage(SG_reloadL[0].image, my.X - SG_reloadL[0].x, my.Y - SG_reloadL[0].y);
                                break;
                            case "carbine":
                                g.DrawImage(AR_reloadL[0].image, my.X - AR_reloadL[0].x, my.Y - AR_reloadL[0].y);
                                break;
                            case "sniper":
                                g.DrawImage(SR_reloadL[0].image, my.X - SR_reloadL[0].x, my.Y - SR_reloadL[0].y);
                                break;
                        }
                    }
                    else
                    {
                        switch (Key.weapon)
                        {
                            case "knife":
                                if (Key.wait > 0) g.DrawImage(KN_shotL[5].image, my.X - KN_shotL[5].x, my.Y - KN_shotL[5].y);
                                else g.DrawImage(KN_shotL[Ani_time].image, my.X - KN_shotL[Ani_time].x, my.Y - KN_shotL[Ani_time].y);
                                break;
                            case "handgun":
                                if (hg_mag != 0) g.DrawImage(HG_shotL[Ani_time].image, my.X - HG_shotL[Ani_time].x, my.Y - HG_shotL[Ani_time].y);
                                else g.DrawImage(HG_shotL[2].image, my.X - HG_shotL[2].x, my.Y - HG_shotL[2].y);
                                break;
                            case "shotgun":
                                if (sg_mag != 0) g.DrawImage(SG_shotL[Ani_time].image, my.X - SG_shotL[Ani_time].x, my.Y - SG_shotL[Ani_time].y);
                                else g.DrawImage(SG_shotL[2].image, my.X - SG_shotL[2].x, my.Y - SG_shotL[2].y);
                                break;
                            case "carbine":
                                if (ar_mag != 0) g.DrawImage(AR_shotL[Ani_time].image, my.X - AR_shotL[Ani_time].x, my.Y - AR_shotL[Ani_time].y);
                                else g.DrawImage(AR_shotL[2].image, my.X - AR_shotL[2].x, my.Y - AR_shotL[2].y);
                                break;
                            case "sniper":
                                if (sr_mag != 0) g.DrawImage(SR_shotL[Ani_time].image, my.X - SR_shotL[Ani_time].x, my.Y - SR_shotL[Ani_time].y);
                                else g.DrawImage(SR_shotL[2].image, my.X - SR_shotL[2].x, my.Y - SR_shotL[2].y);
                                break;
                        }
                    }
                }
                else
                {
                    before_health = health;
                    if (damage_flag)
                    {
                        health_timer -= 1000;
                        damage_flag = false;
                    }

                }
                if (!Key.up && !Key.down && !Key.left && !Key.right)
                {
                    if(health_timer > 300)
                    {
                        health_timer = 0;
                        health ++;
                        if (health > 100) health = 100;
                    }
                }
                else
                {
                    if(health_timer > 800)
                    {
                        health_timer = 0;
                        health ++;
                        if (health > 100) health = 100;
                    }
                }
                health_timer++;
            }
            else
            {
                g.DrawImage(death, my.X, my.Y);
                fadeout = true;
            }

        }
        public void Tick(Point mouse, List<Enemy> enemies)
        {
            for (int i = Bullets_P.Count - 1; i >= 0; i--)
            {
                Bullets_P[i].Tick();
                //死んでいたら消す
                if (Bullets_P[i].dead) Bullets_P.RemoveAt(i);
            }
            center = new PointF(my.X + img.Width / 2, my.Y + img.Height / 2);
            if (health <= 0)
            {
                dead = true;
                health = 0;
            }

            if (before_weapon != Key.weapon)
            {
                switch (Key.weapon)
                {
                    case "knife":
                        Change(KN_changeL);
                        break;
                    case "handgun":
                        Change(HG_changeL);
                        break;
                    case "shotgun":
                        Change(SG_changeL);
                        break;
                    case "carbine":
                        Change(AR_changeL);
                        break;
                    case "sniper":
                        Change(SR_changeL);
                        break;
                }

            }
            if(Key.reload) Reload();
            Item();
            Enter();
            Rotate(mouse);
            move.Pmove(speed, center, ref my, All_Rect[now_stage], img.Width / 2, Key.up, Key.down, Key.left, Key.right);
            if (shot_now)
            {
                switch (Key.weapon)
                {
                    case "knife":
                        Shooter(cut, KN_shotL, dummy, ref dummy);
                        break;
                    case "handgun":
                        Shooter(HG_shot, HG_shotL, hg_b, ref hg_mag);
                        break;
                    case "shotgun":
                        Shooter(SG_shot, SG_shotL, sg_b, ref sg_mag);
                        break;
                    case "carbine":
                        Shooter(HG_shot, AR_shotL, ar_b, ref ar_mag);
                        break;
                    case "sniper":
                        Shooter(SR_shot, SR_shotL, sr_b, ref sr_mag);
                        break;
                }
            }
            if ((!fadeout) && (!fadein) && (!reload_now)&& (!change_now)&& (!shot_now)) Shot(enemies);
            time++;
        }
        private void Change(List<position> interval)
        {
            if (!change_now)
            {
                HG_set.Play();
                change_now = true;
                time = 0;
                change_time = 0;
            }
            if (time % 2 == 0) change_time++;
            if (change_time > interval.Count - 1)
            {
                change_time = 0;
                time = 0;
                change_now = false;
                before_weapon = Key.weapon;
            }
        }
        private void Reload()
        {
            switch (Key.weapon)
            {
                case "handgun":
                    Reloader(MAG_release,MAG_set,hg_reload_interval,ref hg_b,ref hg_mag,mag_hg_b);
                    break;

                case "shotgun":
                    Reloader(MAG_release, MAG_set, sg_reload_interval, ref sg_b, ref sg_mag, mag_sg_b);
                    break;

                case "carbine":
                    Reloader(MAG_release, AR_release, ar_reload_interval, ref ar_b, ref ar_mag, mag_ar_b);
                    break;

                case "sniper":
                    Reloader(SR_reload, SR_set, sr_reload_interval, ref sr_b, ref sr_mag, mag_sr_b);
                    break;
            }
        }
        private void Reloader(SoundPlayer release,SoundPlayer set,int interval,ref int All_b,ref int mag_b,int max_b)
        {
            if (!reload_now)
            {
                release.Play();
                reload_now = true;
                time = 0;
                Ani_time = 0;
            }
            if (time % 2 == 0) Ani_time++;
            if ((Key.weapon == "carbine")&&(Ani_time == interval / 2)) MAG_set.Play();
            if (Ani_time > interval)
            {
                if (All_b >= max_b - mag_b)
                {
                    All_b -= max_b - mag_b;
                    mag_b = max_b;
                }
                else
                {
                    mag_b = All_b + mag_b;
                    All_b = 0;
                }
                set.Play();
                Ani_time = 0;
                time = 0;
                reload_now = false;
                Key.reload = false;
            }
        }
        private void Shooter(SoundPlayer shot, List<position> interval,int All_b,ref int mag_b)
        {
            if (!shot_now)
            {
                if ((All_b == 0) && (mag_b == 0)) Out.Play();
                else if (mag_b == 0) Out.Play();
                else
                {
                    shot.Play();
                    if (Key.weapon != "knife") mag_b--;
                }
                time = 0;
                shot_now = true;
                Ani_time = 0;
            }
            if (time % 2 == 0) Ani_time++;
            if (Ani_time > interval.Count - 1)
            {
                if((Key.weapon == "shotgun")&&(mag_b != 0))SG_pomp.Play();
                Ani_time = 0;
                time = 0;
                shot_now = false;
                Key.reload = false;
            }

        }
        private void Shot(List<Enemy> Enemies)
        {
            if (Key.wait <= 0)
            {
                if (Key.click == true)
                {
                    switch (Key.weapon)
                    {
                        case "knife":
                            Key.wait += 50;
                            Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - s_b.Width / 2, center.Y - s_b.Width / 2, angle - 90, s_b, Enemies));
                            Shooter(cut, KN_shotL, dummy, ref dummy);
                            Key.click = false;
                            break;

                        case "handgun":
                            Key.wait += 5;
                            if (hg_mag != 0) Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - s_b.Width / 2, center.Y - s_b.Width / 2, angle - 90, s_b, Enemies));
                            Shooter(HG_shot, HG_shotL, hg_b, ref hg_mag);
                            Key.click = false;
                            break;

                        case "shotgun":
                            Key.wait += 30;
                            if (sg_mag != 0)
                            {
                                Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - s_b.Width / 2, center.Y - s_b.Width / 2, angle + 5 - 90, s_b, Enemies));
                                Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - s_b.Width / 2, center.Y - s_b.Width / 2, angle - 90, s_b, Enemies));
                                Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - s_b.Width / 2, center.Y - s_b.Width / 2, angle - 5 - 90, s_b, Enemies));
                            }
                            Shooter(SG_shot, SG_shotL, sg_b, ref sg_mag);
                            Key.click = false;
                            break;

                        case "carbine":
                            Key.wait += 10;
                            if (ar_mag != 0) Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - m_b.Width / 2, center.Y - m_b.Width / 2, angle - 90, m_b, Enemies));
                            Shooter(HG_shot, AR_shotL, ar_b, ref ar_mag);
                            break;

                        case "sniper":
                            Key.wait += 30;
                            if (sr_mag != 0) Bullets_P.Add(shot = new clsBullet_P(Key.weapon, center.X - m_b.Width / 2, center.Y - m_b.Width / 2, angle - 90, m_b, Enemies));
                            Shooter(SR_shot, SR_shotL, sr_b, ref sr_mag);
                            Key.click = false;
                            break;
                    }
                }
            }
            else Key.wait--;
        }
        private void Rotate(Point m)
        {
            //旋回角度を計算
            //偏角を計算     Math.Atan2(y, x) * 180 / Math.PI
            angle = -(float)(Math.Atan2(center.X - m.X, center.Y - m.Y) * 180 / Math.PI);
        }
        private void Enter()
        {
            if (!fadeout)
            {
                for (int i = 0; i < Door_In.Count; i++)
                {
                    //今のステージにあるものなら
                    if ((Door_In[i].stage == now_stage) && (stage_enemy == 0))
                    {
                        //ぶつかったら
                        if (((Door_In[i].rect.X - img.Width / 2 < center.X) && (Door_In[i].rect.Right + img.Width / 2 > center.X)) &&
                           ((Door_In[i].rect.Y - img.Width / 2 < center.Y) && (Door_In[i].rect.Bottom + img.Width / 2 > center.Y)))
                        {
                            fadeout = true;
                            Bullets_P = new List<clsBullet_P>();
                        }
                    }
                }
            }
        }
        private void Item()
        {
            //All_bを後ろから走査
            for (int i = All_Bullet.Count - 1; i >= 0; i--)
            {
                //今のステージにあるものなら
                if (All_Bullet[i].stage == now_stage)
                {
                    //ぶつかったら
                    if (ItemCollision(All_Bullet[i].x, All_Bullet[i].y, center.X, center.Y, img.Width / 2, All_Bullet[i].img.Width))
                    {
                        switch (All_Bullet[i].type)
                        {
                            case "hg_b":
                                if (hg_b + All_Bullet[i].amount > max_hg_b) hg_b = max_hg_b;
                                else hg_b += All_Bullet[i].amount;
                                break;
                            case "ar_b":
                                if (ar_b + All_Bullet[i].amount > max_ar_b) ar_b = max_ar_b;
                                else ar_b += All_Bullet[i].amount;
                                break;
                            case "sg_b":
                                if (sg_b + All_Bullet[i].amount > max_sg_b) sg_b = max_sg_b;
                                else sg_b += All_Bullet[i].amount;
                                break;
                            case "sr_b":
                                if (sr_b + All_Bullet[i].amount > max_sr_b) sr_b = max_sr_b;
                                else sr_b += All_Bullet[i].amount;
                                break;
                        }
                        All_Bullet.RemoveAt(i);
                        HG_set.Play();
                    }
                }
            }
            //All_hを後ろから走査
            for (int i = All_heal.Count - 1; i >= 0; i--)
            {
                //今のステージにあるものなら
                if (All_heal[i].stage == now_stage)
                {
                    //ぶつかったら
                    if (ItemCollision(All_heal[i].x, All_heal[i].y, center.X, center.Y, img.Width / 2, All_heal[i].img.Width))
                    {
                        if (All_heal[i].amount + health > max_health) health = max_health;
                        else health += All_heal[i].amount;
                        All_heal.RemoveAt(i);
                        HG_set.Play();
                    }
                }
            }
            //All_wを後ろから走査
            for (int i = All_weapon.Count - 1; i >= 0; i--)
            {
                //今のステージにあるものなら
                if (All_weapon[i].stage == now_stage)
                {
                    //ぶつかったら
                    if (ItemCollision(All_weapon[i].x, All_weapon[i].y, center.X, center.Y, img.Width / 2, All_weapon[i].img.Width))
                    {
                        switch (All_weapon[i].type)
                        {
                            case "hg":
                                Key.hg = true;
                                break;
                            case "ar":
                                Key.ar = true;
                                break;
                            case "sg":
                                Key.sg = true;
                                break;
                            case "sr":
                                Key.sr = true;
                                break;
                        }
                        All_weapon.RemoveAt(i);
                        HG_set.Play();
                    }
                }
            }
        }
        private bool ItemCollision(int obj_x, int obj_y, float x, float y, int radius, int width)
        {
            //大雑把に判定
            if (((obj_x - radius < x) && (obj_x + width + radius > x)) &&
                    ((obj_y - radius < y) && (obj_y + width + radius > y))) return true;
            return false;
        }
    }
}
