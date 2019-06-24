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
                .ForMember(t => t.BirthDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))); 

            // employee card
            CreateMap<EmployeeCard, EmployeeCardModel>();
            CreateMap<EmployeeCardModel, EmployeeCard>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.IssueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.IssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.EndDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(t => t.RenewalDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.RenewalDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))); 

            ;

            #endregion

            #region Map Portfolio
            // portfolio
            CreateMap<Portfolio, PortfolioModel>();
            CreateMap<PortfolioModel, Portfolio>()
                 .ForSourceMember(t => t.Count, opt => opt.DoNotValidate())
                .ForMember(t => t.EstablishDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.EstablishDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

            // portfolio accounts
            CreateMap<PortfolioAccount, PortfolioAccountModel>();
            CreateMap<PortfolioAccountModel, PortfolioAccount>();

            // portfolio shareholders
            CreateMap<PortfolioShareHolder, PortfolioShareholderModel>();
            CreateMap<PortfolioShareholderModel, PortfolioShareHolder>() ;

            CreateMap<Setting, SettingModel>();
            CreateMap<SettingModel, Setting>();

            CreateMap<SellingOrder, SellingOrderModel>();
            CreateMap<SellingOrderModel, SellingOrder>().ForSourceMember(t => t.Count, opt => opt.DoNotValidate());
            CreateMap<Entry, EntryModel>();
            CreateMap<EntryModel, Entry>().ForSourceMember(t => t.Count, opt => opt.DoNotValidate());

            CreateMap<EntryDetail, EntryDetailsModel>();
            CreateMap<EntryDetailsModel, EntryDetail>();

            CreateMap<SellingOrderDetail, SelingOrderDetailsModel>();
            CreateMap<SelingOrderDetailsModel, SellingOrderDetail>();




            #endregion

        }
    }
}
