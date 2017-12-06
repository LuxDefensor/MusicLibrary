using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MusicLibrary
{
    public class Library
    {
        private List<Country> countries;
        private List<Genre> genres;

        public Library()
        {
            countries = LoadCountries();
            genres = LoadGenres();
        }

        #region Load data
        private List<Genre> LoadGenres()
        {
            List<Genre> result = new List<Genre>();
            string sql = "select id_genre, id_parentgenre, genrename from genres";
            DataTable genres = DataManager.GetData(sql, 30);
            foreach (DataRow row in genres.Rows)
            {
                result.Add(new Genre()
                {
                    ID = (int)row[0],
                    ParentID = (int)row[1],
                    Name = row[1].ToString()
                });
            }
            return result;
        }

        private List<Country> LoadCountries()
        {
            List<Country> result = new List<Country>();
            string sql = "select id_country,countryname from countries";
            DataTable countries = DataManager.GetData(sql, 30);
            foreach (DataRow row in countries.Rows)
            {
                result.Add(new Country() { ID = (int)row[0], Name = row[1].ToString() });
            }
            return result;
        }

        #endregion
    }
}
