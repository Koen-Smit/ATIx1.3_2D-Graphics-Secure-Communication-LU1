using System;

public class Patient
{
    public Guid PatientID { get; set; } = Guid.NewGuid();
    public string UserID { get; set; } = string.Empty;
    public string Naam { get; set; } = string.Empty;
    public DateTime Geboortedatum { get; set; }
    public string? Behandelplan { get; set; }
    public string? Arts { get; set; }
    public DateTime? EersteAfspraak { get; set; }
    public int? AvatarID { get; set; }
}

public class PatientDTO
{
    public Guid PatientID { get; set; }
    public string Naam { get; set; } = string.Empty;
    public DateTime Geboortedatum { get; set; }
    public string? Behandelplan { get; set; }
    public string? Arts { get; set; }
    public DateTime? EersteAfspraak { get; set; }
    public int? AvatarID { get; set; }
}


public class ModuleVoortgangDTO
{
    public int ModuleID { get; set; }
    public int StickerID { get; set; }
}
