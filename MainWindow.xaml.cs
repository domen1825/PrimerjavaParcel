using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PrimerjavaParcelWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NapolniComboBoxe();
            PosodobiOznake();
        }

        private void PrimerjajButton_Click(object sender, RoutedEventArgs e)
        {
            // Preberi podatke iz TextBox-ov
            string podatkiLeto1 = PodatkiLeto1TextBox.Text;
            string podatkiLeto2 = PodatkiLeto2TextBox.Text;

            // Pretvori podatke v sezname parcel
            List<string> seznamLeto1 = ParseParcelData(podatkiLeto1);
            List<string> seznamLeto2 = ParseParcelData(podatkiLeto2);

            // Ustvari sklope (sets) za hitrejše iskanje
            HashSet<string> setLeto1 = new HashSet<string>(seznamLeto1);
            HashSet<string> setLeto2 = new HashSet<string>(seznamLeto2);

            // Poišči parcelne številke, ki so nove v letu 2
            List<string> novoV2 = NajdiNovo(setLeto1, seznamLeto2);

            // Poišči parcelne številke, ki so bile odstranjene iz leta 1
            List<string> odstranjenoV2 = NajdiOdstranjeno(setLeto2, seznamLeto1);

            // Pripravi rezultate za prikaz
            string rezultati = $"Parcelne številke, ki so v letu {Leto2ComboBox.SelectedItem} in niso v letu {Leto1ComboBox.SelectedItem}:\n" +
                               string.Join(", ", novoV2) +
                               "\n\n" +
                               $"Parcelne številke, ki so v letu {Leto1ComboBox.SelectedItem} in niso v letu {Leto2ComboBox.SelectedItem}:\n" +
                               string.Join(", ", odstranjenoV2);

            // Prikaz rezultatov v TextBox-u
            RezultatiTextBox.Text = rezultati;
        }

        private List<string> ParseParcelData(string data)
        {
            // Razdeli niz glede na vejice in odstrani presledke
            return data.Split(',')
                       .Select(p => p.Trim())
                       .Where(p => !string.IsNullOrEmpty(p))
                       .ToList();
        }

        private List<string> NajdiNovo(HashSet<string> data1, List<string> data2)
        {
            return data2.Where(parcel => !data1.Contains(parcel)).ToList();
        }

        private List<string> NajdiOdstranjeno(HashSet<string> data2, List<string> data1)
        {
            return data1.Where(parcel => !data2.Contains(parcel)).ToList();
        }

        private void NapolniComboBoxe()
        {
            // Ustvari seznam let od 1996 do 2024
            List<int> leta = Enumerable.Range(1996, 2024 - 1996 + 1).ToList();

            // Napolni ComboBox za leto 1
            Leto1ComboBox.ItemsSource = leta;
            Leto1ComboBox.SelectedItem = 2018; // Privzeto izbrano leto

            // Napolni ComboBox za leto 2
            Leto2ComboBox.ItemsSource = leta;
            Leto2ComboBox.SelectedItem = 2024; // Privzeto izbrano leto
        }

        private void PosodobiOznake()
        {
            // Posodobi oznake za podatke glede na izbrana leta
            Leto1Label.Text = $"Podatki za Leto {Leto1ComboBox.SelectedItem}:";
            Leto2Label.Text = $"Podatki za Leto {Leto2ComboBox.SelectedItem}:";
        }

        // Dogodki za posodabljanje oznak ob spremembi ComboBox-ov
        private void Leto1ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PosodobiOznake();
        }

        private void Leto2ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PosodobiOznake();
        }
    }
}
