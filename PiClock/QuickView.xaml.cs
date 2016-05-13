using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PiClock.classes;


namespace PiClock
{
    public sealed partial class QuickView : Page
    {
        private List<Employee> employeeList;

        public QuickView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Retrieve the data from the previous parent page
            //Note: You cannot set control values until the "Loaded" event fires
            this.employeeList = e.Parameter as List<Employee>;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Populate the Employee ListView
            foreach (Employee employee in this.employeeList)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Tag = employee.id;
                lvItem.Content = BuildListViewRow(employee);
                listView_Employees.Items.Add(lvItem);
            }
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            this.employeeList = null;
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private string BuildListViewRow(Employee employee)
        {
            return String.Format("{0}", FormatFullName(employee));
        }

        private string FormatFullName(Employee employee)
        {
            return string.Format("{0} {1} {2}", employee.fname, employee.mname, employee.lname);
        }

        private void listView_Employees_ItemActivate(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = listView_Employees.SelectedIndex;
            ListViewItem name = (ListViewItem)listView_Employees.Items[selectedIndex];
            textBlock.Text = String.Format("ID: {0}\nName: {1}", name.Tag.ToString(), name.Content.ToString());
        }
    }

}
