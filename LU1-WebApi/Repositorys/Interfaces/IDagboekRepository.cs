public interface IDagboekRepository
{
    Task<Result?> CreateDagboek(DagboekDTO dagboek, Guid userId);
    Task<IEnumerable<DagboekDTO>?> GetDagboekenByUserId(Guid userId);
}
