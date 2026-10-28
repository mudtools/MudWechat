// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>生成 URL Scheme 请求体（<c>POST /wxa/generatescheme</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaSchemeRequest
{
    /// <summary>跳转小程序配置（<c>jump_wxa</c>），见 <see cref="WxaJumpInfo"/>。</summary>
    [JsonPropertyName("jump_wxa")]
    public WxaJumpInfo? JumpWxa { get; set; }

    /// <summary>是否过期（<c>is_expire</c>，选填；<c>false</c> 表示长期有效）。</summary>
    [JsonPropertyName("is_expire")]
    public bool? IsExpire { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）；取值语义以官方页面为准。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，选填）。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效绝对时间（<c>expire_time</c>，选填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}
