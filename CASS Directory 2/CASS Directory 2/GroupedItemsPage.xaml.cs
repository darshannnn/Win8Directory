using CASS_Directory_2.Data;
using CASS_Directory_2.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace CASS_Directory_2
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : CASS_Directory_2.Common.LayoutAwarePage
    {
        DispatcherTimer dTimer = new DispatcherTimer();

        public GroupedItemsPage()
        {
            this.InitializeComponent();
             
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
           // var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
         //   var contactDataGroups = ContactDataSource.GetGroups();
            var contactDataGroups = ContactDataSource.GetFunctionalGroup();
            this.DefaultViewModel["Groups"] = contactDataGroups;
            dTimer.Tick += dTimer_Tick;
            int hrs = 0;
            int mins = 0;
            int sec = 60;
            dTimer.Interval = new TimeSpan(hrs, mins, sec);
        }

        void dTimer_Tick(object sender, object e)
        {
            Home(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }


        private void SearchBoxEventsSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            var contactDataGroups = ContactDataSource.GetGroups();
            string queryText = e.QueryText;
            if (!string.IsNullOrEmpty(queryText))
            {
                Windows.ApplicationModel.Search.SearchSuggestionCollection suggestionCollection = e.Request.SearchSuggestionCollection;
                foreach (SampleDataGroup group in contactDataGroups)
                {
                    foreach (SampleDataItem item in group.Items)
                    {

                        if (item.GivenName.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase) || item.Surname.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase))
                            suggestionCollection.AppendQuerySuggestion(item.GivenName + " " + item.Surname);
                    }

                }

            }
        }

        private void SearchBoxEventsQuerySubmitted(object sender, SearchBoxQuerySubmittedEventArgs e)
        {
            //var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(ItemDetailPage), itemId);
            ////MainPage.Current.NotifyUser(e.QueryText, NotifyType.StatusMessage);
            //itemGridView.SetBinding = 
            
            var searchResult = ContactDataSource.SearchGroup(e.QueryText);
            this.DefaultViewModel["Groups"] = searchResult;
            SearchContacts.QueryText = "";
            dTimer.Start();
            //Binding searchedBinding = new Binding();
            //searchedBinding.Source = "searchResult";

            //itemGridView.SetBinding(GridView.DataContextProperty,searchedBinding);

            

        }

        private void Alpha_Click(object sender, RoutedEventArgs e)
        {
            Button al = (Button)sender;
            var searchResult = ContactDataSource.SearchGroupStartsWith(al.Name);
            this.DefaultViewModel["Groups"] = searchResult;
            dTimer.Start();
        }

        private void Home(object sender, RoutedEventArgs e)
        {
            var contactDataGroups = ContactDataSource.GetFunctionalGroup();
            this.DefaultViewModel["Groups"] = contactDataGroups;
            dTimer.Stop();
            ContactDataSource.refreshData();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            SearchContacts.QueryText = "";
        }
    }
}
