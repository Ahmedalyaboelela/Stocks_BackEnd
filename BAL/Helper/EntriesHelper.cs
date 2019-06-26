﻿using BAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public static class EntriesHelper
    {

        // Calculate Entry, EntryDetails using SellingorderModel 
        public static EntryModel CalculateSellingEntry(SellingOrderModel sellingOrderModel=null ,
            PurchaseOrderModel purchaseOrderModel=null,EntryModel LastEntryModel=null)
        {

            #region Definitions
            decimal TotalNet = 0, TotalBankCommision = 0, TotalTaxOnCommision = 0, TotalSTocksSales = 0, TotalSTocksPurchase = 0;
            List<EntryDetailModel> DetailListModel = null;
            int AccBankCommision = 0, AccTaxonCommision = 0, AccSalesStocks = 0, AccPuchaseStocks=0;
            EntryModel Entrymodel = new EntryModel();

            #endregion
            #region SellingOrder
            if (sellingOrderModel !=null)
            {
                var SettingAccsell = sellingOrderModel.SettingModel.SettingAccs;
                var SellDetails = sellingOrderModel.DetailsModels;
                //Get EntryDetail Accounts From Setting
                #region SettingAccounts
                foreach (var Accs in SettingAccsell)
                {
                    if (Accs.AccountType == 1)
                    {
                        AccBankCommision = Accs.AccountID;
                    }
                    else if (Accs.AccountType == 2)
                    {
                        AccTaxonCommision = Accs.AccountID;
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
        
            }
            #endregion
            #region PurchaseOrder  
            if (purchaseOrderModel !=null)
            {
                var SettingAccpurchase = purchaseOrderModel.SettingModel.SettingAccs;
                var PurchaseDetails = purchaseOrderModel.DetailsModels;
                //Get EntryDetail Accounts From Setting
                #region SettingAccounts
                foreach (var Accs in SettingAccpurchase)
                {
                    if (Accs.AccountType == 1)
                    {
                        AccPuchaseStocks = Accs.AccountID;
                    }
                    else if (Accs.AccountType == 2)
                    {
                        AccBankCommision = Accs.AccountID;
                    }
                    else
                    {
                        AccTaxonCommision = Accs.AccountID;
                    }
                }
                #endregion

                #region Calculate EntryDetailsValues
                if (PurchaseDetails != null)
                {
                    foreach (var item in PurchaseDetails)
                    {
                        TotalNet += item.NetAmmount;
                        TotalBankCommision += item.BankCommission;
                        TotalTaxOnCommision += item.TaxOnCommission;
                        TotalSTocksPurchase += item.PurchaseValue;
                    }
                }
                #endregion

                #region EntryMaster
                if (LastEntryModel.Code == null)
                {
                    Entrymodel.Code = "1";
                }
                else
                {
                    Entrymodel.Code = (int.Parse(LastEntryModel.Code) + 1).ToString();
                }
                Entrymodel.Date = DateTime.Now.ToShortDateString();
                Entrymodel.PurchaseOrderID  = purchaseOrderModel.PurchaseOrderID;
                #endregion

                #region EntryDetails
                //Add Debit Accounts with values
                #region Debit
                EntryDetailModel DetailModel1 = new EntryDetailModel();
                DetailModel1.Debit = TotalSTocksPurchase;
                DetailModel1.AccountID = purchaseOrderModel.PortfolioAccount;
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
                DetailModel4.Credit = TotalNet;
                DetailModel4.AccountID = AccPuchaseStocks;
                DetailListModel.Add(DetailModel4);
                #endregion
                Entrymodel.EntryDetailModel = DetailListModel;
                #endregion
            }
            #endregion

            return Entrymodel;

        }



   
    }
}