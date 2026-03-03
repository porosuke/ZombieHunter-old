using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using static shooting.Main;
using System.Media;
using System.IO;

namespace shooting
{
    internal class Enemy
    {
        //リスト
        private List<Point> Points = new List<Point>();
        private List<clsBullet_E> Bullets_E = new List<clsBullet_E>();
        private clsBullet_E shot;

        private int[,] Table;
        //インスタンス作成
        private Move move = new Move();
        //座標
        public PointF my, center;
        //速度
        public int speed;
        //回転角度
        public float angle;
        //死んだか
        public bool dead;
        //画像
        public Image img,bullet;
        //体力
        public int health, before_health;
        //最短距離計算用
        private double shortest_my, shortest_goal, shortest_my_sub, shortest_goal_sub;
        //最短の表添え字を入れる
        private int shortest_my_point, shortest_goal_point;
        //行先のポイント
        private PointF target;
        //タイプ
        private bool shooter = false;
        private bool boss = false;
        //発射間隔
        private const int shot_interval = 50;
        private const int claw_interval = 30;
        //ダメージ
        private const int damage = 20;
        //発射タイマー
        private int time = 0;
        //アニメーションタイマー
        private int Ani_time = 0;
        //音
        SoundPlayer claw, damage_s;
        Stream strm;
        Random r = new Random();
        List<SoundPlayer> zombie_sound = new List<SoundPlayer>();
        //Maptip
        private Image Enemy_img;
        //scratch List
        private List<position> Scratch = new List<position>();
        public struct position
        {
            public Image image;
            public float x;
            public float y;
        }


