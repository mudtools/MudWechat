// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;

/// <summary>文本消息体（<c>text</c> 分支）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomerMessage")]
public class MpTextMessage
{
    /// <summary>
    /// 文本内容。
    /// </summary>
    /// <remarks>
    /// 支持插入跳小程序的文字链：<c>data-miniprogram-appid</c>（须与该公众号有绑定关系）+
    /// <c>data-miniprogram-path</c>（与 <c>app.json</c> 一致，可带参数）；对不支持该属性的旧客户端
    /// （6.5.16 以下）仍走 <c>href</c> 中的网页链接。
    /// </remarks>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
