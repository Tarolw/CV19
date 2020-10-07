using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CV19Console
{
    class Program
    {
        // Ссылка на гит .csv файла института Хопкинса в котором находятся и постоянно обновляются данные о заболевших
        private const string data_url = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";




        // Метод, который возвращает поток
        private static async Task<Stream> GetDataStream()
        {
            // Создаем клиента
            var client = new HttpClient();
            // Получаем ответ от удаленного сервера, при этом скачиваем изначально только заголовки (все тело пока что остается в сетевой карте)
            var response = await client.GetAsync(data_url, HttpCompletionOption.ResponseHeadersRead);
            // Возвращаем поток, который обеспечит процесс чтения данных из сетевой карты
            return await response.Content.ReadAsStreamAsync();
        }




        /* Читая текстовые данные разобьем их на строки, 
         * каждая строка должна извлекаться отдельно.
         * Для этого заведем метод (именованный итератор) который будет возвращать перечисление строк.
         */



        // Разбиваем поток на последовательность строк
        // Метод (именованный итератор) который будет возвращать перечисление строк
        private static  IEnumerable<string> GetDataLines()
        {
            // Получаем поток (происходит запрос к серверу)
            using var data_stream = GetDataStream().Result;
            // Создаем объект на основе потока, который будет читать строковые данные и передаем ему поток
            using var data_reader = new StreamReader(data_stream);

            // Читаем данные пока не встретится конец потока
            while (!data_reader.EndOfStream)
            {
                // Пока не конец потока, извлекаем из ридера очередную строку и помещаем ее в переменную
                var line = data_reader.ReadLine();
                // Проверяем, что строка не пуста, если пуста, то делаем след. цикл
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return line.Replace("Korea,", "Korea -").Replace("Bonaire,", "Bonaire -");
            }
        }




        /* Распарсим первую строку, извлечем из нее все даты, 
         * чтобы получить массив дат, по которому мы будем работать
         */


        // Получаем массив дат, по которым у нас будут разбиты данные
        private static DateTime[] GetDates() => GetDataLines()  // GetDataLines() - получаем перечисление строк всего запроса
            .First() // Указываем, что нас интересует только первая строка
            .Split(',') // Разбиваем строку на массив строк, каждая из которых содержит заголовок каждой колонки данных csv файла
            .Skip(4) // Пропускаем первые 4 строки (Название провинции, название страны, широта и долгота)
            .Select(s => DateTime.Parse(s, CultureInfo.InvariantCulture)) // Указываем, что у нас есть строка "s", и нам надо её преобразовать в DataTime
            .ToArray();

           

        /*Теперь будем получать данные по количеству зараженных по каждой стране,
         * и каждой провинции этой страны, для этого создадим еще один метод, 
         * который будет возращать также перечисление.
         */



        // Извлекаем данные в виде кортежа
        private static IEnumerable<(string Country, string Province, int[] Counts)> GetData()
        {
            // Извлекаем общие данные, отбрасываем первую строку (заголовок)
            var lines = GetDataLines()
                .Skip(1) // Отбрасываем первую строку (заголовок)
                .Select(line => line.Split(',')); // Разбиваем строки по разделителю ",", получаем перечисление массивов строк, где каждый элемент это колонка (ячейка таблицы)
          
            // Преобразовываем данные в нужный нам кортеж
            foreach (var row in lines)
            {
                // Сначала выделим все данные в переменные, а потом разгруппируем в кортеж и вернем его
                var province = row[0].Trim();
                var country_name = row[1].Trim(' ', '"');
                var counts = row.Skip(4).Select(int.Parse).ToArray();

                yield return (country_name, province, counts);
            }
        }




        static void Main(string[] args)
        {
            //var web_client = new WebClient(); 

            //var client = new HttpClient();

            //var response = client.GetAsync(data_url).Result;
            //var csv_str = response.Content.ReadAsStringAsync().Result;

            //foreach (var data_line in GetDataLines())
            //{
            //    Console.WriteLine(data_line);
            //}

            //var dates = GetDates();

            //Console.WriteLine(string.Join("\r\n", dates));

            var russia_data = GetData().First(v => v.Country.Equals("Russia", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine(string.Join("\r\n", GetDates().Zip(russia_data.Counts, (date, count) => $"Дата: {date:dd:MM:yy.} - Заражений: {count}")));

            Console.ReadLine();
        }
    }
}
