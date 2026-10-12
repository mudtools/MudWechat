// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Charge;

/// <summary>获取某个付费能力的最近用量数据应答（<c>POST /wxa/charge/usage/get_recent_average</c>）。</summary>
/// <remarks>官方文档：<c>charge/api_getrecentaverageusage.html</c>；查询某个付费能力最近三个月的平均用量。</remarks>
[HttpJsonSerializable(SerializerClassName = "Charge")]
public class WxaChargeRecentAverageResponse : WxaResponse
{
    /// <summary>最近用量列表（<c>resources</c>），见 <see cref="WxaChargeResourceAverage"/>。</summary>
    [JsonPropertyName("resources")]
    public List<WxaChargeResourceAverage>? Resources { get; set; }
}
