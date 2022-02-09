using System.Text;
using System.Net;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace gcp_kms_demo;

public class KeyManagementDemo
{
    private KeyManagementServiceClient _client;
    private string _projectId;
    private string _locationId;
    private string _keyRingId;
    private string _keyId;

    public KeyManagementDemo(
        KeyManagementServiceClient client
        , string projectId
        , string locationId
        , string keyRingId
        , string keyId)
    {
        _client = client;
        _projectId = projectId;
        _locationId = locationId;
        _keyRingId = keyRingId;
        _keyId = keyId;
    }

    public async Task<KeyRing> CreateKeyRingAsync()
    {
        LocationName locationName = new LocationName(_projectId, _locationId);
        KeyRingName keyRingName = new KeyRingName(_projectId, _locationId, _keyRingId);

        try
        {
            return await _client.CreateKeyRingAsync(locationName, _keyRingId, new KeyRing());
        }
        catch(Grpc.Core.RpcException ex)
        {
            if(ex.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
            {
                return await _client.GetKeyRingAsync(keyRingName);
            }

            throw;
        }
    }

    public async Task<CryptoKey> CreateSymmetricKeyAsync()
    {
        KeyRingName keyRingName = new KeyRingName(_projectId, _locationId, _keyRingId);
        CryptoKeyName cryptoKeyName = new CryptoKeyName(_projectId, _locationId, _keyRingId, _keyId);

        // Create a new symmetric key for encrypting/decriptying that will
        // rotate once a year starting in 24 hours.
        CryptoKey key = new CryptoKey
        {
            Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt,
            VersionTemplate = new CryptoKeyVersionTemplate
            {
                Algorithm = CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.GoogleSymmetricEncryption
            },
            RotationPeriod = new Duration
            {
                Seconds = 60 * 60 * 24 * 365
            },
            NextRotationTime = new Timestamp
            {
                Seconds = new DateTimeOffset(DateTime.UtcNow.AddHours(24)).ToUnixTimeSeconds()
            }
        };

        try
        {
            return await _client.CreateCryptoKeyAsync(keyRingName, _keyId, key);
        }
        catch(Grpc.Core.RpcException ex)
        {
            if(ex.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
            {
                return await _client.GetCryptoKeyAsync(cryptoKeyName);
            }

            throw;
        }
    }

    public async Task<string> EncryptAsync(string plaintext)
    {
        CryptoKeyName cryptoKeyName = new CryptoKeyName(_projectId, _locationId, _keyRingId, _keyId);

        ByteString plaintextAsByteString = ByteString.CopyFrom(Encoding.UTF8.GetBytes(plaintext));

        EncryptResponse encryptResponse = await _client.EncryptAsync(cryptoKeyName, plaintextAsByteString);

        var ciphertext = Convert.ToBase64String(encryptResponse.Ciphertext.ToByteArray());

        return ciphertext;
    }

    public async Task<string> DecryptAsync(string ciphertext)
    {
        CryptoKeyName cryptoKeyName = new CryptoKeyName(_projectId, _locationId, _keyRingId, _keyId);

        ByteString ciphertextAsByteString = ByteString.CopyFrom(Convert.FromBase64String(ciphertext));

        DecryptResponse decryptResponse = await _client.DecryptAsync(cryptoKeyName, ciphertextAsByteString);

        string plaintext = Encoding.UTF8.GetString(decryptResponse.Plaintext.ToByteArray());

        return plaintext;
    }

    public async void RotateKeyAsync()
    {

    }

    public async void DisableKeyAsync()
    {

    }
}
