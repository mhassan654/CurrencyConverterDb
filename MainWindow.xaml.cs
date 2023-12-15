using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Configuration;

namespace CurrencyConverterDb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();

        private int CurrencyId = 0;
       // private int FromAmount = 0;
      //  private int ToAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            GetData();
        }

        public void MyConn()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(Conn);
            conn.Open();
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }

                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;

                }
                else
                {
                    if (CurrencyId > 0)
                    {
                        if (MessageBox.Show("Are you sure want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
                        {
                            MyConn();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id =@Id", conn)
                            {
                                CommandType = CommandType.Text
                            };
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            MessageBox.Show("Data update successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }else
                    {
                        if (MessageBox.Show("Are you sure want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) ;
                        {
                            MyConn();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount,CurrencyName) VALUES(@Amount,@CurrencyName)", conn)
                            {
                                CommandType = CommandType.Text
                            };
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }                      

                    }

                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }

        private void GetData()
        {
            MyConn();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Currency_Master",conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt != null && dt.Rows.Count > 0)
            {
                dgCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                dgCurrency.ItemsSource = null;
            }

            conn.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertableValue;

            // check amount textbox is nul or blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //if amount textbox is null or blank it will show the below message box
                MessageBox.Show("Please Enter currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //after clicking on message box ok sets the focus on amount txtbox
                txtCurrency.Focus();
                return;
            }
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currecny From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currecny To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                cmbFromCurrency.Focus();
                return;
            }

            //check if value from and to combox sletec boxes are the same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // amount textbox value set in converted value
                //double.parse is used for converting the datstype string
                // textbox text have string and convertedvalue is double
                ConvertableValue = double.Parse(txtCurrency.Text);

                //show the lable converted currency and converted currency name and tostring
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertableValue.ToString("N3");
            }
            else
            {
                ConvertableValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) *
                    double.Parse(txtCurrency.Text)) /
                double.Parse(cmbToCurrency.SelectedValue.ToString());

                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertableValue.ToString("N3");

            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }


        private void NumberValidationText(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BindCurrency()
        {
            MyConn();

            //create an object for databale

            DataTable dt = new DataTable();

            // write query to get data from currency master table
            cmd = new SqlCommand("SELECT Id, CurrencyName FROM Currency_Master", conn)
            {
                CommandType = CommandType.Text
            };

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            //create an object for datarow
            DataRow newRow = dt.NewRow();

            // assign a value to Id column
            newRow["Id"] = 0;

            //assign value to currencyname column
            newRow["CurrencyName"] = "--SELECT--";

            // insert a new row in dt with the data at a 0 index position
            dt.Rows.InsertAt(newRow, 0);

            // dt is not null and rows count greater than 0
            if (dt != null && dt.Rows.Count > 0)
            {
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            conn.Close();


            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;

            //combox to currency
            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }

            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = "";
            txtCurrency.Focus();


        }

        private void DgCurrency_SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            try
            {
                DataGrid grd = (DataGrid)sender;

                if (grd.CurrentItem is DataRowView row_selected)
                {
                    if (dgCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }

                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                //DisplayIndex is equal to one in the deleted cell
                                if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                                {
                                    //Show confirmation dialog box
                                    if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    {
                                        MyConn();
                                        DataTable dt = new DataTable();

                                        //Execute delete query to delete record from table using Id
                                        cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", conn);
                                        cmd.CommandType = CommandType.Text;

                                        //CurrencyId set in @Id parameter and send it in delete statement
                                        cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                        cmd.ExecuteNonQuery();
                                        conn.Close();

                                        MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                        ClearMaster();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) 
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }
    }
}
