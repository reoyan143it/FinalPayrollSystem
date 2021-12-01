using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using FinalPayrollSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Serialization;
using System.Web;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace FinalPayrollSystem.Controllers
{
    public class RecordsController : Controller
    {
        IConfiguration _configuration; // GET THE CONNECTION STRING
        List<EmployeeModel> employeelist = new List<EmployeeModel>();
        List<RatesModel> rateslist = new List<RatesModel>();
        public RecordsController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        [HttpGet] // ADDING OF EMPLOYEES TO THE DATABASE
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost] // POST METHOD FOR ADDING EMPLOYEES
        public async Task<IActionResult> AddEmployee(EmployeeModel employee)
        {
            Dictionary<string, string> infoids = new Dictionary<string, string>();
            Type getPropertyName = employee.GetType();


            if (ModelState.IsValid)
            {
                try
                {
                    string[] empinfo = { "barangays", "towns", "cities", "provinces", "department", "positions" };
                    string[] empdata = { employee.barangays, employee.towns, employee.cities, employee.provinces, employee.department, employee.positions };

                    for (int countID = 0; countID <= (empinfo.Length-1); countID++)
                    {
                        PropertyInfo propertyInfo = getPropertyName.GetProperty($"{empinfo[countID]}");
                        Task<string> addresult = FindAddressInfos(empdata[countID], propertyInfo.Name);
                        string outPUT = addresult.Result.ToString();
                        if (outPUT.Contains("Error"))
                        {
                            ViewBag.Error += " " + outPUT;
                        }
                        else if (outPUT == "")
                        {
                            infoids.Add(empinfo[countID], null);
                        }
                        else
                        {
                            infoids.Add(empinfo[countID], outPUT);
                        }
                    }
                    
                    MySqlConnection sqlConnect3 = new MySqlConnection(_configuration.GetConnectionString("Default"));
                    await sqlConnect3.OpenAsync();
                    string commandText2 = @$"INSERT INTO employees(employeeid,firstname,middlename,lastname,gender,birthdate,datehired,employmentstatus,employeestatus,barangayid,townid,cityid,provinceid,departmentid,positionid,addedby,dateadded)
                                     VALUES('{employee.employeeid}','{employee.firstname}','{employee.middlename}','{employee.lastname}','{employee.gender}','{employee.birthdate.ToString("yyyy-MM-dd")}','{employee.datehired.ToString("yyyy-MM-dd")}','{employee.employmentstatus}','{employee.employeestatus}',{infoids["barangays"]},{infoids["towns"]},{infoids["cities"]},{infoids["provinces"]},{infoids["department"]},{infoids["positions"]},'{employee.addedby}','{employee.dateadded.ToString("yyyy-MM-dd HH:mm:ss")}')";
                    await using var command = new MySqlCommand(commandText2, sqlConnect3);

                    if (await command.ExecuteNonQueryAsync() > 0)
                    {
                        ViewBag.Message = "Added Successfully";
                    }
                    else
                    {
                        ViewBag.Result += "Data is not added!";
                    }
                    await sqlConnect3.CloseAsync();
                    await sqlConnect3.DisposeAsync();

                }
                catch (Exception e)
                {
                    ViewBag.Result += e.Message;
                }
            }

            return View();
        }

        // DISPLAY LIST OF EMPLOYEES
        [HttpGet]
        public async Task<IActionResult> ListOfEmployee()
        {
            if (!User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login/Index");
            }
            List<EmployeeModel> emplist = EmployeeListView();
            return View(emplist);
        }

        public async Task<IActionResult> EmployeeFullDetails(string employeeID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login/Index");
            }

            EmployeeModel empdata = new EmployeeModel();
            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
            await dbcon.OpenAsync();
            string commandText = $"SELECT * FROM employees WHERE employeeid='{employeeID}'";
            await using var cmd = new MySqlCommand(commandText, dbcon);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                empdata.employeeid = reader["employeeid"].ToString();
                empdata.firstname = reader["firstname"].ToString();
                empdata.middlename = reader["middlename"].ToString();
                empdata.lastname = reader["lastname"].ToString();
                empdata.gender = reader["gender"].ToString();
                empdata.birthdate = Convert.ToDateTime(reader["birthdate"].ToString());
                empdata.datehired = Convert.ToDateTime(reader["datehired"].ToString());
                empdata.employmentstatus = reader["employmentstatus"].ToString();
                empdata.employeestatus = reader["employeestatus"].ToString();
                empdata.barangays = GetAddressInfos(Convert.ToInt32(reader["barangayid"]), reader.GetName(9).ToString()).ToString();
                empdata.towns = GetAddressInfos(Convert.ToInt32(reader["townid"]), reader.GetName(10).ToString()).ToString();
                empdata.cities = GetAddressInfos(Convert.ToInt32(reader["cityid"]), reader.GetName(11).ToString()).ToString();
                empdata.provinces = GetAddressInfos(Convert.ToInt32(reader["provinceid"]), reader.GetName(12).ToString()).ToString();
                empdata.department = GetAddressInfos(Convert.ToInt32(reader["departmentid"]), reader.GetName(13).ToString()).ToString();
                empdata.positions = GetAddressInfos(Convert.ToInt32(reader["positionid"]), reader.GetName(14).ToString()).ToString();
            }
            await dbcon.CloseAsync();
            await dbcon.DisposeAsync();
            return View(empdata);
        }

        // UPDATING DATA FROM THE DATABASE
        [HttpGet]
        public async Task<IActionResult> EditEmployeeDetails(string employeeID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login/Index");
            }
            EmployeeModel dataemployee = new EmployeeModel();
            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
            await dbcon.OpenAsync();
            string commandText = $"SELECT * FROM employees WHERE employeeid='{employeeID}'";
            await using var cmd = new MySqlCommand(commandText, dbcon);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                dataemployee.employeeid = reader["employeeid"].ToString();
                dataemployee.firstname = reader["firstname"].ToString();
                dataemployee.middlename = reader["middlename"].ToString();
                dataemployee.lastname = reader["lastname"].ToString();
                dataemployee.gender = reader["gender"].ToString();
                dataemployee.birthdate = Convert.ToDateTime(reader["birthdate"]);
                dataemployee.datehired = Convert.ToDateTime(reader["datehired"]);
                dataemployee.employmentstatus = reader["employmentstatus"].ToString();
                dataemployee.employeestatus = reader["employeestatus"].ToString();
                dataemployee.barangays = GetAddressInfos(Convert.ToInt32(reader["barangayid"]), reader.GetName(9).ToString()).ToString();
                dataemployee.towns = GetAddressInfos(Convert.ToInt32(reader["townid"]), reader.GetName(10).ToString()).ToString();
                dataemployee.cities = GetAddressInfos(Convert.ToInt32(reader["cityid"]), reader.GetName(11).ToString()).ToString();
                dataemployee.provinces = GetAddressInfos(Convert.ToInt32(reader["provinceid"]), reader.GetName(12).ToString()).ToString();
                dataemployee.department = GetAddressInfos(Convert.ToInt32(reader["departmentid"]), reader.GetName(13).ToString()).ToString();
                dataemployee.positions = GetAddressInfos(Convert.ToInt32(reader["positionid"]), reader.GetName(14).ToString()).ToString();
            }
            await dbcon.CloseAsync();
            await dbcon.DisposeAsync();
            return View(dataemployee);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployeeDetails(EmployeeModel employee)
        {
            string[] empkeyval = {
                employee.firstname,
                employee.middlename,
                employee.lastname,
                employee.gender,
                employee.birthdate.ToString("yyyy-MM-dd"),
                employee.datehired.ToString("yyyy-MM-dd"),
                employee.employmentstatus,
                employee.employeestatus,
                await FindAddressInfos(employee.barangays, "barangays"),
                await FindAddressInfos(employee.towns, "towns"),
                await FindAddressInfos(employee.cities, "cities"),
                await FindAddressInfos(employee.provinces, "provinces"),
                await FindAddressInfos(employee.department, "department"),
                await FindAddressInfos(employee.positions, "positions")
            };

            try
            {
                MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                await dbcon.OpenAsync();
                string commandText = $"SELECT * FROM employees where employeeid='{employee.employeeid}'";
                await using var cmd = new MySqlCommand(commandText, dbcon);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync() == true)
                {
                    string[] d = new String[reader.FieldCount];
                    string[] e = new String[reader.FieldCount];

                    for (int c = 0; c < (reader.FieldCount-1); c++)
                    {
                        d[c] = reader.GetValue((c+1)).ToString();
                        e[c] = reader.GetName((c+1)).ToString();
                    }
                    await dbcon.CloseAsync();
                    await dbcon.DisposeAsync();

                    for (int count = 0; count < (empkeyval.Length-1); count++)
                    {
                        if(d[count] != empkeyval[count])
                        {
                            MySqlConnection dbcon1 = new MySqlConnection(_configuration.GetConnectionString("Default"));
                            await dbcon1.OpenAsync();
                            string commandText1 = $"UPDATE employees SET {e[count]}='{empkeyval[count]}' WHERE employeeid='{employee.employeeid}'";
                            await using var cmd1 = new MySqlCommand(commandText1, dbcon1);
                            await using var reader1 = await cmd1.ExecuteReaderAsync();
                            await reader1.ReadAsync();
                            if(reader1.RecordsAffected > 0)
                            {
                                ViewBag.Result = "Successfully Updated!";
                            }
                            await dbcon1.CloseAsync();
                            await dbcon1.DisposeAsync();
                        }
                    }
                }
                else
                {
                    await dbcon.CloseAsync();
                    await dbcon.DisposeAsync();
                }
                
                
            }
            catch(Exception e)
            {
                ViewBag.Result = e.Message;
            }

            return View();

        }

        // DELETION OF DATA FROM THE DATABASE
        public async Task<RedirectResult> DeleteEmployee(string employeeID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login/Index");
            }
            try
            {
                MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                await dbcon.OpenAsync();
                string commandtext = $"DELETE FROM employees WHERE employeeid='{employeeID}'";
                await using var cmd = new MySqlCommand(commandtext, dbcon);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync() == true)
                {
                    TempData["Result"] = "SUCCESSFULLY DELETED!";
                }
                else
                {
                    TempData["Result"] = "FAILED TO DELETE!";
                }

                await dbcon.CloseAsync();
                await dbcon.DisposeAsync();
            }
            catch(Exception e)
            {
                TempData["Result"] = e.Message;
            }
            return Redirect("/Records/ListOfEmployee");
        }

        // FIND THE ADDRESS ID BY USING ADDRESS NAME AND IF NOT FOUND ADD THE ADDRESS
        public async Task<string> FindAddressInfos(string addressinfo, string addressproperty)
        {
            string returnvalue = "";
            string address = "";
            int GetIndex = addressproperty.IndexOf("s");
            if (GetIndex < 0 || addressproperty.Contains("ies"))
            {
                address = addressproperty;
            }
            else
            {
                if(addressproperty.LastIndexOf("s") == (addressproperty.Length-1))
                {
                    address = addressproperty.Replace(addressproperty, addressproperty.Remove(addressproperty.LastIndexOf("s")));
                }
                else
                {
                    address = addressproperty;
                }
                
            }
            string addressid;
            string addressname;
            if (addressproperty.Contains("ies"))
            {
                string remadd = addressproperty.Remove((addressproperty.Length - 1) - 2);
                address = remadd.Insert(remadd.Length, "y");
                addressid = address.Insert(address.Length, "id");
                addressname = address.Insert(address.Length, "name");
            }
            else
            {
                addressid = address.Insert(address.Length, "id");
                addressname = address.Insert(address.Length, "name");
            }

            if (addressinfo == "")
            {
                return returnvalue;
            }
            else
            {
                try
                {
                    MySqlConnection sqlConnect = new MySqlConnection(_configuration.GetConnectionString("Default"));
                    await sqlConnect.OpenAsync();
                    string commandText = $"SELECT * FROM {addressproperty} WHERE {addressname}='{addressinfo}'";
                    await using var cmd = new MySqlCommand(commandText, sqlConnect);
                    await using var reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync() == true)
                    {
                        returnvalue = reader.GetValue(0).ToString();
                        await sqlConnect.CloseAsync();
                        await sqlConnect.DisposeAsync();
                    }
                    else
                    {
                        await sqlConnect.CloseAsync();
                        await sqlConnect.DisposeAsync();
                        try
                        {
                            MySqlConnection sqlConnect1 = new MySqlConnection(_configuration.GetConnectionString("Default"));
                            await sqlConnect1.OpenAsync();
                            await using var cmd1 = new MySqlCommand($"INSERT INTO {addressproperty} ({addressid},{addressname}) VALUES(null,'{addressinfo}')", sqlConnect1);
                            if (await cmd1.ExecuteNonQueryAsync() > 0)
                            {
                                await sqlConnect1.CloseAsync();
                                await sqlConnect1.DisposeAsync();

                                try
                                {
                                    MySqlConnection sqlConnect2 = new MySqlConnection(_configuration.GetConnectionString("Default"));
                                    await sqlConnect2.OpenAsync();
                                    await using var cmd2 = new MySqlCommand($"SELECT * FROM {addressproperty} WHERE {addressname}='{addressinfo}'", sqlConnect2);
                                    await using var reader2 = await cmd2.ExecuteReaderAsync();

                                    while (await reader2.ReadAsync())
                                    {
                                        returnvalue = reader2.GetValue(0).ToString();
                                    }

                                    await sqlConnect2.CloseAsync();
                                    await sqlConnect2.DisposeAsync();
                                }catch(Exception e)
                                {
                                    returnvalue = "Error: " + e.Message.ToString();
                                }
                                

                            }
                        }
                        catch (Exception e)
                        {
                            returnvalue = "Error: " + e.Message.ToString();
                        }

                    }

                }
                catch (Exception e)
                {
                    returnvalue = "Error: " + e.Message.ToString(); ;
                }

            }
            return returnvalue;
        }

        // GET THE ADDRESS NAME BY USING THE ADDRESS ID
        public string GetAddressInfos(int addressID, string colname)
        {
            string returnvalue = "";
            string tblname;
            if (colname.Contains("ty"))
            {
                string remchar = colname.Remove((colname.Length - 2));
                tblname = remchar.Replace("ty", "ties");
            }else if (colname == "departmentid")
            {
                tblname = colname.Remove((colname.Length - 2));
            } 
            else 
            {
                string remchar = colname.Remove((colname.Length - 2));
                tblname = remchar.Insert(remchar.Length, "s");
            }
            try
            {
                MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                dbcon.Open();
                string commandtext = $"SELECT * FROM {tblname} WHERE {colname}={addressID}";
                using var cmd = new MySqlCommand(commandtext, dbcon);
                using var reader = cmd.ExecuteReader();

                if (reader.Read() == true)
                {
                    returnvalue = reader[1].ToString();
                }
            }catch(Exception e)
            {
                return e.Message;
            }
            return returnvalue;
        }

        // SHOW ALL EMPLOYEES
        public List<EmployeeModel> EmployeeListView()
        {
            if(employeelist.Count > 0)
            {
                employeelist.Clear();
            }
            try
            {
                MySqlConnection sqlConnect = new MySqlConnection(_configuration.GetConnectionString("Default"));
                sqlConnect.Open();
                string commandtext = "SELECT * FROM employees";
                using var cmd = new MySqlCommand(commandtext, sqlConnect);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employeelist.Add(new EmployeeModel()
                    {
                        employeeid = reader["employeeid"].ToString(),
                        firstname = reader["firstname"].ToString(),
                        middlename = reader["middlename"].ToString(),
                        lastname = reader["lastname"].ToString(),
                        gender = reader["gender"].ToString(),
                        birthdate = Convert.ToDateTime(reader["birthdate"].ToString()),
                        datehired = Convert.ToDateTime(reader["datehired"].ToString()),
                        //classofemployment = reader["classofemployment"].ToString(),
                        //employeestatus = reader["employeestatus"].ToString(),
                        //barangays = GetAddressInfos(Convert.ToInt32(reader["barangayid"]), reader.GetName(9).ToString()).ToString(),
                        //towns = GetAddressInfos(Convert.ToInt32(reader["townid"]), reader.GetName(10).ToString()).ToString(),
                        //cities = GetAddressInfos(Convert.ToInt32(reader["cityid"]), reader.GetName(11).ToString()).ToString(),
                        //provinces = GetAddressInfos(Convert.ToInt32(reader["provinceid"]), reader.GetName(12).ToString()).ToString(),
                        //department = GetAddressInfos(Convert.ToInt32(reader["departmentid"]), reader.GetName(13).ToString()).ToString(),
                        //positions = GetAddressInfos(Convert.ToInt32(reader["positionid"]), reader.GetName(14).ToString()).ToString()
                    });
                }
                sqlConnect.CloseAsync();
                sqlConnect.DisposeAsync();
            }
            catch(Exception e)
            {
                Response.WriteAsync(e.Message);
            }
            return employeelist;
        }

        [HttpGet]
        public async Task<IActionResult> AddRates()
        {
            if (!User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login/Index");
            }

            RatesModel rlist = new RatesModel();
            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
            await dbcon.OpenAsync();
            string commandtext = "SELECT * FROM rates";
            await using var cmd = new MySqlCommand(commandtext, dbcon);
            await using var reader = cmd.ExecuteReader();

            while (await reader.ReadAsync())
            {
                rateslist.Add(new RatesModel()
                {
                    rateid = Convert.ToInt32(reader["rateid"]),
                    employeeid = reader["employeeid"].ToString(),
                    salary = Convert.ToDecimal(reader["salary"]),
                    paytype = reader["paytype"].ToString(),
                    employeename = EmployeeName(reader["employeeid"].ToString())
                });
            }
            await reader.CloseAsync();
            await reader.DisposeAsync();
            await dbcon.CloseAsync();
            await dbcon.DisposeAsync();

            rlist.rateslist = rateslist.ToList();
            return View(rlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddRates(RatesModel rates)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    MySqlConnection dDBcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                    await dDBcon.OpenAsync();
                    string dcommandtext = $"SELECT * FROM rates WHERE employeeid='{rates.employeeid}'";
                    await using var dcmd = new MySqlCommand(dcommandtext, dDBcon);
                    await using var reader = await dcmd.ExecuteReaderAsync();

                    if(await reader.ReadAsync() == true)
                    {
                        TempData["Result"] = "Already added to the system!";
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                        await dDBcon.CloseAsync();
                        await dDBcon.DisposeAsync();
                    }
                    else
                    {
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                        await dDBcon.CloseAsync();
                        await dDBcon.DisposeAsync();
                        try
                        {
                            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                            await dbcon.OpenAsync();
                            string commandtext = $@"INSERT INTO rates(rateid,paytype,salary,employeeid,addedby,dateadded)
                                          VALUES (null,'{rates.paytype}',{rates.salary},'{rates.employeeid}','{User.Identity.Name}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";

                            await using var cmd = new MySqlCommand(commandtext, dbcon);

                            if (await cmd.ExecuteNonQueryAsync() > 0)
                            {
                                TempData["Result"] = "Successfully Added";
                            }
                            else
                            {
                                TempData["Result"] = "Data not saved to database!";
                            }

                            await dbcon.CloseAsync();
                            await dbcon.DisposeAsync();
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = e.Message;
                        }
                    }
                    
                        
                }catch(Exception e)
                {
                    ViewBag.Error = "Error: " + e.Message;
                }
            }
            return Redirect("/Records/AddRates");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRate(int rateID)
        {
            try
            {
                MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                await dbcon.OpenAsync();
                string commandtext = $"SELECT * FROM rates WHERE rateid={rateID}";
                await using var cmd = new MySqlCommand(commandtext, dbcon);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync() == true)
                {
                    await dbcon.CloseAsync();
                    await dbcon.DisposeAsync();
                    try
                    {
                        MySqlConnection dbcon1 = new MySqlConnection(_configuration.GetConnectionString("Default"));
                        await dbcon1.OpenAsync();
                        string commandtext1 = $"DELETE FROM rates WHERE rateid='{rateID}'";
                        await using var cmd1 = new MySqlCommand(commandtext1, dbcon1);
                        await using var reader1 = await cmd1.ExecuteReaderAsync();
                        await reader1.ReadAsync();

                        if (reader1.RecordsAffected > 0)
                        {
                            TempData["Result"] = "Successfully Deleted!";
                        }
                        else
                        {
                            TempData["Result"] = "Failed to delete the data!";
                        }
                        await reader1.CloseAsync();
                        await reader1.DisposeAsync();
                        await dbcon1.CloseAsync();
                        await dbcon1.DisposeAsync();
                    }catch(Exception e)
                    {
                        TempData["Error"] = "Error: " + e.Message;
                    }
                    
                }
                else
                {
                    TempData["Result"] = "Data not Exist!";
                }
            }catch(Exception e)
            {
                TempData["Error"] = "Error: " + e.Message;
            }
            


            return Redirect("/Records/AddRates");
        }

        public async Task<string> EditRate(int rateID, string colnm, string inpval)
        {
            string response = "";
            try
            {
                MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
                await dbcon.OpenAsync();
                string commandtext = $"SELECT * FROM rates WHERE rateid={rateID}";
                await using var cmd = new MySqlCommand(commandtext, dbcon);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync() == true)
                {
                    await dbcon.CloseAsync();
                    await dbcon.DisposeAsync();
                    try
                    {
                        MySqlConnection dbcon1 = new MySqlConnection(_configuration.GetConnectionString("Default"));
                        await dbcon1.OpenAsync();
                        string commandtext1 = $"UPDATE rates SET {colnm}='{inpval}' WHERE rateid='{rateID}'";
                        await using var cmd1 = new MySqlCommand(commandtext1, dbcon1);
                        await using var reader1 = await cmd1.ExecuteReaderAsync();
                        await reader1.ReadAsync();

                        if (reader1.RecordsAffected > 0)
                        {
                            response = "Successfully Updated!";
                        }
                        else
                        {
                            response = "Failed to delete the data!";
                        }
                        await reader1.CloseAsync();
                        await reader1.DisposeAsync();
                        await dbcon1.CloseAsync();
                        await dbcon1.DisposeAsync();
                    }
                    catch (Exception e)
                    {
                        response = "Error: " + e.Message;
                    }

                }
                else
                {
                    response = "Data not Exist!";
                }
            }
            catch (Exception e)
            {
                response = "Error: " + e.Message;
            }

            return response;
        }


        // ADD THE EMPLOYEE ID TO THE DATALIST OF EMPLOYEE ID IN RATES
        public async Task<JsonResult> dtEmployeeID()
        {
            List<EmployeeModel> emplist = new List<EmployeeModel>();
            EmployeeModel empl = new EmployeeModel();

            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
            await dbcon.OpenAsync();
            string commandtext = "SELECT employeeid,firstname,lastname FROM employees";
            await using var cmd = new MySqlCommand(commandtext, dbcon);
            await using var reader = await cmd.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                emplist.Add(new EmployeeModel()
                {
                    employeeid = reader["employeeid"].ToString(),
                    firstname = reader["firstname"].ToString(),
                    lastname = reader["lastname"].ToString()
                });
            }
            await dbcon.CloseAsync();
            await dbcon.DisposeAsync();

            return Json(emplist);
        }

        // SHOW THE VALUE IN THE INPUT BOX EMPLOYEE NAME BASED ON WHAT EMPLOYEE ID SELECTED IN INPUT BOX
        // OF EMPLOYEE ID
        public JsonResult valEmpName(string employeeID)
        {
            List<EmployeeModel> emplist = new List<EmployeeModel>();
            EmployeeModel empl = new EmployeeModel();

            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
            dbcon.Open();
            string commandtext = $"SELECT firstname,lastname FROM employees WHERE employeeid='{employeeID}'";
            var cmd = new MySqlCommand(commandtext, dbcon);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                emplist.Add(new EmployeeModel()
                {
                    firstname = reader["firstname"].ToString(),
                    lastname = reader["lastname"].ToString()
                });
            }
            dbcon.Close();
            dbcon.Dispose();

            return Json(emplist);
        }

        // RETURN THE STRING EMPLOYEE NAME OF THE SPECIFIED EMPLOYEE ID PARAMETER
        public string EmployeeName(string employeeID)
        {
            string employeename = "";

            MySqlConnection dbcon = new MySqlConnection(_configuration.GetConnectionString("Default"));
            dbcon.Open();
            string commandtext = $"SELECT firstname,lastname FROM employees WHERE employeeid='{employeeID}'";
            var cmd = new MySqlCommand(commandtext, dbcon);
            var reader = cmd.ExecuteReader();
            if(reader.Read() == true)
            {
               employeename = reader["firstname"].ToString() + " " + reader["lastname"].ToString();
            }
            else
            {

            }
            dbcon.Close();
            dbcon.Dispose();

            return employeename;
        }




    }
}
