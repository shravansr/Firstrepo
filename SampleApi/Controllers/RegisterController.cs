using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static SampleApi.Models.Class1;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using System.Dynamic;

namespace SampleApi.Controllers
{
    public class RegisterController : ApiController
    {
        //[HttpGet]
        //[ActionName("validateLogin")]
        //public HttpResponseMessage validate()
        //{
        //    string result = string.Empty;
        //    result = "working";
        //    return Request.CreateResponse(HttpStatusCode.OK, result,"application/json");
        //}

        [HttpPost]
        [ActionName("registerUser")]
        public HttpResponseMessage Create([FromBody]UserDetails Details)
        {            
            string Destination = JsonConvert.SerializeObject(Details);
            //using (StreamWriter writer = new StreamWriter("D:\\log.txt", true))
            //{
            //    writer.WriteLine(Destination + " " + DateTime.Now.ToString());
            //}
            UserResponse resp = new UserResponse();
            string result = valiidate(Details, 0);
            if(!string.IsNullOrEmpty(result))
            {
                resp.Status = "0";
                resp.StatusMessage = result + (result.Contains(',') ? " are required" : " is required");
                return Request.CreateResponse(HttpStatusCode.OK, resp, "application/json");
            }
            //MySqlConnection conn = new MySqlConnection("server=omniazuredb.mysql.database.azure.com;user id=azureuser@omniazuredb;database=test;password=Omni1234!;Port=3306");
            MySqlConnection conn = new MySqlConnection("server=localhost;user id=shravan;database=test;password=Omni1234!;Port=3306");
            MySqlCommand cmd = new MySqlCommand();
            DataSet ds = new DataSet();
            try
            {                
                try
                {
                    conn.Open();
                    cmd.CommandText = "createUser";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userName", Details.UserName);
                    cmd.Parameters.AddWithValue("@pwd", Details.Password);
                    cmd.Parameters.AddWithValue("@country", Details.Country);
                    cmd.Parameters.AddWithValue("@mobile_number", Details.MobileNumber);
                    cmd.Connection = conn;
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                    adp.Fill(ds);                    
                }
                catch(MySqlException ex)
                {
                    resp.Status = "0";
                    resp.StatusMessage =  "MySQL Exception" + ex.Message.ToString();
                    //conn.Clone();
                    cmd.Dispose();
                    conn.Close();
                    return Request.CreateResponse(HttpStatusCode.OK, resp, "application/json");
                }
                if (ds != null)
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() == "1")
                    {
                        resp.Status = "1";
                        resp.StatusMessage = "Registration successful";
                    }
                    else
                    {
                        resp.Status = "0";
                        resp.StatusMessage = "User already exists";
                    }
                }
            }   
            catch(Exception ex)
            {
                resp.Status = "0";
                resp.StatusMessage = ex.Message.ToString();
            }
            finally
            {
                //conn.Clone();
                cmd.Dispose();
                conn.Close();
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp, "application/json");
        }

        public string valiidate(UserDetails user,int mode)
        {
            string res = string.Empty;
            if(mode == 0)
            {
                if (string.IsNullOrEmpty(user.UserName))
                    res = "UserName";
                if (string.IsNullOrEmpty(user.Password))
                    if (string.IsNullOrEmpty(res))
                        res = "Password";
                    else
                        res = res + "," + " Password";
                if (string.IsNullOrEmpty(user.Country))
                    if (string.IsNullOrEmpty(res))
                        res = "Country";
                    else
                        res = res + "," +  "Country";
                if (string.IsNullOrEmpty(user.MobileNumber))
                    if (string.IsNullOrEmpty(res))
                        res = "MobileNumber";
                    else
                        res = res + "," + " MobileNumber";
            }
            else
            {
                if (string.IsNullOrEmpty(user.UserName))
                    res = "UserName";
                if (string.IsNullOrEmpty(user.Password))
                    if (string.IsNullOrEmpty(res))
                        res = "Password";
                    else
                        res = res + "," + " Password";
            }
            return res;
        }

        [HttpPost]
        [ActionName("validateLogin")]
        public HttpResponseMessage Verify([FromBody]UserDetails Details)
        {
            UserResponse resp = new UserResponse();
            string result = valiidate(Details, 1);
            if (!string.IsNullOrEmpty(result))
            {
                resp.Status = "0";
                resp.StatusMessage = result + (result.Contains(',') ? " are required" : " is required");
                return Request.CreateResponse(HttpStatusCode.OK, resp, "application/json");
            }
            //resp.input = JsonConvert.SerializeObject(Details);
            string uName = System.Configuration.ConfigurationManager.AppSettings["UserName"].ToString();
            string pwd = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
            DataSet ds = new DataSet();
            //MySqlConnection conn = new MySqlConnection("server=omniazuredb.mysql.database.azure.com;user id=azureuser@omniazuredb;database=test;password=Omni1234!;Port=3306");
            MySqlConnection conn = new MySqlConnection("server=localhost;user id=root;database=test;password=Omni1234!;Port=3306");
            MySqlCommand cmd = new MySqlCommand();
            try
            {                
                try
                {                    
                    conn.Open();                    
                    cmd.CommandText = "validateUser";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userName", Details.UserName);
                    //cmd.Parameters["@name"].Direction = ParameterDirection.Input;
                    cmd.Parameters.AddWithValue("@pwd", Details.Password);
                    //cmd.Parameters["@pwd"].Direction = ParameterDirection.Input;
                    cmd.Connection = conn;
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                    adp.Fill(ds);
                }                
                catch(MySqlException ex)
                {
                    resp.Status = "0";
                    resp.StatusMessage = ex.ToString();
                    //conn.Clone();
                    cmd.Dispose();
                    conn.Close();
                    return Request.CreateResponse(HttpStatusCode.OK, resp, "application/json");
                }                
                if(ds!=null)
                {
                    if(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        resp.Status = "1";
                        resp.StatusMessage = "Login Successful";
                    }
                    else
                    {
                        resp.Status = "0";
                        resp.StatusMessage = "invalid credentials";
                    }
                }
                else
                {
                    resp.Status = "0";
                    resp.StatusMessage = "invalid credentials";
                }

                //if (uName == Details.UserName && pwd == Details.Password)
                //{
                //    resp.Status = "1";
                //    resp.StatusMessage = "Login Successful";
                //}
                //else
                //{
                //    resp.Status = "0";
                //    resp.StatusMessage = "Invalid Credentials";
                //}
            }           
            catch (Exception ex)
            {
                resp.Status = "0";
                resp.StatusMessage = ex.ToString();                
            }
            finally
            {
                //conn.Clone();
                cmd.Dispose();
                conn.Close();
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp, "application/json");
        }


        
        [HttpGet]
        [ActionName("getAllProducts")]
        public HttpResponseMessage getAll()
        {
            AllProducts product = new AllProducts();
            DataSet ds = new DataSet();
            AllProductsDynamic dynProd = new AllProductsDynamic();
            var dynamicDt = new List<dynamic>();
            MySqlConnection conn = new MySqlConnection("server=localhost;user id=shravan;database=test;password=Omni1234!;Port=3306");
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = "getAllProducts";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                    adp.Fill(ds);
                }
                catch (MySqlException ex)
                {
                    conn.Clone();
                    cmd.Dispose();
                    return Request.CreateResponse(HttpStatusCode.OK, "Error", "application/json");
                }
                if (ds != null)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        dynamic dyn = new ExpandoObject();
                        dynamicDt.Add(dyn);
                        foreach (DataColumn column in ds.Tables[0].Columns)
                        {
                            var dic = (IDictionary<string, object>)dyn;
                            dic[column.ColumnName] = row[column];
                        }
                    }

                    //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    //{
                    //    List<Products> userList = new List<Products>();                        
                    //    userList = (from row in ds.Tables[0].AsEnumerable()
                    //                select new Products
                    //                {
                    //                    id = row.Field<int>("id"),
                    //                    name = row.Field<string>("name"),
                    //                    Price = row.Field<decimal>("Price"),
                    //                    type = row.Field<string>("type"),
                    //                    created_date = row.Field<DateTime>("created_date"),
                    //                }).ToList();
                    //    product.Products = userList;

                    //}
                }

                dynProd.Products = dynamicDt;
            }
            catch (Exception ex)
            {
                //resp.Status = "0";
                //resp.StatusMessage = ex.ToString();
            }
            finally
            {
                conn.Clone();
                cmd.Dispose();
            }
            return Request.CreateResponse(HttpStatusCode.OK, dynProd, "application/json");
        }        
    }
}
