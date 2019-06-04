using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using BondoraWH.Models;
using System.Threading;

namespace BondoraWH
{
    public partial class Form1 : Form
    {
        ApiClient apiClient;
        ApiClient defApiClient;
        private HttpListener listener;

        List<SecondMarketItem> defaultedLoansQue;

        const bool BUY_FIRST_PAYMENT = false;
        const decimal DEFAULT_MAX_PRICE = 5;
        const int DEFAULT_DISCOUNT = -50;

        bool isRunning;
        static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        List<int> auctionNumbers;

        int totalLoans;
        int totalDiscountedLoans;
        int buyLoans;
        int discountedLoans;
        decimal totalPrice;
        decimal totalDiscountedPrice;
        decimal totalBuyPrice;
        decimal totalDiscountedBuyPrice;
        decimal maxPrice;

        int[] daysArray = new int[7] { 1, 1, 1, 1, 1, 1, 1 };

        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            apiClient = new ApiClient(ApiConfig.ApiBaseUri);
            defApiClient = new ApiClient(ApiConfig.ApiBaseUri);
            listener = new HttpListener();
            listener.Prefixes.Add("http://+:22555/");
            auctionNumbers = new List<int>();
            defaultedLoansQue = new List<SecondMarketItem>();
            isRunning = false;
        }

        private async void listen(int[] buyForXDaysArr)
        {
            while (true)
            {
                var context = await listener.GetContextAsync();
                Task.Factory.StartNew(() => processRequest(context, buyForXDaysArr));
            }
            listener.Close();
        }

        private async void processRequest(HttpListenerContext context, int[] buyForXDaysArr)
        {
            SecondMarketItem secondMarketItem = null;
            // MessageBox.Show("Gavau..." + context.Request.Headers.ToString());
            var body = new StreamReader(context.Request.InputStream).ReadToEnd();

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            try
            {
                ApiResultEvent apiResultEvent = jsonSerializer.Deserialize<ApiResultEvent>(body);
                secondMarketItem = apiResultEvent.Payload;
            }
            catch (Exception e) { }



            byte[] b = Encoding.UTF8.GetBytes("ACK");
            context.Response.StatusCode = 200;
            context.Response.KeepAlive = false;
            context.Response.ContentLength64 = b.Length;

            var output = context.Response.OutputStream;
            output.Write(b, 0, b.Length);
            context.Response.Close();

            if (secondMarketItem.LoanStatusCode == 2)
                checkCurrentLoan(secondMarketItem, buyForXDaysArr[(int)DateTime.Today.DayOfWeek], maxPrice);
            else
                checkDefaultedLoanToQue(secondMarketItem, DEFAULT_DISCOUNT);

            if (secondMarketItem.DesiredDiscountRate <= 0)
            {

                string log = "";
                log = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\t" + secondMarketItem.LoanStatusCode.ToString() + "\t" + secondMarketItem.LoanPartId.ToString() + "\t" + secondMarketItem.UserName + "\t" +
                    secondMarketItem.PrincipalRemaining.ToString("0.##€") + "\t" + secondMarketItem.DesiredDiscountRate.ToString("0.##") + "\t" +
                    secondMarketItem.Price.ToString("0.##€") + "\t" + secondMarketItem.PrincipalRepaid.ToString("0.##€") + "\t" +
                    secondMarketItem.LastPaymentDate.ToString() + "\t" + secondMarketItem.NextPaymentDate.ToString() + "\t" + secondMarketItem.NextPaymentNr.ToString("0.##") + "\t" +
                    secondMarketItem.LateAmountTotal.ToString("0.##€") + "\t" + secondMarketItem.ReScheduledOn.ToString();
                textBox3.Invoke(new Action(() => textBox3.AppendText(log + "\r\n")));
            }

        }

