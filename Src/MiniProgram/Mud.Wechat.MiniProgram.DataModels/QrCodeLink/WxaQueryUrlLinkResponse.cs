// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>查询 URL Link 应答（<c>url_link_info</c> 或 <c>quota_info</c>）。</summary>
/// <remarks>
/// 官方示例另含 <c>cloud_base.doamin</c>（<b>官方拼写如此</b>）等未列入参数表的字段；
/// 本 DTO <b>不建模示例专属字段</b>（不据示例断言线上字段名），宿主可按需自行读取原始报文。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQueryUrlLinkResponse : WxaResponse
{
    /// <summary>链接配置（<c>url_link_info</c>；<c>query_type = 1</c> 时缺省），见 <see cref="WxaUrlLinkInfo"/>。</summary>
    [JsonPropertyName("url_link_info")]
    public WxaUrlLinkInfo? UrlLinkInfo { get; set; }

    /// <summary>额度配置（<c>quota_info</c>；<c>query_type = 1</c> 时返回），见 <see cref="WxaQuotaInfo"/>。</summary>
    [JsonPropertyName("quota_info")]
    public WxaQuotaInfo? QuotaInfo { get; set; }
}
