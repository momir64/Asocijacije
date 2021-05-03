using System;
using System.Windows.Forms;

namespace Asocijacije {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            //Search4("džez");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DobroVečeForm());
        }

        async static void Search4(params string[] str) {
            SlagalicaTV client = new SlagalicaTV();
            DateTime start = new DateTime(2020, 1, 1);
            DateTime today = DateTime.Today;
            DateTime date = start;
            while (date != today) {
                string data = await client.FindData(date.ToString("yyyy-MM-dd"), str);
                Console.Title = ((date - start).TotalDays / (today - start).TotalDays).ToString("P");
                if (data.Length > 0) {
                    Console.WriteLine(date.ToString("yyyy-MM-dd"));
                    Console.WriteLine(data);
                    Console.WriteLine();
                }
                date = date.AddDays(1);
            }
            Console.Beep();
        }
    }
}
