using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using ListBox = System.Windows.Forms.ListBox;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace FinalProjectTemplate1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const int TAB_MAIN_MENU = 0;
		private const int TAB_BOOK_RENTAL = 1;
		private const int TAB_BOOK_MANAGEMENT = 2;
		private const int TAB_CUSTOMER_MANAGEMENT = 3;
		private const int TAB_REGISTER = 4;
		private const int TAB_BOOK_DETAILS = 5;
		private const int TAB_CUSTOMER_DETAILS = 6;

		private bool dirtyDatabase = false;


		private static bool RegisterPopulated = false;
		private static bool loaded = false;
		private static bool BooksAdded = false;

		private static Book[] Books;
		private static LinkedList<Customer> Customers = new LinkedList<Customer>();
		private static LinkedList<Customer> CustomerSearchResults = new LinkedList<Customer>();
		public MainWindow()
		{
			FindBooks();
			FindCustomers();
			InitializeComponent();
		}

		private void MySampleBtn_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void SignInBtn_OnClick(object sender, RoutedEventArgs e)
		{
			if (EmailLogInText.Text.Equals("Admin") && PasswordLogInText.Password.Equals("Admin"))
			{
				SignInBtn.Visibility = Visibility.Collapsed;
				CancelBtn.Visibility = Visibility.Collapsed;
				EmailLogInLabel.Visibility = Visibility.Collapsed;
				PasswordLogInLabel.Visibility = Visibility.Collapsed;
				EmailLogInText.Visibility = Visibility.Collapsed;
				PasswordLogInText.Visibility = Visibility.Collapsed;
				SaveDatabaseBtn.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Email and Password do not match a Registered Admin");
			}
		}

		private void SaveDatabaseBtn_OnClick(object sender, RoutedEventArgs e)
		{
			if (!dirtyDatabase)
			{
				MessageBox.Show("Everything is up to date, nothing to update");
				return;
			}
			try
			{
				string CustomersOutputFilePath = "../../Resources/customers.csv";
				string BooksOutputFilePath = "../../Resources/books.csv";
				string recordStr = "";

				StreamWriter BookstreamWriter = new StreamWriter(BooksOutputFilePath);
				foreach (Book book in Books)
				{
					recordStr = "";
					recordStr += book.ID + ",";
					recordStr += book.Title + ",";
					recordStr += book.ISBN + ",";
					recordStr += book.Price + ",";
					recordStr += book.WeeklyRentCost + ",";
					recordStr += book.DailyRentCost + ",";
					recordStr += book.PageCount + ",";
					recordStr += book.Description + ",";
					recordStr += book.Author + ",";
					recordStr += book.Category + ",";
					recordStr += book.IsCheckedOut + ",";
					recordStr += string.IsNullOrEmpty(book.CheckOutDate.tostring()) ? "1900-01-01" : book.CheckOutDate.tostring() + ",";
					DateFormat df = new DateFormat(book.CheckOutDate.tostring());
					recordStr += df.AddDays(book.CheckOutDuration).tostring() + ",";
					recordStr += ".";
					BookstreamWriter.WriteLine(recordStr);
				}
				BookstreamWriter.Close();

				StreamWriter CustomertreamWriter = new StreamWriter(CustomersOutputFilePath);
				//for (int i = 0; i < Customers.Count; i++)//each (Customer customer in Customers)
				foreach (var customer in Customers)
				{
					recordStr = "";
					recordStr += customer.FirstName + ",";
					recordStr += customer.LastName + ",";
					recordStr += customer.Address + ",";
					recordStr += customer.City + ",";
					recordStr += customer.State.Substring(0, 2) + ",";
					recordStr += customer.ZipCode + ",";
					recordStr += customer.PhoneNumber + ",";
					recordStr += customer.Email + ",";
					recordStr += customer.Gender.Substring(0, 1) + ",";
					recordStr += customer.Password + ",";
					recordStr += customer.Employment.Substring(0, 1) + ",";
					recordStr += customer.FirstBook == null ? "0," : customer.FirstBook.ID + ",";
					recordStr += customer.SecondBook == null ? "0," : customer.SecondBook.ID + ",";
					recordStr += ".";
					CustomertreamWriter.WriteLine(recordStr);
				}
				CustomertreamWriter.Close();

				StatusNotificationLabel.Content = "Everything is Up To Date";
				StatusNotificationLabel.Background = System.Windows.Media.Brushes.Peru;
				StatusNotificationLabel.Foreground = System.Windows.Media.Brushes.LawnGreen;
			}
			catch (Exception exc)
			{

			}
			finally
			{

			}
		}

		private void EmailLogInText_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			EmailLogInText.Text = "";
		}

		private void PasswordLogInText_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			PasswordLogInText.Clear();
		}

		private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
		{
			EmailLogInText.Clear();
			PasswordLogInText.Clear();
			EmailLogInText.Focus();
		}

		private void SignUpBtn_OnClick(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_REGISTER));
			PopulateRegister();
		}

		private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void TemplateMethod(object sender, RoutedEventArgs e)
		{
			;
		}

		private void ClearTextBoxMethod(object sender, RoutedEventArgs e)
		{
			((TextBox)sender).Clear();
		}

		private void UpdateComboBoxMethod(int locator)
		{
			BookISBNBookManagementCombo.SelectedIndex = locator;
			BookTitleBookManagementCombo.SelectedIndex = locator;
			BookAuthorBookManagementCombo.SelectedIndex = locator;
			BookPriceBookManagementCombo.SelectedIndex = locator;
			BookIsRentedBookManagementCombo.SelectedIndex = locator;
			BookStartDateBookManagementCombo.SelectedIndex = locator;
			BookEndDateBookManagementCombo.SelectedIndex = locator;
			
			BookTitleBookManagementText.Text = string.IsNullOrEmpty(BookTitleBookManagementCombo.SelectedValue.ToString())? "" : BookTitleBookManagementCombo.SelectedValue.ToString();
			BookAuthorBookManagementText.Text = string.IsNullOrEmpty(BookAuthorBookManagementCombo.SelectedValue.ToString()) ? "" : BookAuthorBookManagementCombo.SelectedValue.ToString();
			BookPriceBookManagementText.Text = string.IsNullOrEmpty(BookPriceBookManagementCombo.SelectedValue.ToString()) ? "" : BookPriceBookManagementCombo.SelectedValue.ToString();
			BookIsRentedBookManagementText.Text = string.IsNullOrEmpty(BookIsRentedBookManagementCombo.SelectedValue.ToString()) ? "N" : BookIsRentedBookManagementCombo.SelectedValue.ToString();
			BookStartDateBookManagementText.Text = string.IsNullOrEmpty(BookStartDateBookManagementCombo.SelectedValue.ToString()) ? "" : BookStartDateBookManagementCombo.SelectedValue.ToString();
			BookEndDateBookManagementText.Text = string.IsNullOrEmpty(BookEndDateBookManagementCombo.SelectedValue.ToString()) ? "" : BookEndDateBookManagementCombo.SelectedValue.ToString();

			Books[locator].Title = BookTitleBookManagementText.Text;
			Books[locator].Author = BookAuthorBookManagementText.Text;
			Books[locator].Price = double.TryParse(BookPriceBookManagementText.Text, out double dblVal) ? dblVal : 0.0;
			Books[locator].IsCheckedOut = BookIsRentedBookManagementText.Text;
			Books[locator].CheckOutDate = new DateFormat(BookStartDateBookManagementText.Text);
			Books[locator].CheckOutDuration = Books[locator].CheckOutDate.DaysApart(new DateFormat(BookEndDateBookManagementText.Text));
		}
		

		private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			if (tabControl.SelectedIndex == TAB_BOOK_RENTAL)
			{
				PopulateBooks();
				PopulateRegister();
			}
			if(tabControl.SelectedIndex == TAB_BOOK_MANAGEMENT)
			{
				PopulateBooks();
			}

			if (tabControl.SelectedIndex == TAB_REGISTER)
			{
				Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_REGISTER));
				PopulateRegister();
			}

			if (tabControl.SelectedIndex == TAB_CUSTOMER_MANAGEMENT)
			{
				PhoneNumberCustomerText.Clear();
				LastNameCustomerText.Clear();
				LastNameCustomerRadio.IsChecked = false;
				PhoneNumberCustomerRadio.IsChecked = true;
				PhoneNumberCustomerLabel.Visibility = Visibility.Visible;
				PhoneNumberCustomerText.Visibility = Visibility.Visible;
				LastNameCustomerLabel.Visibility = Visibility.Collapsed;
				LastNameCustomerText.Visibility = Visibility.Collapsed;
				PhoneNumberCustomerRadioText.Background = System.Windows.Media.Brushes.Aquamarine;
				LastNameCustomerRadioText.Background = System.Windows.Media.Brushes.DimGray;
			}
		}

		private void PopulateBooks()
		{
			if (!BooksAdded)
			{
				BookISBNBookManagementCombo.Items.Clear();
				BookTitleBookManagementCombo.Items.Clear();
				BookAuthorBookManagementCombo.Items.Clear();
				BookPriceBookManagementCombo.Items.Clear();
				BookIsRentedBookManagementCombo.Items.Clear();
				BookStartDateBookManagementCombo.Items.Clear();
				BookEndDateBookManagementCombo.Items.Clear();

				foreach (Book book in Books)
				{
					BookISBNBookManagementCombo.Items.Add(book.ISBN.Length > 0 ? book.ISBN : "");
					RentBookByISBNCombo.Items.Add(book.ISBN.Length > 0 ? book.ISBN : "");
					BookTitleBookManagementCombo.Items.Add(book.Title.Length > 0 ? book.Title : "");
					RentBookByTitleCombo.Items.Add(book.Title.Length > 0 ? book.Title : "");
					BookAuthorBookManagementCombo.Items.Add(book.Author.Length > 0 ? book.Author : "");
					BookPriceBookManagementCombo.Items.Add(book.Price);
					if (book.IsCheckedOut.ToUpper().Equals("Y"))
					{
						// Temp DateFormat set to today
						DateFormat tempDate;
						BookIsRentedBookManagementCombo.Items.Add("Y");
						BookStartDateBookManagementCombo.Items.Add(book.CheckOutDate.tostring());
						tempDate = new DateFormat(book.CheckOutDate.tostring());
						tempDate = tempDate.AddDays(book.CheckOutDuration);
						BookEndDateBookManagementCombo.Items.Add(tempDate.tostring());
					}
					else
					{
						BookIsRentedBookManagementCombo.Items.Add("N");
						BookStartDateBookManagementCombo.Items.Add("1900-01-01");
						BookEndDateBookManagementCombo.Items.Add("1900-01-01");
					}
				}

				BooksAdded = true;
			}
			
			if (!loaded)
			{
				BookISBNBookManagementCombo.SelectedIndex = 0;
				BookTitleBookManagementCombo.SelectedIndex = 0;
				BookAuthorBookManagementCombo.SelectedIndex = 0;
				BookPriceBookManagementCombo.SelectedIndex = 0;
				BookStartDateBookManagementCombo.SelectedIndex = 0;
				BookEndDateBookManagementCombo.SelectedIndex = 0;
				BookIsRentedBookManagementCombo.SelectedIndex = 0;
				loaded = true;
			}
		}

		private void PopulateRegister()
		{
			if (!RegisterPopulated)
			{
				for (int i = 1; i < 10; i++)
				{
					for (int j = 0; j < 10; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							PhoneAreaCodeRegisterCombo.Items.Add(i + "" + j + "" + k);
							// PhoneAreaCodeRentalCombo.Items.Add(i + "" + j + "" + k);
						}
					}
				}
				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < 10; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							PhonePrefixRegisterCombo.Items.Add(i + "" + j + "" + k);
							// PhonePrefixRentalCombo.Items.Add(i + "" + j + "" + k);
						}
					}
				}
				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < 10; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							for (int m = 0; m < 10; m++)
							{
								PhoneLineNumberRegisterCombo.Items.Add(i + "" + j + "" + k + "" + m);
								// PhoneLineNumberRentalCombo.Items.Add(i + "" + j + "" + k + "" + m);
							}
						}
					}
				}

				foreach (Customer customer in Customers)
				{
					FirstLastNameRentalCombo.Items.Add(customer.LastName + " " + customer.LastName);
					PhoneAreaCodeRentalCombo.Items.Add(customer.PhoneNumber.Substring(0, 3));
					PhonePrefixRentalCombo.Items.Add(customer.PhoneNumber.Substring(4, 3));
					PhoneLineNumberRentalCombo.Items.Add(customer.PhoneNumber.Substring(8, 4));
				}

				PhoneAreaCodeRegisterCombo.SelectedIndex = 0;
				PhonePrefixRegisterCombo.SelectedIndex = 0;
				PhoneLineNumberRegisterCombo.SelectedIndex = 0;
				RegisterPopulated = true;
			}
		}

		private void ClearRegisterBtn_OnClick(object sender, RoutedEventArgs e)
		{
			FirstNameRegisterText.Clear();
			LastNameRegisterText.Clear();
			Address1RegisterText.Clear();
			CityRegisterText.Clear();
			StatesRegisterCombo.SelectedIndex = 0;
			ZipRegisterText.Clear();
			EmailRegisterText.Clear();
			PhoneAreaCodeRegisterCombo.SelectedIndex = 0;
			PhonePrefixRegisterCombo.SelectedIndex = 0;
			PhoneLineNumberRegisterCombo.SelectedIndex = 0;
			Password1RegisterText.Clear();
			Password2RegisterText.Clear();
			NoDiscountCustomerRadio.IsChecked = false;
			VeteranDiscountCustomerRadio.IsChecked = false;
			StudentDiscountCustomerRadio.IsChecked = false;
		}

		private void FindCustomers()
		{
			int customersCount = 0;
			string customersFilePath = "../../Resources/customers.csv";

			string line;
			try
			{
				//Pass the file path and file name to the StreamReader constructor
				StreamReader sr = new StreamReader(customersFilePath);

				line = sr.ReadLine();
				while (line != null)
				{
					customersCount++;
					line = sr.ReadLine();
				}
				
				sr.Close();
				if (customersCount > 0)
				{
					Customers = new LinkedList<Customer>();
				}
				
				//Pass the file path and file name to the StreamReader constructor
				sr = new StreamReader(customersFilePath);

				//Read the first line of text
				line = sr.ReadLine();

				//Continue to read until you reach end of file
				string[] lineValues;
				int entryTracker = 0;
				
				entryTracker = 0;
				while (line != null)
				{
					Customer customer = new Customer();
					//int j = 0;
					lineValues = line.Split(',');
					customer.FirstName = lineValues[0];
					customer.LastName = lineValues[1];
					customer.Address = lineValues[2];
					customer.City = lineValues[3];
					customer.State = lineValues[4];
					customer.ZipCode = lineValues[5];
					customer.PhoneNumber = lineValues[6];
					customer.Email = lineValues[7];
					customer.Gender = lineValues[8];
					customer.Password = lineValues[9];
					customer.Employment = lineValues[10];
					customer.FirstBook.ISBN = string.IsNullOrEmpty(lineValues[11]) ? "None" : lineValues[10];
					customer.SecondBook.ISBN = string.IsNullOrEmpty(lineValues[12]) ? "None" : lineValues[11];

					Customers.AddLast(customer);

					//Read the next line
					line = sr.ReadLine();

					entryTracker++;
				}

				//close the file
				sr.Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
			finally
			{
			}
		}

		private void FindBooks()
		{
			int booksCount = 0;
			string booksFilePath = "../../Resources/books.csv";

			string line;
			try
			{
				//Pass the file path and file name to the StreamReader constructor
				StreamReader sr = new StreamReader(booksFilePath);

				line = sr.ReadLine();
				while (line != null)
				{
					booksCount++;
					line = sr.ReadLine();
				}

				if (booksCount > 0)
				{
					Books = new Book[booksCount];
				}
				sr.Close();
				
				//Pass the file path and file name to the StreamReader constructor
				sr = new StreamReader(booksFilePath);
				
				//Read the first line of text
				line = sr.ReadLine();
				
				//Continue to read until you reach end of file
				string[] lineValues;
				int entryTracker = 0;

				for (int i = 0; i < booksCount; i++)
				{
					Books[entryTracker++] = new Book();
				}

				entryTracker = 0;
				while (line != null)
				{
					lineValues = line.Split(',');
					Books[entryTracker].ID = int.Parse(lineValues[0]);
					Books[entryTracker].Title = lineValues[1];
					Books[entryTracker].ISBN = lineValues[2];
					Books[entryTracker].Price = double.Parse(lineValues[3]);
					Books[entryTracker].WeeklyRentCost = double.Parse(lineValues[4]);
					Books[entryTracker].DailyRentCost = double.Parse(lineValues[5]);
					Books[entryTracker].PageCount = (lineValues[6].Length > 0 ? int.Parse(lineValues[6]) : 0);
					Books[entryTracker].Description = lineValues[7];
					Books[entryTracker].Author = lineValues[8];
					Books[entryTracker].Category = lineValues[9];
					Books[entryTracker].IsCheckedOut = lineValues[10];
					
					//Read the next line
					line = sr.ReadLine();

					entryTracker++;
				}

				//close the file
				sr.Close();
			}
			catch (Exception e)
			{

			}
			finally
			{
			}
		}

		private void AddBookManagementBtn_OnClick(object sender, RoutedEventArgs e)
		{
			BookISBNBookManagementCombo.Visibility = Visibility.Collapsed;
			BookTitleBookManagementCombo.Visibility = Visibility.Collapsed;
			BookAuthorBookManagementCombo.Visibility = Visibility.Collapsed;
			BookPriceBookManagementCombo.Visibility = Visibility.Collapsed;
			BookStartDateBookManagementCombo.Visibility = Visibility.Collapsed;
			BookEndDateBookManagementCombo.Visibility = Visibility.Collapsed;
			BookIsRentedBookManagementCombo.Visibility = Visibility.Collapsed;
			UpdateBookManagementBtn.Visibility = Visibility.Collapsed;
			DeleteBookManagementBtn.Visibility = Visibility.Collapsed;

			BookISBNBookManagementText.Visibility = Visibility.Visible;
            SaveBookManagementBtn.Visibility = Visibility.Visible;
            AddBookManagementBtn.Visibility = Visibility.Collapsed;

			BookISBNBookManagementText.SetValue(Grid.ColumnProperty, 5);
			BookTitleBookManagementText.SetValue(Grid.ColumnProperty, 5);
			BookAuthorBookManagementText.SetValue(Grid.ColumnProperty, 5);
			BookPriceBookManagementText.SetValue(Grid.ColumnProperty, 5);
			BookStartDateBookManagementText.SetValue(Grid.ColumnProperty, 5);
			BookEndDateBookManagementText.SetValue(Grid.ColumnProperty, 5);
			BookIsRentedBookManagementText.SetValue(Grid.ColumnProperty, 5);

			BookISBNBookManagementText.Clear();
			BookTitleBookManagementText.Clear();
			BookAuthorBookManagementText.Clear();
			BookPriceBookManagementText.Clear();
			BookStartDateBookManagementText.Clear();
			BookEndDateBookManagementText.Clear();
			BookIsRentedBookManagementText.Clear();

			UpdateBookManagementBtn.Visibility = Visibility.Collapsed;
			CancelBookManagementBtn.Visibility = Visibility.Visible;
		}

        private void SaveBookManagementBtn_OnClick(object sender, RoutedEventArgs e)
        {
			Book newBook = new Book();
			newBook.ID = Books.Length;
			newBook.ISBN = BookISBNBookManagementText.Text;
			newBook.Title = BookTitleBookManagementText.Text;
			newBook.Author = BookAuthorBookManagementText.Text;
			newBook.Price = Convert.ToDouble(BookPriceBookManagementText.Text);
			newBook.WeeklyRentCost = newBook.Price * 0.12;
			newBook.DailyRentCost = newBook.Price * 0.12 * 0.3;
			newBook.CheckOutDate = new DateFormat("1900-01-01");
			newBook.CheckOutDuration = 0;
			newBook.IsCheckedOut = "N";

			Book[] tempBooks = new Book[Books.Length + 1];

			for (int i = 0; i < Books.Length; i++)
			{
				tempBooks[i] = Books[i];
			}

			tempBooks[Books.Length] = newBook;

			Books = tempBooks;

			BooksAdded = false;
			PopulateBooks();
			BookISBNBookManagementCombo.SelectedIndex = 0;

			dirtyDatabase = true;

	        StatusNotificationLabel.Content = "Database is outdated, log in as admin and save your changes";
	        StatusNotificationLabel.Background = System.Windows.Media.Brushes.Maroon;
	        StatusNotificationLabel.Foreground = System.Windows.Media.Brushes.White;

			ButtonAutomationPeer peer = new ButtonAutomationPeer(CancelBookManagementBtn);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        private void UpdateBookManagementBtn_OnClick(object sender, RoutedEventArgs e)
		{
			BookISBNBookManagementCombo.Visibility = Visibility.Visible;
			BookTitleBookManagementCombo.Visibility = Visibility.Visible;
			BookAuthorBookManagementCombo.Visibility = Visibility.Visible;
			BookPriceBookManagementCombo.Visibility = Visibility.Visible;
			BookStartDateBookManagementCombo.Visibility = Visibility.Visible;
			BookEndDateBookManagementCombo.Visibility = Visibility.Visible;
			BookIsRentedBookManagementCombo.Visibility = Visibility.Visible;

			BookISBNBookManagementText.Visibility = Visibility.Collapsed;
			CancelBookManagementBtn.Visibility = Visibility.Collapsed;
			

			BookISBNBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookTitleBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookAuthorBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookPriceBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookStartDateBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookEndDateBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookIsRentedBookManagementText.SetValue(Grid.ColumnProperty, 12);

			string inputVal;
			int locator = BookISBNBookManagementCombo.SelectedIndex;
			if (!string.IsNullOrEmpty(BookTitleBookManagementText.Text))
			{
				Books[locator].Title = BookTitleBookManagementText.Text;
				BookTitleBookManagementCombo.SelectedValue = BookTitleBookManagementText.Text;
			}

			if (!string.IsNullOrEmpty(BookAuthorBookManagementText.Text))
			{
				Books[locator].Author = BookAuthorBookManagementText.Text;
				BookAuthorBookManagementCombo.SelectedValue = BookAuthorBookManagementText.Text;
			}
			if (double.TryParse(BookPriceBookManagementText.Text, out double tempVal))
			{
				Books[locator].Price = tempVal;
				BookPriceBookManagementCombo.SelectedValue = "" + tempVal;
			}
			inputVal = BookStartDateBookManagementText.Text;
			if (!string.IsNullOrEmpty(inputVal))
			{
				Regex dateRegex = new Regex(@"\d{4}-\d{2}-\d{2}");
				Match match = dateRegex.Match(inputVal);
				if (match.Success)
				{
					Books[locator].CheckOutDate = new DateFormat(inputVal);
					BookStartDateBookManagementCombo.SelectedValue = inputVal;

				}
			}
			inputVal = BookEndDateBookManagementText.Text;
			if (!string.IsNullOrEmpty(inputVal) && Books[locator].CheckOutDate != null)
			{
				Regex dateRegex = new Regex(@"\d{4}-\d{2}-\d{2}");
				Match match = dateRegex.Match(inputVal);
				if (match.Success)
				{
					DateFormat newDate = new DateFormat(inputVal);
					Books[locator].CheckOutDuration = newDate.DaysApart(Books[locator].CheckOutDate);
					BookEndDateBookManagementCombo.SelectedValue = inputVal;
				}
			}

			Books[locator].IsCheckedOut = BookIsRentedBookManagementText.Text.ToUpper().Equals("Y") ? "Y" : "N";
			BookIsRentedBookManagementCombo.SelectedItem = Books[locator].IsCheckedOut;

			BooksAdded = false;
			loaded = false;

			PopulateBooks();


			BookDetailsTextBox.Text += "\t\tBook Updated\n" + Books[locator].GetBookDetails() +
									   "\n====================================\n";
			StatusNotificationLabel.Content = "Database is outdated, log in as admin and save your changes";
			StatusNotificationLabel.Background = System.Windows.Media.Brushes.Maroon;
			StatusNotificationLabel.Foreground = System.Windows.Media.Brushes.White;
			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_BOOK_DETAILS));

        }

        private void DeleteBookManagementBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
	            Books[BookISBNBookManagementCombo.SelectedIndex] = null;
	            Book[] temp = new Book [Books.Length - 1];
	            for (int i = 0; i < Books.Length - 1; i++)
	            {
		            temp[i] = Books[i + 1];
	            }

	            Books = temp;
	            BooksAdded = false;
				PopulateBooks();
				BookISBNBookManagementCombo.SelectedIndex = 0;

				dirtyDatabase = true;

				StatusNotificationLabel.Content = "Database is outdated, log in as admin and save your changes";
				StatusNotificationLabel.Background = System.Windows.Media.Brushes.Maroon;
				StatusNotificationLabel.Foreground = System.Windows.Media.Brushes.White;

				System.Windows.MessageBox.Show("Deleted");
            }
            else
            {
                System.Windows.MessageBox.Show("Delete operation Terminated");
            }
        }

        private void CancelBookManagementBtn_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateBookManagementBtn.Visibility = Visibility.Visible;

			BookISBNBookManagementText.Clear();
			BookTitleBookManagementText.Clear();
			BookAuthorBookManagementText.Clear();
			BookPriceBookManagementText.Clear();
			BookStartDateBookManagementText.Clear();
			BookEndDateBookManagementText.Clear();
			BookIsRentedBookManagementText.Clear();


			BookISBNBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookTitleBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookAuthorBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookPriceBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookStartDateBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookEndDateBookManagementText.SetValue(Grid.ColumnProperty, 12);
			BookIsRentedBookManagementText.SetValue(Grid.ColumnProperty, 12);

			BookISBNBookManagementCombo.Visibility = Visibility.Visible;
			BookTitleBookManagementCombo.Visibility = Visibility.Visible;
			BookAuthorBookManagementCombo.Visibility = Visibility.Visible;
			BookPriceBookManagementCombo.Visibility = Visibility.Visible;
			BookStartDateBookManagementCombo.Visibility = Visibility.Visible;
			BookEndDateBookManagementCombo.Visibility = Visibility.Visible;
			BookIsRentedBookManagementCombo.Visibility = Visibility.Visible;
			DeleteBookManagementBtn.Visibility = Visibility.Visible;

			BookISBNBookManagementText.Visibility = Visibility.Collapsed;
			CancelBookManagementBtn.Visibility = Visibility.Collapsed;
            SaveBookManagementBtn.Visibility = Visibility.Collapsed;
            AddBookManagementBtn.Visibility = Visibility.Visible;
		}

        private void PhoneAreaCodeRentalComboClosedMethod(object sender, EventArgs e)
        {
			PhonePrefixRentalCombo.SelectedIndex = PhoneAreaCodeRentalCombo.SelectedIndex;
			PhoneLineNumberRentalCombo.SelectedIndex = PhoneAreaCodeRentalCombo.SelectedIndex;
			FirstLastNameRentalCombo.SelectedIndex = PhoneAreaCodeRentalCombo.SelectedIndex;
		}

        private void PhonePrefixRentalComboClosedMethod(object sender, EventArgs e)
		{
			PhoneAreaCodeRentalCombo.SelectedIndex = PhonePrefixRentalCombo.SelectedIndex;
			PhoneLineNumberRentalCombo.SelectedIndex = PhonePrefixRentalCombo.SelectedIndex;
			FirstLastNameRentalCombo.SelectedIndex = PhonePrefixRentalCombo.SelectedIndex;
		}

        private void PhoneLineNumberRentalComboClosedMethod(object sender, EventArgs e)
		{
			PhoneAreaCodeRentalCombo.SelectedIndex = PhoneLineNumberRentalCombo.SelectedIndex;
			PhonePrefixRentalCombo.SelectedIndex = PhoneLineNumberRentalCombo.SelectedIndex;
			FirstLastNameRentalCombo.SelectedIndex = PhoneLineNumberRentalCombo.SelectedIndex;
		}

        private void FirstLastNameRentalComboClosedMethod(object sender, EventArgs e)
		{
			PhoneAreaCodeRentalCombo.SelectedIndex = FirstLastNameRentalCombo.SelectedIndex;
			PhonePrefixRentalCombo.SelectedIndex = FirstLastNameRentalCombo.SelectedIndex;
			PhoneLineNumberRentalCombo.SelectedIndex = FirstLastNameRentalCombo.SelectedIndex;
		}

		private void BookISBNDropDownClosedMethod(object sender, EventArgs e)
        {
	        UpdateComboBoxMethod(BookISBNBookManagementCombo.SelectedIndex);
        }

		private void BookRentalISBNDropDownClosedMethod(object sender, EventArgs e)
		{
			RentBookByTitleCombo.SelectedIndex = RentBookByISBNCombo.SelectedIndex;
		}

		private void BookRentalTitleDropDownClosedMethod(object sender, EventArgs e)
		{
			RentBookByISBNCombo.SelectedIndex =	RentBookByTitleCombo.SelectedIndex;
		}

		private void BookTitleDropDownClosedMethod(object sender, EventArgs e)
		{
			UpdateComboBoxMethod(BookTitleBookManagementCombo.SelectedIndex);
		}

		private void BookAuthorDropDownClosedMethod(object sender, EventArgs e)
		{
			UpdateComboBoxMethod(BookAuthorBookManagementCombo.SelectedIndex);
		}

		private void BookPriceDropDownClosedMethod(object sender, EventArgs e)
		{
			UpdateComboBoxMethod(BookPriceBookManagementCombo.SelectedIndex);
		}

		private void BookIsRentedDropDownClosedMethod(object sender, EventArgs e)
		{
			UpdateComboBoxMethod(BookIsRentedBookManagementCombo.SelectedIndex);
		}

		private void BookStartDateDropDownClosedMethod(object sender, EventArgs e)
		{
			UpdateComboBoxMethod(BookStartDateBookManagementCombo.SelectedIndex);
		}

		private void BookEndDateDropDownClosedMethod(object sender, EventArgs e)
		{
			UpdateComboBoxMethod(BookEndDateBookManagementCombo.SelectedIndex);
		}

		private void RentABookBtn_OnClick(object sender, RoutedEventArgs e)
		{
			string searchString = PhoneAreaCodeRentalCombo.SelectedValue.ToString() + "" + PhonePrefixRentalCombo.SelectedValue.ToString() + "" + PhoneLineNumberRentalCombo.SelectedValue.ToString();
			string customerPhoneNumber = "";
			int selectedBook = RentBookByTitleCombo.SelectedIndex;
			if (Books[selectedBook].IsCheckedOut.ToUpper().Equals("Y"))
			{
				MessageBox.Show("This book is already checked out");
				return;
			}
			foreach (Customer customer in Customers)
			{
				if (customer.PhoneNumber.Length > 0)
				{
					customerPhoneNumber = customer.PhoneNumber;
					customerPhoneNumber = customerPhoneNumber.Substring(0, 3) + customerPhoneNumber.Substring(4, 3) +
										  customerPhoneNumber.Substring(8, 4);
					if (customerPhoneNumber.Substring(0, searchString.Length).Equals(searchString))
					{
						if (customer.addBookRental(Books[selectedBook]))
						{
							double rentalTotal = Books[selectedBook].WeeklyRentCost;
							if (customer.Employment.ToUpper().Equals("V"))
							{
								rentalTotal *= 0.8;
							}
							else if (customer.Employment.ToUpper().Equals("S"))
							{
								rentalTotal *= 0.9;
							}

							String emp = "";
							if (customer.Employment.ToUpper().Equals("N"))
								emp = "Valued Customer - Everyday low prices";
							else if (customer.Employment.ToUpper().Equals("V"))
								emp = "Veteran - 20% discount";
							else if (customer.Employment.ToUpper().Equals("S"))
								emp = "Student - 10% discount";
							Books[selectedBook].IsCheckedOut = "Y";
							DateTime currentDate = DateTime.Now;
							Books[selectedBook].CheckOutDate =
								new DateFormat(currentDate.Year + "-" + currentDate.Month + "-" + currentDate.Day);
							Books[selectedBook].CheckOutDuration = 7;

							string lastSearches = CustomerDetailsTextBox.Text;
							CustomerDetailsTextBox.Clear();
							CustomerDetailsTextBox.Text += "\t\tSummary of your Rental\n";
							CustomerDetailsTextBox.Text +=
								"Customer Name: " + customer.FirstName + " " + customer.LastName;
							CustomerDetailsTextBox.Text +=
								"\nRental Details:\n " + Books[selectedBook].GetBookDetails();
							CustomerDetailsTextBox.Text += "\nToday's Total: $" + rentalTotal.ToString("F2") + " (" + emp + ")";
							CustomerDetailsTextBox.Text += "\n========================================================\n";
							CustomerDetailsTextBox.Text += lastSearches;

							dirtyDatabase = true;

							StatusNotificationLabel.Content = "Database is outdated, log in as admin and save your changes";
							StatusNotificationLabel.Background = System.Windows.Media.Brushes.Maroon;
							StatusNotificationLabel.Foreground = System.Windows.Media.Brushes.White;
							Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_BOOK_DETAILS));

							Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_CUSTOMER_DETAILS));
						}
					}
				}
			}
		}

		private void FindCustomerBtn_OnClick(object sender, RoutedEventArgs e)
		{
			string searchByPhoneString = PhoneNumberCustomerText.Text;
			string searchByLastNameString = LastNameCustomerText.Text;
			string searchField;
			string tempTargetString;
			string lastSearches = CustomerDetailsTextBox.Text;
			CustomerDetailsTextBox.Clear();
			CustomerSearchResults = new LinkedList<Customer>();
			if (PhoneNumberCustomerRadio.IsChecked == true)
			{
				String tempString = "";
				for (int i = 0; i < searchByPhoneString.Length; i++)
				{
					if (searchByPhoneString[i] == '-' || searchByPhoneString[i] == ' ' || searchByPhoneString[i] == '(' || searchByPhoneString[i] == ')')
					{
						continue;
					}
					tempString += searchByPhoneString[i];
				}
				searchByPhoneString = tempString;
				try
				{
					Regex regex = new Regex(@"[1-9]\d{0,8}");
					Match match = regex.Match(searchByPhoneString);
					if (match.Success)
					{
						foreach (Customer customer in Customers)
						{
							if (customer.PhoneNumber.Length > 0)
							{
								tempTargetString = customer.PhoneNumber;
								tempTargetString = tempTargetString.Substring(0, 3) + tempTargetString.Substring(4, 3) +
								                   tempTargetString.Substring(8, 4);
								if (tempTargetString.Substring(0, searchByPhoneString.Length).Equals(searchByPhoneString))
								{
									CustomerSearchResults.AddLast(customer);
								}
							}
						}
						tempString = "**********";
						searchByPhoneString += tempString.Substring(searchByPhoneString.Length);
						searchByPhoneString = searchByPhoneString.Substring(0, 3) + "-" + searchByPhoneString.Substring(3, 3) +
						                      "-" + searchByPhoneString.Substring(6);
						CustomerDetailsTextBox.Text += "\n\t\tSearching Phone Numbers starting with: \"" + searchByPhoneString + "\"\n";
						if (CustomerSearchResults.Count > 0)
						{
							CustomerDetailsTextBox.Text += "\n" +
							                               $"{"FirstName", -12} {"LastName", -15} {"PhoneNumber", -15} {"Email", -25} {"Gender", -2}" +
							                               $" {"ISBN",-15} {"Title",-20} {"Start Date",-15} {"End Date",-15} {"ISBN",-15} {"Title",-20} {"Start Date",-15} {"End Date",-15}\n" +
							                               $"================================================================================================================\n";
							foreach (Customer customer in CustomerSearchResults)
							{
								CustomerDetailsTextBox.Text += "\n" +
								                               $"--{customer.FirstName, -12} {customer.LastName, -15} {customer.PhoneNumber, -15} {customer.Email, -25} {customer.Gender, -2}";
								if (customer.getRentedBooksCount() > 0)
								{
									CustomerDetailsTextBox.Text += $" {customer.getRentedBooks()[0].ISBN, -15} {customer.getRentedBooks()[0].Title, -20} " +
									                               $"{customer.getRentedBooks()[0].CheckOutDate.tostring(), -15} " +
									                               $"{customer.getRentedBooks()[0].CheckOutDate.AddDays(7).tostring(), -15}";
									if (customer.getRentedBooksCount() > 1)
									{
										CustomerDetailsTextBox.Text += $" {customer.getRentedBooks()[1].ISBN, -15} {customer.getRentedBooks()[1].Title, -20} " +
										                               $"{customer.getRentedBooks()[1].CheckOutDate.tostring(), -15} " +
										                               $"{customer.getRentedBooks()[1].CheckOutDate.AddDays(7).tostring(), -15}";
									}
								}
							}
						}
						else
						{
							CustomerDetailsTextBox.Text += "No customers found with matching phone number";
						}
					}
					else
					{
						CustomerDetailsTextBox.Text += "Phone number can only contain up to 10 digits, hyphens, parentheses and spaces";
						CustomerDetailsTextBox.Text += "Please enter a valid phone number format, or first few digits of valid phone number format" +
						                               "\n(000)000-0000" +
						                               "\n000 000 0000" +
						                               "\n000-000-0000";
					}
				}
				catch (Exception ex)
				{

				}
			}
			else if (LastNameCustomerRadio.IsChecked == true)
			{
				if (searchByLastNameString.Length > 0)
				{
					foreach (Customer customer in Customers)
					{
						if (customer.LastName.Length > 0 && customer.LastName.Length >= searchByLastNameString.Length)
						{
							tempTargetString = customer.LastName;
							if (tempTargetString.Substring(0, searchByLastNameString.Length).ToLower().Equals(searchByLastNameString.ToLower()))
							{
								CustomerSearchResults.AddLast(customer);
							}
						}
					}
					CustomerDetailsTextBox.Text += "\n\t\tSearching Last Names starting with: \"" + searchByLastNameString.ToUpper() + "\"\n";
					if (CustomerSearchResults.Count > 0)
					{
						CustomerDetailsTextBox.Text += "\n" +
						                               $"{"FirstName",-12} {"LastName",-15} {"PhoneNumber",-15} {"Email",-25} {"Gender",-2}" +
						                               $" {"ISBN",-15} {"Title",-20} {"Start Date",-15} {"End Date",-15} {"ISBN",-15} {"Title",-20} {"Start Date",-15} {"End Date",-15}\n" +
						                               $"================================================================================================================\n";
						foreach (Customer customer in CustomerSearchResults)
						{
							CustomerDetailsTextBox.Text += "\n" +
							                               $"--{customer.FirstName,-12} {customer.LastName,-15} {customer.PhoneNumber,-15} {customer.Email,-25} {customer.Gender,-2}";
							if (customer.getRentedBooksCount() > 0)
							{
								CustomerDetailsTextBox.Text += $" {customer.getRentedBooks()[0].ISBN,-15} {customer.getRentedBooks()[0].Title,-20} " +
								                               $"{customer.getRentedBooks()[0].CheckOutDate.tostring(),-15} " +
								                               $"{customer.getRentedBooks()[0].CheckOutDate.AddDays(7).tostring(),-15}";
								if (customer.getRentedBooksCount() > 1)
								{
									CustomerDetailsTextBox.Text += $" {customer.getRentedBooks()[1].ISBN,-15} {customer.getRentedBooks()[1].Title,-20} " +
									                               $"{customer.getRentedBooks()[1].CheckOutDate.tostring(),-15} " +
									                               $"{customer.getRentedBooks()[1].CheckOutDate.AddDays(7).tostring(),-15}";
								}
							}
						}
					}
					else
					{
						CustomerDetailsTextBox.Text += "No customers found with matching last name";
					}
					CustomerDetailsTextBox.Text += "\n========================================================\n";
					CustomerDetailsTextBox.Text += lastSearches;
				}
				else
				{
					CustomerDetailsTextBox.Text += "Last names must be at least one character long";
				}
			}

			CustomerDetailsTextBox.Text += "\n========================================================\n";
			CustomerDetailsTextBox.Text += lastSearches;

			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_CUSTOMER_DETAILS));
		}

		private void ClearCustomerDetailBtn_OnClick(object sender, RoutedEventArgs e)
		{
			CustomerDetailsTextBox.Text = "";
			PhoneNumberCustomerText.Clear();
			LastNameCustomerText.Clear();
			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_CUSTOMER_MANAGEMENT));
		}

		private void ClearBookDetailBtn_OnClick(object sender, RoutedEventArgs e)
		{
			BookDetailsTextBox.Text = "";
            Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = TAB_BOOK_MANAGEMENT));
        }

		private void SignUpRegisterBtn_OnClick(object sender, RoutedEventArgs e)
		{
			bool validEntry = true;
			if (FirstNameRegisterText.Text.Length < 3)
			{
				MessageBox.Show("First Name must be at least 3 characters long");
				validEntry = false;
			}
			if (LastNameRegisterText.Text.Length < 3)
			{
				MessageBox.Show("Last Name must be at least 3 characters long");
				validEntry = false;
			}
			if (Address1RegisterText.Text.Length < 3)
			{
				MessageBox.Show("Address must be at least 3 characters long");
				validEntry = false;
			}
			if (CityRegisterText.Text.Length < 2)
			{
				MessageBox.Show("City Name must be at least 2 characters long");
				validEntry = false;
			}
			if (ZipRegisterText.Text.Length != 5)
			{
				Regex regex = new Regex(@"\d{5}");
				Match match = regex.Match("");
				if (!match.Success)
				{
					MessageBox.Show("Zip Code must be at 5 digits long");
					validEntry = false;

				}
			}
			if (EmailRegisterText.Text.Length < 4)
			{
				Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
				Match match = regex.Match("");
				if (!match.Success)
				{
					MessageBox.Show("Email must be in the form username@exchangeServer.domain");
					validEntry = false;

				}
				MessageBox.Show("Email must be in the form username@exchangeServer.domain");
				validEntry = false;
			}
			if (Password1RegisterText.Password.Length < 7 || Password1RegisterText.Password.CompareTo(Password2RegisterText.Password) != 0)
			{
				MessageBox.Show("Password must be at least 7 characters long\nBoth passwords must match");
				validEntry = false;
			}

			if (validEntry)
			{
				Customer newCustomer = new Customer();
				newCustomer.FirstName = FirstNameRegisterText.Text;
				newCustomer.LastName = LastNameRegisterText.Text;
				newCustomer.Address = Address1RegisterText.Text;
				newCustomer.City = CityRegisterText.Text;
				newCustomer.State = StatesRegisterCombo.Text.ToString();
				newCustomer.ZipCode = ZipRegisterText.Text;
				newCustomer.Email = EmailRegisterText.Text;
				newCustomer.PhoneNumber = PhoneAreaCodeRegisterCombo.SelectedValue.ToString() +
					  "-" + PhonePrefixRegisterCombo.SelectedValue.ToString() +
					  "-" + PhoneLineNumberRegisterCombo.SelectedValue.ToString();
				newCustomer.Gender = GenderRegisterCombo.Text.ToString();
				newCustomer.Password = Password1RegisterText.Password;
				newCustomer.Employment = VeteranDiscountCustomerRadio.IsChecked == true ? "V" : StudentDiscountCustomerRadio.IsChecked == true ? "S" : "N";
				newCustomer.FirstBook = null;
				newCustomer.SecondBook = null;
				Customers.AddLast(newCustomer);

				FirstNameRegisterText.Clear();
				LastNameRegisterText.Clear();
				Address1RegisterText.Clear();
				CityRegisterText.Clear();
				StatesRegisterCombo.SelectedIndex = -1;
				ZipRegisterText.Clear();
				EmailRegisterText.Clear();
				PhoneAreaCodeRegisterCombo.SelectedIndex = -1;
				PhonePrefixRegisterCombo.SelectedIndex = -1;
				PhoneLineNumberRegisterCombo.SelectedIndex = -1;
				GenderRegisterCombo.SelectedIndex = -1;
				Password1RegisterText.Clear();
				Password2RegisterText.Clear();
				VeteranDiscountCustomerRadio.IsChecked = false;
				StudentDiscountCustomerRadio.IsChecked = false;
				NoDiscountCustomerRadio.IsChecked = true;
			}
		}

		private void BookIsRentedBookManagementText_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			bool canCheckOut = true;
			if (BookStartDateBookManagementText.Text.Length < 10 && BookStartDateBookManagementCombo.SelectedValue.Equals(""))
			{
				BookIsRentedBookManagementText.Text = "N";
				canCheckOut = false;
			}
			if (BookEndDateBookManagementText.Text.Length < 10 && BookEndDateBookManagementCombo.SelectedValue.Equals(""))
			{
				BookIsRentedBookManagementText.Text = "N";
				canCheckOut = false;
			}

			if (canCheckOut && BookIsRentedBookManagementText.Text.ToUpper().Equals("Y"))
			{
				BookIsRentedBookManagementCombo.SelectedValue = "Y";
			}
		}

		private void BookStartDateBookManagementText_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			string strVal = BookStartDateBookManagementText.Text;
			DateFormat tempDate = new DateFormat(strVal);
			if (!tempDate.getEnteredCorrectly())
			{
				MessageBox.Show("Dates must be in the format [yyyy-mm-dd]");
			}

			if (tempDate.isPastDate())
			{
				MessageBox.Show("Dates must not be retrospective");
			}

		}

		private void BookEndDateBookManagementText_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			string strVal = BookEndDateBookManagementText.Text;
			DateFormat tempDate = new DateFormat(strVal);
			if (!tempDate.getEnteredCorrectly())
			{
				MessageBox.Show("Dates must be in the format [yyyy-mm-dd]");
			}

			if (tempDate.isPastDate())
			{
				MessageBox.Show("Dates must not be retrospective");
			}
		}

		private void PhoneNumberCustomerRadio_OnClick(object sender, RoutedEventArgs e)
		{
			if (PhoneNumberCustomerRadio.IsChecked == true)
			{
				PhoneNumberCustomerLabel.Visibility = Visibility.Visible;
				PhoneNumberCustomerText.Visibility = Visibility.Visible;
				LastNameCustomerLabel.Visibility = Visibility.Collapsed;
				LastNameCustomerText.Visibility = Visibility.Collapsed;
				PhoneNumberCustomerRadioText.Background = System.Windows.Media.Brushes.Aquamarine;
				LastNameCustomerRadioText.Background = System.Windows.Media.Brushes.DimGray;
			}
			else
			{
				LastNameCustomerLabel.Visibility = Visibility.Visible;
				LastNameCustomerText.Visibility = Visibility.Visible;
				PhoneNumberCustomerLabel.Visibility = Visibility.Collapsed;
				PhoneNumberCustomerText.Visibility = Visibility.Collapsed;
				PhoneNumberCustomerRadioText.Background = System.Windows.Media.Brushes.DimGray;
				LastNameCustomerRadioText.Background = System.Windows.Media.Brushes.Aquamarine;
			}
		}

		private void CustomerRentalRadio_OnClick(object sender, RoutedEventArgs e)
		{
			if (PhoneNumberRentalRadio.IsChecked == true)
			{
				RentalCustomerByPhoneNumberLabel.Visibility = Visibility.Visible;
				PhoneAreaCodeRentalCombo.Visibility = Visibility.Visible;
				PhonePrefixRentalCombo.Visibility = Visibility.Visible;
				PhoneLineNumberRentalCombo.Visibility = Visibility.Visible;
				RentalCustomerByFirstLastNameLabel.Visibility = Visibility.Collapsed;
				FirstLastNameRentalCombo.Visibility = Visibility.Collapsed;
			}
			else
			{
				RentalCustomerByPhoneNumberLabel.Visibility = Visibility.Collapsed;
				PhoneAreaCodeRentalCombo.Visibility = Visibility.Collapsed;
				PhonePrefixRentalCombo.Visibility = Visibility.Collapsed;
				PhoneLineNumberRentalCombo.Visibility = Visibility.Collapsed;
				RentalCustomerByFirstLastNameLabel.Visibility = Visibility.Visible;
				FirstLastNameRentalCombo.Visibility = Visibility.Visible;
			}
		}
	}

	

	public class Book
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string ISBN { get; set; }
		public double Price { get; set; }
		public double WeeklyRentCost { get; set; }
		public double DailyRentCost { get; set; }
		public int PageCount { get; set; }
		public string Description { get; set; }
		public string Author { get; set; }
		public string Category { get; set; }
		public string IsCheckedOut { get; set; }
		public Customer RentingCustomer { get; set; }
		public DateFormat CheckOutDate { get; set; }
		public int CheckOutDuration { get; set; }

		public Book()
		{
			ID = 0;
			Title = "";
			ISBN = "";
			Price = 0.0;
			WeeklyRentCost = 0.0;
			DailyRentCost = 0.0;
			PageCount = 0;
			Description = "";
			Author = "";
			Category = "";
			IsCheckedOut = "N";
			CheckOutDate = new DateFormat("1900-01-01");
			CheckOutDuration = 0;
			RentingCustomer = null;
		}

		public string GetBookDetails()
		{
			
			string returnStr = "";
			returnStr = "ID: " + this.ID;
			returnStr += "\nTitle: " + this.Title;
			returnStr += "\nISBN: " + this.ISBN;
			returnStr += "\nPrice: $" + this.Price;
			returnStr += "\nWeekly Rental Cost: $" + this.WeeklyRentCost;
			returnStr += "\nDaily Rental Cost: $" + this.DailyRentCost;
			returnStr += "\nPage Count: " + this.PageCount;
			returnStr += "\nDescription: " + (this.Description.Length > 25 ? this.Description.Substring(0, 22) + "..." : this.Description);
			returnStr += "\nAuthor: " + (!string.IsNullOrEmpty(this.Author) ? this.Author : "Unknown");
			returnStr += "\nCategories: " + (!string.IsNullOrEmpty(this.Category) ? this.Category : "Unknown");
			returnStr += "\nChecked Out: " + (this.IsCheckedOut.ToUpper().Equals("Y") ? "Yes" : "No");
			returnStr += "\nCheckout Date: " + this.CheckOutDate.Month + "/" + this.CheckOutDate.Day + "/" +
						 this.CheckOutDate.Year;
			return returnStr;
		}
	}

	public class Customer
	{
		private double discount;
		private int rentedBooks;
		private Book[] rentalBooksList;
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public string Gender { get; set; }
		public string Password { get; set; }
		public string Employment { get; set; }
		public Book FirstBook { get; set; }
		public Book SecondBook { get; set; }

		public Customer()
		{
			FirstName = "";
			LastName = "";
			Address = "";
			City = "";
			State = "";
			ZipCode = "";
			PhoneNumber = "";
			Email = "";
			Gender = "";
			Password = "";
			Employment = "";
			discount = 0.0;
			FirstBook = new Book();
			SecondBook = new Book();
			rentedBooks = 0;
			rentalBooksList = new Book[2];
			rentalBooksList[0] = null;
			rentalBooksList[1] = null;
		}

		public void setDiscount(double discount)
		{
			this.discount = discount;
		}

		public double getDiscountRate()
		{
			return discount;
		}

		public bool setRentedBooksCount(int rentalCount)
		{
			if (rentalCount < 0)
			{
				MessageBox.Show("You don't have any checked out books to return");
				return false;
			}

			else if (rentalCount > 2)
			{
				MessageBox.Show("You reached the limit of allowed rentals");
				return false;
			}

			else
			{
				this.rentedBooks = rentalCount;
			}
			return true;
		}

		public int getRentedBooksCount()
		{
			return rentedBooks;
		}

		public void setRentedBooks(Book bookOne, Book bookTwo)
		{
			rentalBooksList[0] = bookOne;
			rentalBooksList[0] = bookTwo;
		}

		public Book[] getRentedBooks()
		{
			return rentalBooksList;
		}

		public bool addBookRental(Book rentalBook)
		{
			if (setRentedBooksCount(rentedBooks + 1))
			{
				rentalBooksList[rentedBooks - 1] = rentalBook;
				return true;
			}
			return false;
		}

		public void returnBookRental(int rentalIndex)
		{
			if (rentalIndex < 1 && rentalIndex > 2)
			{
				MessageBox.Show("");
				return;
			}

			if (rentedBooks <= 0)
			{
				return;
			}

			rentedBooks--;
			rentalBooksList[rentalIndex - 1] = null;

			if (rentalIndex == 1)
			{
				rentalBooksList[0] = rentalBooksList[1];
				rentalBooksList[1] = null;
			}
			else
			{
				rentalBooksList[1] = null;
			}
		}

		public string GetCustomerDetail()
		{
			string returnStr = "";
			returnStr += "FirstName :" + FirstName;
			returnStr += "\nLast Name: " + LastName;
			returnStr += "\nCity: " + City;
			returnStr += "\nState: " + State;
			returnStr += "\nZip Code: " + ZipCode;
			returnStr += "\nPhone Number: " + PhoneNumber;
			returnStr += "\nEmail: " + Email;
			returnStr += "\nGender: " + Gender;
			returnStr += "\nPassword: " + "***";
			returnStr += "\nEmployment: " + Employment;
			returnStr += "\nFirst Book: " + FirstBook == null ? "None" : "" + FirstBook.ID;
			returnStr += "\nSecond Book: " + SecondBook == null ? "None" : "" + SecondBook.ID;
			return returnStr;
		}
	}

	public class DateFormat
	{
		private DateFormat value;
		public static bool enteredCorrectly;

		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }

		public DateFormat(string stringFormat)
		{
			try
			{
				Regex regex = new Regex(@"\d{4}-\d{2}-\d{2}");
				Match match = regex.Match(stringFormat);
				if (match.Success)
				{
					this.Year = int.Parse(stringFormat.Substring(0, 4));
					this.Month = int.Parse(stringFormat.Substring(5, 2));
					this.Day = int.Parse(stringFormat.Substring(8, 2));
					enteredCorrectly = true;
				}
				else
				{
					this.Year = 1900;
					this.Month = 01;
					this.Day = 01;
					enteredCorrectly = false;
				}
			}
			catch (Exception e)

			{
				this.Year = 1900;
				this.Month = 01;
				this.Day = 01;
				enteredCorrectly = false;
			}
		}

		public DateFormat(DateFormat value)
		{
			this.value = value;
		}

		public DateFormat AddDays(int daysToAdd)
		{
			DateTime date = new DateTime(this.Year, this.Month, this.Day);
			date = date.AddDays(daysToAdd);
			DateFormat newDate = new DateFormat(date.Year + "-" + date.Month + "-" + date.Day);
			return newDate;
		}

		public int DaysApart(DateFormat laterDateFormat)
		{
			DateTime Date1 = new DateTime(this.Year, this.Month, this.Day);
			DateTime Date2 = new DateTime(laterDateFormat.Year, laterDateFormat.Month, laterDateFormat.Day);

			// Returns Zero if both dates are the same,
			// Negative value if laterDateFormat is earlier than the calling DateFormat,
			// Positive value if LaterDateFormat is after the calling DateFormat
			return Date2.CompareTo(Date1);
		}

		public bool getEnteredCorrectly()
		{
			return enteredCorrectly;
		}

		public bool isPastDate()
		{
			DateTime currentDate = DateTime.Now;
			this.Year = currentDate.Year;
			this.Month = currentDate.Month;
			this.Day = currentDate.Day;

			DateFormat tempDateFormat = new DateFormat(currentDate.Year + "-" + currentDate.Month + "-" +currentDate.Day);
			return (this.DaysApart(tempDateFormat) < 0);
		}

		public string tostring()
		{
			string returnStr = "";
			returnStr += this.Year + "-";
			returnStr += (this.Month < 10 ? "0" : "");
			returnStr += this.Month + "-";
			returnStr += (this.Day < 10 ? "0" : "");
			returnStr += this.Day;
			return returnStr;
		}
	}
}
