using TempBackend.Models;

partial class Program
{
    static void Main()
    {
        var place = new Place
        {
            Name = "Hidden Gem Cafe",
            Address = "123 Street",
            Latitude = 30.0,
            Longitude = 31.0,
            OpeningHours = "9 AM - 9 PM",
            AveragePrice = 25.0m,
            CategoryId = 1
        };

        Console.WriteLine("Place created: " + place.Name);
    }
}
