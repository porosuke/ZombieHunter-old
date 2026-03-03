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

namespace shooting
{
    internal class clsBullet_E
    {
        //private Bitmap imgbullet, imgknife;
        private string type;
        private int damage;
        private float x, y, dir, v, start_x, start_y;
        private Image img;
        private Player player;
        private PointF center;
        public bool dead, first = true;



        public clsBullet_E(string startType, float startX, float startY, float startDir, Image startImg, Player p)
        {
            type = startType;
            img = startImg;
            x = startX;
            y = startY;
            start_x = startX;
            start_y = startY;
            dir = startDir;
            player = p;
            v = 10;
            damage = 10;
            if(type == "claw") damage = 7;


            //imgbullet = zombieyamasaki.Properties.Resources.nasu;
            //imgknife = zombieyamasaki.Properties.Resources.tama;
        }

        public void Tick()
        {
            if (!dead)
            {
                for (int i = 1; i <= v; i++)
                {
                    center = new PointF(x + img.Width / 2, y + img.Height / 2);
                    x += (float)(Math.Cos(Math.PI / 180 * dir));
                    y += (float)(Math.Sin(Math.PI / 180 * dir));
                    if (type == "claw") DamageCheck(center, 40);
                    else DamageCheck(center, img.Width / 2);
                    CollisionCheck(center, img.Width / 2);
                }
            }

        }

        public void draw(Graphics gr)
        {
            if ((!dead) && (type != "claw"))
            {
                gr.ResetTransform();
                gr.TranslateTransform(-x + -img.Width / 2, -y + -img.Height / 2);
                gr.RotateTransform((float)dir, MatrixOrder.Append);
                gr.TranslateTransform(x + img.Width / 2, y + img.Height / 2, MatrixOrder.Append);
                gr.DrawImage(img, x, y);
            }

        }
        private void DamageCheck(PointF c, int r)
        {
            //射程外なら
            if (type == "claw") if (Math.Sqrt(Math.Pow(player.center.X - start_x, 2) + Math.Pow(player.center.Y - start_y, 2)) > 80) dead = true;

            //プレイヤーに当たったら
            if (Math.Sqrt(Math.Pow(player.center.X - c.X, 2) + Math.Pow(player.center.Y - c.Y, 2)) <= (r + player.img.Width / 2))
            {
                if (first)
                {
                    player.health -= damage;
                    first = false;
                    player.damage_flag = true;
                }
                dead = true;
            }
        }
        private void CollisionCheck(PointF c, int r)
        {
            //ぶつかったら死ぬ
            foreach (Rectangle rect in Main.All_Rect[Main.now_stage])
            {
                if (((rect.X - r < c.X) && (rect.Right + r > c.X)) &&
                        ((rect.Y - r < c.Y) && (rect.Bottom + r > c.Y)))
                    dead = true;
            }
        }
    }
}
