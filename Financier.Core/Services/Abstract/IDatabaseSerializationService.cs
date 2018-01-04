namespace Financier.Services
{
    public interface IDatabaseSerializationService
    {
        void Load(string path);
        void Save(string path);
    }
}
