using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
namespace getlog
{
    struct LogData
    {
        public long InstanceID;
        public string Message;
        public DateTime TimeWritten;
        public override string ToString()
        {
            return $"InstanceID: {this.InstanceID}\nTimeWritten: {this.TimeWritten}\nMessage: {this.Message}\n\n";
        }
    }
    class Program
    {
        static DateTime _startDate;
        static DateTime _endDate;
        static string _fileName;
        static void Main(string[] args)
        {
            _startDate = new DateTime(2019, 08, 15, 09, 00, 00);
            _endDate = new DateTime(2019, 08, 15, 10, 00, 00);
            _fileName = Path.Combine(Directory.GetCurrentDirectory(), 
                $"./savedLogs_{_endDate.Year}_{_endDate.Month}_{_endDate.Day}.txt");
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
            Console.WriteLine($"Start Date: {_startDate}, end date: {_endDate}\nLogs file: {_fileName}");
            _saveToFile(_fileName, _parseLogsByDate("Security", _startDate, _endDate));
            Console.WriteLine("Done! Press any key to exit...");
        }
        static IEnumerable<LogData> _parseLogsByDate(string logsName, DateTime dateStart, DateTime dateEnd)
        {
            var secLog = new EventLog(logsName);
            foreach(EventLogEntry entry in secLog.Entries)
            {
                if (_checkTime(entry.TimeWritten, dateStart, dateEnd))
                {
                    yield return new LogData
                    {
                        TimeWritten = entry.TimeWritten,
                        InstanceID = entry.InstanceId,
                        Message = entry.Message
                    };
                }
            }
        }
        static bool _checkTime(DateTime dt, DateTime startDate, DateTime endDate)
        {
            return (dt >= startDate) && (dt <= endDate);
        }
        static void _saveToFile(string fileName, IEnumerable<LogData> logDatas)
        {
            using(var writer = new StreamWriter(fileName, true))
            {
                foreach(LogData ld in logDatas)
                {
                    writer.WriteLine(ld.ToString());
                }
            }
        }
    }
}
