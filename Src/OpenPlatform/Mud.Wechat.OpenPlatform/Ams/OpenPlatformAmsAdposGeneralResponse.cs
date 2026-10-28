// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「获取小程序广告汇总数据」应答（<c>agency_get_adpos_genenral</c>，官方拼写 <c>genenral</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposGenenral.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAdposGeneralResponse : OpenPlatformAmsResponse
{
    /// <summary>按广告位/日期明细（官方字段名 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<OpenPlatformAmsAdposGeneralItem>? Items { get; set; }

    /// <summary>汇总数据（官方字段名 <c>summary</c>）。</summary>
    [JsonPropertyName("summary")]
    public List<OpenPlatformAmsAdposSummary>? Summary { get; set; }

    /// <summary>list 返回总条数（官方字段名 <c>total_num</c>，number）。</summary>
    [JsonPropertyName("total_num")]
    public long TotalNum { get; set; }
}
