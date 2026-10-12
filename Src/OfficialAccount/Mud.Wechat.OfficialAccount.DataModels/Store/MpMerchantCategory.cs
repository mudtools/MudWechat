// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 门店小程序类目（官方 <c>data.all_category_info.categories[]</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表仅列 <c>id</c> / <c>level</c> / <c>sensitive_type</c>；官方返回示例另出现
/// <c>name</c> / <c>children</c> / <c>father</c> / <c>qualify</c> / <c>scene</c>。
/// 按「字段存在性冲突取并集」：纳入示例中<b>形态明确</b>的 <c>name</c>（类目名）与
/// <c>children</c>（同类目自引用数组，用于树形展开）；<c>father</c>（父类目，可由 <c>children</c>
/// 递归覆盖，冗余不建模）、<c>qualify</c>（子字段 <c>exter_list</c>/<c>inner_list</c> 元素形态未定义）、
/// <c>scene</c>（形态未定义）<b>不建模</b>（照录官方缺陷）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：字段说明称 <c>level</c>「一级或者二级类目」，但示例根节点出现
/// <c>id:0, name:"root", level:0</c>（第三级层级描述）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantCategory
{
    /// <summary>获取或设置类目 id（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>获取或设置类目级别（官方 <c>level</c>；官方说明「一级或者二级类目」，示例另现 0，矛盾照录）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置敏感类型（官方 <c>sensitive_type</c>；<c>0</c> 不用特殊处理 / <c>1</c> 创建该类目的门店小程序时需添加相关证件）。</summary>
    [JsonPropertyName("sensitive_type")]
    public int? SensitiveType { get; set; }

    /// <summary>获取或设置类目名称（官方返回示例字段 <c>name</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置子类目（官方返回示例字段 <c>children</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("children")]
    public List<MpMerchantCategory>? Children { get; set; }
}
