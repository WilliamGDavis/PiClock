using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PiClock.classes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PiClock
{
    public sealed partial class QuickView : Page
    {
        private List<Employee> EmployeeList { get; set; }

        public QuickView()
        { InitializeComponent(); }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        { EmployeeList = e.Parameter as List<Employee>; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Populate the Employee ListView
            foreach (Employee employee in EmployeeList)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Tag = employee.id;
                lvItem.Content = BuildListViewRow(employee);
                listView_Employees.Items.Add(lvItem);
            }
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }

        private string BuildListViewRow(Employee employee)
        { return String.Format("{0}", FormatFullName(employee)); }

        private string FormatFullName(Employee employee)
        { return string.Format("{0} {1} {2}", employee.fname, employee.mname, employee.lname); }

        private async void listView_Employees_ItemActivate(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = listView_Employees.SelectedIndex;
            var name = (ListViewItem)listView_Employees.Items[selectedIndex];

            var employee = new Employee();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "CheckLoginStatus");
            paramDictionary.Add("employeeId", name.Tag.ToString());
            employee.ParamDictionary = paramDictionary;

            bool isLoggedIn = ("true" == await employee.CheckLoginStatus()) ? true : false;

            Job currentJob = null;
            if (true == isLoggedIn)
            {
                paramDictionary.Clear();
                Job job = new Job();
                paramDictionary.Add("action", "GetCurrentJob");
                paramDictionary.Add("employeeId", name.Tag.ToString());
                job.ParamDictionary = paramDictionary;

                //Should return a JSON string or null (if there were errors)
                string result = await job.GetCurrentJob();

                if ("null" != result)
                { currentJob = JsonConvert.DeserializeObject<Job>(result); }
            }
            string jobDescription = (null != currentJob) ? currentJob.Description : "None";
            textBlock.Text = string.Format("ID: {0}\nName: {1}\nLogged In: {2}", name.Tag.ToString(),
                                                                                 name.Content.ToString(),
                                                                                 (true == isLoggedIn) ? "Yes (" + jobDescription + ")" : "No"
                                                                                 );
        }

        private async Task<Job> GetCurrentJob()
        {
            return new Job();
        }

        private async Task<string> TryGetCurrentJob()
        {
            return null;
        }
    }

}
