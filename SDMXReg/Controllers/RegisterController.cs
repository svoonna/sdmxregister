using Microsoft.AspNetCore.Mvc;
using SDMXReg.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SDMXReg.Controllers
{
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly SDMXContext _context;

        private readonly string azureSqlConnString = "Server=tcp:ics20sdmx.database.windows.net,1433;Initial Catalog=SDMX;Persist Security Info=False;User ID=icsuser;Password=ics204Imf;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly string azureSqlTableName = "dbo.tblRegister";

        public RegisterController(SDMXContext context)
        {
            _context = context;

            if (_context.SDMXItems.Count() == 0)
            {
                _context.SDMXItems.Add(new Register { Query = "Item1" });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<Register> GetAll()
        {
            return _context.SDMXItems.ToList();
        }

        [HttpGet("{id}", Name = "GetRegister")]
        public IActionResult GetById(long id)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "ics20sdmx.database.windows.net,1433";
            builder.UserID = "icsuser";
            builder.Password = "ics204Imf";
            builder.InitialCatalog = "SDMX";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                //String sql = "Select * from tblRegister where Id=" + id + "";
                String sql = "uspGetSDMXRegister " + id + "";
                Register reg = null;
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            reg = new Register();
                            reg.Id = Convert.ToInt32(reader.GetValue(0));
                            reg.Query = reader.GetValue(1).ToString();
                            reg.UpdatedDate = (DateTime)reader.GetValue(2);
                        }
                    }
                }
                if (reg == null)
                {
                    return NotFound();
                }
                return new ObjectResult(reg);
            }
            //    SqlDataReader reader = null;
            //SqlConnection myConnection = new SqlConnection();
            //// Specify the sink Azure SQL Database information

            //myConnection.ConnectionString = azureSqlConnString;

            //SqlCommand sqlCmd = new SqlCommand();
            //sqlCmd.CommandType = CommandType.Text;
            //sqlCmd.CommandText = "Select * from tblRegister where Id=" + id + "";
            //sqlCmd.Connection = myConnection;
            //myConnection.Open();
            //reader = sqlCmd.ExecuteReader();

            //Register reg = null;
            //while (reader.Read())
            //{
            //    reg = new Register();
            //    reg.Id = Convert.ToInt32(reader.GetValue(0));
            //    reg.Query = reader.GetValue(1).ToString();
            //    reg.UpdatedDate = (DateTime)reader.GetValue(2);
            //}

           
            //myConnection.Close();
            

            //var item = _context.SDMXItems.FirstOrDefault(t => t.Id == id);
            //if (item == null)
            //{
            //    return NotFound();
            //}
            //return new ObjectResult(item);
        }

        [HttpPost]
        public void Create([FromBody] Register item)
        //public IActionResult Create([FromBody] Register item)
        {
            if (item == null)
            {
                //return BadRequest();
            }

            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = azureSqlConnString;
            SqlCommand sqlCmd = new SqlCommand("uspUpdateRegister",myConnection);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            //sqlCmd.CommandText = "INSERT INTO tblRegister (SDMXQuery,UpdatedDate) Values (@Query,@UpdatedDate)";
            //sqlCmd.Connection = myConnection;

            //sqlCmd.Parameters.AddWithValue("@EmployeeId", item.Id);
            sqlCmd.Parameters.AddWithValue("@sdmxQuery", item.Query);
            sqlCmd.Parameters.AddWithValue("@updatedDate", item.UpdatedDate);
            myConnection.Open();
            int rowInserted = sqlCmd.ExecuteNonQuery();
            myConnection.Close();

            //_context.SDMXItems.Add(item);
            //_context.SaveChanges();

            //return CreatedAtRoute("GetRegister", new { id = item.Id }, item);
        }
    }
}
