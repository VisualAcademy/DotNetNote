using DotNetNote.Models;

namespace DotNetNote.Repositories;

public class AssetRecordRepository : IAssetRecordRepository
{
    private static readonly List<AssetRecord> _items =
    [
        new AssetRecord { Id = 1, Name = "Sample Asset", Category = "Machine", Status = "Active" }
    ];

    public IEnumerable<AssetRecord> GetAll() => _items;

    public AssetRecord? GetById(int id) =>
        _items.FirstOrDefault(x => x.Id == id);
}