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
        private const string Rate = "Rate";
        private const string Return = "Return";
        private DataSet ds;
        public Form1()
        {
            InitializeComponent();
            this.txtDate.Value = DateTime.Today;
            ds = new DataSet("Data");

            DataTable dtData = new DataTable(Rate);
            dtData.Columns.Add("Date", typeof(DateTime));
            dtData.Columns.Add("Money", typeof(double));
            DataTable dtReturn = new DataTable(Return);
            dtReturn.Columns.Add("Value", typeof(double));

            ds.Tables.Add(dtData);
            ds.Tables.Add(dtReturn);

            try
            {
                ds.ReadXml(Path.Combine(Environment.CurrentDirectory, "Data.xml"));
            }
            catch(Exception ex)
            {

                dtReturn.Rows.Add(0);
            }

            this.dataGridView1.DataSource = ds.Tables[Rate];
            this.txtReturn.Text = ds.Tables[Return].Rows[0][0].ToString();
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
            DataTable dtData = ds.Tables[0];
            if (dtData.Rows.Count > 0)
            {
                double result = CalcYield(decReturn, dtEndDate);
                double money = CalcMoney();
                this.txtResult.Text = result.ToString("p");
                this.txtMoney.Text = money.ToString("N2");

                ds.Tables[Return].Rows[0][0] = decReturn;
                ds.WriteXml(Path.Combine(Environment.CurrentDirectory, "Data.xml"));
            }
        }


        private double CalcYield(double decReturn, DateTime dtEndDate)
        {
            double sumMul = 0;
            DataTable dtData = ds.Tables[0];
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
            return result;
        }

        private double CalcMoney()
        {
            double sumMoney = 0;
            DataTable dtData = ds.Tables[0];
            DataRow[] rows = dtData.Select(null, "Date");
            foreach (DataRow row in rows)
            {
                DateTime date = (DateTime)row["Date"];
                double money = (double)row["Money"];

                sumMoney += money;
            }

            return sumMoney;
        }
    }
}
