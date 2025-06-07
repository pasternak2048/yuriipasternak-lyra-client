# 🛡️ LYRA.Client — *Sign and Verify Anywhere*

**LYRA.Client** is a lightweight SDK for signing outgoing HTTP requests and verifying incoming ones through a centralized **LYRA.Server**.

It supports two modes of operation:
- ✍️ **Caller mode**: signs outgoing requests and adds verification headers
- 🛡️ **Receiver mode**: extracts headers, builds a `VerifyRequest`, and delegates signature validation to `LYRA.Server`

> ⚡ Built for distributed systems. Ready for microservice trust boundaries.

---

## 🌐 What is LYRA.Client?

- ✍️ **AsCaller** — signs requests with HMAC/RSA and adds `X-Lyra-*` headers
- 🛡️ **AsReceiver** — verifies incoming requests using `LYRA.Server` via middleware
- 🔐 **Powered by LYRA.Security** — reuses the same contracts, enums, and signature tools

---

## ⚙️ Setup

```csharp
services.AddLyraAsCaller(opts =>
{
    opts.Touchpoints = new List<LyraTouchpoint>
    {
        new()
        {
            SystemName = "gateway@bcorp",
            Secret = "topsecret",
            Context = AccessContext.Http,
            SignatureType = SignatureType.HmacSha512
        }
    };
});

services.AddLyraAsReceiver(opts =>
{
    opts.LyraServerHost = "https://lyra.acorp.com";
});
```

---

## 📤 Caller Mode (Outbound Requests)

> Use `ILyraCaller` to sign and send requests

```csharp
var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders")
{
    Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
};

await _lyraCaller.SignRequestAsync(request, payloadJson);
```

Adds headers like:
```
X-Lyra-Caller: gateway@bcorp
X-Lyra-Timestamp: 2025-05-31T12:00:00Z
X-Lyra-Payload-Hash: ...
X-Lyra-Signature: ...
```

---

## 📥 Receiver Mode (Inbound Verification)

> Automatically verifies requests through middleware

```csharp
app.UseLyraVerification(); // Validates all incoming requests using LYRA.Server
```

- Extracts headers from request
- Builds `VerifyRequest`
- Sends to `LYRA.Server /api/verify`
- Aborts request if invalid (403)

---

## 🧠 Core Concepts

| Concept             | Description |
|--------------------|-------------|
| `LyraTouchpoint`    | Represents one identity for signing requests |
| `AccessContext`     | Type of interaction: `Http`, `Event`, `Cache`, etc. |
| `VerifyRequest`     | Built from headers and sent to `LYRA.Server` |
| `SignatureType`     | Algorithm used for signing: HMAC, RSA, etc. |

---

## ✅ Highlights

- ✅ Built-in support for signing and verifying HTTP requests
- ✅ No runtime dependencies beyond .NET + LYRA.Security
- ✅ Centralized verification via LYRA.Server
- ✅ Fully pluggable via DI and middleware
- ✅ Seamlessly integrates with GLORIA and other trusted services

---

## 🔧 Tech Stack

- C# 12 / .NET 8
- LYRA.Security
- System.Net.Http
- Microsoft.Extensions.DependencyInjection
- Designed to run in ASP.NET Core apps and background services

---

## 📄 License

Licensed under the [MIT License](LICENSE).

**LYRA.Client. Sign it. Trust it. Forward it.**
> *"Verification is just a request away."*
