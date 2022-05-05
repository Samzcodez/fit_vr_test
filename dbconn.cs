using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Linq;

public class dbconn : MonoBehaviour
{
    private string dbName = "URI=file:Statistics.sqlite3";
    public double weight = 50;
    public double calories = 0;
    public double MET = 0;
    public double duration;

    // Start is called before the first frame update
    void Start()
    {
        CreateDB();
        /*userdata user = new userdata();
        user.userid = 3;
        user.exercise_id = 4;
        user.exercise_start = DateTime.Now.ToString("yyyy - MM - dd\\THH: mm:ss\\Z");
        user.exercise_end = DateTime.Now.ToString("yyyy - MM - dd\\THH: mm:ss\\Z");
        user.noOfSets = 7;
        user.avg_HR = 97.7;
        user.max_HR = 100.01;
        user.calories_burnt = 22;
        //AddRecord(user);

        user.userid = 3;
        user.exercise_id = 4;
        user.exercise_start = DateTime.Now.AddDays(-1).ToString("yyyy - MM - dd\\THH: mm:ss\\Z");
        user.exercise_end = DateTime.Now.ToString("yyyy - MM - dd\\THH: mm:ss\\Z");
        user.noOfSets = 5;
        user.avg_HR = 72.01;
        user.max_HR = 93.27;
        user.calories_burnt = 32;
        //AddRecord(user);

        user.userid = 3;
        user.exercise_id = 4;
        user.exercise_start = DateTime.Now.AddDays(-1).ToString("yyyy - MM - dd\\THH: mm:ss\\Z");
        user.exercise_end = DateTime.Now.ToString("yyyy - MM - dd\\THH: mm:ss\\Z");
        user.noOfSets = 5;
        user.avg_HR = 20.1;
        user.max_HR = 93.27;
        user.calories_burnt = 32;
        //AddRecord(user);
        //maxHRGraphData(3, 4);*/
        
    }

