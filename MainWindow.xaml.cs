using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using AutoLotModel;

namespace Onit_Diana_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource customerOrdersViewSource;
        CollectionViewSource inventoryViewSource;
    
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerViewSource =
 ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customer.Local;
            customerOrdersViewSource =
((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));

            
            ctx.Customer.Load();
            
            ctx.Inventory.Load();
            cmbCustomers.ItemsSource = ctx.Customer.Local;
            
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventory.Local;
            
            cmbInventory.SelectedValuePath = "CarId";
            BindDataGrid();



            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventory.Local;
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]
        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty,
           firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
           
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty,
           lastNameValidationBinding); //setare binding nou
        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Order
                             join cust in ctx.Customer on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventory on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
           
            firstNameTextBox.IsEnabled = true;
            
            lastNameTextBox.IsEnabled = true;
            Keyboard.Focus(firstNameTextBox);

            customerDataGrid.SelectedItem = null;

            SetValidationBinding();



        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

          
            action = ActionState.Edit;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
           
            Keyboard.Focus(firstNameTextBox);
            SetValidationBinding();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            customerDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
           

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                   
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };

                   
                    ctx.Customer.Add(customer);
                    customerViewSource.View.Refresh();
                    
                    ctx.SaveChanges();
                }
                
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
              
            }
            else
        if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
               
                customerViewSource.View.MoveCurrentTo(customer);
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
               

            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customer.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
              

            }
            SetValidationBinding();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }


     

        private void btnNewInventory_Click(object sender, RoutedEventArgs e)
        {

            action = ActionState.New;
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            makeTextBox.Text = "";
            makeTextBox.IsEnabled = true;
            colorTextBox.Text = "";
            colorTextBox.IsEnabled = true;
            Keyboard.Focus(colorTextBox);

        }
        private void btnEditInventory_Click(object sender, RoutedEventArgs e)
        {
            string tempId = carIdTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            action = ActionState.Edit;
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            carIdTextBox.Text = tempId;
            makeTextBox.Text = tempMake;
            colorTextBox.Text = tempColor;
            Keyboard.Focus(colorTextBox);
        }
        private void btnDeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempId = carIdTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            carIdTextBox.Text = tempId;
            makeTextBox.Text = tempMake;
            colorTextBox.Text = tempColor;
        }
        private void btnCancelInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
            btnDeleteInv.IsEnabled = true;
            btnSaveInv.IsEnabled = false;
            btnCancelInv.IsEnabled = false;
            btnPrevInv.IsEnabled = true;
            btnNextInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = true;
            makeTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
           
        }
        private void btnSaveInventory_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                   
                    inventory = new Inventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };
                    
                    ctx.Inventory.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    
                    ctx.SaveChanges();
                }
                
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
               
            }
            else
        if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Make = makeTextBox.Text.Trim();
                    inventory.Color = colorTextBox.Text.Trim();
                    
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
              
                inventoryViewSource.View.MoveCurrentTo(inventory);
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
               
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventory.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                
            }
        }
        private void btnNextInventory_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }
        private void btnPreviousInventory_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    
                    ctx.Order.Add(order);
                    customerOrdersViewSource.View.Refresh();
                   
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
              
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Order.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            btnNewOrders.IsEnabled = true;
            btnDeleteOrders.IsEnabled = true;
            btnEditOrders.IsEnabled = true;
            btnPrevOrders.IsEnabled = true;
            btnNextOrders.IsEnabled = true;

        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewOrders.IsEnabled = false;
            btnDeleteOrders.IsEnabled = false;
            btnEditOrders.IsEnabled = false;
            btnPrevOrders.IsEnabled = false;
            btnNextOrders.IsEnabled = false;

        }
        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNewOrders.IsEnabled = false;
            btnDeleteOrders.IsEnabled = false;
            btnEditOrders.IsEnabled = false;
            btnPrevOrders.IsEnabled = false;
            btnNextOrders.IsEnabled = false;

        }
        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNewOrders.IsEnabled = false;
            btnDeleteOrders.IsEnabled = false;
            btnEditOrders.IsEnabled = false;
            btnPrevOrders.IsEnabled = false;
            btnNextOrders.IsEnabled = false;

        }

        private void btnNextOrder_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }
        private void btnPreviousOrder_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }
    }
}

