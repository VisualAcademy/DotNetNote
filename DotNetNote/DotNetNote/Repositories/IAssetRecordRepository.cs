using DotNetNote.Models;

namespace DotNetNote.Repositories;

public interface IAssetRecordRepository
{
    IEnumerable<AssetRecord> GetAll();
    AssetRecord? GetById(int id);
}