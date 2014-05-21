using CASS_Directory_2.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASS_Directory_2.DataModel
{
    class ContactDataSource
    {
        private static ContactDataSource _contactDataSource = new ContactDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; } 
        }

        public static IEnumerable<SampleDataGroup> GetGroups()
        {
            return _contactDataSource.AllGroups;
        }

        public static IEnumerable<SampleDataGroup> GetFunctionalGroup()
        {
            var functionalGroup =  _contactDataSource.AllGroups.SelectMany(group => group.Items).Where(item => item.UserType.Equals("F"));
            ObservableCollection<SampleDataGroup> searchedResultGroup = new ObservableCollection<SampleDataGroup>();

             var group1 = new SampleDataGroup("functionalGroup",
                 "Functional Areas",
                 "Assets/DarkGray.png",
                 "Group Description: functional Areas");

            searchedResultGroup.Add(group1);

            foreach (SampleDataItem i in functionalGroup)
                searchedResultGroup.ElementAt(0).Items.Add(i);

            return searchedResultGroup;

        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            var matches = _contactDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            var matches = _contactDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static IEnumerable<SampleDataGroup> SearchGroup(string query)
        {
          
            string [] name = query.Split(new Char [] {' '});

            string first = name[0];
          //string second = name[1];
           
            if(name.Length >1)
            {
            
            var matches = _contactDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.GivenName.IndexOf(name[0], StringComparison.OrdinalIgnoreCase) >= 0 && item.Surname.IndexOf(name[1], StringComparison.OrdinalIgnoreCase) >= 0);
            
            ObservableCollection<SampleDataGroup> searchedResultGroup = new ObservableCollection<SampleDataGroup>();

            var group1 = new SampleDataGroup("searchedResultGroup-1",
                 "Search Result",
                 "Assets/DarkGray.png",
                 "Group Description: searchedResultGroup");

            searchedResultGroup.Add(group1);

            foreach (SampleDataItem i in matches)
                searchedResultGroup.ElementAt(0).Items.Add(i);

            return searchedResultGroup;
            }

            else
            {
                var matches = _contactDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.GivenName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 || item.Surname.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
            
            ObservableCollection<SampleDataGroup> searchedResultGroup = new ObservableCollection<SampleDataGroup>();

            var group1 = new SampleDataGroup("searchedResultGroup-1",
                 "Search Result",
                 "Assets/DarkGray.png",
                 "Group Description: searchedResultGroup");

            searchedResultGroup.Add(group1);

            foreach (SampleDataItem i in matches)
                searchedResultGroup.ElementAt(0).Items.Add(i);

            return searchedResultGroup;
            }
        }

        public static IEnumerable<SampleDataGroup> SearchGroupStartsWith(string query)
        {
            var firstnameMatches = _contactDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.GivenName.StartsWith(query)); //IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
            var surnameMatches = _contactDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.Surname.StartsWith(query));

            ObservableCollection<SampleDataGroup> searchedResultGroup = new ObservableCollection<SampleDataGroup>();

            var firstnameGroup = new SampleDataGroup("searchedResultGroup-1",
                 "Results By Firstname",
                 "Assets/DarkGray.png",
                 "Group Description: searchedResultGroup");

            var surnameGroup = new SampleDataGroup("searchedResultGroup-1",
                 "Results By Surname",
                 "Assets/DarkGray.png",
                 "Group Description: searchedResultGroup");

            searchedResultGroup.Add(firstnameGroup);
            searchedResultGroup.Add(surnameGroup);

            foreach (SampleDataItem i in firstnameMatches)
                searchedResultGroup.ElementAt(0).Items.Add(i);

            foreach (SampleDataItem i in surnameMatches)
                searchedResultGroup.ElementAt(1).Items.Add(i);

            return searchedResultGroup;
        }

        //public static IEnumerable<SampleDataGroup> SearchGroupUID(string query)
        //{
        //    var uidMatches = _contactDataSource.AllGroups.SelectMany(group => group.Items).Where(item => item.UniqueId.StartsWith(query));
            
        //    ObservableCollection<SampleDataGroup> searchedResultGroup = new ObservableCollection<SampleDataGroup>();

        //    var uidGroup = new SampleDataGroup("searchedResultGroup-1",
        //         "Administration Areas",
        //         "Assets/DarkGray.png",
        //         "Group Description: searchedResultGroup");


        //    searchedResultGroup.Add(uidGroup);
           
        //    foreach (SampleDataItem i in uidMatches)
        //        searchedResultGroup.ElementAt(0).Items.Add(i);

        //    return searchedResultGroup;
        //}




        public static void refreshData() 
        {
            using (MySqlConnection connection = new MySqlConnection("Server=150.203.139.164;Database=rptdb_prod;Uid=remote;Password=CASSAdmin;Connection Timeout=2200"))
            {
                connection.Open();

                // Category list
                MySqlCommand categoriesCommand = new MySqlCommand("SELECT property_cd, property_name FROM space_property WHERE property_cd='H013'", connection);
                using (MySqlDataReader reader = categoriesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SampleDataGroup group = new SampleDataGroup(reader.GetString("property_cd"),
                          reader.GetString("property_name"),
                          "Assets/LightGray.png",
                          string.Empty);
                    

                        // Film list by Category
                        using (MySqlConnection filmsConnection = (MySqlConnection)connection.Clone())
                        {
                            filmsConnection.Open();
                            MySqlCommand filmsCommand = new MySqlCommand(@"SELECT ifnull(uni_id,'') as uni_id,ifnull(given_name,'') as given_name,ifnull(surname,'') as surname,ifnull(user_type,'') as user_type,ifnull(tel_no,'') as tel_no,ifnull(email,'') as email,
                                                                           ifnull(level_no,'') as level_no,ifnull(room_no,'') as room_no FROM space_room_allocation"
                                                                           + " WHERE given_name <> '' AND property_cd = ?id ORDER BY given_name",
                                                                            filmsConnection);
                            filmsCommand.Parameters.Add(new MySqlParameter("?id", reader.GetString("property_cd")));
                            using (MySqlDataReader filmsReader = filmsCommand.ExecuteReader())
                            {
                                while (filmsReader.Read())
                                {
                                    group.Items.Add(new SampleDataItem(filmsReader.GetString("uni_id"),
                                    filmsReader.GetString("given_name"),
                                    filmsReader.GetString("surname"),
                                     filmsReader.GetString("user_type"),
                                    filmsReader.GetString("tel_no"),
                                    filmsReader.GetString("email"),
                                    filmsReader.GetString("level_no") + "." + filmsReader.GetString("room_no"),
                                    "Assets/DarkGray.png",
                                    string.Empty,
                                    string.Empty,
                                    group));


                                }
                            }
                        }
                        _contactDataSource.AllGroups.Clear();
                        _contactDataSource.AllGroups.Add(group);
                    }
                }
            }
        }


        public ContactDataSource()
        {
            using (MySqlConnection connection = new MySqlConnection("Server=150.203.139.164;Database=rptdb_prod;Uid=remote;Password=CASSAdmin;Connection Timeout=2200"))
            {
                connection.Open();

                // Category list
                MySqlCommand categoriesCommand = new MySqlCommand("SELECT property_cd, property_name FROM space_property WHERE property_cd='H013'", connection);
                using (MySqlDataReader reader = categoriesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SampleDataGroup group = new SampleDataGroup(reader.GetString("property_cd"),
                          reader.GetString("property_name"),
                          "Assets/LightGray.png",
                          string.Empty);

                        // Film list by Category
                        using (MySqlConnection filmsConnection = (MySqlConnection)connection.Clone())
                        {
                            filmsConnection.Open();
                            MySqlCommand filmsCommand = new MySqlCommand(@"SELECT ifnull(uni_id,'') as uni_id,ifnull(given_name,'') as given_name,ifnull(surname,'') as surname,ifnull(user_type,'') as user_type,ifnull(tel_no,'') as tel_no,ifnull(email,'') as email,
                                                                           ifnull(level_no,'') as level_no,ifnull(room_no,'') as room_no FROM space_room_allocation",
                                                                            filmsConnection);
                            filmsCommand.Parameters.Add(new MySqlParameter("?id", reader.GetString("property_cd")));
                            using (MySqlDataReader filmsReader = filmsCommand.ExecuteReader())
                            {
                                while (filmsReader.Read())
                                {
                                    group.Items.Add(new SampleDataItem(filmsReader.GetString("uni_id"),
                                    filmsReader.GetString("given_name"),
                                    filmsReader.GetString("surname"),
                                     filmsReader.GetString("user_type"),
                                    filmsReader.GetString("tel_no"),
                                    filmsReader.GetString("email"),
                                    filmsReader.GetString("level_no") + "." + filmsReader.GetString("room_no"),
                                    "Assets/DarkGray.png",
                                    string.Empty,
                                    string.Empty,
                                    group));


                                }
                            }
                        }
                        this.AllGroups.Add(group);
                    }
                }
            }
        }
    }
}
