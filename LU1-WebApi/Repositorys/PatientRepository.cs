using Dapper;
using Microsoft.Data.SqlClient;

public class PatientRepository : IPatientRepository
{
    private readonly string _connectionString;

    public PatientRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<Result?> CreatePatient(PatientDTO patient, Guid userId)
    {
        if (userId == Guid.Empty)
            return Result.Failure("No authenticated user found.");

        const string checkPatientQuery = @"
        SELECT COUNT(1) 
        FROM Patient 
        WHERE UserID = @UserID";

        using var connection = new SqlConnection(_connectionString);
        var patientExists = await connection.ExecuteScalarAsync<int>(checkPatientQuery, new { UserID = userId });

        if (patientExists > 0)
        {
            return Result.Failure("This user already has a linked patient. Cannot create a new one.");
        }

        var patientId = Guid.NewGuid();

        const string insertPatientQuery = @"
        INSERT INTO Patient (PatientID, UserID, Naam, Geboortedatum, Behandelplan, Arts, EersteAfspraak, AvatarID)
        VALUES (@PatientID, @UserID, @Naam, @Geboortedatum, @Behandelplan, @Arts, @EersteAfspraak, @AvatarID)";

        await connection.ExecuteAsync(insertPatientQuery, new
        {
            PatientID = patientId,
            UserID = userId,
            Naam = patient.Naam,
            Geboortedatum = patient.Geboortedatum,
            Behandelplan = patient.Behandelplan,
            Arts = patient.Arts,
            EersteAfspraak = patient.EersteAfspraak,
            AvatarID = patient.AvatarID
        });

        return Result.Success("Patient successfully created!");
    }


    //public async Task<PatientDTO?> GetPatient(Guid patientId)
    //{
    //    const string query = @"
    //        SELECT PatientID, UserID, Naam, Geboortedatum, Behandelplan, Arts, EersteAfspraak, AvatarID
    //        FROM Patient
    //        WHERE PatientID = @PatientID";

    //    using var connection = new SqlConnection(_connectionString);
    //    var patient = await connection.QuerySingleOrDefaultAsync<PatientDTO>(query, new { PatientID = patientId });

    //    if (patient == null)
    //        return null;

    //    return patient;
    //}

    public async Task<PatientDTO?> GetPatientFromUserLoggedIn(Guid userId)
    {
        const string query = @"
            SELECT PatientID, UserID, Naam, Geboortedatum, Behandelplan, Arts, EersteAfspraak, AvatarID
            FROM Patient
            WHERE UserID = @UserID";

        using var connection = new SqlConnection(_connectionString);
        var patient = await connection.QuerySingleOrDefaultAsync<PatientDTO>(query, new { UserID = userId });

        if (patient == null)
            return null;

        return patient;
    }

    public async Task<Result> MarkModuleDone(Guid patientId, int moduleId, int stickerId)
    {
        const string checkQuery = @"
    SELECT COUNT(1)
    FROM PatientModuleVoortgang
    WHERE PatientID = @PatientID AND ModuleID = @ModuleID";

        using var connection = new SqlConnection(_connectionString);

        var existingModuleCount = await connection.ExecuteScalarAsync<int>(checkQuery, new
        {
            PatientID = patientId,
            ModuleID = moduleId
        });

        if (existingModuleCount > 0)
        {
            return Result.Failure("Module has already been marked as done for this patient.");
        }

        const string insertQuery = @"
    INSERT INTO PatientModuleVoortgang (PatientID, ModuleID, StickerID)
    VALUES (@PatientID, @ModuleID, @StickerID)";

        var result = await connection.ExecuteAsync(insertQuery, new
        {
            PatientID = patientId,
            ModuleID = moduleId,
            StickerID = stickerId
        });

        if (result > 0)
            return Result.Success("Module marked as done!");
        else
            return Result.Failure("Failed to mark module as done.");
    }



    public async Task<IEnumerable<ModuleVoortgangDTO>> GetModules(Guid patientId)
    {
        const string query = @"
        SELECT ModuleID, StickerID
        FROM PatientModuleVoortgang
        WHERE PatientID = @PatientID";

        using var connection = new SqlConnection(_connectionString);
        var modules = await connection.QueryAsync<ModuleVoortgangDTO>(query, new { PatientID = patientId });

        return modules;
    }
}


