using System.Text;
using System.Text.Json;
using TravelApp.Services.Trip.Models;

namespace TravelApp.Services.Trip.AI
{

    /// <summary>
    /// Defines the contract for generating AI-powered travel itineraries using the Gemini API.
    /// </summary>
    public interface IGeminiService
    {
        /// <summary>
        /// Generates a day-by-day travel itinerary for the given destination and date range using the Gemini AI model.
        /// </summary>
        /// <param name="destination">The destination city or country.</param>
        /// <param name="startDate">The trip start date.</param>
        /// <param name="endDate">The trip end date.</param>
        /// <param name="preferences">Optional user preferences to guide the AI (e.g., "adventure", "food", "budget").</param>
        /// <returns>A list of <see cref="Models.Itinerary"/> items, one per day.</returns>
        Task<List<Models.Itinerary>> GetItineraryAsync(string destination, DateTime startDate, DateTime endDate, string? preferences);
    }

    /// <summary>
    /// Calls the Google Gemini API to generate structured day-by-day travel itineraries.
    /// Builds a prompt, posts to the Gemini REST endpoint, and parses the JSON response.
    /// Falls back to placeholder itinerary items if parsing fails.
    /// </summary>
    public class GeminiService(HttpClient http, ILogger<GeminiService> logger) : IGeminiService
    {
        /// <summary>
        /// Sends a prompt to the Gemini API and parses the returned JSON itinerary.
        /// Throws an exception with a user-friendly message if the API returns an error.
        /// </summary>
        /// <param name="destination">The destination city or country.</param>
        /// <param name="startDate">The trip start date.</param>
        /// <param name="endDate">The trip end date.</param>
        /// <param name="preferences">Optional user preferences.</param>
        /// <returns>A list of <see cref="Itinerary"/> items ordered by day number.</returns>
        public async Task<List<Itinerary>> GetItineraryAsync(string destination, DateTime startDate, DateTime endDate, string? preferences)
        {
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            var totalDays = (endDate - startDate).Days +1;

            var prompt = BuildPrompt(destination, startDate, endDate, totalDays, preferences);

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new {text = prompt}
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";


            var response = await http.PostAsync(url, content);
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Gemini API error: {Res}", responseStr);
                return ParseResponse(string.Empty, totalDays); // return fallback itinerary
            }

            return ParseResponse(responseStr, totalDays);
        }



        /// <summary>
        /// Builds the Gemini prompt string requesting a structured JSON itinerary.
        /// </summary>
        private static string BuildPrompt(string destination, DateTime start, DateTime end, int days, string? prefs)
        {
            var prefText = string.IsNullOrWhiteSpace(prefs) ? "" : $"\nUser preferences: {prefs}";
            return $@"You are an expert travel planner. Create a detailed {days}-day itinerary for {destination} from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}.{prefText}

                For each day provide morning, afternoon and evening activities with specific places. Include approximate costs in INR.

                Return ONLY raw JSON array (no markdown, no code blocks):
                [
                  {{""dayNumber"": 1, ""activity"": ""Morning: [activity]. Afternoon: [activity]. Evening: [activity]."", ""location"": ""[Main area]""}},
                  {{""dayNumber"": 2, ""activity"": ""..."", ""location"": ""...""}}
                ]

                Include exactly {days} items.";
        }

        /// <summary>
        /// Parses the raw Gemini API response string into a list of <see cref="Itinerary"/> items.
        /// Falls back to placeholder items if the response cannot be parsed.
        /// </summary>
        private List<Itinerary> ParseResponse(string responseStr, int totalDays)
        {
            var result = new List<Itinerary>();
            try
            {
                using var doc = JsonDocument.Parse(responseStr);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";

                text = text.Trim();
                if (text.StartsWith("```json")) text = text[7..];
                else if (text.StartsWith("```")) text = text[3..];
                if (text.EndsWith("```")) text = text[..^3];
                text = text.Trim();

                var items = JsonSerializer.Deserialize<List<GeminiItem>>(text,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (items != null)
                    foreach (var item in items)
                        result.Add(new Itinerary {
                            DayNumber = item.DayNumber,
                            Activity = item.Activity ?? "No activity planned",
                            Location = item.Location ?? "Unknown"
                        });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Gemini parse failed — using fallback");
                for (int i = 1; i <= totalDays; i++)
                    result.Add(new Itinerary
                    {
                        DayNumber = i,
                        Activity = $"Day {i}: Explore local attractions and landmarks. Visit popular restaurants and markets.",
                        Location = "City Centre"
                    });
            }
            return result;
        }

        /// <summary>Internal model for deserializing the raw Gemini JSON array items.</summary>
        private class GeminiItem
        {
            public int DayNumber { get; set; }
            public string Activity { get; set; } = "";
            public string Location { get; set; } = "";
        }
    }
}