        public Enemy(float x, float y, int s, float a, int h, String E_type, List<Point> All_p, int[,] All_t)
        {
            my = new PointF(x, y);
            speed = s;
            angle = a;
            health = h;
            before_health = h;
            dead = false;

            Points = All_p;
            Table = All_t;
            if (E_type == "gun") shooter = true;
            if(E_type == "boss")
            {
                shooter = true;
                boss = true;
            }
            if (shooter)
            {
                Enemy_img = Properties.Resources.zombie2;
                bullet = Division(32, 32, 937, 0, 1f);
            }
            else
            {
                Enemy_img = Properties.Resources.zombie1;
                bullet = Division(32, 32, 937, 0, 1f);
            }
            img = Division(101, 101, 0, 1, 0.5f);
            Set(Division(102, 119, 102, 0, 0.5f), 0, 18 / 2);
            Set(Division(101, 115, 205, 0, 0.5f), 0, 14 / 2);
            Set(Division(137, 109, 310, 0, 0.5f), 15, 9 / 2);
            Set(Division(167, 103, 450, 0, 0.5f), 31, 3 / 2);
            Set(Division(167, 103, 450, 0, 0.5f), 31, 3 / 2);
            Set(Division(137, 109, 310, 0, 0.5f), 15, 9 / 2);
            Set(Division(101, 115, 205, 0, 0.5f), 0, 14 / 2);
            Set(Division(102, 119, 102, 0, 0.5f), 0, 18 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            Set(Division(100, 177, 620, 0, 0.5f), 0, 75 / 2);
            //Set(Division(102, 122, 722, 55, 1f), 0, 75);
            //Set(Division(102, 122, 822, 55, 1f), 0, 75);

            strm = Properties.Resources.claw;
            claw = new SoundPlayer(strm);
            strm = Properties.Resources.damage;
            damage_s = new SoundPlayer(strm);
            strm = Properties.Resources.zombie3;
            zombie_sound.Add(new SoundPlayer(strm));
            strm = Properties.Resources.zombie4;
            zombie_sound.Add(new SoundPlayer(strm));
            strm = Properties.Resources.zombie5;
            zombie_sound.Add(new SoundPlayer(strm));
            strm = Properties.Resources.zombie6;
            zombie_sound.Add(new SoundPlayer(strm));
            strm = Properties.Resources.zombie7;
            zombie_sound.Add(new SoundPlayer(strm));
        }
        private void Set(Image img,float gapx,float gapy)
        {
            position p = new position()
            {
                image = img,
                x = gapx,
                y = gapy
            };
            Scratch.Add(p);
        }
        private Bitmap Division(int width, int height, int x, int y, float scale)
        {//画像切り出し
            Rectangle sourceRectange = new Rectangle(new Point(x, y), new Size(width, height));
            Bitmap bitmap1 = new Bitmap((int)(width * scale), (int)(height * scale));
            Graphics graphics = Graphics.FromImage(bitmap1);
            graphics.DrawImage(Enemy_img, new Rectangle(0, 0, (int)(width * scale), (int)(height * scale)), sourceRectange, GraphicsUnit.Pixel);
            graphics.Dispose();
            return bitmap1;
        }
        public void Draw(Graphics g)
        {
            foreach (clsBullet_E bullet_E in Bullets_E)
            {
                bullet_E.draw(g);
            }
            g.ResetTransform();
            g.TranslateTransform(-center.X, -center.Y);
            g.RotateTransform((float)angle, MatrixOrder.Append);
            g.TranslateTransform(center.X, center.Y, MatrixOrder.Append);
            
            if (!dead)
            {
                if (before_health == health) g.DrawImage(Scratch[Ani_time].image, my.X - Scratch[Ani_time].x, my.Y - Scratch[Ani_time].y);
                before_health = health;
            }
        }
        public void Tick(PointF Player, Player p)
        {
            center = new PointF(my.X + img.Width / 2, my.Y + img.Height / 2);
            DistanceCheck(Player, p);
            Route(Player, center);
            if (Ani_time == 0) Rotate(target);
            if (Ani_time == 0) move.Emove(speed, target, center, ref my);
            if (health <= 0) dead = true;
            time++;
            if (r.Next(200) == 1) zombie_sound[r.Next(zombie_sound.Count)].Play();
        }
        private void Claw(Player p)
        {
            if (time > claw_interval)
            {
                if (time % 2 == 0) Ani_time++;
                //ひっかく瞬間にプレイヤーに当たったら
                if (Ani_time == 7)
                {
                    claw.Play();
                    Bullets_E.Add(shot = new clsBullet_E("claw", center.X - bullet.Width / 2, center.Y - bullet.Height / 2, angle - 90, bullet, p));
                }
                if (Ani_time > Scratch.Count - 1)
                {
                    Ani_time = 0;
                    time = 0;
                }
            }
        }
        private void DistanceCheck(PointF Player, Player p)
        {
            if (shooter)
            {
                //近距離なら
                if (Distance(center, Player) < 100) Claw(p);
                //距離が一定以下なら
                else
                {
                    Ani_time = 0;
                    //視線が通っていたら(障害物に当たっていなかったら)
                    if (!CheckCollisionRect(center, Player))
                    {
                        //弾を撃つ
                        if (time > shot_interval)
                        {
                            if (boss)
                            {
                                for(int i = 0;i < 360; i += 36)
                                {
                                    Bullets_E.Add(shot = new clsBullet_E("shot", center.X - bullet.Width / 2, center.Y - bullet.Height / 2, angle - 90 + i, bullet, p));
                                }
                            }
                           else Bullets_E.Add(shot = new clsBullet_E("shot", center.X - bullet.Width / 2, center.Y - bullet.Height / 2, angle - 90, bullet, p));
                            time = 0;
                        }
                    }
                }
            }
            else
            {
                //近距離なら
                if (Distance(center, Player) < 100) Claw(p);
                else Ani_time = 0;
            }
            for (int i = Bullets_E.Count - 1; i >= 0; i--)
            {
                Bullets_E[i].Tick();
                //死んでいたら消す
                if (Bullets_E[i].dead) Bullets_E.RemoveAt(i);
            }
        }
        private bool CheckCollisionRect(PointF c, PointF p)
        {
            Rectangle[] rect = All_Rect_ex[now_stage];
            PointF Start, End;
            List<Point> points;
            Start = c;
            End = p;

            //対象ステージの障害物を走査する
            for (int b = 0; b < rect.Length; b++)
            {
                //壁には絶対当たらない
                if (b > 3)
                {
                    points = new List<Point>();
                    //左上
                    points.Add(new Point(rect[b].X, rect[b].Y));
                    //右上
                    points.Add(new Point(rect[b].X + rect[b].Width, rect[b].Y));
                    //右下
                    points.Add(new Point(rect[b].X + rect[b].Width, rect[b].Y + rect[b].Height));
                    //左下
                    points.Add(new Point(rect[b].X, rect[b].Y + rect[b].Height));
                    //左上
                    points.Add(new Point(rect[b].X, rect[b].Y));

                    if (coli(Start, End, points[0], points[1])) return true;
                    else if (coli(Start, End, points[1], points[2])) return true;
                    else if (coli(Start, End, points[2], points[3])) return true;
                    else if (coli(Start, End, points[3], points[0])) return true;
                }
            }
            //全てに当たらなかったなら
            return false;
        }
        private bool coli(PointF a, PointF b, PointF c, PointF d)
        {
            double s, t;
            s = (a.X - b.X) * (c.Y - a.Y) - (a.Y - b.Y) * (c.X - a.X);
            t = (a.X - b.X) * (d.Y - a.Y) - (a.Y - b.Y) * (d.X - a.X);
            if (s * t > 0) return false;

            s = (c.X - d.X) * (a.Y - c.Y) - (c.Y - d.Y) * (a.X - c.X);
            t = (c.X - d.X) * (b.Y - c.Y) - (c.Y - d.Y) * (b.X - c.X);
            if (s * t > 0) return false;
            return true;
        }
        private void Rotate(PointF p)
        {
            //旋回角度を計算
            //偏角を計算     Math.Atan2(y, x) * 180 / Math.PI
            angle = -(float)(Math.Atan2(center.X - p.X, center.Y - p.Y) * 180 / Math.PI);
        }
        public void Route(PointF p, PointF c)
        {
            shortest_my = 100000000;
            shortest_goal = 100000000;
            for (int i = 0; i < Points.Count; i++)
            {
                //より短いのが見つかったら
                shortest_my_sub = Distance(c, Points[i]);
                if (shortest_my_sub < shortest_my)
                {
                    shortest_my = shortest_my_sub;
                    shortest_my_point = i;
                }
                //より短いのが見つかったら
                shortest_goal_sub = Distance(p, Points[i]);
                if (shortest_goal_sub < shortest_goal)
                {
                    shortest_goal = shortest_goal_sub;
                    shortest_goal_point = i;
                }
            }
            if (shortest_my <= img.Width / 2)
            {
                if (shortest_my_point == shortest_goal_point) target = p;
                else target = Points[Table[shortest_my_point, shortest_goal_point]];
            }

        }
        private double Distance(PointF S, PointF E)
        {
            return Math.Sqrt(Math.Pow((S.X - E.X), 2) + Math.Pow((S.Y - E.Y), 2));
        }
    }
}
