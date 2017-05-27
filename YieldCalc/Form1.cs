using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YieldCalc
{
    public partial class Form1 : Form
    {
        private DataTable dtData;
        public Form1()
        {
            InitializeComponent();
            dtData = new DataTable();
            dtData.TableName = "Rate";
            this.txtDate.Value = DateTime.Today;
            dtData.Columns.Add("Date", typeof(DateTime));
            dtData.Columns.Add("Money", typeof(double));

            //dtData.ReadXml(Path.Combine(Environment.CurrentDirectory + "Data.xml"));
            dtData.Rows.Add(new DateTime(2017, 5, 26), 295.2);
            dtData.Rows.Add(new DateTime(2017, 5, 13), 315.2);
            dtData.Rows.Add(new DateTime(2017, 5, 13), 100);
            dtData.Rows.Add(new DateTime(2017, 5, 14), 630);
            dtData.Rows.Add(new DateTime(2017, 5, 21), 315.2);
            dtData.Rows.Add(new DateTime(2017, 5, 24), 315.2);

            this.dataGridView1.DataSource = dtData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double decReturn;
            if (!double.TryParse(this.txtReturn.Text, out decReturn))
            {
                this.txtResult.Text = "收益无效！";
                return;
            }

            DateTime dtEndDate = this.txtDate.Value;
            if (dtData.Rows.Count > 0)
            {
                double sumMul = 0;
                DataRow[] rows = dtData.Select(null, "Date");
                foreach (DataRow row in rows)
                {
                    DateTime date = (DateTime)row["Date"];
                    double money = (double)row["Money"];

                    TimeSpan span = dtEndDate - date;
                    int dates = (int)span.TotalDays;
                    double timeMoney = dates * money;
                    sumMul += timeMoney;
                }

                DateTime firstDate = (DateTime)rows[0]["Date"];
                double firstTotalDay = (dtEndDate - firstDate).TotalDays;
                double conMoney = sumMul / firstTotalDay;

                //double rateDates = decReturn / firstTotalDay / conMoney;

                double countToYear = 365 / firstTotalDay;
                double result = Math.Pow(1 + decReturn / conMoney, countToYear) - 1;
                this.txtResult.Text = result.ToString("p");
                dtData.WriteXml(Path.Combine(Environment.CurrentDirectory, "Data.xml"), XmlWriteMode.IgnoreSchema);
            }
        }
    }
}
