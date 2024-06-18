namespace TBMF.Core;

public class TbmfDbException : Exception
{
    public TbmfDbException() : base()
    {
    }

    public TbmfDbException(string message) : base(message)
    {
    }

    public TbmfDbException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
