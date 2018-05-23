using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace MANvFAT_Football.Models.Repositories
{/// <summary>
 /// BaseRepository handles creation and disposal of a db context
 /// </summary>
    public class BaseRepository : IDisposable
    {

        protected DBEntities db;


        #region Constructors


        /// <summary>
        /// An instance of the repository which creates a new db context itself
        /// </summary>
        public BaseRepository()
        {
            db = new DBEntities();
            db.Database.CommandTimeout = 300;
        }

        /// <summary>
        /// An instance of the repository into which a db context is passed, for use in transactions
        /// </summary>
        public BaseRepository(DBEntities dbContext)
        {
            db = dbContext;
        }

        #endregion


        public void SaveDBChanges()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Call this before all database transactions, so that we can step through them without encountering a timeout
        /// </summary>
        protected static TransactionOptions SetTransactionTimeoutForDebugging(HttpContext httpContext)
        {

            TransactionOptions transOptions = new TransactionOptions();

            if (httpContext.IsDebuggingEnabled)
            {
                //If we're debugging then allow a long time before the transaction times out, to prevent errors when stepping through the code
                transOptions.Timeout = TransactionManager.MaximumTimeout;

            }

            return transOptions;

        }



        #region Disposal Methods

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion




    }
}