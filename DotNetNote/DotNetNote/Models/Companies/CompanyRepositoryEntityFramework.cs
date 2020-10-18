using Dul.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNote.Models.Companies
{
    public class CompanyRepositoryEntityFramework : ICompanyRepository
    {
        public CompanyModel Add(CompanyModel model)
        {
            using (var db = new CompanyContext())
            {
                db.Companies.Add(model);
                db.SaveChanges(); 
            }

            return model; 
        }

        public CompanyModel Browse(int id)
        {
            throw new NotImplementedException();
        }

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
            using (var db = new CompanyContext())
            {
                companies = db.Companies.ToList();
            }
            return companies; 
        }

        public List<CompanyModel> Search(string query)
        {
            throw new NotImplementedException();
        }
    }
}
