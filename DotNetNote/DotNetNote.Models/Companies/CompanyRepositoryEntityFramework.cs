﻿using Dul.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNote.Models.Companies;

public class CompanyRepositoryEntityFramework(CompanyContext context) : ICompanyRepository
{
    public CompanyModel Add(CompanyModel model)
    {
        //using (var db = new CompanyContext())
        {
            context.Companies.Add(model);
            context.SaveChanges();
        }

        return model;
    }

    public CompanyModel Browse(int id) => throw new NotImplementedException();

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }

    public bool Edit(CompanyModel model)
    {
        throw new NotImplementedException();
    }

    public int Has()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CompanyModel> Ordering(OrderOption orderOption)
    {
        throw new NotImplementedException();
    }

    public List<CompanyModel> Paging(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public List<CompanyModel> Read()
    {
        var companies = new List<CompanyModel>();
        //using (var db = new CompanyContext())
        {
            companies = context.Companies.ToList();
        }
        return companies;
    }

    public List<CompanyModel> Search(string query)
    {
        throw new NotImplementedException();
    }
}
