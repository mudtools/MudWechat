# Mud.Wechat.Pay.Abstractions

微信支付 **APIv3** SDK **凭据与密码学基座包**：商户配置面、请求签名/应答与回调验签端口、`AEAD_AES_256_GCM` 加解密、时间戳窗口闸、平台证书存储端口、多商户上下文、命名 HTTP 客户端与签名 Handler、`AddPayApp` 注册入口。

> **本包是企业微信线之外的另一条产品线**：凭据是**商户 API 证书 RSA 私钥对请求签名**，**没有 `access_token`**、**不声明任何 `[Token]`**（守卫 `PAY-B1` fail-closed），也没有令牌管理器/刷新阈值这类概念；请求恒发往 `https://api.mch.weixin.qq.com`。企业微信的 `AddPayApi()`（企业支付）/ `AddPayToolApi()`（收银台）走企微 `access_token`，与本包无关。

## 内容

- **商户配置面**（`Configuration/WechatPayMerchantConfig.cs`）：`MchId` / `SerialNumber`（商户 API 证书序列号）/ `PrivateKeySecretName` / `ApiKeySecretName` / `SpMchId` / `SubMchId`。派生面：`IsServicePartner`（`SpMchId` + `SubMchId` 齐备即服务商模式）、`MerchantKey`（服务商为 `{SpMchId}:{SubMchId}`，否则 `MchId`，即多商户路由键）、`AuthorizationMchId`（签名头里的 `mchid`）。`Validate()` 在启动期校验互斥必填，并**拒绝任何密钥材料形状**（`-----BEGIN` / PEM / `MII…` Base64）——配置里只允许出现密钥**名称**。`ToString()` 输出掩码。本类**刻意不继承** `WechatAppConfigBase`（后者的 `BaseUrl` / `AppKey` / `TokenRefreshThreshold` 在支付线无对应概念）。
- **签名串构造与常量**（`Credential/WechatPaySignatureMessages.cs`）：`AuthorizationSchema = "WECHATPAY2-SHA256-RSA2048"`、`SignatureAlgorithm = "SHA256-RSA2048"`、`DefaultTimestampSkewSeconds = 300`、`NonceLength = 32`、`MiniProgramPaySignType = "RSA"`；`BuildCanonicalUrl` / `BuildRequestMessage`（`方法\nURL\ntimestamp\nnonce\nbody\n`）/ `BuildVerifyMessage`（`timestamp\nnonce\nbody\n`）/ `BuildMiniProgramPaySignMessage` / `BuildPackageValue`（`prepay_id=…`）/ `BuildAuthorization` / `CreateNonce`。**逐字节对齐官方**是 `PAY-B2` 黄金向量的锁定项——任一字节漂移都会让线上验签 100% 失败且只在真实交易时暴露。
- **签名/验签端口与实现**（`Credential/`）：`IWechatPaySignatureProvider`（`Sign(message)` → `WechatPaySignatureResult`（只带**签名值 + 序列号**两个可公开字段）、`Verify(serialNumber, message, signature)`）与按商户缓存的 `IWechatPaySignatureProviderFactory`；算法为 RSA-SHA256 + `RSASignaturePadding.Pkcs1`，BCL 直调。
- **时间戳窗口闸**（`Credential/WechatPayTimestampGate.cs`）：返回 `Verdict`（`Valid` / `Missing` / `NotNumeric` / `OutOfWindow`）而非异常——结论枚举天然不含报文内容（`PAY-B7`）；缺失/非数字/超窗一律拒（`PAY-B3`）。
- **GCM 加解密**（`Credential/WechatPayAesGcmCodec.cs`）：`AEAD_AES_256_GCM`，`KeySizeBytes = 32` / `NonceSizeBytes = 12` / `TagSizeBytes = 16`（tag 附在 Base64 密文尾部）。`TryDecrypt` / `TryDecryptOfficialPayload` **fail-closed 返回 false 而不抛出**——异常消息会夹带密文字节；`WechatPayGcmPayload` 为 `readonly struct`（Nonce/Ciphertext/Tag）。
- **凭据取用端口**（`Credential/IWechatPayMerchantCredentialProvider.cs`）：`LoadPrivateKeyAsync(config)` 返回**调用方自有**的 `RSA` 实例（用完须 `Dispose`），`LoadApiKeyAsync(config)` 返回 32 字节 APIv3 密钥；两者一律经组件端口 `ISecretProvider` 按**名称**取用。
- **多商户管理与上下文**（`Credential/WechatPayMerchant*`）：`IWechatPayMerchantManager`（注册表单一来源，未登记键 fail-fast 并给出已注册键清单）、`IWechatPayMerchantContext`（`AsyncLocal` 环境商户上下文：`ResolveCurrent()` / `UseMerchant(merchantKey)` 一次性 `using` 作用域；单商户即默认商户，多商户未开作用域**直接抛**，绝不静默挑第一个）。
- **平台证书端口**（`Credential/`）：读端口 `IWechatPayPlatformCertificateStore`（`TryGetCertificate(serial, out cert)`）、写端口 `IWechatPayPlatformCertificateWriter`、刷新端口 `IWechatPayPlatformCertificateRefresher`（实现随证书域在主包）；默认进程内实现 `WechatPayPlatformCertificateCache` 同时实现读写两端口并**指向同一实例**（否则刷新写 A、验签读 B 会静默不一致）。
- **传输层**（`Transport/`）：`WechatPayHttpClientNames`（`ClientName = "wechat-pay"`、`BaseAddress = "https://api.mch.weixin.qq.com"`、`TypeName` 供 `[HttpClientApi(HttpClient = …)]` 引用）、标记接口 `IWechatPayHttpClient : IEnhancedHttpClient`、`WechatPayAuthorizationHandler`（DelegatingHandler：取当前商户 → 构造签名串 → 只添加一个 **`Authorization`** 头，其值内含 `mchid` / `nonce_str` / `timestamp` / `serial_no` / `signature`；请求已带 `Authorization` 时**幂等跳过**，签名失败一律上抛不静默放行）。
- **注册入口**（`Extensions/PayAppExtensions.cs`）：`AddPayApp` 三重载（`IConfiguration + sectionName`（默认节名 `WechatPayMerchants`）/ `Action<WechatPayMerchantConfig>` / `List<WechatPayMerchantConfig>`），装配上述全部单例并建命名客户端；配置绑定走源生成器。
- **错误码与异常**（`Enums/WechatPayErrorCodes.cs`、`Exceptions/WechatPayException.cs`）：官方 `code` 字符串常量表（`SYSTEM_ERROR` / `SIGN_ERROR` / `PARAM_ERROR` / `NO_AUTH` / `FREQUENCY_LIMITED` / `ORDER_NOT_EXIST` / `OUT_TRADE_NO_USED` …），`WechatPayException : WechatApiException` 带 `PayErrorCode`，`NumericFailureSentinel = -1` 表示无官方码。

