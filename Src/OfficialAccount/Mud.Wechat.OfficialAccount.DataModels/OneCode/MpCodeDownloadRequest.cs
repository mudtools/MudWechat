// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

// ---------------------------------------------------------------- 下载二维码包（POST /intp/marketcode/applycodedownload）

/// <summary>
/// 下载二维码包（<c>POST /intp/marketcode/applycodedownload</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>application_id</c> / <c>code_start</c> / <c>code_end</c>（后两者「来自查询二维码申请接口」）。
/// </para>
/// <para>
/// 官方注意事项原文：「<b>下载前需确保申请单状态为 FINISH</b>」——SDK 不编排该前置校验（宿主自行编排）。
/// 官方文档缺陷（照录）：<c>code_start</c>/<c>code_end</c> 未说明取值范围、最大下载数量、是否含边界值。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeDownloadRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>，必填）。</summary>
    [JsonPropertyName("application_id")]
    public long ApplicationId { get; set; }

    /// <summary>获取或设置开始位置（官方 <c>code_start</c>，必填；来自 <c>applycodequery</c> 的码段）。</summary>
    [JsonPropertyName("code_start")]
    public int CodeStart { get; set; }

    /// <summary>获取或设置结束位置（官方 <c>code_end</c>，必填；来自 <c>applycodequery</c> 的码段）。</summary>
    [JsonPropertyName("code_end")]
    public int CodeEnd { get; set; }
}
