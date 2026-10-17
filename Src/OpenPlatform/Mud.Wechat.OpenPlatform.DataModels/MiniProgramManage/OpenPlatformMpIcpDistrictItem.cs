// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案行政区划项（<c>query_icp_district_code</c> 应答 <c>items[]</c>，树形自递归结构）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/queryIcpDistrictCode.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpDistrictItem
{
    /// <summary>区划层级类型（官方 <c>type</c>，number）。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>区划编码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>区划名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>下级区划列表（官方 <c>sub_list</c>，array，自递归）。</summary>
    [JsonPropertyName("sub_list")]
    public List<OpenPlatformMpIcpDistrictItem>? SubList { get; set; }
}
