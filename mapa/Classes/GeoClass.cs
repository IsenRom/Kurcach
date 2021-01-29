using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using GMap.NET.WindowsPresentation;
using System.Windows.Shapes;
using System.Windows.Media;
using GMap.NET;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace mapa.Classes
{
    public class GeoClass  
    {
        PointLatLng point = new PointLatLng();

        string name;


        public GeoClass(PointLatLng point, string name)
        {
            
            this.point = point;
            this.name = name;

        }

        public  PointLatLng getFocus()
        {
            return point;
        }

        public  GMapMarker GetMarker()
        {
            GMapMarker marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Margin = new System.Windows.Thickness(-16, -16, 0, 0),
                    Width = 32, // ширина маркера
                    Height = 32, // высота маркера                  
                    ToolTip = name,
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/001.png")) // картинка
                }
            };
            return marker;
        }
     
        public partial class Artists
        {
            [JsonProperty("offers")]
            public Offer[] Offers { get; set; }

            [JsonProperty("venue")]
            public Venue Venue { get; set; }

            [JsonProperty("datetime")]
            public DateTimeOffset Datetime { get; set; }

            [JsonProperty("artist", NullValueHandling = NullValueHandling.Ignore)]
            public Artist Artist { get; set; }

            [JsonProperty("on_sale_datetime")]
            public string OnSaleDatetime { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("lineup")]
            public string[] Lineup { get; set; }

            [JsonProperty("id")]
            [JsonConverter(typeof(ParseStringConverter))]
            public long Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("artist_id")]
            [JsonConverter(typeof(ParseStringConverter))]
            public long ArtistId { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }

            
        }

        public partial class Artist
        {
            [JsonProperty("thumb_url")]
            public Uri ThumbUrl { get; set; }

            [JsonProperty("mbid")]
            public string Mbid { get; set; }

            [JsonProperty("support_url")]
            public string SupportUrl { get; set; }

            [JsonProperty("facebook_page_url")]
            public Uri FacebookPageUrl { get; set; }

            [JsonProperty("image_url")]
            public Uri ImageUrl { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("options")]
            public Options Options { get; set; }

            [JsonProperty("id")]
            [JsonConverter(typeof(ParseStringConverter))]
            public long Id { get; set; }

            [JsonProperty("tracker_count")]
            public double TrackerCount { get; set; }

            [JsonProperty("upcoming_event_count")]
            public double UpcomingEventCount { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }
        }

        public partial class Options
        {
            [JsonProperty("display_listen_unit")]
            public bool DisplayListenUnit { get; set; }
        }

        public partial class Offer
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public partial class Venue
        {
            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("latitude")]
            public string Latitude { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("longitude")]
            public string Longitude { get; set; }

            [JsonProperty("_squareName")]
            public string _squareName { get { return Name; } set { _squareName = value; } }
            
        }



        public partial class Artists // самый главнный класс
        {
            public static Artists[] FromJson(string json) => JsonConvert.DeserializeObject<Artists[]>(json, Converter.Settings);

            

        }



        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

        internal class ParseStringConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                long l;
                if (Int64.TryParse(value, out l))
                {
                    return l;
                }
                throw new Exception("Cannot unmarshal type long");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (long)untypedValue;
                serializer.Serialize(writer, value.ToString());
                return;
            }

            public static readonly ParseStringConverter Singleton = new ParseStringConverter();


        }

    }

    

   

        

    }

    


