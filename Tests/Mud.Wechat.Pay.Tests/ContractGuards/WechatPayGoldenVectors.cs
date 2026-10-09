// -------------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -------------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 支付签名黄金向量（**固定常量，勿手改**）—— PAY-B2 / PAY-B3 / PAY-B4 的被测基准。
/// </summary>
/// <remarks>
/// <para>
/// <b>来源</b>：签名串与验签串的**拼接规则**逐字对齐官方 SDK <c>WechatPay2Credential#buildMessage</c> 与
/// <c>WechatPay2Validator#validate</c>（均为「段 + <c>\n</c>」逐段拼接、**结尾带一个换行**、报文体取原文）；
/// 签名值由**本仓库生成的一次性测试密钥对**确定性产出（RSA-SHA256 + PKCS#1 v1.5 无随机性，可稳定复现）。
/// </para>
/// <para>
/// <b>为何用 base64 存消息</b>：签名串以换行结尾，任何以「文本行」方式存储/读取都会把尾部换行吃掉，
/// 导致黄金断言对错误实现**放行**（本仓实测踩过：读回来验签恒 false，差点把错误格式反向固化）。
/// base64 使「逐字节」成为字面语义。
/// </para>
/// <para>
/// <b>字面量形态</b>：含引号的 JSON 原文与 PEM 一律用 **raw string literal**（<c>"""</c>，C# 11+），
/// 零转义 —— 用普通字面量的 <c>\\"</c> / 逐字面量的 <c>""</c> 混用会静默产出错值，且**编译期不报错**。
/// </para>
/// <para>
/// <b>密钥用途</b>：仅供单元测试构造签名/验签，<b>非任何真实商户凭据</b>；
/// 不入日志、不随包分发（PAY-B7 约束的是生产侧私钥，此处为公开测试向量）。
/// </para>
/// </remarks>
internal static class WechatPayGoldenVectors
{
    /// <summary>
    /// 商户 API 证书序列号（写入 Authorization 的 serial_no）。
    /// </summary>
    public const string MerchantSerialNumber = "4946B705E468CEC8";

    /// <summary>
    /// 商户 RSA-2048 私钥（**测试向量专用**，非真实凭据）。
    /// </summary>
    public const string MerchantPrivateKeyPem =
        """
        -----BEGIN RSA PRIVATE KEY-----
        MIIEpAIBAAKCAQEAzr4skUhnrr3acAdf2xANHlDYwmu7IvF3b+vsSYwliKLdmv/E
        ZaBAAxFS8ndprTx0cp0zurNaHAbIpj5qnjBo87ed3gX7BYA8A0n77i30STCJdZS6
        1KLdj374N2cfVwNJKLRDWUXGQWcxTRBTgzmimL0eLZAtC/jeutZihaBg/1fw41FD
        P+IIcUhq5GT/qTCnIj7bKJSHbG7ARnCpXoCteOARpVK0VuXm4poeYQY3Y3tttVGM
        KYDXQpOS8Kh7TcbGVLLBH0fz4VpmppxAJQ/jrosbNIAZn+9HIYGEZc1mNbmRMs48
        TVZHwKAY4tZyk+MEoQVY31Ndbq43/uHhoLnqRQIDAQABAoIBAQCfONWIGnRHns3h
        Y7yOMOF5jJgzu9aHBeKPXuo+hmHNxmjXW8282tsRwcDZHeBUW+/u6WUSj9iBJzRW
        3FEufWgG803w1ThLN0SA3/TXraqqx2fGF4KACuKKBiZSPOhlEhHA+Hu6AEO80SWm
        DUHUGYpBCb15J8WZp9SZnkyIT0kEvSiEuxfXi8hrOraJ/Rh5b9uWtEW4l5gwP09I
        aebXwlprUkfWxlEAB8d3a/b/z0a1KlgWY+GEp/x7LkzbddUsO2dC87hvHzY9imp4
        GxijhwMfNXieJmq8sUkKfjSk+1GSuKg0nB0GO7Mf7Y/47qvxdYXX8s9Pc48+tWcF
        OF8T3W/NAoGBAP62LWl2AzNx3K36K297XBlCCdQeDAGlnFELLSPArR3VsGV6wHpO
        N1liaAbjX0OSnhfGOjGypFk3c8tAIUmb4hCxaNVGP/3UambkOYYUvTaYD8s4Yk49
        nYusI9Ed89zZSB99bffm5rxMDC+IPRTd1Ck5y9PIgu315HrevlkIelETAoGBAM/J
        4fJ0oxbBo07jn3kRdfdLfHHHA1KDuhDaT6m9u8W8vEnRz2GUJKhBMFmUk4dt/tcU
        O9jWA89Y2U8oyYhtFBHMuN/zGmLNCr0a80u9NvGw2cOzJSOqxJMCntg5a4gkxDRl
        hpASqZ9j6vP0/PIVD0V7awv8/4+kbKX392fO2JpHAoGBAKRVj4kKPdh0LugEjv+X
        PH2DUOoNFLv/SJI1PsNhbY/hLbTbdNG0IxvFVM6U5gKn1t4J48PquQRitY+96Xwf
        WcRhYfywHVs8MAYAW2i5erZ9dzsrPqmyMTJLNrSVuZ+UhGbkuw2FaPq1qesE4DvV
        Pyv2AR67sFWtHuNzUKYKxTgHAoGAMynJidKwaaUKWh1wIfZrrpWWsclIByRKR2YJ
        4YjHWHwNfLu9rZA1qnxKtHQCE4HBEyJ3Tf/8diyxbW1kmkQJggx/pR0N7TABqeid
        6ZNQmRhrtmVBCtQ8jNpDVIhm8uHiseghxw3hWE7ZBnLXaaBH31rxU8VbA/p/iipR
        b+0dpncCgYAUdGYFvvSI7LgNpQ3uycIupU7nvq4zG5HLV2ss5z3dnvo5B2bK1KJp
        eF4dvf41giiRGPi6lRXlTOBE5LT1QCz9UmmEQy9NbWK4eVbqWa7sdr65yf3KrSTJ
        msssPrka+yWIaaEctbCSELWn6DLZjvAdrrZO1DE+6hzFHdhGwkbFuA==
        -----END RSA PRIVATE KEY-----
        """;

