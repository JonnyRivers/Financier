namespace Financier.Services
{
    public interface IDatabaseCreationService
    {
        void Create();
        void Obliterate();
        void Populate();
    }
}
