namespace DotNetNote.Models;

/// <summary>
/// 리포지토리 인터페이스 
/// </summary>
public interface IOneRepository
{
    One Add(One model);

    List<One> GetAll();
}
