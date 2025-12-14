using PDFtoImage;
using SkiaSharp;

namespace Mira.Core.Services;

public interface IPdfImageService
{
    /// <summary>
    /// Converts PDF pages to PNG images
    /// </summary>
    List<byte[]> ConvertPdfToImages(string filePath);
}

public class PdfImageService : IPdfImageService
{
    private readonly ILoggerService _logger;

    public PdfImageService(ILoggerService logger)
    {
        _logger = logger;
        _logger.LogInfo("PDF image service initialized");
    }

    public List<byte[]> ConvertPdfToImages(string filePath)
    {
        _logger.LogInfo($"Starting PDF to images conversion: {filePath}");
        
        if (!File.Exists(filePath))
        {
            _logger.LogError($"PDF file not found: {filePath}");
            throw new FileNotFoundException($"PDF file not found: {filePath}");
        }

        var images = new List<byte[]>();

        try
        {
            // Read PDF file and convert to base64 for PDFtoImage library
            byte[] pdfBytes = File.ReadAllBytes(filePath);
            string base64Pdf = Convert.ToBase64String(pdfBytes);
            
            _logger.LogDebug($"PDF file read: {pdfBytes.Length} bytes");
            
            // Convert PDF to images using PDFtoImage library
            var pdfImages = Conversion.ToImages(base64Pdf);
            
            int pageNum = 1;
            foreach (var image in pdfImages)
            {
                _logger.LogDebug($"Converting page {pageNum} to PNG");

                using var pngData = image.Encode(SKEncodedImageFormat.Png, 90);
                byte[] imageBytes = pngData.ToArray();
                
                images.Add(imageBytes);
                _logger.LogDebug($"Page {pageNum} converted: {imageBytes.Length} bytes");
                pageNum++;
            }

            _logger.LogInfo($"PDF conversion completed: {images.Count} images created");
            return images;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to convert PDF to images: {filePath}", ex);
            throw new Exception($"Error converting PDF to images: {ex.Message}", ex);
        }
    }
}

