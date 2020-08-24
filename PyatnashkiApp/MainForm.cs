using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyatnashkiApp
{
    public partial class MainForm : Form
    {
        //Точка отсчета времени
        private DateTime startTime;
        //Адресс пустой ячейки
        private TableLayoutPanelCellPosition empty;
        public MainForm()
        {
            InitializeComponent();

            markUp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            InitKeys();

            resetButton.Click += (send, e) =>
            {
                timer1.Stop();
                timesCounter.Text = "";
                movesCounter.Text = "";
                markUp.Controls.Clear();
                InitKeys();
            };

            resetButton.PerformClick();

            timer1.Tick += (send, e) =>
                timesCounter.Text = new DateTime(DateTime.Now.Ticks - startTime.Ticks).ToString("mm:ss");
        }

        private void InitKeys()
        {
            Button[] buttons = new Button[16];
            buttons.ToList().ForEach(btn =>
            {
                btn = new Button();
                btn.Dock = DockStyle.Fill;
                btn.BackColor = Color.Silver;
                markUp.Controls.Add(btn);
            });

            Random r = new Random(DateTime.Now.Millisecond);
            int text = 1;
            List<Button> btns = markUp.Controls.Cast<Button>().ToList();
            //Псевдосортировка перед назначением текста
            btns.Sort((a, b) => r.Next(-1, 1));
            btns.Sort((a, b) => r.Next(-1, 1));
            btns.Sort((a, b) => r.Next(-1, 1));
            btns.ForEach(btn => btn.Text = text++.ToString());
            //Текст со значением 16 заменяем на пустой
            btns.Last().Text = "";
            
            //Устанавливаем адресс пустой ячейки и подписуем на ивент
            empty = markUp.GetPositionFromControl(btns.Last());
            SubscribeNearestButtons();

            //Второй вариант сортировки не работает при инициализации
            //for (int i = 0; i < 139; i++)
            //    //markUp.Controls.Cast<Button>().Where(btn => checkPosition(btn)).Take(new Random().Next(1, 4)).Last().PerformClick();
            //    btns.Where(btn => checkPosition(btn))
            //        .Take(new Random().Next(1, 4))
            //        .Last().PerformClick();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            //Получаем кнопки для замены значений
            Button current = (Button)sender;
            //Button next = markUp.Controls.Cast<Button>().First(btn => btn.Text == "");
            Button next = markUp.GetControlFromPosition(empty.Column, empty.Row) as Button;

            //Обмен значениями
            next.Text = current.Text;
            current.Text = "";

            //Устанавливаем адресс пустой ячейки и подписуем на ивент
            empty = markUp.GetPositionFromControl(current);
            SubscribeNearestButtons();

            //Обновляем счетчики
            updateCounters();
        }
        
        void SubscribeNearestButtons()
        {
            List<Button> buttons = markUp.Controls.Cast<Button>().ToList();
            //Обновляем подписки 
            buttons.ForEach(btn =>
            {
                if (checkPosition(btn))
                    btn.Click += Btn_Click;
                else
                    btn.Click -= Btn_Click;
            });
        }
        
 
        bool checkPosition(Button button)
        {
            TableLayoutPanelCellPosition pos = markUp.GetPositionFromControl(button);
            //Проверяем соседство по рядам
            bool isRowNeighbor =
                pos.Column == empty.Column && Math.Abs(empty.Row - pos.Row) == 1;
            //По колонкам
            bool isColumnNeighbor =
                pos.Row == empty.Row && Math.Abs(empty.Column - pos.Column) == 1;

            return isColumnNeighbor || isRowNeighbor;
        }

        
        void updateCounters()
        {
            if (movesCounter.Text == "")
            { 
                movesCounter.Text = "1";
                timerUpdate();
            }
            else
            {
                int count = Int32.Parse(movesCounter.Text);
                movesCounter.Text = (++count).ToString();
            }
        }
        void timerUpdate()
        {
            startTime = DateTime.Now;
            timer1.Start();
        }
    }
}
