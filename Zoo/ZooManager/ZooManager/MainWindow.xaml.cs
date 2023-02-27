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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlCon;
        public MainWindow()
        {
            InitializeComponent();
            

           string connectionString = ConfigurationManager.ConnectionStrings["ZooManager.Properties.Settings.ManoloDBConnectionString"].ConnectionString;
           
            sqlCon = new SqlConnection(connectionString);
            ShowZoos();
            ShowAllAnimal();
           
        }

        private void ShowZoos()
        {
           try
            {
                string query = "select * from Zoo";
              
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlCon);
               
                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    listZoos.DisplayMemberPath = "Location";
                    listZoos.SelectedValuePath = "Id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
               
            } 

            
        }

        private void ShowAssociatedAnimals()
        {
            try
            {

                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                   
                    
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue ?? DBNull.Value);
                       
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    associatedAnimalList.DisplayMemberPath = "Name";
                    associatedAnimalList.SelectedValuePath = "Id";
                    associatedAnimalList.ItemsSource = animalTable.DefaultView;
                } 
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
               
            }
            
            

        }


        private void ListZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            InsertZooIntoText();
        }

        private void ListAnimal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InsertAnimalIntoText();
        }


        private void ShowAllAnimal()
        {
            try 
            {

                string query = "select * from Animal";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlCon);
                using (sqlDataAdapter)
                {
                    DataTable dataTable= new DataTable();
                    sqlDataAdapter.Fill(dataTable);

                    animalList.DisplayMemberPath = "Name";
                    animalList.SelectedValuePath = "Id";
                    animalList.ItemsSource = dataTable.DefaultView;
                }


            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteZoo_Click(object sender, EventArgs e)
        {
            if (listZoos.Items.Count == 0)
            {
                del.IsEnabled = false;
                sqlCon.Dispose();
            } else
            try
            {

                string query = "delete from Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);

                sqlCon.Open();

                SqlDataAdapter sqlAdap = new SqlDataAdapter(sqlCommand);
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               
            } finally
            {
                sqlCon.Close();
                ShowZoos();
               
            }

        }

        private void DeleteAnimal(object sender, EventArgs e)
        {
            try 
            {
                string query = "delete from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);

                sqlCon.Open();

                sqlCommand.Parameters.AddWithValue("@AnimalId", animalList.SelectedValue);
                sqlCommand.ExecuteScalar();


            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                sqlCon.Close();
                ShowAllAnimal();
            }
        }

        private void AddAnimal(object sender, EventArgs e)
        {
           try
            {
                string query = "insert into Animal values (@Name)";

                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);

                sqlCon.Open();

                sqlCommand.Parameters.AddWithValue("@Name", inputField.Text);
                sqlCommand.ExecuteScalar();

            } catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
            } finally
            { 
                sqlCon.Close();
                ShowAllAnimal();
                inputField.Text = "";
            
            }
        }

        private void AddAnimalToZoo (object sender, EventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", animalList.SelectedValue);
                sqlCommand.ExecuteScalar();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            } finally
            {
                sqlCon.Close();
                ShowAssociatedAnimals();
            }
        }

        private void RemoveAnimalAssociatedList (object sender, EventArgs e)
        {
            try
            {
                string query = "delete from ZooAnimal where AnimalId = @AnimalId AND ZooId = @ZooId";
                
                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();

              
                sqlCommand.Parameters.AddWithValue("@AnimalId", associatedAnimalList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            finally
            {
                sqlCon.Close();
                ShowAssociatedAnimals();
            }

        }

        private void AddZoo(object sender, EventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlCommand.Parameters.AddWithValue("@Location", inputField.Text);
                sqlCommand.ExecuteScalar();

            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                sqlCon.Close();
                ShowZoos();
                inputField.Text = "";
            }
        }

        

        

        private void UpdateZoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Zoo Set Location = @Location where Id = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", inputField.Text);

                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
               
                sqlCon.Close();
                inputField.Text = "";
              
                ShowZoos();
            
            }
            
        }

        private void UpdateAnimals(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Animal Set Name = @Name where Id = @AnimalId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                sqlCommand.Parameters.AddWithValue("@AnimalId", animalList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", inputField.Text);

                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                sqlCon.Close();
                inputField.Text = "";

                ShowAllAnimal();

            }
        }

        private void InsertZooIntoText()
        {
           try
            {
                string query = "select Location from Zoo where Id = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();                
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);

                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue ?? DBNull.Value);
               

                DataTable dataTable= new DataTable();

                sqlAdapter.Fill(dataTable);

              

                sqlCommand.ExecuteScalar();

               if (dataTable.Rows.Count > 0 )
                {
                    inputField.Text = dataTable.Rows[0]["Location"].ToString();
                }


            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                sqlCon.Close();
            }
            
        }

        private void InsertAnimalIntoText ()
        {
            try
            {
                string query = "select Name from Animal where Id = @AnimalId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlCon);
                sqlCon.Open();

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCommand);

                sqlCommand.Parameters.AddWithValue("@AnimalId", animalList.SelectedValue ?? DBNull.Value);

                DataTable dataTable = new DataTable();
                sqlAdapter.Fill(dataTable);
                sqlCommand.ExecuteScalar();

                if(dataTable.Rows.Count > 0 )
                {
                    inputField.Text = dataTable.Rows[0]["Name"].ToString();
                }


            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                sqlCon.Close();
            }
        }

       
    }
}
