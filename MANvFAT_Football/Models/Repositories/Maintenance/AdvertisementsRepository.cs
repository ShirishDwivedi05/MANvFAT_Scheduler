using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.Models.Repositories
{
    public class AdvertisementsRepository : BaseRepository
    {
        public List<SelectAdvertisementPercentAge_Result> Read_Dashboard()
        {
            return db.SelectAdvertisementPercentAge().ToList();
        }
        public List<AdvertisementsExt> ReadAll()
        {
            return db.Advertisements.OrderBy(o => o.SortBy).ToList().Select(m => Map(m)).ToList();
        }

        public List<string> ReadAll_OtherDetailsList()
        {
            return db.Players.Where(m => m.AdvertisementOther == true && m.AdvertisementOtherDetails!= "Before Question Asked").Select(m => m.AdvertisementOtherDetails).ToList();
        }

        public Advertisements Map(AdvertisementsExt model)
        {
            Advertisements tblModel = new Advertisements()
            {
                AdvertisementID = model.AdvertisementID,
                Advertisement = model.Advertisement,
                SortBy = model.SortBy
            };

            return tblModel;
        }

        public AdvertisementsExt Map(Advertisements model)
        {
            AdvertisementsExt tblModel = new AdvertisementsExt()
            {
                AdvertisementID = model.AdvertisementID,
                Advertisement = model.Advertisement,
                SortBy = model.SortBy
            };

            return tblModel;
        }

        public void MapUpdate(ref Advertisements dbmodel, AdvertisementsExt model)
        {
            dbmodel.AdvertisementID = model.AdvertisementID;
            dbmodel.Advertisement = model.Advertisement;
            dbmodel.SortBy = model.SortBy;
        }
    }

    public class AdvertisementsExt
    {
        public long AdvertisementID { get; set; }
        public string Advertisement { get; set; }
        public int SortBy { get; set; }
    }
}