    /// <summary>
    /// 平台证书序列号（应答/回调头 Wechatpay-Serial）。
    /// </summary>
    public const string PlatformSerialNumber = "00F5140D349A73226C";

    /// <summary>
    /// 平台证书（**验签只用其公钥**）；证书↔公钥绑定由 PAY-B3 断言。
    /// </summary>
    public const string PlatformCertificatePem =
        """
        -----BEGIN CERTIFICATE-----
        MIICyzCCAbOgAwIBAgIJAPUUDTSacyJsMA0GCSqGSIb3DQEBCwUAMCUxIzAhBgNV
        BAMTGk11ZFdlY2hhdFBheVBsYXRmb3JtR29sZGVuMB4XDTI2MDkwOTAxNTc1OVoX
        DTM2MTAwNjAxNTc1OVowJTEjMCEGA1UEAxMaTXVkV2VjaGF0UGF5UGxhdGZvcm1H
        b2xkZW4wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC9YV3XPz5nv/wK
        GYO5bFQN4fxLSbK+ieWlDSj5GJS06Uf/pWvjj9MjK7ImqImUOHYkAnuuy+vShz+w
        AL8n93G/Lz8KqHKlPrJJn2hTLKuozXija/MrMlNtxpbNieaSSkQqryu6JcCkuZED
        QD7btBdyuq4nv0mXVmpvg1+eLJz1uIiS13Cz3tItaHbprmFux5VQBrRlf+sJs4VC
        ZL+Xt7tHRl/ncYDN6BUC9+V6k2uj4DmdrUXw60YWyPFnvxRYvkZgE0/hmLNkXcAn
        fweO170mjdQwiOkRgnOx7vE6vRc1ykVlS7OXK1dS0jC5uPHhqudJXT/cxaD8zmK4
        PhYKJuHdAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAH7ndyo+FKdpZL8Y/TMdlzs+
        IEJHnEfhkkgl5Wk5he1/iBJQPqjMPN9fUE+E+HWzdYNF4thkPUGppxOIVRzjaM4g
        +CYRKmR+0e5W/wVB5bcixEqco1gEuoE/RUy/y8s3hB/ixRASKcMUFgZA/mkcSmYp
        3aj/RBxFTRjV9an/G6eiyoCmqCD+iGmu7zwrl2KgJ1ftVf0Ll9furIdRe5sSBkVo
        uKSW3WarI8tvUuR/g1J2JAk9x6lAa0nE2LydrobU0KfhDCAbmisLQlP9f1QBL7+Y
        tZXeYC/tvsuSWo3Z7l+XZDeAoFXELTN7HkingGLXRuWkRAktzianiqCZxoNNoHc=
        -----END CERTIFICATE-----
        """;

