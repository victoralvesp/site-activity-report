using System;

namespace CrossOver.WebsiteActivity.Models
{
    public record Activity(string Key, int Value)
    {
        public DateTime RegisterDate { get; init; } = DateTime.UtcNow;
    }
}