using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CronJobber
{
    public partial class Form1 : Form
    {
        private bool running = false;

        private System.Timers.Timer timer;
        private System.Timers.Timer second_timer;

        private int timer_delay;

        public Form1()
        {
            InitializeComponent();
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;

            second_timer = new System.Timers.Timer();
            second_timer.AutoReset = true;
            second_timer.Elapsed += Second_timer_Elapsed; ;
        }

        private void Second_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var x = Regex.Replace(time_remaining.Text, @"[^\d]", "");
            int remaining = int.Parse( x );
            remaining -= 1;
            time_remaining.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                time_remaining.Text = remaining.ToString() + "s";
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            running = !running;

            button1.Text = (running ? "PARAR" : "INICIAR");
            if (running) { StartTimer(); } else { StopTimer(); };
        }

        private void StartTimer()
        {
            timer.Enabled = true;
            timer.Interval = int.Parse(numericUpDown1.Value.ToString()) * 1000;
            timer.Start();

            second_timer.Enabled = true;
            second_timer.Interval = 1000;
            second_timer.Start();

            time_remaining.Text = int.Parse(numericUpDown1.Value.ToString()) + "s";
            timer_delay = int.Parse(numericUpDown1.Value.ToString());
            Console.WriteLine("Ticking timer every: " + timer.Interval);

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Timer ticked");
            using (HttpClient client = new HttpClient()) 
            {
                _ = client.GetAsync(textBox1.Text).Result;
            }
            time_remaining.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                time_remaining.Text = timer_delay + "s";
            });
        }

        private void StopTimer() 
        {
            timer.Enabled = false;
            timer.Stop();

            second_timer.Enabled = false;
            second_timer.Stop();

            time_remaining.Text = "0s";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            timer.Stop();

            second_timer.Enabled = false;
            second_timer.Stop();

            time_remaining.Text = "0s";

            Timer_Elapsed(null, null);

            timer.Enabled = true;
            timer.Interval = timer_delay * 1000;
            timer.Start();

            second_timer.Enabled = true;
            second_timer.Interval = 1000;
            second_timer.Start();

            time_remaining.Text = timer_delay + "s";

        }
    }
}