    /// <summary>
    /// 请求签名串入参：大写 HTTP 方法。
    /// </summary>
    public const string RequestMethod = "POST";

    /// <summary>
    /// 请求签名串入参：规范 URL（rawPath + 可选 ?rawQuery）。
    /// </summary>
    public const string RequestCanonicalUrl = "/v3/pay/transactions/jsapi";

    /// <summary>
    /// 请求签名串入参：秒级时间戳。
    /// </summary>
    public const long RequestTimestamp = 1729040000;

    /// <summary>
    /// 请求签名串入参：随机串。
    /// </summary>
    public const string RequestNonce = "MUDWECHATPAYNONCE0123456789ABCDEF";

    /// <summary>
    /// 请求签名串入参：请求体原文。
    /// </summary>
    public const string RequestBody = """{"appid":"wxa9d9651ae0f4c2e1","mchid":"1900000000","description":"TestOrder","out_trade_no":"MUD20261009000001","notify_url":"https://example.com/pay/notify","amount":{"total":100,"currency":"CNY"}}""";

    /// <summary>
    /// **期望**请求签名串（base64(UTF8)，结尾含单个换行）。
    /// </summary>
    public const string RequestMessageB64 = "UE9TVAovdjMvcGF5L3RyYW5zYWN0aW9ucy9qc2FwaQoxNzI5MDQwMDAwCk1VRFdFQ0hBVFBBWU5PTkNFMDEyMzQ1Njc4OUFCQ0RFRgp7ImFwcGlkIjoid3hhOWQ5NjUxYWUwZjRjMmUxIiwibWNoaWQiOiIxOTAwMDAwMDAwIiwiZGVzY3JpcHRpb24iOiJUZXN0T3JkZXIiLCJvdXRfdHJhZGVfbm8iOiJNVUQyMDI2MTAwOTAwMDAwMSIsIm5vdGlmeV91cmwiOiJodHRwczovL2V4YW1wbGUuY29tL3BheS9ub3RpZnkiLCJhbW91bnQiOnsidG90YWwiOjEwMCwiY3VycmVuY3kiOiJDTlkifX0K";

    /// <summary>
    /// **期望** RSA-SHA256 签名（Base64）。
    /// </summary>
    public const string RequestSignature = "ytD1w8Og3ib7I8qQL35Uh4EwTK6ToSFsha10PutKmHiDzQwNkpX3QEYBKWox1dHLHlOxQxm3KRSUMD14L1DIEUo36MsurEpsXwM5S+EZphOEidsLN981pf8Cpih+aVYT/DJG6vJc8oecYD55GQeDes8zNVHhrIKKp71e+KbV7leq6XT1NnDr0E6EPxwjeE6WY4dt/8P4lkiFV9WMjKctvPvsjoMIupvYBpvXS1s/0UZi3t060t0TaivmRP3BiYzDNamW92RvbuGQaBbrOiKDWxt0dRClN5zFmbIzRf8Pmd4pIAn1HppSH/4YJHQ+4WfvjziOFCBf91bbu0dcT1aiYQ==";

    /// <summary>
    /// 验签串入参：Wechatpay-Timestamp 原文。
    /// </summary>
    public const string VerifyTimestamp = "1729040000";

    /// <summary>
    /// 验签串入参：Wechatpay-Nonce 原文。
    /// </summary>
    public const string VerifyNonce = "MUDWECHATPAYNONCE0123456789ABCDEF";

