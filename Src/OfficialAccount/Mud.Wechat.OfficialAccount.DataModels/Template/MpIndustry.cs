// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>行业信息（官方 <c>first_class</c>/<c>second_class</c> 两字段结构）。</summary>
/// <remarks>
/// <b>命名差异（勿互相「对齐」）</b>：本页副营行业键为 <c>secondary_industry</c>，
/// 而模板列表页的二级行业字段名为 <c>deputy_industry</c>——同域两处「二级」命名不一致（官方原文如此）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpIndustry
{
    /// <summary>获取或设置一级类目（官方 <c>first_class</c>）。</summary>
    [JsonPropertyName("first_class")]
    public string? FirstClass { get; set; }

    /// <summary>获取或设置二级类目（官方 <c>second_class</c>）。</summary>
    [JsonPropertyName("second_class")]
    public string? SecondClass { get; set; }
}
