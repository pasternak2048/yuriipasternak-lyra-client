# 🛡️LYRA. Let Yourself Remain Authenticated.
---

## What is LYRA?

**LYRA** is a self-hosted authorization system for verifying **signed requests** between **trusted systems**.  
It ensures that each request across service or company boundaries is intentional, validated, and safe — without inspecting the business payload.

---

## What is LYRA.Client?

**LYRA.Client** is a lightweight SDK for signing outgoing requests and verifying incoming ones via a centralized **LYRA.Server**.

It enables trust-based communication between services using cryptographic signatures and shared policies.

---

## What does LYRA.Client do?

- ✅ Generates `GenericMetadata` for a given request
- ✅ Computes the `payloadHash` (SHA-512 of body)
- ✅ Constructs canonical `StringToSign` and signs it (HMAC, RSA)
- ✅ Returns a ready-to-send `VerifyRequest`, or just `SignedMetadata` for headers

---

## Sample Usage

```csharp
var metadata = new GenericMetadata
{
    CallerSystemName = "gateway@bcorp",
    TargetSystemName = "billing@acorp",
    Action = "POST",
    Resource = "/api/orders",
    PayloadHash = EncryptionHelper.ComputeSha512(payload),
    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
};

var signer = new LyraSigner(touchpoint);
var signed = signer.Sign(metadata);

var verifyRequest = new VerifyRequest
{
    Metadata = metadata,
    Signed = signed,
    RequestId = Guid.NewGuid().ToString()
};
```

---

## Example: Adding to HTTP Headers

```csharp
request.Headers.Add("X-Lyra-Caller", signed.CallerSystemName);
request.Headers.Add("X-Lyra-Timestamp", metadata.Timestamp);
request.Headers.Add("X-Lyra-Payload-Hash", metadata.PayloadHash);
request.Headers.Add("X-Lyra-Signature", signed.Signature);
```

These headers are parsed by the receiver and verified via `LYRA.Server`.

---

## Core Concepts

| Concept             | Description |
|--------------------|-------------|
| `LyraTouchpoint`    | A system identity with secret and algorithm |
| `GenericMetadata`   | Canonical fields that describe the request |
| `SignedMetadata`    | The signature + metadata needed for verification |
| `VerifyRequest`     | Bundles metadata + signature, sent to `LYRA.Server` |
| `SignatureStringBuilder` | Builds the canonical string from metadata |

---

## Highlights

- ✅ Deterministic, platform-agnostic signature creation
- ✅ Supports multiple `Touchpoints` (multi-service apps)
- ✅ Reuses contracts from **[LYRA.Security](https://github.com/pasternak2048/yuriipasternak-lyra-security)**
- ✅ Easily pluggable in any .NET app — Web, API, Worker
- ✅ No third-party dependencies

---

## Tech Stack

- C# 12 / .NET 8
- **[LYRA.Security](https://github.com/pasternak2048/yuriipasternak-lyra-security)**
- System.Security.Cryptography
- System.Text.Json / Http

---

## License

Licensed under the [MIT License](LICENSE).

**🛡️LYRA. Let Yourself Remain Authenticated.**