    /// <summary>
    /// 验签串入参：**原始**报文体（不得反序列化后重排）。
    /// </summary>
    public const string VerifyBody = """{"id":"29c3ea6b-6d1e-4a55-9b1f-8c1e0d3f4a71","create_time":"2026-10-09T08:00:00+08:00","event_type":"TRANSACTION.SUCCESS","resource_type":"transaction-success","resource":{"algorithm":"AEAD_AES_256_GCM","ciphertext":"Gi8AnJGFhKQhELFfm2VJ2g==","associated_data":"transaction","nonce":"e0f0f6a1b2c3d4e5f6071829"}}""";

    /// <summary>
    /// **期望**验签串（base64(UTF8)，结尾含单个换行）。
    /// </summary>
    public const string VerifyMessageB64 = "MTcyOTA0MDAwMApNVURXRUNIQVRQQVlOT05DRTAxMjM0NTY3ODlBQkNERUYKeyJpZCI6IjI5YzNlYTZiLTZkMWUtNGE1NS05YjFmLThjMWUwZDNmNGE3MSIsImNyZWF0ZV90aW1lIjoiMjAyNi0xMC0wOVQwODowMDowMCswODowMCIsImV2ZW50X3R5cGUiOiJUUkFOU0FDVElPTi5TVUNDRVNTIiwicmVzb3VyY2VfdHlwZSI6InRyYW5zYWN0aW9uLXN1Y2Nlc3MiLCJyZXNvdXJjZSI6eyJhbGdvcml0aG0iOiJBRUFEX0FFU18yNTZfR0NNIiwiY2lwaGVydGV4dCI6IkdpOEFuSkdGaEtRaEVMRmZtMlZKMmc9PSIsImFzc29jaWF0ZWRfZGF0YSI6InRyYW5zYWN0aW9uIiwibm9uY2UiOiJlMGYwZjZhMWIyYzNkNGU1ZjYwNzE4MjkifX0K";

    /// <summary>
    /// **期望**平台签名（Base64）。
    /// </summary>
    public const string VerifySignature = "F9o2EATWoV66eR1itQtVTcpQXaHebfQ+ImP7gEg39VZKsNmU5jvlE3KEqWmBQ29jQaTnbuDga51pP8uWw1qX5y/AeHbzc+eozhRLxbBR7kssPtd+U9UvjSlGWKvl1OvFoJtz7HZaGZvM2+5gZH3tdQM9MCjATFOrapFBeH9gvTLmemar3BgPjvw7to+htgTzNt5Z0HAKML6Dofwvg3zPXfiCEt1msLWBNi1cU9TspvnGfOjlB9FraobwF4R7aI32EaMsHdsuv4UmrP9/wAL+NK0pO6KGtAJz5dhM+blSzpkZudDyYIBj8sSMYNqo2dZp6vR1lLqi7ouzwhTGjL3HgQ==";

    /// <summary>
    /// AES-256-GCM 黄金向量：32 字节 APIv3 密钥。
    /// </summary>
    public const string AesKeyB64 = "AwoRGB8mLTQ7QklQV15lbHN6gYiPlp2kq7K5wMfO1dw=";

    /// <summary>
    /// AES-256-GCM 黄金向量：12 字节 nonce。
    /// </summary>
    public const string AesNonceB64 = "CxAVGh8kKS4zOD1C";

    /// <summary>
    /// AES-256-GCM 黄金向量：关联数据。
    /// </summary>
    public const string AesAssociatedDataB64 = "dHJhbnNhY3Rpb24=";

    /// <summary>
    /// AES-256-GCM 黄金向量：明文。
    /// </summary>
    public const string AesPlaintextB64 = "eyJvdXRfdHJhZGVfbm8iOiJNVUQyMDI2MTAwOTAwMDAwMSIsInRyYW5zYWN0aW9uX2lkIjoiNDIwMDAwMTU2OTIwMjIwODMwNDcwMTIzNDU2NyJ9";

    /// <summary>
    /// AES-256-GCM 黄金向量：密文。
    /// </summary>
    public const string AesCiphertextB64 = "HUi5UBHImtd7CLfv8m/1NNSBMVbR74xO0ZpmsbLUmyiDzIpZtQA1kCJx+jocA3p5Ys1BtlMOgxaUuLuqkG5/Q0w309jcenPV6WZENo2Gv4jeWYby";

    /// <summary>
    /// AES-256-GCM 黄金向量：16 字节认证标签。
    /// </summary>
    public const string AesTagB64 = "k1biOCL+nuJr0TqIeBrbCA==";

}
