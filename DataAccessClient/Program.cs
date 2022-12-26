// See https://aka./new-console-template for more information
using DataAccessClient.Entities;
using DataAccessClient.Services;
using DataAccessClient.TestData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessClient
{
    class Program
    {
        
        
        static StringBuilder sbTime = new StringBuilder();
        static StringBuilder sbProc = new StringBuilder();
        static StringBuilder sbMem = new StringBuilder();

        static int queryCycles = 2;
        static int numTests = 2;
        static int numUsers = 2;
        static bool useGet = true;
        static bool isParallel = false;
        async static Task<bool> GetAPIConnect(int factor)
        {
            List<User> users;
            for (int i = 0; i < queryCycles*factor; i++)
            {
                users = await ApiService.GetAllUsersAsync();
            }
            
            return true;
        }
        async static Task<bool> GetAPIConnectParallel(int factor)
        {
            Task[] tasks = new Task[queryCycles*factor];


            for (int i = 0; i < queryCycles*factor; i++)
            {
                tasks[i] = ApiService.GetAllUsersAsync();
            }
            await Task.WhenAll(tasks);

            return true;
        }

        static void GetDirectConnect(int factor)
        {

            SqlConnection cnn = DataService.DirectConnect();

            SqlCommand command;
            SqlDataReader dataReader;
            string sql1;

            sql1 = "SELECT * FROM [User]";

            command = new SqlCommand(sql1, cnn);

            DateTime start = DateTime.Now;
            for (int i = 0; i < queryCycles*factor; i++)
            {

                    cnn.Open();
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    cnn.Close();


            }
            DateTime end = DateTime.Now;
            sbTime.Append((end - start).TotalMilliseconds + "\n");
            cnn.Dispose();
        }
        static void InsertGetDeleteDirectConnect(int factor)
        {
            SqlConnection cnn = DataService.DirectConnect();

            SqlCommand command;
            SqlDataReader dataReader;
            String sql1, sql2, sql3, Output = "";
            List<User> users = new List<User>();
            List<User> resultsend = new List<User>();
            List<User> resultsDelete = new List<User>();
            users = TestUser.GenerateUserList(numUsers);

            sql1 = "SELECT * FROM [User]";
            sql2 = "INSERT INTO [User] (Email, Phone, Name, Address, City) VALUES ";
            sql3 = "DELETE FROM [User] WHERE Email = 'TestEmail'";
            string newVals;
            for(int i = 0; i < users.Count(); i++)
            {
                User user = users[i];
                if(i < users.Count()-1)
                {
                    newVals = "('" + user.Email + "','" + user.Phone + "','" + user.Name + "','" + user.Address + "','" + user.City + "'),";

                } else
                {
                    newVals = "('" + user.Email + "','" + user.Phone + "','" + user.Name + "','" + user.Address + "','" + user.City + "')";
                }
                sql2 += newVals;

            }
            SqlCommand selectCommand = new SqlCommand(sql1, cnn);
            SqlCommand insertCommand = new SqlCommand(sql2, cnn);
            SqlCommand deleteCommand = new SqlCommand(sql3, cnn);
            //sbTime.Append($"Executing Insert, Select, Delete {queryCycles} times...\n");


            for (int i = 0; i < queryCycles*factor; i++)
            {
                cnn.Open();
                insertCommand.ExecuteReader();
                cnn.Close();
                cnn.Open();
                dataReader = selectCommand.ExecuteReader();
                dataReader.Read();
                cnn.Close();
                cnn.Open();
                deleteCommand.ExecuteReader();

                cnn.Close();
            }
        }
        static async Task InsertGetDeleteAPIConnect(int factor)
        {
            // Making call with CreateNewUsers
            ConfigurationManager config = new ConfigurationManager();
            List<User> users = new List<User>();
            List<User> resultsend = new List<User>();
            List<User> resultsDelete = new List<User>();
            users = TestUser.GenerateUserList(numUsers);

            //sbTime.Append($"Executing InsertGetDelete API {queryCycles} times...\n");

            for (int i = 0; i < queryCycles*factor; i++)
            {
                resultsend = await ApiService.CreateNewUsers(users);
                _ = await ApiService.GetAllUsersAsync();
                resultsDelete = await ApiService.DeleteUsers(resultsend);
            }
                




        }
        static async Task InsertGetDeleteAPIConnectParallel(int factor)
        {
            // Making call with CreateNewUsers
            ConfigurationManager config = new ConfigurationManager();
            List<User> users = new List<User>();
            List<User> resultsend = new List<User>();
            List<User> resultsDelete = new List<User>();
            users = TestUser.GenerateUserList(numUsers);


            for (int i = 0; i < queryCycles*factor; i++)
            {
                resultsend = await ApiService.CreateNewUsers(users);
                _ = await ApiService.GetAllUsersAsync();
                resultsDelete = await ApiService.DeleteUsers(resultsend);
            }



        }

        static async Task TestGet()
        {
            PerformanceCounterCategory.Exists("PerformanceCounter");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set", "_Total");
            cpuCounter.NextValue();
            memCounter.NextValue();

            //Execute GetDirectConnect in one thread

            sbTime.Append($"Executing Get Query {queryCycles} times...\n");
            sbProc.Append($"Executing Get Query {queryCycles} times...\n");
            sbMem.Append($"Executing Get Query {queryCycles} times...\n");
            for (int i = 0; i < numTests; i++)
            {
                


                GetDirectConnect(i+1);


                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();

                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                Console.WriteLine("Completed test " + i);

                GC.Collect();
            }




            sbTime.Append($"Executing Get API {queryCycles} times...\n");
            sbProc.Append($"Executing Get API {queryCycles} times...\n");
            sbMem.Append($"Executing Get API {queryCycles} times...\n");

            //Execute GetAPIConnect in one thread

            for(int i = 0; i < numTests; i++)
            {
                DateTime start2 = DateTime.Now;
                
                await GetAPIConnect(i+1);

                DateTime end2 = DateTime.Now;
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                sbTime.Append((end2 - start2).TotalMilliseconds + "\n");
                //Console.WriteLine("Completed test " + i);
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                GC.Collect();
            }





        }
        static async Task TestGetParallel()
        {
            PerformanceCounterCategory.Exists("PerformanceCounter");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set", "_Total");
            //Execute GetDirectConnect in one thread
            cpuCounter.NextValue();
            memCounter.NextValue();
            sbTime.Append($"Executing Get Query {queryCycles} times...\n");
            sbProc.Append($"Executing Get Query {queryCycles} times...\n");
            sbMem.Append($"Executing Get Query {queryCycles} times...\n");
            for (int i = 0; i < numTests; i++)
            {
                DateTime start = DateTime.Now;
                
                GetDirectConnect(i+1);

                DateTime end = DateTime.Now;
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                sbTime.Append((end - start).TotalMilliseconds + "\n");
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                Console.WriteLine("Completed test " + i);

                
                GC.Collect();
            }



            sbTime.Append($"Executing Get API {queryCycles} times...\n");
            sbProc.Append($"Executing Get API {queryCycles} times...\n");
            sbMem.Append($"Executing Get API {queryCycles} times...\n");

            //Execute GetAPIConnect in one thread

            for (int i = 0; i < numTests; i++)
            {
                DateTime start2 = DateTime.Now;
                
                await GetAPIConnectParallel(i+1);

                DateTime end2 = DateTime.Now;
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                sbTime.Append((end2 - start2).TotalMilliseconds + "\n");
                //Console.WriteLine("Completed test " + i);
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                GC.Collect();
            }


        }
        static async Task TestInsertGetDelete()
        {
            PerformanceCounterCategory.Exists("PerformanceCounter");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set", "_Total");
            cpuCounter.NextValue();
            memCounter.NextValue();
            //Execute InsertGetDeleteDirectConnect in one thread
            sbTime.Append($"Executing InsertGetDeleteDirectConnect {queryCycles} times...\n");
            sbProc.Append($"Executing InsertGetDeleteDirectConnect {queryCycles} times...\n");
            sbMem.Append($"Executing InsertGetDeleteDirectConnect {queryCycles} times...\n");
            for (int i = 0; i < numTests; i++)
            {
                DateTime start = DateTime.Now;
                
                InsertGetDeleteDirectConnect(i+1);
                DateTime end = DateTime.Now;
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                sbTime.Append((end - start).TotalMilliseconds + "\n");
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                Console.WriteLine("Completed test " + i);
                GC.Collect();
            }


            //Execute InsertGetDeleteAPIConnect in one thread
            sbTime.Append($"Executing InsertGetDeleteAPIConnect API {queryCycles} times...\n");
            sbProc.Append($"Executing InsertGetDeleteAPIConnect API {queryCycles} times...\n");
            sbMem.Append($"Executing InsertGetDeleteAPIConnect API {queryCycles} times...\n");
            for (int i = 0; i < numTests; i++)
            {
                DateTime start2 = DateTime.Now;
                
                await InsertGetDeleteAPIConnect(i+1);

                DateTime end2 = DateTime.Now;
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                sbTime.Append((end2 - start2).TotalMilliseconds + "\n");
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                Console.WriteLine("Completed test " + i);
                GC.Collect();
            }
        }
        static async Task TestInsertGetDeleteParallel()
        {
            PerformanceCounterCategory.Exists("PerformanceCounter");
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set", "_Total");
            cpuCounter.NextValue();
            memCounter.NextValue();
            //Execute InsertGetDeleteDirectConnect in one thread
            sbTime.Append($"Executing InsertGetDeleteDirectConnect {queryCycles} times...\n");
            sbProc.Append($"Executing InsertGetDeleteDirectConnect {queryCycles} times...\n");
            sbMem.Append($"Executing InsertGetDeleteDirectConnect {queryCycles} times...\n");

            for (int i = 0; i < numTests; i++)
            {
                DateTime start = DateTime.Now;
                
                InsertGetDeleteDirectConnect(i + 1);
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                DateTime end = DateTime.Now;

                sbTime.Append((end - start).TotalMilliseconds + "\n");
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem/1024/1024) + "\n");
                Console.WriteLine("Completed test " + i);
                GC.Collect();
            }

            //Execute InsertGetDeleteAPIConnect in one thread
            sbTime.Append($"Executing InsertGetDeleteAPIConnect API {queryCycles} times...\n");
            sbProc.Append($"Executing InsertGetDeleteAPIConnect API {queryCycles} times...\n");
            sbMem.Append($"Executing InsertGetDeleteAPIConnect API {queryCycles} times...\n");
            for (int i = 0; i < numTests; i++)
            {
                DateTime start2 = DateTime.Now;
                
                await InsertGetDeleteAPIConnectParallel(i + 1);

                DateTime end2 = DateTime.Now;
                float endProc = cpuCounter.NextValue();
                float endMem = memCounter.NextValue();
                sbTime.Append((end2 - start2).TotalMilliseconds + "\n");
                sbProc.Append((endProc) + "\n");
                sbMem.Append((endMem / 1024 / 1024) + "\n");
                Console.WriteLine("Completed test " + i);

                GC.Collect();
            }
        }
    

        static async Task Main(string[] args)
        {
            Global.facultyIds = new string[] {"AFE3FA80-C76E-4154-A2B6-0985A0AA2C35",
                                        "DFE22AE5-1C51-4C06-B8F1-0B21C1C66959",
                                        "316D918B-2EA6-4D9E-975E-29CAD91238E7",
                                        "3FA85F64-5717-4562-B3FC-2C963F66AFA6",
                                        "452DF401-642C-4704-909B-2FC9A28D3808",
                                        "F95185B9-DD92-4A1A-89C4-59E93B2A7AD4",
                                        "907452FA-DB99-4C2D-82F5-6315F4D28799",
                                        "140BFDB2-DE93-425C-9A05-64FA42C1282B",
                                        "4A4D312B-14FB-46E0-B8AE-80F8B6CA2A51",
                                        "AD352F6B-EF68-4202-8102-8B0EC7E3B4EA",
                                        "59E40423-813A-4A4F-A55D-9037389C6DC1",
                                        "B9112CE9-0B12-4682-8266-D3EB20346816",
                                        "CCD3C5D8-CDDE-4F7E-A812-D9C917D3A0AD",
                                        "9824581E-A84B-4979-908E-FAA935AA8985" };

            if (useGet)
            {
                if(isParallel)
                {
                    await TestGetParallel();
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestGetParallel.txt", sbTime.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestGetParallelProc.txt", sbProc.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestGetParallelMem.txt", sbMem.ToString());
                    
                }
                else
                {
                    await TestGet();
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestGetSerial.txt", sbTime.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestGetSerialProc.txt", sbProc.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestGetSerialMem.txt", sbMem.ToString());
                }

            }
            else
            {
                if(isParallel)
                {
                    await TestInsertGetDeleteParallel();
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestInsertGetDeleteParallel.txt", sbTime.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestInsertGetDeleteParallelProc.txt", sbProc.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestInsertGetDeleteParallelMem.txt", sbMem.ToString());
                }
                else
                {
                    await TestInsertGetDelete();
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestInsertGetDeleteSerial.txt", sbTime.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestInsertGetDeleteSerialProc.txt", sbProc.ToString());
                    await File.WriteAllTextAsync("..\\..\\..\\..\\TestLogs\\TestInsertGetDeleteSerialMem.txt", sbMem.ToString());
                }
            }
            Console.WriteLine("Logs stored at WKEXP905\\TestLogs)");

        }
        
    }
}