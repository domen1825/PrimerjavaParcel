using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace PrimerjavaParcelWPF
{
    public partial class MainWindow : Window
    {
        private string potDoExcelDatoteke = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            NapolniComboBoxe();
            PosodobiOznake();
        }

        private void PrimerjajButton_Click(object sender, RoutedEventArgs e)
        {
            string podatkiLeto1 = PodatkiLeto1TextBox.Text;
            string podatkiLeto2 = PodatkiLeto2TextBox.Text;

            List<string> seznamLeto1 = ParseParcelData(podatkiLeto1);
            List<string> seznamLeto2 = ParseParcelData(podatkiLeto2);

            HashSet<string> setLeto1 = new HashSet<string>(seznamLeto1);
            HashSet<string> setLeto2 = new HashSet<string>(seznamLeto2);

            List<string> novoV2 = NajdiNovo(setLeto1, seznamLeto2);
            List<string> odstranjenoV2 = NajdiOdstranjeno(setLeto2, seznamLeto1);

            string rezultati = $"Parcelne številke, ki so v letu {Leto2ComboBox.SelectedItem} in niso v letu {Leto1ComboBox.SelectedItem}:\n" +
                               string.Join(", ", novoV2) +
                               "\n\n" +
                               $"Parcelne številke, ki so v letu {Leto1ComboBox.SelectedItem} in niso v letu {Leto2ComboBox.SelectedItem}:\n" +
                               string.Join(", ", odstranjenoV2);

            RezultatiTextBox.Text = rezultati;
        }

        private void IzberiDatoteko_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Izberi Excel datoteko"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                potDoExcelDatoteke = openFileDialog.FileName;
                IzbranaDatotekaTextBlock.Text = $"Izbrana datoteka: {System.IO.Path.GetFileName(potDoExcelDatoteke)}";
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(potDoExcelDatoteke))
            {
                MessageBox.Show("Prosimo, izberite Excel datoteko.", "Napaka", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ImeZvezkaTextBox.Text))
            {
                MessageBox.Show("Prosimo, vnesite ime novega lista.", "Napaka", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook(potDoExcelDatoteke))
                {
                    // Ustvari nov list v Excel datoteki
                    var worksheet = workbook.Worksheets.Add(ImeZvezkaTextBox.Text);

                    // Dinamični naslovi glede na izbrana leta
                    var leto1 = Leto1ComboBox.SelectedItem.ToString();
                    var leto2 = Leto2ComboBox.SelectedItem.ToString();

                    worksheet.Cell(1, 1).Value = $"Podatki za {leto1}";
                    worksheet.Cell(1, 3).Value = $"Podatki za {leto2}";
                    worksheet.Cell(1, 5).Value = $"V letu {leto2} in ne v letu {leto1}";
                    worksheet.Cell(1, 7).Value = $"V letu {leto1} in ne v letu {leto2}";

                    // Priprava podatkov
                    var podatkiLeto1 = string.Join(", ", PodatkiLeto1TextBox.Text.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
                    var podatkiLeto2 = string.Join(", ", PodatkiLeto2TextBox.Text.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));

                    var setLeto1 = new HashSet<string>(podatkiLeto1.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
                    var setLeto2 = new HashSet<string>(podatkiLeto2.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));

                    var onlyInYear2 = string.Join(", ", setLeto2.Except(setLeto1));
                    var onlyInYear1 = string.Join(", ", setLeto1.Except(setLeto2));

                    // Zapis podatkov v celice
                    worksheet.Cell(2, 1).Value = podatkiLeto1; // Vse številke leta 1
                    worksheet.Cell(2, 3).Value = podatkiLeto2; // Vse številke leta 2
                    worksheet.Cell(2, 5).Value = onlyInYear2; // Razlika: v letu 2 in ne v letu 1
                    worksheet.Cell(2, 7).Value = onlyInYear1; // Razlika: v letu 1 in ne v letu 2

                    // Samodejno prilagodite širino stolpcev
                    worksheet.Columns().AdjustToContents();

                    // Shrani spremembe
                    workbook.Save();
                    MessageBox.Show("Podatki so bili uspešno shranjeni.", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Napaka: {ex.Message}", "Napaka", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private List<string> ParseParcelData(string data) =>
            data.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList();

        private List<string> NajdiNovo(HashSet<string> data1, List<string> data2) =>
            data2.Where(parcel => !data1.Contains(parcel)).ToList();

        private List<string> NajdiOdstranjeno(HashSet<string> data2, List<string> data1) =>
            data1.Where(parcel => !data2.Contains(parcel)).ToList();

        private void NapolniComboBoxe()
        {
            List<int> leta = Enumerable.Range(1996, 2024 - 1996 + 1).ToList();
            Leto1ComboBox.ItemsSource = leta;
            Leto1ComboBox.SelectedItem = 2018;
            Leto2ComboBox.ItemsSource = leta;
            Leto2ComboBox.SelectedItem = 2024;
        }

        private void PosodobiOznake()
        {
            Leto1Label.Text = $"Podatki za Leto {Leto1ComboBox.SelectedItem}:";
            Leto2Label.Text = $"Podatki za Leto {Leto2ComboBox.SelectedItem}:";
        }

        private void Leto1ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
            PosodobiOznake();

        private void Leto2ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
            PosodobiOznake();
    }
}
