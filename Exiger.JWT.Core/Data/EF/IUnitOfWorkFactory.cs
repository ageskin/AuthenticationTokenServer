namespace Exiger.JWT.Core.Data.EF
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
