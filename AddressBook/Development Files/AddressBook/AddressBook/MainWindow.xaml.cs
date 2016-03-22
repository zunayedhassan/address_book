/* ****************************************************************** *\
 * APPLICATION: ADDRESSBOOK
 * VERSION:     1.0
 * ------------------------------------------------------------------
 * MAINTAINER:  Mohammad Zunayed Hassan
 * EMAIL:       zunayed-hassan@live.com
\* ****************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using Microsoft.Win32;
using System.IO;

namespace AddressBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int STANDARD_ICON_SIZE = 32;
        private static int STANDARD_USER_PICTURE_SIZE = 100;
        private static string DEFAULT_USER_PICTURE_IMAGE = "Content/default-user.png";
        private ToolBarButton addButton, refreshButton, editButton, saveButton, deleteButton, searchButton, helpButton;
        private DataGrid addressBookDataGrid;
        private StackPanel userPictureStackPanel = new StackPanel();
        private TextBlock pictureHeaderTextBlock = new TextBlock();
        private Canvas browseImageCanvas;
        private TextBox imageSourceTextBox;
        private Button browseButton;
        private Window searchWindow;
        private ComboBox searchByComboBox;
        private TextBox searchForTextBox;
        private Button searchForButton, closeButton;

        private CustomLabel firstNameLabel,
                            lastNameLabel,
                            groupLabel,
                            defaultPhoneNoLabel,
                            homePhoneNoLabel,
                            workPhoneNoLabel,
                            addressLabel,
                            emailLabel,
                            websiteLabel,
                            noteLabel;

        private CustomTextBox firstNameTextBox,
                              lastNameTextBox,
                              defaultPhoneNoTextBox,
                              homePhoneNoTextBox,
                              workPhoneNoTextBox,
                              addressTextBox,
                              emailTextBox,
                              websiteTextBox,
                              noteTextBox;

        private ComboBox groupComboBox;
        private List<Contact> contacts = new List<Contact>();
        private List<string> imageUrl = new List<string>();
        private OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=MyContactDatabase.accdb;Jet OLEDB:Database Password=noromeo;");
        private bool edit = false;

        public MainWindow()
        {
            InitializeComponent();

            // Window: MainWindow
            this.Title = "Address Book";
            this.Width = 1024;
            this.Height = 600;

            try
            {
                this.Icon = BitmapFrame.Create(new Uri(@"Contents/icon.ico", UriKind.RelativeOrAbsolute));
            }
            catch (FileNotFoundException)
            {
                // Do nothing
            }

            // DockPanel: mainDockPanel
            DockPanel mainDockPanel = new DockPanel();
            mainDockPanel.Background = new SolidColorBrush(Colors.LightGray);

            // Grid: toolBarGrid
            Grid toolBarGrid = new Grid();
            toolBarGrid.VerticalAlignment = VerticalAlignment.Top;
            for (int i = 1; i <= 2; i++)
                toolBarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            DockPanel.SetDock(toolBarGrid, Dock.Top);

            // ToolBar: mainToolBar
            ToolBar mainToolBar = new ToolBar();
            mainToolBar.Background = new SolidColorBrush(Colors.LightGray);
            mainToolBar.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetColumn(mainToolBar, 0);

            // ToolBarButton: addButton, refreshButton, editButton, saveButton, deleteButton, searchButton
            addButton = new ToolBarButton(new CustomImage("Content/add-contact.png", STANDARD_ICON_SIZE), "Add");
            refreshButton = new ToolBarButton(new CustomImage("Content/view-refresh.png", STANDARD_ICON_SIZE), "Refresh");
            editButton = new ToolBarButton(new CustomImage("Content/document-edit.png", STANDARD_ICON_SIZE), "Edit");
            saveButton = new ToolBarButton(new CustomImage("Content/document-save.png", STANDARD_ICON_SIZE), "Save");
            deleteButton = new ToolBarButton(new CustomImage("Content/edit-delete.png", STANDARD_ICON_SIZE), "Delete");
            searchButton = new ToolBarButton(new CustomImage("Content/system-search.png", STANDARD_ICON_SIZE), "Search");

            // ToolBar: helpToolBar
            ToolBar helpToolBar = new ToolBar();
            helpToolBar.Background = new SolidColorBrush(Colors.LightGray);
            helpToolBar.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(helpToolBar, 1);

            // ToolBarButton: helpButton
            helpButton = new ToolBarButton(new CustomImage("Content/system-help.png", STANDARD_ICON_SIZE), "Help");

            // Grid: bodyGrid
            Grid bodyGrid = new Grid();
            bodyGrid.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            //bodyGrid.ShowGridLines = true;
            for (int i = 1; i <= 50; i++)
                bodyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 1; i <= 50; i++)
                bodyGrid.RowDefinitions.Add(new RowDefinition());

            // DataGrid: addressBookDataGrid
            addressBookDataGrid = new DataGrid();
            addressBookDataGrid.Background = new SolidColorBrush(Colors.LightGray);
            addressBookDataGrid.IsReadOnly = true;
            addressBookDataGrid.AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(190, 220, 255));
            Grid.SetColumn(addressBookDataGrid, 1);
            Grid.SetColumnSpan(addressBookDataGrid, 33);
            Grid.SetRow(addressBookDataGrid, 1);
            Grid.SetRowSpan(addressBookDataGrid, 46);

            // StackPanel: userPictureStackPanel
            userPictureStackPanel.Background = new SolidColorBrush(Color.FromRgb(220, 230, 245));
            Grid.SetColumn(userPictureStackPanel, 35);
            Grid.SetColumnSpan(userPictureStackPanel, 15);
            Grid.SetRow(userPictureStackPanel, 0);
            Grid.SetRowSpan(userPictureStackPanel, 20);

            // Canvas: browseImageCanvas
            browseImageCanvas = new Canvas();
            browseImageCanvas.Width = 200;
            browseImageCanvas.Height = 80;

            // Label: imageSourceLabel
            Label imageSourceLabel = new Label();
            imageSourceLabel.Content = "Image source:";
            imageSourceLabel.Margin = new Thickness(20, 10, 0, 0);

            // imageSourceTextBox
            imageSourceTextBox = new TextBox();
            imageSourceTextBox.Text = string.Empty;
            imageSourceTextBox.Width = 130;
            imageSourceTextBox.Margin = new Thickness(23, 35, 0, 0);

            // Button: browseButton
            browseButton = new Button();
            browseButton.Content = "...";
            browseButton.MinWidth = 30;
            browseButton.Margin = new Thickness(160, 35, 0, 0);

            // ScrollViewer: userProfileScrollViewer
            ScrollViewer userProfileScrollViewer = new ScrollViewer();
            userProfileScrollViewer.Background = new SolidColorBrush(Color.FromRgb(233, 233, 235));
            Grid.SetColumn(userProfileScrollViewer, 35);
            Grid.SetColumnSpan(userProfileScrollViewer, 15);
            Grid.SetRow(userProfileScrollViewer, 20);
            Grid.SetRowSpan(userProfileScrollViewer, 28);

            // Canvas: userProfileCanvas
            Canvas userProfileCanvas = new Canvas();
            userProfileCanvas.Height = 500;

            // CustomLabel: firstNameLabel, lastNameLabel, groupLabel, defaultPhoneNoLabel, homePhoneNoLabel, workPhoneNoLabel, addressLabel, emailLabel, websiteLabel, noteLabel
            firstNameLabel = new CustomLabel("First Name:", 20);
            lastNameLabel = new CustomLabel("Last Name:", 50);
            groupLabel = new CustomLabel("Group", 80);
            defaultPhoneNoLabel = new CustomLabel("Default Phone No:", 110);
            homePhoneNoLabel = new CustomLabel("Home Phone No:", 140);
            workPhoneNoLabel = new CustomLabel("Work Phone No:", 170);
            addressLabel = new CustomLabel("Address:", 200);
            emailLabel = new CustomLabel("Email:", 310);
            websiteLabel = new CustomLabel("Website:", 340);
            noteLabel = new CustomLabel("Note:", 370);

            // CustomTextBox firstNameTextBox, lastNameTextBox, defaultPhoneNoTextBox, homePhoneNoTextBox, workPhoneNoTextBox, addressTextBox, emailTextBox, websiteTextBox, noteTextBox;
            firstNameTextBox = new CustomTextBox(25);
            lastNameTextBox = new CustomTextBox(55);
            defaultPhoneNoTextBox = new CustomTextBox(115);
            homePhoneNoTextBox = new CustomTextBox(145);
            workPhoneNoTextBox = new CustomTextBox(175);

            addressTextBox = new CustomTextBox(205);
            addressTextBox.AcceptsReturn = true;
            addressTextBox.Height = 100;
            addressTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            addressTextBox.TextWrapping = TextWrapping.Wrap;

            emailTextBox = new CustomTextBox(315);
            websiteTextBox = new CustomTextBox(345);

            noteTextBox = new CustomTextBox(375);
            noteTextBox.Background = new SolidColorBrush(Color.FromRgb(252, 254, 197));
            noteTextBox.AcceptsReturn = true;
            noteTextBox.Height = 110;
            noteTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            noteTextBox.TextWrapping = TextWrapping.Wrap;
            noteTextBox.FontFamily = new FontFamily("Comic Sans MS");
            noteTextBox.FontStyle = FontStyles.Oblique;

            // ComboBox: groupComboBox
            groupComboBox = new ComboBox();
            groupComboBox.Width = 130;
            groupComboBox.Margin = new Thickness(140, 85, 0, 0);
            groupComboBox.SelectedIndex = 0;
            

            // StatusBar: statusBar
            StatusBar statusBar = new StatusBar();
            statusBar.Background = new SolidColorBrush(Colors.LightGray);
            Grid.SetColumn(statusBar, 0);
            Grid.SetColumnSpan(statusBar, 50);
            Grid.SetRow(statusBar, 48);
            Grid.SetRowSpan(statusBar, 2);

            // Adding components
            this.Content = mainDockPanel;
            mainDockPanel.Children.Add(toolBarGrid);
            toolBarGrid.Children.Add(mainToolBar);
            mainToolBar.Items.Add(addButton);
            mainToolBar.Items.Add(refreshButton);
            mainToolBar.Items.Add(new Separator());
            mainToolBar.Items.Add(editButton);
            mainToolBar.Items.Add(saveButton);
            mainToolBar.Items.Add(deleteButton);
            mainToolBar.Items.Add(new Separator());
            mainToolBar.Items.Add(searchButton);
            toolBarGrid.Children.Add(helpToolBar);
            helpToolBar.Items.Add(helpButton);
            mainDockPanel.Children.Add(bodyGrid);
            bodyGrid.Children.Add(addressBookDataGrid);
            bodyGrid.Children.Add(userPictureStackPanel);
            userPictureStackPanel.Children.Add(pictureHeaderTextBlock);
            userPictureStackPanel.Children.Add(new CustomImage(DEFAULT_USER_PICTURE_IMAGE, STANDARD_USER_PICTURE_SIZE));
            userPictureStackPanel.Children.Add(browseImageCanvas);
            browseImageCanvas.Children.Add(imageSourceLabel);
            browseImageCanvas.Children.Add(imageSourceTextBox);
            browseImageCanvas.Children.Add(browseButton);
            bodyGrid.Children.Add(userProfileScrollViewer);
            userProfileScrollViewer.Content = userProfileCanvas;
            userProfileCanvas.Children.Add(firstNameLabel);
            userProfileCanvas.Children.Add(lastNameLabel);
            userProfileCanvas.Children.Add(groupLabel);
            userProfileCanvas.Children.Add(defaultPhoneNoLabel);
            userProfileCanvas.Children.Add(homePhoneNoLabel);
            userProfileCanvas.Children.Add(workPhoneNoLabel);
            userProfileCanvas.Children.Add(addressLabel);
            userProfileCanvas.Children.Add(emailLabel);
            userProfileCanvas.Children.Add(websiteLabel);
            userProfileCanvas.Children.Add(noteLabel);
            userProfileCanvas.Children.Add(firstNameTextBox);
            userProfileCanvas.Children.Add(lastNameTextBox);
            userProfileCanvas.Children.Add(groupComboBox);
            groupComboBox.Items.Add("No Group");
            groupComboBox.Items.Add("Friend");
            groupComboBox.Items.Add("Family");
            groupComboBox.Items.Add("Acquaintance");
            userProfileCanvas.Children.Add(defaultPhoneNoTextBox);
            userProfileCanvas.Children.Add(homePhoneNoTextBox);
            userProfileCanvas.Children.Add(workPhoneNoTextBox);
            userProfileCanvas.Children.Add(addressTextBox);
            userProfileCanvas.Children.Add(emailTextBox);
            userProfileCanvas.Children.Add(websiteTextBox);
            userProfileCanvas.Children.Add(noteTextBox);
            bodyGrid.Children.Add(statusBar);

            refreshDataGrid();
            enableOrDisableComponents(true, true, false, false, false, true, false, false);
            enableUserProfileContents(false);

            // Adding EventHandler
            refreshButton.Click += new RoutedEventHandler(refreshButton_Click);
            addressBookDataGrid.SelectionChanged += new SelectionChangedEventHandler(addressBookDataGrid_SelectionChanged);
            addButton.Click += new RoutedEventHandler(addButton_Click);
            editButton.Click += new RoutedEventHandler(editButton_Click);
            saveButton.Click += new RoutedEventHandler(saveButton_Click);
            deleteButton.Click += new RoutedEventHandler(deleteButton_Click);
            searchButton.Click += new RoutedEventHandler(searchButton_Click);
            helpButton.Click += new RoutedEventHandler(helpButton_Click);
            browseButton.Click += new RoutedEventHandler(browseButton_Click);
        }

        private void refreshDataGrid()
        {
            contacts.Clear();
            imageUrl.Clear();

            try
            {
                connection.Open();
                OleDbDataReader reader = (new OleDbCommand("SELECT * FROM Contacts;", connection)).ExecuteReader();                           // executes query

                while (reader.Read()) // if can read row from database
                {
                    contacts.Add(new Contact()
                                     {
                                         Default_Phone_Number = reader.GetValue(0).ToString(),
                                         First_Name = reader.GetValue(1).ToString(),
                                         Last_Name = reader.GetValue(2).ToString(),
                                         Address = reader.GetValue(3).ToString(),
                                         Home_Phone_No = reader.GetValue(4).ToString(),
                                         Work_Phone_No = reader.GetValue(5).ToString(),
                                         Email = reader.GetValue(6).ToString(),
                                         Website = reader.GetValue(7).ToString(),
                                         Group = reader.GetValue(9).ToString(),
                                         Description = reader.GetValue(10).ToString()
                                     });

                    imageUrl.Add(reader.GetValue(8).ToString());
                }

                addressBookDataGrid.ItemsSource = contacts;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't establish connection.\n\n" + exception, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                connection.Close();
                addressBookDataGrid.Items.Refresh();
                refreshUserImage(DEFAULT_USER_PICTURE_IMAGE);
            }
        }

        private void refreshUserImage(string imageLink)
        {
            userPictureStackPanel.Children.Clear();
            userPictureStackPanel.Children.Add(pictureHeaderTextBlock);

            try
            {
                if (imageLink != DEFAULT_USER_PICTURE_IMAGE)
                {
                    userPictureStackPanel.Children.Add(new CustomImage(imageLink, STANDARD_USER_PICTURE_SIZE));
                    imageSourceTextBox.Text = imageUrl[addressBookDataGrid.SelectedIndex];
                }
                else if (imageLink == string.Empty | imageLink == DEFAULT_USER_PICTURE_IMAGE)
                {
                    userPictureStackPanel.Children.Add(new CustomImage(DEFAULT_USER_PICTURE_IMAGE, STANDARD_USER_PICTURE_SIZE));
                    imageSourceTextBox.Text = string.Empty;
                }
            }
            catch (Exception)
            {
                imageSourceTextBox.Text = string.Empty;
            }

            userPictureStackPanel.Children.Add(browseImageCanvas);
        }

        private void enableOrDisableComponents(bool status1, bool status2, bool status3, bool status4, bool status5, bool status6, bool status7, bool status8)
        {
            addButton.IsEnabled = status1;
            refreshButton.IsEnabled = status2;
            editButton.IsEnabled = status3;
            saveButton.IsEnabled = status4;
            deleteButton.IsEnabled = status5;
            searchButton.IsEnabled = status6;
            imageSourceTextBox.IsEnabled = status7;
            browseButton.IsEnabled = status8;
        }

        private void enableUserProfileContents(bool status)
        {
            firstNameTextBox.IsEnabled = status;
            lastNameTextBox.IsEnabled = status;
            groupComboBox.IsEnabled = status;
            defaultPhoneNoTextBox.IsEnabled = status;
            homePhoneNoTextBox.IsEnabled = status;
            workPhoneNoTextBox.IsEnabled = status;
            addressTextBox.IsEnabled = status;
            emailTextBox.IsEnabled = status;
            websiteTextBox.IsEnabled = status;
            noteTextBox.IsReadOnly = !status;
        }

        private void clearUserProfileContent()
        {
            firstNameTextBox.Text = string.Empty;
            lastNameTextBox.Text = string.Empty;
            groupComboBox.SelectedIndex = 0;
            defaultPhoneNoTextBox.Text = string.Empty;
            homePhoneNoTextBox.Text = string.Empty;
            workPhoneNoTextBox.Text = string.Empty;
            addressTextBox.Text = string.Empty;
            emailTextBox.Text = string.Empty;
            websiteTextBox.Text = string.Empty;
            noteTextBox.Text = string.Empty;
        }

        private void addressBookDataGrid_SelectionChanged(object sender, EventArgs evnt)
        {
            Contact currentContact = (Contact) addressBookDataGrid.SelectedItem;

            try
            {
                if (imageUrl[addressBookDataGrid.SelectedIndex] != string.Empty)
                    refreshUserImage(imageUrl[addressBookDataGrid.SelectedIndex]);
                else
                    refreshUserImage(DEFAULT_USER_PICTURE_IMAGE);

                firstNameTextBox.Text = currentContact.First_Name;
                lastNameTextBox.Text = currentContact.Last_Name;
                groupComboBox.SelectedItem = currentContact.Group;
                defaultPhoneNoTextBox.Text = currentContact.Default_Phone_Number;
                homePhoneNoTextBox.Text = currentContact.Home_Phone_No;
                workPhoneNoTextBox.Text = currentContact.Work_Phone_No;
                addressTextBox.Text = currentContact.Address;
                emailTextBox.Text = currentContact.Email;
                websiteTextBox.Text = currentContact.Website;
                noteTextBox.Text = currentContact.Description;
            }
            catch (Exception)
            {
                refreshUserImage(DEFAULT_USER_PICTURE_IMAGE);
                clearUserProfileContent();
            }

            enableOrDisableComponents(true, true, true, false, true, true, false, false); 
        }

        private void addButton_Click(object sender, EventArgs evnt)
        {
            refreshDataGrid();
            addressBookDataGrid.IsEnabled = false;
            imageSourceTextBox.Text = string.Empty;
            enableOrDisableComponents(false, false, false, true, false, false, true, true);
            enableUserProfileContents(true);
        }

        private void refreshButton_Click(object sender, EventArgs evnt)
        {
            refreshDataGrid();
            enableOrDisableComponents(true, true, false, false, false, true, false, false);
            imageSourceTextBox.Text = string.Empty;
        }

        private void editButton_Click(object sender, EventArgs evnt)
        {
            edit = true;
            enableOrDisableComponents(false, false, false, true, false, false, true, true);
            enableUserProfileContents(true);
        }

        private void saveButton_Click(object sender, EventArgs evnt)
        {
            bool flag = false; 

            try
            {
                connection.Open();
                OleDbCommand command;

                if (edit)
                {
                    command =
                        new OleDbCommand(
                            "UPDATE Contacts SET DefaultPhoneNumber = '" + defaultPhoneNoTextBox.Text.Trim() +
                            "', FirstName = '" + firstNameTextBox.Text.Trim() + "', LastName = '" +
                            lastNameTextBox.Text.Trim() + "', Address = '" + addressTextBox.Text.Trim() +
                            "', HomePhoneNumber = '" + homePhoneNoTextBox.Text.Trim() + "', WorkPhoneNumber = '" +
                            workPhoneNoTextBox.Text.Trim() + "', Email = '" + emailTextBox.Text.Trim() +
                            "', Website = '" + websiteTextBox.Text.Trim() + "', ImageURL = '" +
                            imageSourceTextBox.Text.Trim() + "', GroupType = '" + groupComboBox.SelectedItem +
                            "', Description = '" + noteTextBox.Text.Trim() + "' WHERE DefaultPhoneNumber = '" +
                            ((Contact)addressBookDataGrid.SelectedItem).Default_Phone_Number + "';", connection);

                    edit = false;

                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure to add this contact?", "Confirmation", MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        command =
                        new OleDbCommand(
                            "INSERT INTO Contacts (DefaultPhoneNumber, FirstName, LastName, Address, HomePhoneNumber, WorkPhoneNumber, Email, Website, ImageURL, GroupType, Description) VALUES ('" +
                            defaultPhoneNoTextBox.Text.Trim() + "', '" + firstNameTextBox.Text.Trim() + "', '" +
                            lastNameTextBox.Text.Trim() + "', '" + addressTextBox.Text.Trim() + "', '" +
                            homePhoneNoTextBox.Text.Trim() + "', '" + workPhoneNoTextBox.Text.Trim() + "', '" +
                            emailTextBox.Text.Trim() + "', '" + websiteTextBox.Text.Trim() + "', '" +
                            imageSourceTextBox.Text.Trim() + "', '" + groupComboBox.SelectedItem + "', '" +
                            noteTextBox.Text.Trim() + "');", connection);

                        command.ExecuteNonQuery();
                        command.Dispose();
                    }
                    else
                    {
                        flag = true;
                    }
                }
                
            }
            catch (Exception exception)
            {
                if (!flag)
                {
                    MessageBox.Show("You must input at least Default Phone Number and Last Name.\n\n" + exception,
                                    "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            finally
            {
                connection.Close();
                addressBookDataGrid.IsEnabled = true;
                refreshDataGrid();
                enableOrDisableComponents(true, true, false, false, false, true, false, false);
                enableUserProfileContents(false);
                imageSourceTextBox.Text = string.Empty;
                clearUserProfileContent();
            }
        }

        private void deleteButton_Click(object sender, EventArgs evnt)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure that you wan to delete this contact?",
                                                      "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand("DELETE * FROM Contacts WHERE DefaultPhoneNumber = '" + ((Contact)addressBookDataGrid.SelectedItem).Default_Phone_Number + "';", connection);
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can't establish connection.\n\n" + exception, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                    refreshDataGrid();
                    enableUserProfileContents(false);
                    imageSourceTextBox.Text = string.Empty;
                }
            }
            else
            {
                refreshDataGrid();
                imageSourceTextBox.Text = string.Empty;
            }

            enableOrDisableComponents(true, true, false, false, false, true, false, false);
        }

        private void searchButton_Click(object sender, EventArgs evnt)
        {
            searchWindow = new Window();
            searchWindow.Title = "Search Contact";
            searchWindow.Width = 420;
            searchWindow.Height = 135;
            searchWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            searchWindow.WindowStyle = WindowStyle.ToolWindow;
            searchWindow.ResizeMode = ResizeMode.NoResize;
            searchWindow.Topmost = true;

            try
            {
                searchWindow.Icon = BitmapFrame.Create(new Uri(@"Contents/search_icon.ico", UriKind.RelativeOrAbsolute));
            }
            catch (FileNotFoundException)
            {
                // Do nothing
            }

            Canvas searchCanvas = new Canvas();
            searchCanvas.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));

            searchByComboBox = new ComboBox();
            searchByComboBox.Width = 120;
            searchByComboBox.Margin = new Thickness(20);
            searchByComboBox.SelectedIndex = 1;

            Label searchForLabel = new Label();
            searchForLabel.Content = "Search for:";
            searchForLabel.Margin = new Thickness(150, 18, 0, 0);

            searchForTextBox = new TextBox();
            searchForTextBox.Width = 150;
            searchForTextBox.Margin = new Thickness(230, 22, 0, 0);

            closeButton = new Button();
            closeButton.Content = "Close";
            closeButton.Width = 70;
            closeButton.Height = 25;
            closeButton.Margin = new Thickness(230, 60, 0, 0);

            searchForButton = new Button();
            searchForButton.Content = "Search";
            searchForButton.Width = 70;
            searchForButton.Height = 25;
            searchForButton.Margin = new Thickness(310, 60, 0, 0);

            searchWindow.Content = searchCanvas;
            searchCanvas.Children.Add(searchByComboBox);
            searchByComboBox.Items.Add("Name");
            searchByComboBox.Items.Add("Email");
            searchByComboBox.Items.Add("Phone Number");
            searchByComboBox.Items.Add("Group");
            searchByComboBox.Items.Add("Address");
            searchCanvas.Children.Add(searchForLabel);
            searchCanvas.Children.Add(searchForTextBox);
            searchCanvas.Children.Add(closeButton);
            searchCanvas.Children.Add(searchForButton);

            searchWindow.Show();

            closeButton.Click += new RoutedEventHandler(closeButton_Click);
            searchForButton.Click += new RoutedEventHandler(searchForButton_Click);
        }

        private void closeButton_Click(object sender, EventArgs evnt)
        {
            searchWindow.Close();
        }

        private void searchForButton_Click(object sender, EventArgs evnt)
        {
            if (searchForTextBox.Text.Trim() != string.Empty)
            {
                List<Contact> currentContactForSearch = new List<Contact>();

                try
                {
                    string sqlSyntex = string.Empty;

                    switch (searchByComboBox.SelectedItem.ToString())
                    {
                        case "Name":
                            sqlSyntex = "SELECT * FROM Contacts WHERE FirstName LIKE '%" + searchForTextBox.Text.Trim() +
                                        "%' OR LastName LIKE '%" + searchForTextBox.Text.Trim() + "%';";
                            break;

                        case "Email":
                            sqlSyntex = "SELECT * FROM Contacts WHERE Email LIKE '%" + searchForTextBox.Text.Trim() + "%';";
                            break;

                        case "Phone Number":
                            sqlSyntex = "SELECT * FROM Contacts WHERE DefaultPhoneNumber LIKE '%" +
                                        searchForTextBox.Text.Trim() + "%' OR HomePhoneNumber LIKE '%" +
                                        searchForTextBox.Text.Trim() + "%' OR WorkPhoneNumber LIKE '%" +
                                        searchForTextBox.Text.Trim() + "%';";
                            break;
                            
                        case "Group":
                            sqlSyntex = "SELECT * FROM Contacts WHERE GroupType LIKE '%" + searchForTextBox.Text.Trim() +
                                        "%';";
                            break;

                        case "Address":
                            sqlSyntex = "SELECT * FROM Contacts WHERE Address LIKE '%" + searchForTextBox.Text.Trim() +
                                        "%';";
                            break;
                    }

                    imageUrl.Clear();
                    connection.Open();
                    OleDbDataReader reader = (new OleDbCommand(sqlSyntex, connection)).ExecuteReader();                           // executes query

                    while (reader.Read()) // if can read row from database
                    {
                        currentContactForSearch.Add(new Contact()
                        {
                            Default_Phone_Number = reader.GetValue(0).ToString(),
                            First_Name = reader.GetValue(1).ToString(),
                            Last_Name = reader.GetValue(2).ToString(),
                            Address = reader.GetValue(3).ToString(),
                            Home_Phone_No = reader.GetValue(4).ToString(),
                            Work_Phone_No = reader.GetValue(5).ToString(),
                            Email = reader.GetValue(6).ToString(),
                            Website = reader.GetValue(7).ToString(),
                            Group = reader.GetValue(9).ToString(),
                            Description = reader.GetValue(10).ToString()
                        });

                        imageUrl.Add(reader.GetValue(8).ToString());
                    }

                    addressBookDataGrid.ItemsSource = currentContactForSearch;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can't establish connection.\n\n" + exception, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                }
                finally
                {
                    connection.Close();
                    addressBookDataGrid.Items.Refresh();
                    imageSourceTextBox.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("At least type something to search.", "Warning", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                searchForTextBox.Focus();
            }
        }

        private void helpButton_Click(object sender, EventArgs evnt)
        {
            try
            {
                System.Diagnostics.Process.Start("User Guide.chm");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("User Guide.chm file can not be found.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void browseButton_Click(object sender, EventArgs evnt)
        {
            OpenFileDialog browseForImageFileDialog = new OpenFileDialog();
            browseForImageFileDialog.Filter = "Bitmap Files (.bmp)|*.bmp|Joint Picture Expert Group (.jpg)|*.jpg|Portable Network Graphics (.png)|*.png|Graphical Interchange Format (.gif)|*.gif|Windows Icon File (.ico)|*.ico|All Files (*.*)|*.*";
            browseForImageFileDialog.FilterIndex = 6;
            try
            {
                browseForImageFileDialog.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("That image file might be used by another program!", "ERROR", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                string fileName = browseForImageFileDialog.FileName;

                if ((fileName.Trim() != string.Empty) && ((System.IO.Path.GetExtension(fileName).ToLower() == ".bmp") | (System.IO.Path.GetExtension(fileName).ToLower() == ".jpg") | (System.IO.Path.GetExtension(fileName).ToLower() == ".jpeg") | (System.IO.Path.GetExtension(fileName).ToLower() == ".png") | (System.IO.Path.GetExtension(fileName).ToLower() == ".gif") | (System.IO.Path.GetExtension(fileName).ToLower() == ".ico")))
                {
                    refreshUserImage(fileName);
                    imageSourceTextBox.Text = fileName;
                }
                else
                {
                    MessageBox.Show("Please, select correct image file format.", "Warning", MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
        }
    }
}
