using GDQScraper.DTOs;

namespace GDQScraper.EventData
{
    public interface IData<D> where D:IDTO, new()
    {
        IData<D> Init(D dto);
    }
}
