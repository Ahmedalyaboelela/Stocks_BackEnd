using BAL.Model;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public static class EntriesHelper
    {

        // Calculate Entry, EntryDetails using Models Case Insert
        public static EntryModel InsertCalculatedEntries(SellingOrderModel sellingOrderModel=null ,
            PurchaseOrderModel purchaseOrderModel=null, ReceiptExchangeModel receiptExchangeModel=null,
            NoticeModel noticeModel=null,Entry LastEntry =null)
        {

            #region Definitions
            decimal TotalNet = 0, TotalBankCommision = 0, TotalTaxOnCommision = 0, TotalSTocksSales = 0, TotalSTocksPurchase = 0;
            List<EntryDetailModel> DetailListModel = new List<EntryDetailModel>();
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
                if (LastEntry.Code == null)
                {
                    Entrymodel.Code = "1";
                }
                else
                {
                    Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
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
                if (LastEntry.Code == null)
                {
                    Entrymodel.Code = "1";
                }
                else
                {
                    Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
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
            #region ReceiptExchange
            if (receiptExchangeModel != null )
            {
                #region ReceiptExchange
       
                    
                    var ReceiptExchangeDetails = receiptExchangeModel.RecExcDetails;
                    #region EntryMaster
                    if (LastEntry.Code == null)
                    {
                        Entrymodel.Code = "1";
                    }
                    else
                    {
                        Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
                    }
                    Entrymodel.Date = DateTime.Now.ToShortDateString();
                    Entrymodel.ReceiptID = receiptExchangeModel.ReceiptID;
                    #endregion

                    #region EntryDetails
                    if(ReceiptExchangeDetails != null)
                    {
                        foreach (var item in ReceiptExchangeDetails)
                        {
                            if(item.Debit!=null)
                            {
                                //Add Debit Accounts with values
                                #region Debit
                                EntryDetailModel DetailModel = new EntryDetailModel();
                                DetailModel.Debit = item.Debit;
                                DetailModel.AccountID = item.AccountID;
                                DetailListModel.Add(DetailModel);
                                #endregion
                            }
                            if(item.Credit !=null)
                            {

                                //Add Credit Accounts with values
                                #region Credit
                                EntryDetailModel DetailModel = new EntryDetailModel();
                                DetailModel.Credit = item.Credit;
                                DetailModel.AccountID = item.AccountID;
                                DetailListModel.Add(DetailModel);
                                #endregion
                            }
                        }


                        Entrymodel.EntryDetailModel = DetailListModel;
                    }

                    #endregion
                
                #endregion

                

            }
            #endregion
            #region NoticeDebitCredit
            if (noticeModel != null)
            {
                #region ReceiptExchange


                var NoticeDetails = noticeModel.NoticeModelDetails;
                #region EntryMaster
                if (LastEntry.Code == null)
                {
                    Entrymodel.Code = "1";
                }
                else
                {
                    Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
                }
                Entrymodel.Date = DateTime.Now.ToShortDateString();
                Entrymodel.NoticeID = noticeModel.NoticeID;
                #endregion

                #region EntryDetails
                if (NoticeDetails != null)
                {
                    foreach (var item in NoticeDetails)
                    {
                        if (item.Debit != null )
                        {
                            //Add Debit Accounts with values
                            #region Debit
                            EntryDetailModel DetailModel = new EntryDetailModel();
                            DetailModel.Debit = item.Debit;
                            if (item.StocksDebit != null)
                                DetailModel.StocksDebit = item.StocksDebit;
                            DetailModel.AccountID = item.AccountID;
                            DetailListModel.Add(DetailModel);
                            #endregion
                        }
                        if (item.Credit != null)
                        {

                            //Add Credit Accounts with values
                            #region Credit
                            EntryDetailModel DetailModel = new EntryDetailModel();
                            DetailModel.Credit = item.Credit;
                            if (item.StocksCredit != null)
                                DetailModel.StocksCredit = item.StocksCredit;
                            DetailModel.AccountID = item.AccountID;
                            DetailListModel.Add(DetailModel);
                            #endregion
                        }
                    }


                    Entrymodel.EntryDetailModel = DetailListModel;
                }

                #endregion

                #endregion



            }
            #endregion
            return Entrymodel;

        }

        // Calculate EntryDetails using Models Case Update
        public static List<EntryDetailModel> UpdateCalculateEntries(int EntryID,SellingOrderModel sellingOrderModel = null,
            PurchaseOrderModel purchaseOrderModel = null, ReceiptExchangeModel receiptExchangeModel = null,
            NoticeModel noticeModel = null)
        {
            #region Definitions
            decimal TotalNet = 0, TotalBankCommision = 0, TotalTaxOnCommision = 0, TotalSTocksSales = 0, TotalSTocksPurchase = 0;
            List<EntryDetailModel> DetailListModel = new List<EntryDetailModel>();
            int AccBankCommision = 0, AccTaxonCommision = 0, AccSalesStocks = 0, AccPuchaseStocks = 0;
            EntryModel Entrymodel = new EntryModel();

            #endregion
            #region SellingOrder
            if (sellingOrderModel != null)
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


                #region EntryDetails
                //Add Debit Accounts with values
                #region Debit
                EntryDetailModel DetailModel1 = new EntryDetailModel();
                DetailModel1.Debit = TotalNet;
                DetailModel1.AccountID = sellingOrderModel.PortfolioAccount;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel1);
                EntryDetailModel DetailModel2 = new EntryDetailModel();
                DetailModel2.Debit = TotalBankCommision;
                DetailModel2.AccountID = AccBankCommision;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel2);
                EntryDetailModel DetailModel3 = new EntryDetailModel();
                DetailModel3.Debit = TotalTaxOnCommision;
                DetailModel3.AccountID = AccTaxonCommision;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel3);
                #endregion

                //Add Credit Accounts with values
                #region Credit
                EntryDetailModel DetailModel4 = new EntryDetailModel();
                DetailModel4.Credit = TotalSTocksSales;
                DetailModel4.AccountID = AccSalesStocks;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel4);
                #endregion
          
                #endregion

            }
            #endregion
            #region PurchaseOrder  
            if (purchaseOrderModel != null)
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

          

                #region EntryDetails
                //Add Debit Accounts with values
                #region Debit
                EntryDetailModel DetailModel1 = new EntryDetailModel();
                DetailModel1.Debit = TotalSTocksPurchase;
                DetailModel1.AccountID = purchaseOrderModel.PortfolioAccount;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel1);
                EntryDetailModel DetailModel2 = new EntryDetailModel();
                DetailModel2.Debit = TotalBankCommision;
                DetailModel2.AccountID = AccBankCommision;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel2);
                EntryDetailModel DetailModel3 = new EntryDetailModel();
                DetailModel3.Debit = TotalTaxOnCommision;
                DetailModel3.AccountID = AccTaxonCommision;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel3);
                #endregion

                //Add Credit Accounts with values
                #region Credit
                EntryDetailModel DetailModel4 = new EntryDetailModel();
                DetailModel4.Credit = TotalNet;
                DetailModel4.AccountID = AccPuchaseStocks;
                DetailModel1.EntryID = EntryID;
                DetailListModel.Add(DetailModel4);
                #endregion
  
                #endregion
            }
            #endregion
            #region ReceiptExchange
            if (receiptExchangeModel != null)
            {
                #region ReceiptExchange


                var ReceiptExchangeDetails = receiptExchangeModel.RecExcDetails;
        

                #region EntryDetails
                if (ReceiptExchangeDetails != null)
                {
                    foreach (var item in ReceiptExchangeDetails)
                    {
                        if (item.Debit != null)
                        {
                            //Add Debit Accounts with values
                            #region Debit
                            EntryDetailModel DetailModel = new EntryDetailModel();
                            DetailModel.Debit = item.Debit;
                            DetailModel.AccountID = item.AccountID;
                            DetailModel.EntryID = EntryID;
                            DetailListModel.Add(DetailModel);
                            #endregion
                        }
                        if (item.Credit != null)
                        {

                            //Add Credit Accounts with values
                            #region Credit
                            EntryDetailModel DetailModel = new EntryDetailModel();
                            DetailModel.Credit = item.Credit;
                            DetailModel.AccountID = item.AccountID;
                            DetailModel.EntryID = EntryID;
                            DetailListModel.Add(DetailModel);
                            #endregion
                        }
                    }


                  
                }

                #endregion

                #endregion



            }
            #endregion
            #region NoticeDebitCredit
            if (noticeModel != null)
            {
                #region ReceiptExchange


                var NoticeDetails = noticeModel.NoticeModelDetails;
          

                #region EntryDetails
                if (NoticeDetails != null)
                {
                    foreach (var item in NoticeDetails)
                    {
                        if (item.Debit != null)
                        {
                            //Add Debit Accounts with values
                            #region Debit
                            EntryDetailModel DetailModel = new EntryDetailModel();
                            DetailModel.Debit = item.Debit;
                            if (item.StocksDebit != null)
                                DetailModel.StocksDebit = item.StocksDebit;
                            DetailModel.AccountID = item.AccountID;
                            DetailModel.EntryID = EntryID;
                            DetailListModel.Add(DetailModel);
                            #endregion
                        }
                        if (item.Credit != null)
                        {

                            //Add Credit Accounts with values
                            #region Credit
                            EntryDetailModel DetailModel = new EntryDetailModel();
                            DetailModel.Credit = item.Credit;
                            if (item.StocksCredit != null)
                                DetailModel.StocksCredit = item.StocksCredit;
                            DetailModel.AccountID = item.AccountID;
                            DetailModel.EntryID = EntryID;
                            DetailListModel.Add(DetailModel);
                            #endregion
                        }
                    }


                
                }

                #endregion

                #endregion



            }
            #endregion
            return DetailListModel;
        }

  


    }
}
