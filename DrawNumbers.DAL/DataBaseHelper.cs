using System.Collections.Generic;
using System.Linq;

namespace DrawNumbers.DAL
{
    /// <summary>
    /// Manages access of database content
    /// </summary>
    public class DataBaseHelper
    {
        private readonly DatabaseEntities context;
        private readonly object drawnNumberLock = new object();
        private static List<DrawnNumber> drawnNumbers;

        /// <summary>
        /// Get or add drawn numbers
        /// </summary>
        public List<DrawnNumber> DrawnNumbers
        {
            get
            {
                lock (drawnNumberLock)
                {
                    if (drawnNumbers == null)
                        drawnNumbers = context.DrawnNumber.ToList();
                    
                    return drawnNumbers;
                };
            }

            private set
            {
                lock (drawnNumberLock)
                {
                    drawnNumbers.AddRange(value);
                    context.DrawnNumber.AddRange(value);
                }
            }
        }

        public DataBaseHelper()
        {
            context = new DatabaseEntities();
        }


        /// <summary>
        /// Add number to data base
        /// </summary>
        /// <param name="number"></param>
        public void Add(List<DrawnNumber> numbers)
        {
            this.DrawnNumbers = numbers;
        }

        /// <summary>
        /// Save all changes in database
        /// </summary>
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
