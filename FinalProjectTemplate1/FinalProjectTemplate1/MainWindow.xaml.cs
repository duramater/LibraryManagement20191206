using System;
using System.Collections.Generic;
using System.IO;
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

namespace FinalProjectTemplate1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static bool RegisterPopulated = false;
		private static bool loaded = false;
		private static bool BooksAdded = false;

		private static Book[] Books;
		private static Customer[] Customers;
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
			try
			{
				string CustomersOutputFilePath = "../../Resources/customers1.csv";
				string BooksOutputFilePath = "../../Resources/books1.csv";
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
					recordStr += book.PublishDate + ",";
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
				for (int i = 0; i < Customers.Length; i++)//each (Customer customer in Customers)
				{
					recordStr = "";
					recordStr += Customers[i].FirstName + ",";
					recordStr += Customers[i].LastName + ",";
					recordStr += Customers[i].Address + ",";
					recordStr += Customers[i].City + ",";
					recordStr += Customers[i].State + ",";
					recordStr += Customers[i].ZipCode + ",";
					recordStr += Customers[i].PhoneNumber + ",";
					recordStr += Customers[i].Email + ",";
					recordStr += Customers[i].Gender + ",";
					recordStr += Customers[i].Password + ",";
					recordStr += Customers[i].FirstBook == null ? "None" : Customers[i].FirstBook.ID + ",";
					recordStr += Customers[i].SecondBook == null ? "None" : Customers[i].SecondBook.ID + ",";
					recordStr += ".";
					CustomertreamWriter.WriteLine(recordStr);
				}
				CustomertreamWriter.Close();
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
			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = 5));
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
			if(tabControl.SelectedIndex == 2)
			{
				PopulateBooks();
			}


			if (tabControl.SelectedIndex == 1)
			{
				PopulateBooks();
				PopulateRegister();
			}


			if (tabControl.SelectedIndex == 5)
			{
				Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = 5));
				PopulateRegister();
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
							PhoneAreaCodeRentalCombo.Items.Add(i + "" + j + "" + k);
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
							PhonePrefixRentalCombo.Items.Add(i + "" + j + "" + k);
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
								PhoneLineNumberRentalCombo.Items.Add(i + "" + j + "" + k + "" + m);
							}
						}
					}
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
					Customers = new Customer[customersCount];
				}
				
				//Pass the file path and file name to the StreamReader constructor
				sr = new StreamReader(customersFilePath);

				//Read the first line of text
				line = sr.ReadLine();

				//Continue to read until you reach end of file
				string[] lineValues;
				int entryTracker = 0;
				for (int i = 0; i < customersCount; i++)
				{
					Customers[entryTracker++] = new Customer();
				}
				
				entryTracker = 0;
				while (line != null)
				{
					int j = 0;
					lineValues = line.Split(',');
					Customers[entryTracker].FirstName = lineValues[0];
					Customers[entryTracker].LastName = lineValues[1];
					Customers[entryTracker].Address = lineValues[2];
					Customers[entryTracker].City = lineValues[3];
					Customers[entryTracker].State = lineValues[4];
					Customers[entryTracker].ZipCode = lineValues[5];
					Customers[entryTracker].PhoneNumber = lineValues[6];
					Customers[entryTracker].Email = lineValues[7];
					Customers[entryTracker].Gender = lineValues[8];
					Customers[entryTracker].Password = lineValues[9];
					Customers[entryTracker].FirstBook.ISBN = string.IsNullOrEmpty(lineValues[10]) ? "None" : lineValues[10];
					Customers[entryTracker].SecondBook.ISBN = string.IsNullOrEmpty(lineValues[11]) ? "None" : lineValues[11];
					

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
					Books[entryTracker].PublishDate = new DateFormat(lineValues[7]);
					Books[entryTracker].Description = lineValues[8];
					Books[entryTracker].Author = lineValues[9];
					Books[entryTracker].Category = lineValues[10];
					Books[entryTracker].IsCheckedOut = lineValues[11];
					
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

		private void UpdateBookManagementBtn_OnClick(object sender, RoutedEventArgs e)
		{
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

			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = 6));

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

		//FindCustomerBtn_OnClick

		private void RentABookBtn_OnClick(object sender, RoutedEventArgs e)
		{
			string searchString = PhoneAreaCodeRentalCombo.SelectedValue + "" + PhonePrefixRentalCombo.SelectedValue + "" + PhoneLineNumberRentalCombo;
			string customerPhoneNumber = "";
			int selectedBook = RentBookByTitleCombo.SelectedIndex;
			if (Books[selectedBook].IsCheckedOut.ToUpper().Equals("Y"))
			{
				MessageBox.Show("This book is already checked out");
				return;
			}
			for (int i = 0; i < Customers.Length; i++)
			{
				if (Customers[i].PhoneNumber.Length > 0)
				{
					customerPhoneNumber = Customers[i].PhoneNumber;
					customerPhoneNumber = customerPhoneNumber.Substring(0, 3) + customerPhoneNumber.Substring(4, 3) +
										  customerPhoneNumber.Substring(8, 4);
					if (customerPhoneNumber.Substring(0, searchString.Length).Equals(searchString))
					{
						if (Customers[i].FirstBook.ISBN.Length <= 0)
						{
							Books[selectedBook].IsCheckedOut = "Y";
							Customers[i].FirstBook = Books[selectedBook];
							return;
						}
						else if (Customers[i].SecondBook.ISBN.Length <= 0)
						{
							Books[selectedBook].IsCheckedOut = "Y";
							Customers[i].SecondBook = Books[selectedBook];
							return;
						}
					}
				}
			}
		}

		private void FindCustomerBtn_OnClick(object sender, RoutedEventArgs e)
		{
			string searchString = PhoneNumberCustomerText.Text;
			string tempTargetString;
			string lastSearches = CustomerDetailsTextBox.Text;
			CustomerDetailsTextBox.Clear();
			CustomerSearchResults = new LinkedList<Customer>();
			try
			{
				Regex regex = new Regex(@"[1-9]\d{0,8}");
				Match match = regex.Match(searchString);
				if (match.Success)
				{
					for (int i = 0; i < Customers.Length; i++)
					{
						if (Customers[i].PhoneNumber.Length > 0)
						{
							tempTargetString = Customers[i].PhoneNumber;
							tempTargetString = tempTargetString.Substring(0, 3) + tempTargetString.Substring(4, 3) +
											   tempTargetString.Substring(8, 4);
							if (tempTargetString.Substring(0, searchString.Length).Equals(searchString))
							{
								CustomerSearchResults.AddLast(Customers[i]);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{

			}
			CustomerDetailsTextBox.Text += "\n\t\tSearching [" + searchString + "]\n";
			foreach (Customer customer in CustomerSearchResults)
			{
				CustomerDetailsTextBox.Text += "\n" +
				   "Name: " + customer.FirstName + " " + customer.LastName + ", Email: " +
				   customer.Email + ", Phone: " +
				   customer.PhoneNumber;
			}

			CustomerDetailsTextBox.Text += "\n========================================================\n";
			CustomerDetailsTextBox.Text += lastSearches;

			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = 7));
		}

		private void ClearCustomerDetailBtn_OnClick(object sender, RoutedEventArgs e)
		{
			CustomerDetailsTextBox.Text = "";
			Dispatcher.BeginInvoke((Action)(() => tabControl.SelectedIndex = 3));
		}

		private void ClearBookDetailBtn_OnClick(object sender, RoutedEventArgs e)
		{
			BookDetailsTextBox.Text = "";
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
				newCustomer.State = StatesRegisterCombo.SelectedValue.ToString();
				newCustomer.ZipCode = ZipRegisterText.Text;
				newCustomer.Email = EmailRegisterText.Text;
				newCustomer.PhoneNumber = PhoneAreaCodeRegisterCombo.SelectedValue.ToString() +
					  "-" + PhonePrefixRegisterCombo.SelectedValue.ToString() +
					  "-" + PhoneLineNumberRegisterCombo.SelectedValue.ToString();
				newCustomer.Gender = GenderRegisterCombo.SelectedValue.ToString();
				newCustomer.Password = Password1RegisterText.Password;
				newCustomer.FirstBook = null;
				newCustomer.SecondBook = null;
				Customer[] tempCustomer = new Customer[Customers.Length + 1];
				for (int i = 0; i < Customers.Length; i++)
				{
					tempCustomer[i] = new Customer();
					tempCustomer[i] = Customers[i];
				}

				tempCustomer[Customers.Length] = newCustomer;
				Customers = tempCustomer;
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
		public DateFormat PublishDate { get; set; }
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
			PageCount = 0; DateTime localDate = DateTime.Now;
			PublishDate = new DateFormat(localDate.Year + "-" + localDate.Month + "-" + localDate.Day);
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
			returnStr += "\nPublish Date: " + (this.PublishDate).tostring();
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
			FirstBook = new Book();
			SecondBook = new Book();
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
			this.Year = date.Year;
			this.Month = date.Month;
			this.Day = date.Day;
			return this;
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
