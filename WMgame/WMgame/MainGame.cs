using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMgame
{

    public partial class MainGame : Form
    {
        private Image[] fruitImages = new Image[]
        {
            Properties.Resources._1, // 체리
            Properties.Resources._2, // 딸기
            Properties.Resources._3, // 포도
            Properties.Resources._4, // 포도
            Properties.Resources._5, // 포도
            Properties.Resources._6, // 포도
            Properties.Resources._7, // 포도
            Properties.Resources._8, // 포도
            Properties.Resources._9, // 포도
            Properties.Resources._10 // 포도
            // ... 단계 늘려가며 추가
        };
        public class Fruit
        {
            public PointF Position;
            public PointF Velocity;
            public int Level;      // 과일 단계 (0부터 시작)
            public float Radius;
            public Image Image;
            public bool Merged = false; // 이번 프레임에 합쳐졌는지 체크용

            public Fruit(PointF pos, int level, Image img, float radius)
            {
                Position = pos;
                Level = level;
                Image = img;
                Radius = radius;
                Velocity = PointF.Empty;
            }

            public RectangleF Bounds =>
                new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }
        private List<Fruit> fruits = new List<Fruit>();
        private System.Windows.Forms.Timer gameTimer;
        private const float Gravity = 0.9f;
        // private const float GroundY = 740f; // 바닥 y좌표
        private float GroundY => this.ClientSize.Height * 0.9779f;
        private bool isGameOver = false;
        private float dangerTimer = 0f;
        private const float DangerTimeLimit = 60f; // 약 1.5초 유지되면 게임오버
        private float DangerLineY => this.ClientSize.Height * 0.22f;
        private int nextFruitLevel = 0;
        private int score = 0;
        private int bestScore = 0;
        private Random rand = new Random();

        public MainGame()
        {

            InitializeComponent();
            this.DoubleBuffered = true;


            timer1.Start();

        }
        private void MainGame_MouseClick_1(object sender, MouseEventArgs e)
        {

            if (isGameOver)
            {
                RestartGame();
                return;
            }// ← 추가
            int level = nextFruitLevel;
            float radius = 30 + level * 8; // 단계별 크기
            var fruit = new Fruit(new PointF(e.X, 50), level, fruitImages[level], radius);
            fruits.Add(fruit);
            nextFruitLevel = rand.Next(0,5); // 다음 낙하 과일은 낮은 단계 중 랜덤
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isGameOver) return; // ← 추가
            UpdatePhysics();
            CheckCollisions();
            CheckGameOver(); // ← 추가
            this.Invalidate(); // 다시 그리기 요청
        }

        private void UpdatePhysics()
        {
            foreach (var f in fruits)
            {
                f.Velocity.Y += Gravity;
                f.Position = new PointF(f.Position.X + f.Velocity.X, f.Position.Y + f.Velocity.Y);
            }

            for (int iter = 0; iter < 4; iter++)
            {
                for (int i = 0; i < fruits.Count; i++)
                {
                    for (int j = i + 1; j < fruits.Count; j++)
                    {
                        var a = fruits[i];
                        var b = fruits[j];
                        float dx = b.Position.X - a.Position.X;
                        float dy = b.Position.Y - a.Position.Y;
                        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                        float minDist = a.Radius + b.Radius;

                        if (dist < minDist)
                        {
                            if (dist < 0.01f) { dx = 0.01f; dist = 0.01f; }

                            float overlap = minDist - dist;
                            float nx = dx / dist, ny = dy / dist;
                            float pushFactor = 0.7f; // 힘 세기, 0.3~0.6 사이로 조정 가능

                            a.Position.X -= nx * overlap / 2 * pushFactor;
                            a.Position.Y -= ny * overlap / 2 * pushFactor;
                            b.Position.X += nx * overlap / 2 * pushFactor;
                            b.Position.Y += ny * overlap / 2 * pushFactor;

                            // 밀린 만큼 속도도 감쇠시켜서 계속 흔들리는 걸 막음
                            a.Velocity.X *= 0.8f;
                            a.Velocity.Y *= 0.8f;
                            b.Velocity.X *= 0.8f;
                            b.Velocity.Y *= 0.8f;
                        }
                    }
                }

                foreach (var f in fruits)
                {
                    ClampToBounds(f);
                }
            }
        }

        private void ClampToBounds(Fruit f)
        {
            if (f.Position.Y + f.Radius > GroundY)
            {
                f.Position.Y = GroundY - f.Radius;
                f.Velocity.Y = 0;
            }

            if (f.Position.Y - f.Radius < 0)
            {
                f.Position.Y = f.Radius;
                f.Velocity.Y = 0;
            }

            if (f.Position.X - f.Radius < 0)
            {
                f.Position.X = f.Radius;
                f.Velocity.X = 0;
            }
            if (f.Position.X + f.Radius > this.ClientSize.Width)
            {
                f.Position.X = this.ClientSize.Width - f.Radius;
                f.Velocity.X = 0;
            }
        }
        private void CheckCollisions()
        {
            var toAdd = new List<Fruit>();
            var toRemove = new List<Fruit>();

            for (int i = 0; i < fruits.Count; i++)
            {
                for (int j = i + 1; j < fruits.Count; j++)
                {
                    var a = fruits[i];
                    var b = fruits[j];
                    if (a.Merged || b.Merged) continue;
                    if (a.Level != b.Level) continue;

                    float dx = b.Position.X - a.Position.X;
                    float dy = b.Position.Y - a.Position.Y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (dist < a.Radius + b.Radius)
                    {
                        // 합체!
                        a.Merged = true;
                        b.Merged = true;
                        toRemove.Add(a);
                        toRemove.Add(b);

                        if (a.Level + 1 < fruitImages.Length)
                        {
                            var mid = new PointF((a.Position.X + b.Position.X) / 2,
                                                  (a.Position.Y + b.Position.Y) / 2);
                            int newLevel = a.Level + 1;
                            float newRadius = 30 + newLevel * 8;
                            toAdd.Add(new Fruit(mid, newLevel, fruitImages[newLevel], newRadius));

                            score += (newLevel + 1) * 10;
                        }
                    }
                }
            }

            foreach (var f in toRemove) fruits.Remove(f);
            fruits.AddRange(toAdd);
        }

        private void MainGame_Paint_1(object sender, PaintEventArgs e)
        {
            foreach (var f in fruits)
            {
                e.Graphics.DrawImage(f.Image, f.Bounds.X, f.Bounds.Y, f.Bounds.Width, f.Bounds.Height);
            }

            DrawScoreBoard(e.Graphics);

            if (isGameOver)
            {
                DrawGameOverOverlay(e.Graphics);
            }
        }
        private void DrawScoreBoard(Graphics g)
        {
            string scoreText = $"점수: {score}";
            using (Font font = new Font("맑은 고딕", 18, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (Brush shadowBrush = new SolidBrush(Color.Black))
            {
                // 그림자 효과로 가독성 높이기 (나무 배경 위에서도 잘 보이게)
                g.DrawString(scoreText, font, shadowBrush, 22, 17);
                g.DrawString(scoreText, font, textBrush, 20, 15);
            }
        }
        private void DrawGameOverOverlay(Graphics g)
        {
            // 반투명 검은 배경
            using (Brush overlayBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
            {
                g.FillRectangle(overlayBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }

            // "GAME OVER" 텍스트
            string text = "GAME OVER";
            using (Font font = new Font("맑은 고딕", 36, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                SizeF textSize = g.MeasureString(text, font);
                float x = (this.ClientSize.Width - textSize.Width) / 2;
                float y = (this.ClientSize.Height - textSize.Height) / 2 - 30;
                g.DrawString(text, font, textBrush, x, y);
            }
            string scoreText = $"점수: {score}";
            using (Font scoreFont = new Font("맑은 고딕", 20))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                SizeF size = g.MeasureString(scoreText, scoreFont);
                float x = (this.ClientSize.Width - size.Width) / 2;
                float y = (this.ClientSize.Height - size.Height) / 2+ 80;
                g.DrawString(scoreText, scoreFont, textBrush, x, y);
            }

            // ↓ 최고 점수 표시 추가
            string bestText = $"최고 점수: {bestScore}";
            using (Font bestFont = new Font("맑은 고딕", 16))
            using (Brush textBrush = new SolidBrush(Color.Yellow))
            {
                SizeF size = g.MeasureString(bestText, bestFont);
                float x = (this.ClientSize.Width - size.Width) / 2;
                float y = (this.ClientSize.Height - size.Height) / 2 + 130;
                g.DrawString(bestText, bestFont, textBrush, x, y);
            }
            // 재시작 안내 문구
            string subText = "클릭하면 다시 시작합니다";
            using (Font subFont = new Font("맑은 고딕", 14))
            using (Brush subBrush = new SolidBrush(Color.White))
            {
                SizeF subSize = g.MeasureString(subText, subFont);
                float x = (this.ClientSize.Width - subSize.Width) / 2;
                float y = (this.ClientSize.Height - subSize.Height) / 2 + 30;
                g.DrawString(subText, subFont, subBrush, x, y);
            }
        }
        private bool IsSettled(Fruit f)
        {
            return Math.Abs(f.Velocity.X) < 0.6f && Math.Abs(f.Velocity.Y) < 0.6f;
        }
        private void CheckGameOver()
        {
            bool anyInDanger = fruits.Any(f => f.Position.Y - f.Radius < DangerLineY && IsSettled(f));

            if (anyInDanger)
            {
                dangerTimer++;
                if (dangerTimer > DangerTimeLimit)
                {
                    isGameOver = true;
                    timer1.Stop();
                    if (score > bestScore) // ← 추가
                    {
                        bestScore = score;
                    }
                    //MessageBox.Show("게임 오버!", "Suika Game");
                }
            }
            else
            {
                dangerTimer = 0f;
            }
        }
        private void RestartGame()
        {
            fruits.Clear();
            isGameOver = false;
            dangerTimer = 0f;
            nextFruitLevel = 0;
            score = 0;
            timer1.Start();
        }


    }

}
