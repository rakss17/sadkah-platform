using System.Globalization;
using System.Text.RegularExpressions;

namespace Sadkah.API.Helpers
{
    public static partial class ReceiptParser
    {
        [GeneratedRegex(@"(?:₱|P(?:hp)?|PHP)?\s*(\d{1,3}(?:,\d{3})*\.\d{2})", RegexOptions.IgnoreCase)]
        private static partial Regex MoneyRegex();

        [GeneratedRegex(@"^[A-Za-z0-9\-]{5,30}$")]
        private static partial Regex ReferenceTokenRegex();

        private static readonly string[] AmountPriorityKeywords =
        [
            "amount sent", "you sent", "total amount", "amount", "transferred", "sent", "paid", "total"
        ];

        private static readonly string[] AmountExcludeKeywords =
        [
            "fee", "charge", "surcharge"
        ];

        private static readonly string[] ReferencePriorityKeywords =
        [
            "reference no", "reference number", "ref no", "reference",
            "transaction id", "transaction no", "transaction number", "txn id", "txn no",
            "trace id", "trace no",
            "confirmation no", "confirmation number", "approval code"
        ];

        public static decimal? ExtractAmount(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var lines = text.Split('\n');
            var candidates = new List<(decimal Value, string Context)>();

            for (var i = 0; i < lines.Length; i++)
            {
                foreach (Match match in MoneyRegex().Matches(lines[i]))
                {
                    if (!decimal.TryParse(match.Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                        continue;

                    candidates.Add((value, BuildContext(lines, i)));
                }
            }

            if (candidates.Count == 0) return null;

            var filtered = candidates.Where(c => !AmountExcludeKeywords.Any(c.Context.Contains)).ToList();
            if (filtered.Count == 0) filtered = candidates;

            foreach (var keyword in AmountPriorityKeywords)
            {
                var match = filtered.FirstOrDefault(c => c.Context.Contains(keyword));
                if (match.Context != null) return match.Value;
            }

            return filtered
                .GroupBy(c => c.Value)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .First().Key;
        }

        public static string? ExtractReferenceNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var lines = text.Split('\n').Select(l => l.Trim()).ToArray();
            var candidates = new List<(string Token, string Context)>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!ReferenceTokenRegex().IsMatch(line) || !line.Any(char.IsDigit))
                    continue;

                candidates.Add((line, BuildContext(lines, i)));
            }

            if (candidates.Count == 0) return null;

            foreach (var keyword in ReferencePriorityKeywords)
            {
                var match = candidates.FirstOrDefault(c => c.Context.Contains(keyword));
                if (match.Context != null) return match.Token;
            }

            return candidates
                .OrderByDescending(c => c.Token.Any(char.IsLetter) && c.Token.Any(char.IsDigit))
                .ThenByDescending(c => c.Token.Length)
                .First().Token;
        }

        private static string BuildContext(string[] lines, int index)
        {
            var contextLines = new List<string> { lines[index] };
            for (var back = index - 1; back >= 0 && contextLines.Count < 3; back--)
            {
                if (!string.IsNullOrWhiteSpace(lines[back]))
                    contextLines.Add(lines[back]);
            }

            return string.Join(" ", contextLines).ToLowerInvariant();
        }
    }
}