        private async void checkCurrentLoan(SecondMarketItem item, int buyForNextXDays, decimal maxPrice)
        {

            bool onlyDiscounted = true;
            int minPaymets = 1;
            if (BUY_FIRST_PAYMENT)
                minPaymets = 0;
            string pastaba = "\t";
            DateTime nextPaymentT = DateTime.Today.AddDays(buyForNextXDays);

            bool buyThisLoan = true;
            bool logThisLoan = true;

            if (item.DesiredDiscountRate > 0)
            {
                pastaba += "Su antkainiu ";
                buyThisLoan = false;
                logThisLoan = false;
            }

            if (item.DesiredDiscountRate == 0 && item.NextPaymentDate > nextPaymentT)
            {
                pastaba += "Neatitinka kriterijai ";
                buyThisLoan = false;
            }

            if (item.DesiredDiscountRate == 0 && auctionNumbers.Contains(item.AuctionNumber))
            {
                pastaba += "Jau turim ";
                buyThisLoan = false;
            }

            if (item.Price > maxPrice)
            {
                pastaba += "Brangu ";
                buyThisLoan = false;
            }

            if (item.NextPaymentNr <= minPaymets)
            {
                pastaba += "Mažai įmokų ";
                buyThisLoan = false;
            }

            if (item.LateAmountTotal > 0)
            {
                pastaba += string.Format("Vėluojanti suma {0} ", item.LateAmountTotal.ToString("0.##€"));
                buyThisLoan = false;
            }

            if (item.DesiredDiscountRate == 0 && item.LastPaymentDate > DateTime.Today.AddDays(-20))
            {
                pastaba += "Nesenas mokėjimas ";
                buyThisLoan = false;
            }
            int daysSpan = 0;
            if (item.LastPaymentDate != null)
                daysSpan = (int)(item.NextPaymentDate - item.LastPaymentDate).Value.TotalDays;

            if (daysSpan > 45)
            {
                pastaba += "Didelis tarpas ";
                buyThisLoan = false;
            }

            string rowString = "";
            string buyFile;
            string currFile;

            if (item.DesiredDiscountRate < 0)
            {
                totalDiscountedLoans++;
                totalDiscountedPrice += item.Price;
                rowString = "%%\t";
                buyFile = "buydiscounted.txt";
                currFile = "discLoans.txt";
            }
            else
            {
                totalLoans++;
                totalPrice += item.Price;
                rowString = "--\t";
                buyFile = "buyloans.txt";
                currFile = "currLoans.txt";
            }

            rowString += DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString() + "\t" + item.LoanPartId.ToString() + "\t" + item.UserName + "\t" +
               item.PrincipalRemaining.ToString("0.##€") + "\t" + item.DesiredDiscountRate.ToString("0.##") + "\t" +
               item.Price.ToString("0.##€") + "\t" + item.PrincipalRepaid.ToString("0.##€") + "\t" +
               item.LastPaymentDate.ToString() + "\t" + item.NextPaymentDate.ToString() + "\t" + item.NextPaymentNr.ToString("0.##") + "\t" +
               item.LateAmountTotal.ToString("0.##€") + "\t" + item.ReScheduledOn.ToString();

            if (buyThisLoan)
            {
                var list = new List<Guid>();
                list.Add(item.Id);
                pastaba += "Perkam... ";
                int buyStatusCode = await apiClient.BuySecondaryMarketItem(list);
                if (buyStatusCode == 202)
                {
                    pastaba += " Nupirkom!";
                    if (onlyDiscounted)
                    {
                        discountedLoans++;
                        totalDiscountedBuyPrice += item.Price;
                    }
                    else
                    {
                        buyLoans++;
                        totalBuyPrice += item.Price;
                    }

                    auctionNumbers.Add(item.AuctionNumber);

                    WriteToFileThreadSafe(rowString + pastaba, buyFile);

                }
                else
                {
                    switch (buyStatusCode)
                    {
                        case 400:
                            pastaba += "No items specified";
                            break;
                        case 401:
                            pastaba += "Too many items";
                            break;
                        case 402:
                            pastaba += "User is not Authorized";
                            break;
                        case 403:
                            pastaba += "User has no rights";
                            break;
                        case 404:
                            pastaba += "Not found";
                            break;
                        case 409:
                            pastaba += "Investment cannot be bought";
                            break;

                        default:
                            pastaba += buyStatusCode.ToString();
                            break;

                    }
                }

            }



           

            if (logThisLoan)
                WriteToFileThreadSafe(rowString + pastaba, currFile);

           
        //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(currFile, true))
        //        {
        //            file.WriteLine(rowString + pastaba);
         //       }

           
        }

        private async void checkDefaultedLoanToQue(SecondMarketItem item, int discountRate)
        {
            if (item.LastPaymentDate > DateTime.Now.AddDays(35) && item.DesiredDiscountRate <= discountRate)
                defaultedLoansQue.Add(item);
        }


        private async void processDefaultedQue()
        {
            List<LoanPartDetails> list = new List<LoanPartDetails>();
            while (true)
            {
                if(defaultedLoansQue.Count > 0)
                {
                    List<Guid> ids = new List<Guid>();
                    foreach (SecondMarketItem item in defaultedLoansQue)
                        ids.Add(item.Id);
                   // list = await apiClient.GetLoanPartDetails(ids).p;
                }
            }
        }

