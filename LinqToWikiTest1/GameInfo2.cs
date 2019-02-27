using Newtonsoft.Json;
using System;

namespace LinqToWikiTest1
{
    public class GameInfo2 : IPrintable
    {
        #region short
        //[JsonProperty("country_of_originLabel")]
        //public string CountryOfOrigin;
        //[JsonProperty("developerLabel")]
        //public string Developer;
        [JsonProperty("gameLabel")]
        public string Game;
        //[JsonProperty("game_modeLabel")]
        //public string GameMode;
        //[JsonProperty("genreLabel")]
        //public string Genre;
        //[JsonProperty("official_websiteLabel")]
        //public string OfficialWebsite;
        //[JsonProperty("part_of_the_seriesLabel")]
        //public string PartOfTheSeries;
        //[JsonProperty("pegi_ratingLabel")]
        //public string PegiRating;
        //[JsonProperty("platformLabel")]
        //public string Platform;
        [JsonProperty("publication_dateLabel")]
        public string PublicationDate;
        //[JsonProperty("publisherLabel")]
        //public string Publisher;
        //[JsonProperty("review_scoreLabel")]
        //public string ReviewScore;
        //[JsonProperty("software_engineLabel")]
        //public string SoftwareEngine;
        //[JsonProperty("titleLabel")]
        //public string Title;
        //[JsonProperty("country_of_origin")]
        //public string CountryOfOriginId;
        //[JsonProperty("developer")]
        //public string DeveloperId;
        [JsonProperty("game")]
        public string GameId;
        //[JsonProperty("game_mode")]
        //public string GameModeId;
        //[JsonProperty("genre")]
        //public string GenreId;
        //[JsonProperty("official_website")]
        //public string OfficialWebsiteId;
        //[JsonProperty("part_of_the_series")]
        //public string PartOfTheSeriesId;
        //[JsonProperty("pegi_rating")]
        //public string PegiRatingId;
        //[JsonProperty("platform")]
        //public string PlatformId;
        //[JsonProperty("publication_date")]
        //public string PublicationDateId;
        //[JsonProperty("publisher")]
        //public string PublisherId;
        //[JsonProperty("review_score")]
        //public string ReviewScoreId;
        //[JsonProperty("software_engine")]
        //public string SoftwareEngineId;
        //[JsonProperty("title")]
        //public string TitleId;
        #endregion

        //#region full
        //[JsonProperty("charactersLabel")]
        //public string Characters;
        //[JsonProperty("country_of_originLabel")]
        //public string CountryOfOrigin;
        //[JsonProperty("creatorLabel")]
        //public string Creator;
        //[JsonProperty("developerLabel")]
        //public string Developer;
        //[JsonProperty("distributionLabel")]
        //public string Distribution;
        //[JsonProperty("distributorLabel")]
        //public string Distributor;
        //[JsonProperty("esrb_ratingLabel")]
        //public string EsrbRating;
        //[JsonProperty("gameLabel")]
        //public string Game;
        //[JsonProperty("game_modeLabel")]
        //public string GameMode;
        //[JsonProperty("genreLabel")]
        //public string Genre;
        //[JsonProperty("narrative_locationLabel")]
        //public string NarrativeLocation;
        //[JsonProperty("official_websiteLabel")]
        //public string OfficialWebsite;
        //[JsonProperty("part_of_the_seriesLabel")]
        //public string PartOfTheSeries;
        //[JsonProperty("pegi_ratingLabel")]
        //public string PegiRating;
        //[JsonProperty("platformLabel")]
        //public string Platform;
        //[JsonProperty("publication_dateLabel")]
        //public string PublicationDate;
        //[JsonProperty("publisherLabel")]
        //public string Publisher;
        //[JsonProperty("review_scoreLabel")]
        //public string ReviewScore;
        //[JsonProperty("software_engineLabel")]
        //public string SoftwareEngine;
        //[JsonProperty("titleLabel")]
        //public string Title;
        //[JsonProperty("characters")]
        //public string CharactersId;
        //[JsonProperty("country_of_origin")]
        //public string CountryOfOriginId;
        //[JsonProperty("creator")]
        //public string CreatorId;
        //[JsonProperty("developer")]
        //public string DeveloperId;
        //[JsonProperty("distribution")]
        //public string DistributionId;
        //[JsonProperty("distributor")]
        //public string DistributorId;
        //[JsonProperty("esrb_rating")]
        //public string EsrbRatingId;
        //[JsonProperty("game")]
        //public string GameId;
        //[JsonProperty("game_mode")]
        //public string GameModeId;
        //[JsonProperty("genre")]
        //public string GenreId;
        //[JsonProperty("narrative_location")]
        //public string NarrativeLocationId;
        //[JsonProperty("official_website")]
        //public string OfficialWebsiteId;
        //[JsonProperty("part_of_the_series")]
        //public string PartOfTheSeriesId;
        //[JsonProperty("pegi_rating")]
        //public string PegiRatingId;
        //[JsonProperty("platform")]
        //public string PlatformId;
        //[JsonProperty("publication_date")]
        //public string PublicationDateId;
        //[JsonProperty("publisher")]
        //public string PublisherId;
        //[JsonProperty("review_score")]
        //public string ReviewScoreId;
        //[JsonProperty("software_engine")]
        //public string SoftwareEngineId;
        //[JsonProperty("title")]
        //public string TitleId;
        //#endregion full

        public override string ToString()
        {
            return $"Game: {Game.Substring(0, Math.Min(Game.Length, 40)),-40} published {PublicationDate}";
        }

        public void Print()
        {
            Console.WriteLine(this.ToString());
        }
    }
}
