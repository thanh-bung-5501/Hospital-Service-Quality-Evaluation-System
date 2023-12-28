using Microsoft.AspNetCore.Http;
using Repositories.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserObjects;

namespace Repositories.Utils
{
    public static class Common
    {
        #region Generation
        public static string GenerateOTPCode(int codeLength)
        {
            // initial byte array with default value in each element in array
            var bytes = new byte[codeLength];
            var randomCode = new StringBuilder();

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // using RNG to random ASCII value
                // then put into each element in byte array
                rng.GetBytes(bytes);
                string numericChar = Constants.NUMERIC_CHARACTERS;

                foreach (byte b in bytes)
                {
                    // in each element in byte array to mod the length of numeric characters
                    // then set this mod value in index of numeric characters to get value
                    randomCode.Append(numericChar[b % numericChar.Count()]);
                }
            }
            return randomCode.ToString();
        }
        public static string GenerateResetPassword(int passwordLength)
        {
            // initial byte array with default value in each element in array
            var bytes = new byte[passwordLength];
            Random rand = new Random();
            var randomPassword = new StringBuilder();

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // using RNG to random ASCII value
                // then put into each element in byte array
                rng.GetBytes(bytes);
                string upperChar = Constants.UPPER_CHARACTERS;
                string lowerChar = Constants.LOWER_CHARACTERS;
                string numericChar = Constants.NUMERIC_CHARACTERS;

                foreach (byte b in bytes)
                {
                    // randomly select a character for each byte
                    // prefer letters case 2:1 number case
                    switch (rand.Next(6))
                    {
                        // in each case use mod to project byte b to the correct range
                        case 0:
                        case 1:
                            randomPassword.Append(upperChar[b % upperChar.Count()]);
                            break;
                        case 2:
                        case 3:
                            randomPassword.Append(lowerChar[b % lowerChar.Count()]);
                            break;
                        case 4:
                        case 5:
                            randomPassword.Append(numericChar[b % numericChar.Count()]);
                            break;
                    }
                }
            }
            return randomPassword.ToString();
        }
        #endregion
        #region Email
        public static SendEmailRequest GetEmailInfo(string email, string userName, string content, string message, string? code = "")
        {
            // note in email in two case 'reset code' and 'reset password'
            string note = content.Equals("reset code") ?
                "<p class=\"code-valid\">\r\nThe code is valid for one minute.\r\n</p>" : content.Equals("reset password") ?
                "<p class=\"notice\" style=\"margin-top: 45px\">\r\nPlease do not share your password with anymore.\r\n</p>" :
                "<p class=\"notice\" style=\"margin-top: 45px\">\r\nPlease do not share your account with anymore.\r\n</p>";
            string subject = $"Your HSE {content}";
            // body message using HTML code
            string body = $"<!DOCTYPE html>\r\n" +
                $"<html>\r\n" +
                // open head
                $"<head>\r\n" +
                $"<meta charset=\"utf-8\" />\r\n" +
                $"<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n" +
                $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />\r\n" +
                // open style
                $"<style>\r\n" +
                $"body {{\r\nfont-family: Arial, Helvetica, sans-serif;\r\n}}\r\n" +
                $"section {{\r\nmargin: 45px;\r\npadding: 40px;\r\n}}\r\n" +
                $".main {{\r\nwidth: 60%;\r\nmargin: auto;\r\n}}\r\n" +
                $".main-character {{\r\npadding: 1px 15px;\r\nborder: 1px solid #c6e2ff;\r\nborder-radius: 10px;\r\noverflow: inherit;\r\n}}\r\n" +
                $".body-content {{\r\nmargin-bottom: 27px;\r\n}}\r\n" +
                $".code-character {{\r\nbackground-color: #e7f3ff;\r\npadding: 10px 15px;\r\nborder: 2px solid #0000ff;\r\nborder-radius: 7px;\r\n" +
                $"display: inline;\r\ntext-align: center;\r\nalign-content: center;\r\nfont-weight: bold;\r\n}}\r\n" +
                $".code-valid {{\r\nmargin-top: 45px;\r\nmargin-bottom: -10px;\r\n}}\r\n" +
                $".footer-content {{\r\ntext-align: center;\r\ncolor: #696969;\r\nfont-size: small;\r\n}}\r\n" +
                $".body-button {{\r\nborder-collapse: collapse;\r\nborder-radius: 6px;\r\ntext-align: center;\r\ndisplay: block;\r\nbackground: #1877f2;\r\npadding: 8px 20px 8px 20px;\r\n}}\r\n" +
                $".button-login {{\r\ntext-decoration: none;\r\ndisplay: block;\r\nfont-weight: bold;\r\n}}" +
                $"@media only screen and (max-width: 768px) {{\r\nsection {{\r\nmargin: 15px;\r\npadding: 10px;\r\n}}\r\n.main {{\r\nwidth: 100%;\r\n}}\r\n}}\r\n" +
                $"</style>\r\n" +
                // close style
                $"</head>\r\n" +
                // close head
                // open body
                $"<body>\r\n" +
                // open section
                $"<section>\r\n" +
                $"<div class=\"main\">\r\n" +
                $"<div class=\"main-character\">\r\n" +
                // open head-content
                $"<div class=\"head-content\">\r\n" +
                $"<h2>HSE System</h2>\r\n" +
                $"</div>\r\n<hr />\r\n" +
                // close head-content
                // open body-contet
                $"<div class=\"body-content\">\r\n" +
                //$"<p>Hi @{email},</p>\r\n" +
                $"<p>Hi {userName},</p>\r\n" +
                $"<p style=\"margin-bottom: 20px\">\r\nWelcome to HSE System.\r\n<br />\r\nHere's your {content}.\r\n<br />\r\n" +
                // content for reset code or password
                (content.Equals("new account") || content.Equals("reset password") ?
                $"Continue {message} HSE by the password below:\r\n</p>\r\n" +
                //$"<p class=\"code-character\">Email: <span>{email}</span></p>\r\n<br />\r\n" +
                $"<p class=\"code-character\">Password: <span style=\"color: red\">{code}</span></p>\r\n" +
                $"<p style=\"margin-top: 20px\">\r\nAlternatively, you can directly sign in HSE.\r\n" +
                $"<p class=\"body-button\">\r\n" +
                $"<a\r\nclass=\"button-login\" style=\"color: #ffffff;\" \r\nhref={Constants.HSE_INTERNAL_USER}\r\ntarget=\"_blank\"\r\n>SIGN IN</a\r\n>" +
                $"\r\n</p>" +
                $"{note}" +
                $"</div>\r\n<hr />\r\n" :
                $"Continue {message} HSE by the code below:\r\n</p>\r\n" +
                $"<p class=\"code-character\">Code: <span>{code}</span></p>\r\n" +
                $"{note}" +
                $"</div>\r\n<hr />\r\n") +
                // close body-content
                // open footer-content
                $"<div class=\"footer-content\">\r\n" +
                $"<p>from</p>\r\n" +
                $"<h3>HSE System</h3>\r\n" +
                $"<p style=\"margin-top: -10px\">Copyright© 2023 HSE System</p>\r\n" +
                $"</div>\r\n" +
                // close footer-content
                $"</div>\r\n" +
                $"</div>\r\n" +
                $"</section>\r\n" +
                // close section
                $"</body>\r\n" +
                // close body
                $"</html>\r\n";
            return new SendEmailRequest(email, subject, body, code);
        }
        public static bool SendMail(SendEmailRequest emailInfo)
        {
            bool result = true;
            string from = Constants.FROM;
            string password = Constants.PASSWORD;
            string to = emailInfo.To;

            // initial information of email
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(to);
            mailMessage.From = new MailAddress(from);
            mailMessage.Subject = emailInfo.Subject;
            mailMessage.Body = emailInfo.Body;
            // accept HTML code for body of email
            mailMessage.IsBodyHtml = true;

            // initial config for smtp client
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            // can not use default credentials for email 'from' because it needs credentials below
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(from, password);
            // custome timeout for send email ensure email sends successfully first
            smtpClient.Timeout = 180000; // 180000 miliseconds
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        #endregion
        #region Datetime
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        #endregion

        public static string GetUserRole(HttpContext context)
        {
            string role = context.User.FindFirstValue(ClaimTypes.Role);
            return role;
        }

        public static int GetUserId(HttpContext context)
        {
            int id = int.Parse(context.User.FindFirstValue("Id"));
            return id;
        }

        public static bool CheckUserIsBlock(HttpContext context)
        {
            var user = new UserDBContext().User.SingleOrDefault(x => x.UserId == GetUserId(context))!;

            if (user.Status == false) return true;
            else return false;
        }

        public static bool CheckViewRights(string role, string function)
        {
            bool result = false;
            string thisRightCheck = "";
            string[] arrRole = role.Split(" ");
            foreach (string right in arrRole)
            {
                if (right.Contains(function))
                {
                    thisRightCheck = right;
                }
            }
            if (thisRightCheck.Contains("1") || thisRightCheck.Contains("7") || thisRightCheck.Contains("15"))
            {
                result = true;
            }
            return result;
        }

        public static bool CheckCreateAndUpdateRights(string role, string function)
        {
            bool result = false;
            string thisRightCheck = "";
            string[] arrRole = role.Split(" ");
            foreach (string right in arrRole)
            {
                if (right.Contains(function))
                {
                    thisRightCheck = right;
                }
            }
            if (thisRightCheck.Contains("7") || thisRightCheck.Contains("15"))
            {
                result = true;
            }
            return result;
        }

        public static bool CheckDeleteRights(string role, string function)
        {
            bool result = false;
            string thisRightCheck = "";
            string[] arrRole = role.Split(" ");
            foreach (string right in arrRole)
            {
                if (right.Contains(function))
                {
                    thisRightCheck = right;
                }
            }
            if (thisRightCheck.Contains("15"))
            {
                result = true;
            }
            return result;
        }

        public static decimal FormatPecentageDecimal(decimal value)
        {
            return Math.Round(value * 100, 2);
        }
    }
}