## 用法

### 注册凭据底座

```csharp
// 密钥端口由宿主实现（SDK 不抢占注册：密钥治理权在宿主）
builder.Services.AddSingleton<ISecretProvider>(new MyVaultSecretProvider());
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants");
```

```json
{
  "WechatPayMerchants": [
    {
      "MchId": "1900000001",
      "SerialNumber": "5157F09EFDC096DE15EBE01A57495577",
      "PrivateKeySecretName": "wechat-pay/apiv3/private-key/1900000001",
      "ApiKeySecretName": "wechat-pay/apiv3-key/1900000001"
    },
    {
      "SpMchId": "1900000100",
      "SubMchId": "1900000200",
      "SerialNumber": "…",
      "PrivateKeySecretName": "wechat-pay/apiv3/private-key/1900000100",
      "ApiKeySecretName": "wechat-pay/apiv3-key/1900000100"
    }
  ]
}
```

值一律是**密钥名称**（经 `ISecretProvider` 解析），不是密钥内容。商户管理器在**注册期**立刻构造一次，把「配置非法 / 重复商户键 / 空集合」钉在启动阶段。

### 只用底座、不起业务接口

回调包验签与解密**只依赖本包**——纯回调宿主调 `AddPayApp` 即可，命名客户端是惰性的，不发支付请求就不会真的建连：