        public void WriteToFileThreadSafe(string text, string path)
        {
            // Set Status to Locked
            locker.EnterWriteLock();
            try
            {
                // Append text to the file
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                locker.ExitWriteLock();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var scopes = string.Join("%20", ApiConfig.OAuthScopes.Split(','));
            string authUri = string.Format("{0}?client_id={1}&scope={2}&response_type=code&redirect_uri={3}",
               ApiConfig.OAuthAuthorizeUri, ApiConfig.ClientId, scopes, ApiConfig.RedirectUri);
            System.Diagnostics.Process.Start(authUri);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            var accessTokenResult = await apiClient.GetAccessTokenByCode(textBox1.Text,
                ApiConfig.ClientId, ApiConfig.ClientSecret, ApiConfig.RedirectUri);
            if (accessTokenResult == null)
            {
                toolStripStatusLabel1.Text = "Current Loans: Not connected";
            }
            else
            {
                toolStripStatusLabel1.Text = "Curren Loans: Connected";
                string validUntil;
                if (accessTokenResult.valid_until == 0)
                    validUntil = "Not expire";
                else
                    validUntil = GetDateTimeFromUnixTime(accessTokenResult.valid_until).ToString();
                toolStripStatusLabel3.Text = "Valid until: " + validUntil;
                apiClient.AccessToken = accessTokenResult.access_token;
                apiClient.RefreshToken = accessTokenResult.refresh_token;
                apiClient.TokenValidUntilUtc = accessTokenResult.valid_until > 0
                    ? (DateTime?)GetDateTimeFromUnixTime(accessTokenResult.valid_until)
                    : null;
                ShowBalance();
            }
            button3.Enabled = true;
            auctionNumbers = await getInvestmentsNumbers();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox2.Text, out maxPrice))
                maxPrice = DEFAULT_MAX_PRICE;
            textBox2.Enabled = true;
            button4.Enabled = true;
            button3.Enabled = false;
            isRunning = true;

            try
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader("config.txt"))
                {
                    string line = file.ReadLine();
                    string[] valuesArray = line.Split(' ');

                    for (int i = 0; i < 7; i++)
                    {
                        if (!int.TryParse(valuesArray[i], out daysArray[i]))
                            daysArray[i] = 0;
                    }
                }
            }
            catch (Exception b) { }

            listener.Start();
            listen(daysArray);
        }

        private async void ShowBalance()
        {
            var balancePayload = await apiClient.GetAccountBalance();
            if (balancePayload != null)
            {
                AccountBalance balance = balancePayload.Payload;
                toolStripStatusLabel5.Text = balance.Balance.ToString("0.## €");
            }
            else
            {
                toolStripStatusLabel5.Text = "Failed";
                //ShowBalance();
            }
        }

        private async void ShowDefaultedBalance()
        {
            var balancePayload = await defApiClient.GetAccountBalance();
            if (balancePayload != null)
            {
                AccountBalance balance = balancePayload.Payload;
                toolStripStatusLabel5.Text = balance.Balance.ToString("0.## €");
            }
            else
            {
                toolStripStatusLabel5.Text = "Failed";
                ShowBalance();
            }
        }

        private async Task<List<int>> getInvestmentsNumbers()
        {
            List<Investment> investments = new List<Investment>();
            var result = await apiClient.GetInvestments(1, 10000, null, null, null, null);
            if (result != null && result.Payload != null)
            {
                investments = result.Payload.ToList();
            }


            List<int> retvalue = new List<int>();
            if (investments != null)
            {
                foreach (Investment item in investments)
                    retvalue.Add(item.AuctionNumber);
            }
            return retvalue;
        }

        public static DateTime GetDateTimeFromUnixTime(long unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(unixTime);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length != 0)
                button2.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button3.Enabled = true;
            isRunning = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var scopes = string.Join("%20", ApiConfig.OAuthScopes.Split(','));
            string authUri = string.Format("{0}?client_id={1}&scope={2}&response_type=code&redirect_uri={3}",
               ApiConfig.OAuthAuthorizeUri, ApiConfig.ClientId, scopes, ApiConfig.RedirectUri);
            System.Diagnostics.Process.Start(authUri);
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            var accessTokenResult = await defApiClient.GetAccessTokenByCode(textBox5.Text,
                ApiConfig.ClientId, ApiConfig.ClientSecret, ApiConfig.RedirectUri);
            if (accessTokenResult == null)
            {
                toolStripStatusLabel7.Text = "Defaulted Loans: Not connected";
            }
            else
            {
                toolStripStatusLabel7.Text = "Defaulted Loans: Connected";
                string validUntil;
                if (accessTokenResult.valid_until == 0)
                    validUntil = "Not expire";
                else
                    validUntil = GetDateTimeFromUnixTime(accessTokenResult.valid_until).ToString();
                toolStripStatusLabel9.Text = "Valid until: " + validUntil;
                defApiClient.AccessToken = accessTokenResult.access_token;
                defApiClient.RefreshToken = accessTokenResult.refresh_token;
                defApiClient.TokenValidUntilUtc = accessTokenResult.valid_until > 0
                    ? (DateTime?)GetDateTimeFromUnixTime(accessTokenResult.valid_until)
                    : null;
                ShowDefaultedBalance();
            }
            button6.Enabled = true;

        }
    }
}
