namespace Survey_Basket.Helper
{
    public static class EmailBodyBuilder
    {
        public static string generateEmailBody(string template, Dictionary<string, string> templateModel)
        {
            var templatePAth = $"{Directory.GetCurrentDirectory()}/templates/{template}.html";
            var streamReader = new StreamReader(templatePAth);
            var body = streamReader.ReadToEnd();
            foreach (var item in templateModel)
                body = body.Replace(item.Key, item.Value);
            return body;

        }
    }
}
