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
               .ForMember(t => t.IssueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.IssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
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
                .ForMember(t => t.BirthDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)));

            // employee card
            CreateMap<EmployeeCard, EmployeeCardModel>();
            CreateMap<EmployeeCardModel, EmployeeCard>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.IssueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.IssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.EndDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.RenewalDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.RenewalDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

            ;

            #endregion


            #region Map Setting
            CreateMap<Setting, SettingModel>();
            CreateMap<SettingModel, Setting>();

            #region Map Selling order

            CreateMap<SellingOrder, SellingOrderModel>();
            CreateMap<SellingOrderModel, SellingOrder>()
                .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.SellDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));


            CreateMap<SellingOrderDetail, SelingOrderDetailsModel>();
            CreateMap<SelingOrderDetailsModel, SellingOrderDetail>();
            #endregion

            #region Map Purchase order

            CreateMap<PurchaseOrder, PurchaseOrderModel>();
            CreateMap<PurchaseOrderModel, PurchaseOrder>().ForSourceMember(t => t.Count, opt => opt.DoNotValidate());

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
                .ForMember(t => t.EstablishDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.EstablishDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)))
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
                .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.ChiqueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.ChiqueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));


            // Reciept & Exchange Details
            CreateMap<ReceiptExchangeDetail, ReceiptExchangeDetailModel>();
            CreateMap<ReceiptExchangeDetailModel, ReceiptExchangeDetail>();


            #endregion

            #region Map Notice
            // Notice
            CreateMap<Notice, NoticeModel>();
            CreateMap<NoticeModel, Notice>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.NoticeDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.NoticeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));


            // Notice Details
            CreateMap<NoticeDetail, NoticeDetailModel>();
            CreateMap<NoticeDetailModel, NoticeDetail>();


            #endregion

            #region Map Entry
            // Entery
            CreateMap<Entry, EntryModel>();
            CreateMap<EntryModel, Entry>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                 .ForMember(t => t.Date, opt => opt.MapFrom(s => DateTime.ParseExact(s.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)));


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

        }
    }
}
