// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 城市服务限定页面附加参数（<c>ext_params</c> 数组项）。
/// </summary>
/// <remarks>
/// <para><c>page_type = 5</c>（服务列表页）时必填；关键字查询用 <c>key = "keyword"</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServicePathExtParam
{
    /// <summary>关键字 key 名称；置顶位填 <c>keyword</c>（必填）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>服务列表名称（以城市服务标签名称为准）（必填）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
