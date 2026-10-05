// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 获取微信观看直播凭证响应体（<c>/cgi-bin/living/get_living_code</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class GetLivingCodeResponse : WechatWorkResponse
{
    /// <summary>获取或设置微信观看直播凭证（5 分钟内可以重复使用，且仅能在微信上使用）。</summary>
    [JsonPropertyName("living_code")]
    public string? LivingCode { get; set; }
}
