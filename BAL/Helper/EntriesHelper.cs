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
        public static EntryModel InsertCalculatedEntries(int portofolioaccount,SellingInvoiceModel sellingInvoiceModel=null ,
             PurchaseInvoiceModel purchaseInvoiceModel =null, ReceiptExchangeModel receiptExchangeModel=null,
            NoticeModel noticeModel=null,Entry LastEntry =null,Entry OldEntry=null)
        {

            #region Definitions
            decimal TotalNet = 0, TotalBankCommision = 0, TotalTaxOnCommision = 0, TotalSTocksSales = 0, TotalSTocksPurchase = 0;
            List<EntryDetailModel> DetailListModel = new List<EntryDetailModel>();
            int AccBankCommision = 0, AccTaxonCommision = 0, AccSalesStocks = 0, AccPuchaseStocks=0;
            EntryModel Entrymodel = new EntryModel();

            #endregion
            #region SellingInvoice
            if (sellingInvoiceModel != null)
            {
                var SettingAccsell = sellingInvoiceModel.SettingModel.SettingAccs;
                var SellDetails = sellingInvoiceModel.DetailsModels;
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
                if(OldEntry !=null)
                {

                    Entrymodel.Code =OldEntry.Code;
                    Entrymodel.Date = OldEntry.Date.Value.ToString("d/M/yyyy");
                    Entrymodel.SellingInvoiceID = sellingInvoiceModel.SellingInvoiceID;
                }
                else
                {
                    if (LastEntry == null)
                    {
                        Entrymodel.Code = "1";
                    }
                    else
                    {
                        Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
                    }
                    Entrymodel.Date = DateTime.Now.ToString("d/M/yyyy");
                    Entrymodel.SellingInvoiceID = sellingInvoiceModel.SellingInvoiceID;
                }
                
                #endregion

                #region EntryDetails
                //Add Debit Accounts with values
                #region Debit
                EntryDetailModel DetailModel1 = new EntryDetailModel();
                DetailModel1.Debit = TotalNet;
                DetailModel1.AccountID = portofolioaccount;
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
            if (purchaseInvoiceModel != null)
            {
                var SettingAccpurchase = purchaseInvoiceModel.SettingModel.SettingAccs;
                var PurchaseDetails = purchaseInvoiceModel.DetailsModels;
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
                if(OldEntry !=null)
                {

                    Entrymodel.Code = OldEntry.Code;
                    Entrymodel.Date = OldEntry.Date.Value.ToString("d/M/yyyy");
                    Entrymodel.PurchaseInvoiceID = purchaseInvoiceModel.PurchaseInvoiceID;
                }
                else
                {
                    if (LastEntry == null)
                    {
                        Entrymodel.Code = "1";
                    }
                    else
                    {
                        Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
                    }
                    Entrymodel.Date = DateTime.Now.ToString("d/M/yyyy");
                    Entrymodel.PurchaseInvoiceID = purchaseInvoiceModel.PurchaseInvoiceID;
                }

                #endregion

                #region EntryDetails
                //Add Debit Accounts with values
                #region Debit
                EntryDetailModel DetailModel1 = new EntryDetailModel();
                DetailModel1.Debit = TotalNet;
                DetailModel1.AccountID = AccPuchaseStocks;
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
                DetailModel4.Credit = TotalSTocksPurchase;
                DetailModel4.AccountID = portofolioaccount;
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
                if(OldEntry != null)
                {

                    Entrymodel.Code = OldEntry.Code;
                    Entrymodel.Date = OldEntry.Date.Value.ToString("d/M/yyyy");
                    Entrymodel.ReceiptID = receiptExchangeModel.ReceiptID;
                }
                else
                {
                    if (LastEntry == null)
                    {
                        Entrymodel.Code = "1";
                    }
                    else
                    {
                        Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
                    }
                    Entrymodel.Date = DateTime.Now.ToString("d/MM/yyyy");
                    Entrymodel.ReceiptID = receiptExchangeModel.ReceiptID;
                }

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
                #region DebitCredit


                var NoticeDetails = noticeModel.NoticeModelDetails;
                #region EntryMaster
                if(OldEntry !=null)
                {

                    Entrymodel.Code = OldEntry.Code;
                    Entrymodel.Date = OldEntry.Date.Value.ToString("d/M/yyyy");
                    Entrymodel.NoticeID = noticeModel.NoticeID;
                }
                else
                {
                    if (LastEntry == null)
                    {
                        Entrymodel.Code = "1";
                    }
                    else
                    {
                        Entrymodel.Code = (int.Parse(LastEntry.Code) + 1).ToString();
                    }
                    Entrymodel.Date = DateTime.Now.ToString("d/MM/yyyy");
                    Entrymodel.NoticeID = noticeModel.NoticeID;
                }

                #endregion

                #region EntryDetails
                if (NoticeDetails != null)
                {
                    foreach (var item in NoticeDetails)
                    {
                        if (item.Debit != null || item.StocksDebit!=null)
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
                        if (item.Credit != null || item.StocksCredit !=null)
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
        public static List<EntryDetailModel> UpdateCalculateEntries(int portofolioaccount,int EntryID,SellingInvoiceModel sellingInvoiceModel = null,
            PurchaseOrderModel purchaseOrderModel = null, ReceiptExchangeModel receiptExchangeModel = null,
            NoticeModel noticeModel = null)
        {
            #region Definitions
            decimal TotalNet = 0, TotalBankCommision = 0, TotalTaxOnCommision = 0, TotalSTocksSales = 0, TotalSTocksPurchase = 0;
            List<EntryDetailModel> DetailListModel = new List<EntryDetailModel>();
            int AccBankCommision = 0, AccTaxonCommision = 0, AccSalesStocks = 0, AccPuchaseStocks = 0;
            EntryModel Entrymodel = new EntryModel();

            #endregion
            #region SellingInvoice
            if (sellingInvoiceModel != null)
            {
                var SettingAccsell = sellingInvoiceModel.SettingModel.SettingAccs;
                var SellDetails = sellingInvoiceModel.DetailsModels;
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
                DetailModel1.AccountID = portofolioaccount;
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
                var PurchaseDetails = purchaseOrderModel.purchaseOrderDetailsModels;
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
                DetailModel1.Debit = TotalNet;
                DetailModel1.AccountID = AccPuchaseStocks;
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
                DetailModel4.Credit = TotalSTocksPurchase;
                DetailModel4.AccountID = portofolioaccount;
                DetailModel4.EntryID = EntryID;
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
                #region DebitCredit


                var NoticeDetails = noticeModel.NoticeModelDetails;
          

                #region EntryDetails
                if (NoticeDetails != null)
                {
                    foreach (var item in NoticeDetails)
                    {
                        if (item.Debit != null || item.StocksDebit != null)
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
                        if (item.Credit != null || item.StocksCredit != null)
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
