using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

class PdfRenamer
{
    static void Main(string[] args)
    {
        // Aceita o caminho do PDF como argumento ou pede ao usuário
        string pdfPath;

        if (args.Length > 0)
        {
            pdfPath = args[0];
        }
        else
        {
            Console.Write("Informe o caminho do arquivo PDF: ");
            pdfPath = Console.ReadLine()?.Trim().Trim('"') ?? "";
        }

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine($"Arquivo não encontrado: {pdfPath}");
            return;
        }

        try
        {
            string companyName = ExtractCompanyName(pdfPath);
            DateTime? dateTimeCreateDoc = ExtractEmissaoDateTime(pdfPath);

            var newFileName = dateTimeCreateDoc?.ToString("yyyyMMddHHmm") + " " + companyName;

            if (string.IsNullOrWhiteSpace(companyName))
            {
                Console.WriteLine("Não foi possível extrair o nome da empresa da primeira linha.");
                return;
            }

            Console.WriteLine($"Nome extraído: {companyName}");

            string newPath = BuildNewPath(pdfPath, newFileName);
            RenameFile(pdfPath, newPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar o arquivo: {ex.Message}");
        }
    }

    /// <summary>
    /// Extrai a data e hora do campo "Emissão" de um PDF.
    /// Formato esperado na linha: "Número: XXXXX Série: XXX Emissão: DD/MM/YYYY HH:mm:ss - Via Consumidor"
    /// </summary>
    /// <param name="pdfPath">Caminho para o arquivo PDF</param>
    /// <returns>DateTime com a data e hora de emissão, ou null se não encontrado</returns>
    public static DateTime? ExtractEmissaoDateTime(string pdfPath)
    {
        using var document = PdfDocument.Open(pdfPath);

        foreach (var page in document.GetPages())
        {
            var text = string.Join(" ", page.GetWords().Select(w => w.Text));

            var result = ParseEmissao(text);
            if (result.HasValue)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Sobrecarga que recebe o stream do PDF em vez do caminho.
    /// </summary>
    public static DateTime? ExtractEmissaoDateTime(byte[] pdfBytes)
    {
        using var document = PdfDocument.Open(pdfBytes);

        foreach (var page in document.GetPages())
        {
            var text = string.Join(" ", page.GetWords().Select(w => w.Text));

            var result = ParseEmissao(text);
            if (result.HasValue)
                return result;
        }

        return null;
    }

    private static DateTime? ParseEmissao(string text)
    {
        // Regex captura: Emissão: DD/MM/YYYY HH:mm:ss
        var regex = new Regex(
            @"Emiss[aã]o:\s*(\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2}:\d{2})",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(text);

        if (!match.Success)
            return null;

        var dateTimeString = match.Groups[1].Value.Trim();

        if (DateTime.TryParseExact(
                dateTimeString,
                "dd/MM/yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Abre o PDF e retorna o texto da primeira linha não-vazia da primeira página.
    /// </summary>
    static string ExtractCompanyName(string pdfPath)
    {
        using var document = PdfDocument.Open(pdfPath);

        // Extrai palavras da primeira página agrupadas por posição vertical (Y)
        // para reconstruir linhas de texto de forma confiável
        Page firstPage = document.GetPage(1);

        var wordsByLine = firstPage.GetWords()
            .GroupBy(w => Math.Round(w.BoundingBox.Bottom, 1)) // agrupa por linha (posição Y)
            .OrderByDescending(g => g.Key)                      // PDF: Y cresce de baixo pra cima
            .Select(g => string.Join(" ", g.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text)));

        foreach (var line in wordsByLine)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
                return trimmed;
        }

        // Fallback: extrai texto puro da página inteira se nenhuma palavra for encontrada
        string rawText = firstPage.Text;
        string firstLine = rawText
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));

        return firstLine ?? "";
    }

    /// <summary>
    /// Constrói o novo caminho do arquivo usando o nome da empresa sanitizado.
    /// Evita sobrescrever arquivos existentes adicionando um sufixo numérico.
    /// </summary>
    static string BuildNewPath(string originalPath, string companyName)
    {
        string directory = Path.GetDirectoryName(originalPath) ?? ".";
        string safeName  = SanitizeFileName(companyName);
        string newPath   = Path.Combine(directory, safeName + ".pdf");

        // Se já existir um arquivo com esse nome, adiciona sufixo (2), (3)...
        int counter = 2;
        while (File.Exists(newPath) && !string.Equals(newPath, originalPath, StringComparison.OrdinalIgnoreCase))
        {
            newPath = Path.Combine(directory, $"{safeName} ({counter}).pdf");
            counter++;
        }

        return newPath;
    }

    /// <summary>
    /// Remove caracteres inválidos para nomes de arquivo e limita o tamanho.
    /// </summary>
    static string SanitizeFileName(string name)
    {
        // Remove caracteres inválidos no Windows/Linux/macOS
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string safe = string.Concat(name.Select(c => invalidChars.Contains(c) ? '_' : c));

        // Remove espaços múltiplos e caracteres de controle
        safe = Regex.Replace(safe, @"\s+", " ").Trim();

        // Limita a 150 caracteres para evitar paths muito longos
        if (safe.Length > 150)
            safe = safe.Substring(0, 150).TrimEnd();

        return safe;
    }

    /// <summary>
    /// Renomeia o arquivo e exibe o resultado.
    /// </summary>
    static void RenameFile(string oldPath, string newPath)
    {
        if (string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("O arquivo já possui o nome correto, nenhuma alteração necessária.");
            return;
        }

        File.Move(oldPath, newPath);
        Console.WriteLine($"Arquivo renomeado com sucesso!");
        Console.WriteLine($"  De: {oldPath}");
        Console.WriteLine($"  Para: {newPath}");
    }
}
