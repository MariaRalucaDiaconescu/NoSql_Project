﻿using System;
using System.Collections.Generic;
using Model;
using Logic;

namespace AutoTicketInsertTool
{
    internal class Program
    {
        static Random rnd = new Random();
        static readonly string[] description = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

        static readonly string[] titleFirstPart = new[] { "Internet", "Computer", "Monitor", "Laptop", "Keyboard", "Application", "Software" };
        static readonly string[] titleLastPart = new[] { "is not working", "is broken", "needs to be repaired", "is not responding", "is having issues" };

        static readonly int[] possibleDeadlineDaysCount = { 7, 14, 28, 180 };

        const int MaxTicketsAtOnce = 100;


        public static void Main()
        {
            Console.WriteLine("/// Auto Ticket Insert Tool ///");
            Console.WriteLine("/// by Yours Truly! ///\n");
            Console.Write("How many tickets do you want to insert? ");

            int ticketsToInsert = int.Parse(Console.ReadLine());
            Console.WriteLine("\n\n");

            if (ticketsToInsert > MaxTicketsAtOnce)
            {
                Console.WriteLine($"Can't add more than {MaxTicketsAtOnce} tickets at once.");
            }
            else
            {
                Start(ticketsToInsert);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void Start(int ticketsToInsert)
        {
            EmployeeService employeeService = new EmployeeService();
            List<Employee> employees = employeeService.GetEmployees();
            var ticketService = new TicketsService();


            int fails = 0;
            for (int i = 0; i < ticketsToInsert; i++)
            {
                try
                {
                    var texts = new TicketTextTransfer(GenerateTitle(), GenerateDescription());
                    var enums = GenerateRandomEnums();
                    var employeesEnum = GenerateRandomAssignedEmployees(employees);
                    var dates = GenerateRandomDates();

                    string ticketDetails = $"Subject: {texts.Subject}\nDescription: {texts.Description}\n" +
                        $"IncidentType: {enums.IncidentType}, Priority: {enums.Priority}, Status: {enums.Status}, EscalationLevel: {enums.EscalationLevel}\n" +
                        $"Reporter: {employeesEnum.Reporter}\nAssignedEmployee{employeesEnum.AssignedEmployee}\n" +
                        $"Date: {dates.Date}\nDeadline: {dates.DeadlineDays}\n\n";

                    Console.WriteLine(ticketDetails);
                    ticketService.InsertTicket(texts, dates, enums, employeesEnum);
                }
                catch
                {
                    fails++;
                }
            }

            Console.WriteLine($"Inserted {ticketsToInsert - fails} tickets.");
            if (fails > 0)
            {
                Console.Write($" ({fails} failed)");
            }
        }

        private static string GenerateRandomSentence(string[] words, int minWords, int maxWords, bool doNotCapitalize)
        {
            int length = rnd.Next(minWords, maxWords);
            string output = "";
            string previousWord = "";
            for (int i = 0; i < length; i++)
            {
                string word;
                do
                {
                    word = words[rnd.Next(words.Length)];
                } while (word == previousWord);


                if (i == 0 && !doNotCapitalize)
                {
                    char upper = char.ToUpper(word[i]);
                    word = upper + word.Substring(1);
                }
                output += word;
                if (i < length - 1)
                {
                    output += rnd.Next(0, 4) == 0 ? ", " : " ";
                }
                previousWord = word;
            }

            return output;
        }

        private static string GenerateRandomSentences(string[] words, int minWords, int maxWords, int minSentences, int maxSentences, bool doNotCapitalize)
        {
            int sentences = rnd.Next(minSentences, maxSentences);
            string output = "";
            for (int i = 0; i < sentences; i++)
            {
                output += GenerateRandomSentence(words, minWords, maxWords, doNotCapitalize);
                output += ". ";
            }
            return output;
        }

        private static string GenerateTitle()
        {
            return GenerateRandomSentence(titleFirstPart, 1, 1, false) + " " + GenerateRandomSentence(titleLastPart, 1, 1, true);
        }

        private static string GenerateDescription()
        {
            return GenerateRandomSentences(description, 5, 10, 2, 10, false);
        }

        private static TicketEmployeeTransfer GenerateRandomAssignedEmployees(List<Employee> employees)
        {
            var output = new TicketEmployeeTransfer();
            output.Reporter = employees[rnd.Next(employees.Count)];
            do
            {
                output.AssignedEmployee = employees[rnd.Next(employees.Count)];
            } while (output.Reporter == output.AssignedEmployee);
            return output;
        }

        private static TicketDateTransfer GenerateRandomDates()
        {
            var output = new TicketDateTransfer();

            int month = rnd.Next(1, DateTime.Now.Month + 1);
            int day = rnd.Next(1, 29);
            if (month == DateTime.Now.Month)
            {
                day = rnd.Next(1, DateTime.Now.Day + 1);
            }

            output.Date = new DateTime(2022, month, day);
            output.DeadlineDays = possibleDeadlineDaysCount[rnd.Next(0, possibleDeadlineDaysCount.Length)];

            return output;
        }

        private static TicketEnumsTransfer GenerateRandomEnums()
        {
            var output = new TicketEnumsTransfer();

            output.EscalationLevel = 0;
            output.Priority = (TicketPriority)rnd.Next(0, Enum.GetValues(typeof(TicketPriority)).Length);
            output.Status = (TicketStatus)rnd.Next(0, Enum.GetValues(typeof(TicketStatus)).Length);
            if (output.Status == TicketStatus.PastDeadline)
            {
                output.Status = TicketStatus.Open;
            }
            output.IncidentType = (IncidentTypes)rnd.Next(0, Enum.GetValues(typeof(IncidentTypes)).Length);

            return output;
        }
    }
}
