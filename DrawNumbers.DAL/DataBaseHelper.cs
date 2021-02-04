using System.Collections.Generic;
using System.Linq;
using EntityFramework.BulkInsert.Extensions;

namespace DrawNumbers.DAL
{
    /// <summary>
    /// Manages access of database content
    /// </summary>
    public class DataBaseHelper
    {
        private static List<DrawnNumber> drawnNumbers;

        /// <summary>
        /// Get or add drawn numbers
        /// </summary>
        public List<DrawnNumber> DrawnNumbers
        {
            get
            {
                if (drawnNumbers == null)
                    InitialLoad();

                return drawnNumbers;
            }

            set
            {
                drawnNumbers.AddRange(value);
                using (var db = new DatabaseEntities())
                    db.BulkInsert(value);
            }
        }


        /// <summary>
        /// Number of records in data base
        /// </summary>
        public int Count
        {
            get
            {
                using (var db = new DatabaseEntities())
                {
                    return (from data in db.DrawnNumber
                            select data).Count();
                }
            }
        }

        /// <summary>
        /// Initial load from data base
        /// </summary>
        private void InitialLoad()
        {
            var count = Count;
            var step = 1000000;

            if (count > step)
            {
                drawnNumbers = new List<DrawnNumber>();
                int readNumbers = 0;
                
                while (readNumbers < count)
                {
                    using (var db = new DatabaseEntities())
                    {
                        drawnNumbers.AddRange((from data in db.DrawnNumber
                                               orderby data.Value
                                               select data).Skip(readNumbers).Take(step));
                    }

                    readNumbers += step;
                    step = ((count - readNumbers) / step) > 0 ? step : (count - readNumbers);
                }
            }
            else
            {
                using (var db = new DatabaseEntities())
                {
                    drawnNumbers = (from data in db.DrawnNumber
                                   select data).ToList();
                }
            }
        }
    }
}
