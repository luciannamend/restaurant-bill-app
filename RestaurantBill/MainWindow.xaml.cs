using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantBill
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Observable collection of ordered items
        private ObservableCollection<MenuItem> orderedItems = new ObservableCollection<MenuItem>();

        // Beverages and their prices
        private readonly string[] beverages = { "Soda", "Tea", "Coffee", "Mineral Water", "Juice", "Milk" };
        private readonly decimal[] beveragePrices = { 1.95m, 1.50m, 1.25m, 2.95m, 2.50m, 1.50m };

        //Appetizers and their prices
        private readonly string[] appetizers = { "Buffalo Wings", "Buffalo Fingers", "Potato Skin", "Nachos",
            "Mushroom Caps", "Shrimp Cocktail", "Chips and Salsa" };
        private readonly decimal[] appetizerPrices = { 5.95m, 6.95m, 8.95m, 8.95m, 10.95m, 12.95m, 6.95m };

        // Main Courses and their prices
        private readonly string[] mainCourses = { "Seafood Alfredo", "Chicken Alfredo", "Chicken Picatta",
            "Turkey Club", "Lobster Pie", "Prime Rib", "Shrimp Scampi", "Turkey Dinner", "Stuffed Chicken" };
        private readonly decimal[] mainCoursePrices = { 15.95m, 13.95m, 13.95m, 11.95m, 19.95m, 20.95m, 18.95m, 13.95m, 14.95m };

        // Desserts and their prices
        private readonly string[] desserts = { "Apple Pie", "Sundae", "Carrot Cake", "Mud Pie", "Apple Crisp" };
        private readonly decimal[] dessertPrices = { 5.95m, 3.95m, 5.95m, 4.95m, 5.95m };

        // subtotal and tax rate of 13%
        private decimal subtotal = 0m;
        private decimal taxRate = 0.13m;

        public MainWindow()
        {
            InitializeComponent();

            // Populate ComboBoxes with menu items
            cmbBeverage.ItemsSource = GetMenuItems("Beverage");
            cmbAppetizer.ItemsSource = GetMenuItems("Appetizer");
            cmbMainCourse.ItemsSource = GetMenuItems("Main Course");
            cmbDessert.ItemsSource = GetMenuItems("Dessert");

            // Set DataGrid's ItemsSource to orderedItems obs. collection
            dgBillItems.ItemsSource = orderedItems;
        }

        // Method to get menu items based on category
        private string[]? GetMenuItems(string category)
        {            
            switch (category)
            {
                case "Beverage":
                    return beverages;
                case "Appetizer":
                    return appetizers;
                case "Main Course":
                    return mainCourses;
                case "Dessert":
                    return desserts;
                default:
                    return null;
            }
        }

        // Method for selecting item do include on the bill
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            // If combobox is not null and has a selected item
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                string selectedItem = comboBox.SelectedItem.ToString();
                decimal itemPrice = GetItemPrice(selectedItem, comboBox.Name);

                // Check if item already exists in orderedItems collection
                MenuItem existingItem = orderedItems.FirstOrDefault(item => item.Name == selectedItem);

                if (existingItem != null)
                {
                    // Item already exists, update quantity and subtotal
                    existingItem.Quantity += 1;
                    existingItem.TotalPrice = existingItem.Quantity * existingItem.Price;
                }
                else
                {
                    // Item does not exist, add new MenuItem to orderedItems
                    MenuItem newItem = new MenuItem { Name = selectedItem, Price = itemPrice, Quantity = 1, TotalPrice = itemPrice};
                    orderedItems.Add(newItem);
                }

                // Update DataGrid
                dgBillItems.Items.Refresh();
                UpdateBillAmounts();
            }
        }

        private decimal GetItemPrice(string itemName, string comboBoxName)
        {
            // Determine item price based on ComboBox name
            switch (comboBoxName)
            {
                case "cmbBeverage":
                    int bevIndex = Array.IndexOf(beverages, itemName);
                    return bevIndex >= 0 ? beveragePrices[bevIndex] : 0m;
                case "cmbAppetizer":
                    int appIndex = Array.IndexOf(appetizers, itemName);
                    return appIndex >= 0 ? appetizerPrices[appIndex] : 0m;
                case "cmbMainCourse":
                    int mainIndex = Array.IndexOf(mainCourses, itemName);
                    return mainIndex >= 0 ? mainCoursePrices[mainIndex] : 0m;
                case "cmbDessert":
                    int dessertIndex = Array.IndexOf(desserts, itemName);
                    return dessertIndex >= 0 ? dessertPrices[dessertIndex] : 0m;
                default:
                    return 0m;
            }
        }

        private void RemoveMenuItem_Click(object sender, RoutedEventArgs e) 
        {           

            if (dgBillItems.SelectedItem != null)
            {
                MenuItem selectedItem = dgBillItems.SelectedItem as MenuItem;
                if (selectedItem != null)
                {
                    orderedItems.Remove(selectedItem);
                    subtotal -= selectedItem.TotalPrice;
                    UpdateBillAmounts();
                }
            }
            MessageBox.Show(this, $"Item Removed!");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Clear the original orderedItems collection
            orderedItems.Clear();
            subtotal = 0m;
            UpdateBillAmounts();
        }        

        private void UpdateBillAmounts()
        {
            subtotal = orderedItems.Sum(item => item.TotalPrice);
            decimal tax = subtotal * taxRate;
            decimal total = subtotal + tax;

            txtSubtotal.Text = subtotal.ToString("C");
            txtTax.Text = tax.ToString("C");
            txtTotal.Text = total.ToString("C");
        }

        private void OpenWebsite(object sender, RoutedEventArgs e)
        {
            // Open Centennial College's website in Internet Explorer
            Process.Start(new ProcessStartInfo("iexplore.exe", "https://www.centennialcollege.ca/"));
        }

        private class MenuItem
        {
            public int Quantity { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }    
}
