using BAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public static class EntriesHelper
    {

        // Calculate Entry, EntryDetails using SellingorderModel 
        public static EntryModel CalculateSellingEntry(SellingOrderModel sellingOrderModel ,EntryModel LastEntryModel)
        {

            #region Definitions
            decimal TotalNet = 0, TotalBankCommision = 0, TotalTaxOnCommision = 0, TotalSTocksSales = 0;
            var SellDetails = sellingOrderModel.DetailsModels;
            List<EntryDetailModel> DetailListModel = null;
            var SettingAccmodel = sellingOrderModel.SettingModel.SettingAccs;
            int AccBankCommision = 0, AccTaxonCommision = 0, AccSalesStocks = 0;
            #endregion

            //Get EntryDetail Accounts From Setting
            #region SettingAccounts
            foreach (var Accs in SettingAccmodel)
            {
               if(Accs.AccountType==1)
                {
                    AccBankCommision = Accs.AccountID;
                }
               else if(Accs.AccountType == 2)
                {
                    AccTaxonCommision= Accs.AccountID;
                }
               else
                {
                    AccSalesStocks = Accs.AccountID;
                }
            }
            #endregion

            #region Calculate EntryDetailsValues
            if (SellDetails != null)
            {
                foreach (var item in SellDetails)
                {
                    TotalNet += item.NetAmmount;
                    TotalBankCommision += item.BankCommission;
                    TotalTaxOnCommision += item.TaxOnCommission;
                    TotalSTocksSales += item.SelingValue;
                }
            }
            #endregion

            #region EntryMaster
            EntryModel Entrymodel = new EntryModel();
            if (LastEntryModel.Code == null)
            {
                Entrymodel.Code = "1";
            }
            else
            {
                Entrymodel.Code = (int.Parse(LastEntryModel.Code) + 1).ToString();
            }
            Entrymodel.Date = DateTime.Now.ToShortDateString();
            Entrymodel.SellingOrderID = sellingOrderModel.SellingOrderID;
            #endregion

            #region EntryDetails
            //Add Debit Accounts with values
            #region Debit
            EntryDetailModel DetailModel1 = new EntryDetailModel();
            DetailModel1.Debit = TotalNet;
            DetailModel1.AccountID = sellingOrderModel.PortfolioAccount;
            DetailListModel.Add(DetailModel1);
            EntryDetailModel DetailModel2 = new EntryDetailModel();
            DetailModel2.Debit = TotalBankCommision;
            DetailModel2.AccountID = AccBankCommision;
            DetailListModel.Add(DetailModel2);
            EntryDetailModel DetailModel3 = new EntryDetailModel();
            DetailModel3.Debit = TotalTaxOnCommision;
            DetailModel3.AccountID = AccTaxonCommision;
            DetailListModel.Add(DetailModel3);
            #endregion

            //Add Credit Accounts with values
            #region Credit
            EntryDetailModel DetailModel4 = new EntryDetailModel();
            DetailModel4.Credit = TotalSTocksSales;
            DetailModel4.AccountID = AccSalesStocks;
            DetailListModel.Add(DetailModel4);
            #endregion
            Entrymodel.EntryDetailModel = DetailListModel;
            #endregion

            return Entrymodel;
        }



   
    }
}
