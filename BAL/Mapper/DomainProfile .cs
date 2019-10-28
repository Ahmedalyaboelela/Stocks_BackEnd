using AutoMapper;
using BAL.Model;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BAL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            #region Map Account
            CreateMap<Account, AccountModel>();
            CreateMap<AccountModel, Account>().ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
               .ForSourceMember(t => t.NameArParent, opt => opt.DoNotValidate())
               .ForSourceMember(t => t.NameEnParent, opt => opt.DoNotValidate());
            #endregion

            #region Map Partner
            CreateMap<Partner, PartenerModel>();
            CreateMap<PartenerModel, Partner>()
               .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
               .ForSourceMember(t => t.AccountNameAr, opt => opt.DoNotValidate())
               .ForSourceMember(t => t.AccountNameEn, opt => opt.DoNotValidate())
               .ForMember(t => t.IssueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.IssueDate, "d/M/yyyy", CultureInfo.InvariantCulture)))
               .ForMember(t => t.Date, opt=> opt.MapFrom(s=>DateTime.ParseExact(s.Date, "d/M/yyyy", CultureInfo.InvariantCulture)));
            #endregion

            #region Map Country
            CreateMap<Country, CountryModel>();
            CreateMap<CountryModel, Country>();
            #endregion

            #region Map Employee
            // employeee
            CreateMap<Employee, EmployeeModel>();
            CreateMap<EmployeeModel, Employee>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.BirthDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.BirthDate, "d/M/yyyy", CultureInfo.InvariantCulture)));

            // employee card
            CreateMap<EmployeeCard, EmployeeCardModel>();
            CreateMap<EmployeeCardModel, EmployeeCard>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.IssueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.IssueDate, "d/M/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.EndDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.EndDate, "d/M/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.RenewalDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.RenewalDate, "d/M/yyyy", CultureInfo.InvariantCulture)));

            ;

            #endregion


            #region Map Setting
            CreateMap<Setting, SettingModel>();
            CreateMap<SettingModel, Setting>();
          
            #endregion 
            #region Map Selling Invoice

            CreateMap<SellingInvoice, SellingInvoiceModel>();
            CreateMap<SellingInvoiceModel, SellingInvoice>()
                .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.SellDate, "d/M/yyyy", CultureInfo.InvariantCulture)));


            CreateMap<SellingInvoiceDetail, SellingInvoiceDetailsModel>();
            CreateMap<SellingInvoiceDetailsModel , SellingInvoiceDetail>();
            #endregion

            #region Map Purchase order

            CreateMap<PurchaseOrder, PurchaseOrderModel>();
            CreateMap<PurchaseOrderModel, PurchaseOrder>()
                .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.PurchaseDate, "d/M/yyyy", CultureInfo.InvariantCulture)));


            CreateMap<PurchaseOrderDetail, PurchaseOrderDetailModel>();
            CreateMap<PurchaseOrderDetailModel, PurchaseOrderDetail>();

            #endregion

            #region Map Currency

            CreateMap<Currency, CurrencyModel>();
            CreateMap<CurrencyModel, Currency>().ForSourceMember(t => t.Count, opt => opt.DoNotValidate());
            #endregion

            #region Map Portfolio
            // portfolio
            CreateMap<Portfolio, PortfolioModel>();
            CreateMap<PortfolioModel, Portfolio>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.EstablishDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.EstablishDate, "d/M/yyyy", CultureInfo.InvariantCulture)))
                 .ForSourceMember(t => t.RSBalance, opt => opt.DoNotValidate());
               

            // portfolio accounts
            CreateMap<PortfolioAccount, PortfolioAccountModel>();
            CreateMap<PortfolioAccountModel, PortfolioAccount>();


            // portfolio shareholders
            CreateMap<PortfolioOpeningStocks, PortfolioOpeningStocksModel>();

            CreateMap<PortfolioOpeningStocksModel, PortfolioOpeningStocks>();


            #endregion

            #region Map Reciept & Exchange
            // Reciept & Exchange
            CreateMap<ReceiptExchange, ReceiptExchangeModel>();
            CreateMap<ReceiptExchangeModel, ReceiptExchange>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.Date, "d/M/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.ChiqueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.ChiqueDate, "d/M/yyyy", CultureInfo.InvariantCulture)));


            // Reciept & Exchange Details
            CreateMap<ReceiptExchangeDetail, ReceiptExchangeDetailModel>();
            CreateMap<ReceiptExchangeDetailModel, ReceiptExchangeDetail>();


            #endregion

            #region Map Notice
            // Notice
            CreateMap<Notice, NoticeModel>();
            CreateMap<NoticeModel, Notice>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.NoticeDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.NoticeDate, "d/M/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.DistributionDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.DistributionDate, "d/M/yyyy", CultureInfo.InvariantCulture)));


            // Notice Details
            CreateMap<NoticeDetail, NoticeDetailModel>();
            CreateMap<NoticeDetailModel, NoticeDetail>();


            #endregion

            #region Map Entry
            // Entery
            CreateMap<Entry, EntryModel>();
            CreateMap<EntryModel, Entry>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.Date, "d/M/yyyy", CultureInfo.InvariantCulture)));


            // Entry Details
            CreateMap<EntryDetail, EntryDetailModel>();
            CreateMap<EntryDetailModel, EntryDetail>();


            #endregion

            #region Map Setting
            // Setting
            CreateMap<Setting, SettingModel>();
            CreateMap<SettingModel, Setting>();


            // Setting Accounts
            CreateMap<SettingAccount, SettingAccountModel>();
            CreateMap<SettingAccountModel, SettingAccount>();


            #endregion

            #region Map PortfolioTransaction
            CreateMap<PortfolioTransaction, PortfolioTransactionModel>();
            CreateMap<PortfolioTransactionModel, PortfolioTransaction>();
            #endregion


            #region ReportSetting
            CreateMap<ReportSetting, ReportSettingModel>();
            CreateMap<ReportSettingModel, ReportSetting>()
                 .ForMember(t => t.CurrentDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.CurrentDate, "d/M/yyyy", CultureInfo.InvariantCulture)));
            #endregion





            #region Map Setting
            CreateMap<Setting, SettingModel>();
            CreateMap<SettingModel, Setting>();

            #endregion



            #region Map Selling order

            CreateMap<SellingOrder, SellingOrderModel>();
            CreateMap<SellingOrderModel, SellingOrder>()
                .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.OrderDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.OrderDateGorg, "d/M/yyyy", CultureInfo.InvariantCulture)));


            CreateMap<SellingOrderDetail, SellingOrderDetailModel>();
            CreateMap<SellingOrderDetailModel, SellingOrderDetail>();
            #endregion

        }
    }
}
