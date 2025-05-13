using System.Security.Cryptography;
using System.Text;
using System;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Services.Implements;

public class FingerprintService : IFingerprintService
{
    private const string Characters = "!@#$%^&*ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public string GenerateFingerprint(int length)
    {
        Random random = new();
        StringBuilder stringBuilder = new();

        for (int i = 0; i < length; i++)
        {
            int randomIndex = random.Next(Characters.Length);
            char randomChar = Characters[randomIndex];
            stringBuilder.Append(randomChar);
        }

        return stringBuilder.ToString();
    }

    public string CreateFingerprintHash(string fingerPrint)
    {
        using var hmac = new HMACSHA256();
        var key = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(fingerPrint));
        var raw = BitConverter.ToString(hash).Replace("-", "") + '-' + BitConverter.ToString(key).Replace("-", "");
        return raw;
    }

    public bool VerifyFingerprintHash(string fingerprint, string fingerprintHash, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(fingerprint));
        var raw = BitConverter.ToString(computedHash).Replace("-", "");

        return raw.Equals(fingerprintHash);
    }
}
