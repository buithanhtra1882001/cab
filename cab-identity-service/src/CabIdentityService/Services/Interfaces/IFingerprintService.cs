namespace WCABNetwork.Cab.IdentityService.Services.Interfaces;

public interface IFingerprintService
{
    public string GenerateFingerprint(int length);
    public string CreateFingerprintHash(string fingerPrint);
    public bool VerifyFingerprintHash(string fingerprint, string fingerprintHash, byte[] key);

}
