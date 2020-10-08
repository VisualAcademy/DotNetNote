using Dul.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DotNetNote.Models
{
    public class CompanyRepositoryAdo : ICompanyRepository
    {
        private string _connectionString;

        public CompanyRepositoryAdo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CompanyModel Add(CompanyModel model)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = _connectionString;
            con.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "Insert Into Companies (Name) Values(@Name);";
            cmd.CommandType = CommandType.Text;

            //[1] SqlParameter 클래스의 인스턴스 생성
            SqlParameter name =
                new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            //[2] Value 속성으로 값 지정
            name.Value = model.Name;
            //[3] 커멘트 개체에 매개 변수 추가
            cmd.Parameters.Add(name);

            cmd.ExecuteNonQuery();

            con.Close();

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
            SqlConnection con = new SqlConnection();
            con.ConnectionString = _connectionString;
            con.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "Select Top 1000 Id, Name From Companies";
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds, "Companies");

            List<CompanyModel> companies = new List<CompanyModel>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var id = Convert.ToInt32(ds.Tables[0].Rows[i]["Id"].ToString());
                var name = ds.Tables[0].Rows[i]["Name"].ToString();

                companies.Add(new CompanyModel
                {
                    Id = id,
                    Name = name
                });
            }
            con.Close();

            return companies;
        }

        public List<CompanyModel> Search(string query)
        {
            throw new NotImplementedException();
        }
    }
}
