using System;
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

        /// <summary>
        /// Get or add drawn numbers
        /// </summary>
        public List<DrawnNumber> DrawnNumbers
        {
            get
            {
                lock (drawnNumberLock)
                {
                    return context.DrawnNumber.ToList();
                };
            }

            private set
            {
                lock (drawnNumberLock)
                {
                    context.DrawnNumber.AddRange(value);
                }
            }
        }

        public DataBaseHelper()
        {
            context = new DatabaseEntities();
            
            // TEST !!!!!!!!!!!!!
            //RemoveAll();
        }


        /// <summary>
        /// Add number to data base
        /// </summary>
        /// <param name="number"></param>
        public void Add(DrawnNumber number)
        {
            this.DrawnNumbers = new List<DrawnNumber> { number };
            Console.WriteLine($"Added number: {number.Value}");
        }

        /// <summary>
        /// Save all changes in database
        /// </summary>
        public void Save()
        {
            context.SaveChanges();
        }

        /// <summary>
        /// Remove all numbers from database
        /// </summary>
        public void RemoveAll()
        {
            var data = from n in context.DrawnNumber
                       select n;
            foreach (var number in data)
                context.DrawnNumber.Remove(number);

            context.SaveChanges();
        }
    }
}
