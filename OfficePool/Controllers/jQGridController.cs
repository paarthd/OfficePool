using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficePool.Models;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Data;

namespace OfficePool.Controllers
{
    public class jQGridController : Controller
    {
        [HttpPost]
        public ActionResult LoadjqData(string sidx, string sord, int page, int rows,
                bool _search, string searchField, string searchOper, string searchString)
        {
            // Get the list of students
            SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=OfficePool;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            conn.Open();
            SqlCommand cmd = new SqlCommand("fc_GetOfficePool", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            var students = dt.AsEnumerable();

            // If search, filter the list against the search condition.
            // Only "contains" search is implemented here.
            var filteredStudents = students;
          
            // Calculate the total number of pages
            var totalRecords = filteredStudents.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / (double)rows);

            // Prepare the data to fit the requirement of jQGrid
            var data = (from s in 
                            filteredStudents
                       select new
                        {
                            id = s.ItemArray[0],
                            cell = new object[] { s.ItemArray[0], s.ItemArray[1],
                            s.ItemArray[2], s.ItemArray[3].ToString()}
                        }).ToArray();

            // Send the data to the jQGrid
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = data.Skip((page - 1) * rows).Take(rows)
            };

            return Json(jsonData);
        }

        // Utility method to sort IQueryable given a field name as "string"
        // May consider to put in a central place to be shared
        private IQueryable<T> SortIQueryable<T>(IQueryable<T> data,
            string fieldName, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(fieldName)) return data;
            if (string.IsNullOrWhiteSpace(sortOrder)) return data;

            var param = Expression.Parameter(typeof(T), "i");
            Expression conversion = Expression.Convert
        (Expression.Property(param, fieldName), typeof(object));
            var mySortExpression = Expression.Lambda<Func<T, object>>(conversion, param);

            return (sortOrder == "desc") ? data.OrderByDescending(mySortExpression)
                : data.OrderBy(mySortExpression);
        }
        
    }
}