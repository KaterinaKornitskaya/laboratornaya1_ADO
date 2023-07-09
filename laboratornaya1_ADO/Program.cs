using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;


namespace practica1
{
    internal class Program
    {
        static SqlConnection connection;
        static void Main(string[] args)
        {
            connection = new SqlConnection();
            connection.ConnectionString = @"Data Source=DESKTOP-7GU49OD\SQLEXPRESS;
                                           Initial Catalog=Sudents_Marks;
                                           Integrated Security=true;";
            try
            {
                //открываем соединение
                connection.Open();
                Console.WriteLine("Connection openned");
                GetAll(connection);
                Console.WriteLine("----------------------------------");
                GetName(connection);
                Console.WriteLine("----------------------------------");
                GetAllMarks(connection);
                Console.WriteLine("----------------------------------");
                int x = 9;
                GetStudMoreThan(connection, x);
                Console.WriteLine("----------------------------------");
                MinAvg();
                Console.WriteLine("----------------------------------");
                MaxAvg();
                Console.WriteLine("----------------------------------");
                MinMath(connection);
                Console.WriteLine("----------------------------------");
                CountMaxMath(connection);
                Console.WriteLine("----------------------------------");
                CountByGroup(connection);
                Console.WriteLine("----------------------------------");
                AvgByGroup(connection);
                Console.WriteLine("----------------------------------");
                SubjMin(connection);
                Console.WriteLine();

            }
            finally
            {
                //закрываем соединение
                connection.Close();
                Console.WriteLine("Connection closed");
            }
        }


        // Отображение всей информации из таблицы со студентами и оценками
        static void GetAll(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "select * from Marks";

            // хотела вывести названия столбцов, но почему-то выдает ошибку
            // хотя в Management studio следующий запрос работает
            //command.CommandText = "select ID as 'ID', StudentName as 'Name', StudentSurname as 'Surname', " +
            //    "GroupName as 'Group', AvgMark as 'Avarage Mark', NameSubjWithMinMark as 'Subject with min mark', " +
            //    "NameSubjWithMaxMark as 'Subject with max mark' from Marks";

            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Отображение всей информации из таблицы:");
            while (reader.Read())
            {
                Console.WriteLine(
                   $"{reader["ID"]}: {reader["StudentName"]} {reader["StudentSurname"]}" +
                   $" {reader["GroupName"]} {reader["AvgMark"]} {reader["NameSubjWithMinMark"]}" +
                   $" {reader["NameSubjWithMaxMark"]}");
            }
            reader.Close();
        }

        //Отображение ФИО всех студентов;
        static void GetName(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select StudentName, StudentSurname from Marks";
            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Отображение ФИО всех студентов:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["StudentName"]} {reader["StudentSurname"]}");
            }
            reader.Close();
        }

        //Отображение всех средних оценок
        static void GetAllMarks(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select AvgMark from Marks";
            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Отображение всех средних оценок:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["AvgMark"]}");
            }
            reader.Close();
        }

        // Показать ФИО всех студентов с минимальной оценкой, больше, чем указанная;
        static void GetStudMoreThan(SqlConnection connection, int mark)
        {
            SqlCommand cmd = new SqlCommand("Proc1", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@mark", System.Data.SqlDbType.Int).Value = mark;
            cmd.ExecuteNonQuery();

            SqlDataReader reader = cmd.ExecuteReader();
            Console.WriteLine($"Показать ФИО всех студентов с минимальной оценкой, больше, чем {mark}:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["StudentName"]} {reader["StudentSurname"]}");
            }
            reader.Close();
        }

        //Показать название всех предметов с минимальными средними оценками.
        //Названия предметов должны быть уникальными
        static void SubjMin(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select NameSubjWithMinMark from Marks group by NameSubjWithMinMark";
            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Показать название всех предметов с минимальными " +
                "средними оценками (без повторений):");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["NameSubjWithMinMark"]}");
            }
            reader.Close();
        }

        //Показать минимальную среднюю оценку;
        static void MinAvg()
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select min(AvgMark) from Marks";
            object minMark = command.ExecuteScalar();

            Console.WriteLine("Минимальная средняя оценка: {0}", minMark);
        }

        //Показать максимальную среднюю оценку;
        static void MaxAvg()
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select max(AvgMark) from Marks";
            object maxMark = command.ExecuteScalar();

            Console.WriteLine("Максимальная средняя оценка: {0}", maxMark);
        }

        //Показать студентов, у которых минимальная средняя оценка по математике;
        static void MinMath(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select StudentName, StudentSurname from Marks where NameSubjWithMinMark='subj2'";
            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Показать студентов, у которых минимальная средняя оценка по subj2");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["StudentName"]} {reader["StudentSurname"]}");
            }
            reader.Close();
        }

        //Показать количество студентов, у которых максимальная средняя оценка по математике;
        static void CountMaxMath(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select count(*) from Marks where NameSubjWithMaxMark='subj5'";
            object maxCount = command.ExecuteScalar();

            Console.WriteLine("Колличество с макс оценкой по subj5 : {0}", maxCount);
        }

        // Показать количество студентов в каждой группе
        static void CountByGroup1(SqlConnection connection)
        {
            // Ошибка - результат дает нам нам группу 123 с кол-вом студентов 2
            // и группу 125 с кол-вом студентов 3, а показывает 2 в обеих группах
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "select count(*), GroupName from Marks group by GroupName";
            object Count = command.ExecuteScalar();  // ошибка думаю здесь

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{Count}, {reader["GroupName"]}");
            }
            reader.Close();
        }

        // предыдущее задание сделала с помощью ф-ии - саму функцию сделала
        // в Management studio, ф-ия возвращает таблицу - но может можно как-то без ф-ии?
        static void CountByGroup(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "select * from Fun1()";

            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Показать количество студентов в каждой группе:");
            while (reader.Read())
            {
                Console.WriteLine(
                   $"{reader["GroupName"]}: {reader["Count"]}");
            }
            reader.Close();
        }

        // Показать среднюю оценку по группе
        // аналогично - создаю ф-ию, возвращающую таблицу, в Management studio
        static void AvgByGroup(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = "select * from Fun2()";

            SqlDataReader reader = command.ExecuteReader();
            Console.WriteLine("Показать среднюю оценку по группе:");
            while (reader.Read())
            {
                Console.WriteLine(
                   $"{reader["GroupName"]}: {reader["Avarage Mark"]}");
            }
            reader.Close();
        }
    }
}