```csharp
builder.Services.AddPayApp(configuration, "WechatPayMerchants")
                .AddWechatPayCallback(configuration);
```

### 直接复用密码学原语（自建传输或离线校验时）

```csharp
long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
string nonce = WechatPaySignatureMessages.CreateNonce();          // 32 位随机串

// 请求签名串（逐字节即官方规范串：方法\nURL\ntimestamp\nnonce\nbody\n）
string message = WechatPaySignatureMessages.BuildRequestMessage(
    "POST", "/v3/pay/transactions/jsapi", timestamp, nonce, body);

// 官方回调 resource 解密：失败返回 false，绝不抛出、绝不回显密文/密钥
bool ok = WechatPayAesGcmCodec.TryDecryptOfficialPayload(
    apiKey32Bytes, resource.Nonce, resource.Ciphertext, resource.AssociatedData, out string? plaintext);
```

## 依赖

- `Mud.Wechat.Abstractions`（跨产品线共享叶层：SSRF 主机登记窄入口、回调异常与抗重放端口、响应判错契约）
- `Mud.Wechat.Pay.DataModels`
- `Mud.HttpUtils` 3.0.3 + `Mud.HttpUtils.Generator` 3.0.3（分析器，`PrivateAssets=all`）
- `Microsoft.Extensions.Http` / `Microsoft.Extensions.Hosting.Abstractions`：net8.0/net10.0 为 10.0.9，net6.0 为 8.0.1
- `InternalsVisibleTo`：`Mud.Wechat.Pay`、`Mud.Wechat.Pay.Callback`、`Mud.Wechat.Pay.Tests`、`Mud.Wechat.Pay.Callback.Tests`、`DynamicProxyGenAssembly2`

## 说明

- **目标框架是记录在案的受控例外**：`net6.0;net8.0;net10.0`，**无 `netstandard2.0`**（本仓红线要求四档）。原因：APIv3 强制 `AEAD_AES_256_GCM`，而 `System.Security.Cryptography.AesGcm` 在 ns2.0 不存在；手写 GCM 与引入 BouncyCastle 等第三方密码学包两项备选均被否。自 net6.0 起 `AesGcm` / `RSA.ImportFromPem` / `RSA.SignData` / `RSA.Decrypt` 原生可用 ⇒ 三档全走 BCL、AOT 安全。守卫 `PAY-B9` / `AB-G8` 锁定该例外不被静默放宽。
- `csproj` 抑制的是 `SYSLIB1100` / `SYSLIB1101`（源生成器相关）；`MUD005` 抑制只随主包，本包无 `[Token]` 声明。
- **不注册默认 `ISecretProvider`**（组件端口，属宿主密钥治理）；宿主漏装时首次解析凭据取用器给出**点名错误**（说明是谁要求注册的），而非难懂的 DI 缺失异常。
- **SSRF 白名单零改动**（`PAY-B8` / `AB-G9`）：`AddPayApp` 经公用层窄入口 `AddWechatApiHosts()` 登记——支付线不走 `[Token]` 因而不会触发 `AddWechatTokenRecovery`，纯支付宿主若不登记，每一笔请求都会被组件的连接期 SSRF 严格模式拦下（实测缺陷）。
- 多实例部署时，`IWechatPayPlatformCertificateStore` 的进程内默认实现可由宿主以 `TryAdd` 前置注册替换为分布式实现；`IWechatPayMerchantManager` 同理（如需从配置中心动态增删商户）。
- 本包端口面（`IWechatPay*`）即支付线全部公开契约面；业务接口、下载通道与证书刷新实现落在主包 `Mud.Wechat.Pay`。
