// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>聊天记录条目（<c>recordlist</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpMsgRecord
{
    /// <summary>用户 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>操作码（<c>2002</c> 客服发送信息 / <c>2003</c> 客服接收消息）。</summary>
    [JsonPropertyName("opercode")]
    public int OperCode { get; set; }

    /// <summary>聊天记录文本。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>操作时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>完整客服账号，格式为「账号前缀@公众号微信号」。</summary>
    [JsonPropertyName("worker")]
    public string? Worker { get; set; }
}
