// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 广告单元细分数据列表项（官方 <c>list</c> 数组内层，<c>agency_get_adunit_general</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposDetail.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsAdunitDetail
{
    /// <summary>广告单元 ID（官方字段名 <c>ad_unit_id</c>）。</summary>
    [JsonPropertyName("ad_unit_id")]
    public string? AdUnitId { get; set; }

    /// <summary>广告位名称（官方字段名 <c>ad_unit_name</c>）。</summary>
    [JsonPropertyName("ad_unit_name")]
    public string? AdUnitName { get; set; }

    /// <summary>按日细分统计（官方字段名 <c>stat_item</c>，date 降序）。</summary>
    [JsonPropertyName("stat_item")]
    public List<OpenPlatformAmsStatItem>? StatItem { get; set; }
}