    public double calculateCaloriesBurnt(int userid, int exercise_id, DateTime exercise_start, DateTime exercise_end)
    {
        duration = (exercise_end - exercise_start).TotalMinutes;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT MET_Value FROM exercise_info where exercise_id = '" + exercise_id + "';";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Debug.Log("User ID: " + reader["userid"] + "\nexercise_name: " + reader["exercise_name"] + "\nexercise_start: " + reader["exercise_start"] + "\nexercise_end: " + reader["exercise_end"] + "\nNo_of_sets: " + reader["No_of_sets"] + "\navg_HR: " + reader["avg_HR"] + "\nmax_HR: " + reader["max_HR"] + "\nCalories Burnt: " + reader["calories_burnt"]);
                        MET = float.Parse(reader["MET_Value"].ToString());
                        print("the met value is " + MET);

                    }
                    reader.Close();
                }
            }
            connection.Close();
        }

        calories = duration * (MET * 3.5 * weight) / 200;
        return calories;
    }

    public void CreateDB()
    {
        string check;
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS session_info (userid INT, exercise_id INT, exercise_start VARCHAR(35), exercise_end VARCHAR(35), No_of_sets INT, avg_HR FLOAT, max_HR FLOAT, calories_burnt FLOAT);";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE IF NOT EXISTS exercise_info (exercise_id INT, exercise_name VARCHAR(35), MET_Value INT,UNIQUE (exercise_id) ON CONFLICT IGNORE);";
                command.ExecuteNonQuery();
                command.CommandText = "SELECT COUNT(*) FROM exercise_info;";
                //command.ExecuteNonQuery();
                check = command.ExecuteScalar().ToString();
                /*using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = int.Parse(reader[0].ToString());
                        //Debug.Log("User ID: " + reader["userid"] + "\nexercise_name: " + reader["exercise_name"] + "\nexercise_start: " + reader["exercise_start"] + "\nexercise_end: " + reader["exercise_end"] + "\nNo_of_sets: " + reader["No_of_sets"] + "\navg_HR: " + reader["avg_HR"] + "\nmax_HR: " + reader["max_HR"] + "\nCalories Burnt: " + reader["calories_burnt"]);
                    }

                    reader.Close();
                }*/
                if (check == "0")
                {
                    command.CommandText = "INSERT INTO exercise_info ( exercise_id,  exercise_name,  MET_Value) VALUES (1, 'Bicep Curls', 3);";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO exercise_info ( exercise_id,  exercise_name,  MET_Value) VALUES (2, 'Front Raises', 4);";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO exercise_info ( exercise_id,  exercise_name,  MET_Value) VALUES (3, 'Squats', 5);";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO exercise_info ( exercise_id,  exercise_name,  MET_Value) VALUES (4, 'Jumping Jacks', 6);";
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }

    public void AddRecord(userdata o)
    {

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO session_info (userid, exercise_id, exercise_start, exercise_end, No_of_sets, avg_HR, max_HR, calories_burnt) VALUES ('" + o.userid + "', '" + o.exercise_id + "', '" + o.exercise_start + "', '" + o.exercise_end + "', '" + o.noOfSets + "', '" + o.avg_HR + "', '" + o.max_HR + "', '" + o.calories_burnt + "');";
                command.ExecuteNonQuery();
            }

            connection.Close();

        }
        DisplayRecord(o.userid);
    }

    public void DisplayRecord(int userid)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM session_info s JOIN exercise_info e ON s.exercise_id = e.exercise_id WHERE s.userid = '" + userid + "'; ";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        Debug.Log("User ID: " + reader["userid"] + "\nexercise_name: " + reader["exercise_name"] + "\nexercise_start: " + reader["exercise_start"] + "\nexercise_end: " + reader["exercise_end"] + "\nNo_of_sets: " + reader["No_of_sets"] + "\navg_HR: " + reader["avg_HR"] + "\nmax_HR: " + reader["max_HR"] + "\nCalories Burnt: " + reader["calories_burnt"]);
                    reader.Close();
                }
            }
            connection.Close();
        }

    }

    public List<int> setsGraphData(int userid, int exercise_id)
    {
        List<int> sets = new List<int>();
        string dateString;
        string check;
        
        

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
               
                for (int i = 0; i < 7; i++)
                {
                    check = null;
                   
                    dateString = DateTime.Now.AddDays(-6+i).ToString("yyyyMMdd");

                    Debug.Log(dateString);
                    
                    command.CommandText = "SELECT count(*) FROM session_info WHERE SUBSTRING(REPLACE(exercise_start,' - ',''),0,9)  like '" + dateString+"' AND userid = '" + userid + "' and exercise_id = '" + exercise_id + "';";
                    check = command.ExecuteScalar().ToString();
                    if (check == "0")
                    {
                        sets.Add(0);
                    }
                    else
                    {
                        command.CommandText = "SELECT SUM(no_of_sets) FROM session_info WHERE SUBSTRING(REPLACE(exercise_start,' - ',''),0,9)  like '" + dateString + "' AND userid = '" + userid + "' and exercise_id = '" + exercise_id + "';";
                        int value = int.Parse(command.ExecuteScalar().ToString());
                        sets.Add(value);
                    }
                    
                }
            }
            connection.Close();
        }
        Debug.Log("List size:" + sets.Count);
        for(int j=0;j<7;j++)
        {
            Debug.Log("List item ["+j+"] = "+sets[j].ToString());
        }
        return sets;
    }

    public List<double> caloriesGraphData(int userid, DateTime date)
    {
        List<double> cal = new List<double>();
        string dateString;
        dateString = date.ToString("yyyyMMdd");
        string check;
        int limit;

        Debug.Log(dateString);

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select count(DISTINCT(exercise_id)) FROM exercise_info";
                limit = int.Parse(command.ExecuteScalar().ToString());
                Debug.Log("Limit:" + limit);
                for (int id = 1; id <= limit; id++)
                {
                    command.CommandText = "SELECT count(*) FROM session_info WHERE userid = '" + userid + "' AND SUBSTRING(REPLACE(exercise_start,' - ',''),0,9) like '" + dateString + "' AND exercise_id = '" + id + "'; ";
                    check = command.ExecuteScalar().ToString();
                    if (check == "0")
                    {
                        cal.Add(0);
                    }
                    else
                    {
                        command.CommandText = "SELECT SUM(calories_burnt) FROM session_info WHERE userid = '" + userid + "' AND SUBSTRING(REPLACE(exercise_start,' - ',''),0,9) like '" + dateString + "' AND exercise_id = '" + id + "'; ";
                        double value = double.Parse(command.ExecuteScalar().ToString());
                        cal.Add(value);
                    }
                }
            }
            connection.Close();
        }
        Debug.Log("List size:" + cal.Count);
        for (int j = 0; j < limit; j++)
        {
            Debug.Log("List item [" + j + "] = " + cal[j].ToString());
        }
        return cal;

    }

    public List<double> avgHRGraphData(int userid, int exercise_id)
    {
        List<double> avgHR = new List<double>();
        string dateString;
        string check;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                for (int i = 0; i < 7; i++)
                {
                    check = null;

                    dateString = DateTime.Now.AddDays(-6 + i).ToString("yyyyMMdd");

                    Debug.Log(dateString);

                    command.CommandText = "SELECT count(*) FROM session_info WHERE SUBSTRING(REPLACE(exercise_start,' - ',''),0,9)  like '" + dateString + "' AND userid = '" + userid + "' and exercise_id = '" + exercise_id + "';";
                    check = command.ExecuteScalar().ToString();
                    if (check == "0")
                    {
                        avgHR.Add(0);
                    }
                    else
                    {
                        command.CommandText = "SELECT AVG(avg_HR) FROM session_info WHERE SUBSTRING(REPLACE(exercise_start,' - ',''),0,9)  like '" + dateString + "' AND userid = '" + userid + "' and exercise_id = '" + exercise_id + "';";
                        double value = double.Parse(command.ExecuteScalar().ToString());
                        avgHR.Add(value);
                    }

                }
            }
            connection.Close();
        }
        Debug.Log("List size:" + avgHR.Count);
        for (int j = 0; j < 7; j++)
        {
            Debug.Log("List item [" + j + "] = " + avgHR[j].ToString());
        }
        return avgHR;
    }

    public List<double> maxHRGraphData(int userid, int exercise_id)
    {
        List<double> maxHR = new List<double>();
        string dateString;
        string check;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                for (int i = 0; i < 7; i++)
                {
                    check = null;

                    dateString = DateTime.Now.AddDays(-6 + i).ToString("yyyyMMdd");

                    Debug.Log(dateString);

                    command.CommandText = "SELECT count(*) FROM session_info WHERE SUBSTRING(REPLACE(exercise_start,' - ',''),0,9)  like '" + dateString + "' AND userid = '" + userid + "' and exercise_id = '" + exercise_id + "';";
                    check = command.ExecuteScalar().ToString();
                    if (check == "0")
                    {
                        maxHR.Add(0);
                    }
                    else
                    {
                        command.CommandText = "SELECT MAX(max_HR) FROM session_info WHERE SUBSTRING(REPLACE(exercise_start,' - ',''),0,9)  like '" + dateString + "' AND userid = '" + userid + "' and exercise_id = '" + exercise_id + "';";
                        double value = double.Parse(command.ExecuteScalar().ToString());
                        maxHR.Add(value);
                    }

                }
            }
            connection.Close();
        }
        Debug.Log("List size:" + maxHR.Count);
        for (int j = 0; j < 7; j++)
        {
            Debug.Log("List item [" + j + "] = " + maxHR[j].ToString());
        }
        return maxHR;
    }


}

public class userdata
{
    public int userid;
    public int exercise_id;
    public string exercise_start;
    public string exercise_end;
    public int noOfSets;
    public double avg_HR;
    public double max_HR;
    public double calories_burnt;
}



