// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();

// app.MapGet("/", () => "Host=aws-0-ap-south-1.pooler.supabase.com;Port=5432;Username=postgres.pidvimlpfbtztwitweml;Password=MAY23/postgressql/v1;Database=postgres");

// app.Run();



using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string connectionString = "Host=aws-0-ap-south-1.pooler.supabase.com;Port=5432;Username=postgres.pidvimlpfbtztwitweml;Password=MAY23/postgressql/v1;Database=postgres";

app.MapGet("/", async context =>
{
    var html = @"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Enrollment Form</title>
        </head>
        <body>
            <h1>Add New Enrollment</h1>
            <form action=""/enrollments"" method=""post"">
                <label for=""studentId"">Student ID:</label>
                <input type=""number"" id=""studentId"" name=""StudentId"" required><br><br>
                <label for=""studentName"">Student Name:</label>
                <input type=""text"" id=""studentName"" name=""StudentName"" required><br><br>
                <label for=""dateOfBirth"">Date of Birth:</label>
                <input type=""date"" id=""dateOfBirth"" name=""DateOfBirth"" required><br><br>
                <label for=""gender"">Gender:</label>
                <select id=""gender"" name=""Gender"">
                    <option value=""Male"">Male</option>
                    <option value=""Female"">Female</option>
                    <option value=""Other"">Other</option>
                </select><br><br>
                <label for=""address"">Address:</label>
                <textarea id=""address"" name=""Address"" required></textarea><br><br>
                <label for=""email"">Email:</label>
                <input type=""email"" id=""email"" name=""Email"" required><br><br>
                <label for=""phoneNumber"">Phone Number:</label>
                <input type=""tel"" id=""phoneNumber"" name=""PhoneNumber"" required><br><br>
                <label for=""courseId"">Course ID:</label>
                <input type=""number"" id=""courseId"" name=""CourseId"" required><br><br>
                <label for=""enrollmentDate"">Enrollment Date:</label>
                <input type=""date"" id=""enrollmentDate"" name=""EnrollmentDate"" required><br><br>
                <label for=""registrationDate"">Registration Date:</label>
                <input type=""date"" id=""registrationDate"" name=""RegistrationDate"" required><br><br>
                <button type=""submit"">Submit</button>
            </form>
        </body>
        </html>";
    await context.Response.WriteAsync(html);
});

app.MapPost("/enrollments", async context =>
{
    var form = await context.Request.ReadFormAsync();

    var studentId = form["StudentId"];
    var studentName = form["StudentName"];
    var dateOfBirth = form["DateOfBirth"];
    var gender = form["Gender"];
    var address = form["Address"];
    var email = form["Email"];
    var phoneNumber = form["PhoneNumber"];
    var courseId = form["CourseId"];
    var enrollmentDate = form["EnrollmentDate"];
    var registrationDate = form["RegistrationDate"];

    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    var createTableQuery = @"
        CREATE TABLE IF NOT EXISTS testx (
            StudentId INTEGER PRIMARY KEY,
            StudentName TEXT,
            DateOfBirth DATE,
            Gender TEXT,
            Address TEXT,
            Email TEXT,
            PhoneNumber TEXT,
            CourseId INTEGER,
            EnrollmentDate DATE,
            RegistrationDate DATE
        )";

    await using (var createTableCmd = new NpgsqlCommand(createTableQuery, connection))
    {
        await createTableCmd.ExecuteNonQueryAsync();
    }

    var insertQuery = @"
        INSERT INTO testx (StudentId, StudentName, DateOfBirth, Gender, Address, Email, PhoneNumber, CourseId, EnrollmentDate, RegistrationDate)
        VALUES (@StudentId, @StudentName, @DateOfBirth, @Gender, @Address, @Email, @PhoneNumber, @CourseId, @EnrollmentDate, @RegistrationDate)";

    await using (var cmd = new NpgsqlCommand(insertQuery, connection))
    {
        cmd.Parameters.AddWithValue("StudentId", int.Parse(studentId));
        cmd.Parameters.AddWithValue("StudentName", studentName.ToString());
        cmd.Parameters.AddWithValue("DateOfBirth", DateTime.Parse(dateOfBirth));
        cmd.Parameters.AddWithValue("Gender", gender.ToString());
        cmd.Parameters.AddWithValue("Address", address.ToString());
        cmd.Parameters.AddWithValue("Email", email.ToString());
        cmd.Parameters.AddWithValue("PhoneNumber", phoneNumber.ToString());
        cmd.Parameters.AddWithValue("CourseId", int.Parse(courseId));
        cmd.Parameters.AddWithValue("EnrollmentDate", DateTime.Parse(enrollmentDate));
        cmd.Parameters.AddWithValue("RegistrationDate", DateTime.Parse(registrationDate));

        await cmd.ExecuteNonQueryAsync();
    }

    await context.Response.WriteAsync("Enrollment added successfully!");
});

app.Run();

// dotnet new web -n Tablex
//dotnet run 

// https://supabase.com/dashboard/project/pidvimlpfbtztwitweml/editor