namespace DotNetNote.Models;

public class SignRepositoryInMemory : ISignRepository
{
    public SignBase AddSign(SignViewModel model)
    {
        // 실제 데이터베이스에 저장하는 코드 들어오는 곳

        return new SignBase { SignId = 1, Email = model.Email, Password = model.Password };
    }

    public SignBase GetSignByEmail(string email)
    {
        return new SignBase { SignId = 1234, Email = email, Name = "홍길동", Username = email };
    }

    public bool IsAuthenticated(SignViewModel model)
    {
        return true; 
    }
}
