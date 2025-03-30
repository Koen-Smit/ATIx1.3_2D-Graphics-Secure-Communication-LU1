public interface IPatientRepository
{
    Task<Result?> CreatePatient(PatientDTO patient, Guid userId);
    //Task<PatientDTO?> GetPatient(Guid patientId);
    Task<PatientDTO?> GetPatientFromUserLoggedIn(Guid userId);
    Task<Result> MarkModuleDone(Guid patientId, int moduleId, int stickerId);
    Task<IEnumerable<ModuleVoortgangDTO>> GetModules(Guid patientId);
}
