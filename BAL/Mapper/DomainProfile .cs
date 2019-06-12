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

        }
    }
}
