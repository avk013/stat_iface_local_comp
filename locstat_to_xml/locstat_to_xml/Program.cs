using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace locstat_to_xml
{
    public class Program
    {

[DllImport("User32.dll", CharSet = CharSet.Unicode)]
public static extern int MessageBox(IntPtr h, string m, string c, int type);

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            if (File.Exists(path + @"\config0.cfg"))
            {
                string path_c = @"e:\!serv\config.cfg";
                string path_log_out = @"c:\statNETifce\out";
                string path_log = @"c:\statNETifce\";

                //string[] tab0 = File.ReadAllLines(path, Encoding.UTF8);
                string[] tab0 = File.ReadAllLines(path_c);
                string[] tab0Values = null;
                DataRow dr = null;
                int nom_if = 0;
                for (int i0 = 0; i0 < tab0.Length; i0++)
                {
                    if (!String.IsNullOrEmpty(tab0[i0]))
                    {
                        tab0Values = tab0[i0].Split(',');
                        path_log = tab0Values[0];
                        nom_if = Convert.ToInt16(tab0Values[1]);
                        ////////////////////////////////////////
                        System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(@path_log);
                        System.IO.DirectoryInfo[] dirs = info.GetDirectories();
                        System.IO.FileInfo[] files = info.GetFiles("*.txt");
                        //foreach (string s in files.name) label1.Text += s;
                        //создание таблицы с именами всех фыйлов, разделенных на столбцы по знаку "_"
                        // name_date_time_uptimeHour_uptimeMinutes_uptimeSecund
                        DataTable dt = new DataTable("tab0");
                        int st = 0;
                        //dt.Clear();
                        //dt = new DataTable("tab0");
                        DataColumn a0 = new DataColumn("хост", typeof(String));
                        DataColumn a1 = new DataColumn("дата", typeof(String));
                        DataColumn a2 = new DataColumn("время", typeof(String));
                        DataColumn a3 = new DataColumn("аптайм", typeof(String));
                        DataColumn a4 = new DataColumn(st++.ToString(), typeof(String));
                        DataColumn a5 = new DataColumn("получено(байт)", typeof(String));
                        DataColumn a6 = new DataColumn("отдано(байт)", typeof(String));
                        // download_upload
                        DataColumn a7 = new DataColumn(st++.ToString(), typeof(String));
                        DataColumn a8 = new DataColumn(st++.ToString(), typeof(String));
                        DataColumn a9 = new DataColumn(st++.ToString(), typeof(String));
                        DataColumn a10 = new DataColumn(st++.ToString(), typeof(String));
                        dt.Columns.AddRange(new DataColumn[] { a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10 });

                        //            string[] tab0Values = null;
                        //DataRow dr = null;
                        /////////////////////////////////


                        for (int i = 0; i < files.Length; i++)
                        {
                            // textBox2.Text += files[i].Name + Environment.NewLine;
                            tab0Values = files[i].Name.Split('_');
                            dr = dt.NewRow();
                            for (int ii = 0; ii < 3; ii++) { dr[ii] = tab0Values[ii]; }
                            //считываем содержимое файла              
                            /* формат файла
                             Flags: D - dynamic, X - disabled, R - running, S - slave 
                             #     NAME               RX-BYTE           TX-BYTE     RX-PACKET     TX-PACKET
                             0  R  Lan         71 292 359 093 1 034 318 413 528   322 675 085   780 861 923
                             1  R  Sky      1 064 572 143 276    83 572 080 930   786 977 972   327 838 501
                             2  X  ether3                   0                 0             0             0
                             3  X  ether4                   0                 0             0             0
                             4  X  globa...                 0                 0             0             0
                             5  X  globa...                 0                 0             0             0
                             6  R  pptp-... 1 020 807 199 464    63 961 725 927   784 864 546   327 233 856
                             идея два пробела меняем на знак подчеркивания, 
                             1. разделить на столбцы, понятно что столбец 4-5-3-7 заканчивается 
                             на количество символов, равное на заголовок 2-й строки
                             #     NAME               RX-BYTE           TX-BYTE     RX-PACKET     TX-PACKET
                            фиксированная ширина поля 
                            2 6 14 34 52 66 80
                            1-2-10-18-18-14-14
                             *///

                            string[] readText = File.ReadAllLines(path_log + files[i].Name, Encoding.GetEncoding(1251));
                            for (int ii = 0; ii < readText.Length; ii++)
                            {// ищем строку с номером интерфейса из конфига и считываем поле 34 и 52
                                string data = readText[ii];//объединяем 2 строчки если длина строки 15
                                if (data.Length <= 17 && data.Length > 10) data = data.Substring(0, 15) + readText[ii + 1].Substring(16);
                                if (data.Length >= 44)
                                {
                                    int number;
                                    bool isNumeric = int.TryParse(data.Substring(0, 2), out number);
                                    if (isNumeric)
                                    {
                                        int num_ifile = Convert.ToInt16(data.Substring(0, 2));
                                        if (num_ifile == nom_if)
                                        {
                                            dr[5] = data.Substring(15, 18).Trim().Replace(" ", "");
                                            dr[6] = data.Substring(34, 18).Trim().Replace(" ", "");
                                        }
                                    }
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                        //dataGridView1.DataSource = dt;
                        // подумать если пусто
                        if (dt.Rows.Count > 0)
                        {//записываем текущие данные
                            string dates = DateTime.Now.ToString();
                            dates = dates.Replace(" ", "_"); dates = dates.Replace(":", "-"); dates = dates.Replace(".", "_");
                            if (!Directory.Exists(@path_log_out)) Directory.CreateDirectory(@path_log_out);
                            //label4.Text = dates;
                            dt.WriteXml(@path_log_out + @"\" + dates + @"out.xml");
                            // ищем уникальніе имена
                            string name_u = dt.Rows[0][0].ToString(), name_old = "";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                name_u = dt.Rows[i][0].ToString();
                                if (name_u != name_old)
                                {
                                    name_old = name_u;
                                    //label3.Text += name_old;
                                    //кидаем в соотвующую дирректорию
                                    /////////////////////
                                    //откуда копируем
                                    move2path(name_old, path_log, path_log_out);
                                }
                                /////////////
                            }
                        }
                    }
                }
            }
            else MessageBox((IntPtr)0, "Необходимо 2 конфига: config0.cfg и config.cfg"+Environment.NewLine+ "config0.cfg содержит:" + Environment.NewLine + "путь_куда_сохраняется_файл_out.xml" + Environment.NewLine + "путь_куда_переносятся_файлы_логов_после_обработки " + Environment.NewLine + Environment.NewLine+ "config.cfg содержит:" + Environment.NewLine + "путь_где_находятся_логи,идентификатор_интерфейса "+ Environment.NewLine + "путь2_где_находятся_логи,идентификатор2_интерфейса", "message", 0);
            ///////////////////////////////////////
        }
        static void move2path(string @name_old, string @path, string @path_new)
        {    //откуда копируем
            string Dir1 = @path;
            //куда копируем
            //string Dir2 = @path + @"\" + @name_old;
            string Dir2 = @path_new + @"\" + @name_old;
            if (!Directory.Exists(Dir2)) Directory.CreateDirectory(Dir2);
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Dir1);
                foreach (FileInfo file in dirInfo.GetFiles(name_old + "*.txt"))
                { File.Move(file.FullName, @Dir2 + @"\" + file.Name); }
            }
            catch (Exception ex)
            {// MessageBox.Show(ex.Message + Environment.NewLine);
            }
        }
    }
}
