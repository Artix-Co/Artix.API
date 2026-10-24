# Security operations

## Rotate the JWT signing key

1. Generate a random value with at least 32 bytes.
2. Store it in the deployment secret store as `Authentication__IssuerSigningKey`.
3. Revoke active sessions and tokens.
4. Restart all API instances.
5. Verify that a token signed with the old key is rejected.
6. Remove the old secret from deployment artifacts and logs.

Do not put the value in `appsettings*.json`, source control, or test output.

## Staging checks

1. Run two OTP verify requests at the same time. Accept one request only.
2. Inspect the OTP table and Redis. Find no six digit OTP value.
3. Call `/health/live` without a token. It must return only liveness data.
4. Call `/health/ready` without a token. It must return `401` or `403`.
5. Upload an unknown extension and an invalid magic header. Both must fail.
6. Send a sensitive request from an unknown country. It must return `403`.
7. Confirm that an expired upload session and its temporary folder are removed.

Record the result, environment, build, and test time for each check.
