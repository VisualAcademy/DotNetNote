namespace DotNetNote.Models
{
    public interface ISignRepository
    {
        /// <summary>
        /// 로그인 되었는지 확인
        /// </summary>
        bool IsAuthenticated(SignViewModel model);

        /// <summary>
        /// 회원 가입
        /// </summary>
        SignBase AddSign(SignViewModel model);

        /// <summary>
        /// 회원 정보
        /// </summary>
        SignBase GetSignByEmail(string email);
    }
}
