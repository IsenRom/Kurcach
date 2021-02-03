using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Forms;
using mapa.Classes;
using System.Net.Http;

using System.Web;
using Newtonsoft.Json;




namespace mapa // сделали список мап обжектов в виде массива, что бы отключать маркеры выбранного исполнителя
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<GeoClass.Artists> mapObjects = new List<GeoClass.Artists>();

        List<GeoClass.Artists> sortmapObject;

        public bool bool_first = true;

        public List<GeoClass.Artists[]> artists_list = new List<GeoClass.Artists[]>();

        string apikey = "33ef8f94b2739a88ee6db22fa3ced553";

        public List<GeoClass.Artists> sorted;

        http client;

        public MainWindow()
        {
            InitializeComponent();
            initMap();
            sortedtb.DisplayMemberPath = "_square";
        }

        public void initMap()
        {
            // настройка параметров карты
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = OpenStreetMapProvider.Instance;
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            Map.Position = new PointLatLng(55.012823, 82.950359);
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;

        }

        async Task getResponseAsync(string artName)
        {
            client = new http();
            string responseBody = await client.getAnsv(artName, apikey);

            GeoClass.Artists[] artistsFromResponce = GeoClass.Artists.FromJson(responseBody);
            List<GeoClass.Artists> artistsWithoutSTREAM = new List<GeoClass.Artists>();

            foreach (GeoClass.Artists p in artistsFromResponce)
                if (p.Venue.Latitude != null)
                    artistsWithoutSTREAM.Add(p);

            GeoClass.Artists[] artists = artistsWithoutSTREAM.ToArray<GeoClass.Artists>();

            if ((responseBody != "\n\n[]\n") && (artists.Count() != 0))
            {
                artists_list.Add(artists);
                listbox_artists.Items.Add(artists[0].Lineup[0]);
                metod();
            }
            else
            {
                System.Windows.MessageBox.Show("Концерты " + artName + " не найдены");
            };
        }

        private void metod(string city)

        {
            Map.Markers.Clear();
            sortedtb.Items.Clear();
            sortmapObject = new List<GeoClass.Artists>();
            sorted = new List<GeoClass.Artists>();
            foreach (GeoClass.Artists[] temp in artists_list)
                foreach (GeoClass.Artists artist in temp)
                    if (artist.Venue.Country == city)
                    {
                        sorted.Add(artist);
                        string lat_str = artist.Venue.Latitude.Replace(".", ",");
                        string lng_str = artist.Venue.Longitude.Replace(".", ",");

                        double lat = Convert.ToDouble(lat_str);
                        double lng = Convert.ToDouble(lng_str);

                        GeoClass.Artists mapObject = new GeoClass.Artists();
                        mapObject.GeoClass(new PointLatLng(lat, lng), artist.Venue.Location);
                        sortmapObject.Add(mapObject);
                        Map.Markers.Add(mapObject.getMarker);
                    }
            foreach (GeoClass.Artists temp in sorted)
                //sortedtb.Items.Add(temp.Venue.Country + " " + temp.Venue.City + " " + temp.Venue.Name);
                sortedtb.Items.Add(temp);


        }

        private void metod()
        {

            if (artists_list.Count != 0)
            {
                foreach (GeoClass.Artists i in artists_list.Last())
                {

                    if (i.Venue.Latitude != null)
                    {
                        string lat_str = i.Venue.Latitude.Replace(".", ",");
                        string lng_str = i.Venue.Longitude.Replace(".", ",");

                        double lat = Convert.ToDouble(lat_str);
                        double lng = Convert.ToDouble(lng_str);

                        GeoClass.Artists mapObject = new GeoClass.Artists();
                        mapObject.GeoClass(new PointLatLng(lat, lng), i.Venue.Location);
                        mapObjects.Add(mapObject);
                        Map.Markers.Add(mapObject.getMarker);

                    }
                }

            }

        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await getResponseAsync(tb_name_art.Text);

            }
            catch (System.Net.Http.HttpRequestException)
            {
                System.Windows.MessageBox.Show("Исполнитель не найден");
            }


        }
        // при двойном нажатии убираем подсказку "имя исполнителя" из текстового поля
        private void tb_name_art_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tb_name_art.Text = "";
        }

        private void listbox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            party_list.Items.Clear();
            foreach (GeoClass.Artists ar in artists_list[listbox_artists.SelectedIndex])
            {
                party_list.Items.Add(ar.Venue.Country + " " + ar.Venue.City + " " + ar.Venue.Name);
            }

        }

        private void party_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (party_list.SelectedIndex != -1)
            {
                int index = 0;

                for (int i = 0; i < listbox_artists.SelectedIndex; i++)
                {
                    index = index + artists_list[i].Count();
                }
                // при нажатии на концерт из списка фокусируем карту
                index = index + party_list.SelectedIndex;
                Map.Position = mapObjects[index].getFocus;
                // выводим  данные об этом концерте 
                lab_art_date.Content = "Дата проведения: " + (((artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Datetime).ToString());
                lab_art_counrty.Content = "Страна мероприятия: " + (artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Venue.Country.ToString();
                lab_art_city.Content = "Город мероприятия: " + (artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Venue.City.ToString();
                lab_art_name.Content = "Название артиста: " + (artists_list[listbox_artists.SelectedIndex][0]).Lineup[0].ToString();
                lab_art_place.Content = "Название площадки: " + (artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Venue.Name.ToString();
            }
        }




        private void citysearchbut_Click(object sender, RoutedEventArgs e)
        {
            metod(citysearchtext.Text);
        }


        private void sortedtb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sortedtb.SelectedIndex != -1)
            {
                GeoClass.Artists artists = sortedtb.SelectedItem as GeoClass.Artists;
                Map.Position = artists.reGetFocus;
                lab_art_name.Content = "Название артиста: " + artists.Lineup[0];
                lab_art_date.Content = "Дата проведения: " + artists.Datetime.ToString();
                lab_art_counrty.Content = "Страна мероприятия: " + artists.Venue.Country;
                lab_art_city.Content = "Город мероприятия: " + artists.Venue.City;
                lab_art_place.Content = "Название площадки: " + artists.Venue.Name;
            }
        }

        private void rstbut_Click(object sender, RoutedEventArgs e)
        {
            foreach (GeoClass.Artists temp in mapObjects)
            {
                Map.Markers.Add(temp.getMarker);
            }
            sortedtb.Items.Clear();
        }
    }

}