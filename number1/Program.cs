using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TicketsLibrary;

namespace number1;
class Program
{
    
    
    static void Main()
    {
        void FeelDB()
        {
            StreamReader datar = new StreamReader("../../../NewFile2.txt");
            List<string> data = new List<string>();
            while (!datar.EndOfStream)
            {
                string? line1 = datar.ReadLine();
                data.Add(line1);
            }

            Parallel.ForEach(data, Print);

            void Print(string data)
            {
                string sURL =
                    $"https://query1.finance.yahoo.com/v7/finance/download/{data}?period1=1629072000&period2=1660608000&interval=1d&events=history&includeAdjustedClose=true";
                Task<HttpResponseMessage> request = new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, sURL));
                Task<Stream> stream1 = request.Result.Content.ReadAsStreamAsync();
                StreamReader sr1 = new StreamReader(stream1.Result);
                string data1 = sr1.ReadToEnd();
                //Thread.Sleep(200);
                //Console.Write(data1);
                if (!data1.Contains("404 Not Found:"))
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        Ticker A = new Ticker() { name = data };
                            db.Tickers.AddAsync(A);
                            List<string> days = new List<string>(data1.Split('\n'));
                            days.RemoveAt(0);
                            List<Price> temp = new List<Price>();
                            foreach (var VARIABLE in days)
                            {
                                string[] main = VARIABLE.Split(',');
                                decimal CurrentPrice;
                                decimal t1, t2;
                                if (decimal.TryParse(main[2], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out t1) && decimal.TryParse(main[3], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out t2))
                                {
                                    CurrentPrice = (t1+t2) / 2;

                                }
                                else
                                {
                                    CurrentPrice = 0;
                                }
                                DateOnly CurrentDate = DateOnly.Parse(main[0]);
                                Price B = new Price() { price = CurrentPrice, date = CurrentDate, ticker = A };
                                temp.Add(B);
                            }

                            List<string> day1 = new List<string>(days[days.Count - 1].Split(','));
                            decimal day1price;
                            if (decimal.TryParse(day1[2], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal t11) && decimal.TryParse(day1[3], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal t22))
                            {
                                day1price = (t11+t22) / 2;

                            }
                            else
                            {
                                day1price = 0;
                            }
                            List<string> day2 = new List<string>(days[days.Count - 2].Split(','));
                            decimal day2price;
                            if (decimal.TryParse(day2[2], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal t111) && decimal.TryParse(day2[3], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out decimal t222))
                            {
                                day2price = (t111+t222) / 2;

                            }
                            else
                            {
                                day2price = 0;
                            }
                            decimal Change = day1price - day2price;
                            TodaysCondition C = new TodaysCondition() { ticker = A, state = Change };
                            db.Conditions.AddAsync(C);
                            db.Prices.AddRangeAsync(temp);
                            db.SaveChangesAsync();
                        
                    }
                }
            }
        }
        FeelDB();
        //Task task1 = Task.Run(FeelDB);
        Console.WriteLine("Enter ticker: ");
        string ans = Console.ReadLine();
        //task1.Wait();
         using (ApplicationContext db = new ApplicationContext())
         {
             Ticker? ticker = db.Tickers.FirstOrDefault(p => p.name == ans);
             if (ticker != null)
             {
                 db.Conditions.Where(u=>u.tickerId == ticker.id).Load();
                 Console.WriteLine(ticker.name+":");
                 if (ticker.Condition.state > 0)
                 {
                     Console.WriteLine($"Stock rose by {ticker.Condition.state}"+'$');
                 }
                 else if (ticker.Condition.state < 0)
                 {
                     Console.WriteLine($"Stock fell by {ticker.Condition.state}"+'$');
                 }
                 else
                 {
                     Console.WriteLine("Share price has not changed");
                 }
             }
        }
    }
}

