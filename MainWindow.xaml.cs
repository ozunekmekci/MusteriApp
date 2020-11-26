using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace MusteriApp
{
    //cost toplam+
    //progress ve goal+
    //istatistikler, giderler ve net
    //## uncheck koy ve değerleri 0 a eşitle netCost = 0 gibi veya direkt class altına 0 olarak tanımla ve methottan double yazılarını kaldır
    class NewItem
    {
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public string NUMBER { get; set; }
        public string COST { get; set; }
        public string DURUM { get; set; }

    }

    public partial class MainWindow : Window
    {
        string path_excel = "D:\\test.xlsx";
        int costSum = 0;

        bool bool_checkKDV = false;
        bool bool_checkBagkur = false;
        bool bool_checkMuhasebe = false;
        bool bool_checkKira = false;

        
        double outcomeSum = 0;
        double netWorth;

        double kdvCost = 0;
        double bagkurCost = 0;
        double muhasebeCost = 0;

        string durumText = "x";
        
        public async void RefreshDataTable()
        {
            await Task.Delay(1000);
            //create an application and open workbook and worksheet
            listviewTable.Items.Clear();    
            var app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(path_excel);
            Excel.Worksheet sheet = app.ActiveSheet as Excel.Worksheet; 

            //get the number of used rows and assign it to a variable
            Excel.Range range = sheet.UsedRange;
            int countRecords = range.Rows.Count;
            // Console.WriteLine(countRecords); //tam olarak satır sayısı.
            int newRow = countRecords + 1;
            for (int i = 2; i != newRow; i++)
            {
                // stringlere atma               
                string name = sheet.Cells[i, 1].Text; 
                string surname = sheet.Cells[i, 2].Text;
                string number = sheet.Cells[i, 3].Text;
                string cost = sheet.Cells[i, 4].Text;
                string durum = sheet.Cells[i, 5].Text;


                //stringleri listviewa ekle
                listviewTable.Items.Add(new NewItem { NAME = name, SURNAME = surname, NUMBER = number, COST = cost , DURUM = durum});
            }
            costSum = 0;
            for (int i = 2; i != newRow; i++)
            {
                // stringlere atma

                string input = sheet.Cells[i, 4].Text;
                int a = Convert.ToInt32(input);
                costSum += a;

                lblCostSum.Content = "Toplam Ücret: " + costSum + " TL";
            }

            //goal
            if(txtGoal.Text != String.Empty)
            {
                int goal = Convert.ToInt32(txtGoal.Text);
                barGoal.Maximum = goal;
                barGoal.Value = costSum;
            }

            //Toplam Müşteri Sayısı
            lblSumCustomer.Content = "Toplam Kişi: " + (countRecords-1) + " Kişi";

            kdvCost = 0;
            bagkurCost = 0;
            muhasebeCost = 0;

            //gider oluşturma
            if (bool_checkKDV == true)
            {
                kdvCost = (costSum * 0.18);
            }
            if (bool_checkBagkur == true)
            {
                bagkurCost = 941;
            }
            if (bool_checkMuhasebe == true)
            {
                muhasebeCost = 200;
            }

            //toplam gider ve networth
            outcomeSum = (kdvCost + bagkurCost + muhasebeCost);
            netWorth = (costSum - outcomeSum);

            lblOutcome.Content = "Toplam Gider: " + outcomeSum;
            lblKDV.Content = "KDV: " + kdvCost;
            lblNetCost.Content = "Net Kazanç: " + netWorth;

            //kapat
            workbook.Save();
            workbook.Close(false);
            app.Quit();
            await Task.Delay(500);

        }
        public void ExcelCreate(){
            Microsoft.Office.Interop.Excel._Application uygulama = new Microsoft.Office.Interop.Excel.Application();
            uygulama.DisplayAlerts = false;
            uygulama.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook kitap = uygulama.Workbooks.Add(System.Reflection.Missing.Value);
            Microsoft.Office.Interop.Excel.Worksheet sayfa = (Microsoft.Office.Interop.Excel.Worksheet)kitap.Sheets[1];
            //Microsoft.Office.Interop.Excel.Range alan = (Microsoft.Office.Interop.Excel.Range)sayfa1.Cells[2, 5];
            //alan.Value2 = txtboxName.Text;
            //      [SATIR , SUTUN]
            sayfa.Cells[1, 1] = "Name";
            sayfa.Cells[1, 2] = "Surname";
            sayfa.Cells[1, 3] = "Tel Num";
            sayfa.Cells[1, 4] = "Cost";
            sayfa.Cells[1, 5] = "Durum";
            kitap.SaveAs(path_excel, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            kitap.Close();
            uygulama.Quit();
        }
        public MainWindow()
        {
            InitializeComponent();

            if ((File.Exists("D:\\test.xlsx")  == false))
            {
                ExcelCreate();
            }
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            //inputları al
            string name = txtboxName.Text;
            string surname = txtboxSurname.Text;
            string telno = txtboxTelNo.Text;
            string cost = txtboxCost.Text;

            //uygulamayı aç
            var app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(path_excel);
            Excel.Worksheet sheet = app.ActiveSheet as Excel.Worksheet;

            //satır sayısını bul
            Excel.Range usedrange = sheet.UsedRange;
            int rows = usedrange.Rows.Count;
            
            //yerleştir
            sheet.Cells[rows + 1, 1] = name;
            sheet.Cells[rows + 1, 2] = surname;
            sheet.Cells[rows + 1, 3] = telno;
            sheet.Cells[rows + 1, 4] = cost;
            sheet.Cells[rows + 1, 5] = durumText;

            //kapat
            workbook.Save();          
            workbook.Close(0);
            app.Quit();
            RefreshDataTable();
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            //seçili listview indexi al.
            int selectedItemIndex = listviewTable.SelectedIndex + 2;

            //uygulamayı aç
            var app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(path_excel);
            Excel.Worksheet sheet = app.ActiveSheet as Excel.Worksheet;

            Excel.Range deletRow = sheet.Range["A" + selectedItemIndex, "E" + selectedItemIndex];
            deletRow.EntireRow.Delete(Type.Missing);

            //kapat
            workbook.Save();
            workbook.Close(0);
            app.Quit();
            RefreshDataTable();

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {    
            //seçili listview indexi al.
            int selectedItemIndex = listviewTable.SelectedIndex + 2;

            //uygulamayı aç
            var app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(path_excel);
            Excel.Worksheet sheet = app.ActiveSheet as Excel.Worksheet;

            txtboxName.Text = sheet.Cells[selectedItemIndex, 1].Text;
            txtboxSurname.Text = sheet.Cells[selectedItemIndex, 2].Text;
            txtboxTelNo.Text = sheet.Cells[selectedItemIndex, 3].Text;
            txtboxCost.Text = sheet.Cells[selectedItemIndex, 4].Text;

            //kapat
            workbook.Save();
            workbook.Close(0);
            app.Quit();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            //start button
            RefreshDataTable();
        }


        private void checkKDV_Checked(object sender, RoutedEventArgs e)
        {
            bool_checkKDV = true;
        }

        private void checkBagkur_Checked(object sender, RoutedEventArgs e)
        {
            bool_checkBagkur = true;
        }

        private void checkMuhasebe_Checked(object sender, RoutedEventArgs e)
        {
            bool_checkMuhasebe = true;
        }

        private void checkKira_Checked(object sender, RoutedEventArgs e)
        {
            bool_checkKira = true;
            
        }

        private void checkKDV_Unchecked(object sender, RoutedEventArgs e)
        {
            bool_checkKDV = false;

        }

        private void checkBagkur_Unchecked(object sender, RoutedEventArgs e)
        {
            bool_checkBagkur = false;

        }

        private void checkMuhasebe_Unchecked(object sender, RoutedEventArgs e)
        {
            bool_checkMuhasebe = false;

        }

        private void checkKira_Unchecked(object sender, RoutedEventArgs e)
        {
            bool_checkKira = false;

        }

        private void radioWaiting_Checked(object sender, RoutedEventArgs e)
        {
            durumText = "Beklemede";
        }

        private void radioPayyed_Checked(object sender, RoutedEventArgs e)
        {
            durumText = "Ödendi";

        }
    }
}
