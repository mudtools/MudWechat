// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调接收接口（对齐 Mud.Feishu.Webhook 的接收端职能）。
/// </summary>
public interface IWechatCallbackReceiver
{
    /// <summary>
    /// 解析并校验回调（URL 验签参数 + 加密 XML + AES 解密），返回结构化事件。
    /// </summary>
    /// <param name="urlQuery">回调 URL 的查询串（含 msg_signature / timestamp / nonce）。</param>
    /// <param name="body">回调请求体（加密 XML，含 Encrypt 节点）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <exception cref="InvalidOperationException">验签失败或解密失败时抛出。</exception>
    Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// 企业微信回调接收器默认实现：SHA1 验签 → XML 解析 → AES 解密 → 事件字段提取。
/// </summary>
public sealed class WechatCallbackReceiver : IWechatCallbackReceiver
{
    private readonly WechatCallbackOptions _options;

    /// <summary>创建回调接收器。</summary>
    public WechatCallbackReceiver(IOptions<WechatCallbackOptions> options)
    {
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        _options.Validate();
    }

    /// <inheritdoc />
    public Task<WechatCallbackEvent> ReceiveAsync(string urlQuery, string body, CancellationToken cancellationToken = default)
    {
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(urlQuery);
        if (string.IsNullOrEmpty(signature))
        {
            throw new InvalidOperationException("回调验签失败：缺少 msg_signature 参数。");
        }

        var encrypt = ExtractEncrypt(body)
            ?? throw new InvalidOperationException("回调报文非法：未找到 Encrypt 节点。");

        if (!WechatCallbackCrypto.VerifySignature(_options.PushToken, timestamp ?? string.Empty, nonce ?? string.Empty, encrypt, signature))
        {
            throw new InvalidOperationException("回调验签失败：msg_signature 不匹配（请检查 PushToken / CorpId 配置）。");
        }

        var decrypted = WechatCallbackCrypto.Decrypt(_options.PushEncodingAESKey, encrypt);
        return Task.FromResult(ParseEvent(decrypted, timestamp, nonce));
    }

    private static string? ExtractEncrypt(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            var doc = XDocument.Parse(body);
            return doc.Root?.Element("Encrypt")?.Value;
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    private static WechatCallbackEvent ParseEvent(string decryptedXml, string? timestamp, string? nonce)
    {
        var evt = new WechatCallbackEvent
        {
            TimeStamp = timestamp,
            Nonce = nonce,
            DecryptedXml = decryptedXml,
        };

        try
        {
            var doc = XDocument.Parse(decryptedXml);
            var root = doc.Root;
            if (root != null)
            {
                evt.InfoType = root.Element("InfoType")?.Value;
                evt.SuiteId = root.Element("SuiteId")?.Value;
                evt.SuiteTicket = root.Element("SuiteTicket")?.Value;
                evt.AuthCode = root.Element("AuthCode")?.Value;

                // R11：create_auth / reset_permanent_code 报文体不含 AuthCorpId，
                // 禁止用 FromUserName 兜底伪造授权企业（该文的授权企业须由 auth_code 换码后反查）。
                evt.AuthCorpId = root.Element("AuthCorpId")?.Value;
                if (string.IsNullOrEmpty(evt.AuthCorpId) && !evt.IsAuthCodeEvent)
                {
                    evt.AuthCorpId = root.Element("FromUserName")?.Value;
                }
            }
        }
        catch (System.Xml.XmlException)
        {
            // 明文非 XML（协议外报文）时保留原文，事件字段为空。
        }

        return evt;
    }
}
