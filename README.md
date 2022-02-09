# Google Cloud Platform (GCP) Cloud Key Management Service (KMS) API Demo
This repo holds a demo written in C# showing how to encrypt/decrypt data using the GCP KMS.

## Preconditions
Make sure you have these things before you begin.
- [GCP Account](https://cloud.google.com/free)
- GCP Project
- Have enabled the *Cloud Key Management Service (KMS) API*
- Have added the [appropriate KMS roles](https://cloud.google.com/kms/docs/reference/permissions-and-roles) to your Service Account (or created a new one with these roles).
   - _roles/cloudkms.admin_ and _roles/cloudkms.cryptoOperator_ will you get everything if you are just looking to run this sample in a development environment and don't need to enforce any specific security principles.

## Usage
1. Clone this repository.
2. Add your GCP Project ID and SA JSON Key to the appsettings.json file or user secrets file.
3. Run ".NET Core Launch (console)"

## Additional Resources
- [Cloud Key Management Service documentation](https://cloud.google.com/kms/docs)

## License
[MIT](https://choosealicense.com/licenses/mit/)