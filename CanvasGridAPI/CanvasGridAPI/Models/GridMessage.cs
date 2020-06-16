namespace CanvasGridAPI.Models
{
    public class GridMessage
    {
        public bool OperationStatus { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public object ReturnObject { get; set; } = null;
    }
}
