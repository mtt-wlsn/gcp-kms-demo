using gcp_kms_demo;
using gcp_kms_demo.Models;
using Google.Cloud.Kms.V1;
using Microsoft.Extensions.Configuration;

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddUserSecrets<Program>()
    .Build();

// Get our configuration settings
var googleConfig = configurationRoot
    .GetSection(nameof(GoogleConfig))
    .Get<GoogleConfig>();

// Build the KMS Client and demo class
KeyManagementServiceClientBuilder builder = new KeyManagementServiceClientBuilder()
{
    JsonCredentials = googleConfig.ServiceAccountJsonKey
};
KeyManagementServiceClient client = await builder.BuildAsync();
var keyManagementDemo = new KeyManagementDemo(client, googleConfig.ProjectId
    , "global", "my-test-key-ring", "my-test-key");

// Create a Key Ring.
await keyManagementDemo.CreateKeyRingAsync();

// Create a Key.
var key = await keyManagementDemo.CreateSymmetricKeyAsync();

var plaintext = "I love pizza!";

// Test out encryption/decryption
var encryptedValue = await keyManagementDemo.EncryptAsync(plaintext);
Console.WriteLine(encryptedValue);
var decryptedValue = await keyManagementDemo.DecryptAsync(encryptedValue);
Console.WriteLine(decryptedValue);